using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile.Modules
{
    public partial class UploadedImages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                alert.Visible = false;
                // Get the Project_Task_Completion_ID from the query string
                int projectTaskCompletionID;
                if (int.TryParse(Request.QueryString["Project_Task_Completion_ID"], out projectTaskCompletionID))
                {
                    // Call BindImages method to bind images for the specific Project_Task_Completion_ID
                    BindImages(projectTaskCompletionID);
                }
                else
                {
                    // Handle missing or invalid Project_Task_Completion_ID
                    Response.Write("Invalid Project_Task_Completion_ID.");
                }
            }
        }



        //private void BindImages(int projectTaskCompletionID)
        //{
        //    // Virtual directory base URL
        //    string virtualDir = "/ProjectImages/";

        //    //C:\Development\UCA\Application\Comnet\Document\Project_Task_Completion\Image
        //    // Directory path for this project ID (this is on the file system, not the URL)
        //    string projectDir = Path.Combine(ConfigurationManager.AppSettings["ImagesPath"], projectTaskCompletionID.ToString());

        //    if (Directory.Exists(projectDir))
        //    {
        //        // Get all image files
        //        string[] files = Directory.GetFiles(projectDir);

        //        // Map to the virtual directory URL
        //        string[] imageUrls = files.Select(file => "../ProjectImages/" + projectTaskCompletionID + "/" + Path.GetFileName(file).Replace("\\", "/")).ToArray();

        //        // Bind to the repeater
        //        imageRepeater.DataSource = imageUrls;
        //        imageRepeater.DataBind();
        //    }
        //    else
        //    {
        //        alert.Visible = true;
        //        //Response.Write("No images found for this project.");
        //    }
        //}

        private void BindImages(int projectTaskCompletionID)
        {

            // Directory path for this project ID (this is on the file system, not the URL)
            string baseDir = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Project_Task_Completion\Image\";
            string projectDir = Path.Combine(baseDir, projectTaskCompletionID.ToString());

            if (Directory.Exists(projectDir))
            {
                // Allowed image extensions
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                // Get all image files with allowed extensions
                string[] files = Directory.GetFiles(projectDir)
                    .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToArray();

                if (files.Length > 0)
                {
                    // Map to the virtual directory URL
                    string[] imageUrls = files.Select(file => "../ProjectImages/" + projectTaskCompletionID + "/" + Path.GetFileName(file).Replace("\\", "/")).ToArray();

                    // Bind to the repeater
                    imageRepeater.DataSource = imageUrls;
                    imageRepeater.DataBind();
                }
                else
                {
                    // No images found
                    alert.Visible = true;
                }
            }
            else
            {
                alert.Visible = true;
                //Response.Write("No images found for this project.");
            }
        }

        protected void btnToProject_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            //lb.PostBackUrl = "~/Modules/Completion-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;

            string targetUrl = "~/Modules/Verification-Project.aspx";
            Response.Redirect(targetUrl);
        }

        protected void btnToScope_Click(object sender, EventArgs e)
        {

            // Example of passing query parameters
            string targetUrl = "~/Modules/Verification-Scope.aspx?Project_ID=" + Request.QueryString["Project_ID"];
            Response.Redirect(targetUrl);
        }

        protected void btnToTask_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            string targetUrl = "~/Modules/Verification-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"];
            Response.Redirect(targetUrl);
        }




    }
}