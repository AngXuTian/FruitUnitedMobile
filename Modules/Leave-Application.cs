using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;

namespace FruitUnitedMobile.Modules
{
    public partial class Leave_Application : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdown();
            }
            if (Request.Browser.IsMobileDevice)
            {
                fileUpload1.Attributes.Add("capture", "environment"); // Use back camera
                fileUpload1.Attributes.Add("accept", "image/*");
            }
        }

        private void BindDropdown()
        {

            string query = "SELECT Leave_Profile_ID, Leave_Type FROM Leave_Profile WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlType.DataSource = reader;
                    ddlType.DataTextField = "Leave_Type"; // Text displayed in dropdown
                    ddlType.DataValueField = "Leave_Profile_ID"; // Value for each item
                    ddlType.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlType.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string rbOfficeStr = "N";

            string selectedType = ddlType.SelectedValue;
            string selectedSession = ddlSession.SelectedValue;
            string dateFromTxt = txtDateFrom.Text;
            string dateToTxt = txtDateTo.Text;
            string fileRequire = con.ExecuteSQLQueryWithOneReturn(
                                    string.Format(@"SELECT Require_File FROM Leave_Profile WHERE Leave_Profile_ID = {0}", selectedType)).ToString();

            if (selectedSession != "Full Day")
            {
                dateToTxt = dateFromTxt;
            }

            DateTime today = DateTime.Today;

            if (fileRequire == "Y" && DateTime.Parse(dateFromTxt) > today)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No forward date is allowed for selected leave');", true);
                return;
            }

           string overlapQuery = @"
           SELECT COUNT(1) FROM Leave_Dates INNER JOIN Leave_Application ON Leave_Application.Leave_Application_ID = Leave_Dates.Leave_Application_ID
                WHERE Leave_Application.Employee_Profile_ID = '{0}'
                        AND Leave_Application.Status <> 'Cancelled'
                        AND CONVERT(date, Leave_Dates.Leave_Date) BETWEEN CONVERT(date, '{1}') AND CONVERT(date, '{2}')
                        AND (Leave_Application.Session = 'Full Day' OR Leave_Application.Session = {3})
            ";

           string overlapLeave = con.ExecuteSQLQueryWithOneReturn(
                                        string.Format(overlapQuery, Session["EmpID"], dateFromTxt, dateToTxt, selectedSession)).ToString();

            if (overlapLeave != "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Overlap date');", true);
                return;
            }

            /*
            string insertQuery = @"
           INSERT INTO Worker_Timesheet				
           (				
               Creation_Time,			
               Employee_Profile_ID,			
               Office,			
               Project_ID,			
               Shift_Type,			
               Clock_Type_In,			
               Date_In,			
               Actual_Time_In,
               System_Time_In,
               Image_In,			
               Remarks,
               Type
           )				
           VALUES				
           (				
               GETDATE(),			
               @EmployeeID,			
               @Office,			
               @Project_ID,			
               @Shift_Type,			
               @Clock_Type,			
               @System_Date,			
               @Actual_Time,
               CAST(GETDATE() AS Time),
               @Image,			
               @Remark,
               @selectedType
           )			
           SELECT SCOPE_IDENTITY();		

       ";
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               conn.Open();

               int timesheetID = 0;
               using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
               {
                   cmdInsert.Parameters.AddWithValue("@EmployeeID", Session["EmpID"]);
                   cmdInsert.Parameters.AddWithValue("@Office", rbOfficeStr);
                   if (ddlProject == "0")
                   {
                       cmdInsert.Parameters.AddWithValue("@Project_ID", DBNull.Value);
                   }
                   else
                   {
                       cmdInsert.Parameters.AddWithValue("@Project_ID", ddlProject);
                   }
                   cmdInsert.Parameters.AddWithValue("@Shift_Type", selectedShiftType);
                   cmdInsert.Parameters.AddWithValue("@Clock_Type", selectedClockType);
                   cmdInsert.Parameters.AddWithValue("@System_Date", today.ToString("yyyy-MM-dd"));
                   cmdInsert.Parameters.AddWithValue("@Actual_Time", txtTimeIn.Text);
                   cmdInsert.Parameters.AddWithValue("@Image", imagePath ?? DBNull.Value.ToString());
                   cmdInsert.Parameters.AddWithValue("@Remark", txtRemark.Text);
                   cmdInsert.Parameters.AddWithValue("@selectedType", selectedType);
                   cmdInsert.Parameters.AddWithValue("@isLate", isLate);


                   timesheetID = Convert.ToInt32(cmdInsert.ExecuteScalar());
               }

               //Call stored procedure
               using (SqlCommand collectionCmd = new SqlCommand("Update_WorkerTimesheet", conn))
               {
                   collectionCmd.CommandType = CommandType.StoredProcedure;
                   collectionCmd.Parameters.AddWithValue("@timesheetID", timesheetID);
                   collectionCmd.ExecuteNonQuery();
               }
            */
            string targetUrl = "~/Modules/Menu.aspx";
               Response.Redirect(targetUrl);
            
           }
        }
    }
}