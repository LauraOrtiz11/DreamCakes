using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Services.Delivery;
using DreamCakes.Utilities;
using System.Web.Mvc;

namespace DreamCakes.Controllers.Delivery
{
    public class DeliveryOrderController : Controller
    {
        private readonly DeliveryOrderService _service;

        public DeliveryOrderController()
        {
            _service = new DeliveryOrderService();
        }

        [HttpGet]
        public ActionResult AssignedOrders(string statusFilter = null)
        {
            try
            {
                // Obtener ID del domiciliario desde la sesión
                var deliveryUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
                if (deliveryUserId == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var response = _service.GetAssignedOrders(deliveryUserId.Value, statusFilter);

                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.Message;
                }

                ViewBag.StatusFilter = statusFilter;
                return View(response.Orders);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al obtener los pedidos asignados.";
                return RedirectToAction("General", "Error");
            }
        }

        [HttpGet]
        public ActionResult OrderDetails(int id)
        {
            try
            {
                var deliveryUserId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);
                if (deliveryUserId == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var response = _service.GetOrderDetails(id, deliveryUserId.Value);

                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.Message;
                    return RedirectToAction("AssignedOrders");
                }

                return View(response.Order);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los detalles del pedido.";
                return RedirectToAction("AssignedOrders");
            }
        }
    }
}
