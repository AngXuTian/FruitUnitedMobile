using DBConnection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile.Modules
{
    public partial class GRTN_Location : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString; // Replace with your actual connection string

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Session["GRTN_Project_ID"].ToString()))
            {
                if (!IsPostBack)
                {
                    BindDropdown();
                }
            }
            else 
            {
                string targetUrl = "~/Modules/GRTN-Project.aspx";
                Response.Redirect(targetUrl);
            }
        }

        private void BindDropdown()
        {

            string query = "SELECT Location_Profile_ID, Location FROM Location_Profile WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlLocation.DataSource = reader;
                    ddlLocation.DataTextField = "Location"; // Text displayed in dropdown
                    ddlLocation.DataValueField = "Location_Profile_ID"; // Value for each item
                    ddlLocation.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlLocation.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Session["GRTN_Location_ID"] = ddlLocation.SelectedValue;
            string targetUrl = "~/Modules/GRTN-MaterialList.aspx";
            Response.Redirect(targetUrl);
        }
    }
}