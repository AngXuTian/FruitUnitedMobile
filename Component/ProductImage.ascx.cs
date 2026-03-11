using System;
using System.Configuration;
using System.IO;
using System.Web.UI;

namespace FruitUnitedMobile.Component
{
    public partial class ProductImage : UserControl
    {
        public object ProductId { get; set; }
        public object FileName { get; set; }
        public string Abbreviation { get; set; }
        public int ImageSize { get; set; } = 90;   // default 90×90 px

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) ApplyImage();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ApplyImage();
            base.OnPreRender(e);
        }

        private void ApplyImage()
        {
            string url = GetProductImageUrl(FileName, ProductId);
            imgProduct.ImageUrl = url;
            imgProduct.AlternateText = string.IsNullOrEmpty(Abbreviation) ? "Product" : Abbreviation;

            int size = ImageSize < 40 ? 40 : ImageSize; // minimum 40px

            imgProduct.Attributes["style"] = string.Format(@"
                display: block;
                width: {0}px !important;
                height: {0}px !important;
                object-fit: cover;
                border-radius: 8px;
                background: #f9f9f9;
                padding: 8px;
                box-sizing: border-box;
                margin: 0 auto 10px auto;   /* centered + space below */
                flex-shrink: 0;",
                size);
        }

        // (GetProductImageUrl and GetMimeType stay exactly the same as before)
        private string GetProductImageUrl(object filenameObj, object productIdObj)
        {
            string filename = (filenameObj != null ? filenameObj.ToString() : "").Trim();
            string id = (productIdObj != null ? productIdObj.ToString() : "").Trim();

            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(id))
                return "~/Images/no-image.png";

            string vault = ConfigurationManager.AppSettings["DocumentVault"];
            if (string.IsNullOrEmpty(vault)) return "~/Images/no-image.png";

            string fullPath = Path.Combine(vault, "Product_Profile", "File", id, filename);

            if (File.Exists(fullPath))
            {
                byte[] bytes = File.ReadAllBytes(fullPath);
                string mime = GetMimeType(filename);
                return "data:" + mime + ";base64," + Convert.ToBase64String(bytes);
            }

            return "~/Images/no-image.png";
        }

        private string GetMimeType(string filename)
        {
            string ext = (Path.GetExtension(filename) ?? "").ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg") return "image/jpeg";
            if (ext == ".png") return "image/png";
            if (ext == ".gif") return "image/gif";
            if (ext == ".webp") return "image/webp";
            return "image/png";
        }
    }
}