using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Admin;
using DreamCakes.Dtos.Admin;

namespace DreamCakes.Services.Admin
{
    public class AdminAssignedOrderService
    {
        private readonly AdminAssignedOrderRepository _repository;
        private readonly EmailService _emailService;

        public AdminAssignedOrderService()
        {
            _repository = new AdminAssignedOrderRepository();
            _emailService = new EmailService();
        }

        public AdminOrderAssignmentViewDto GetOrderAssignmentData()
        {
            var model = new AdminOrderAssignmentViewDto
            {
                UnassignedOrders = new List<AdminUnassignedOrderDto>(),
                DeliveryUsers = new List<AdminDeliveryUserDto>()
            };

            try
            {
                model.UnassignedOrders = _repository.GetUnassignedOrders() ?? new List<AdminUnassignedOrderDto>();
                model.DeliveryUsers = _repository.GetAvailableDeliveryUsers() ?? new List<AdminDeliveryUserDto>();
                return model;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetOrderAssignmentData: {ex.ToString()}");
                model.ErrorMessage = "Error al cargar los datos";
                return model;
            }
        }
        public AdminAssignmentResponse AssignOrder(AdminAssignOrderDto assignment)
        {
            try
            {
                bool success = _repository.AssignOrderToDelivery(assignment.OrderId, assignment.DeliveryUserId);

                if (success)
                {
                    // Obtener correo del cliente
                    string customerEmail = _repository.GetCustomerEmailByOrderId(assignment.OrderId);
                    if (!string.IsNullOrEmpty(customerEmail))
                    {
                        _emailService.SendStatusUpdateNotification(
                            customerEmail,
                            assignment.OrderId,
                            "En camino" // Estado fijo al asignar desde el admin
                        );
                    }
                }

                return new AdminAssignmentResponse
                {
                    Success = success,
                    Message = success ? "Pedido asignado correctamente" : "No se pudo asignar el pedido"
                };
            }
            catch (Exception ex)
            {
                return new AdminAssignmentResponse
                {
                    Success = false,
                    Message = "Error al procesar la asignación",
                    TechnicalMessage = ex.Message
                };
            }
        }

    }



    public class AdminAssignmentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TechnicalMessage { get; set; }
    }
}