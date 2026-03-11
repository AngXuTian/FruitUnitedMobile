using System;
using System.Web.UI;

namespace FruitUnitedMobile.Component
{
    public partial class SearchBar : System.Web.UI.UserControl
    {
        // Property to set custom placeholder text
        public string Placeholder
        {
            get { return txtSearch.Attributes["placeholder"]; }
            set { txtSearch.Attributes["placeholder"] = value; }
        }

        // Property to set search info text
        public string SearchInfoText
        {
            get { return lblSearchInfo.Text; }
            set { lblSearchInfo.Text = value; }
        }

        // Property to set which data attributes to search
        public string SearchFields
        {
            get { return txtSearch.Attributes["data-target"]; }
            set { txtSearch.Attributes["data-target"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set default placeholder if not set
                if (string.IsNullOrEmpty(txtSearch.Attributes["placeholder"]))
                {
                    txtSearch.Attributes["placeholder"] = "Search...";
                }
            }
        }
    }
}