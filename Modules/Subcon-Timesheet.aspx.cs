using DBConnection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile.Modules
{
    public partial class Subcon_Timesheet : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString; // Replace with your actual connection string

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindDropdown();
            }
        }

        private void BindDropdown()
        {

            string query = "SELECT Project_ID, Project_No + ' / ' + Project_Name AS ProjectDisplay FROM Project WHERE Status IN ('Open','Ongoing') AND Mobile_Visibility = 'Y'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlProject.DataSource = reader;
                    ddlProject.DataTextField = "ProjectDisplay"; // Text displayed in dropdown
                    ddlProject.DataValueField = "Project_ID"; // Value for each item
                    ddlProject.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlProject.Items.Insert(0, new ListItem("-- Please Select --", ""));
                }
            }

            string query2 = "SELECT Supplier_Profile.Supplier_Profile_ID, Supplier_Profile.Supplier_Name FROM Supplier_Profile INNER JOIN Company_Type_Profile ON Company_Type_Profile.Company_Type_Profile_ID = Supplier_Profile.Company_Type_Profile_ID WHERE Supplier_Profile.Status = 'Active' AND Company_Type_Profile.Company_Type = 'Subcon'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query2, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSubcon.DataSource = reader;
                    ddlSubcon.DataTextField = "Supplier_Name"; // Text displayed in dropdown
                    ddlSubcon.DataValueField = "Supplier_Profile_ID"; // Value for each item
                    ddlSubcon.DataBind();

                    ddlSubcon.Items.Insert(0, new ListItem("-- Please Select --", ""));

                    //// Add default "Select Reason" option at the top
                    //ddlQtyExceededReason.Items.Insert(0, new ListItem("Select Reason", ""));
                }
            }

        }

        protected void ddlSubcon_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected Supplier_Profile_ID from ddlSubcon
            int subconId = int.Parse(ddlSubcon.SelectedValue);

            // Query to fetch Supplier Workers based on the selected Subcon
            string query3 = "SELECT Supplier_Worker.Supplier_Worker_ID, Supplier_Worker.Display_Name FROM Supplier_Worker WHERE Supplier_Worker.Status = 'Active' AND Supplier_Worker.Supplier_Profile_ID = " + subconId + @" ORDER BY Supplier_Worker.Display_Name ASC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand(query3, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                chkListItems.DataSource = dr;
                chkListItems.DataTextField = "Display_Name";  // What the user will see
                chkListItems.DataValueField = "Supplier_Worker_ID";   // The value for each checkbox
                chkListItems.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in chkListItems.Items)
            {
                if (item.Selected) // Check if the checkbox is ticked
                {
                    string selectedWorker = item.Value;
                    DateTime enteredDateIn = DateTime.Parse(txtDateIn.Text + " " + txtTimeIn.Text);
                    DateTime enteredDateOut = DateTime.Parse(txtDateOut.Text + " " + txtTimeOut.Text);

                    // SQL query to check for overlapping timesheet
                    string checkOverlapQuery = @"
                        SELECT COUNT(1) 
                        FROM Subcon_Timesheet 
                        WHERE Supplier_Worker_ID = @WorkerID
                        AND (
                            @EnteredDateIn BETWEEN 
                                CONVERT(datetime, Date_In + ' ' + CONVERT(varchar, Time_In, 108)) 
                                AND 
                                DATEADD(minute, -1, CONVERT(datetime, Date_Out + ' ' + CONVERT(varchar, Time_Out, 108)))
                            OR
                            @EnteredDateOut BETWEEN 
                                DATEADD(minute, +1,  CONVERT(datetime, Date_In + ' ' + CONVERT(varchar, Time_In, 108)))
                                AND 
                                CONVERT(datetime, Date_Out + ' ' + CONVERT(varchar, Time_Out, 108))
                        )
                    ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Check for overlap
                        using (SqlCommand cmd = new SqlCommand(checkOverlapQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@WorkerID", selectedWorker);
                            cmd.Parameters.AddWithValue("@EnteredDateIn", enteredDateIn);
                            cmd.Parameters.AddWithValue("@EnteredDateOut", enteredDateOut);

                            int overlapCount = (int)cmd.ExecuteScalar();

                            if (overlapCount > 0)
                            {
                                // Overlap found, show alert
                                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Overlap timesheet for the selected worker');", true);
                                return;
                            }
                        }
                    }
                }
            }

            foreach (ListItem item in chkListItems.Items)
            {
                if (item.Selected) // Check if the checkbox is ticked
                {
                    string selectedWorker = item.Value;
                    DateTime enteredDateIn = DateTime.Parse(txtDateIn.Text + " " + txtTimeIn.Text);
                    DateTime enteredDateOut = DateTime.Parse(txtDateOut.Text + " " + txtTimeOut.Text);

                    // Initialize image variable
                    string imagePath = null;

                    // Handle file upload
                    if (fileUploadImage.HasFile)
                    {
                        // Ensure the folder exists
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"];
                        string uploadFolder = baseDir + @"Source\UploadImages\Subcon_Timesheet\";
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Get file extension and validate type
                        string fileExtension = Path.GetExtension(fileUploadImage.FileName).ToLower();
                        string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid file type. Please upload an image file.');", true);
                            return;
                        }

                        // Save file
                        string fileName = Path.GetFileName(fileUploadImage.FileName); // Keep original file name
                        string savePath = Path.Combine(uploadFolder, fileName);
                        fileUploadImage.SaveAs(savePath);

                        // Save relative path to database
                        imagePath = fileName;
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // No overlap, insert data
                        string insertQuery = @"
                            INSERT INTO Subcon_Timesheet 
                            (
                                Creation_Time, Employee_Profile_ID, Project_ID, Supplier_Profile_ID, Supplier_Worker_ID,
                                Date_In, Time_In, Date_Out, Time_Out, Break_Min,Break_Hour, Image_In
                            )
                            VALUES 
                            (
                                GETDATE(), @EmployeeID, @ProjectID, @SubconID, @WorkerID,
                                @DateIn, @TimeIn, @DateOut, @TimeOut, @BreakMin ,@BreakHour, @Image
                            )

                            SELECT SCOPE_IDENTITY();
                        ";

                        int timesheetID = 0;
                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@EmployeeID", Session["EmpID"]);
                            cmdInsert.Parameters.AddWithValue("@ProjectID", ddlProject.SelectedValue);
                            cmdInsert.Parameters.AddWithValue("@SubconID", ddlSubcon.SelectedValue);
                            cmdInsert.Parameters.AddWithValue("@WorkerID", selectedWorker);
                            cmdInsert.Parameters.AddWithValue("@DateIn", txtDateIn.Text);
                            cmdInsert.Parameters.AddWithValue("@TimeIn", txtTimeIn.Text);
                            cmdInsert.Parameters.AddWithValue("@DateOut", txtDateOut.Text);
                            cmdInsert.Parameters.AddWithValue("@TimeOut", txtTimeOut.Text);
                            cmdInsert.Parameters.AddWithValue("@BreakMin", string.IsNullOrWhiteSpace(txtBreakHour.Text) ? (object)DBNull.Value : (decimal.Parse(txtBreakHour.Text)));
                            cmdInsert.Parameters.AddWithValue("@BreakHour", string.IsNullOrWhiteSpace(txtBreakHour.Text) ? (object)DBNull.Value : (decimal.Parse(txtBreakHour.Text) / 60));
                            cmdInsert.Parameters.AddWithValue("@Image", imagePath ?? DBNull.Value.ToString());

                            timesheetID = Convert.ToInt32(cmdInsert.ExecuteScalar());
                        }

                        // Call stored procedure
                        using (SqlCommand collectionCmd = new SqlCommand("Update_SubconTimesheet", conn))
                        {
                            collectionCmd.CommandType = CommandType.StoredProcedure;
                            collectionCmd.Parameters.AddWithValue("@timesheetID", timesheetID);
                            collectionCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            // Success message
            //ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Timesheet submitted successfully!');", true);
            string targetUrl = "~/Modules/Menu.aspx";
            Response.Redirect(targetUrl);
        }
    }
}