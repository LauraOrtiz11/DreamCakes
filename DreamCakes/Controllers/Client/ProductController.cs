using DreamCakes.Dtos.Client;
using DreamCakes.Utilities;
using DreamCakes.Services.Client;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DreamCakes.Controllers.Client
{
    public class ProductController : Controller
    {
        public async Task<ActionResult> Catalog(string category = null)
        {
            using (var service = new ProductService())
            {
                try
                {
                    // 1. Obtener promociones activas (siempre se muestran)
                    var promotions = await service.GetActivePromotions();

                    // 2. Obtener categorías activas
                    var categories = await service.GetActiveCategories();
                    ViewBag.AvailableCategories = categories?
                        .Where(c => c.Response == 1)
                        .Select(c => c.CatName)
                        .ToList();

                    // 3. Obtener productos (todos o por categoría)
                    var productsData = await service.GetProductsByCategory(category);

                    // Crear respuesta
                    var catalogResponse = new CatalogResponseDto
                    {
                        ActivePromotions = promotions ?? new List<PromotionDto>(), // Siempre incluir promociones
                        Products = productsData.Products, // Todos los productos (filtrados o no)
                        ProductsByCategory = productsData.ProductsByCategory, // Productos agrupados
                        Response = productsData.Response,
                        Message = productsData.Message
                    };

                    if (productsData.Response != 1)
                    {
                        ViewBag.Error = productsData.Message;
                    }

                    return View(catalogResponse);
                }
                catch
                {
                    return View("Error");
                }
            }
        }

        // DreamCakes.Controllers.Client/ProductController.cs
        public async Task<ActionResult> ProductDetails(int id)
        {
            using (var service = new ProductService())
            {
                try
                {
                    // Obtener ID de usuario de la sesión
                    int? clientId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);

                    // Obtener el producto con reseñas
                    var product = await service.GetProductWithReviews(id);

                    if (product == null || product.Response != 1)
                    {
                        return HttpNotFound();
                    }

                    // Obtener promoción activa
                    var promotions = await service.GetActivePromotions();
                    ViewBag.ActivePromotion = promotions.FirstOrDefault();

                    return View(product);
                }
                catch
                {
                    return View("Error");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitReview(ReviewRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            int? clientId = SessionManagerUtility.GetCurrentUserId(HttpContext.Session);


            using (var service = new ProductService())
            {
                try
                {
                    var result = await service.SubmitProductReview(request, clientId.Value);

                    if (result.Response == 1)
                    {
                        return Json(new { success = true, message = "Review submitted successfully" });
                    }

                    return Json(new { success = false, message = result.Message });
                }
                catch
                {
                    return Json(new { success = false, message = "Internal error, please try again later" });
                }
            }
        }
        public async Task<ActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Catalog");
            }

            using (var service = new ProductService())
            {
                try
                {
                    // 1. Obtener promociones activas (siempre se muestran)
                    var promotions = await service.GetActivePromotions();

                    // 2. Obtener categorías (para el dropdown)
                    var categories = await service.GetActiveCategories();
                    ViewBag.AvailableCategories = categories?
                        .Where(c => c.Response == 1)
                        .Select(c => c.CatName)
                        .ToList();

                    // 3. Realizar búsqueda
                    var searchResults = await service.SearchProducts(searchTerm);

                    // Crear respuesta
                    var catalogResponse = new CatalogResponseDto
                    {
                        ActivePromotions = promotions, // Mismas promociones que en Catalog
                        Products = searchResults.Products, // Productos encontrados
                        ProductsByCategory = new Dictionary<string, List<ProductDto>>
                        {
                            { "Resultados de Búsqueda", searchResults.Products }
                        },
                        Response = searchResults.Response,
                        Message = searchResults.Message
                    };

                    if (searchResults.Response != 1)
                    {
                        ViewBag.Error = searchResults.Message;
                    }

                    return View("Catalog", catalogResponse);
                }
                catch
                {
                    return View("Error");
                }
            }
        }
    }
}