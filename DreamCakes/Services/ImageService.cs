using DreamCakes.Dtos;
using DreamCakes.Repositories;
using DreamCakes.Repositories.Models; // Añade esta directiva using
using System.Web;
using System.Collections.Generic;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class ImageService
    {
        private readonly ImageRepository _imageRepository;
        private readonly string _imageBasePath = "~/Content/ImgProducts/";

        public ImageService()
        {
            // Cambiar esta línea para crear el contexto correctamente
            _imageRepository = new ImageRepository(new DreamCakesEntities());
        }

        public List<string> SaveUploadedImages(IEnumerable<HttpPostedFileBase> images)
        {
            var savedUrls = new List<string>();
            if (images == null)
                return savedUrls;

            foreach (var image in images)
            {
                if (image != null && image.ContentLength > 0)
                {
                    var imageUrl = FileHelper.SaveFile(image, _imageBasePath);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        savedUrls.Add(imageUrl);
                    }
                }
            }
            return savedUrls;
        }

        public bool AddImagesToProduct(int productId, List<string> imageUrls)
        {
            if (imageUrls == null || imageUrls.Count == 0)
                return false;

            var images = new List<ImageDto>();
            foreach (var url in imageUrls)
            {
                images.Add(new ImageDto
                {
                    Name = System.IO.Path.GetFileName(url),
                    Url = url
                });
            }

            return _imageRepository.AddImagesToProduct(productId, images);
        }

        public bool DeleteImage(int productId, string imageUrl)
        {
            try
            {
                FileHelper.DeleteFile(imageUrl);
                return _imageRepository.DeleteImage(productId, imageUrl);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}