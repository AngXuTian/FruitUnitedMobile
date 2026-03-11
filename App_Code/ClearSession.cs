using System;
using System.Collections.Generic;
using System.Web;

namespace FruitUnitedMobile.ClearSessionHelper
{
    public static class ClearSession
    {
        // Keys that should NEVER be removed (login/employee/vehicle context)
        private static readonly HashSet<string> KeysToPreserve = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "EmpCode",
            "EmpID",
            "Employee_Name",
            "UserID",
            "Pos_Profile_ID",
            "Emp2Code",
            "Emp2ID",
            "Emp2Name",
            "Vehicle_Profile_ID",
            "Vehicle_No"
            // Add more if needed: "IsAuthenticated", "Language", etc.
        };

        /// <summary>
        /// Clears all invoice/delivery/shopping cart related session variables,
        /// while keeping user login, employee and vehicle information intact.
        /// </summary>
        public static void ClearInvoiceAndDeliveryRelatedData()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
                return;

            var session = HttpContext.Current.Session;

            // 1. Collect keys to remove (can't modify Keys while iterating)
            var keysToRemove = new List<string>();

            foreach (string key in session.Keys)
            {
                if (!KeysToPreserve.Contains(key))
                {
                    keysToRemove.Add(key);
                }
            }

            // 2. Actually remove them
            foreach (string key in keysToRemove)
            {
                session.Remove(key);
            }

            // 3. Extra safety – explicitly remove known invoice/delivery keys
            //    (in case they were not caught above or have different casing)
            string[] knownInvoiceKeys = new[]
            {
                "DeliveryDate",
                "SelectedDate",
                "SelectedOutletID",
                "SelectedDeliveryOutletID",
                "DailyLoadingID",
                "DeliveryProducts",
                "ReturnProducts",
                "PlanID",
                "PlanName",
                "DeliveryDayID",
                "SubTotal",
                "TaxAmount",
                "GrandTotal",
                "hdnProposedInvoiceNo",
                // add others you know exist in your app
            };

            foreach (var key in knownInvoiceKeys)
            {
                session.Remove(key);
            }

            // Optional: if you store many temporary/cached items with patterns
            // (prices, barcodes, UOMs, etc.)
            // foreach (string key in session.Keys)
            // {
            //     if (key.StartsWith("Price_") || key.StartsWith("UOM_") || key.StartsWith("Cache_"))
            //         session.Remove(key);
            // }
        }

        // Bonus: small convenience methods you might also want
        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current?.Session["UserID"] != null ||
                   HttpContext.Current?.Session["EmpID"] != null;
        }

        public static void ClearAllSession()
        {
            HttpContext.Current?.Session?.Clear();
        }
    }
}
