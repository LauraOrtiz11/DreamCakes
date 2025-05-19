using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Delivery;
using DreamCakes.Dtos.Delivery;

namespace DreamCakes.Services.Delivery
{
    public class DeliveryPaymentService
    {
        private readonly DeliveryPaymentRepository _repository;
        

        public DeliveryPaymentService()
        {
            _repository = new DeliveryPaymentRepository();

        }
        public bool IsOrderFullyPaid(int orderId)
        {
            return _repository.IsOrderFullyPaid(orderId);
        }


        public decimal GetAmountPaid(int orderId)
        {
            return _repository.GetAmountPaid(orderId);
        }
        public DeliveryPaymentDetailsDto GetOrderPaymentDetails(int orderId, int deliveryUserId)
        {
            return _repository.GetOrderPaymentDetails(orderId, deliveryUserId);
        }
        public decimal GetAmountReceived(int paymentId)
        {
            return _repository.GetAmountReceived(paymentId);
        }
        public DeliveryPaymentResultDto RegisterPayment(DeliveryPaymentRegisterDto paymentDto)
        {
            // Validación básica
            if (paymentDto.AmountReceived <= 0)
                return new DeliveryPaymentResultDto { Success = false, Message = "El monto recibido debe ser mayor a cero" };

            // Obtener detalles del pedido
            var orderDetails = _repository.GetOrderPaymentDetails(paymentDto.OrderId, paymentDto.DeliveryUserId);
            if (orderDetails == null)
                return new DeliveryPaymentResultDto { Success = false, Message = "No se encontraron detalles del pedido" };

            // Obtener total ya pagado
            var amountAlreadyPaid = _repository.GetAmountPaid(paymentDto.OrderId);
            var remainingAmount = orderDetails.TotalAmount - amountAlreadyPaid;

            // Si marcó "Pago completo", forzar el monto a cubrir el saldo pendiente
            if (paymentDto.IsFullPayment)
            {
                paymentDto.AmountReceived = remainingAmount;
            }

            // Validar monto recibido
            if (paymentDto.AmountReceived > remainingAmount)
            {
                return new DeliveryPaymentResultDto
                {
                    Success = false,
                    Message = $"El monto recibido no puede ser mayor al saldo pendiente ({remainingAmount.ToString("C", new System.Globalization.CultureInfo("es-CO"))})"
                };
            }

            // Registrar el pago
            var result = _repository.RegisterPayment(paymentDto);

            if (result.Success)
            {
                result.OrderId = paymentDto.OrderId;
                result.TotalAmount = orderDetails.TotalAmount;
                result.AmountReceived = paymentDto.AmountReceived;
                result.PaymentDate = DateTime.Now;
                result.PendingAmount = Math.Max(0, remainingAmount - paymentDto.AmountReceived);
            }

            return result;
        }

        public byte[] GenerateReceipt(int paymentId)
        {
            return _repository.GenerateReceipt(paymentId);
        }
    }
}