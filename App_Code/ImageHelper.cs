// ~/App_Code/ImageHelper.cs
using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace FruitUnitedMobile.ImageHelper
{
    public static class ImageHelper
    {
        public static string GetProductImage(object filenameObj, object productIdObj)
        {
            string filename = (filenameObj?.ToString() ?? "").Trim();
            string id = (productIdObj?.ToString() ?? "").Trim();

            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(id))
                return NoImage();

            string vault = ConfigurationManager.AppSettings["DocumentVault"]?.Trim();
            if (string.IsNullOrEmpty(vault)) return NoImage();

            string fullPath = Path.Combine(vault, "Product_Profile", "File", id, filename);

            try
            {
                if (File.Exists(fullPath))
                {
                    byte[] bytes = File.ReadAllBytes(fullPath);
                    string mime = GetMime(filename);
                    return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
                }
            }
            catch { }

            return NoImage();
        }

        private static string NoImage()
        {
            string path = HttpContext.Current.Server.MapPath("~/Images/no-image.png");
            if (File.Exists(path))
            {
                byte[] b = File.ReadAllBytes(path);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            }
            return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";
        }

        private static string GetMime(string f)
        {
            return Path.GetExtension(f).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png"
            };
        }
    }
}