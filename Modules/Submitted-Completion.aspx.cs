using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Connection = DBConnection.Connection;

namespace FruitUnitedMobile.Modules
{
    public partial class Submitted_Completion : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                #region Project Info

                txtProject.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
			SELECT CONCAT(ISNULL(Project.Project_Name, 'N/A'), ' / ', ISNULL(Project.Project_No, 'N/A')) AS ProjectName FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}										
 													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtFrom.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	CONCAT('', ISNULL(Project_Scope.From_Building,'N/A') + ' -> ' + ISNULL(Project_Scope.From_Floor,'N/A') + ' -> ' + ISNULL(Project_Scope.From_Location,'N/A')) AS From_Location						
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtTo.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	CASE WHEN (Project_Scope.To_Building IS NOT NULL OR Project_Scope.To_Floor IS NOT NULL OR Project_Scope.To_Location IS NOT NULL)
                    THEN CONCAT('', ISNULL(Project_Scope.To_Building,'N/A') + ' -> ' + ISNULL(Project_Scope.To_Floor,'N/A') + ' -> ' + ISNULL(Project_Scope.To_Location,'N/A'))
                    ELSE '' END AS To_Location						
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtScope.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	Scope_of_Work.Scope_of_Work						
FROM Project_Scope																
INNER JOIN Scope_of_Work ON Scope_of_Work.Scope_of_Work_ID = Project_Scope.Scope_of_Work_ID																
LEFT JOIN System_Profile ON System_Profile.System_Profile_ID = Project_Scope.System_Profile_ID																
LEFT JOIN Scope_Type ON Scope_Type.Scope_Type_ID = Project_Scope.Scope_Type_ID																
LEFT JOIN Ladder_Profile ON Ladder_Profile.Ladder_Profile_ID = Project_Scope.Ladder_Profile_ID																
WHERE Project_Scope.Project_Scope_ID = {0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtScopeInfo.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"SELECT														
	Project_Scope.Scope_Info			
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
               )?.ToString() ?? "";
                #endregion

                #region Task Info
                txtTask.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT										
	Task_Profile.Task										
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                txtTools.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
	SELECT							
	Tool_Profile.Tool							
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                txtRemark.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
	SELECT							
	task.Remark						
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                txtCompletedQty.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    CONCAT(task.Completed_Qty, ' / ', Project_Scope.Qty) AS QTY						
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                txtCompletedPercentage.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    task.Actual_Percent				
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";


                ops.Value = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Task_Profile.Score_Point AS Ops_Point			
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                average.Value = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Task_Profile.Average_Score_Point AS Average_Point				
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                highest.Value = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Task_Profile.Highest_Score_Point AS Highest_Point				
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";

                txtTaskInfo.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
	SELECT							
	task.Task_Info					
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString() ?? "";
                #endregion

                #region Enter value 
                txtQty.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Qty			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
                )?.ToString() ?? "";

                txtNoOfMan.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    No_of_Man			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
                )?.ToString() ?? "";

                txtNoOfHours.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    No_of_Hour			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
                )?.ToString() ?? "";

                //txtOps.Text = (float.Parse(ops.Value) * float.Parse(txtNoOfMan.Text) * float.Parse(txtNoOfHours.Text)).ToString();
                //txtAverage.Text = (float.Parse(average.Value) * float.Parse(txtNoOfMan.Text) * float.Parse(txtNoOfHours.Text)).ToString();
                //txtHighest.Text = (float.Parse(highest.Value) * float.Parse(txtNoOfMan.Text) * float.Parse(txtNoOfHours.Text)).ToString();

                float opsValue, averageValue, highestValue, noOfMan, noOfHours;

                // Safely parse the inputs, defaulting to 0 if parsing fails.
                float.TryParse(ops.Value, out opsValue);
                float.TryParse(average.Value, out averageValue);
                float.TryParse(highest.Value, out highestValue);
                float.TryParse(txtNoOfMan.Text, out noOfMan);
                float.TryParse(txtNoOfHours.Text, out noOfHours);

                // Calculate the values.
                txtOps.Text = (opsValue * noOfMan * noOfHours).ToString("F2");
                txtAverage.Text = (averageValue * noOfMan * noOfHours).ToString("F2");
                txtHighest.Text = (highestValue * noOfMan * noOfHours).ToString("F2");


                txtCompletionRemark.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Remark			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
                )?.ToString() ?? "";

                string employee_id = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
	SELECT			
    employee_profile_id			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
               )?.ToString() ?? "";

                txtBy.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	select Employee_Name from Employee_Profile where Employee_Profile_ID = {0}							
									
 													
 ", employee_id)
                )?.ToString() ?? "";


                txt_completion_date.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    Completion_Date			
