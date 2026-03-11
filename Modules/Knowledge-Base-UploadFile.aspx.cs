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
    public partial class KnowledgeBase_UploadFile : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
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
            string KnowledgeBase_ID = "";
            object KnowledgeBase_IDObj = Session["KnowledgeBase_KnowledgeBaseId"];
            if (KnowledgeBase_IDObj != null)
            {
                KnowledgeBase_ID = KnowledgeBase_IDObj.ToString();
            }
            if (fileUploadPDF.HasFiles)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Knowledge_Base\File\";
                string targetDir = Path.Combine(baseDir, KnowledgeBase_ID.ToString());
                string xmlFilePath = Path.Combine(targetDir, "user_" + KnowledgeBase_ID.ToString() + ".xml");

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
            string targetUrl = "~/Modules/Knowledge-Base.aspx";
            Response.Redirect(targetUrl);
        }

    }
}