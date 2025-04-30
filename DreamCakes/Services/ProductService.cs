using DreamCakes.Dtos;
using DreamCakes.Repositories;
using System.Web;
using System.Collections.Generic;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ImageService _imageService;

        public ProductService()
        {
            _productRepository = new ProductRepository();
            _imageService = new ImageService();
        }

        // Obtiene la lista de todos los productos disponibles con stock.
        public List<ProductDto> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        // Obtiene un producto por su identificador.
        public ProductDto GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

        // Crea un nuevo producto sin imágenes asociadas.
        public int CreateProductWithoutImages(ProductDto productDto)
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
        public bool UpdateProduct(ProductDto productDto, IEnumerable<HttpPostedFileBase> newImages = null)
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
                FileHelperUtility.DeleteFile(image.Url);
            }

            return _productRepository.DeleteProduct(productId);
        }
    }
}