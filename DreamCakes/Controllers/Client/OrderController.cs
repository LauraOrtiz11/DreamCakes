using DreamCakes.Dtos.Client;
using DreamCakes.Services.Client;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;



namespace DreamCakes.Controllers
{
    [Authorize]
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
        public ActionResult ApplyPromotion(int productId, string promotionCode)
        {
            var result = _orderService.ValidatePromotionForProduct(promotionCode, productId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                // Actualizar el carrito con la promoción
                var cart = GetCurrentCart();
                var item = cart.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.PromotionId = result.Promotion.ID_Prom;
                    // Recalcular subtotal con descuento
                    item.Subtotal = item.UnitPrice * item.Quantity *
                                   (100 - result.Promotion.DiscountPer) / 100;
                }
                Session["Cart"] = cart;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemovePromotion(int productId)
        {
            var cart = GetCurrentCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.PromotionId = null;
                // Recalcular subtotal sin descuento
                item.Subtotal = item.UnitPrice * item.Quantity;
                TempData["SuccessMessage"] = "Promoción removida correctamente";
            }
            Session["Cart"] = cart;

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
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PlaceOrder(OrderDto order)
        {
            order.Details = GetCurrentCart();
            order.OrderDate = DateTime.Now;
            order.StatusId = 3; // Pendiente
            order.OrderType = "Inmediato";
            order.DeliveryUserId = null; // Asegurar que sea null

            var result = _orderService.CreateOrder(order);

            if (result.Success)
            {
                Session["Cart"] = null;
                return RedirectToAction("OrderConfirmation", new { orderId = result.OrderId });
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult GetCartCount()
        {
            var cart = GetCurrentCart();
            return Json(new { count = cart.Sum(item => item.Quantity) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ScheduleOrder(OrderDto order)
        {
            order.Details = GetCurrentCart();
            order.OrderType = "Programado";
            order.StatusId = 3; // Pendiente
            order.OrderDate = DateTime.Now;
            order.DeliveryUserId = null; // Asegurar que sea null

            if (order.DeliveryDate < DateTime.Now.AddHours(2))
            {
                TempData["ErrorMessage"] = "La fecha de entrega debe ser al menos 2 horas después del momento actual";
                return RedirectToAction("Index");
            }

            var result = _orderService.CreateOrder(order);

            if (result.Success)
            {
                Session["Cart"] = null;
                return RedirectToAction("OrderConfirmation", new { orderId = result.OrderId });
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        public ActionResult OrderConfirmation(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = GetCurrentCart();
            var summary = _orderService.GetCartSummary(cart);
            return PartialView("_CartSummary", summary);
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
        #region Helpers
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
        private List<OrderDetailDto> GetCurrentCart()
        {
            return Session["Cart"] as List<OrderDetailDto> ?? new List<OrderDetailDto>();
        }

        #endregion
    }
}