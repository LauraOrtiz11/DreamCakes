﻿using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamCakes.Services.Client
{
    public class ProductService : IDisposable
    {
        private readonly ProductRepository _repository;

        public ProductService()
        {
            _repository = new ProductRepository();
        }

        public async Task<CatalogResponseDto> GetCatalogData(string category = null)
        {
            try
            {
                var result = await _repository.GetCatalogData(category);

                // Aseguramos que nunca retorne nulos las listas importantes
                result.ActivePromotions = result.ActivePromotions ?? new List<PromotionDto>();
                result.Products = result.Products ?? new List<ProductDto>();
                result.ProductsByCategory = result.ProductsByCategory ?? new Dictionary<string, List<ProductDto>>();

                return result;
            }
            catch (Exception ex)
            {
                return new CatalogResponseDto
                {
                    Response = -1,
                    Message = $"Service error: {ex.Message}",
                    ActivePromotions = new List<PromotionDto>(),
                    Products = new List<ProductDto>(),
                    ProductsByCategory = new Dictionary<string, List<ProductDto>>()
                };
            }
        }

        public async Task<List<PromotionDto>> GetActivePromotions()
        {
            try
            {
                var catalogData = await _repository.GetCatalogData(null);
                return catalogData.ActivePromotions ?? new List<PromotionDto>();
            }
            catch (Exception ex)
            {
                return new List<PromotionDto>
                {
                    new PromotionDto { Response = -1, Message = $"Error getting promotions: {ex.Message}" }
                };
            }
        }

        public async Task<ProductsByCategoryDto> GetProductsByCategory(string category = null)
        {
            try
            {
                var result = await _repository.GetCatalogData(category);

                return new ProductsByCategoryDto
                {
                    Products = result.Products ?? new List<ProductDto>(),
                    ProductsByCategory = result.ProductsByCategory ?? new Dictionary<string, List<ProductDto>>(),
                    Response = result.Response,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ProductsByCategoryDto
                {
                    Response = -1,
                    Message = $"Error getting products: {ex.Message}",
                    Products = new List<ProductDto>(),
                    ProductsByCategory = new Dictionary<string, List<ProductDto>>()
                };
            }
        }

        public async Task<SearchResponseDto> SearchProducts(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new SearchResponseDto
                    {
                        Response = 0,
                        Message = "Por favor, introduzca un término de búsqueda",
                        Products = new List<ProductDto>(),
                        CurrentPromotion = null
                    };
                }

                var result = await _repository.SearchProducts(searchTerm);

                // Aseguramos que nunca retorne nulos
                result.Products = result.Products ?? new List<ProductDto>();

                return result;
            }
            catch (Exception ex)
            {
                return new SearchResponseDto
                {
                    Response = -1,
                    Message = $"Error en el servicio: {ex.Message}",
                    Products = new List<ProductDto>()
                };
            }
        }

        public async Task<List<CategoryDto>> GetActiveCategories()
        {
            try
            {
                return await _repository.GetActiveCategories() ?? new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                return new List<CategoryDto>
                {
                    new CategoryDto { Response = -1, Message = $"Error: {ex.Message}" }
                };
            }
        }
        public async Task<ProductDto> GetProductWithReviews(int productId)
        {
            try
            {
                var catalogData = await _repository.GetCatalogData();
                var product = catalogData.Products.FirstOrDefault(p => p.ID_Product == productId);

                if (product == null)
                {
                    return new ProductDto { Response = 0, Message = "Product not found" };
                }

                // Obtener reseñas y promedio
                var reviewsData = await _repository.GetProductReviewsWithAverage(productId);
                product.Reviews = reviewsData.Reviews;
                product.AvgRating = reviewsData.AverageRating; // <- esta línea es la clave
                product.TotalReviews = reviewsData.TotalReviews;

                

                return product;
            }
            catch (Exception ex)
            {
                return new ProductDto { Response = -1, Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<ReviewDto> SubmitProductReview(ReviewRequestDto request, int clientId)
        {
            try
            {
                if (request.Rating < 1 || request.Rating > 5)
                {
                    return new ReviewDto { Response = 0, Message = "Rating must be between 1 and 5" };
                }

                
                

                return await _repository.SubmitReview(request.ProductID, clientId, request.Rating, request.Comment);
            }
            catch (Exception ex)
            {
                return new ReviewDto { Response = -1, Message = $"Error: {ex.Message}" };
            }
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}