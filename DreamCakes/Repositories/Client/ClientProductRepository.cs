using DreamCakes.Dtos.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DreamCakes.Repositories.Models;

namespace DreamCakes.Repositories.Client
{
    public class ClientProductRepository : IDisposable
    {
        private readonly DreamCakesEntities _context;

        public ClientProductRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<ProductSearchResultDto> AdvancedSearch(ProductSearchCriteriaDto criteria)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoryID", criteria.CategoryID ?? (object)DBNull.Value),
                new SqlParameter("@MinPrice", criteria.MinPrice ?? (object)DBNull.Value),
                new SqlParameter("@MaxPrice", criteria.MaxPrice ?? (object)DBNull.Value),
                new SqlParameter("@ProductName", string.IsNullOrEmpty(criteria.ProductName) ? (object)DBNull.Value : criteria.ProductName),
                new SqlParameter("@HasStock", criteria.InStock),
                new SqlParameter("@MinRating", criteria.MinRating ?? (object)DBNull.Value),
                new SqlParameter("@SortBy", criteria.SortBy ?? "Rating"),
                new SqlParameter("@SortDirection", criteria.SortDirection ?? "DESC")
            };

            return _context.Database.SqlQuery<ProductSearchResultDto>(
                "EXEC sp_AdvancedProductSearch @CategoryID, @MinPrice, @MaxPrice, @ProductName, @HasStock, @MinRating, @SortBy, @SortDirection",
                parameters).ToList();
        }

        public ClientProductDetailDto GetProductDetails(int productId)
        {
            var product = _context.PRODUCTOes
                .Include(p => p.CATEGORIA)
                .Include(p => p.IMAGENs)
                .Include(p => p.VALORACIONs)
                .Select(p => new ClientProductDetailDto
                {
                    ProductID = p.ID_Producto,
                    ProductName = p.Nombre,
                    ProductDescription = p.Descripcion,
                    ProductPrice = p.Precio,
                    ProductStock = p.Stock,
                    ProductRating = p.PromPuntuacion ?? 0,
                    CategoryName = p.CATEGORIA.Nom_Categ,
                    Images = p.IMAGENs.Select(i => new ProductImageDto
                    {
                        ImageID = i.ID_Imagen,
                        ImageUrl = i.Imagen_URL,
                        ImageName = i.Nombre_Img
                    }).ToList(),
                    Ratings = p.VALORACIONs.Select(r => new ProductRatingDto
                    {
                        RatingID = r.ID_Valoracion,
                        ClientName = r.USUARIO.Nombres + " " + r.USUARIO.Apellidos,
                        Rating = r.Puntuacion,
                        Comment = r.Comentario,
                        CreatedDate = r.Fecha_Creacion
                    }).OrderByDescending(r => r.CreatedDate).ToList()
                })
                .FirstOrDefault(p => p.ProductID == productId);

            return product;
        }

        public RatingResultDto SubmitRating(SubmitRatingDto rating)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductID", rating.ProductID),
                new SqlParameter("@ClientID", rating.ClientID),
                new SqlParameter("@Rating", rating.Rating),
                new SqlParameter("@Comment", string.IsNullOrEmpty(rating.Comment) ? (object)DBNull.Value : rating.Comment)
            };

            return _context.Database.SqlQuery<RatingResultDto>(
                "EXEC sp_InsertProductRating @ProductID, @ClientID, @Rating, @Comment",
                parameters).FirstOrDefault();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}