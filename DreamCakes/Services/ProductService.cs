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

        public List<ProductDto> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public ProductDto GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

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

        public bool UpdateProduct(ProductDto productDto, IEnumerable<HttpPostedFileBase> newImages = null)
        {
            var newImageUrls = newImages != null ? _imageService.SaveUploadedImages(newImages) : new List<string>();
            return _productRepository.UpdateProduct(productDto, newImageUrls);
        }

        public bool DeleteProduct(int productId)
        {
            var product = _productRepository.GetProductByIdForDeletion(productId);
            if (product == null)
                return false;

            foreach (var image in product.Images)
            {
                FileHelper.DeleteFile(image.Url);
            }

            return _productRepository.DeleteProduct(productId);
        }
    }
}