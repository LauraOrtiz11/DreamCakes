using DreamCakes.Services;
using DreamCakes.Dtos;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using DreamCakes.Utilities;
using System.Linq;

namespace DreamCakes.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ImageService _imageService;
        private readonly CategoryService _categoryService;

        public ProductController()
        {
            _productService = new ProductService();
            _imageService = new ImageService();
            _categoryService = new CategoryService();
        }

        public ActionResult Index()
        {
            try
            {
                var products = _productService.GetAllProducts();
                return View(products);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error al cargar el catálogo de productos";
                return View(new List<ProductDto>());
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null) return HttpNotFound();
                return View(product);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al cargar los detalles del producto";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Create()
        {
            try
            {
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                return View();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario de creación";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductDto productDto, IEnumerable<HttpPostedFileBase> productImages)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                // Validar que haya al menos una imagen
                if (productImages == null || !productImages.Any(f => f != null && f.ContentLength > 0))
                {
                    ViewBag.ErrorMessage = "Debe subir al menos una imagen para el producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                // Crear producto
                var productId = _productService.CreateProductWithoutImages(productDto);
                if (productId <= 0)
                {
                    ViewBag.ErrorMessage = "Error al crear el producto en la base de datos";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                // Procesar imágenes
                var savedImageUrls = _imageService.SaveUploadedImages(productImages);
                if (savedImageUrls.Count == 0)
                {
                    // Si falla al guardar imágenes, eliminar el producto creado
                    _productService.DeleteProduct(productId);
                    ViewBag.ErrorMessage = "Error al guardar las imágenes del producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                // Asociar imágenes al producto
                if (!_imageService.AddImagesToProduct(productId, savedImageUrls))
                {
                    // Si falla al asociar imágenes, eliminar el producto y las imágenes
                    _productService.DeleteProduct(productId);
                    foreach (var url in savedImageUrls)
                    {
                        FileHelper.DeleteFile(url);
                    }
                    ViewBag.ErrorMessage = "Error al asociar las imágenes al producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                TempData["SuccessMessage"] = "Producto creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error al crear producto: {ex}");
                ViewBag.ErrorMessage = "Ocurrió un error inesperado al crear el producto";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                return View(productDto);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null) return HttpNotFound();

                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                return View(product);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error loading edit form";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductDto productDto, IEnumerable<HttpPostedFileBase> newImages)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                    return View(productDto);
                }

                var result = _productService.UpdateProduct(productDto, newImages);
                if (result)
                {
                    TempData["SuccessMessage"] = "Product updated successfully";
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Error updating product";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                return View(productDto);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Unexpected error updating product";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "CategoryId", "Name");
                return View(productDto);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var result = _productService.DeleteProduct(id);
                return Json(new
                {
                    success = result,
                    message = result ? "Product deleted successfully" : "Could not delete product"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    error = "Error deleting product"
                });
            }
        }

        [HttpPost]
        public ActionResult DeleteImage(int productId, string imageUrl)
        {
            try
            {
                var result = _imageService.DeleteImage(productId, imageUrl);
                return Json(new { success = result });
            }
            catch (Exception)
            {
                return Json(new { success = false, error = "Error deleting image" });
            }
        }
    }
}