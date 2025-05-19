using System;
using System.Web.Mvc;
using DreamCakes.Services.Admin;
using DreamCakes.Dtos.Admin;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers.Admin
{
    [RoleAuthorizeUtility(1)]
    public class AdminAssignedOrderController : Controller
    {
        private readonly AdminAssignedOrderService _service;

        public AdminAssignedOrderController()
        {
            try
            {
                _service = new AdminAssignedOrderService();
            }
            catch (Exception ex)
            {
                // Loggear el error
                System.Diagnostics.Debug.WriteLine($"Error al crear el servicio: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public ActionResult AssignOrders()
        {
            try
            {
                var model = _service.GetOrderAssignmentData();

                if (!string.IsNullOrEmpty(model.ErrorMessage))
                {
                    TempData["ErrorMessage"] = model.ErrorMessage;
                }
                // En el método GET del controlador, antes de return View(model);
                System.Diagnostics.Debug.WriteLine($"Pedidos: {model.UnassignedOrders?.Count ?? 0}, Domiciliarios: {model.DeliveryUsers?.Count ?? 0}");
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al cargar la página";
                return View(new AdminOrderAssignmentViewDto());
            }
        }

        [HttpPost]
        public ActionResult AssignOrder(AdminAssignOrderDto assignment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Datos de asignación inválidos";
                    return RedirectToAction("AssignOrders");
                }

                var response = _service.AssignOrder(assignment);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                }

                return RedirectToAction("AssignOrders");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al asignar el pedido";
                return RedirectToAction("AssignOrders");
            }
        }
    }
}