using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace FruitUnitedMobile.Modules
{
    public partial class Process_Return : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmpID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadExchangeReasons();
                LoadAvailableProducts();
                LoadReturnList();
            }
        }

        // ================= VALIDATION HELPERS =================

        private bool ValidateReturnSession(out string errorMessage)
        {
            errorMessage = "";

            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            if (returnDT == null || returnDT.Rows.Count == 0)
            {
                //errorMessage = "Please add at least one return item before proceeding.";
                return true;
            }

            string[] requiredColumns = {
                "Product_Profile_ID",
                "Return_Quantity",
                "Exchange_Reason_ID"
            };

            foreach (string col in requiredColumns)
            {
                if (!returnDT.Columns.Contains(col))
                {
                    errorMessage = "Return data is corrupted. Please reload the page.";
                    return false;
                }
            }

            foreach (DataRow row in returnDT.Rows)
            {
                if (row["Return_Quantity"] == DBNull.Value ||
                    Convert.ToInt32(row["Return_Quantity"]) <= 0 ||
                    Convert.ToInt32(row["Return_Quantity"]) > 999)
                {
                    errorMessage = "Invalid return quantity detected.";
                    return false;
                }

                if (row["Exchange_Reason_ID"] == DBNull.Value ||
                    Convert.ToInt32(row["Exchange_Reason_ID"]) <= 0)
                {
                    errorMessage = "Please select exchange reason for all items.";
                    return false;
                }
            }

            return true;
        }

        // ================= LOADING DATA =================

        private DataTable GetReasonDataTable()
        {
            string query = @"SELECT Exchange_Reason_ID, Reason 
                             FROM Exchange_Reason 
                             WHERE Status = 'Active' 
                             ORDER BY Reason";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                new SqlDataAdapter(cmd).Fill(dt);
            }
            return dt;
        }

        private void LoadExchangeReasons()
        {
            ddlExchangeReason.DataSource = GetReasonDataTable();
            ddlExchangeReason.DataTextField = "Reason";
            ddlExchangeReason.DataValueField = "Exchange_Reason_ID";
            ddlExchangeReason.DataBind();
            ddlExchangeReason.Items.Insert(0, new ListItem("-- Select Reason --", ""));
        }

        private void LoadAvailableProducts()
        {
            string query = @"
                SELECT Product_Profile_ID, Abbreviation, Product_Name, Filename
                FROM Product_Profile
                WHERE Status = 'Active'
                ORDER BY Abbreviation";

            DataTable productsDT = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                new SqlDataAdapter(cmd).Fill(productsDT);
            }

            DataTable returnDT = Session["ReturnProducts"] as DataTable;
            if (returnDT != null)
            {
                var ids = returnDT.AsEnumerable()
                                  .Select(r => Convert.ToInt32(r["Product_Profile_ID"]))
                                  .ToList();

                for (int i = productsDT.Rows.Count - 1; i >= 0; i--)
                {
                    if (ids.Contains(Convert.ToInt32(productsDT.Rows[i]["Product_Profile_ID"])))
                        productsDT.Rows.RemoveAt(i);
                }
            }

            rptAvailableProducts.DataSource = productsDT;
            rptAvailableProducts.DataBind();
        }

        private void LoadReturnList()
        {
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            rptReturnItems.DataSource = returnDT;
            rptReturnItems.DataBind();

            lblNoReturns.Visible = (returnDT == null || returnDT.Rows.Count == 0);
            upReturnList.Update();
        }

        // ================= REPEATER EVENTS =================

        protected void rptReturnItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlListReason");
                if (ddl != null)
                {
                    ddl.DataSource = GetReasonDataTable();
                    ddl.DataTextField = "Reason";
                    ddl.DataValueField = "Exchange_Reason_ID";
                    ddl.DataBind();

                    DataRowView drv = (DataRowView)e.Item.DataItem;
                    if (ddl.Items.FindByValue(drv["Exchange_Reason_ID"].ToString()) != null)
                        ddl.SelectedValue = drv["Exchange_Reason_ID"].ToString();
                }
            }
        }

        protected void UpdateReturnItem_Command(object sender, CommandEventArgs e)
        {
            int productID = Convert.ToInt32(e.CommandArgument);
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            if (returnDT == null) return;

            DataRow row = returnDT.AsEnumerable()
                                  .FirstOrDefault(r => Convert.ToInt32(r["Product_Profile_ID"]) == productID);

            if (row == null) return;

            int qty = Convert.ToInt32(row["Return_Quantity"]);
            if (e.CommandName == "Increase" && qty < 999)
                row["Return_Quantity"] = qty + 1;
            else if (e.CommandName == "Decrease" && qty > 1)
                row["Return_Quantity"] = qty - 1;

            Session["ReturnProducts"] = returnDT;
            LoadReturnList();
        }

        protected void txtListQty_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            RepeaterItem item = (RepeaterItem)txt.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfListProdID");

            int qty;
            if (!int.TryParse(txt.Text, out qty) || qty < 1 || qty > 999)
            {
                Toast1.ShowWarning("Quantity must be between 1 and 999.");
                LoadReturnList();
                return;
            }

            DataTable returnDT = Session["ReturnProducts"] as DataTable;
            DataRow row = returnDT?.AsEnumerable()
                                  .FirstOrDefault(r => Convert.ToInt32(r["Product_Profile_ID"]) == Convert.ToInt32(hf.Value));

            if (row != null)
            {
                row["Return_Quantity"] = qty;
                Session["ReturnProducts"] = returnDT;
            }

            LoadReturnList();
        }

        protected void ddlListReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (string.IsNullOrEmpty(ddl.SelectedValue))
            {
                Toast1.ShowWarning("Invalid exchange reason.");
                return;
            }

            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfListProdID");

            DataTable returnDT = Session["ReturnProducts"] as DataTable;
            DataRow row = returnDT?.AsEnumerable()
                                  .FirstOrDefault(r => Convert.ToInt32(r["Product_Profile_ID"]) == Convert.ToInt32(hf.Value));

            if (row != null)
            {
                row["Exchange_Reason_ID"] = Convert.ToInt32(ddl.SelectedValue);
                row["Reason"] = ddl.SelectedItem.Text;
                Session["ReturnProducts"] = returnDT;
            }
        }

        // ================= MAIN ACTIONS =================

        protected void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelectedProductID.Value))
            {
                Toast1.ShowWarning("Please select a product.");
                return;
            }

            int returnQty;
            if (!int.TryParse(txtReturnQty.Text, out returnQty) || returnQty < 1 || returnQty > 999)
            {
                Toast1.ShowWarning("Quantity must be between 1 and 999.");
                return;
            }

            if (string.IsNullOrEmpty(ddlExchangeReason.SelectedValue))
            {
                Toast1.ShowWarning("Please select an exchange reason.");
                return;
            }

            int productID = Convert.ToInt32(hfSelectedProductID.Value);

            string validateSql = @"SELECT COUNT(1) FROM Product_Profile 
                                   WHERE Product_Profile_ID=@ID AND Status='Active'";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(validateSql, conn))
            {
                cmd.Parameters.AddWithValue("@ID", productID);
                conn.Open();

                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    Toast1.ShowError("Selected product is no longer available.");
                    return;
                }
            }

            string query = @"SELECT Abbreviation, Product_Name, Filename 
                             FROM Product_Profile WHERE Product_Profile_ID=@ID";

            string abbr = "", name = "", file = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", productID);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        abbr = r["Abbreviation"].ToString();
                        name = r["Product_Name"].ToString();
                        file = r["Filename"].ToString();
                    }
                }
            }

            DataTable returnDT = Session["ReturnProducts"] as DataTable ?? CreateReturnTable();

            DataRow row = returnDT.AsEnumerable()
                                  .FirstOrDefault(r => Convert.ToInt32(r["Product_Profile_ID"]) == productID)
                           ?? returnDT.NewRow();

            row["Product_Profile_ID"] = productID;
            row["Abbreviation"] = abbr;
            row["Product_Name"] = name;
            row["Return_Quantity"] = returnQty;
            row["Exchange_Reason_ID"] = Convert.ToInt32(ddlExchangeReason.SelectedValue);
            row["Reason"] = ddlExchangeReason.SelectedItem.Text;
            row["Filename"] = file;

            if (row.RowState == DataRowState.Detached)
                returnDT.Rows.Add(row);

            Session["ReturnProducts"] = returnDT;

            LoadReturnList();
            LoadAvailableProducts();
            upModalProducts.Update();
        }

        private DataTable CreateReturnTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Product_Profile_ID", typeof(int));
            dt.Columns.Add("Abbreviation", typeof(string));
            dt.Columns.Add("Product_Name", typeof(string));
            dt.Columns.Add("Return_Quantity", typeof(int));
            dt.Columns.Add("Exchange_Reason_ID", typeof(int));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Filename", typeof(string));
            return dt;
        }

        protected void btnRemoveReturn_Click(object sender, EventArgs e)
        {
            int productID = Convert.ToInt32(((Button)sender).CommandArgument);
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            if (returnDT != null)
            {
                foreach (DataRow row in returnDT.Select($"Product_Profile_ID={productID}"))
                    returnDT.Rows.Remove(row);

                Session["ReturnProducts"] = returnDT;
            }

            LoadReturnList();
            LoadAvailableProducts();
            upModalProducts.Update();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Modules/Process_Delivery.aspx");
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string error;
            if (!ValidateReturnSession(out error))
            {
                Toast1.ShowWarning(error);
                return;
            }

            Response.Redirect("~/Modules/Process_Outstanding.aspx");
        }

        protected string GetProductImageUrl(object filename)
        {
            return string.IsNullOrEmpty(filename?.ToString())
                ? "/Images/Products/no-image.png"
                : "/Images/Products/" + filename;
        }
    }
}
