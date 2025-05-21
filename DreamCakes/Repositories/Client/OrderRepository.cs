using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;

namespace DreamCakes.Repositories.Client
{
    public class OrderRepository
    { 
        private readonly DreamCakesEntities _context;
        private readonly string _connectionString;

        public OrderRepository()
        {
            _context = new DreamCakesEntities();
            var entityConnectionString = ConfigurationManager.ConnectionStrings["DreamCakesEntities"].ConnectionString;
            var entityBuilder = new EntityConnectionStringBuilder(entityConnectionString);
            _connectionString = entityBuilder.ProviderConnectionString;
        }
    
    
       

        public ProductDto GetProductById(int productId)
        {
            try
            {
                var product = _context.PRODUCTOes
                    .Include(p => p.CATEGORIA)
                    .Include(p => p.IMAGENs)
                    .FirstOrDefault(p => p.ID_Producto == productId);

                if (product == null)
                {
                    throw new Exception($"Producto con ID {productId} no encontrado");
                }

                var images = product.IMAGENs.Select(i => new ImageDto
                {
                    ID_Image = i.ID_Imagen,
                    ID_Product = i.ID_Producto,
                    ImgName = i.Nombre_Img,
                    ImgUrl = i.Imagen_URL,
                    Response = 1
                }).ToList();

                return new ProductDto
                {
                    ID_Product = product.ID_Producto,
                    Name = product.Nombre,
                    Description = product.Descripcion,
                    Price = product.Precio,
                    Stock = product.Stock,
                    Images = images,
                    MainImageUrl = images.FirstOrDefault()?.ImgUrl,
                    ID_Category = product.ID_Categoria,
                    CategoryName = product.CATEGORIA?.Nom_Categ,
                    Response = 1
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en OrderRepository.GetProductById: {ex.Message}");
            }
        }

        public PromotionDto GetPromotionByCode(string promotionCode)
        {
            var currentDate = DateTime.Now;

            return _context.PROMOCIONs
                .Where(p => p.Nombre_Prom == promotionCode
                         && p.Estado
                         && currentDate >= p.Fecha_Ini
                         && currentDate <= p.Fecha_Fin)
                .Select(p => new PromotionDto
                {
                    ID_Prom = p.ID_Promocion,
                    NameProm = p.Nombre_Prom,
                    DescriProm = p.Descrip_Prom,
                    DiscountPer = p.Porc_Desc,
                    StartDate = p.Fecha_Ini,
                    EndDate = p.Fecha_Fin,
                    IsActive = p.Estado,
                    Response = 1
                })
                .FirstOrDefault();
        }

        public bool ValidateProductPromotion(int productId, int promotionId)
        {
            return _context.PROMOCION_PRODUCTO
                .Any(pp => pp.ID_Promocion == promotionId
                         && pp.ID_Producto == productId);
        }

        public List<PromotionDto> GetValidPromotionsForProduct(int productId)
        {
            var currentDate = DateTime.Now;

            return _context.PROMOCION_PRODUCTO
                .Where(pp => pp.ID_Producto == productId)
                .Select(pp => pp.PROMOCION)
                .Where(p => p.Estado && currentDate >= p.Fecha_Ini && currentDate <= p.Fecha_Fin)
                .Select(p => new PromotionDto
                {
                    ID_Prom = p.ID_Promocion,
                    NameProm = p.Nombre_Prom,
                    DescriProm = p.Descrip_Prom,
                    DiscountPer = p.Porc_Desc,
                    StartDate = p.Fecha_Ini,
                    EndDate = p.Fecha_Fin,
                    IsActive = p.Estado,
                    Response = 1
                })
                .ToList();
        }
        public PEDIDO GetOrderWithDetails(int orderId)
        {
            return _context.PEDIDOes
                .Include(o => o.ESTADO)
                .Include(o => o.USUARIO) // Client
                .Include(o => o.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
                .FirstOrDefault(o => o.ID_Pedido == orderId);
        }
        public int CreateOrder(OrderDto order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Validar productos y calcular totales
                        decimal total = 0;
                        foreach (var detail in order.Details)
                        {
                            // Verificar stock
                            var stockQuery = "SELECT Stock FROM PRODUCTO WHERE ID_Producto = @ProductId";
                            using (var command = new SqlCommand(stockQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                var currentStock = (int)command.ExecuteScalar();

                                if (currentStock < detail.Quantity) 
                                    throw new Exception($"Stock insuficiente {detail.ProductId}");
                            }

                            // Calcular subtotal
                            decimal subtotal = detail.UnitPrice * detail.Quantity;

                            // Aplicar promoción si existe
                            if (detail.PromotionId.HasValue)
                            {
                                var promoQuery = @"
                                    SELECT p.Porc_Desc 
                                    FROM PROMOCION p
                                    INNER JOIN PROMOCION_PRODUCTO pp ON p.ID_Promocion = pp.ID_Promocion
                                    WHERE p.ID_Promocion = @PromotionId
                                    AND pp.ID_Producto = @ProductId
                                    AND p.Estado = 1
                                    AND GETDATE() BETWEEN p.Fecha_Ini AND p.Fecha_Fin";

                                using (var command = new SqlCommand(promoQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@PromotionId", detail.PromotionId.Value);
                                    command.Parameters.AddWithValue("@ProductId", detail.ProductId);

                                    var discount = command.ExecuteScalar();
                                    if (discount != null)
                                    {
                                        decimal discountPercent = Convert.ToDecimal(discount);
                                        subtotal = detail.UnitPrice * detail.Quantity * (100 - discountPercent) / 100;
                                    }
                                }
                            }

                            total += subtotal;
                        }

                        // 2. Crear el pedido principal
                        var insertOrderQuery = @"
                            INSERT INTO PEDIDO (ID_Cliente, ID_Estado, Fecha_Pedido, Tip_Pedido, Direccion_Ent, Fecha_Entrega, Total)
                            VALUES (@ClientId, @StatusId, @OrderDate, @OrderType, @DeliveryAddress, @DeliveryDate, @Total);
                            SELECT SCOPE_IDENTITY();";

                        int orderId;
                        using (var command = new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ClientId", order.ClientId);
                            command.Parameters.AddWithValue("@StatusId", order.StatusId);
                            command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                            command.Parameters.AddWithValue("@OrderType", order.OrderType);
                            command.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress);
                            command.Parameters.AddWithValue("@DeliveryDate", order.DeliveryDate);
                            command.Parameters.AddWithValue("@Total", total);

                            orderId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 3. Insertar detalles del pedido
                        foreach (var detail in order.Details)
                        {
                            decimal subtotal = detail.UnitPrice * detail.Quantity;

                            if (detail.PromotionId.HasValue)
                            {
                                // Recalcular subtotal con descuento si es necesario
                                var promoQuery = @"
                                    SELECT p.Porc_Desc 
                                    FROM PROMOCION p
                                    INNER JOIN PROMOCION_PRODUCTO pp ON p.ID_Promocion = pp.ID_Promocion
                                    WHERE p.ID_Promocion = @PromotionId
                                    AND pp.ID_Producto = @ProductId
                                    AND p.Estado = 1
                                    AND GETDATE() BETWEEN p.Fecha_Ini AND p.Fecha_Fin";

                                using (var command = new SqlCommand(promoQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@PromotionId", detail.PromotionId.Value);
                                    command.Parameters.AddWithValue("@ProductId", detail.ProductId);

                                    var discount = command.ExecuteScalar();
                                    if (discount != null)
                                    {
                                        decimal discountPercent = Convert.ToDecimal(discount);
                                        subtotal = detail.UnitPrice * detail.Quantity * (100 - discountPercent) / 100;
                                    }
                                }
                            }

                            var insertDetailQuery = @"
                                INSERT INTO DETALLE_PEDIDO (ID_Pedido, ID_Producto, ID_Promocion, Cantidad, PrecioUni, Subtotal)
                                VALUES (@OrderId, @ProductId, @PromotionId, @Quantity, @UnitPrice, @Subtotal)";

                            using (var command = new SqlCommand(insertDetailQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderId);
                                command.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                command.Parameters.AddWithValue("@PromotionId", detail.PromotionId ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                                command.Parameters.AddWithValue("@Subtotal", subtotal);

                                command.ExecuteNonQuery();
                            }

                           
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error creating order: " + ex.Message, ex);
                    }
                }
            }
        }
        public PROMOCION GetPromotionById(int promotionId)
        {
            return _context.PROMOCIONs.Find(promotionId);
        }

        public OrderDto GetOrderById(int orderId)
        {
            var order = _context.PEDIDOes
                .Include(o => o.DETALLE_PEDIDO)
                .Include(o => o.ESTADO)
                .FirstOrDefault(o => o.ID_Pedido == orderId);

            if (order == null) return null;

            return new OrderDto
            {
                ID_Order = order.ID_Pedido,
                ClientId = order.ID_Cliente,
                DeliveryUserId = order.ID_UsEntrega,
                StatusId = order.ID_Estado,
                OrderDate = order.Fecha_Pedido,
                OrderType = order.Tip_Pedido,
                DeliveryAddress = order.Direccion_Ent,
                DeliveryDate = order.Fecha_Entrega,
                Total = order.Total,
                Details = order.DETALLE_PEDIDO.Select(d => new OrderDetailDto
                {
                    ID_Detail = d.ID_DP,
                    OrderId = d.ID_Pedido,
                    ProductId = d.ID_Producto,
                    PromotionId = d.ID_Promocion,
                    Quantity = d.Cantidad,
                    UnitPrice = d.PrecioUni,
                    Subtotal = d.Subtotal,
                    ProductName = d.PRODUCTO?.Nombre,
                    ProductImageUrl = d.PRODUCTO?.IMAGENs.FirstOrDefault()?.Imagen_URL
                }).ToList()
            };
        }
        public string GetCustomerEmailByOrderId(int orderId)
        {
            var email = _context.PEDIDOes
                .Where(o => o.ID_Pedido == orderId)
                .Select(o => o.USUARIO.Email)
                .FirstOrDefault();

            return email ?? string.Empty;
        }

        public CartSummaryDto GetCartSummary(List<OrderDetailDto> cartItems)
        {
            var subtotal = cartItems.Sum(item => item.OriginalSubtotal);
            var discountTotal = cartItems.Sum(item => item.DiscountAmount);
            var total = subtotal - discountTotal;

            return new CartSummaryDto
            {
                Subtotal = subtotal,
                DiscountTotal = discountTotal,
                Total = total,
                ItemCount = cartItems.Sum(item => item.Quantity)
            };
        }
    }
}