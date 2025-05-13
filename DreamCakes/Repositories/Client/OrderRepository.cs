using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

using System.Collections.Generic;

namespace DreamCakes.Repositories.Client
{
    public class OrderRepository
    { private readonly DreamCakesEntities _context;

        public OrderRepository()
        {
            _context = new DreamCakesEntities();
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Validate products and calculate totals
                    decimal total = 0;
                    var orderDetails = new List<DETALLE_PEDIDO>();

                    foreach (var detail in order.Details)
                    {
                        var product = _context.PRODUCTOes.Find(detail.ProductId);
                        if (product == null)
                            throw new Exception($"Product ID {detail.ProductId} not found");

                        if (product.Stock < detail.Quantity)
                            throw new Exception($"Insufficient stock for product {product.Nombre}");

                        decimal subtotal = detail.UnitPrice * detail.Quantity;

                        // Apply promotion if exists
                        if (detail.PromotionId.HasValue)
                        {
                            var promotion = _context.PROMOCIONs
                                .Include(p => p.PROMOCION_PRODUCTO)
                                .FirstOrDefault(p => p.ID_Promocion == detail.PromotionId
                                                  && p.Estado
                                                  && p.PROMOCION_PRODUCTO.Any(pp => pp.ID_Producto == detail.ProductId)
                                                  && DateTime.Now >= p.Fecha_Ini
                                                  && DateTime.Now <= p.Fecha_Fin);

                            if (promotion != null)
                            {
                                subtotal = detail.UnitPrice * detail.Quantity * (100 - promotion.Porc_Desc) / 100;
                            }
                        }

                        total += subtotal;

                        orderDetails.Add(new DETALLE_PEDIDO
                        {
                            ID_Producto = detail.ProductId,
                            ID_Promocion = detail.PromotionId,
                            Cantidad = detail.Quantity,
                            PrecioUni = detail.UnitPrice,
                            Subtotal = subtotal
                        });
                    }

                    // Create main order
                    var newOrder = new PEDIDO
                    {
                        ID_Cliente = order.ClientId,
                        ID_Estado = order.StatusId,
                        Fecha_Pedido = DateTime.Now,
                        Tip_Pedido = order.OrderType,
                        Direccion_Ent = order.DeliveryAddress,
                        Fecha_Entrega = order.DeliveryDate,
                        Total = total,
                        DETALLE_PEDIDO = orderDetails
                    };

                    _context.PEDIDOes.Add(newOrder);
                    _context.SaveChanges();

                    // Update product stocks
                    foreach (var detail in order.Details)
                    {
                        var product = _context.PRODUCTOes.Find(detail.ProductId);
                        product.Stock -= detail.Quantity;
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return newOrder.ID_Pedido;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
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