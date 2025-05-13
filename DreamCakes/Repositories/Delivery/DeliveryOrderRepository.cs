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
    public class DeliveryOrderRepository
    {
        private readonly DreamCakesEntities _context;

        public DeliveryOrderRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<DeliveryAssignedOrderDto> GetAssignedOrders(int deliveryUserId, string statusFilter = null)
        {
            var query = _context.PEDIDOes
                .Where(p => p.ID_UsEntrega == deliveryUserId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(p => p.ESTADO.Nombre == statusFilter);
            }

            return query
                .Select(p => new DeliveryAssignedOrderDto
                {
                    OrderId = p.ID_Pedido,
                    CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                    CustomerPhone = p.USUARIO.Telefono,
                    DeliveryAddress = p.Direccion_Ent,
                    Status = p.ESTADO.Nombre,
                    EstimatedDeliveryTime = p.Fecha_Entrega,
                    TotalAmount = p.Total
                })
                .ToList();
        }

        public DeliveryOrderDetailDto GetOrderDetails(int orderId, int deliveryUserId)
        {
            return _context.PEDIDOes
                .Where(p => p.ID_Pedido == orderId && p.ID_UsEntrega == deliveryUserId)
                .Select(p => new DeliveryOrderDetailDto
                {
                    OrderId = p.ID_Pedido,
                    CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                    CustomerPhone = p.USUARIO.Telefono,
                    DeliveryAddress = p.Direccion_Ent,
                    Status = p.ESTADO.Nombre,
                    EstimatedDeliveryTime = p.Fecha_Entrega,
                    TotalAmount = p.Total,
                    Items = p.DETALLE_PEDIDO.Select(d => new DeliveryOrderItemDto
                    {
                        ProductName = d.PRODUCTO.Nombre,
                        Quantity = d.Cantidad,
                        UnitPrice = d.PrecioUni,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .FirstOrDefault();
        }
    }
}