using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Client;
using System;
using System.Threading.Tasks;

namespace DreamCakes.Services.Client
{
    public class ProductRatingService : IDisposable
    {
        private readonly ProductRatingRepository _repository;

        public ProductRatingService()
        {
            _repository = new ProductRatingRepository();
        }

        public async Task<ProductDetailsDto> GetProductDetailsAsync(int productId)
        {
            var response = new ProductDetailsDto();

            try
            {
                // Validación básica del ID
                if (productId <= 0)
                {
                    response.Response = 0;
                    response.Message = "ID de producto no válido";
                    return response;
                }

                var product = await _repository.GetProductWithRatingsAsync(productId);

                if (product == null || product.Response != 1)
                {
                    response.Response = product?.Response ?? 0;
                    response.Message = product?.Message ?? "Producto no encontrado";
                    return response;
                }

                return product;
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = $"Error en servicio: {ex.Message}";
                return response;
            }
        }

        public async Task<RatingResponseDto> SubmitRatingAsync(ProductRatingDto ratingDto)
        {
            var response = new RatingResponseDto();

            try
            {
                // Validación básica
                if (ratingDto.UsuarioID <= 0)
                {
                    response.Response = 0;
                    response.Message = "ID de usuario no válido";
                    return response;
                }

                if (ratingDto.Rating < 1 || ratingDto.Rating > 5)
                {
                    response.Response = 0;
                    response.Message = "La valoración debe ser entre 1 y 5 estrellas";
                    return response;
                }

                // Validación de longitud de comentario
                if (!string.IsNullOrEmpty(ratingDto.Comment) && ratingDto.Comment.Length > 500)
                {
                    response.Response = 0;
                    response.Message = "El comentario no puede exceder 500 caracteres";
                    return response;
                }

                // Procesar con el repositorio
                var result = await _repository.SubmitRatingAsync(ratingDto);

                return result;
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = $"Error al procesar valoración: {ex.Message}";
                return response;
            }
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}