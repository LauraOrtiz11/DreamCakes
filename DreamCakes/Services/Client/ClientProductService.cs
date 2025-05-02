using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Client;
using System;
using System.Collections.Generic;

namespace DreamCakes.Services.Client
{
    public class ClientProductService : IDisposable
    {
        private readonly ClientProductRepository _productRepository;

        public ClientProductService()
        {
            _productRepository = new ClientProductRepository();
        }

        public List<ProductSearchResultDto> SearchProducts(ProductSearchCriteriaDto criteria)
        {
            try
            {
                return _productRepository.AdvancedSearch(criteria);
            }
            catch (Exception ex)
            {
                // Log error (implementar sistema de logging)
                Console.WriteLine($"Error searching products: {ex.Message}");
                return new List<ProductSearchResultDto>();
            }
        }

        public ClientProductDetailDto GetProductDetails(int productId)
        {
            try
            {
                return _productRepository.GetProductDetails(productId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product details: {ex.Message}");
                return null;
            }
        }

        public RatingResultDto SubmitRating(SubmitRatingDto ratingDto)
        {
            try
            {
                if (ratingDto.Rating < 1 || ratingDto.Rating > 5)
                {
                    return new RatingResultDto
                    {
                        Success = false,
                        Message = "Rating must be between 1 and 5"
                    };
                }

                return _productRepository.SubmitRating(ratingDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting rating: {ex.Message}");
                return new RatingResultDto
                {
                    Success = false,
                    Message = "An error occurred while submitting your rating"
                };
            }
        }

        public void Dispose()
        {
            _productRepository?.Dispose();
        }
    }
}