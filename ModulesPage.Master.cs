using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FruitUnitedMobile.Component; // For Toast control

namespace FruitUnitedMobile
{
    public partial class ModulesPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Employee_Name"] != null && !string.IsNullOrEmpty(Session["Employee_Name"].ToString()))
            {
                //UserButton.ToolTip = "Hi, " + Session["Employee_Name"].ToString();
                CurrentEmployeeName.Text = Session["Employee_Name"].ToString();
            }
            else
            {
                string targetUrl = "~/Login.aspx";
                Response.Redirect(targetUrl);
            }

            //litSessionData.Text = SessionHelper.GetAllSessionData();
        }

        // Public method to show toast from content pages
        public void ShowToast(string message, string type = "info", string title = null, int duration = 5000)
        {
            if (Toast1 == null) return; // just safety check

            switch (type.ToLower())
            {
                case "success":
                    Toast1.ShowSuccess(message, title, duration);
                    break;
                case "error":
                    Toast1.ShowError(message, title, duration);
                    break;
                case "warning":
                    Toast1.ShowWarning(message, title, duration);
                    break;
                default:
                    Toast1.ShowInfo(message, title, duration);
                    break;
            }
        }
    }
}