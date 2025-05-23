﻿using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DreamCakes.Repositories.Client
{
    public class ProductRepository : IDisposable
    {
        private readonly DreamCakesEntities _context;

        public ProductRepository()
        {
            _context = new DreamCakesEntities();
        }
        public async Task<CatalogResponseDto> GetCatalogData(string categoryFilter = null)
        {
            var response = new CatalogResponseDto
            {
                ActivePromotions = new List<PromotionDto>(),
                Products = new List<ProductDto>(),
                ProductsByCategory = new Dictionary<string, List<ProductDto>>()
            };

            try
            {
                // 1. Obtener promociones activas mediante SP
                response.ActivePromotions = await _context.Database.SqlQuery<PromotionDto>(
                    "EXEC sp_GetActivePromotions"
                ).ToListAsync();

                // 2. Consulta base optimizada para LINQ to Entities
                var query = _context.PRODUCTOes
                    .Include(p => p.CATEGORIA)
                    .Include(p => p.IMAGENs)
                    .Where(p => p.CATEGORIA.Estado);

                // 3. Aplicar filtro de categoría si existe
                if (!string.IsNullOrEmpty(categoryFilter))
                {
                    query = query.Where(p => p.CATEGORIA.Nom_Categ == categoryFilter);
                }

                // 4. Proyección compatible con LINQ to Entities
                var productData = await query
                    .OrderBy(p => p.ID_Producto)
                    .Select(p => new
                    {
                        Producto = p,
                        Categoria = p.CATEGORIA,
                        Imagenes = p.IMAGENs
                    })
                    .ToListAsync();

                // 5. Mapeo en memoria
                response.Products = productData.Select(p => new ProductDto
                {
                    ID_Product = p.Producto.ID_Producto,
                    Name = p.Producto.Nombre,
                    Description = p.Producto.Descripcion,
                    Price = p.Producto.Precio,
                    Stock = p.Producto.Stock,
                    AvgRating = p.Producto.PromPuntuacion ?? 0m, 
                    ID_Category = p.Producto.ID_Categoria,
                    Category = new CategoryDto
                    {
                        ID_Category = p.Categoria.ID_Categoria,
                        CatName = p.Categoria.Nom_Categ,
                        CatDescription = p.Categoria.Descrip_Categ,
                        CatIsActive = p.Categoria.Estado,
                        Response = 1
                    },
                    Images = p.Imagenes.Select(i => new ImageDto
                    {
                        ID_Image = i.ID_Imagen,
                        ID_Product = i.ID_Producto,
                        ImgName = i.Nombre_Img,
                        ImgUrl = i.Imagen_URL,
                        Response = 1
                    }).ToList(),
                    Response = 1
                }).ToList();

                // 6. Agrupar productos por categoría
                if (response.Products.Any())
                {
                    response.ProductsByCategory = response.Products
                        .GroupBy(p => p.Category.CatName)
                        .ToDictionary(g => g.Key, g => g.ToList());
                }

                response.Response = 1;
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = $"Repository error: {ex.Message}";
            }

            return response;
        }


        public async Task<SearchResponseDto> SearchProducts(string searchTerm)
        {
            var response = new SearchResponseDto
            {
                Products = new List<ProductDto>(),
                CurrentPromotion = null,
                Response = -1 // Valor por defecto indica error
            };

            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    response.Response = 0;
                    response.Message = "El término de búsqueda no puede estar vacío";
                    return response;
                }

                // 1. Crear parámetro correctamente
                var searchParam = new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 100)
                {
                    Value = searchTerm
                };

                // 2. Ejecutar SP con parámetro tipado
                var spResults = await _context.Database.SqlQuery<ProductSearchResultDto>(
                    "EXEC sp_SearchProducts @SearchTerm",
                    searchParam
                ).ToListAsync();

                if (!spResults.Any())
                {
                    response.Response = 0;
                    response.Message = "No se encontraron resultados";
                    return response;
                }

                // 3. Obtener IDs de productos para imágenes
                var productIds = spResults.Select(r => r.ID_Producto).Distinct().ToList();

                // 4. Cargar imágenes en una sola consulta
                var imagesDict = await _context.IMAGENs
                    .Where(i => productIds.Contains(i.ID_Producto))
                    .GroupBy(i => i.ID_Producto)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                // 5. Mapear resultados
                response.Products = spResults.Select(r =>
                {
                    var productDto = new ProductDto
                    {
                        ID_Product = r.ID_Producto,
                        Name = r.Nombre,
                        Description = r.Descripcion,
                        Price = r.Precio,
                        Stock = r.Stock,
                        AvgRating = r.AvgRating ?? 0m,
                        ID_Category = r.ID_Categoria,
                        Category = new CategoryDto
                        {
                            ID_Category = r.ID_Categoria,
                            CatName = r.Nom_Categ,
                            CatDescription = r.Descrip_Categ,
                            CatIsActive = r.Estado,
                            Response = 1
                        },
                        Response = 1
                    };

                    // Asignar imágenes si existen
                    if (imagesDict.TryGetValue(r.ID_Producto, out var images))
                    {
                        productDto.Images = images.Select(i => new ImageDto
                        {
                            ID_Image = i.ID_Imagen,
                            ID_Product = i.ID_Producto,
                            ImgName = i.Nombre_Img,
                            ImgUrl = i.Imagen_URL,
                            Response = 1
                        }).ToList();
                    }
                    else
                    {
                        productDto.Images = new List<ImageDto>();
                    }

                    return productDto;
                }).ToList();

                // 6. Obtener promoción activa (opcional)
                try
                {
                    response.CurrentPromotion = await _context.Database.SqlQuery<PromotionDto>(
                        "EXEC sp_GetActivePromotions"
                    ).FirstOrDefaultAsync();
                }
                catch
                {
                    // Si falla, continuar sin promoción
                    response.CurrentPromotion = null;
                }

                response.Response = 1;
                response.Message = "Búsqueda exitosa";
            }
            catch (SqlException sqlEx)
            {
                response.Message = $"Error de base de datos: {sqlEx.Message}";
                // Log adicional para Number y LineNumber
                System.Diagnostics.Debug.WriteLine($"SQL Error #{sqlEx.Number}: {sqlEx.Message} (Line: {sqlEx.LineNumber})");
            }
            catch (Exception ex)
            {
                response.Message = $"Error inesperado: {ex.Message}";
            }

            return response;
        }

        

        public async Task<List<PromotionDto>> GetActivePromotions()
        {
            try
            {
                return await _context.Database.SqlQuery<PromotionDto>(
                    "EXEC sp_GetActivePromotions"
                ).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<PromotionDto>
                {
                    new PromotionDto { Response = -1, Message = $"Error getting promotions: {ex.Message}" }
                };
            }
        }

        public async Task<List<CategoryDto>> GetActiveCategories()
        {
            try
            {
                return await _context.CATEGORIAs
                    .Where(c => c.Estado)
                    .OrderBy(c => c.Nom_Categ)
                    .Select(c => new CategoryDto
                    {
                        ID_Category = c.ID_Categoria,
                        CatName = c.Nom_Categ,
                        CatDescription = c.Descrip_Categ,
                        CatIsActive = c.Estado,
                        Response = 1
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<CategoryDto>
                {
                    new CategoryDto { Response = -1, Message = $"Error: {ex.Message}" }
                };
            }
        }
        public async Task<ProductReviewsResponseDto> GetProductReviewsWithAverage(int productId)
        {
            var response = new ProductReviewsResponseDto();

            try
            {
                using (var command = _context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "sp_GetProductReviews";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ProductID", productId));

                    await _context.Database.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var reviews = new List<ReviewDto>();
                        while (await reader.ReadAsync())
                        {
                            reviews.Add(new ReviewDto
                            {
                                ID_Review = reader.GetInt32(0),
                                ID_Client = reader.GetInt32(1),
                                ClientName = reader.IsDBNull(2) ? "Anónimo" : reader.GetString(2),
                                Rating = reader.GetDecimal(3),
                                Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CreatedDate = reader.GetDateTime(5),
                                Response = reader.GetInt32(6)
                            });
                        }

                        response.Reviews = reviews;

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            response.AverageRating = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                            response.TotalReviews = reader.GetInt32(1);
                        }
                    }
                }

                response.Response = 1;
            }
            catch (Exception ex)
            {
                response.Response = -1;
                response.Message = ex.Message;
            }
            finally
            {
                _context.Database.Connection.Close();
            }

            return response;
        }

        public async Task<ReviewDto> SubmitReview(int productId, int clientId, int rating, string comment)
        {
            try
            {
                var result = await _context.Database.SqlQuery<ReviewDto>(
                    "EXEC sp_InsertProductRating @ProductID, @ClientID, @Rating, @Comment",
                    new System.Data.SqlClient.SqlParameter("@ProductID", productId),
                    new System.Data.SqlClient.SqlParameter("@ClientID", clientId),
                    new System.Data.SqlClient.SqlParameter("@Rating", rating),
                    new System.Data.SqlClient.SqlParameter("@Comment", comment ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync();

                return result ?? new ReviewDto { Response = -1, Message = "No response from database" };
            }
            catch (Exception ex)
            {
                return new ReviewDto { Response = -1, Message = $"Error submitting review: {ex.Message}" };
            }
        }

        
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}