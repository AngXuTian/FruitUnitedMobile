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
    public partial class Completion_Project_ViewFile : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        string Project_ID = "";
        string FilePath = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Project\Test Report\";
        protected void Page_Load(object sender, EventArgs e)
        {
            Project_ID = Request.QueryString["Project_ID"].ToString();
            if (!IsPostBack)
            {
                BindFileGrid();
            }
        }


        protected void BindFileGrid()
        {
            DataSet FileNameDS = new DataSet();
            string ProjectFilePath = FilePath + "\\" + Project_ID;
            string ProjectXMLFilePath = FilePath + "\\" + Project_ID + "\\user_" + Project_ID + ".xml";
            if (!Directory.Exists(ProjectFilePath))
            {
                Directory.CreateDirectory(ProjectFilePath);
            }
            if (File.Exists(ProjectXMLFilePath))
            {
                FileNameDS.ReadXml(ProjectXMLFilePath);
                ViewFileGrid.DataSource = FileNameDS;
                ViewFileGrid.DataBind();
            }
        }

        protected void PorjectFileGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Modules/Completion-Project.aspx");
        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            Label FileName = (Label)row.FindControl("Name");
            Response.ContentType = "Application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName.Text);
            Response.TransmitFile(Server.MapPath("~/Document/Project/Test Report/" + Project_ID + "/" + FileName.Text));
            Response.End();
        }


    }
}