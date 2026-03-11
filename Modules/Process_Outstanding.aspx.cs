using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FruitUnitedMobile.Common;
using DBConnection;

namespace FruitUnitedMobile.Modules
{
    public partial class Process_Outstanding : BasePage
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
                EnsureIsOutstandingColumn();
                CheckAndUpdateOutstandingStatus();
                LoadReturnItems();
            }
        }

        private void CheckAndUpdateOutstandingStatus()
        {
            DataTable dt = Session["ReturnProducts"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) return;

            int outletId = GetOutletId();
            if (outletId == 0) return;

            List<int> productIds = dt.AsEnumerable()
                .Select(r => Convert.ToInt32(r["Product_Profile_ID"]))
                .ToList();

            // FIXED: Use new method instead of obsolete one
            Dictionary<int, int> outstandingProducts = StockHelper.GetPreviousOutstandingBalance(outletId, productIds);

            List<string> changedProducts = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                int returnQty = Convert.ToInt32(row["Return_Quantity"]);
                int availableBalance = outstandingProducts.ContainsKey(productId) ? outstandingProducts[productId] : 0;
                bool currentIsOutstanding = Convert.ToBoolean(row["IsOutstanding"]);

                bool hasOutstanding = availableBalance > 0;

                if (hasOutstanding && !currentIsOutstanding)
                {
                    row["IsOutstanding"] = true;
                    string productName = row["Abbreviation"] != null ? row["Abbreviation"].ToString() : "Unknown";
                    changedProducts.Add($"{productName} (Outstanding: {availableBalance}, Return: {returnQty})");
                }
            }

            if (changedProducts.Count > 0)
            {
                Session["ReturnProducts"] = dt;
                string productList = string.Join("\n• ", changedProducts);
                string message = changedProducts.Count == 1
                    ? $"The following product has been set to Outstanding (YES) because it has outstanding balance:\n\n• {productList}"
                    : $"The following products have been set to Outstanding (YES) because they have outstanding balance:\n\n• {productList}";
                Session["OutstandingWarning"] = message;
            }
        }

        private int GetOutletId()
        {
            if (Session["Outlet_Profile_ID"] != null)
                return Convert.ToInt32(Session["Outlet_Profile_ID"]);
            else if (Session["SelectedOutletID"] != null)
                return Convert.ToInt32(Session["SelectedOutletID"]);
            return 0;
        }

        private void EnsureIsOutstandingColumn()
        {
            DataTable dt = Session["ReturnProducts"] as DataTable;
            if (dt == null || dt.Rows.Count == 0) return;

            bool columnMissing = !dt.Columns.Contains("IsOutstanding");

            if (columnMissing)
            {
                dt.Columns.Add("IsOutstanding", typeof(bool));
            }

            bool anyChange = columnMissing;

            foreach (DataRow row in dt.Rows)
            {
                if (row["IsOutstanding"] == DBNull.Value || row["IsOutstanding"] == null)
                {
                    row["IsOutstanding"] = true;
                    anyChange = true;
                }
            }

            if (anyChange)
            {
                Session["ReturnProducts"] = dt;
            }
        }

        private void LoadReturnItems()
        {
            DataTable dt = Session["ReturnProducts"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                pnlOutstandingItems.Visible = false;
                pnlEmpty.Visible = true;
                return;
            }

            if (!dt.Columns.Contains("Filename"))
            {
                dt.Columns.Add("Filename", typeof(string));
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
                {
                    con.Open();
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Convert.ToInt32(row["Product_Profile_ID"]);
                        using (SqlCommand cmd = new SqlCommand("SELECT Filename FROM Product_Profile WHERE Product_Profile_ID = @ID", con))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            object result = cmd.ExecuteScalar();
                            row["Filename"] = result != null ? result.ToString() : "";
                        }
                    }
                }
                Session["ReturnProducts"] = dt;
            }

            rptOutstandingItems.DataSource = dt;
            rptOutstandingItems.DataBind();
            pnlOutstandingItems.Visible = true;
            pnlEmpty.Visible = false;

            if (Session["OutstandingWarning"] != null)
            {
                string warning = Session["OutstandingWarning"].ToString();
                Toast1.ShowWarning(warning);
                Session.Remove("OutstandingWarning");
            }
        }

        private bool UpdateIsOutstandingFromControls()
        {
            DataTable returnDT = Session["ReturnProducts"] as DataTable;
            if (returnDT == null || returnDT.Rows.Count == 0) return true;

            DataTable deliveryDT = Session["DeliveryProducts"] as DataTable;

            int outletId = GetOutletId();
            List<int> productIds = returnDT.AsEnumerable().Select(r => Convert.ToInt32(r["Product_Profile_ID"])).ToList();

            // FIXED: Use new method
            Dictionary<int, int> outstandingProducts = outletId > 0
                ? StockHelper.GetPreviousOutstandingBalance(outletId, productIds)
                : new Dictionary<int, int>();

            List<string> forcedToYes = new List<string>();
            List<string> forcedToNoDueToNoDelivery = new List<string>();
            List<string> forcedToNoDueToExcessReturn = new List<string>();
            List<int> problematicProductIds = new List<int>();

            int index = 0;
            foreach (RepeaterItem item in rptOutstandingItems.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hf = (HiddenField)item.FindControl("hfIsOutstanding");
                    bool userWantsOutstanding = hf != null && hf.Value == "1";

                    DataRow row = returnDT.Rows[index];
                    int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                    int returnQty = Convert.ToInt32(row["Return_Quantity"]);
                    string productName = row["Abbreviation"] != null ? row["Abbreviation"].ToString() : "Unknown";

                    // Previous outstanding → force YES
                    int prevOutstanding = outstandingProducts.ContainsKey(productId) ? outstandingProducts[productId] : 0;
                    if (prevOutstanding > 0 && !userWantsOutstanding)
                    {
                        row["IsOutstanding"] = true;
                        forcedToYes.Add($"{productName} (Prev Outstanding: {prevOutstanding}, Return: {returnQty})");
                        problematicProductIds.Add(productId);
                    }

                    // Return > Delivery → force NO
                    int deliveryQty = 0;
                    if (deliveryDT != null)
                    {
                        DataRow[] deliveryRows = deliveryDT.Select($"Product_Profile_ID = {productId}");
                        if (deliveryRows.Length > 0)
                            deliveryQty = Convert.ToInt32(deliveryRows[0]["Quantity"]);
                    }

                    bool excessReturn = returnQty > deliveryQty;
                    if (excessReturn && userWantsOutstanding)
                    {
                        row["IsOutstanding"] = false;
                        forcedToNoDueToExcessReturn.Add($"{productName} (Delivery: {deliveryQty}, Return: {returnQty})");
                        problematicProductIds.Add(productId);
                    }

                    // Not in delivery → force NO
                    bool existsInDelivery = deliveryQty > 0;
                    if (!existsInDelivery && userWantsOutstanding)
                    {
                        row["IsOutstanding"] = false;
                        forcedToNoDueToNoDelivery.Add($"{productName} (Not in today's delivery)");
                        problematicProductIds.Add(productId);
                    }

                    // Apply user choice only if no conflict
                    if (!problematicProductIds.Contains(productId))
                    {
                        row["IsOutstanding"] = userWantsOutstanding;
                    }

                    index++;
                }
            }

            Session["ReturnProducts"] = returnDT;

            bool hasIssues = forcedToYes.Count > 0 || forcedToNoDueToExcessReturn.Count > 0 || forcedToNoDueToNoDelivery.Count > 0;

            if (hasIssues)
            {
                Session["ProblematicProducts"] = problematicProductIds.Distinct().ToList();

                StringBuilder messageBuilder = new StringBuilder("Cannot proceed! Please correct the following:\n\n");

                if (forcedToYes.Count > 0)
                {
                    string list = string.Join("\n• ", forcedToYes);
                    messageBuilder.AppendLine(forcedToYes.Count == 1
                        ? "This product has previous outstanding balance and must be set to YES:\n\n• " + list
                        : "These products have previous outstanding balance and must be set to YES:\n\n• " + list);
                    messageBuilder.AppendLine();
                }

                if (forcedToNoDueToExcessReturn.Count > 0)
                {
                    string list = string.Join("\n• ", forcedToNoDueToExcessReturn);
                    messageBuilder.AppendLine(forcedToNoDueToExcessReturn.Count == 1
                        ? "This product returns more than delivered — must be set to NO:\n\n• " + list
                        : "These products return more than delivered — must be set to NO:\n\n• " + list);
                    messageBuilder.AppendLine();
                }

                if (forcedToNoDueToNoDelivery.Count > 0)
                {
                    string list = string.Join("\n• ", forcedToNoDueToNoDelivery);
                    messageBuilder.AppendLine(forcedToNoDueToNoDelivery.Count == 1
                        ? "This product is not in today's delivery — must be set to NO:\n\n• " + list
                        : "These products are not in today's delivery — must be set to NO:\n\n• " + list);
                }

                Toast1.ShowError(messageBuilder.ToString(), "Validation Error", 10000);
                ScriptManager.RegisterStartupScript(this, GetType(), "highlight", "highlightProblematicProducts();", true);

                return false;
            }

            Session.Remove("ProblematicProducts");
            return true;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            DataTable returnDT = Session["ReturnProducts"] as DataTable;
            if (returnDT != null && returnDT.Rows.Count > 0)
            {
                int index = 0;
                foreach (RepeaterItem item in rptOutstandingItems.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        HiddenField hf = (HiddenField)item.FindControl("hfIsOutstanding");
                        bool isOutstanding = hf != null && hf.Value == "1";
                        returnDT.Rows[index]["IsOutstanding"] = isOutstanding;
                        index++;
                    }
                }
                Session["ReturnProducts"] = returnDT;
            }

            Response.Redirect("~/Modules/Process_Return.aspx");
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string primaryCode = Session["EmpCode"]?.ToString();
            string secondaryCode = Session["Emp2Code"]?.ToString();

            string posProfileID = GetFinalPosProfileID(primaryCode, secondaryCode);

            if (posProfileID == "0")
            {
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("POS profile is not assigned to the driver.");
                messageBuilder.AppendLine("Please contact administrator.");

                Toast1.ShowWarning(messageBuilder.ToString(), "Validation Error", 10000);
                return;
            }

            // store if needed
            Session["Pos_Profile_ID"] = posProfileID;

            if (!UpdateIsOutstandingFromControls())
            {
                LoadReturnItems();
                return;
            }

            Response.Redirect("~/Modules/Review_Invoice.aspx");
        }

        protected string GetProductImageUrl(object filename)
        {
            string f = filename != null ? filename.ToString() : "";
            return string.IsNullOrEmpty(f) ? "/Images/Products/no-image.png" : "/Images/Products/" + f;
        }

        protected string GetUOM(object productIdObj)
        {
            if (productIdObj == null) return "Unit";
            int id = Convert.ToInt32(productIdObj);
            string key = "UOM_" + id;
            if (ViewState[key] != null) return ViewState[key].ToString();

            string uom = "Unit";
            string sql = "SELECT ISNULL(u.UOM, 'Unit') FROM Product_Profile p LEFT JOIN UOM_Profile u ON u.UOM_Profile_ID = p.UOM_Profile_ID WHERE p.Product_Profile_ID = @ID";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null) uom = res.ToString();
            }
            ViewState[key] = uom;
            return uom;
        }

        public static string GetFinalPosProfileID(string primaryCode, string secondaryCode)
        {
            Connection con = new Connection();

            string pos1 = "0";
            string pos2 = "0";

            if (!string.IsNullOrEmpty(primaryCode))
            {
                pos1 = con.ExecuteSQLQueryWithOneReturn($@"
                    SELECT Pos_Profile_ID1
                    FROM Employee_Profile
                    LEFT JOIN Users 
                        ON Users.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID
                    WHERE employee_code = '{primaryCode}'")?.ToString() ?? "0";
            }

            if (!string.IsNullOrEmpty(secondaryCode) && secondaryCode != "0")
            {
                pos2 = con.ExecuteSQLQueryWithOneReturn($@"
                    SELECT Pos_Profile_ID1
                    FROM Employee_Profile
                    LEFT JOIN Users 
                        ON Users.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID
                    WHERE employee_code = '{secondaryCode}'")?.ToString() ?? "0";
            }

            // Determine final value
            if (!string.IsNullOrEmpty(pos1) && pos1 != "0")
            {
                return pos1; // Primary priority
            }
            else if (!string.IsNullOrEmpty(pos2) && pos2 != "0")
            {
                return pos2; // Secondary fallback
            }

            return "0";
        }
    }
}