using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using FruitUnitedMobile.InvoiceQRHelper;
using FruitUnitedMobile.InvoiceIssueHelper;
using FruitUnitedMobile.InvoiceNoGenerator;
using FruitUnitedMobile.ClearSessionHelper;

namespace FruitUnitedMobile.Modules
{
    public partial class Review_Invoice : System.Web.UI.Page
    {
        protected string ChargeableItemsHtml = "";
        protected string ExchangeItemsHtml = "";
        protected string OutstandingItemsHtml = "";
        protected string DeliveredItemsHtml = "";
        protected decimal SubTotal = 0m;
        protected decimal TaxAmount = 0m;
        protected decimal GrandTotal = 0m;
        protected string OutletDisplayName = "Unknown Outlet";
        protected string DeliveryDateDisplay = DateTime.Today.ToString("dd MMM yyyy (dddd)");

        private Dictionary<int, string> _reasonCache;

        // QR fields
        private List<string> qrJsonPages = new List<string>();
        private int currentQrPage = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadReviewData();   // Always load data

            if (!IsPostBack)
            {
                GenerateAndDisplayProposedInvoiceNo();
            }
        }

        private void GenerateAndDisplayProposedInvoiceNo()
        {
            lblInvoiceNo.Text = "Generating...";

            try
            {
                if (Session["DeliveryDate"] == null ||
                    Session["SelectedOutletID"] == null ||
                    Session["Pos_Profile_ID"] == null)
                {
                    lblInvoiceNo.Text = "Missing session data";
                    Toast1.ShowError("Cannot generate invoice number - missing required session data");
                    return;
                }

                DateTime delDate = Convert.ToDateTime(Session["DeliveryDate"]);
                int outletId = Convert.ToInt32(Session["SelectedOutletID"]);
                int posProfileId = Convert.ToInt32(Session["Pos_Profile_ID"]);

                string customerCode = GetCustomerCode(outletId);
                if (string.IsNullOrEmpty(customerCode))
                {
                    lblInvoiceNo.Text = "Customer code not found";
                    Toast1.ShowError("Cannot determine customer code for this outlet");
                    return;
                }

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
                {
                    con.Open();
                    // Generate preview (no transaction needed for read-only max check)
                    string proposedNo = InvoiceNoGenerate.GenerateMobileStyleInvoiceNo(
                        con, null,
                        customerCode,
                        posProfileId
                    );

                    lblInvoiceNo.Text = proposedNo;
                    hdnProposedInvoiceNo.Value = proposedNo;   // store for issue button
                    Session["InvoiceNo"] = proposedNo;
                }
            }
            catch (Exception ex)
            {
                lblInvoiceNo.Text = "Error generating number";
                Toast1.ShowError("Failed to generate invoice number: " + ex.Message);
            }
        }

