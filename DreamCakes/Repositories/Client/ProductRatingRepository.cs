using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DreamCakes.Repositories.Client
{
    public class ProductRatingRepository : IDisposable
    {
        private readonly DreamCakesEntities _context;

        public ProductRatingRepository()
        {
            _context = new DreamCakesEntities();
        }

        public async Task<ProductDetailsDto> GetProductWithRatingsAsync(int productId)
        {
            var response = new ProductDetailsDto();

            try
            {
                var product = await _context.PRODUCTOes
                    .Include(p => p.IMAGENs)
                    .Include(p => p.VALORACIONs.Select(v => v.USUARIO))
                    .FirstOrDefaultAsync(p => p.ID_Producto == productId);

                if (product == null)
                {
                    response.Response = 0;
                    response.Message = "Producto no encontrado";
                    return response;
                }

                response.ID_Product = product.ID_Producto;
                response.Name = product.Nombre;
                response.Description = product.Descripcion;
                response.Price = product.Precio;
                response.Stock = product.Stock;
                response.AvgRating = product.PromPuntuacion ?? 0;
                response.Images = product.IMAGENs.Select(i => new ImageDto
                {
                    ID_Image = i.ID_Imagen,
                    ImgUrl = i.Imagen_URL,
                    Response = 1,
                    Message = string.Empty
                }).ToList();

                response.Ratings = product.VALORACIONs.Select(v => new ProductRatingDisplayDto
                {
                    ClientName = $"{v.USUARIO.Nombres} {v.USUARIO.Apellidos}",
                    Rating = v.Puntuacion,
                    Comment = v.Comentario,
                    CreatedDate = v.Fecha_Creacion,
                    Response = 1,
                    Message = string.Empty
                }).OrderByDescending(r => r.CreatedDate).ToList();

                response.Response = 1;
                response.Message = "Producto obtenido correctamente";
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = $"Error en repositorio: {ex.Message}";
            }

            return response;
        }

        public async Task<RatingResponseDto> SubmitRatingAsync(ProductRatingDto ratingDto)
        {
            var response = new RatingResponseDto();

            try
            {
                var conn = _context.Database.Connection;
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_InsertProductRating";

                    cmd.Parameters.Add(new SqlParameter("@ProductID", ratingDto.ProductId));
                    cmd.Parameters.Add(new SqlParameter("@ClientID", ratingDto.UsuarioID));
                    cmd.Parameters.Add(new SqlParameter("@Rating", ratingDto.Rating));
                    cmd.Parameters.Add(new SqlParameter("@Comment",
                        string.IsNullOrEmpty(ratingDto.Comment) ? (object)DBNull.Value : ratingDto.Comment));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.Success = reader.GetBoolean(0);
                            response.Message = reader.GetString(1);
                            response.Response = response.Success ? 1 : 0;

                            // Obtener el nuevo promedio
                            if (response.Success)
                            {
                                response.NewAverageRating = await _context.PRODUCTOes
                                    .Where(p => p.ID_Producto == ratingDto.ProductId)
                                    .Select(p => p.PromPuntuacion ?? 0)
                                    .FirstOrDefaultAsync();
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                response.Response = -1;
                response.Message = $"Error de base de datos: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                _context.Database.Connection.Close();
            }

            return response;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}