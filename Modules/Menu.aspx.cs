using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.DynamicData;
using iTextSharp.text;
using ForSessionValue;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using ZXing.QrCode;
using Image = System.Drawing.Image;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using ZXing.Common;
using ZXing;
using System.IO;
using FruitUnitedMobile.ClearSessionHelper;

namespace FruitUnitedMobile.Modules
{
    public partial class Menu : BasePage
    {
        Connection con = new Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                //Button1.Visible = GetModuleStatus("Test Module");
                //Button2.Visible = GetModuleStatus("Work Scope Completion");
                //Button3.Visible = GetModuleStatus("Work Scope Verification");
                //Button4.Visible = GetModuleStatus("Work Scope Add-On");
                //Button5.Visible = GetModuleStatus("Daily Transport Booking");
                //Button6.Visible = GetModuleStatus("Equipment Rental / Maintenance");
                //Button7.Visible = GetModuleStatus("Inventory GRN");
                //Button8.Visible = GetModuleStatus("Inventory Return");

                //Button1.Visible = IsModuleAccessible("Test Module");
                //Button2.Visible = IsModuleAccessible( "Work Scope Completion");
                //Button3.Visible = IsModuleAccessible( "Work Scope Verification");
                //Button4.Visible = IsModuleAccessible( "Work Scope Add-On");
                //Button5.Visible = IsModuleAccessible( "Daily Transport Booking");
                //Button6.Visible = IsModuleAccessible( "Equipment Rental / Maintenance");
                //Button7.Visible = IsModuleAccessible( "Inventory GRN");
                //Button8.Visible = IsModuleAccessible( "Inventory Return");

            }
            GenerateModuleButtons();
            ClearSession.ClearInvoiceAndDeliveryRelatedData();
        }

        protected void Export_ServerClick(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Modules/BookingSummary.aspx");
        }

        protected void Import_ServerClick(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Modules/ReceivingList.aspx");
        }

        private bool GetModuleStatus(string moduleName)
        {
            string query = "SELECT Status FROM Mobile_Module WHERE Module_Name = @ModuleName";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString.ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ModuleName", moduleName);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && result.ToString() == "Active"; // Assuming 'Active' is the value for active modules
                }
            }
        }

        protected void ModuleButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string moduleName = clickedButton.CommandArgument;

            //string work_timesheet_in = con.ExecuteSQLQueryWithOneReturn(
            //        string.Format(@"SELECT Worker_Timesheet_ID FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Date_In IS NOT NULL AND Date_Out IS NULL", Session["EmpID"])
            //    )?.ToString();

            //string work_timesheet_out = con.ExecuteSQLQueryWithOneReturn(
            //        string.Format(@"SELECT Worker_Timesheet_ID FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Date_In IS NOT NULL AND Date_Out IS NULL", Session["EmpID"])
            //    )?.ToString();

            //string notifyOnID = con.ExecuteSQLQueryWithOneReturn(
            //        string.Format(@"SELECT TOP 1 Notify_On_ID FROM Notify_On WHERE DATEDIFF(Day, Notify_On, GETDATE()) BETWEEN 0 AND 7 ORDER BY Notify_On.Notify_On DESC"))?.ToString();

            //if (string.IsNullOrEmpty(notifyOnID))
            //    notifyOnID = "0";

            //string notifyAcknowledgement = con.ExecuteSQLQueryWithOneReturn(string.Format(@"SELECT TOP 1 Notify_Acknowledge_ID FROM Notify_Acknowledge WHERE Employee_Profile_ID = ISNULL(NULLIF({0}, ''), 0) AND Notify_On_ID = ISNULL(NULLIF({1}, ''), 0)", Session["EmpID"], notifyOnID.ToString()))?.ToString();

            // Redirect based on module
            switch (moduleName)
            {
                case "Load Vehicle":
                    Response.Redirect("~/Modules/LoadVehicle.aspx");
                    break;

                case "Delivery Plan":
                    Response.Redirect("~/Modules/Delivery_Plan.aspx");
                    break;

                case "Complete Delivery":
                    Response.Redirect("~/Modules/CompleteDelivery.aspx");
                    break;

                //case "Worker Timesheet":
                //    if (string.IsNullOrEmpty(work_timesheet_in))
                //    {
                //        Response.Redirect("~/Modules/Worker-Timesheet-In.aspx");
                //        return;
                //    }
                //    if (!string.IsNullOrEmpty(work_timesheet_out))
                //    {
                //        Response.Redirect("~/Modules/Worker-Timesheet-Out.aspx?worker_timesheet_id=" + work_timesheet_out);
                //        return;
                //    }
                //    break;
                default:

                    break;
            }
        }

        private void GenerateModuleButtons()
        {
            string query = @"
  SELECT M.Mobile_Module_ID, M.Module_Name, M.Display_Name, M.Status, ISNULL(E.Available,'N') AS Available, M.S_N
  FROM Mobile_Module M
  LEFT JOIN Employee_Mobile_Access E
    ON M.Mobile_Module_ID = E.Mobile_Module_ID
  WHERE E.Employee_Profile_ID = @Employee_Profile_ID
  ORDER BY M.S_N ASC"; 

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Employee_Profile_ID", Session["EmpID"]);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string moduleName = reader["Module_Name"].ToString();
                            string displayName = reader["Display_Name"].ToString();
                            string status = reader["Status"]?.ToString() ?? "Inactive";
                            string available = reader["Available"]?.ToString() ?? "N";

                            if (status == "Active" && available == "Y")
                            {
                                // Create button dynamically
                                Button button = new Button
                                {
                                    Text = displayName,
                                    CssClass = "btn btn-outline-dark btn-custom",
                                    ID = "btn" + moduleName.Replace(" ", ""),
                                    CommandArgument = moduleName
                                };

                                //if (moduleName == "WFH History" && WFHHistoryHighlight != "0")
                                //{
                                //    button.BackColor = Color.Yellow; // Highlight yellow
                                //}


                                // Attach a command event handler
                                button.Click += ModuleButton_Click;

                                // Add button to the placeholder
                                ModuleContainer.Controls.Add(button);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("check");
        }

        private bool IsModuleAccessible(string moduleName)
        {
            string query = "SELECT Employee_Mobile_Access.Available FROM Employee_Mobile_Access INNER JOIN Mobile_Module ON Mobile_Module.Mobile_Module_ID = Employee_Mobile_Access.Mobile_Module_ID WHERE Employee_Mobile_Access.Employee_Profile_ID = @Employee_Profile_ID AND Mobile_Module.Module_Name = @ModuleName";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString.ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Employee_Profile_ID", Session["EmpID"]);
                    cmd.Parameters.AddWithValue("@ModuleName", moduleName);
                    con.Open();
                    var result = cmd.ExecuteScalar() ?? 'N'; // Set result to 1 if null
                    if (result.ToString() == "Y")
                    {
                        return true;
                    }
                    else
                    {
                        // Handle the case where the input is null or invalid
                        return false;
                    }


                }
            }
        }



    }
}