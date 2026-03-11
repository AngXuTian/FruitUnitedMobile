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

namespace FruitUnitedMobile.Modules
{
    public partial class GR_Project : System.Web.UI.Page
    {
        Connection con = new Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["GR_Project_ID"] = string.Empty;
                BindProjectData("","","");
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string soNo = SONoTextBox.Text.Trim();
            string projectName = ProjectNameTextBox.Text.Trim();
            string customerName = CustomerNameTextBox.Text.Trim();

            BindProjectData(soNo, projectName, customerName);
        }


        protected void BindProjectData(string soNo = null, string projectName = null, string customerName = null)
        {
            string filterCondition = "";

            if (!string.IsNullOrEmpty(soNo))
            {
                filterCondition += " AND Project.Project_No LIKE '%' + @SONo + '%'";
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                filterCondition += " AND Project.Project_Name LIKE '%' + @ProjectName + '%'";
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                filterCondition += " AND Customer_Profile.Customer_Name LIKE '%' + @CustomerName + '%'";
            }

            string sql = $@"
            SELECT										
	            Project.Project_ID,									
	            Project.Project_No,									
	            Project.Project_Name,									
	            Customer_Profile.Customer_Name,									
	            Project.Estimated_Start_Date									
            FROM Project										
            INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID										
            INNER JOIN Customer_Profile ON Customer_Profile.Customer_Profile_ID = Quotation.Customer_Profile_ID										
            WHERE Project.Status IN ('Open','Ongoing')										
            {filterCondition}
            ORDER BY Project.Estimated_Start_Date ASC";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(soNo))
            {
                parameters.Add(new SqlParameter("@SONo", soNo));
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                parameters.Add(new SqlParameter("@ProjectName", projectName));
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                parameters.Add(new SqlParameter("@CustomerName", customerName));
            }

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            ProjectGrid.DataSource = dt;
            ProjectGrid.DataBind();
        }

        protected void ProjectGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ProjectGrid.Columns[2].Visible = false;
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "center");
        }

        protected void BindProjectData()
        {
            string sql = string.Format(@"
                        	SELECT									
		Project.Project_ID,								
		Project.Project_No,								
		CONCAT('Project: ', Project.Project_Name) AS Project_Name,	
        CONCAT('Customer: ', Customer_Profile.Customer_Name) AS Customer_Name,
		Project.Estimated_Start_Date								
	FROM Project									
	INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID									
	INNER JOIN Customer_Profile ON Customer_Profile.Customer_Profile_ID = Quotation.Customer_Profile_ID									
	INNER JOIN									
	(									
		SELECT								
			Project_ID,							
			COUNT(1) AS Scope							
		FROM Project_Scope								
		WHERE Mobile_Visibility = 'Y'								
		GROUP BY Project_ID								
	) scopeTable ON scopeTable.Project_ID = Project.Project_ID									
	WHERE Project.Status IN ('Open','Ongoing')									
	ORDER BY Project.Estimated_Start_Date ASC									

                       ");

            DataTable dt = con.FillDatatable(sql);

            ProjectGrid.DataSource = dt;
            ProjectGrid.DataBind();
        }


        protected void ProjectGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewLocation")
            {
                string Project_ID = e.CommandArgument.ToString();
                Session["GR_Project_ID"] = Project_ID;
                Response.Redirect("~/Modules/GR-Location.aspx");
            }
        }
    }
}