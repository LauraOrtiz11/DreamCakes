using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using DreamCakes.Dtos.Admin;
namespace DreamCakes.Repositories.Admin
{
    public class AdminImageRepository
    {
        private readonly DreamCakesEntities _context;

        public AdminImageRepository()
        {
            _context = new DreamCakesEntities();
        }

        // Agrega una lista de imágenes a un producto específico en la base de datos.
        public bool AddImagesToProduct(int productId, List<AdminImageDto> images)
        {
            try
            {
                foreach (var image in images)
                {
                    _context.IMAGENs.Add(new IMAGEN
                    {
                        ID_Producto = productId,
                        Nombre_Img = image.ImgName,
                        Imagen_URL = image.ImgUrl
                    });
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Elimina una imagen específica de un producto, según su URL e ID de producto.
        public bool DeleteImage(int productId, string imageUrl)
        {
            try
            {
                var image = _context.IMAGENs
                    .FirstOrDefault(i => i.ID_Producto == productId && i.Imagen_URL == imageUrl);

                if (image != null)
                {
                    _context.IMAGENs.Remove(image);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}