using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Data.Entity;
using System.Data;
using System.Linq;


namespace DreamCakes.Repositories.Client
{
    public class OrderRepository
    {
        private readonly DreamCakesEntities _context;

        public OrderRepository()
        {
            _context = new DreamCakesEntities();
        }

        // Método para obtener producto por ID
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
                    throw new Exception($"Producto con ID {productId} no encontrado en la base de datos");
                }

                return new ProductDto
                {
                    ID_Product = product.ID_Producto,
                    Name = product.Nombre,
                    Description = product.Descripcion,
                    Price = product.Precio,
                    Stock = product.Stock,
                    // Otras propiedades necesarias
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
                    IsActive = p.Estado
                })
                .FirstOrDefault();
        }

        public int CreateOrder(OrderDto orderDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Create order entity
                    var order = new PEDIDO
                    {
                        ID_Cliente = orderDto.ClientId,
                        ID_UsEntrega = orderDto.DeliveryUserId,
                        ID_Estado = orderDto.StatusId,
                        Fecha_Pedido = orderDto.OrderDate,
                        Tip_Pedido = orderDto.OrderType,
                        Direccion_Ent = orderDto.DeliveryAddress,
                        Fecha_Entrega = orderDto.DeliveryDate,
                        Total = orderDto.Total,
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}