using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FruitUnitedMobile.Common
{
    /// <summary>
    /// Helper class for stock validation and availability checks
    /// </summary>
    public static class StockHelper
    {
        /// <summary>
        /// Gets previous outstanding balance (from past invoices) for products at a specific outlet
        /// </summary>
        public static Dictionary<int, int> GetPreviousOutstandingBalance(int outletId, List<int> productIds)
        {
            var balances = new Dictionary<int, int>();
            if (productIds == null || productIds.Count == 0 || outletId <= 0)
                return balances;

            string productIdList = string.Join(",", productIds);
            string query = @"
                SELECT
                    ii.Product_Profile_ID,
                    SUM(ii.Balance) AS Balance
                FROM Invoice_Items ii
                INNER JOIN Invoice i ON i.Invoice_ID = ii.Invoice_ID
                WHERE
                    ii.Balance > 0
                    AND ii.Item_Type = 'Outstanding'
                    AND ii.Status = 'Outstanding'
                    AND i.Outlet_Profile_ID = @OutletId
                    AND ii.Product_Profile_ID IN (" + productIdList + @")
                GROUP BY ii.Product_Profile_ID";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OutletId", outletId);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["Product_Profile_ID"]);
                                int balance = Convert.ToInt32(reader["Balance"]);
                                balances[productId] = balance;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving previous outstanding balance: " + ex.Message, ex);
            }

            return balances;
        }

        /// <summary>
        /// Gets today's loaded (delivered) balance from Daily_Loading for a vehicle
        /// </summary>
        public static Dictionary<int, int> GetTodayLoadedBalance(int vehicleId, List<int> productIds)
        {
            var balances = new Dictionary<int, int>();
            if (productIds == null || productIds.Count == 0 || vehicleId <= 0)
                return balances;

            string productIdList = string.Join(",", productIds);
            string query = @"
                SELECT 
                    DLI.Product_Profile_ID,
                    SUM(DLI.Balance) AS Balance
                FROM Daily_Loading_Items DLI
                INNER JOIN Daily_Loading DL ON DLI.Daily_Loading_ID = DL.Daily_Loading_ID
                WHERE 
                    DL.Vehicle_Profile_ID = @VehicleId
                    AND DL.Status = 'Loaded'
                    AND DLI.Product_Profile_ID IN (" + productIdList + @")
                GROUP BY DLI.Product_Profile_ID";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["Product_Profile_ID"]);
                                int balance = Convert.ToInt32(reader["Balance"]);
                                balances[productId] = balance;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving today's loaded balance: " + ex.Message, ex);
            }

            return balances;
        }

        /// <summary>
        /// Validates return items for Process_Outstanding page
        /// Returns list of validation issues with reasons
        /// </summary>
        public static List<ReturnValidationIssue> ValidateReturnForOutstanding(
            DataTable returnItems,
            DataTable deliveryItems,
            int outletId,
            int vehicleId)
        {
            var issues = new List<ReturnValidationIssue>();

            if (returnItems == null || returnItems.Rows.Count == 0)
                return issues;

            // Get lists of product IDs
            var returnProductIds = returnItems.AsEnumerable()
                .Select(r => Convert.ToInt32(r["Product_Profile_ID"]))
                .ToList();

            // Get previous outstanding balances
            var previousOutstanding = GetPreviousOutstandingBalance(outletId, returnProductIds);

            // Get today's loaded (delivered) quantities
            var todayLoaded = GetTodayLoadedBalance(vehicleId, returnProductIds);

            // Build delivery lookup
            var deliveryLookup = new Dictionary<int, int>();
            if (deliveryItems != null)
            {
                foreach (DataRow row in deliveryItems.Rows)
                {
                    int pid = Convert.ToInt32(row["Product_Profile_ID"]);
                    int qty = Convert.ToInt32(row["Quantity"]);
                    deliveryLookup[pid] = qty;
                }
            }

            foreach (DataRow row in returnItems.Rows)
            {
                int productId = Convert.ToInt32(row["Product_Profile_ID"]);
                int returnQty = Convert.ToInt32(row["Return_Quantity"]);
                string abbr = row["Abbreviation"]?.ToString() ?? "Unknown Product";

                // 1. Previous outstanding exists → must be Outstanding = YES
                int prevOut = previousOutstanding.ContainsKey(productId) ? previousOutstanding[productId] : 0;
                if (prevOut > 0)
                {
                    issues.Add(new ReturnValidationIssue
                    {
                        ProductId = productId,
                        ProductName = abbr,
                        IssueType = ReturnIssueType.MustBeOutstanding_PreviousBalance,
                        Details = $"Has previous outstanding balance: {prevOut}",
                        RecommendedOutstanding = true
                    });
                }

                // 2. Return Qty > Delivery Qty → cannot offset → must be Outstanding = NO
                int deliveryQty = deliveryLookup.ContainsKey(productId) ? deliveryLookup[productId] : 0;
                if (returnQty > deliveryQty)
                {
                    issues.Add(new ReturnValidationIssue
                    {
                        ProductId = productId,
                        ProductName = abbr,
                        IssueType = ReturnIssueType.MustBeNonOutstanding_ExcessReturn,
                        Details = $"Return ({returnQty}) exceeds delivery ({deliveryQty})",
                        RecommendedOutstanding = false
                    });
                }

                // 3. Product not in today's delivery at all → cannot be Outstanding = YES
                if (deliveryQty == 0 && returnQty > 0)
                {
                    issues.Add(new ReturnValidationIssue
                    {
                        ProductId = productId,
                        ProductName = abbr,
                        IssueType = ReturnIssueType.MustBeNonOutstanding_NotDeliveredToday,
                        Details = "Not included in today's delivery plan",
                        RecommendedOutstanding = false
                    });
                }
            }

            return issues;
        }

        /// <summary>
        /// Represents a validation issue for return items in outstanding processing
        /// </summary>
        public class ReturnValidationIssue
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public ReturnIssueType IssueType { get; set; }
            public string Details { get; set; }
            public bool RecommendedOutstanding { get; set; }

            public override string ToString()
            {
                return $"{ProductName}: {Details} → Outstanding should be {(RecommendedOutstanding ? "YES" : "NO")}";
            }
        }

        public enum ReturnIssueType
        {
            MustBeOutstanding_PreviousBalance,
            MustBeNonOutstanding_ExcessReturn,
            MustBeNonOutstanding_NotDeliveredToday
        }

        // Keep old methods for backward compatibility if needed elsewhere
        [Obsolete("Use GetPreviousOutstandingBalance instead")]
        public static Dictionary<int, int> GetOutstandingBalance(int outletId, List<int> productIds)
        {
            return GetPreviousOutstandingBalance(outletId, productIds);
        }

        public class StockIssue
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Available { get; set; }
            public int Requested { get; set; }
            public override string ToString()
            {
                return $"{ProductName} (Available: {Available}, Requested: {Requested})";
            }
        }
    }
}