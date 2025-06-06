﻿using DreamCakes.Services.Admin;
using DreamCakes.Dtos.Admin;
using DreamCakes.Dtos;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using DreamCakes.Utilities;
using System.Linq;
using System.Diagnostics;


namespace DreamCakes.Controllers.Admin
{
    [RoleAuthorizeUtility(1)]
    public class AdminProductController : Controller
    {
        private readonly AdminProductService _productService;
        private readonly AdminImageService _imageService;
        private readonly AdminCategoryService _categoryService;

        // Inicializa los servicios necesarios para la gestión de productos, imágenes y categorías.
        public AdminProductController()
        {
            _productService = new AdminProductService();
            _imageService = new AdminImageService();
            _categoryService = new AdminCategoryService();
        }

        // Muestra la lista de todos los productos disponibles.
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
                return View(new List<AdminProductDto>());
            }
        }

        // Muestra los detalles de un producto específico por su ID.
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

        // Muestra el formulario para crear un nuevo producto.
        public ActionResult Create()
        {
            try
            {
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                return View();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario de creación";
                return RedirectToAction("Index");
            }
        }

        // Procesa la creación de un nuevo producto con validación de datos e imágenes.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminProductDto productDto, IEnumerable<HttpPostedFileBase> productImages)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                // Validar que haya al menos una imagen
                if (productImages == null || !productImages.Any(f => f != null && f.ContentLength > 0))
                {
                    ViewBag.ErrorMessage = "Debe subir al menos una imagen para el producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                // Crear producto
                var productId = _productService.CreateProductWithoutImages(productDto);
                if (productId <= 0)
                {
                    ViewBag.ErrorMessage = "Error al crear el producto en la base de datos";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                // Procesar imágenes
                var savedImageUrls = _imageService.SaveUploadedImages(productImages);
                if (savedImageUrls.Count == 0)
                {
                    // Si falla al guardar imágenes, eliminar el producto creado
                    _productService.DeleteProduct(productId);
                    ViewBag.ErrorMessage = "Error al guardar las imágenes del producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                // Asociar imágenes al producto
                if (!_imageService.AddImagesToProduct(productId, savedImageUrls))
                {
                    // Si falla al asociar imágenes, eliminar el producto y las imágenes
                    _productService.DeleteProduct(productId);
                    foreach (var url in savedImageUrls)
                    {
                        FileHelperUtility.DeleteFile(url);
                    }
                    ViewBag.ErrorMessage = "Error al asociar las imágenes al producto";
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                TempData["SuccessMessage"] = "Producto creado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear producto: {ex}");
                ViewBag.ErrorMessage = "Ocurrió un error inesperado al crear el producto";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                return View(productDto);
            }
        }

        // Muestra el formulario para editar un producto existente.
        public ActionResult Edit(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null) return HttpNotFound();
                // Cargar promociones disponibles y aplicadas
                product.ProductPromotion = _productService.GetProductPromotionInfo(id);
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                return View(product);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error loading edit form";
                return RedirectToAction("Index");
            }
        }

        // Procesa la edición de un producto, incluyendo la posible adición de nuevas imágenes.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminProductDto productDto, IEnumerable<HttpPostedFileBase> newImages)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                    return View(productDto);
                }

                var result = _productService.UpdateProduct(productDto, newImages);
                if (result)
                {
                    TempData["SuccessMessage"] = "Producto actualizado correctamente";
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Error al actualizar el producto";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                return View(productDto);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error intente más tarde";
                ViewBag.Categories = new SelectList(_categoryService.GetActiveCategories(), "ID_Category", "CatName ");
                return View(productDto);
            }
        }

        // Elimina un producto según su ID. Retorna el resultado como JSON.
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
        public ActionResult AddPromotion(AdminProductPromotionDto model)
        {
            try
            {

                var success = _productService.AddPromotionToProduct(model.ProductId, model.PromotionId);

                if (success)
                {
                    TempData["SuccessMessage"] = "La promoción se ha vinculado al producto exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "La promoción ya estaba vinculada a este producto o ocurrió un error.";
                }

                return RedirectToAction("Edit", new { id = model.ProductId });
            }
            catch (Exception)
            {
                // Log error
                TempData["ErrorMessage"] = "Ocurrió un error al vincular la promoción. Por favor intente nuevamente.";
                return RedirectToAction("Edit", new { id = model.ProductId });
            }
        }
        // Método para mostrar las promociones vinculadas (parcial)
        public ActionResult GetPromotionsPartial(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null) return HttpNotFound();

                product.ProductPromotion = _productService.GetProductPromotionInfo(id);
                return PartialView("_ProductPromotionsPartial", product);
            }
            catch (Exception ex)
            {
                return PartialView("_ErrorPartial", "Error al cargar promociones");
            }
        }

        // Elimina una imagen específica asociada a un producto. Retorna el resultado como JSON.
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

        [HttpPost]
        public JsonResult RemovePromotion(int productId, int promotionId)
        {
            try
            {
                
                var result = _productService.RemovePromotionFromProduct(productId, promotionId);

                if (result)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "La relación no existe o ya fue eliminada." });
            }
            catch (Exception ex)
            {
                // Log error (ex)
                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error al eliminar la promoción."
                });
            }
        }
    }
}