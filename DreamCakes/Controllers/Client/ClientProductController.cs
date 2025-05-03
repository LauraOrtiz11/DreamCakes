using DreamCakes.Dtos.Client;
using DreamCakes.Services.Client;
using System;
using System.Web.Mvc;

namespace DreamCakes.Controllers.Client
{
    public class ClientProductController : Controller
    {
        private readonly ClientProductService _productService = new ClientProductService();

        public ActionResult Search(ProductSearchCriteriaDto criteria)
        {
            try
            {
                var results = _productService.SearchProducts(criteria);
                return View(results);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in ClientProduct/Search: {ex.Message}");
                return View("Error", new HandleErrorInfo(ex, "ClientProduct", "Search"));
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var product = _productService.GetProductDetails(id);

                if (product == null)
                {
                    return HttpNotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClientProduct/Details: {ex.Message}");
                return View("Error", new HandleErrorInfo(ex, "ClientProduct", "Details"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitRating(SubmitRatingDto ratingDto)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new RatingResultDto
                    {
                        Success = false,
                        Message = "Please login to submit a rating"
                    });
                }

                // Set client ID from logged in user
                ratingDto.ClientID = GetCurrentUserId();

                var result = _productService.SubmitRating(ratingDto);

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClientProduct/SubmitRating: {ex.Message}");
                return Json(new RatingResultDto
                {
                    Success = false,
                    Message = "An error occurred while processing your rating"
                });
            }
        }

        private int GetCurrentUserId()
        {
            // Implementar lógica para obtener ID de usuario actual
            // Esto es un ejemplo - debes adaptarlo a tu sistema de autenticación
            return 1; // Reemplazar con ID real del usuario
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _productService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}