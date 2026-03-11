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
    public partial class KnowledgeBase_ViewFileList : System.Web.UI.Page
    {
        string KnowledgeBase_ID = "";
        string FilePath = ConfigurationManager.AppSettings["SystemPath"] + @"Document\Knowledge_Base\";
        protected void Page_Load(object sender, EventArgs e)
        {
            object KnowledgeBase_IDObj = Session["KnowledgeBase_KnowledgeBaseId"];
            if (KnowledgeBase_IDObj != null)
            {
                Connection con = new Connection();
                KnowledgeBase_ID = KnowledgeBase_IDObj.ToString();
                ProjectTitle.Text = con.ExecuteSQLQueryWithOneReturn("SELECT TITLE FROM Knowledge_Base WHERE Knowledge_Base_ID = " + KnowledgeBase_ID).ToString();
            }
            BindFileGrid();
        }


        protected void BindFileGrid()
        {
            DataSet FileNameDS = new DataSet();
            string EquipmentFilePath = FilePath + "File\\" + KnowledgeBase_ID;
            string EquipmentXMLFilePath = FilePath + "File\\" + KnowledgeBase_ID + "\\user_" + KnowledgeBase_ID + ".xml";
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
                string FileName = ((Label)e.Row.Cells[0].FindControl("Name")).Text;
                if (FileName.ToLower().IndexOf(".png") > -1 || FileName.ToLower().IndexOf(".jpg") > -1 || FileName.ToLower().IndexOf(".jpeg") > -1 || FileName.ToLower().IndexOf(".gif") > -1 || FileName.ToLower().IndexOf(".pdf") > -1)
                {
                    string NavigateURL = ResolveUrl(("~/Modules/Knowledge-Base-ViewFile.aspx?ID=" + KnowledgeBase_ID + "&FileName=" + FileName));
                    e.Row.Cells[0].Attributes.Add("onClick", string.Format("javascript:window.location=\'{0}\';", NavigateURL));
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Modules/Knowledge-Base.aspx");
        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            Label FileName = (Label)row.FindControl("Name");
            Response.ContentType = "Application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName.Text);
            Response.TransmitFile(Server.MapPath("~/Document/Knowledge_Base/File/" + KnowledgeBase_ID + "/" + FileName.Text));
            Response.End();
        }


    }
}