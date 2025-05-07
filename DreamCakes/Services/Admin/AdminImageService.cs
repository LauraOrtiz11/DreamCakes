using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Admin;
using DreamCakes.Repositories.Models;
using System.Web;
using System.Collections.Generic;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services.Admin
{
    public class AdminImageService
    {
        private readonly AdminImageRepository _imageRepository;
        private readonly string _imageBasePath = "~/Content/ImgProducts/";

        public AdminImageService()
        {
            _imageRepository = new AdminImageRepository();
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
                    var imageUrl = FileHelperUtility.SaveFile(image, _imageBasePath);
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

            var images = new List<AdminImageDto>();
            foreach (var url in imageUrls)
            {
                images.Add(new AdminImageDto
                {
                    ImgName = System.IO.Path.GetFileName(url),
                    ImgUrl = url
                });
            }

            return _imageRepository.AddImagesToProduct(productId, images);
        }

        public bool DeleteImage(int productId, string imageUrl)
        {
            try
            {
                FileHelperUtility.DeleteFile(imageUrl);
                return _imageRepository.DeleteImage(productId, imageUrl);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}