        private string GetCustomerCode(int outletId)
        {
            string sql = @"
                SELECT cp.Customer_code
                FROM customer_profile cp
                LEFT JOIN OutLet_Profile op ON op.customer_profile_Id = cp.customer_profile_ID
                WHERE op.outlet_profile_ID = @OutletID";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                return res?.ToString()?.Trim()?.ToUpper();
            }
        }

        private bool DocNoAlreadyExists(SqlConnection con, SqlTransaction trans, string docNo)
        {
            string sql = "SELECT COUNT(*) FROM Invoice WHERE Doc_No = @DocNo";
            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@DocNo", docNo);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void LoadReviewData()
        {
            int outletId = 0;
            if (Session["SelectedOutletID"] != null)
            {
                outletId = Convert.ToInt32(Session["SelectedOutletID"]);
                OutletDisplayName = QRHelper.GetOutletName(outletId) ?? $"Outlet ID: {outletId}";
            }

            if (Session["DeliveryDate"] != null)
            {
                DateTime delDate = Convert.ToDateTime(Session["DeliveryDate"]);
                DeliveryDateDisplay = delDate.ToString("dd MMM yyyy (dddd)");
            }

            DataTable deliveryDT = Session["DeliveryProducts"] as DataTable;
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            if (deliveryDT == null || deliveryDT.Rows.Count == 0)
            {
                ChargeableItemsHtml = "<tr><td colspan='5' class='text-center text-muted py-5'>No delivery items</td></tr>";
                DeliveredItemsHtml = "<tr><td colspan='3' class='text-center text-muted py-5'>No items delivered</td></tr>";
                return;
            }

            _reasonCache = GetReasonDescriptions();

            var finalChargeableQty = new Dictionary<int, int>();

            foreach (DataRow row in deliveryDT.Rows)
            {
                int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                finalChargeableQty[productId] = qty;
            }

            if (returnDT != null && returnDT.Rows.Count > 0)
            {
                foreach (DataRow r in returnDT.Rows)
                {
                    int productId = Convert.ToInt32(r["Product_Profile_ID"]);
                    int returnQty = Convert.ToInt32(r["Return_Quantity"]);
                    string abbr = r["Abbreviation"]?.ToString() ?? "N/A";
                    string uom = GetUOM(productId);
                    int reasonId = Convert.ToInt32(r["Exchange_Reason_ID"]);
                    string reason = _reasonCache.ContainsKey(reasonId) ? _reasonCache[reasonId] : "Unknown";

                    bool isOutstanding = r.Table.Columns.Contains("IsOutstanding") &&
                                         r["IsOutstanding"] != DBNull.Value &&
                                         !Convert.ToBoolean(r["IsOutstanding"]); // FIX: invert logic

                    if (isOutstanding)
                    {
                        if (finalChargeableQty.ContainsKey(productId))
                        {
                            finalChargeableQty[productId] -= returnQty;
                            if (finalChargeableQty[productId] < 0) finalChargeableQty[productId] = 0;
                        }
                        OutstandingItemsHtml += $"<tr><td>{abbr}</td><td>{returnQty}</td><td>{uom}</td><td>{reason}</td></tr>";
                    }
                    else
                    {
                        ExchangeItemsHtml += $"<tr><td>{abbr}</td><td>{returnQty}</td><td>{uom}</td><td>{reason}</td></tr>";
                    }
                }
            }

            foreach (DataRow row in deliveryDT.Rows)
            {
                int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                string abbr = row["Abbreviation"]?.ToString() ?? "N/A";
                string uom = GetUOM(productId);
                int deliveredQty = Convert.ToInt32(row["Quantity"]);
                decimal price = GetCurrentSellingPrice(productId, outletId);

                int chargeableQty = finalChargeableQty.ContainsKey(productId) ? finalChargeableQty[productId] : deliveredQty;
                decimal lineTotal = chargeableQty * price;

                if (chargeableQty > 0)
                {
                    ChargeableItemsHtml += $"<tr><td>{abbr}</td><td>{chargeableQty}</td><td>{uom}</td><td>{price:N2}</td><td>{lineTotal:N2}</td></tr>";
                    SubTotal += lineTotal;
                }

                DeliveredItemsHtml += $"<tr><td>{abbr}</td><td>{deliveredQty}</td><td>{uom}</td></tr>";
            }

            if (string.IsNullOrEmpty(ExchangeItemsHtml))
                ExchangeItemsHtml = "<tr><td colspan='4' class='text-center text-muted py-4'>No exchange items</td></tr>";
            if (string.IsNullOrEmpty(OutstandingItemsHtml))
                OutstandingItemsHtml = "<tr><td colspan='4' class='text-center text-muted py-4'>No outstanding items</td></tr>";

            decimal taxRate = GetTaxRate(outletId);
            TaxAmount = Math.Round(SubTotal * taxRate, 2);
            GrandTotal = SubTotal + TaxAmount;

            // Store in session for QR helper
            Session["SubTotal"] = SubTotal;
            Session["TaxAmount"] = TaxAmount;
            Session["GrandTotal"] = GrandTotal;
        }

        // ────────────────────────────── QR CODE IMPLEMENTATION ──────────────────────────────

        protected void btnQRCode_Click(object sender, EventArgs e)
        {
            string errorMessage;
            qrJsonPages = QRHelper.GenerateQrPages(Session, out errorMessage);

            if (qrJsonPages.Count > 0)
            {
                currentQrPage = 0;
                ShowCurrentQrPage();

                string script = @"
            var header = document.getElementById('ModuleHeader');
            if (header) header.style.visibility = 'hidden';

            var qrModalEl = document.getElementById('qrModal');
            var qrModal = new bootstrap.Modal(qrModalEl, { backdrop: false });
            qrModal.show();
        ";

                ScriptManager.RegisterStartupScript(this, GetType(), "showQRAndHideHeader", script, true);
            }
            else
            {
                // Handle error
                // Toast1.ShowError(errorMessage ?? "Cannot generate QR code");
            }
        }

        private void ShowCurrentQrPage()
        {
            if (currentQrPage < 0) currentQrPage = 0;
            if (currentQrPage >= qrJsonPages.Count) currentQrPage = qrJsonPages.Count - 1;

            string json = qrJsonPages[currentQrPage];

            imgQR.ImageUrl = QRHelper.GenerateQrImageBase64(json, 20);
            imgQR.Visible = true;

            lblPageInfo.Text = $"{currentQrPage + 1} / {qrJsonPages.Count}";
            divPageInfo.Visible = qrJsonPages.Count > 1;
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            currentQrPage--;
            ShowCurrentQrPage();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            currentQrPage++;
            ShowCurrentQrPage();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Modules/Process_Outstanding.aspx");
        }

        protected void btnIssue_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["DailyLoadingID"] == null)
            {
                Toast1.ShowError("User or loading information missing.");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            int dailyLoadingId = Convert.ToInt32(Session["DailyLoadingID"]);
            int outletId = Convert.ToInt32(Session["SelectedOutletID"]);
            DateTime delDate = Convert.ToDateTime(Session["DeliveryDate"]);

            DataTable deliveryDT = Session["DeliveryProducts"] as DataTable;
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            int vehicleId = Convert.ToInt32(Session["Vehicle_Profile_ID"] ?? 0);
            int driverId = Convert.ToInt32(Session["EmpID"] ?? 0);
            int driver2Id = 0;

            if (Session["Emp2ID"] != null && !string.IsNullOrWhiteSpace(Session["Emp2ID"].ToString()))
            {
                int.TryParse(Session["Emp2ID"].ToString(), out driver2Id);
            }

            if (deliveryDT == null || deliveryDT.Rows.Count == 0)
            {
                Toast1.ShowError("No delivery data.");
                return;
            }

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Get Customer_Profile_ID
                        int customerId = 0;
                        string sqlCustomer = "SELECT Customer_Profile_ID FROM Outlet_Profile WHERE Outlet_Profile_ID = @OutletID";
                        using (SqlCommand cmd = new SqlCommand(sqlCustomer, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@OutletID", outletId);
                            object res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                                customerId = Convert.ToInt32(res);
                        }

                        // 1a. Get Customer_Code
                        string customerCode = null;
                        string sqlCustomerCode = @"
                            SELECT cp.Customer_code
                            FROM customer_profile cp
                            LEFT JOIN OutLet_Profile op ON op.customer_profile_Id = cp.customer_profile_ID
                            WHERE op.outlet_profile_ID = @OutletID";
                        using (SqlCommand cmd = new SqlCommand(sqlCustomerCode, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@OutletID", outletId);
                            object res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                                customerCode = res.ToString().Trim().ToUpper();
                        }
                        if (string.IsNullOrEmpty(customerCode))
                        {
                            throw new Exception("Cannot determine customer code for selected outlet.");
                        }

                        int posProfileId = Convert.ToInt32(Session["Pos_Profile_ID"]);

                        // 2. Use proposed number if available, otherwise regenerate
                        string docNo = hdnProposedInvoiceNo.Value;
                        if (string.IsNullOrEmpty(docNo) || DocNoAlreadyExists(con, trans, docNo))
                        {
                            docNo = InvoiceNoGenerate.GenerateMobileStyleInvoiceNo(
                                con, trans,
                                customerCode,
                                posProfileId
                            );
                            // Optional: update display after postback
                            lblInvoiceNo.Text = docNo;
                            hdnProposedInvoiceNo.Value = docNo;
                        }

                        // 3. Get currency & tax
                        int currencyId = GetCurrencyId(outletId);
                        double exchangeRate = GetExchangeRate(con, trans, outletId);
                        int taxId = GetTaxId(con, trans, outletId);

                        string recipient = "";
                        string poNo = "";
                        DateTime? poDate = null;

                        // 4. Insert Invoice header
                        int invoiceId = InvoiceIssue.InsertInvoice(
                            docNo, dailyLoadingId, outletId, delDate, userId,
                            SubTotal, TaxAmount, GrandTotal,
                            vehicleId, driverId, driver2Id,currencyId, exchangeRate, taxId,
                            recipient, poNo, poDate, customerId,
                            con, trans);

                        // 5. Recompute final chargeable quantities
                        var finalChargeableQty = new Dictionary<int, decimal>();
                        foreach (DataRow row in deliveryDT.Rows)
                        {
                            int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                            decimal qty = Convert.ToDecimal(row["Quantity"]);
                            finalChargeableQty[productId] = qty;
                        }

                        if (returnDT != null)
                        {
                            foreach (DataRow r in returnDT.Rows)
                            {
                                bool isOutstanding = r.Table.Columns.Contains("IsOutstanding") &&
                                            r["IsOutstanding"] != DBNull.Value &&
                                            !Convert.ToBoolean(r["IsOutstanding"]); // FIX

                                if (isOutstanding)
                                {
                                    int productId = Convert.ToInt32(r["Product_Profile_ID"]);
                                    decimal returnQty = Convert.ToDecimal(r["Return_Quantity"]);
                                    if (finalChargeableQty.ContainsKey(productId))
                                    {
                                        finalChargeableQty[productId] -= returnQty;
                                        if (finalChargeableQty[productId] < 0) finalChargeableQty[productId] = 0;
                                    }
                                }
                            }
                        }

                        // 6. Insert chargeable items
                        foreach (var kv in finalChargeableQty)
                        {
                            int productId = kv.Key;
                            decimal qty = kv.Value;
                            if (qty <= 0) continue;

                            decimal price = GetCurrentSellingPrice(productId, outletId);
                            decimal total = qty * price;

                            InvoiceIssue.InsertIntoLoadingMovement(
                                InvoiceIssue.InsertInvoiceItem(con, trans,
                                    invoiceId, productId, qty, price, total,
                                    "Chargeable", "Completed", 0,
                                    null, null, null, null),
                                userId, con, trans);
                        }

                        // 7. Insert exchange & outstanding items
                        if (returnDT != null && returnDT.Rows.Count > 0)
                        {
                            foreach (DataRow r in returnDT.Rows)
                            {
                                int productId = Convert.ToInt32(r["Product_Profile_ID"]);
                                decimal qty = Convert.ToDecimal(r["Return_Quantity"]);
                                int reasonId = Convert.ToInt32(r["Exchange_Reason_ID"]);

                                bool isOutstanding = r.Table.Columns.Contains("IsOutstanding") &&
                                                r["IsOutstanding"] != DBNull.Value &&
                                                !Convert.ToBoolean(r["IsOutstanding"]); // FIX

                                string itemType = isOutstanding ? "Outstanding" : "Exchange";
                                string status = isOutstanding ? "Outstanding" : "Completed";
                                decimal balance = isOutstanding ? qty : 0m;
                                bool offset = !isOutstanding;

                                int itemId = InvoiceIssue.InsertInvoiceItem(con, trans,
                                    invoiceId, productId, qty, 0, 0,
                                    itemType, status, balance,
                                    offset ? 1 : (int?)null,
                                    reasonId,
                                    !isOutstanding ? productId : (int?)null,
                                    !isOutstanding ? qty : (decimal?)null);

                                if (isOutstanding)
                                {
                                    InvoiceIssue.UpdateInvoiceOutstanding(itemId, con, trans);
                                }
                                else
                                {
                                    InvoiceIssue.InsertIntoLoadingMovement(itemId, userId, con, trans);
                                    if (offset)
                                    {
                                        InvoiceIssue.DeductOffsetOutstandingDelivery(itemId, userId, con, trans);
                                    }
                                }
                            }
                        }

                        // 8. Final stock/movement updates
                        InvoiceIssue.InsertProductMovement(dailyLoadingId, con, trans);
                        InvoiceIssue.UpdateProductBatchBalance(dailyLoadingId, con, trans);

                        Session["InvoiceID"] = invoiceId;

                        Toast1.ShowSuccess("Invoice issued successfully.");
                        trans.Commit();
                        //ClearSession.ClearInvoiceAndDeliveryRelatedData();
                        Response.Redirect("~/Modules/print_invoice.aspx");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (trans.Connection != null && trans.Connection.State != ConnectionState.Closed)
                                trans.Rollback();
                        }
                        catch { }

                        Toast1.ShowError("Error issuing invoice: " + ex.Message);
                    }
                }
            }
        }

        // ────────────────────────────── Helper methods ──────────────────────────────

        private string GetUOM(int productId)
        {
            string key = "UOM_" + productId;
            if (ViewState[key] != null) return ViewState[key].ToString();

            string uom = "Unit";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(u.UOM,'Unit') FROM Product_Profile p LEFT JOIN UOM_Profile u ON u.UOM_Profile_ID = p.UOM_Profile_ID WHERE p.Product_Profile_ID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", productId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null) uom = res.ToString();
            }
            ViewState[key] = uom;
            return uom;
        }

        private decimal GetCurrentSellingPrice(int productId, int outletId)
        {
            string key = $"{productId}_{outletId}";
            if (Session[key] != null) return (decimal)Session[key];

            decimal price = 0m;
            string sql = @"
                SELECT TOP 1 price.Selling_Price
                FROM Selling_Price_Entitled_Outlet o
                JOIN Product_Selling_Price price ON price.Product_Selling_Price_ID = o.Product_Selling_Price_ID
                JOIN Product_Selling_Price_Range r ON r.Product_Selling_Price_Range_ID = price.Product_Selling_Price_Range_ID
                WHERE r.Product_Profile_ID = @ProductID
                  AND o.Outlet_Profile_ID = @OutletID
                  AND GETDATE()-1 BETWEEN r.Date_From AND ISNULL(r.Date_To, GETDATE())
                ORDER BY r.Date_From DESC";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null) price = Convert.ToDecimal(res);
            }
            Session[key] = price;
            return price;
        }

        private Dictionary<int, string> GetReasonDescriptions()
        {
            var dict = new Dictionary<int, string>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT Exchange_Reason_ID, Reason FROM Exchange_Reason WHERE Status = 'Active'", con))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        dict[Convert.ToInt32(r["Exchange_Reason_ID"])] = r["Reason"].ToString();
                }
            }
            return dict;
        }

        private decimal GetTaxRate(int outletId)
        {
            decimal taxRate = 0.08m; // fallback
            string sql = @"
                SELECT tp.Tax_Rate 
                FROM Outlet_Profile op 
                LEFT JOIN Tax_Profile tp ON tp.Tax_Profile_ID = op.Tax_Profile_ID 
                WHERE op.Outlet_Profile_ID = @OutletID";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    taxRate = Convert.ToDecimal(res);
            }
            return taxRate;
        }

        private int GetTaxId(SqlConnection con, SqlTransaction trans, int outletId)
        {
            string sql = @"
                SELECT op.Tax_Profile_ID 
                FROM Outlet_Profile op 
                WHERE op.Outlet_Profile_ID = @OutletID";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
            }

            // Fallback to default tax profile
            sql = @"
                SELECT TOP 1 Tax_Profile_ID 
                FROM Tax_Profile 
                WHERE Default_ = 1";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
            }

            return 1;  // ultimate fallback
        }

        private int GetCurrencyId(SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
                SELECT TOP 1 Currency_Profile_ID 
                FROM Currency_Profile 
                WHERE Default_ = 1";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
            }

            return 1;  // fallback
        }

        private double GetExchangeRate(SqlConnection con, SqlTransaction trans, int outletId)
        {
            string sql = @"
                SELECT cp.Exchange_Rate 
                FROM Outlet_Profile op 
                LEFT JOIN Currency_Profile cp ON cp.Currency_Profile_ID = op.Currency_Profile_ID 
                WHERE op.Outlet_Profile_ID = @OutletID";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    return Convert.ToDouble(res);
            }

            return 1.0; // fallback
        }

        private int GetCurrencyId(int outletId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                con.Open();
                string sql = "SELECT Currency_Profile_ID FROM Outlet_Profile WHERE Outlet_Profile_ID = @OutletID";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@OutletID", outletId);
                    object res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value && Convert.ToInt32(res) > 0)
                        return Convert.ToInt32(res);
                }
            }
            return 1; // fallback
        }


    }


}