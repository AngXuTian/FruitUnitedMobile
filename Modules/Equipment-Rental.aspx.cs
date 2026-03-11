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
using ForSessionValue;
using System.Windows.Controls;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using Button = System.Web.UI.WebControls.Button;
using Label = System.Web.UI.WebControls.Label;
using System.Xml.Linq;

namespace FruitUnitedMobile.Modules
{
    public partial class Equipment_Rental : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTypeDropdownList();
            }
        }

        protected void TypeListDropDown_Change(object sender, EventArgs e)
        {
            BindEquipmentDropdownList();
            BindChecklistData();
            string Equipment_Value = ddlEquipment.SelectedValue;
            if (Equipment_Value == "0")
            {
                btnSubmit.Text = "-------";
                btnSubmit.Enabled = false;
                btnSubmit.BackColor = Color.White;
            }
        }

        protected void EquipmentListDropDown_Change(object sender, EventArgs e)
        {
            BindMovementDropdownList();
            string Equipment_Value = ddlEquipment.SelectedValue;
            if (Equipment_Value == "0")
            {
                btnSubmit.Text = "-------";
                btnSubmit.Enabled = false;
                btnSubmit.BackColor = Color.White;
            }
            ShowProject();
            ShowExpiryDate();
        }

        protected void MovementListDropDown_Change(object sender, EventArgs e)
        {
            ShowProject();
            ShowExpiryDate();
        }

        private void BindMovementDropdownList()
        {
            string Equipment_Value = ddlEquipment.SelectedValue;
            int CheckIN = int.Parse(con.ExecuteSQLQueryWithOneReturn("SELECT Count(1) FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL").ToString());
            ddlMovement.Items.Clear();
            ddlProject.Items.Clear();
            ddlLocation.Items.Clear();
            if (CheckIN > 0)
            {
                btnSubmit.Text = "Check In";
                btnSubmit.Enabled = true;
                btnSubmit.BackColor = Color.Green;
                string query1 = "SELECT Type FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlMovement.DataSource = reader;
                        ddlMovement.DataTextField = "Type"; // Text displayed in dropdown
                        ddlMovement.DataValueField = "Type"; // Value for each item
                        ddlMovement.DataBind();
                        ddlMovement.Enabled = false;
                    }
                }

                string query2 = "SELECT Project_ID, Project_Name +' \\ '+ Project_No AS Project_Name FROM Project WHERE Project_ID IN (SELECT Project_ID FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL)";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query2, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlProject.DataSource = reader;
                        ddlProject.DataTextField = "Project_Name"; // Text displayed in dropdown
                        ddlProject.DataValueField = "Project_ID"; // Value for each item
                        ddlProject.DataBind();
                        ddlProject.Enabled = false;
                    }
                }

                ddlLocation.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                ddlLocation.Items.Insert(1, new ListItem("Office", "Office"));
                ddlLocation.Items.Insert(2, new ListItem("Dormitory", "Dormitory"));
                ddlLocation.Items.Insert(3, new ListItem("Transit", "Transit"));
            }
            else
            {
                btnSubmit.Text = "Check Out";
                btnSubmit.Enabled = true;
                btnSubmit.BackColor = Color.Red;
                ddlMovement.Enabled = true;
                ddlMovement.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                ddlMovement.Items.Insert(1, new ListItem("Project", "Project"));
                ddlMovement.Items.Insert(2, new ListItem("Maintenance", "Maintenance"));
                ddlMovement.Items.Insert(3, new ListItem("Calibration", "Calibration"));
                ddlMovement.Items.Insert(4, new ListItem("Others", "Others"));

                string query3 = "SELECT Project_ID, Project_Name +' \\ '+ Project_No AS Project_Name FROM Project WHERE Status IN ('Open','Ongoing')";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query3, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlProject.DataSource = reader;
                        ddlProject.DataTextField = "Project_Name"; // Text displayed in dropdown
                        ddlProject.DataValueField = "Project_ID"; // Value for each item
                        ddlProject.DataBind();
                        ddlProject.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                        ddlProject.Enabled = true;
                    }
                }


                string query4 = "SELECT Current_Status FROM Equipment_Profile where Equipment_Profile_ID ='" + ddlEquipment.SelectedValue + "'";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query4, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlLocation.DataSource = reader;
                        ddlLocation.DataTextField = "Current_Status"; // Text displayed in dropdown
                        ddlLocation.DataValueField = "Current_Status"; // Value for each item
                        ddlLocation.DataBind();
                        ddlLocation.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                    }
                }
            }
        }


        protected void ChecklistGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            e.Row.Cells[0].Style.Add("text-align", "center");
            e.Row.Cells[1].Style.Add("text-align", "left");
            e.Row.Cells[2].Style.Add("text-align", "center");
            e.Row.Cells[0].Style.Add("vertical-align", "middle");
            e.Row.Cells[1].Style.Add("vertical-align", "middle");
            e.Row.Cells[2].Style.Add("vertical-align", "middle");
        }

        private void BindEquipmentDropdownList()
        {
            string Type_Value = ddlType.SelectedValue;
            string query = @"
            SELECT Equipment_Profile_ID, Equipment_Name 
            FROM Equipment_Profile 
            WHERE Equipment_Type_ID = " + Type_Value + @" 
            AND Status = 'Active'
            EXCEPT
            SELECT Equipment_Profile_ID, Equipment_Name 
            FROM Equipment_Profile 
            WHERE Equipment_Profile_ID IN 
                (SELECT Equipment_Profile_ID 
                FROM Equipment_Movement 
                WHERE Employee_Profile_ID != "+ Session["EmpID"] + @" 
                AND Check_Out_Date is not null 
                AND Check_In_Date is null)
            ";
            ddlEquipment.Items.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlEquipment.DataSource = reader;
                    ddlEquipment.DataTextField = "Equipment_Name"; // Text displayed in dropdown
                    ddlEquipment.DataValueField = "Equipment_Profile_ID"; // Value for each item
                    ddlEquipment.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlEquipment.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        private void BindTypeDropdownList()
        {

            string query = "SELECT Equipment_Type_ID, Equipment_Type FROM Equipment_Type WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlType.DataSource = reader;
                    ddlType.DataTextField = "Equipment_Type"; // Text displayed in dropdown
                    ddlType.DataValueField = "Equipment_Type_ID"; // Value for each item
                    ddlType.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlType.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        protected void BindChecklistData()
        {
            string Type_Value = ddlType.SelectedValue;
            string sql = string.Format(@"SELECT Equipment_Checklist_ID, Checklist, S_N FROM Equipment_Checklist WHERE Equipment_Type_ID = {0} ORDER BY S_N ASC", Type_Value);

            DataTable dt = con.FillDatatable(sql);

            ChecklistGrid.DataSource = dt;
            ChecklistGrid.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string ButtonName = clickedButton.Text;

            if (!ValidateChecklist())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Error", @"alert('Please check atleast 1 from the checklist'); window.location=""./Menu.aspx"";", true);
                return;
            }

            if (string.IsNullOrEmpty(Latitude.Text) || string.IsNullOrEmpty(Longitude.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Error", @"alert('Please allow system to trace your location before check out the equipment.');", true);
                return;
            }

            if (ButtonName.IndexOf("Check Out") > -1)
            {
                string Equipment_Value = ddlEquipment.SelectedValue;
                string query = "SELECT Count(1) FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL";
                int CheckIN = int.Parse(con.ExecuteSQLQueryWithOneReturn(query).ToString());
                string ddlProjectValue = ddlProject.SelectedValue;
                if (ddlProjectValue == "0")
                {
                    ddlProjectValue = "NULL";
                }
                else
                {
                    ddlProjectValue = @"'" + ddlProjectValue + @"'";
                }
                if (CheckIN < 1)
                {
                    string SQLQuery = string.Format(@"
                        DECLARE @movementID int									
                        INSERT INTO Equipment_Movement					
                        (					
	                        Creation_Time,				
	                        Employee_Profile_ID,				
	                        Check_Out_Date,				
	                        Check_Out_Location,				
	                        Equipment_Profile_ID,				
	                        Type,				
	                        Project_ID,				
	                        Check_Out_Remarks,
                            Check_Out_Latitude,
                            Check_Out_Longitude
                        )					
                        VALUES					
                        (					
	                        GETDATE(),				
	                        '{0}',				
	                        GETDATE(),				
	                        '{1}',				
	                        '{2}',				
	                        '{3}',				
	                        {4},
	                        '{5}',
                            '{6}',
                            '{7}'
                        )					
                        SET @movementID = SCOPE_IDENTITY()
                        SELECT @movementID
                        ", Session["EmpID"], ddlLocation.SelectedValue, ddlEquipment.SelectedValue, ddlMovement.SelectedItem, ddlProjectValue, txtRemark.Text, Latitude.Text, Longitude.Text);

                    string movementID = con.ExecuteSQLQueryWithOneReturn(SQLQuery).ToString();
                    int SN = 0;
                    string SQLQuery1 = "";

                    foreach (GridViewRow row in ChecklistGrid.Rows)
                    {
                        SN++;
                        string Checked = "";
                        if (((CheckBox)row.FindControl("ChecklistCB")).Checked)
                        {
                            Checked = "Y";
                        }
                        else
                        {
                            Checked = "N";
                        }
                        string Checklist = (((Label)row.FindControl("ChecklistTxt")).Text);

                        SQLQuery1 += string.Format(@"
                        INSERT INTO Equipment_Movement_Checklist					
                        (					
	                        Creation_Time,				
	                        Equipment_Movement_ID,				
	                        Checklist,				
	                        Checked,				
	                        S_N,				
	                        In_Out				
                        )					
                        VALUES					
                        (					
	                        GETDATE(),				
	                        '{0}',				
	                        '{1}',				
	                        '{2}',				
	                        '{3}',				
	                        'Out'				
                        )					
                    ", movementID, Checklist, Checked, SN.ToString());
                    }

                    con.ExecuteSQLQuery(SQLQuery1);

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand collectionCmd = new SqlCommand("Update_EquipmentStatus", conn))
                        {
                            collectionCmd.CommandType = CommandType.StoredProcedure;

                            collectionCmd.Parameters.AddWithValue("@equipmentID", ddlEquipment.SelectedValue);

                            collectionCmd.ExecuteNonQuery();
                        }
                    }

                    if (ImageUpload1.HasFiles)
                    {
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\Out Image\";
                        string targetDir = Path.Combine(baseDir, movementID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + movementID + ".xml");

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

                        foreach (HttpPostedFile uploadedFile in ImageUpload1.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload1" + fileExtension;
                            string filePath = Path.Combine(targetDir, fileName);
                            uploadedFile.SaveAs(filePath);

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

                    if (ImageUpload2.HasFiles)
                    {
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\Out Image\";
                        string targetDir = Path.Combine(baseDir, movementID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + movementID + ".xml");

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

                        XDocument xmlDoc6 = XDocument.Load(xmlFilePath);
                        int fileId = xmlDoc6.Descendants("UserFile").Count(); // Get the last file ID

                        foreach (HttpPostedFile uploadedFile in ImageUpload2.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload2" + fileExtension;
                            string filePath = Path.Combine(targetDir, fileName);
                            uploadedFile.SaveAs(filePath);

                            // Get file details
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSize = fileInfo.Length; // File size in bytes
                            string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                            DateTime currentDate = DateTime.Now;

                            // Add file details to XML
                            xmlDoc6.Root.Add(new XElement("UserFile",
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
                        xmlDoc6.Save(xmlFilePath);
                    }

                    if (ImageUpload3.HasFiles)
                    {
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\Out Image\";
                        string targetDir = Path.Combine(baseDir, movementID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + movementID + ".xml");

                        // Ensure the target directory exists
                        if (!Directory.Exists(targetDir))
                        {
                            Directory.CreateDirectory(targetDir);
                        }

                        // Create XML document if it doesn't exist
                        if (!File.Exists(xmlFilePath))
                        {
                            XDocument xmlDoc7 = new XDocument(
                                new XDeclaration("1.0", "utf-16", "yes"),
                                new XElement("NewDataSet")
                            );
                            xmlDoc7.Save(xmlFilePath);
                        }

                        XDocument xmlDoc8 = XDocument.Load(xmlFilePath);
                        int fileId = xmlDoc8.Descendants("UserFile").Count(); // Get the last file ID

                        foreach (HttpPostedFile uploadedFile in ImageUpload3.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload3" + fileExtension;
                            string filePath = Path.Combine(targetDir, fileName);
                            uploadedFile.SaveAs(filePath);

                            // Get file details
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSize = fileInfo.Length; // File size in bytes
                            string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                            DateTime currentDate = DateTime.Now;

                            // Add file details to XML
                            xmlDoc8.Root.Add(new XElement("UserFile",
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
                        xmlDoc8.Save(xmlFilePath);
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", @"alert('Equipment check out.'); window.location=""./Menu.aspx"";", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", @"alert('Equipment already check out.'); window.location=""./Menu.aspx"";", true);
                }
            }
            else if (ButtonName.IndexOf("Check In") > -1)
            {
                string Equipment_Value = ddlEquipment.SelectedValue;
                string query = "SELECT Count(1) FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL";
                int CheckIN = int.Parse(con.ExecuteSQLQueryWithOneReturn(query).ToString());
                string ExpiryDateValue = txtExpiryDate.Text;
                if (string.IsNullOrEmpty(ExpiryDateValue))
                {
                    ExpiryDateValue = "NULL";
                }
                else
                {
                    ExpiryDateValue = @"'" + ExpiryDateValue + @"'";
                }

                if (CheckIN > 0)
                {
                    string Equipment_Movement_ID = con.ExecuteSQLQueryWithOneReturn(@"SELECT Equipment_Movement_ID FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + @" AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL").ToString();
                    string SQLQuery = string.Format(@"
                    DECLARE @movementID int									
                    UPDATE Equipment_Movement			
                    SET Check_In_Date = GETDATE(),			
	                    Check_In_Location = '{0}',		
	                    Check_In_Remarks = '{1}',		
	                    Expiry_Date = {2},
                        Check_In_Latitude = '{4}',
                        Check_In_Longitude = '{5}'
                    WHERE Equipment_Movement_ID = '{3}'			

                    ", ddlLocation.SelectedValue, txtRemark.Text, ExpiryDateValue, Equipment_Movement_ID, Latitude.Text, Longitude.Text);

                    con.ExecuteSQLQuery(SQLQuery);

                    int SN = 0;
                    string SQLQuery1 = "";

                    foreach (GridViewRow row in ChecklistGrid.Rows)
                    {
                        SN++;
                        string Checked = "";
                        if (((CheckBox)row.FindControl("ChecklistCB")).Checked)
                        {
                            Checked = "Y";
                        }
                        else
                        {
                            Checked = "N";
                        }
                        string Checklist = (((Label)row.FindControl("ChecklistTxt")).Text);

                        SQLQuery1 += string.Format(@"
                        INSERT INTO Equipment_Movement_Checklist					
                        (					
	                        Creation_Time,				
	                        Equipment_Movement_ID,				
	                        Checklist,				
	                        Checked,				
	                        S_N,				
	                        In_Out				
                        )					
                        VALUES					
                        (					
	                        GETDATE(),				
	                        '{0}',				
	                        '{1}',				
	                        '{2}',				
	                        '{3}',				
	                        'In'				
                        )					
                    ", Equipment_Movement_ID, Checklist, Checked, SN.ToString());
                    }

                    con.ExecuteSQLQuery(SQLQuery1);

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand collectionCmd = new SqlCommand("Update_EquipmentStatus", conn))
                        {
                            collectionCmd.CommandType = CommandType.StoredProcedure;
                            collectionCmd.Parameters.AddWithValue("@equipmentID", ddlEquipment.SelectedValue);
                            collectionCmd.ExecuteNonQuery();
                        }
                    }

                    if (fileUpload1.HasFiles)
                    {

                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\In File\";
                        string targetDir = Path.Combine(baseDir, Equipment_Movement_ID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + Equipment_Movement_ID + ".xml");

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

                        foreach (HttpPostedFile uploadedFile in fileUpload1.PostedFiles)
                        {
                            // Generate the target file path
                            string fileName = Path.GetFileName(uploadedFile.FileName);
                            string filePath = Path.Combine(targetDir, fileName);
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

                    if (ImageUpload1.HasFiles)
                    {
                        string baseDir1 = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\In Image\";
                        string targetDir1 = Path.Combine(baseDir1, Equipment_Movement_ID);
                        string xmlFilePath1 = Path.Combine(targetDir1, "user_" + Equipment_Movement_ID + ".xml");

                        // Ensure the target directory exists
                        if (!Directory.Exists(targetDir1))
                        {
                            Directory.CreateDirectory(targetDir1);
                        }

                        // Create XML document if it doesn't exist
                        if (!File.Exists(xmlFilePath1))
                        {
                            XDocument xmlDoc3 = new XDocument(
                                new XDeclaration("1.0", "utf-16", "yes"),
                                new XElement("NewDataSet")
                            );
                            xmlDoc3.Save(xmlFilePath1);
                        }

                        XDocument xmlDoc4 = XDocument.Load(xmlFilePath1);
                        int fileId1 = xmlDoc4.Descendants("UserFile").Count(); // Get the last file ID

                        foreach (HttpPostedFile uploadedFile in ImageUpload1.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload1" + fileExtension;
                            string filePath = Path.Combine(targetDir1, fileName);
                            uploadedFile.SaveAs(filePath);

                            // Get file details
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSize = fileInfo.Length; // File size in bytes
                            string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                            DateTime currentDate = DateTime.Now;

                            // Add file details to XML
                            xmlDoc4.Root.Add(new XElement("UserFile",
                                new XElement("File_ID", fileId1++),
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
                                new XElement("Path", targetDir1.Replace(@"\", "/"))
                            ));
                        }

                        // Save the updated XML document
                        xmlDoc4.Save(xmlFilePath1);
                    }

                    if (ImageUpload2.HasFiles)
                    {
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\In Image\";
                        string targetDir = Path.Combine(baseDir, Equipment_Movement_ID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + Equipment_Movement_ID + ".xml");

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

                        XDocument xmlDoc6 = XDocument.Load(xmlFilePath);
                        int fileId = xmlDoc6.Descendants("UserFile").Count(); // Get the last file ID

                        foreach (HttpPostedFile uploadedFile in ImageUpload2.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload2" + fileExtension;
                            string filePath = Path.Combine(targetDir, fileName);
                            uploadedFile.SaveAs(filePath);

                            // Get file details
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSize = fileInfo.Length; // File size in bytes
                            string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                            DateTime currentDate = DateTime.Now;

                            // Add file details to XML
                            xmlDoc6.Root.Add(new XElement("UserFile",
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
                        xmlDoc6.Save(xmlFilePath);
                    }

                    if (ImageUpload3.HasFiles)
                    {
                        string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Movement\In Image\";
                        string targetDir = Path.Combine(baseDir, Equipment_Movement_ID);
                        string xmlFilePath = Path.Combine(targetDir, "user_" + Equipment_Movement_ID + ".xml");

                        // Ensure the target directory exists
                        if (!Directory.Exists(targetDir))
                        {
                            Directory.CreateDirectory(targetDir);
                        }

                        // Create XML document if it doesn't exist
                        if (!File.Exists(xmlFilePath))
                        {
                            XDocument xmlDoc7 = new XDocument(
                                new XDeclaration("1.0", "utf-16", "yes"),
                                new XElement("NewDataSet")
                            );
                            xmlDoc7.Save(xmlFilePath);
                        }

                        XDocument xmlDoc8 = XDocument.Load(xmlFilePath);
                        int fileId = xmlDoc8.Descendants("UserFile").Count(); // Get the last file ID

                        foreach (HttpPostedFile uploadedFile in ImageUpload3.PostedFiles)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string fileName = "ImageUpload3" + fileExtension;
                            string filePath = Path.Combine(targetDir, fileName);
                            uploadedFile.SaveAs(filePath);

                            // Get file details
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSize = fileInfo.Length; // File size in bytes
                            string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                            DateTime currentDate = DateTime.Now;

                            // Add file details to XML
                            xmlDoc8.Root.Add(new XElement("UserFile",
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
                        xmlDoc8.Save(xmlFilePath);
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", @"alert('Equipment check in.'); window.location=""./Menu.aspx"";", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", @"alert('Equipment already check in.'); window.location=""./Menu.aspx"";", true);
                }
            }
            else
            {
                //ErrorMessage.Text = "----------";
            }
        }

        private void ShowExpiryDate()
        {
            string Equipment_Value = ddlEquipment.SelectedValue;
            string Movement_Value = ddlMovement.SelectedValue;
            int CheckIN = int.Parse(con.ExecuteSQLQueryWithOneReturn("SELECT Count(1) FROM Equipment_Movement WHERE Equipment_Profile_ID = " + Equipment_Value + " AND Check_Out_Date IS NOT NULL AND Check_In_Date IS NULL").ToString());

            if (CheckIN > 0 && (Movement_Value.IndexOf("Maintenance") > -1 || Movement_Value.IndexOf("Calibration") > -1))
            {
                divExpiryDate.Style.Remove("display");
            }
            else
            {
                divExpiryDate.Style["display"] = "none";
            }

            if (CheckIN > 0)
            {
                divFileUpload1.Style.Remove("display");
            }
            else
            {
                divFileUpload1.Style["display"] = "none";
            }
        }

        private void ShowProject()
        {
            string Movement_Value = ddlMovement.SelectedValue;
            if (Movement_Value.IndexOf("Project") > -1)
            {
                divProject.Style.Remove("display");
            }
            else
            {
                divProject.Style["display"] = "none";
            }
        }

        protected bool ValidateChecklist()
        {
            bool Validate = false;
            foreach (GridViewRow row in ChecklistGrid.Rows)
            {
                if (((CheckBox)row.FindControl("ChecklistCB")).Checked)
                {
                    Validate = true;
                }
            }
            return Validate;
        }

        private Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private void SaveCompressedImage(Bitmap image, string savePath, int quality)
        {
            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            image.Save(savePath, jpgEncoder, encoderParams);
        }
    }
}