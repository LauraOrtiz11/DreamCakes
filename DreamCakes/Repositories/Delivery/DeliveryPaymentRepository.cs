using DreamCakes.Dtos.Delivery;
using DreamCakes.Repositories.Models;
using System;
using System.Data.Entity;
using System.Data;
using System.Linq;
using DreamCakes.Utilities;

namespace DreamCakes.Repositories.Delivery

{
    public class DeliveryPaymentRepository
    {
        private readonly DreamCakesEntities _context;

        public DeliveryPaymentRepository()
        {
            _context = new DreamCakesEntities();
        }

        public DeliveryPaymentDetailsDto GetOrderPaymentDetails(int orderId, int deliveryUserId)
        {
            return _context.PEDIDOes
                .Where(p => p.ID_Pedido == orderId &&
                           p.ID_UsEntrega == deliveryUserId &&
                           p.ID_Estado == 5) // Estado 5 = Entregado
                .Select(p => new DeliveryPaymentDetailsDto
                {
                    OrderId = p.ID_Pedido,
                    CustomerName = p.USUARIO.Nombres + " " + p.USUARIO.Apellidos,
                    TotalAmount = p.Total,
                    DeliveryDate = p.Fecha_Entrega,
                    DeliveryAddress = p.Direccion_Ent
                })
                .FirstOrDefault();
        }

        public DeliveryPaymentResultDto RegisterPayment(DeliveryPaymentRegisterDto paymentDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.PEDIDOes.Find(paymentDto.OrderId);
                    if (order == null)
                        return new DeliveryPaymentResultDto { Success = false, Message = "Pedido no encontrado" };

                    if (order.ID_UsEntrega != paymentDto.DeliveryUserId)
                        return new DeliveryPaymentResultDto { Success = false, Message = "No tienes permisos para registrar pagos en este pedido" };

                    if (order.ID_Estado != 5) // Verificar que esté en estado Entregado
                        return new DeliveryPaymentResultDto { Success = false, Message = "El pedido no está marcado como entregado" };

                    // Registrar el pago
                    var payment = new PAGO
                    {
                        ID_Pedido = paymentDto.OrderId,
                        Monto = paymentDto.AmountReceived,
                        Fecha_Pago = DateTime.Now,
                        ID_Metodo = 1, // 1=Efectivo, etc.
                        ID_Estado = paymentDto.IsFullPayment ? 7 : 8 // 7=Pagado, 8=Parcial
                    };

                    _context.PAGOes.Add(payment);
                    _context.SaveChanges();

                    // Si es pago completo, marcar pedido como pagado
                    if (paymentDto.IsFullPayment)
                    {
                        order.ID_Estado = 7; //
                        _context.SaveChanges();
                    }

                    transaction.Commit();

                    return new DeliveryPaymentResultDto
                    {
                        Success = true,
                        Message = paymentDto.IsFullPayment ?
                                 "Pago registrado exitosamente" :
                                 "Pago parcial registrado",
                        PendingAmount = paymentDto.IsFullPayment ? 0 : order.Total - paymentDto.AmountReceived,
                        PaymentId = payment.ID_Pago
                    };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new DeliveryPaymentResultDto { Success = false, Message = "Error al registrar el pago" };
                }
            }
        }

        public decimal GetAmountReceived(int paymentId)
        {
            return _context.PAGOes
                .Where(p => p.ID_Pago == paymentId)
                .Select(p => p.Monto)
                .FirstOrDefault();
        }
        public byte[] GenerateReceipt(int paymentId)
        {
            try
            {
                var payment = _context.PAGOes
                    .Include(p => p.PEDIDO)
                    .Include(p => p.PEDIDO.USUARIO)
                    .FirstOrDefault(p => p.ID_Pago == paymentId);

                if (payment == null) return null;

                var paymentDetails = new DeliveryPaymentDetailsDto
                {
                    OrderId = payment.PEDIDO.ID_Pedido,
                    CustomerName = payment.PEDIDO.USUARIO.Nombres + " " + payment.PEDIDO.USUARIO.Apellidos,
                    TotalAmount = payment.PEDIDO.Total,
                    DeliveryDate = payment.PEDIDO.Fecha_Entrega,
                    DeliveryAddress = payment.PEDIDO.Direccion_Ent
                };

                bool isFullPayment = payment.Monto == payment.PEDIDO.Total;

                return DeliveryReportGenerator.GeneratePaymentReceipt(
                    paymentDetails,
                    payment.Monto,
                    isFullPayment);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error generando comprobante: {ex.Message}");
                return null;
            }
        }
    }
}