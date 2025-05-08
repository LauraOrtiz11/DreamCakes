using DreamCakes.Dtos.Client;
using DreamCakes.Services.Client;
using DreamCakes.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace DreamCakes.Controllers
{
    [RoleAuthorizeUtility(2)] // Asumo que 2 es el rol de cliente
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController()
        {
            _orderService = new OrderService();
        }

        public ActionResult Index()
        {
            var model = new OrderDto
            {
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(1),
                StatusId = 1, // Pendiente
                OrderType = "Inmediato",
                Details = GetCurrentCart()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ApplyPromotion(string promotionCode)
        {
            var result = _orderService.ValidatePromotionCode(promotionCode);

            SetResponseMessages(result);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                var currentCart = GetCurrentCart();
                var result = _orderService.AddToCart(currentCart, productId, quantity);

                if (result.Success)
                {
                    Session["Cart"] = result.Order.Details;
                }
                else
                {
                    // Registrar ubicación del error
                    System.Diagnostics.Debug.WriteLine($"Error en capa: {result.ErrorLocation}");
                    System.Diagnostics.Debug.WriteLine($"Mensaje técnico: {result.TechnicalMessage}");
                }

                SetResponseMessages(result);
                return RedirectOrAjax(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR NO MANEJADO EN CONTROLLER: {ex.ToString()}");
                TempData["ErrorMessage"] = "Error crítico al procesar tu solicitud";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var currentCart = GetCurrentCart();
            var result = _orderService.RemoveFromCart(currentCart, productId);

            if (result.Success)
            {
                Session["Cart"] = result.Order.Details;
            }

            SetResponseMessages(result);
            return RedirectOrAjax(result);
        }

        [HttpPost]
        public ActionResult PlaceOrder(OrderDto order)
        {
            order.Details = GetCurrentCart();
            order.OrderDate = DateTime.Now;
            order.StatusId = 1; // Pendiente
            order.Total = order.Details.Sum(d => d.Subtotal);
            order.PromotionCode = TempData["Promotion"]?.ToString();

            var result = _orderService.CreateOrder(order);

            if (result.Success)
            {
                Session["Cart"] = null;
                return View("OrderConfirmation", result.Order);
            }

            SetResponseMessages(result);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ScheduleOrder(OrderDto order)
        {
            order.Details = GetCurrentCart();
            order.OrderType = "Programado";
            order.PromotionCode = TempData["Promotion"]?.ToString();

            if (order.DeliveryDate < DateTime.Now.AddHours(2))
            {
                TempData["ErrorMessage"] = "La fecha de entrega debe ser al menos 2 horas después del momento actual";
                return RedirectToAction("Index");
            }

            var result = _orderService.CreateOrder(order);

            if (result.Success)
            {
                Session["Cart"] = null;
                return View("OrderConfirmation", result.Order);
            }

            SetResponseMessages(result);
            return RedirectToAction("Index");
        }

        #region Helpers

        private List<OrderDetailDto> GetCurrentCart()
        {
            return Session["Cart"] as List<OrderDetailDto> ?? new List<OrderDetailDto>();
        }

        private void SetResponseMessages(OrderResponseDto result)
        {
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }

        private ActionResult RedirectOrAjax(OrderResponseDto result)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    redirect = Url.Action("Index")
                });
            }
            return RedirectToAction("Index");
        }

        #endregion
    }
}