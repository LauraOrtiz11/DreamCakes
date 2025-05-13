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
                    order.ID_Estado == 4 && updateDto.NewStatusId == 5)   // De "En camino" a "Entregado"
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