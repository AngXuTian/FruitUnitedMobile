using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;

namespace FruitUnitedMobile.Modules
{
    public partial class Delivery_Plan_Detail : BasePage
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmpID"] == null) { Response.Redirect("~/Login.aspx"); return; }
                if (Session["SelectedOutletID"] == null || Session["SelectedDeliveryOutletID"] == null)
                {
                    Response.Redirect("~/Modules/Delivery_Plan.aspx");
                    return;
                }

                LoadOutletInfo();
                LoadProducts();
            }
        }

        private void LoadOutletInfo()
        {
            string query = @"
                SELECT outlet.Outlet_Number, outlet.Outlet_Name, outlet.Postcode,
       CONVERT(nvarchar(max), outlet.Address) AS Address, Delivery_Outlet.Operating_Hour,
       Delivery_Outlet.Primary_Contact, Delivery_Outlet.Secondary_Contact, CONVERT(nvarchar(max), Delivery_Outlet.Remarks) AS Remarks
FROM Delivery_Outlet 
INNER JOIN Outlet_Profile outlet ON outlet.Outlet_Profile_ID = delivery_outlet.Outlet_Profile_ID
WHERE outlet.Outlet_Profile_ID = @OutletID AND delivery_outlet.Delivery_Outlet_ID = @DeliveryOutletID";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OutletID", Session["SelectedOutletID"]);
                    cmd.Parameters.AddWithValue("@DeliveryOutletID", Session["SelectedDeliveryOutletID"]);

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblPostCode.Text = reader["Postcode"].ToString();
                            lblOutletName.Text = reader["Outlet_Name"].ToString();
                            lblAddress.Text = reader["Address"].ToString();

                        }
                    }
                }
            }

        }

        private void LoadProducts()
        {
            string query = @"
                SELECT Product_Profile.Product_Profile_ID, Product_Profile.Abbreviation, 
                       Product_Profile.Filename,
                       CAST(ISNULL(SUM(ISNULL(product.Quantity, 0)), 0) AS INT) AS Quantity
                FROM Delivery_Product product
                INNER JOIN Product_Profile ON Product_Profile.Product_Profile_ID = product.Product_Profile_ID
                WHERE product.Delivery_Outlet_ID = @DeliveryOutletID
                GROUP BY Product_Profile.Product_Profile_ID, Product_Profile.Abbreviation,
                         Product_Profile.Product_Name, Product_Profile.Filename
                ORDER BY Product_Profile.Abbreviation";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DeliveryOutletID", Session["SelectedDeliveryOutletID"]);

                    connection.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rpProducts.DataSource = dt;
                        rpProducts.DataBind();
                        lblNoProducts.Visible = false;
                        Session["DeliveryProducts"] = dt;
                    }
                    else
                    {
                        rpProducts.Visible = false;
                        lblNoProducts.Visible = true;
                    }
                }
            }
        }

        //protected string GetProductImageUrl(object filename)
        //{
        //    if (filename == null || string.IsNullOrEmpty(filename.ToString()))
        //    {
        //        return "~/Images/no-image.png";
        //    }
        //    return "~/Images/Products/" + filename.ToString();
        //}

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Modules/Delivery_Plan.aspx");
        }

        protected void btnPrepareInvoice_Click(object sender, EventArgs e)
        {
            int dailyLoadingID = Convert.ToInt32(Session["DailyLoadingID"]);

            if (dailyLoadingID == 0)
            {
                Toast1.ShowWarning(
                    "Unable to prepare invoice. The selected vehicle has not been loaded yet. Please load the vehicle first."
                );
                return;
            }

            DataTable dt = Session["DeliveryProducts"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                Toast1.ShowWarning("No products found for this delivery.");
                return;
            }
            Response.Redirect("~/Modules/Process_Delivery.aspx");
        }
    }
}
