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
    public class DeliveryPaymentController : Controller
    {
        private readonly DeliveryPaymentService _service;
        private readonly DeliveryOrderService _orderService;

        public DeliveryPaymentController()
        {
            _service = new DeliveryPaymentService();
            _orderService = new DeliveryOrderService();
        }

        [HttpGet]
        public ActionResult RegisterPayment(int orderId)
        {

            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            // Verificar si el pedido ya está pagado completamente
            var isFullyPaid = _orderService.IsOrderFullyPaid(orderId);
            System.Diagnostics.Debug.WriteLine($"IsFullPayment: {isFullyPaid}");

            if (isFullyPaid)
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

            // Obtener monto ya pagado si existe pago parcial
            var amountPaid = _orderService.GetAmountPaid(orderId);
            ViewBag.AmountPaid = amountPaid;
            ViewBag.RemainingAmount = paymentDetails.TotalAmount - amountPaid;

            return View(paymentDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPayment(DeliveryPaymentRegisterDto model)
        {
            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Index", "Home");

            model.DeliveryUserId = currentUserId.Value;

            if (!ModelState.IsValid)
            {
                var details = _service.GetOrderPaymentDetails(model.OrderId, model.DeliveryUserId);
                return View(details);
            }


            var result = _service.RegisterPayment(model);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                var details = _service.GetOrderPaymentDetails(model.OrderId, model.DeliveryUserId);
                return View(details);
            }

            if (result.PendingAmount > 0)
            {
                TempData["WarningMessage"] = $"{result.Message}. Saldo pendiente: {result.PendingAmount.ToString("C", new System.Globalization.CultureInfo("es-CO"))}";
            }
            else
            {
                TempData["SuccessMessage"] = $"{result.Message}. Pago completado exitosamente.";
            }

            // Guardar el ID de pago para generar el comprobante
            TempData["PaymentId"] = result.PaymentId;
            TempData["IsFullPayment"] = result.PendingAmount == 0;

            return RedirectToAction("PaymentConfirmation", new
            {
                orderId = model.OrderId,
                isFullPayment = result.PendingAmount == 0
            });
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

        [HttpGet]
        public ActionResult PaymentConfirmation(int orderId, bool isFullPayment)
        {
            var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            var paymentId = TempData["PaymentId"] as int?;
            if (!paymentId.HasValue)
            {
                TempData["ErrorMessage"] = "No se encontró información del pago";
                return RedirectToAction("AssignedOrders", "DeliveryOrder");
            }

            var model = _service.GetOrderPaymentDetails(orderId, currentUserId.Value);
            if (model == null)
            {
                TempData["ErrorMessage"] = "No se encontró el pedido";
                return RedirectToAction("AssignedOrders", "DeliveryOrder");
            }

            ViewBag.PaymentId = paymentId.Value;
            ViewBag.IsFullPayment = isFullPayment;
            ViewBag.PendingAmount = model.TotalAmount - _service.GetAmountReceived(paymentId.Value);

            return View(model);
        }
    }
}