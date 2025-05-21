using DreamCakes.Dtos.Delivery;
using DreamCakes.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DreamCakes.Repositories.Delivery
{
    public class DeliveryHistoryRepository
    {
        private readonly DreamCakesEntities _context;

        public DeliveryHistoryRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<DeliveryHistoryDto> GetDeliveryHistory(int deliveryUserId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.PEDIDOes
                    .Include(p => p.USUARIO) // Cliente
                    .Include(p => p.ESTADO)  // Estado
                    .Include(p => p.DETALLE_PEDIDO.Select(dp => dp.PRODUCTO)) // Productos
                    .Where(p => p.ID_UsEntrega == deliveryUserId && p.ID_Estado == 5 || p.ID_Estado == 7); // Estado 5 = Entregado

                // Aplicar filtros de fecha si existen
                if (startDate.HasValue)
                    query = query.Where(p => p.Fecha_Entrega >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(p => p.Fecha_Entrega <= endDate.Value);

                return query.OrderByDescending(p => p.Fecha_Entrega)
                    .Select(p => new DeliveryHistoryDto
                    {
                        OrderId = p.ID_Pedido,
                        CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                        DeliveryDate = p.Fecha_Entrega,
                        DeliveryAddress = p.Direccion_Ent,
                        TotalAmount = p.Total,
                        Status = p.ESTADO.Nombre,
                        Items = p.DETALLE_PEDIDO.Select(dp => new DeliveryOrderItemDto
                        {
                            ProductName = dp.PRODUCTO.Nombre,
                            Quantity = dp.Cantidad,
                            UnitPrice = dp.PrecioUni,
                            Subtotal = dp.Subtotal
                        }).ToList()
                    }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}