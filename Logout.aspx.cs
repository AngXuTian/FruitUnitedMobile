using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ForValidation;
using System.Configuration;

namespace FruitUnitedMobile
{
    public partial class Logout : System.Web.UI.Page
    {
        Validation validate = new Validation();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //string downloadPath = ConfigurationManager.AppSettings["ReportPath"];
                //validate.DeletePDFFiles(Server.MapPath("Uploads/Reports/"));

                Session.Clear();
                Session.Abandon();
                Response.Cache.SetExpires(DateTime.Now.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                FormsAuthentication.SignOut();

                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(@"{0} : {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}