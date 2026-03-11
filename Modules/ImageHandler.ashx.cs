using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BintangMasMobile.Modules
{
    public partial class ImageHandler : System.Web.UI.Page
    {
        public void ProcessRequest(HttpContext context)
        {
            string imagePath = context.Request.QueryString["path"];
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                context.Response.ContentType = GetContentType(Path.GetExtension(imagePath));
                context.Response.WriteFile(imagePath);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Image not found.");
            }
        }

        public bool IsReusable => false;

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                default: return "application/octet-stream";
            }
        }
    }
}