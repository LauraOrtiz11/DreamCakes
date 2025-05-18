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
        private readonly DeliveryOrderRepository _orderRepository;

        public DeliveryPaymentService()
        {
            _repository = new DeliveryPaymentRepository();
            _orderRepository = new DeliveryOrderRepository();
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
            if (paymentDto.AmountReceived <= 0)
                return new DeliveryPaymentResultDto { Success = false, Message = "El monto recibido debe ser mayor a cero" };

            var orderDetails = _repository.GetOrderPaymentDetails(paymentDto.OrderId, paymentDto.DeliveryUserId);
            if (orderDetails == null)
                return new DeliveryPaymentResultDto { Success = false, Message = "No se encontraron detalles del pedido" };

            if (paymentDto.AmountReceived > orderDetails.TotalAmount)
                return new DeliveryPaymentResultDto { Success = false, Message = "El monto recibido no puede ser mayor al total del pedido" };

            var amountAlreadyPaid = _orderRepository.GetAmountPaid(paymentDto.OrderId);
            var totalPaid = amountAlreadyPaid + paymentDto.AmountReceived;

            // Aquí sí se evalúa correctamente si ya cubrió el total
            paymentDto.IsFullPayment = totalPaid >= orderDetails.TotalAmount;

            var result = _repository.RegisterPayment(paymentDto);

            if (result.Success)
            {
                result.OrderId = paymentDto.OrderId;
                result.TotalAmount = orderDetails.TotalAmount;
                result.AmountReceived = paymentDto.AmountReceived;
                result.PaymentDate = DateTime.Now;
                result.PendingAmount = Math.Max(0, orderDetails.TotalAmount - totalPaid);
            }

            return result;
        }

        public byte[] GenerateReceipt(int paymentId)
        {
            return _repository.GenerateReceipt(paymentId);
        }
    }
}