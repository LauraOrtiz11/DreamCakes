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

        public bool IsOrderFullyPaid(int orderId)
        {
            return _context.PEDIDOes
                .Any(p => p.ID_Pedido == orderId && p.ID_Estado == 7); // Estado 6 = Pagado
        }

        public decimal GetAmountPaid(int orderId)
        {
            return _context.PAGOes
                .Where(p => p.ID_Pedido == orderId)
                .Sum(p => (decimal?)p.Monto) ?? 0; // Manejo de null con operador coalescente
        }
        public List<DeliveryAssignedOrderDto> GetAssignedOrders(int deliveryUserId, string statusFilter = null)
        {
            var query = _context.PEDIDOes
            .Include(p => p.USUARIO) // Cliente
            .Include(p => p.ESTADO)  // Estado
            .Where(p => p.ID_UsEntrega == deliveryUserId);



            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(p => p.ESTADO.Nombre == statusFilter);
            }
            query = query.Where(p => p.ID_Estado != 7);
            return query
                .Select(p => new DeliveryAssignedOrderDto
                {
                    OrderId = p.ID_Pedido,
                    CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                    CustomerPhone = p.USUARIO.Telefono,
                    DeliveryAddress = p.Direccion_Ent,
                    Status = p.ESTADO.Nombre,
                    EstimatedDeliveryTime = p.Fecha_Entrega,
                    TotalAmount = p.Total,
                    HasPartialPayment = p.PAGOes.Any(pg => pg.ID_Estado == 2)
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