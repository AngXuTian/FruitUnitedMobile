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
    public partial class Equipment_ViewFile : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        string Equipment_Profile_ID = "", ViewType = "";
        string FilePath = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Equipment_Profile\";
        protected void Page_Load(object sender, EventArgs e)
        {
            Equipment_Profile_ID = Request.QueryString["ID"].ToString();
            ViewType = Request.QueryString["Type"].ToString();
            BindFileGrid();
            iFrameDiv.Visible = false;
            ImageDiv.Visible = false;
        }


        protected void BindFileGrid()
        {
            DataSet FileNameDS = new DataSet();
            string EquipmentFilePath = FilePath + "\\" + ViewType + "\\" + Equipment_Profile_ID;
            string EquipmentXMLFilePath = FilePath + "\\" + ViewType + "\\" + Equipment_Profile_ID + "\\user_" + Equipment_Profile_ID + ".xml";
            if (!Directory.Exists(EquipmentFilePath))
            {
                Directory.CreateDirectory(EquipmentFilePath);
            }
            if (File.Exists(EquipmentXMLFilePath))
            {
                FileNameDS.ReadXml(EquipmentXMLFilePath);
                ViewFileGrid.DataSource = FileNameDS;
                ViewFileGrid.DataBind();
            }
        }

        protected void EquipmentFileGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string NavigateURL = ResolveUrl(("~/Modules/Equipment-ViewFile.aspx?Type=" + ViewType + "&ID=" + Equipment_Profile_ID + "&FileName=" + ((Label)e.Row.Cells[0].FindControl("Name")).Text));
                e.Row.Cells[0].Attributes.Add("onClick", string.Format("javascript:window.location=\'{0}\';", NavigateURL));
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Modules/Equipment-List.aspx");
        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            Label FileName = (Label)row.FindControl("Name");
            Response.ContentType = "Application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName.Text);
            Response.TransmitFile(Server.MapPath("~/Document/Equipment_Profile/" + ViewType + "/" + Equipment_Profile_ID + "/" + FileName.Text));
            Response.End();
        }


    }
}