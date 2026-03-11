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
using System.Xml.Linq;

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
            string selectedType = ddlType.SelectedValue;
            string selectedSession = ddlSession.SelectedValue;
            string dateFromTxt = txtDateFrom.Text;
            string dateToTxt = txtDateTo.Text;
            if (selectedType == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select a Type.');", true);
                return;
            }
            if (selectedSession == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select a Session.');", true);
                return;
            }

            //If full day, to date cannot be empty
            if (selectedSession == "Full Day" && dateToTxt == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('To date cannot be empty.');", true);
                return;
            }

            //If full day, to date cannot be earlier than from date
            if (dateFromTxt != "" && dateToTxt != "")
            {
                DateTime dateFrom = DateTime.Parse(dateFromTxt);
                DateTime dateTo = DateTime.Parse(dateToTxt);

                if (dateTo < dateFrom) 
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('To Date cannot be earlier than From Date.');", true);
                    return;
                }
            }

            //Default to date same as from date for session AM & PM
            if (selectedSession != "Full Day")
            {
                dateToTxt = dateFromTxt;
            }

            //Check if selected leave type can apply for future date
            DateTime today = DateTime.Today;
            string allowFwdDate = con.ExecuteSQLQueryWithOneReturn(
                        string.Format(@"SELECT Allow_Fwd_Date FROM Leave_Profile WHERE Leave_Profile_ID = '{0}'", selectedType)).ToString();

            if (allowFwdDate == "N" && DateTime.Parse(dateFromTxt) > today)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No forward date is allowed for selected leave');", true);
                return;
            }

            //Check if selected leave type required upload file
            string fileRequire = con.ExecuteSQLQueryWithOneReturn(
                        string.Format(@"SELECT Require_File FROM Leave_Profile WHERE Leave_Profile_ID = '{0}'", selectedType)).ToString();

            if (fileRequire == "Y" && !fileUpload.HasFiles)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Upload of file is required for selected leave');", true);
                return;
            }

            //Check if overlap date
            string overlapSQL = string.Format(@"
                                        SELECT COUNT(1) FROM Leave_Dates INNER JOIN Leave_Application ON Leave_Application.Leave_Application_ID = Leave_Dates.Leave_Application_ID
                WHERE Leave_Application.Employee_Profile_ID = '{0}' 
                        AND Leave_Application.Status <> 'Cancelled'
                        AND CONVERT(date, Leave_Dates.Leave_Date) BETWEEN CONVERT(date, '{1}') AND CONVERT(date, '{2}')
                        AND (Leave_Application.Session = 'Full Day' OR Leave_Application.Session = CASE WHEN ('{3}' = 'Full Day') THEN Leave_Application.Session ELSE '{3}' END)
                                        ", Session["EmpID"], dateFromTxt, dateToTxt, selectedSession);

            int overlapLeave = int.Parse(con.ExecuteSQLQueryWithOneReturn(overlapSQL).ToString());

            if (overlapLeave > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Overlap date');", true);
                return;
            }

            //Check if selected leave type required upload file
            string autoApprove = con.ExecuteSQLQueryWithOneReturn(
                        string.Format(@"SELECT Delete_Schedule FROM Leave_Profile WHERE Leave_Profile_ID = '{0}'", selectedType)).ToString();

            string status = "Submitted";

            if (autoApprove == "Y")
            {
                status = "Approved";
            }

            string SQLQuery = string.Format(@"
            DECLARE @applicationID int									
            INSERT INTO Leave_Application				
               (				
                   Creation_Time,			
                   Employee_Profile_ID,			
                   Status,
                    Leave_Profile_ID,
                    Session,
                    From_Date,
                    To_Date,
                    Remarks
               )				
            VALUES					
            (					
	            GETDATE(),				
	            '{0}',				
	            '{1}',				
	            '{2}',				
	            '{3}',				
	            '{4}',				
	            '{5}',
                '{6}'
            )					
            SET @applicationID = SCOPE_IDENTITY()
            SELECT @applicationID
            ", Session["EmpID"], status, ddlType.SelectedValue, ddlSession.SelectedItem, dateFromTxt, dateToTxt, txtRemark.Text);

            string applicationID = con.ExecuteSQLQueryWithOneReturn(SQLQuery).ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand collectionCmd = new SqlCommand("Insert_LeaveDates", conn))
                {
                    collectionCmd.CommandType = CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@applicationID", applicationID);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            if (fileUpload.HasFiles)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Leave_Application\File\";
                string targetDir = Path.Combine(baseDir, applicationID);
                string xmlFilePath = Path.Combine(targetDir, "user_" + applicationID + ".xml");

                // Ensure the target directory exists
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Create XML document if it doesn't exist
                if (!File.Exists(xmlFilePath))
                {
                    XDocument xmlDoc = new XDocument(
                        new XDeclaration("1.0", "utf-16", "yes"),
                        new XElement("NewDataSet")
                    );
                    xmlDoc.Save(xmlFilePath);
                }

                XDocument xmlDoc2 = XDocument.Load(xmlFilePath);
                int fileId = xmlDoc2.Descendants("UserFile").Count(); // Get the last file ID

                foreach (HttpPostedFile uploadedFile in fileUpload.PostedFiles)
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
                xmlDoc2.Save(xmlFilePath);
            }

            string targetUrl = "~/Modules/Menu.aspx";
           Response.Redirect(targetUrl);
        }
    }
}