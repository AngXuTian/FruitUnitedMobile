using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.Web.UI.HtmlControls;
using ForSessionValue;
using System.Web.Security;
using System.Data.SqlClient;
using System.Globalization;

namespace FruitUnitedMobile.Modules
{
    public partial class Knowledge_Base : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                object DepartmentObj = Session["KnowledgeBase_Department"];
                if (DepartmentObj != null)
                {
                    DepartmentTextBox.Text = DepartmentObj.ToString();
                }

                object CategoryObj = Session["KnowledgeBase_Category"];
                if (CategoryObj != null)
                {
                    CategoryTextBox.Text = CategoryObj.ToString();
                }

                object TitleObj = Session["KnowledgeBase_Title"];
                if (TitleObj != null)
                {
                    TitleTextBox.Text = TitleObj.ToString();
                }


                BindKnowledgeBaseData();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string Department = DepartmentTextBox.Text.Trim();
            string Category = CategoryTextBox.Text.Trim();
            string Title = TitleTextBox.Text.Trim();
            Session["KnowledgeBase_Department"] = DepartmentTextBox.Text.Trim();
            Session["KnowledgeBase_Category"] = CategoryTextBox.Text.Trim();
            Session["KnowledgeBase_Title"] = TitleTextBox.Text.Trim();
            BindKnowledgeBaseData();
        }


        protected void BindKnowledgeBaseData()
        {
            string Department = DepartmentTextBox.Text.Trim();
            string Category = CategoryTextBox.Text.Trim();
            string Title = TitleTextBox.Text.Trim();
            string filterCondition = "";
            string sql = string.Format(@"
                        SELECT Department_Profile.Department, KB_Category.Category, Knowledge_Base.Knowledge_Base_ID, Knowledge_Base.Title, Knowledge_Base.Remarks FROM Knowledge_Base 													
                        INNER JOIN Department_Profile ON Department_Profile.Department_Profile_ID = Knowledge_Base.Department_Profile_ID													
                        INNER JOIN KB_Category ON KB_Category.KB_Category_ID = Knowledge_Base.KB_Category_ID													
                        WHERE Knowledge_Base.Mobile_Visibility = 'Y' 										
									
                       ");

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(Department))
            {
                filterCondition += "  AND Department_Profile.Department LIKE '%" + Department + "%'";
            }

            if (!string.IsNullOrEmpty(Category))
            {
                filterCondition += "  AND KB_Category.Category LIKE '%" + Category + "%'";
            }

            if (!string.IsNullOrEmpty(Title))
            {
                filterCondition += "  AND Title LIKE '%" + Title + "%'";
            }

            if (!string.IsNullOrEmpty(filterCondition))
            {
                sql = sql + filterCondition;
            }

            sql = sql + "ORDER BY Department_Profile.Department ASC,KB_Category.Category ASC, Knowledge_Base.Title ASC";
            DataTable dt = con.FillDatatable(sql);
            KnowledgeBaseGrid.DataSource = dt;
            KnowledgeBaseGrid.DataBind();
        }



        protected void KnowledgeBaseGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "left");
            e.Row.Cells[2].Style.Add("text-align", "center");
            e.Row.Cells[0].Style.Add("vertical-align", "middle");
            e.Row.Cells[1].Style.Add("vertical-align", "middle");
            e.Row.Cells[2].Style.Add("vertical-align", "middle");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }


        protected void KnowledgeBaseGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewFile")
            {
                string KnowledgeBaseId = e.CommandArgument.ToString();
                Session["KnowledgeBase_KnowledgeBaseId"] = KnowledgeBaseId;
                Response.Redirect("Knowledge-Base-ViewFileList.aspx");
            }
            else if (e.CommandName == "UploadFile")
            {
                string KnowledgeBaseId = e.CommandArgument.ToString();
                Session["KnowledgeBase_KnowledgeBaseId"] = KnowledgeBaseId;
                Response.Redirect("Knowledge-Base-UploadFile.aspx");
            }
        }
    }
}