FROM Project_Task_Completion task														
WHERE Project_Task_Completion_ID = {0}								
									
 													
 ", Request.QueryString["Project_Task_Completion_ID"])
                )?.ToString() ?? "";

                #endregion
                BindDropdown();
                LoadUploadedFiles();
            }

        }

        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Retrieve values from textboxes
            decimal qty = ParseDecimal(txtQty.Text);
            decimal noOfMan = ParseDecimal(txtNoOfMan.Text);
            decimal noOfHours = ParseDecimal(txtNoOfHours.Text);

            // Perform calculations
            decimal ops = noOfMan * noOfHours;
            decimal average = (ops + qty) / 2; // Example formula for Average
            decimal highest = Math.Max(ops, Math.Max(average, qty)); // Example formula for Highest

            // Update the output fields
            txtOps.Text = $"{ops:F2} Ops";
            txtAverage.Text = $"{average:F2} Average";
            txtHighest.Text = $"{highest:F2} Highest";
        }

        private decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return 0;
        }

        private void BindDropdown()
        {



            float completedQty = float.Parse(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
    task.Completed_Qty				
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString());

            float scopeQty = float.Parse(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
	SELECT			
Project_Scope.Qty AS Scope_Qty FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_Task_ID = {0}								
ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
 ", Request.QueryString["Project_Scope_Task_ID"])
                )?.ToString());

            if (!string.IsNullOrEmpty(txtQty.Text))
            {
                if ((float.Parse(txtQty.Text) + completedQty) > scopeQty)
                {
                    ddlQtyExceededReason.Enabled = true;

                    string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString; // Replace with your actual connection string
                    string query = "SELECT Exceeded_Reason_ID, Reason FROM Exceeded_Reason";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            ddlQtyExceededReason.DataSource = reader;
                            ddlQtyExceededReason.DataTextField = "Reason"; // Text displayed in dropdown
                            ddlQtyExceededReason.DataValueField = "Exceeded_Reason_ID"; // Value for each item
                            ddlQtyExceededReason.DataBind();

                            // Add default "Select Reason" option at the top
                            ddlQtyExceededReason.Items.Insert(0, new ListItem("Select Reason", ""));
                        }
                    }
                }
                else
                {
                    ddlQtyExceededReason.Enabled = false;


                }
            }
            else
            {
                ddlQtyExceededReason.Enabled = false;

            }

        }

        protected void btnToProject_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            //lb.PostBackUrl = "~/Modules/Completion-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;

            string targetUrl = "~/Modules/Submitted-WorkScope.aspx";
            Response.Redirect(targetUrl);
        }

        protected void btnToScope_Click(object sender, EventArgs e)
        {

            // Example of passing query parameters
            string targetUrl = "~/Modules/Submitted-Scope.aspx?Project_ID=" + Request.QueryString["Project_ID"];
            Response.Redirect(targetUrl);
        }

        protected void btnToTask_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            string targetUrl = "~/Modules/Submitted-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "DELETE FROM Project_Task_Completion WHERE Project_Task_Completion_ID = @Project_Task_Completion_ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Project_Task_Completion_ID", Request.QueryString["Project_Task_Completion_ID"]);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectTaskCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectTaskID", Request.QueryString["Project_Scope_Task_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectScopeCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectScopeID", Request.QueryString["Project_Scope_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectBillingInfo", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectID", Request.QueryString["Project_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            string targetUrl = "~/Modules/Submitted-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters

            string score_point = (float.Parse(txtQty.Text) / float.Parse(txtNoOfMan.Text) * float.Parse(txtNoOfHours.Text)).ToString();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Project_Task_Completion SET Qty = @Qty,Remark = @Remark,Exceeded_Reason_ID = @Exceeded_Reason_ID, Score_Point = @Score_Point WHERE Project_Task_Completion_ID = @Project_Task_Completion_ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Qty", txtQty.Text);
                    cmd.Parameters.AddWithValue("@Remark", txtRemark.Text);
                    cmd.Parameters.AddWithValue("@Exceeded_Reason_ID",
                        string.IsNullOrEmpty(ddlQtyExceededReason.SelectedItem?.Value) ? (object)DBNull.Value : (object)ddlQtyExceededReason.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@Score_Point", score_point);
                    cmd.Parameters.AddWithValue("@Project_Task_Completion_ID", Request.QueryString["Project_Task_Completion_ID"]);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Project_Task_Completion SET Employee_Profile_ID1 = @empId, Verified_On = GETDATE() " +
                    "WHERE Project_Task_Completion_ID = @Project_Task_Completion_ID AND Verified_On IS NULL";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@empId", Session["EmpID"].ToString());
                    cmd.Parameters.AddWithValue("@Project_Task_Completion_ID", Request.QueryString["Project_Task_Completion_ID"]);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectTaskCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectTaskID", Request.QueryString["Project_Scope_Task_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectScopeCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectScopeID", Request.QueryString["Project_Scope_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectBillingInfo", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectID", Request.QueryString["Project_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            //        string script = "alert('Confirmed Successfully!'); window.location.href = '~/Modules/Verification-Task.aspx?Project_ID=" +
            //Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"] + "';";
            //        ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);

            string targetUrl = "~/Modules/Submitted-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);



        }

        private void LoadUploadedFiles()
        {
            string projectId = Request.QueryString["Project_Task_Completion_ID"];
            if (string.IsNullOrEmpty(projectId))
            {
                return; // No ID, nothing to show
            }

            string baseDir = ConfigurationManager.AppSettings["SystemPath"];
            string baseFolderPath = baseDir + @"Document\Project_Task_Completion\Completion File";
            string targetFolder = Path.Combine(baseFolderPath, projectId);

            List<FileData> fileList = new List<FileData>();

            if (Directory.Exists(targetFolder))
            {
                string[] files = Directory.GetFiles(targetFolder);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.ToLower() == ".xml") continue; // Skip XML files

                    fileList.Add(new FileData
                    {
                        FileName = fileInfo.Name,
                        FilePath = fileInfo.FullName // Use full path for secure download
                    });
                }
            }

            if (fileList.Count > 0)
            {
                rptFiles.DataSource = fileList;
                rptFiles.DataBind();
                pnlFiles.Visible = true; // Show the table only if files exist
            }
        }

        // Handle File Download
        protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Download")
            {
                string filePath = e.CommandArgument.ToString();
                if (File.Exists(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.WriteFile(file.FullName);
                    Response.End();
                }
                else
                {
                    lblMessage.Text = "File wasn't available on site.";
                    lblMessage.Visible = true;
                }
            }
        }

        // Model for file details
        public class FileData
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
    }
}