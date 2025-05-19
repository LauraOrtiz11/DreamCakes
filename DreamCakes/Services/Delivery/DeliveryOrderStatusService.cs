using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Delivery;
using DreamCakes.Dtos.Delivery;


namespace DreamCakes.Services.Delivery
{
    public class DeliveryOrderStatusService
    {
        private readonly DeliveryOrderStatusRepository _repository;
        private readonly EmailService _emailService;

        public DeliveryOrderStatusService()
        {
            _repository = new DeliveryOrderStatusRepository();
            _emailService = new EmailService();
        }

        public DeliveryStatusUpdateResponse UpdateOrderStatus(DeliveryOrderStatusUpdateDto updateDto)
        {
            try
            {
                bool success = _repository.UpdateOrderStatus(updateDto);
                string message = success ? "Estado actualizado correctamente" : "No se pudo actualizar el estado";

                if (success)
                {
                    // Notificar al cliente
                    string customerEmail = _repository.GetCustomerEmail(updateDto.OrderId);
                    if (!string.IsNullOrEmpty(customerEmail))
                    {
                        string statusName = _repository.GetAvailableStatuses()
                            .FirstOrDefault(s => s.StatusId == updateDto.NewStatusId)?.StatusName ?? "nuevo estado";

                        _emailService.SendStatusUpdateNotification(
                            customerEmail,
                            updateDto.OrderId,
                            statusName);
                    }
                }

                return new DeliveryStatusUpdateResponse
                {
                    Success = success,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new DeliveryStatusUpdateResponse
                {
                    Success = false,
                    Message = "Error al actualizar el estado",
                    TechnicalMessage = ex.Message
                };
            }
        }
        public DeliveryOrderDetailDto GetOrderDetails(int orderId)
        {
            return _repository.GetOrderDetails(orderId);
        }
        public List<DeliveryOrderStatusDto> GetAvailableStatuses()
        {
            return _repository.GetAvailableStatuses();
        }
    }

    // Response class
    public class DeliveryStatusUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TechnicalMessage { get; set; }
    }
}