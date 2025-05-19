using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Admin;
using System.Web;
using System.Collections.Generic;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services.Admin
{
    public class AdminProductService
    {
        private readonly AdminProductRepository _productRepository;
        private readonly AdminImageService _imageService;

        public AdminProductService()
        {
            _productRepository = new AdminProductRepository();
            _imageService = new AdminImageService();
        }

        // Obtiene la lista de todos los productos disponibles con stock.
        public List<AdminProductDto> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        // Obtiene un producto por su identificador.
        public AdminProductDto GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }
        public AdminProductPromotionDto GetProductPromotionInfo(int productId)
        {
            return new AdminProductPromotionDto
            {
                ProductId = productId,
                AvailablePromotions = _productRepository.GetActivePromotions(),
                CurrentPromotions = _productRepository.GetPromotionsForProduct(productId)
            };
        }

        public bool RemovePromotionFromProduct(int productId, int promotionId)
        {
            try
            {
                return _productRepository.RemoveProductFromPromotion(productId, promotionId);
            }
            catch
            {
                // Logear error si es necesario
                return false;
            }
        }
        public bool AddPromotionToProduct(int productId, int promotionId)
        {
  
            return _productRepository.AddProductToPromotion(productId, promotionId);
        }
        // Crea un nuevo producto sin imágenes asociadas.
        public int CreateProductWithoutImages(AdminProductDto productDto)
        {
            using (var transaction = _productRepository.BeginTransaction())
            {
                try
                {
                    var productId = _productRepository.CreateProductWithoutImages(productDto);
                    if (productId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    transaction.Commit();
                    return productId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error al crear producto: {ex}");
                    return 0;
                }
            }
        }

        // Actualiza los datos de un producto existente y opcionalmente guarda nuevas imágenes.
        public bool UpdateProduct(AdminProductDto productDto, IEnumerable<HttpPostedFileBase> newImages = null)
        {
            var newImageUrls = newImages != null ? _imageService.SaveUploadedImages(newImages) : new List<string>();
            return _productRepository.UpdateProduct(productDto, newImageUrls);
        }

        // Elimina un producto y sus imágenes asociadas tanto en la base de datos como en el sistema de archivos.
        public bool DeleteProduct(int productId)
        {
            var product = _productRepository.GetProductByIdForDeletion(productId);
            if (product == null)
                return false;

            foreach (var image in product.Images)
            {
                FileHelperUtility.DeleteFile(image.ImgUrl);
            }

            return _productRepository.DeleteProduct(productId);
        }
    }
}