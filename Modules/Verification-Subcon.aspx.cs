using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;


namespace FruitUnitedMobile.Modules
{
    public partial class Verification_Subcon : System.Web.UI.Page
    {
        Connection con = new Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
			{
                txtProject.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
		SELECT									
		Project.Project_Name										
	FROM Project										
	INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID										
	INNER JOIN Customer_Profile ON Customer_Profile.Customer_Profile_ID = Quotation.Customer_Profile_ID										
	INNER JOIN										
	(										
		SELECT									
			Project_ID,								
			COUNT(1) AS Scope								
		FROM Project_Scope									
		WHERE Mobile_Visibility = 'Y'									
		GROUP BY Project_ID									
	) scopeTable ON scopeTable.Project_ID = Project.Project_ID										
	WHERE Project.Project_ID = {0}											
	ORDER BY Project.Estimated_Start_Date ASC										
 													
 ", Request.QueryString["Project_ID"])
               )?.ToString() ?? "";


                txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtDate.Attributes["type"] = "date";

                DateTime taskDate;
                if (DateTime.TryParse(txtDate.Text, out taskDate))
                {
                    // Use parameterized query to ensure SQL safety
                    string query = @"
        SELECT COUNT(1) 
        FROM Project_with_Subcon 
        WHERE Project_ID = @ProjectID 
          AND Task_Date = @TaskDate";

                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString["Project_ID"]);
                            cmd.Parameters.AddWithValue("@TaskDate", taskDate); // Pass the DateTime directly

                            int existsProject = (int)cmd.ExecuteScalar();

                            // Update radio button selection based on query result
                            if (existsProject == 1)
                            {
                                rblSubcon.SelectedValue = "Y";
                            }
                            else
                            {
                                rblSubcon.SelectedValue = "N";
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid date format.");
                }




            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate the date
            ValidateDate();

            // Check if there are any validation errors
            if (!lblDateError.Visible)
            {
                // If date is valid, process the form submission
                string selectedDate = txtDate.Text;
                string subconInvolved = rblSubcon.SelectedValue;
                DateTime taskDate;

                // Try parsing the date to DateTime format
                if (DateTime.TryParse(selectedDate, out taskDate))
                {
                    string formattedDate = taskDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    // Connection string for the database (ensure it's correct)
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
                    {
                        con.Open();

                        // Parameterized query to avoid SQL injection and ensure correct date formatting
                        string query = @"
            SELECT COUNT(1) 
            FROM Project_with_Subcon 
            WHERE Project_ID = @ProjectID 
              AND Task_Date = @TaskDate";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Add parameters for Project_ID and Task_Date
                            cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString["Project_ID"]);
                            cmd.Parameters.AddWithValue("@TaskDate", taskDate);

                            // Execute query and get the count
                            int existsProject = (int)cmd.ExecuteScalar();
                            con.Close();

                            if (existsProject == 1)
                            {
                                    // Define the query with parameters
                                    string query1 = @"
        UPDATE Project_with_Subcon
        SET Subcon = @Subcon, 
            Employee_Profile_ID = @EmployeeProfileID
        WHERE Project_ID = @ProjectID 
          AND Task_Date = @TaskDate";

                                    using (SqlCommand cmd1 = new SqlCommand(query1, con))
                                    {
                                        // Determine the value for Subcon based on the radio button selection

                                        // Add parameters to prevent SQL injection
                                        cmd1.Parameters.AddWithValue("@Subcon", rblSubcon.SelectedItem.Value);
                                        cmd1.Parameters.AddWithValue("@EmployeeProfileID", Session["EmpID"].ToString()); // Assuming userSession contains logged-in employee info
                                        cmd1.Parameters.AddWithValue("@ProjectID", Request.QueryString["Project_ID"]);
                                        cmd1.Parameters.AddWithValue("@TaskDate", taskDate);
                                       

                                        // Open the connection and execute the query
                                        con.Open();
                                        cmd1.ExecuteNonQuery();
                                        con.Close();
                                    }
                                

                            }
                            else
                            {

                                    string query2 = @"INSERT INTO Project_with_Subcon (Creation_Time, Task_Date, Subcon, Employee_Profile_ID, Project_ID)	
VALUES (GETDATE(), @TaskDate, @subcon, @empID, @projectID)	
";
                                    using (SqlCommand cmd2 = new SqlCommand(query2, con))
                                    {

                                        cmd2.Parameters.AddWithValue("@TaskDate", taskDate); 
                                        cmd2.Parameters.AddWithValue("@subcon", rblSubcon.SelectedItem.Value);
                                        cmd2.Parameters.AddWithValue("@empID", Session["EmpID"].ToString());
                                        cmd2.Parameters.AddWithValue("@projectID", Request.QueryString["Project_ID"]);
                                        con.Open();
                                        cmd2.ExecuteScalar();
                                        con.Close();
                                     }
                            }
                            
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid date format.");
                }


                

                // Add your processing logic here
                // e.g., Save data to the database or perform any other action

                // Example: Display success message (optional)
                string targetUrl = "~/Modules/Verification-Scope.aspx?Project_ID=" + Request.QueryString["Project_ID"];
                Response.Redirect(targetUrl);
            }
        }

        protected void btnToProject_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            //lb.PostBackUrl = "~/Modules/Completion-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;

            string targetUrl = "~/Modules/Verification-Project.aspx";
            Response.Redirect(targetUrl);
        }

        protected void ValidateDate()
        {
            lblDateError.Visible = false;

            DateTime enteredDate;
            if (DateTime.TryParse(txtDate.Text, out enteredDate))
            {
                var today = DateTime.Today;
                if (enteredDate != today && enteredDate != today.AddDays(-1))
                {
                    lblDateError.Text = "Date must be either today or one day earlier.";
                    lblDateError.Visible = true;
                }
            }
            else
            {
                lblDateError.Text = "Please enter a valid date.";
                lblDateError.Visible = true;
            }
        }


    }
}