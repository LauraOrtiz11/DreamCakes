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
    public class DeliveryOrderStatusController : Controller
    {
        private readonly DeliveryOrderStatusService _service;

        public DeliveryOrderStatusController()
        {
            _service = new DeliveryOrderStatusService();
        }

        [HttpGet]
        public ActionResult OrderDetails(int orderId)
        {
            try
            {
                var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
                if (currentUserId == null) return RedirectToAction("Login", "Account");

                // Obtener detalles del pedido a través del servicio
                var orderDetails = _service.GetOrderDetails(orderId);
                if (orderDetails == null) return HttpNotFound();

                // Verificar permisos (domiciliario asignado o admin)

                // Obtener estados disponibles a través del servicio
                var availableStatuses = _service.GetAvailableStatuses();
                ViewBag.AvailableStatuses = availableStatuses;

                return View(orderDetails);
            }
            catch (Exception ex)
            {
                
                TempData["ErrorMessage"] = "Error al cargar los detalles del pedido";
                return RedirectToAction("AssignedOrders", "AdminAssignedOrder");
            }
        }

        [HttpPost]
        public ActionResult UpdateStatus(int orderId, int newStatusId)
        {
            try
            {
                var deliveryUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
                if (deliveryUserId == null)
                {
                    return Json(new { success = false, message = "Usuario no autenticado" });
                }

                var updateDto = new DeliveryOrderStatusUpdateDto
                {
                    OrderId = orderId,
                    DeliveryUserId = deliveryUserId.Value,
                    NewStatusId = newStatusId
                };

                var response = _service.UpdateOrderStatus(updateDto);

                return Json(new
                {
                    success = response.Success,
                    message = response.Message,
                    
                    newStatusId = newStatusId
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Error inesperado al actualizar el estado"
                });
            }
        }

        [HttpGet]
        public ActionResult GetStatusOptions()
        {
            try
            {
                var statuses = _service.GetAvailableStatuses();
                return Json(statuses, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<DeliveryOrderStatusDto>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
