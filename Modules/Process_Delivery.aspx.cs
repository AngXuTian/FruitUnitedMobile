using FruitUnitedMobile.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile.Modules
{
    public partial class Process_Delivery : System.Web.UI.Page
    {
        public class StockIssue
        {
            public int ProductId { get; set; }
            public int Requested { get; set; }
            public int Available { get; set; }
            public string ProductName { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmpID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                if (Session["DeliveryProducts"] == null)
                {
                    Response.Redirect("~/Modules/Delivery_Plan.aspx");
                    return;
                }

                LoadDeliveryItems();
                LoadAvailableProducts();
            }
            else
            {
                LoadDeliveryItems();
            }

            // Always re-check stock on every load/postback
            CheckAndHighlightStockIssues();
        }

        private void LoadDeliveryItems()
        {
            DataTable dt = Session["DeliveryProducts"] as DataTable;

            if (dt != null && dt.Rows.Count > 0)
            {
                rptDeliveryItems.DataSource = dt;
                rptDeliveryItems.DataBind();

                lblItemCount.Text = dt.Rows.Cast<DataRow>().Count(r => Convert.ToInt32(r["Quantity"]) > 0).ToString();
                pnlProducts.Visible = true;
                pnlEmpty.Visible = false;
            }
            else
            {
                pnlProducts.Visible = false;
                pnlEmpty.Visible = true;
                lblItemCount.Text = "0";
            }
        }

        private void LoadAvailableProducts()
        {
            DataTable deliveryDt = Session["DeliveryProducts"] as DataTable;

            List<int> deliveryProductIds = deliveryDt?.AsEnumerable()
                .Select(r => r.Field<int>("Product_Profile_ID"))
                .ToList() ?? new List<int>();

            string query = @"
                SELECT Product_Profile_ID, Abbreviation, Filename
                FROM Product_Profile
                WHERE status = 'active'";

            if (deliveryProductIds.Count > 0)
            {
                string ids = string.Join(",", deliveryProductIds);
                query += $" AND Product_Profile_ID NOT IN ({ids})";
            }

            query += " ORDER BY Abbreviation";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    rpAvailableProducts.DataSource = dt;
                    rpAvailableProducts.DataBind();
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Modules/Delivery_Plan_Detail.aspx");
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string jsonData = hfProductData.Value;

            if (string.IsNullOrEmpty(jsonData))
            {
                Toast1.ShowWarning("Please add at least one product with quantity > 0.");
                return;
            }

            try
            {
                List<ProductQty> clientProductList = JsonConvert.DeserializeObject<List<ProductQty>>(jsonData);

                if (clientProductList == null || !clientProductList.Any(p => p.qty > 0))
                {
                    Toast1.ShowWarning("Please add at least one product with quantity > 0.");
                    return;
                }

                DataTable sessionDt = Session["DeliveryProducts"] as DataTable;

                if (sessionDt == null)
                {
                    Toast1.ShowError("Session expired. Please start again.");
                    return;
                }

                // Create a list of current Product IDs from client
                var currentProductIds = clientProductList
                    .Where(p => p.qty > 0)
                    .Select(p => int.Parse(p.productId))
                    .ToList();

                // Step 1: Remove products from Session that are no longer in client list
                var rowsToRemove = sessionDt.AsEnumerable()
                    .Where(row => !currentProductIds.Contains(row.Field<int>("Product_Profile_ID")))
                    .ToList();

                foreach (var row in rowsToRemove)
                {
                    sessionDt.Rows.Remove(row);
                }

                // Step 2: Update quantities for remaining products
                foreach (var prod in clientProductList.Where(p => p.qty > 0))
                {
                    int prodId = int.Parse(prod.productId);
                    DataRow row = sessionDt.AsEnumerable()
                        .FirstOrDefault(r => r.Field<int>("Product_Profile_ID") == prodId);

                    if (row != null)
                    {
                        row["Quantity"] = prod.qty;
                    }
                    // If somehow missing (should not happen), skip
                }

                sessionDt.AcceptChanges();
                Session["DeliveryProducts"] = sessionDt;

                // Now re-check stock with the cleaned + updated session data
                CheckAndHighlightStockIssues();

                if (!string.IsNullOrEmpty(hfStockIssues.Value))
                {
                    var issues = JsonConvert.DeserializeObject<List<StockIssue>>(hfStockIssues.Value);
                    string msg = string.Join("; ", issues.Select(i => $"{i.ProductName} (Shortage: {i.Requested - i.Available})"));
                    Toast1.ShowError($"Delivery exceeds loaded balance: {msg}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "highlight", "highlightInsufficientStock();", true);
                    return;
                }

                // All good — proceed to next page
                Response.Redirect("~/Modules/Process_Return.aspx");
            }
            catch (Exception ex)
            {
                Toast1.ShowError("Error processing delivery items: " + ex.Message);
            }
        }

        protected void rpAvailableProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddProduct")
            {
                int productId = int.Parse(e.CommandArgument.ToString());
                DataTable dt = Session["DeliveryProducts"] as DataTable ?? new DataTable();

                if (dt.Columns.Count == 0)
                {
                    dt.Columns.Add("Product_Profile_ID", typeof(int));
                    dt.Columns.Add("Abbreviation", typeof(string));
                    dt.Columns.Add("Filename", typeof(string));
                    dt.Columns.Add("Quantity", typeof(int));
                }

                if (!dt.AsEnumerable().Any(r => r.Field<int>("Product_Profile_ID") == productId))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
                    {
                        string query = "SELECT Abbreviation, Filename FROM Product_Profile WHERE Product_Profile_ID = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", productId);
                            conn.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    DataRow row = dt.NewRow();
                                    row["Product_Profile_ID"] = productId;
                                    row["Abbreviation"] = reader["Abbreviation"];
                                    row["Filename"] = reader["Filename"];
                                    row["Quantity"] = 1;
                                    dt.Rows.Add(row);
                                }
                            }
                        }
                    }
                    Session["DeliveryProducts"] = dt;
                }

                LoadDeliveryItems();
                LoadAvailableProducts();
                CheckAndHighlightStockIssues();
            }
        }

        private int GetCurrentVehicleId()
        {
            if (Session["Vehicle_Profile_ID"] != null)
                return Convert.ToInt32(Session["Vehicle_Profile_ID"]);
            throw new Exception("Vehicle Session Expired.");
        }

        private void CheckAndHighlightStockIssues()
        {
            DataTable dt = Session["DeliveryProducts"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                hfStockIssues.Value = string.Empty;
                return;
            }

            int vehicleId = GetCurrentVehicleId();
            var issues = new List<StockIssue>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    int prodId = Convert.ToInt32(row["Product_Profile_ID"]);
                    int requestedQty = Convert.ToInt32(row["Quantity"]);

                    if (requestedQty <= 0) continue;

                    string sql = @"
                        SELECT SUM(DLI.Balance) AS Balance 
                        FROM Daily_Loading_Items DLI 
                        INNER JOIN Daily_Loading DL ON DLI.Daily_Loading_ID = DL.Daily_Loading_ID 
                        WHERE DL.Vehicle_Profile_ID = @VID AND DL.Status = 'Loaded' AND DLI.Product_Profile_ID = @ProductID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@VID", vehicleId);
                        cmd.Parameters.AddWithValue("@ProductID", prodId);
                        object result = cmd.ExecuteScalar();
                        int available = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);

                        if (requestedQty > available)
                        {
                            issues.Add(new StockIssue
                            {
                                ProductId = prodId,
                                Available = available,
                                Requested = requestedQty,
                                ProductName = row["Abbreviation"]?.ToString() ?? "Unknown"
                            });
                        }
                    }
                }
            }

            hfStockIssues.Value = issues.Count > 0 ? JsonConvert.SerializeObject(issues) : string.Empty;
        }

        private class ProductQty
        {
            public string productId { get; set; }
            public int qty { get; set; }
        }
    }
}