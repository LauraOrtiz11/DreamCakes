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
    public class DeliveryOrderStatusRepository
    {
        private readonly DreamCakesEntities _context;

        public DeliveryOrderStatusRepository()
        {
            _context = new DreamCakesEntities();
        }
        // DeliveryOrderStatusRepository.cs
        public PEDIDO GetOrder(int orderId)
        {
            return _context.PEDIDOes.FirstOrDefault(p => p.ID_Pedido == orderId);
        }

        public void UpdateOrder(PEDIDO order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // DeliveryOrderStatusRepository.cs
        public DeliveryOrderDetailDto GetOrderDetails(int orderId)
        {
            var order = _context.PEDIDOes
                .Include(p => p.USUARIO) // Cliente
                .Include(p => p.ESTADO) // Estado
                .Include(p => p.DETALLE_PEDIDO.Select(dp => dp.PRODUCTO)) // Productos
                .FirstOrDefault(p => p.ID_Pedido == orderId);

            if (order == null) return null;

            return new DeliveryOrderDetailDto
            {
                OrderId = order.ID_Pedido,
                CustomerName = order.USUARIO.Nombres + " " + order.USUARIO.Apellidos,
                DeliveryAddress = order.Direccion_Ent,
                OrderDate = order.Fecha_Pedido,
                EstimatedDelivery = order.Fecha_Entrega,
                TotalAmount = order.Total,
                Status = order.ESTADO.Nombre,
                StatusId = order.ID_Estado,
                DeliveryUserId = order.ID_UsEntrega,
                Items = order.DETALLE_PEDIDO.Select(dp => new DeliveryOrderItemDto
                {
                    ProductName = dp.PRODUCTO.Nombre,
                    Quantity = dp.Cantidad,
                    UnitPrice = dp.PrecioUni,
                    Subtotal = dp.Subtotal
                }).ToList()
            };
        }

        public List<ESTADO> GetAvailableStatuses(int currentStatusId)
        {
            // Solo permitir ciertas transiciones de estado según tu lógica de negocio
            if (currentStatusId == 3) // Preparado
            {
                return _context.ESTADOes.Where(e => e.ID_Estado == 4).ToList(); // En camino
            }
            else if (currentStatusId == 4) // En camino
            {
                return _context.ESTADOes.Where(e => e.ID_Estado == 5).ToList(); // Entregado
            }
            return new List<ESTADO>();
        }
        public List<DeliveryOrderStatusDto> GetAvailableStatuses()
        {
            return _context.ESTADOes
                .Where(e => e.ID_Estado == 4 || e.ID_Estado == 5) // 4: En camino, 5: Entregado
                .Select(e => new DeliveryOrderStatusDto
                {
                    StatusId = e.ID_Estado,
                    StatusName = e.Nombre
                })
                .ToList();
        }

        public bool UpdateOrderStatus(DeliveryOrderStatusUpdateDto updateDto)
        {
            try
            {
                var order = _context.PEDIDOes
                    .FirstOrDefault(p => p.ID_Pedido == updateDto.OrderId &&
                                       p.ID_UsEntrega == updateDto.DeliveryUserId);

                if (order == null)
                    return false;

                // Validar transición de estados válida
                if (order.ID_Estado == 3 && updateDto.NewStatusId == 4 || // De "Preparado" a "En camino"
                    order.ID_Estado == 4 && updateDto.NewStatusId == 5 ||
                    order.ID_Estado == 5 && updateDto.NewStatusId == 4)   // De "En camino" a "Entregado"
                {
                    order.ID_Estado = updateDto.NewStatusId;



                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetCustomerEmail(int orderId)
        {
            return _context.PEDIDOes
                .Where(p => p.ID_Pedido == orderId)
                .Select(p => p.USUARIO.Email)
                .FirstOrDefault();
        }
    }
}