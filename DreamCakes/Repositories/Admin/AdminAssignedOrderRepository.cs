using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Models;
using DreamCakes.Dtos.Admin;

namespace DreamCakes.Repositories.Admin
{
    public class AdminAssignedOrderRepository
    {
        private readonly DreamCakesEntities _context;

        public AdminAssignedOrderRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<AdminUnassignedOrderDto> GetUnassignedOrders()
        {
            return _context.PEDIDOes
                .Where(p => p.ID_UsEntrega == null && p.ID_Estado == 3) // Pendientes sin asignar
                .Select(p => new AdminUnassignedOrderDto
                {
                    OrderId = p.ID_Pedido,
                    CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                    DeliveryAddress = p.Direccion_Ent,
                    OrderDate = p.Fecha_Pedido,
                    EstimatedDelivery = p.Fecha_Entrega,
                    TotalAmount = p.Total
                })
                .ToList();
        }

        public List<AdminDeliveryUserDto> GetAvailableDeliveryUsers()
        {
            return _context.USUARIOs
                .Where(u => u.ID_Rol == 3) // Rol de domiciliario
                .Select(u => new AdminDeliveryUserDto
                {
                    UserId = u.ID_Usuario,
                    FullName = u.Nombres + " " + u.Apellidos,
                    Phone = u.Telefono
                })
                .ToList();
        }

        public bool AssignOrderToDelivery(int orderId, int deliveryUserId)
        {
            try
            {
                var order = _context.PEDIDOes.Find(orderId);
                if (order == null || order.ID_UsEntrega != null)
                    return false;

                var deliveryUser = _context.USUARIOs.Find(deliveryUserId);
                if (deliveryUser == null || deliveryUser.ID_Rol != 3)
                    return false;

                order.ID_UsEntrega = deliveryUserId;
                order.ID_Estado = 4; // Cambiar estado a "En camino" o el que corresponda

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}