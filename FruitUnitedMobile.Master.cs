using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile
{
    public partial class FruitUnited : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["EmpCode"] != null && !string.IsNullOrEmpty(Session["EmpCode"].ToString()))
            //{
            //    DefaultHeader.Visible = false; // Hide the default header
            //    LoggedInHeader.Visible = true; // Show the logged-in header

            //    // Optionally, set the user's name or EmpCode in the header
            //    UserNameLiteral.Text = $"Welcome: {Session["Employee_Name"]}";
            //}
            //else
            //{
            //    // User is not logged in
            //    DefaultHeader.Visible = true; // Show the default header
            //    LoggedInHeader.Visible = false; // Hide the logged-in header
            //}
        }

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