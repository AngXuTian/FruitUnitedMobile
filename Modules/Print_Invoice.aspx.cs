using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections.Generic;

namespace FruitUnitedMobile.Modules
{
    public partial class Print_Invoice : BasePage
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["InvoiceID"] == null || Session["SelectedOutletID"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }
                BindInvoiceData();
            }
        }

        private void BindInvoiceData()
        {
            try
            {
                DataSet ds = FetchInvoiceData();

                if (ds != null && ds.Tables.Count >= 6)
                {
                    MapCompany(ds.Tables[0]);
                    MapOutlet(ds.Tables[1]);
                    MapTax(ds.Tables[2]);
                    MapMain(ds.Tables[3]);
                    MapCurrency(ds.Tables[4]);
                    MapCustomer(ds.Tables[5]);
                }

                BuildReceiptContentFromSession();
            }
            catch (Exception ex)
            {
                litChargeableRows.Text = $"<tr><td colspan='5' style='color:red;text-align:center'>Error: {ex.Message}</td></tr>";
            }
        }

        private DataSet FetchInvoiceData()
        {
            string sql = @"
                SELECT TOP 1 Company_Name, Logo_Filename, Address, Fax_No, Email, Website, GST_Reg_No, Co_Reg_No, Phone_No FROM Company_Profile;
                SELECT outlet_name, Outlet_Number, Address, Abbreviation, Postcode FROM Outlet_Profile WHERE outlet_profile_id = @OutID;
                SELECT tp.Tax_Rate FROM Tax_Profile tp LEFT JOIN Outlet_Profile op ON op.Tax_Profile_ID = tp.Tax_Profile_ID WHERE op.outlet_profile_id = @OutID;
                SELECT doc_no, doc_date, Amount, Tax_Amount, Total_Amount FROM invoice WHERE invoice_id = @InvID;
                SELECT cp.Currency_Code FROM Currency_Profile cp LEFT JOIN Outlet_Profile op ON op.Currency_Profile_ID = cp.Currency_Profile_ID WHERE op.Outlet_Profile_ID = @OutID;
                SELECT TOP 1 cp.Customer_Name, ca.Address FROM Customer_Profile cp LEFT JOIN Invoice inv ON inv.Customer_Profile_ID = cp.Customer_Profile_ID LEFT JOIN customer_address ca on ca.customer_profile_id = cp.customer_profile_id WHERE inv.Invoice_ID = @InvID;";

            using (SqlConnection con = new SqlConnection(_connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@OutID", Session["SelectedOutletID"]);
                    cmd.Parameters.AddWithValue("@InvID", Session["InvoiceID"]);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }

        private void MapCompany(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;
            var r = dt.Rows[0];
            litCompName.Text = r["Company_Name"].ToString();
            litCompAddr.Text = r["Address"].ToString();
            litCompPhone.Text = r["Phone_No"].ToString();
            litCoReg.Text = r["Co_Reg_No"].ToString();
            imgLogo.ImageUrl = "~/Images/" + r["Logo_Filename"].ToString();
        }

        private string outletName = "";
        private string outletAddr = "";
        private string outletInfo = "";
        private void MapOutlet(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;
            var r = dt.Rows[0];
            litOutletCode.Text = r["Outlet_Number"].ToString();
            outletName = r["outlet_name"].ToString();
            outletAddr = r["Address"].ToString();
            string postcode = r["Postcode"]?.ToString() ?? "";
            string abbr = r["Abbreviation"]?.ToString() ?? "N/A";
            outletInfo = $"{abbr} / {r["Outlet_Number"]} ({postcode})".Trim();
        }

        private decimal taxRateDecimal = 0m;
        private string taxRateStr = "0";
        private void MapTax(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                taxRateDecimal = Convert.ToDecimal(dt.Rows[0]["Tax_Rate"]);
                taxRateStr = (taxRateDecimal * 100).ToString("N0");
            }
        }

        private string invNo = "";
        private string invDate = "";
        private void MapMain(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;
            var r = dt.Rows[0];
            invNo = r["doc_no"].ToString();
            invDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Set invoice number for PDF filename
            litInvoiceNoForFilename.Text = invNo;
        }

        private string currency = "SGD";
        private void MapCurrency(DataTable dt)
        {
            currency = dt.Rows.Count > 0 ? dt.Rows[0]["Currency_Code"].ToString() : "SGD";
        }

        private void MapCustomer(DataTable dt) { }

        private void BuildReceiptContentFromSession()
        {
            // Centered header
            string header = $"Invoice NO : {invNo}<br />" +
                            $"{invDate}<br />" +
                            $"{outletName}<br />" +
                            $"{outletAddr}<br />" +
                            $"Outlet : {outletInfo}";
            litInvoiceHeader.Text = header;

            StringBuilder chSb = new StringBuilder();
            StringBuilder totSb = new StringBuilder();
            StringBuilder exSb = new StringBuilder();
            StringBuilder delSb = new StringBuilder();

            DataTable deliveryDT = Session["DeliveryProducts"] as DataTable;
            DataTable returnDT = Session["ReturnProducts"] as DataTable;

            if (deliveryDT == null || deliveryDT.Rows.Count == 0)
            {
                chSb.Append("<tr><td colspan='6' style='text-align:center;padding:8px;'>No items delivered</td></tr>");
                exSb.Append("<tr><td colspan='4' style='text-align:center;padding:8px;'>No exchange items</td></tr>");
                delSb.Append("<tr><td colspan='3' style='text-align:center;padding:8px;'>No delivered items</td></tr>");
                litChargeableRows.Text = chSb.ToString();
                litTotals.Text = "";
                litExchangeRows.Text = exSb.ToString();
                litDeliveredRows.Text = delSb.ToString();
                return;
            }

            var reasonCache = GetReasonDescriptions();
            var finalChargeable = new Dictionary<int, decimal>();
            var exchangeQty = new Dictionary<int, decimal>();

            decimal subTotal = 0m;
            int outletId = Convert.ToInt32(Session["SelectedOutletID"]);

            foreach (DataRow row in deliveryDT.Rows)
            {
                int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                decimal qty = Convert.ToDecimal(row["Quantity"]);
                finalChargeable[pid] = qty;
                exchangeQty[pid] = 0m;
            }

            if (returnDT != null)
            {
                foreach (DataRow r in returnDT.Rows)
                {
                    int pid = Convert.ToInt32(r["Product_Profile_ID"]);
                    decimal retQty = Convert.ToDecimal(r["Return_Quantity"]);

                    if (finalChargeable.ContainsKey(pid))
                        finalChargeable[pid] = Math.Max(0, finalChargeable[pid] - retQty);

                    exchangeQty[pid] = exchangeQty.ContainsKey(pid) ? exchangeQty[pid] + retQty : retQty;
                }
            }

            // ====================== CHARGEABLE (single line per item with (SKU)) ======================
            foreach (DataRow row in deliveryDT.Rows)
            {
                int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                string abbr = row["Abbreviation"]?.ToString() ?? "N/A";
                string sku = GetSKU(pid);

                decimal chQty = finalChargeable.ContainsKey(pid) ? finalChargeable[pid] : 0m;
                decimal exQty = exchangeQty.ContainsKey(pid) ? exchangeQty[pid] : 0m;
                decimal price = GetCurrentSellingPrice(pid, outletId);
                decimal amt = chQty * price;

                if (chQty > 0 || exQty > 0)
                {
                    chSb.Append($@"
            <tr class='chargeable-item-row'>
                <td class='item-name'>{Server.HtmlEncode(abbr)}</td>
                <td class='sku'>({Server.HtmlEncode(sku)})</td>
                <td class='qty'>{chQty:N0}</td>
                <td class='exc'>{exQty:N0}</td>
                <td class='up'>{price:N2}</td>
                <td class='amt'>{amt:N2}</td>
            </tr>");
                    subTotal += amt;
                }
            }

            // Totals
            decimal taxAmt = Math.Round(subTotal * taxRateDecimal, 2);
            decimal grandTotal = subTotal + taxAmt;

            //totSb.Append($"<tr class='total-row'><td class='label' colspan='5'>SUB TOTAL</td><td class='value amt'>{currency} {subTotal:N2}</td></tr>");
            //totSb.Append($"<tr class='total-row'><td class='label' colspan='5'>GST {taxRateStr}%</td><td class='value amt'>{currency} {taxAmt:N2}</td></tr>");
            //totSb.Append($"<tr class='total-row'><td class='label' colspan='5'>GRAND TOTAL</td><td class='value amt'>{currency} {grandTotal:N2}</td></tr>");
            totSb.Append($@"
    <tr class='total-row'>
        <td class='label'>SUB TOTAL</td>
        <td class='currency'>{currency}</td>
        <td class='value amt'>{subTotal:N2}</td>
    </tr>");

            totSb.Append($@"
    <tr class='total-row'>
        <td class='label'>GST {taxRateStr}%</td>
        <td class='currency'>{currency}</td>
        <td class='value amt'>{taxAmt:N2}</td>
    </tr>");

            totSb.Append($@"
    <tr class='total-row'>
        <td class='label'>GRAND TOTAL</td>
        <td class='currency'>{currency}</td>
        <td class='value amt'>{grandTotal:N2}</td>
    </tr>");

            // ====================== EXCHANGE (2 lines) ======================
            if (returnDT != null && returnDT.Rows.Count > 0)
            {
                foreach (DataRow r in returnDT.Rows)
                {
                    int pid = Convert.ToInt32(r["Product_Profile_ID"]);
                    string abbr = r["Abbreviation"]?.ToString() ?? "N/A";
                    string sku = GetSKU(pid);
                    decimal qty = Convert.ToDecimal(r["Return_Quantity"]);
                    int reasonId = Convert.ToInt32(r["Exchange_Reason_ID"]);
                    string reason = reasonCache.ContainsKey(reasonId) ? reasonCache[reasonId] : "Unknown";

                    exSb.Append($@"
            <tr class='single-item-row'>
                <td class='item-name'>{Server.HtmlEncode(abbr)}</td>
                <td class='sku'>({Server.HtmlEncode(sku)})</td>
                <td class='qty'>{qty:N0}</td>
                <td class='reason'>{Server.HtmlEncode(reason)}</td>
            </tr>");
                }
            }
            else
            {
                exSb.Append("<tr><td colspan='4' style='text-align:center; padding:6px;'>No exchange items</td></tr>");
            }

            // ====================== TOTAL DELIVERED (2 lines) ======================
            foreach (DataRow row in deliveryDT.Rows)
            {
                int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                string abbr = row["Abbreviation"]?.ToString() ?? "N/A";
                string sku = GetSKU(pid);
                decimal qty = Convert.ToDecimal(row["Quantity"]);

                delSb.Append($@"
        <tr class='single-item-row'>
            <td class='item-name'>{Server.HtmlEncode(abbr)}</td>
            <td class='sku'>({Server.HtmlEncode(sku)})</td>
            <td class='qty'>{qty:N0}</td>
        </tr>");
            }

            litChargeableRows.Text = chSb.ToString();
            litTotals.Text = totSb.ToString();
            litExchangeRows.Text = exSb.ToString();
            litDeliveredRows.Text = delSb.ToString();
        }
        // Helpers (unchanged from previous version)
        private string GetUOM(int productId)
        {
            string key = "UOM_" + productId;
            if (ViewState[key] != null) return ViewState[key].ToString();

            string uom = "Unit";
            string sql = "SELECT ISNULL(u.UOM,'Unit') FROM Product_Profile p LEFT JOIN UOM_Profile u ON u.UOM_Profile_ID = p.UOM_Profile_ID WHERE p.Product_Profile_ID = @ID";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ID", productId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null) uom = res.ToString();
            }
            ViewState[key] = uom;
            return uom;
        }

        private string GetSKU(int productId)
        {
            string key = "SKU_" + productId;
            if (ViewState[key] != null) return ViewState[key].ToString();

            string sku = "N/A";
            string sql = "SELECT ISNULL(sku, 'N/A') FROM Product_Profile WHERE Product_Profile_ID = @ID";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ID", productId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null) sku = res.ToString();
            }
            ViewState[key] = sku;
            return sku;
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

            using (SqlConnection con = new SqlConnection(_connStr))
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
            string sql = "SELECT Exchange_Reason_ID, Reason FROM Exchange_Reason WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
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

        protected void btnHome_Click(object sender, EventArgs e) => Response.Redirect("~/modules/menu.aspx");
    }
}