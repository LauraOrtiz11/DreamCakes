using System;
using System.IO;
using System.Web;

namespace DreamCakes.Utilities
{
    public static class FileHelper
    {
        public static string SaveFile(HttpPostedFileBase file, string basePath)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            try
            {
                var serverPath = HttpContext.Current.Server.MapPath(basePath);
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var relativePath = $"{basePath}{fileName}".Replace("~", "");
                var fullPath = Path.Combine(serverPath, fileName);

                file.SaveAs(fullPath);
                return relativePath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return false;

            try
            {
                var fullPath = HttpContext.Current.Server.MapPath("~" + fileUrl);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
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