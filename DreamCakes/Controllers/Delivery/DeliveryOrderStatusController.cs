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
    public class DeliveryOrderStatusController : Controller
    {
        private readonly DeliveryOrderStatusService _service;

        public DeliveryOrderStatusController()
        {
            _service = new DeliveryOrderStatusService();
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
                    message = response.Message
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
