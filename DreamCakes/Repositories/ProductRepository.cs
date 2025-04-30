using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class ProductRepository
    {
        private readonly DreamCakesEntities _context;
        private readonly ImageRepository _imageRepository;

        public ProductRepository()
        {
            _context = new DreamCakesEntities();
            _imageRepository = new ImageRepository(_context);
        }

        // Inicia una transacción de base de datos manualmente.
        public DbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        // Obtiene todos los productos con stock disponible, incluyendo sus imágenes y nombre de categoría.
        public List<ProductDto> GetAllProducts()
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Include(p => p.CATEGORIA)
                .Where(p => p.Stock > 0)
                .Select(p => new ProductDto
                {
                    ProductId = p.ID_Producto,
                    CategoryId = p.ID_Categoria,
                    Name = p.Nombre,
                    Description = p.Descripcion,
                    Price = p.Precio,
                    Stock = p.Stock,
                    Images = p.IMAGENs.Select(i => new ImageDto
                    {
                        Name = i.Nombre_Img,
                        Url = i.Imagen_URL
                    }).ToList(),
                    CategoryName = p.CATEGORIA.Nom_Categ
                }).ToList();
        }

        // Obtiene un producto específico por su ID, incluyendo imágenes y categoría.
        public ProductDto GetProductById(int productId)
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Include(p => p.CATEGORIA)
                .Where(p => p.ID_Producto == productId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ID_Producto,
                    CategoryId = p.ID_Categoria,
                    Name = p.Nombre,
                    Description = p.Descripcion,
                    Price = p.Precio,
                    Stock = p.Stock,
                    Images = p.IMAGENs.Select(i => new ImageDto
                    {
                        Name = i.Nombre_Img,
                        Url = i.Imagen_URL
                    }).ToList(),
                    CategoryName = p.CATEGORIA.Nom_Categ
                }).FirstOrDefault();
        }

        // Obtiene un producto con sus imágenes para fines de eliminación.
        public ProductDto GetProductByIdForDeletion(int productId)
        {
            return _context.PRODUCTOes
                .Include(p => p.IMAGENs)
                .Where(p => p.ID_Producto == productId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ID_Producto,
                    Images = p.IMAGENs.Select(i => new ImageDto
                    {
                        Url = i.Imagen_URL
                    }).ToList()
                }).FirstOrDefault();
        }

        // Crea un nuevo producto en la base de datos sin imágenes.
        public int CreateProductWithoutImages(ProductDto productDto)
        {
            try
            {
                var product = new PRODUCTO
                {
                    ID_Categoria = productDto.CategoryId,
                    Nombre = productDto.Name,
                    Descripcion = productDto.Description,
                    Precio = productDto.Price,
                    Stock = productDto.Stock
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
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error en repositorio: {ex.ToString()}");
                return 0;
            }
        }

        // Actualiza un producto existente, incluyendo la adición de nuevas imágenes.
        public bool UpdateProduct(ProductDto productDto, List<string> newImageUrls)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var product = _context.PRODUCTOes.Find(productDto.ProductId);
                    if (product == null) return false;

                    product.ID_Categoria = productDto.CategoryId;
                    product.Nombre = productDto.Name;
                    product.Descripcion = productDto.Description;
                    product.Precio = productDto.Price;
                    product.Stock = productDto.Stock;

                    if (newImageUrls != null && newImageUrls.Count > 0)
                    {
                        var images = newImageUrls.Select(url => new ImageDto
                        {
                            Name = System.IO.Path.GetFileName(url),
                            Url = url
                        }).ToList();

                        if (!_imageRepository.AddImagesToProduct(productDto.ProductId, images))
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

        // Elimina un producto y sus imágenes asociadas de la base de datos.
        public bool DeleteProduct(int productId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var product = _context.PRODUCTOes
                        .Include(p => p.IMAGENs)
                        .FirstOrDefault(p => p.ID_Producto == productId);

                    if (product == null) return false;

                    _context.IMAGENs.RemoveRange(product.IMAGENs);
                    _context.PRODUCTOes.Remove(product);

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
    }
}