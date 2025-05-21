using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Services.Delivery;
using DreamCakes.Utilities;
using System.Web.Mvc;
using DreamCakes.Dtos.Delivery;


namespace DreamCakes.Controllers.Delivery
{
    [RoleAuthorizeUtility(3)]
    public class DeliveryPaymentController : Controller
    {
        private readonly DeliveryPaymentService _service;
        

        public DeliveryPaymentController()
        {
            _service = new DeliveryPaymentService();
        }

        [HttpGet]
        public ActionResult RegisterPayment(int orderId)
        {
            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Index", "Home");

            // Verificar si el pedido ya está pagado completamente
            if (_service.IsOrderFullyPaid(orderId))
            {
                TempData["ErrorMessage"] = "Este pedido ya ha sido pagado completamente";
                return RedirectToAction("AssignedOrders");
            }

            var paymentDetails = _service.GetOrderPaymentDetails(orderId, currentUserId.Value);
            if (paymentDetails == null)
            {
                TempData["ErrorMessage"] = "No se encontró el pedido o no tienes permisos";
                return RedirectToAction("AssignedOrders");
            }

            // Obtener monto ya pagado
            var amountPaid = _service.GetAmountPaid(orderId);
            ViewBag.AmountPaid = amountPaid;
            ViewBag.RemainingAmount = paymentDetails.TotalAmount - amountPaid;

            return View(paymentDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPayment(DeliveryPaymentRegisterDto model)
        {
            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            try
            {
                model.DeliveryUserId = currentUserId.Value;

                // Convertir el monto a formato decimal correcto
                model.AmountReceived = decimal.Parse(
                    model.AmountReceived.ToString().Replace(".", "").Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture
                );

                // Verificar estado actual del pedido
                var orderDetails = _service.GetOrderPaymentDetails(model.OrderId, model.DeliveryUserId);
                if (orderDetails == null)
                {
                    TempData["ErrorMessage"] = "El pedido no existe o no tienes permisos";
                    return RedirectToAction("AssignedOrders", "DeliveryOrder");
                }

                // Calcular saldo pendiente
                var amountPaid = _service.GetAmountPaid(model.OrderId);
                var remaining = orderDetails.TotalAmount - amountPaid;

                // Validar que el pedido aún no esté pagado completamente
                if (remaining <= 0)
                {
                    TempData["ErrorMessage"] = "Este pedido ya ha sido pagado completamente";
                    return RedirectToAction("AssignedOrders", "DeliveryOrder");
                }

                // Procesar el pago
                var result = _service.RegisterPayment(model);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(_service.GetOrderPaymentDetails(model.OrderId, model.DeliveryUserId));
                }

                // Preparar mensajes para la vista
                if (result.PendingAmount > 0)
                {
                    TempData["WarningMessage"] = $"Pago parcial registrado. Saldo pendiente: {result.PendingAmount.ToString("C", new System.Globalization.CultureInfo("es-CO"))}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Pago completado exitosamente. Saldo pendiente: $0.00";
                }

                // Redireccionar con parámetros
                TempData["PaymentId"] = result.PaymentId;
                TempData["OrderId"] = model.OrderId;
                TempData["IsFullPayment"] = result.PendingAmount == 0;

                return RedirectToAction("PaymentConfirmation");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error procesando el pago: " + ex.Message;
                return View(_service.GetOrderPaymentDetails(model.OrderId, model.DeliveryUserId));
            }
        }

        [HttpGet]
        public ActionResult PaymentConfirmation()
        {
            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            // Obtener parámetros de TempData
            var paymentId = TempData["PaymentId"] as int?;
            var orderId = TempData["OrderId"] as int?;
            var isFullPayment = TempData["IsFullPayment"] as bool? ?? false;

            if (!paymentId.HasValue || !orderId.HasValue)
            {
                TempData["ErrorMessage"] = "Sesión de pago no encontrada";
                return RedirectToAction("AssignedOrders", "DeliveryOrder");
            }

            // Obtener detalles del pedido
            var model = _service.GetOrderPaymentDetails(orderId.Value, currentUserId.Value);
            if (model == null)
            {
                TempData["ErrorMessage"] = "No se encontró el pedido";
                return RedirectToAction("AssignedOrders", "DeliveryOrder");
            }

            // Calcular saldo pendiente
            var pendingAmount = isFullPayment ? 0 : (model.TotalAmount - _service.GetAmountReceived(paymentId.Value));

            // Preparar ViewBag
            ViewBag.PaymentId = paymentId.Value;
            ViewBag.IsFullPayment = isFullPayment;
            ViewBag.PendingAmount = pendingAmount;

            return View(model);
        }

        [HttpGet]
        public ActionResult DownloadReceipt(int paymentId)
        {
            var receipt = _service.GenerateReceipt(paymentId);
            if (receipt == null || receipt.Length == 0)
            {
                TempData["ErrorMessage"] = "No se pudo generar el comprobante";
                return RedirectToAction("AssignedOrders", "DeliveryOrder");
            }

            return File(receipt, "application/pdf", $"ComprobantePago_{paymentId}.pdf");
        }

        
    }
}