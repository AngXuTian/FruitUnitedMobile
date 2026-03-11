using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Connection = DBConnection.Connection;

namespace FruitUnitedMobile.Modules
{
    public partial class Completion_TaskCompletion : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
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

                BindDropdown();
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

            string targetUrl = "~/Modules/Completion-Project.aspx";
            Response.Redirect(targetUrl);
        }

        protected void btnToScope_Click(object sender, EventArgs e)
        {

            // Example of passing query parameters
            string targetUrl = "~/Modules/Completion-Scope.aspx?Project_ID=" + Request.QueryString["Project_ID"];
            Response.Redirect(targetUrl);
        }

        protected void btnToTask_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            string targetUrl = "~/Modules/Completion-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int projectTaskCompletionID = 0;
            string score_point = (float.Parse(txtQty.Text) / float.Parse(txtNoOfMan.Text) * float.Parse(txtNoOfHours.Text)).ToString();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = @"INSERT INTO Project_Task_Completion 
                     (Creation_Time, Completion_Date, Employee_Profile_ID, Qty, No_of_Man, No_of_Hour, Remark, Exceeded_Reason_ID, Project_Scope_Task_ID, Score_Point) 
                     OUTPUT INSERTED.Project_Task_Completion_ID
                     SELECT GETDATE(), GETDATE(), @Employee_Profile_ID, @Entered_Qty, @Entered_No_of_Man, @Entered_No_of_Hr, 
                            @Entered_Remark, @Selected_Exceeded_Reason, Project_Scope_Task_ID, @score_point 
                     FROM Project_Scope_Task 
                     WHERE Project_Scope_Task_ID = @Project_Scope_Task_ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Employee_Profile_ID", Session["EmpID"]);
                    cmd.Parameters.AddWithValue("@Entered_Qty", txtQty.Text);
                    cmd.Parameters.AddWithValue("@Entered_No_of_Man", txtNoOfMan.Text);
                    cmd.Parameters.AddWithValue("@Entered_No_of_Hr", txtNoOfHours.Text);
                    cmd.Parameters.AddWithValue("@Entered_Remark", txtRemark.Text);
                    cmd.Parameters.AddWithValue("@Selected_Exceeded_Reason",

                    string.IsNullOrEmpty(ddlQtyExceededReason.SelectedItem?.Value) ? (object)DBNull.Value : (object)ddlQtyExceededReason.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@score_point", score_point);
                    cmd.Parameters.AddWithValue("@Project_Scope_Task_ID", Request.QueryString["Project_Scope_Task_ID"]);
                    con.Open();
                    projectTaskCompletionID = (int)cmd.ExecuteScalar();
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



            //C:\Development\UCA\Application\Comnet\Document\Project_Task_Completion\Image
            //string baseDir = ConfigurationManager.AppSettings["ImagesPath"];
            //string targetDir = Path.Combine(baseDir, projectTaskCompletionID.ToString());


            //// Check if files are uploaded
            //if (fileUploadImages.HasFiles)
            //{
            //    if (!Directory.Exists(targetDir))
            //    {
            //        Directory.CreateDirectory(targetDir);
            //    }
            //    foreach (HttpPostedFile uploadedFile in fileUploadImages.PostedFiles)
            //    {
            //        // Generate the target file path
            //        string fileName = Path.GetFileName(uploadedFile.FileName);
            //        string filePath = Path.Combine(targetDir, fileName);

            //        // Save the file
            //        uploadedFile.SaveAs(filePath);
            //    }
            //}
            if (fileUploadPDF.HasFiles)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Project_Task_Completion\Completion File\";
                string targetDir = Path.Combine(baseDir, projectTaskCompletionID.ToString());
                string xmlFilePath = Path.Combine(targetDir, "user_" + projectTaskCompletionID.ToString() + ".xml");

                // Ensure the target directory exists
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Create XML document if it doesn't exist
                if (!File.Exists(xmlFilePath))
                {
                    XDocument xmlDoc4 = new XDocument(
                        new XDeclaration("1.0", "utf-16", "yes"),
                        new XElement("NewDataSet")
                    );
                    xmlDoc4.Save(xmlFilePath);
                }


                XDocument xmlDoc = XDocument.Load(xmlFilePath);
                int fileId = xmlDoc.Descendants("UserFile").Count(); // Get the last file ID

                foreach (HttpPostedFile uploadedFile in fileUploadPDF.PostedFiles)
                {
                    // Generate the target file path
                    string fileName = Path.GetFileName(uploadedFile.FileName);
                    string filePath = Path.Combine(targetDir, fileName);

                    // Save the file
                    uploadedFile.SaveAs(filePath);

                    // Get file details
                    FileInfo fileInfo = new FileInfo(filePath);
                    long fileSize = fileInfo.Length; // File size in bytes
                    string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                    DateTime currentDate = DateTime.Now;

                    // Add file details to XML
                    xmlDoc.Root.Add(new XElement("UserFile",
                        new XElement("File_ID", fileId++),
                        new XElement("Name", fileName),
                        new XElement("User", currentUser),
                        new XElement("Date_Modified", currentDate.ToString("M/d/yyyy h:mm:ss tt")),
                        new XElement("Size", fileSize),
                        new XElement("Owner", currentUser),
                        new XElement("OwnerID", Session["EmpID"]), // Replace with actual OwnerID
                        new XElement("Remarks"),
                        new XElement("Action", "Upload"),
                        new XElement("Executor", currentUser),
                        new XElement("ExecutorID", Session["EmpID"].ToString()), // Replace with actual ExecutorID
                        new XElement("Path", targetDir.Replace(@"\", "/"))
                    ));
                }

                // Save the updated XML document
                xmlDoc.Save(xmlFilePath);

            }

            //Upload test report to project
            if (fileUploadTestReport.HasFiles)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Project\Test Report\";
                string targetDir = Path.Combine(baseDir, Request.QueryString["Project_ID"].ToString());
                string xmlFilePath = Path.Combine(targetDir, "user_" + Request.QueryString["Project_ID"].ToString() + ".xml");

                // Ensure the target directory exists
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Create XML document if it doesn't exist
                if (!File.Exists(xmlFilePath))
                {
                    XDocument xmlDoc5 = new XDocument(
                        new XDeclaration("1.0", "utf-16", "yes"),
                        new XElement("NewDataSet")
                    );
                    xmlDoc5.Save(xmlFilePath);
                }


                XDocument xmlDocTest = XDocument.Load(xmlFilePath);
                int fileId = xmlDocTest.Descendants("UserFile").Count(); // Get the last file ID

                foreach (HttpPostedFile uploadedFile in fileUploadTestReport.PostedFiles)
                {
                    // Generate the target file path
                    string fileName = Path.GetFileName(uploadedFile.FileName);
                    string filePath = Path.Combine(targetDir, fileName);

                    // Save the file
                    uploadedFile.SaveAs(filePath);

                    // Get file details
                    FileInfo fileInfo = new FileInfo(filePath);
                    long fileSize = fileInfo.Length; // File size in bytes
                    string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                    DateTime currentDate = DateTime.Now;

                    // Add file details to XML
                    xmlDocTest.Root.Add(new XElement("UserFile",
                        new XElement("File_ID", fileId++),
                        new XElement("Name", fileName),
                        new XElement("User", currentUser),
                        new XElement("Date_Modified", currentDate.ToString("M/d/yyyy h:mm:ss tt")),
                        new XElement("Size", fileSize),
                        new XElement("Owner", currentUser),
                        new XElement("OwnerID", Session["EmpID"]), // Replace with actual OwnerID
                        new XElement("Remarks"),
                        new XElement("Action", "Upload"),
                        new XElement("Executor", currentUser),
                        new XElement("ExecutorID", Session["EmpID"].ToString()), // Replace with actual ExecutorID
                        new XElement("Path", targetDir.Replace(@"\", "/"))
                    ));
                }

                // Save the updated XML document
                xmlDocTest.Save(xmlFilePath);

            }


            // Process uploaded files
            if (fileUploadImages.HasFiles)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Project_Task_Completion\Image\";
                string targetDir = Path.Combine(baseDir, projectTaskCompletionID.ToString());
                string xmlFilePath = Path.Combine(targetDir, "user_" + projectTaskCompletionID.ToString() + ".xml");

                // Ensure the target directory exists
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Create XML document if it doesn't exist
                if (!File.Exists(xmlFilePath))
                {
                    XDocument xmlDoc3 = new XDocument(
                        new XDeclaration("1.0", "utf-16", "yes"),
                        new XElement("NewDataSet")
                    );
                    xmlDoc3.Save(xmlFilePath);
                }

                XDocument xmlDoc2 = XDocument.Load(xmlFilePath);
                int fileId = xmlDoc2.Descendants("UserFile").Count(); // Get the last file ID

                foreach (HttpPostedFile uploadedFile in fileUploadImages.PostedFiles)
                {
                    // Generate the target file path
                    string fileName = Path.GetFileName(uploadedFile.FileName);
                    string filePath = Path.Combine(targetDir, fileName);

                    // Save the file
                    uploadedFile.SaveAs(filePath);

                    // Get file details
                    FileInfo fileInfo = new FileInfo(filePath);
                    long fileSize = fileInfo.Length; // File size in bytes
                    string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                    DateTime currentDate = DateTime.Now;

                    // Add file details to XML
                    xmlDoc2.Root.Add(new XElement("UserFile",
                        new XElement("File_ID", fileId++),
                        new XElement("Name", fileName),
                        new XElement("User", currentUser),
                        new XElement("Date_Modified", currentDate.ToString("M/d/yyyy h:mm:ss tt")),
                        new XElement("Size", fileSize),
                        new XElement("Owner", currentUser),
                        new XElement("OwnerID", Session["EmpID"]), // Replace with actual OwnerID
                        new XElement("Remarks"),
                        new XElement("Action", "Upload"),
                        new XElement("Executor", currentUser),
                        new XElement("ExecutorID", Session["EmpID"].ToString()), // Replace with actual ExecutorID
                        new XElement("Path", targetDir.Replace(@"\", "/"))
                    ));
                }

                // Save the updated XML document
                xmlDoc2.Save(xmlFilePath);
            }

          


                //        string script = "alert('Submitted Successfully!'); window.location.href = '~/Modules/Completion-Task.aspx?Project_ID=" +
                //Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"] + "';";
                //        ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);


                //// Example of passing query parameters
                string targetUrl = "~/Modules/Completion-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);
        }

    }
}