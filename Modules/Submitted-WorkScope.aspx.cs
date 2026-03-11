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
using Org.BouncyCastle.Ocsp;

namespace FruitUnitedMobile.Modules
{
    public partial class Submitted_WorkScope : System.Web.UI.Page
    {
        Connection con = new Connection();
        string DONo = string.Empty;
        string Customer = string.Empty;
        string VehicleNo = string.Empty;
        string Status = string.Empty;
        string DODate = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDOData();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string soNo = SONoTextBox.Text.Trim();
            string projectName = ProjectNameTextBox.Text.Trim();
            string customerName = CustomerNameTextBox.Text.Trim();

            BindDOData(soNo, projectName, customerName);
        }


        protected void BindDOData(string soNo = null, string projectName = null, string customerName = null)
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
	AND (SELECT COUNT(1) FROM Project_Task_Completion 									
	INNER JOIN Project_Scope_Task ON Project_Scope_Task.Project_Scope_Task_ID = Project_Task_Completion.Project_Scope_Task_ID					 	 			
	INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = Project_Scope_Task.Project_Scope_ID WHERE Project_Scope.Project_ID = Project.Project_ID AND Project_Task_Completion.Verified_On IS NULL AND Project_Task_Completion.Employee_Profile_ID = @emp) > 0									
							        
        {filterCondition}";

            List<SqlParameter> parameters = new List<SqlParameter>   
            {
                new SqlParameter("@emp",  Session["EmpID"])  
            };


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
            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }



        protected void InvoiceGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            InvoiceGrid.Columns[2].Visible = false;
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "center");
            e.Row.Cells[2].Style.Add("text-align", "center");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lb = (LinkButton)e.Row.Cells[1].FindControl("ViewInvoice");
                //HtmlAnchor anchorLink = (HtmlAnchor)e.Row.Cells[4].FindControl("iconLink");

                lb.PostBackUrl = "~/Modules/Submitted-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;
                //lb.PostBackUrl = "~/Modules/Verification-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;
            }
        }

        protected void BindDOData()
        {
            string sql = string.Format(@"
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
	AND (SELECT COUNT(1) FROM Project_Task_Completion 									
	INNER JOIN Project_Scope_Task ON Project_Scope_Task.Project_Scope_Task_ID = Project_Task_Completion.Project_Scope_Task_ID					 	 			
	INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = Project_Scope_Task.Project_Scope_ID WHERE Project_Scope.Project_ID = Project.Project_ID AND Project_Task_Completion.Verified_On IS NULL AND Project_Task_Completion.Employee_Profile_ID = @emp) > 0		

                       ");

            List<SqlParameter> parameters = new List<SqlParameter>
            {
        new SqlParameter("@emp", Session["EmpID"])  
    };

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());

            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }
    }
}