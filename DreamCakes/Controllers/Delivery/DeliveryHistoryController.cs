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
    public class DeliveryHistoryController : Controller
    {
        private readonly DeliveryHistoryService _service;

        public DeliveryHistoryController()
        {
            _service = new DeliveryHistoryService();
        }

        [HttpGet]
        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var currentUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
                if (currentUserId == null) return RedirectToAction("Login", "Account");

                var filter = new DeliveryHistoryFilterDto
                {
                    DeliveryUserId = currentUserId.Value,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var history = _service.GetDeliveryHistory(filter);

                if (history == null)
                {
                    TempData["ErrorMessage"] = "No fue posible cargar el historial de pedidos";
                    return View(new List<DeliveryHistoryDto>());
                }

                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

                return View(history);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar su solicitud";
                return View(new List<DeliveryHistoryDto>());
            }
        }
    }
}
