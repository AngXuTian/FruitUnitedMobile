using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using QRCoder;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace FruitUnitedMobile.InvoiceQRHelper
{
    /// <summary>
    /// Helper class for generating Invoice QR Codes - C# 4.0 compatible
    /// </summary>
    public static class QRHelper
    {
        private const string COMPANY_CODE = "71793";
        private const int ITEMS_PER_PAGE = 20;

        public static List<string> GenerateQrPages(
            HttpSessionState session,
            out string errorMessage)
        {
            errorMessage = null;
            List<string> qrJsonPages = new List<string>();

            DataTable deliveryDT = session["DeliveryProducts"] as DataTable;
            DataTable returnDT = session["ReturnProducts"] as DataTable;

            if (deliveryDT == null || deliveryDT.Rows.Count == 0)
            {
                errorMessage = "No delivery items found in session";
                return qrJsonPages;
            }

            int outletId = Convert.ToInt32(session["SelectedOutletID"] ?? "0");
            string driverName = session["Employee_Name"] != null
                ? session["Employee_Name"].ToString()
                : "Unknown Driver";

            string outletCode = GetOutletCode(outletId);
            if (string.IsNullOrEmpty(outletCode)) outletCode = "Unknown";

            // TODO: Replace with real invoice number logic
            string invNo = session["InvoiceNo"] != null ?
                session["InvoiceNo"].ToString() : "Unknown Invoice No";

            string formattedDate = MonthConvertor(DateTime.Now.ToString("yyyy-MM-dd"));

            Dictionary<int, int> finalChargeableQty = new Dictionary<int, int>();
            //Dictionary<int, int> honoredReturnQtys = new Dictionary<int, int>();

            // 1. Load delivery quantities
            foreach (DataRow row in deliveryDT.Rows)
            {
                int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                finalChargeableQty[pid] = qty;
            }

            // 2. Process all returns — track total returned qty for QR "R" field
            Dictionary<int, int> totalReturnQtys = new Dictionary<int, int>(); // new: total returns for "R"

            if (returnDT != null && returnDT.Rows.Count > 0)
            {
                foreach (DataRow r in returnDT.Rows)
                {
                    int pid = Convert.ToInt32(r["Product_Profile_ID"]);
                    int rQty = Convert.ToInt32(r["Return_Quantity"]);

                    // Accumulate total returned quantity (for QR "R")
                    if (!totalReturnQtys.ContainsKey(pid))
                        totalReturnQtys[pid] = 0;
                    totalReturnQtys[pid] += rQty;

                    // Only subtract from chargeable if it's NOT outstanding (honored/exchange)
                    bool isOutstanding = false;
                    if (r.Table.Columns.Contains("IsOutstanding"))
                    {
                        isOutstanding = Convert.ToBoolean(r["IsOutstanding"]);
                    }

                    if (!isOutstanding && finalChargeableQty.ContainsKey(pid))
                    {
                        int deduct = Math.Min(finalChargeableQty[pid], rQty);
                        finalChargeableQty[pid] -= deduct;
                        // We can keep honoredReturnQtys if you still need it elsewhere
                        // honoredReturnQtys[pid] = deduct;   // optional now
                    }
                }
            }

            // 3. Build item list (short field names like Java)
            List<object> items = new List<object>();
            int lineNo = 1;

            foreach (DataRow row in deliveryDT.Rows)
            {
                int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                int gross = Convert.ToInt32(row["Quantity"]);
                if (gross == 0) continue;

                int net = finalChargeableQty.ContainsKey(pid) ? finalChargeableQty[pid] : gross;
                // ── Changed: use total returned qty instead of only honored ──
                int returned = totalReturnQtys.ContainsKey(pid) ? totalReturnQtys[pid] : 0;

                decimal price = GetCurrentSellingPrice(pid, outletId, session);
                string barcode = GetBarcode(pid, session);
                if (string.IsNullOrEmpty(barcode)) barcode = "";

                var item = new
                {
                    N = lineNo.ToString(),
                    E = barcode,
                    G = gross.ToString(),
                    R = returned.ToString(),
                    U = price.ToString("F2"),
                    F = "N"     // Change to "Y" when you detect FOC items
                };

                items.Add(item);
                lineNo++;
            }

            if (items.Count == 0)
            {
                errorMessage = "No valid items for QR after processing";
                return qrJsonPages;
            }

            // 4. Split into pages
            int totalPages = (int)Math.Ceiling((double)items.Count / ITEMS_PER_PAGE);

            for (int p = 0; p < totalPages; p++)
            {
                int skip = p * ITEMS_PER_PAGE;
                List<object> pageItems = new List<object>();

                for (int i = skip; i < skip + ITEMS_PER_PAGE && i < items.Count; i++)
                {
                    pageItems.Add(items[i]);
                }

                // You should preferably calculate these once outside the loop
                string subTotalStr = session["SubTotal"] != null ? session["SubTotal"].ToString() : "0.00";
                string taxStr = session["TaxAmount"] != null ? session["TaxAmount"].ToString() : "0.00";
                string grandStr = session["GrandTotal"] != null ? session["GrandTotal"].ToString() : "0.00";

                int totalGrossQty = 0;
                foreach (var it in items)
                {
                    dynamic dyn = it;
                    totalGrossQty += Convert.ToInt32(dyn.G);
                }

                var payload = new
                {
                    S = COMPANY_CODE,
                    I = invNo,
                    D = formattedDate,
                    IT = pageItems,
                    TN = totalGrossQty.ToString(),
                    SU = subTotalStr,
                    TX = taxStr,
                    GT = grandStr,
                    DI = "0.00",
                    DN = driverName,
                    P = (p + 1).ToString(),
                    M = totalPages.ToString(),
                    SC = outletCode
                };

                string json = JsonConvert.SerializeObject(payload);
                qrJsonPages.Add(json);
            }
            session["QR_Code"] = qrJsonPages;

            return qrJsonPages;
        }

        // ────────────────────────────────────────────────
        //   Helper methods (C# 4.0 compatible)
        // ────────────────────────────────────────────────

        private static string GetBarcode(int productId, HttpSessionState session)
        {
            string key = "Barcode_" + productId.ToString();
            if (session[key] != null) return session[key].ToString();

            string barcode = "";
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT Barcode FROM Product_Profile WHERE Product_Profile_ID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", productId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    barcode = res.ToString();
            }

            session[key] = barcode;
            return barcode;
        }

        public static decimal GetCurrentSellingPrice(int productId, int outletId, HttpSessionState session)
        {
            string key = "Price_" + productId.ToString() + "_" + outletId.ToString();
            if (session[key] != null) return Convert.ToDecimal(session[key]);

            decimal price = 0m;
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

            string sql = @"
                SELECT TOP 1 price.Selling_Price
                FROM Selling_Price_Entitled_Outlet o
                JOIN Product_Selling_Price price ON price.Product_Selling_Price_ID = o.Product_Selling_Price_ID
                JOIN Product_Selling_Price_Range r ON r.Product_Selling_Price_Range_ID = price.Product_Selling_Price_Range_ID
                WHERE r.Product_Profile_ID = @ProductID
                  AND o.Outlet_Profile_ID = @OutletID
                  AND GETDATE()-1 BETWEEN r.Date_From AND ISNULL(r.Date_To, GETDATE())
                ORDER BY r.Date_From DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    price = Convert.ToDecimal(res);
            }

            session[key] = price;
            return price;
        }

        public static string MonthConvertor(string dateInNumber)
        {
            if (string.IsNullOrEmpty(dateInNumber) || dateInNumber.Length < 10) return dateInNumber;

            string day = dateInNumber.Substring(8, 2);
            string month = dateInNumber.Substring(5, 2);
            string year = dateInNumber.Substring(0, 4);

            string monthStr = "";
            switch (month)
            {
                case "01": monthStr = "JAN"; break;
                case "02": monthStr = "FEB"; break;
                case "03": monthStr = "MAR"; break;
                case "04": monthStr = "APR"; break;
                case "05": monthStr = "MAY"; break;
                case "06": monthStr = "JUN"; break;
                case "07": monthStr = "JUL"; break;
                case "08": monthStr = "AUG"; break;
                case "09": monthStr = "SEP"; break;
                case "10": monthStr = "OCT"; break;
                case "11": monthStr = "NOV"; break;
                case "12": monthStr = "DEC"; break;
                default: monthStr = month; break;
            }

            return day + "-" + monthStr + "-" + year;
        }

        public static string GenerateQrImageBase64(string jsonContent, int pixelsPerModule)
        {
            try
            {
                QRCodeGenerator qrGen = new QRCodeGenerator();
                QRCodeData data = qrGen.CreateQrCode(jsonContent, QRCodeGenerator.ECCLevel.Q);
                QRCode code = new QRCode(data);

                using (Bitmap bmp = code.GetGraphic(pixelsPerModule))
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    return "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetOutletName(int outletId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT Outlet_Name FROM Outlet_Profile WHERE Outlet_Profile_ID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                return res != null && res != DBNull.Value ? res.ToString() : "";
            }
        }

        public static string GetOutletCode(int outletId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT Outlet_Number FROM Outlet_Profile WHERE Outlet_Profile_ID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", outletId);
                con.Open();
                object res = cmd.ExecuteScalar();
                return res != null && res != DBNull.Value ? res.ToString() : "";
            }
        }
    }
}