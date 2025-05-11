using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Data.Entity;
using System.Linq;
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

        public int CreateOrder(OrderDto orderDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Validar que no se esté asignando repartidor al crear el pedido
                    if (orderDto.DeliveryUserId.HasValue)
                    {
                        throw new InvalidOperationException("No se puede asignar repartidor al crear el pedido");
                    }

                    // Aplicar promociones y calcular subtotales
                    foreach (var detail in orderDto.Details)
                    {
                        if (detail.PromotionId.HasValue)
                        {
                            // Validar que la promoción aplica al producto
                            if (!ValidateProductPromotion(detail.ProductId, detail.PromotionId.Value))
                            {
                                throw new Exception($"La promoción no aplica al producto ID {detail.ProductId}");
                            }

                            // Obtener el porcentaje de descuento
                            var promotion = _context.PROMOCIONs.Find(detail.PromotionId.Value);
                            if (promotion == null || !promotion.Estado ||
                                DateTime.Now < promotion.Fecha_Ini ||
                                DateTime.Now > promotion.Fecha_Fin)
                            {
                                throw new Exception("Promoción no válida o fuera de vigencia");
                            }

                            // Calcular subtotal con descuento
                            detail.Subtotal = detail.UnitPrice * detail.Quantity *
                                            (100 - promotion.Porc_Desc) / 100;
                        }
                        else
                        {
                            // Calcular subtotal sin descuento
                            detail.Subtotal = detail.UnitPrice * detail.Quantity;
                        }
                    }

                    // Calcular el total sumando todos los subtotales
                    var total = orderDto.Details.Sum(d => d.Subtotal);

                    // Crear entidad de pedido
                    var order = new PEDIDO
                    {
                        ID_Cliente = orderDto.ClientId,
                        ID_UsEntrega = null, // Siempre NULL al crear
                        ID_Estado = orderDto.StatusId,
                        Fecha_Pedido = orderDto.OrderDate,
                        Tip_Pedido = orderDto.OrderType,
                        Direccion_Ent = orderDto.DeliveryAddress,
                        Fecha_Entrega = orderDto.DeliveryDate,
                        Total = total,
                        DETALLE_PEDIDO = orderDto.Details.Select(d => new DETALLE_PEDIDO
                        {
                            ID_Producto = d.ProductId,
                            ID_Promocion = d.PromotionId,
                            Cantidad = d.Quantity,
                            PrecioUni = d.UnitPrice,
                            Subtotal = d.Subtotal
                        }).ToList()
                    };

                    _context.PEDIDOes.Add(order);
                    _context.SaveChanges();

                    transaction.Commit();
                    return order.ID_Pedido;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Error al crear el pedido: {ex.Message}");
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