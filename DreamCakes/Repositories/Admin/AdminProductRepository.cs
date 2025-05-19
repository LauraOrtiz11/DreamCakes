using DreamCakes.Dtos.Admin;
using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DreamCakes.Repositories.Admin
{
    public class AdminProductRepository
    {
        private readonly DreamCakesEntities _context;
        private readonly AdminImageRepository _imageRepository;

        public AdminProductRepository()
        {
            _context = new DreamCakesEntities();
            _imageRepository = new AdminImageRepository();
        }

        public List<PromotionDto> GetActivePromotions()
        {
            var promotions = new List<PromotionDto>();

            using (var context = new DreamCakesEntities())
            {
                var result = context.Database.SqlQuery<PromotionDto>(
                    "EXEC sp_GetActivePromotions");

                promotions = result.ToList();
            }

            return promotions;
        }

        public List<PromotionDto> GetPromotionsForProduct(int productId)
        {
            var promotions = new List<PromotionDto>();

            using (var context = new DreamCakesEntities())
            {
                promotions = context.PROMOCION_PRODUCTO
                    .Where(pp => pp.ID_Producto == productId)
                    .Join(context.PROMOCIONs,
                        pp => pp.ID_Promocion,
                        p => p.ID_Promocion,
                        (pp, p) => new PromotionDto
                        {
                            ID_Prom = p.ID_Promocion,
                            NameProm = p.Nombre_Prom,
                            DiscountPer = p.Porc_Desc,
                            StartDate = p.Fecha_Ini,
                            EndDate = p.Fecha_Fin,
                            StateProm = p.Estado,
                            DescriProm = p.Descrip_Prom
                        })
                    .ToList();
            }

            return promotions;
        }

        public bool AddProductToPromotion(int productId, int promotionId)
        {
            try
            {
                using (var context = new DreamCakesEntities())
                {
                    // Check if relationship already exists
                    var exists = context.PROMOCION_PRODUCTO
                        .Any(pp => pp.ID_Producto == productId && pp.ID_Promocion == promotionId);

                    if (!exists)
                    {
                        var newRelation = new PROMOCION_PRODUCTO
                        {
                            ID_Producto = productId,
                            ID_Promocion = promotionId
                        };

                        context.PROMOCION_PRODUCTO.Add(newRelation);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        // Inicia una transacción de base de datos manualmente.
        public DbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        // Obtiene todos los productos con stock disponible, incluyendo sus imágenes y nombre de categoría.
        public List<AdminProductDto> GetAllProducts()
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Include(p => p.CATEGORIA)
                .Where(p => p.Stock > 0)
                .Select(p => new AdminProductDto
                {
                    ID_Product = p.ID_Producto,
                    ID_Category = p.ID_Categoria,
                    ProdName = p.Nombre,
                    ProdDescription = p.Descripcion,
                    ProdPrice = p.Precio,
                    ProdStock = p.Stock,
                    Images = p.IMAGENs.Select(i => new AdminImageDto
                    {
                        ImgName = i.Nombre_Img,
                        ImgUrl = i.Imagen_URL
                    }).ToList(),
                    CategoryName = p.CATEGORIA.Nom_Categ
                }).ToList();
        }

        // Obtiene un producto específico por su ID, incluyendo imágenes y categoría.
        public AdminProductDto GetProductById(int productId)
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Include(p => p.CATEGORIA)
                .Where(p => p.ID_Producto == productId)
                .Select(p => new AdminProductDto
                {
                    ID_Product = p.ID_Producto,
                    ID_Category = p.ID_Categoria,
                    ProdName = p.Nombre,
                    ProdDescription = p.Descripcion,
                    ProdPrice = p.Precio,
                    ProdStock = p.Stock,
                    Images = p.IMAGENs.Select(i => new AdminImageDto
                    {
                        ImgName = i.Nombre_Img,
                        ImgUrl = i.Imagen_URL
                    }).ToList(),
                    CategoryName = p.CATEGORIA.Nom_Categ
                }).FirstOrDefault();
        }

        public bool RemoveProductFromPromotion(int productId, int promotionId)
        {
            
                var relation = _context.PROMOCION_PRODUCTO
                    .FirstOrDefault(pp => pp.ID_Producto == productId && pp.ID_Promocion == promotionId);

                if (relation == null) return false;

                _context.PROMOCION_PRODUCTO.Remove(relation);
                return _context.SaveChanges() > 0;
            
        }
        // Obtiene un producto con sus imágenes para fines de eliminación.
        public AdminProductDto GetProductByIdForDeletion(int productId)
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Where(p => p.ID_Producto == productId)
                .Select(p => new AdminProductDto
                {
                    ID_Product = p.ID_Producto,
                    Images = p.IMAGENs.Select(i => new AdminImageDto
                    {
                        ImgUrl = i.Imagen_URL
                    }).ToList()
                }).FirstOrDefault();
        }

        // Crea un nuevo producto en la base de datos sin imágenes.
        public int CreateProductWithoutImages(AdminProductDto productDto)
        {
            try
            {
                var product = new PRODUCTO
                {
                    ID_Categoria = productDto.ID_Category,
                    Nombre = productDto.ProdName,
                    Descripcion = productDto.ProdDescription,
                    Precio = productDto.ProdPrice,
                    Stock = productDto.ProdStock
                };

                _context.PRODUCTOes.Add(product);
                _context.SaveChanges();

                if (product.ID_Producto <= 0)
                {
                    throw new Exception("No se generó el ID del producto automáticamente");
                }

                return product.ID_Producto;
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Error en repositorio: {ex}");
                return 0;
            }
        }

        // Actualiza un producto existente, incluyendo la adición de nuevas imágenes.
        public bool UpdateProduct(AdminProductDto productDto, List<string> newImageUrls)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var product = _context.PRODUCTOes.Find(productDto.ID_Product);
                    if (product == null) return false;

                    product.ID_Categoria = productDto.ID_Category;
                    product.Nombre = productDto.ProdName;
                    product.Descripcion = productDto.ProdDescription;
                    product.Precio = productDto.ProdPrice;
                    product.Stock = productDto.ProdStock;

                    if (newImageUrls != null && newImageUrls.Count > 0)
                    {
                        var images = newImageUrls.Select(url => new AdminImageDto
                        {
                            ImgName = System.IO.Path.GetFileName(url),
                            ImgUrl = url
                        }).ToList();

                        if (!_imageRepository.AddImagesToProduct(productDto.ID_Product, images))
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Obtener el producto con sus imágenes
                    var product = _context.PRODUCTOes
                        .Include(p => p.IMAGENs)
                        .FirstOrDefault(p => p.ID_Producto == productId);

                    if (product == null) return false;

                    // Eliminar imágenes
                    if (product.IMAGENs.Any())
                    {
                        _context.IMAGENs.RemoveRange(product.IMAGENs);
                    }

                    // Eliminar relaciones con promociones
                    var promoRelations = _context.PROMOCION_PRODUCTO
                        .Where(pp => pp.ID_Producto == productId)
                        .ToList();
                    if (promoRelations.Any())
                    {
                        _context.PROMOCION_PRODUCTO.RemoveRange(promoRelations);
                    }

                    // Eliminar valoraciones del producto
                    var valoraciones = _context.VALORACIONs
                        .Where(v => v.ID_Producto == productId)
                        .ToList();
                    if (valoraciones.Any())
                    {
                        _context.VALORACIONs.RemoveRange(valoraciones);
                    }

                    // Eliminar el producto
                    _context.PRODUCTOes.Remove(product);

                    // Guardar cambios
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Puedes registrar el error si lo deseas
                    return false;
                }
            }
        }

    }
}