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
    public partial class Completion_Task : System.Web.UI.Page
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
                #region Project Info
                
                txtProject.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
			SELECT ISNULL(Project.Project_Name,'N/A') AS ProjectName FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}										
 													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtFrom.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	CONCAT('', ISNULL(Project_Scope.From_Building,'N/A') + ' -> ' + ISNULL(Project_Scope.From_Floor,'N/A') + ' -> ' + ISNULL(Project_Scope.From_Location,'N/A')) AS From_Location						
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtTo.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	CASE WHEN (Project_Scope.To_Building IS NOT NULL OR Project_Scope.To_Floor IS NOT NULL OR Project_Scope.To_Location IS NOT NULL)
                    THEN CONCAT('', ISNULL(Project_Scope.To_Building,'N/A') + ' -> ' + ISNULL(Project_Scope.To_Floor,'N/A') + ' -> ' + ISNULL(Project_Scope.To_Location,'N/A'))
                    ELSE '' END AS To_Location						
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtScope.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	Scope_of_Work.Scope_of_Work						
FROM Project_Scope																
INNER JOIN Scope_of_Work ON Scope_of_Work.Scope_of_Work_ID = Project_Scope.Scope_of_Work_ID																
LEFT JOIN System_Profile ON System_Profile.System_Profile_ID = Project_Scope.System_Profile_ID																
LEFT JOIN Scope_Type ON Scope_Type.Scope_Type_ID = Project_Scope.Scope_Type_ID																
LEFT JOIN Ladder_Profile ON Ladder_Profile.Ladder_Profile_ID = Project_Scope.Ladder_Profile_ID																
WHERE Project_Scope.Project_Scope_ID = {0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";

                txtScopeInfo.Text = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT														
	Project_Scope.Scope_Info			
FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}													
 ", Request.QueryString["Project_Scope_ID"])
                )?.ToString() ?? "";
                #endregion

//                txtTaskInfo.Text = con.ExecuteSQLQueryWithOneReturn(
//                   string.Format(@"														
//	SELECT							
//	task.Task_Info					
//FROM Project_Scope_Task task								
//INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
//INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
//LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
//WHERE task.Project_Scope_Task_ID = {0}								
//ORDER BY task.Project_Scope_Task_ID ASC								
									
 													
// ", Request.QueryString["Project_Scope_Task_ID"])
//                )?.ToString() ?? "";
                BindDOData();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string soNo = SONoTextBox.Text.Trim();

            BindDOData(soNo);
        }


        protected void BindDOData(string soNo = null)
        {
            string filterCondition = "";

            if (!string.IsNullOrEmpty(soNo))
            {
                filterCondition += " AND Task_Profile.Task LIKE '%' + @SONo + '%'";
            }
            string sql = $@"
SELECT
    task.S_N,
    task.Task_Info,
	task.Project_Scope_Task_ID,							
	task.Project_Scope_ID,							
	Task_Profile.Task,							
	Project_Scope.Qty AS Scope_Qty,							
	task.Scope_Percent,							
	task.Completed_Qty,							
	task.Completed_Percent,							
	task.Actual_Percent,							
	Tool_Profile.Tool,							
	task.Remark,							
	Task_Profile.Score_Point AS Ops_Point,							
	Task_Profile.Average_Score_Point AS Average_Point,							
	Task_Profile.Highest_Score_Point AS Highest_Point							
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_ID = @Project_Scope_ID	
{filterCondition}
ORDER BY ISNULL(task.S_N,1) ASC, task.Project_Scope_Task_ID ASC		



";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(soNo))
            {
                parameters.Add(new SqlParameter("@SONo", soNo));
            }

            parameters.Add(new SqlParameter("@Project_Scope_ID", Request.QueryString["Project_Scope_ID"]));


            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }



        protected void InvoiceGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //SessionValue sessionValue = new SessionValue();
            //InvoiceGrid.Columns[2].Visible = false;
            e.Row.Cells[0].Style.Add("text-align", "left");
            //e.Row.Cells[1].Style.Add("text-align", "center");
            //e.Row.Cells[2].Style.Add("text-align", "center");


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var linkButton = (LinkButton)e.Row.FindControl("ViewInvoice");
                if (linkButton != null)
                {
                    var projectScopeTaskId = DataBinder.Eval(e.Row.DataItem, "Project_Scope_Task_ID");
                    linkButton.PostBackUrl = $"~/Modules/Completion-TaskCompletion.aspx?Project_Scope_Task_ID={projectScopeTaskId}&Project_Scope_ID="+ Request.QueryString["Project_Scope_ID"] + "&Project_ID=" + Request.QueryString["Project_ID"];
                }
                var projectScopeColumn = e.Row.FindControl("ProjectScopeColumn") as HtmlTableCell;
                if (projectScopeColumn != null)
                {
                    projectScopeColumn.Visible = false; // Completely hides the column
                }
            }
        }

        protected void BindDOData()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql = string.Format(@"
                        	SELECT
    task.S_N,
    task.Task_Info,
	task.Project_Scope_Task_ID,							
	task.Project_Scope_ID,							
	Task_Profile.Task,							
	Project_Scope.Qty AS Scope_Qty,							
	task.Scope_Percent,							
	task.Completed_Qty,							
	task.Completed_Percent,							
	task.Actual_Percent,							
	Tool_Profile.Tool,							
	task.Remark,							
	Task_Profile.Score_Point AS Ops_Point,							
	Task_Profile.Average_Score_Point AS Average_Point,							
	Task_Profile.Highest_Score_Point AS Highest_Point							
FROM Project_Scope_Task task								
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID								
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID								
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID								
WHERE task.Project_Scope_ID = @Project_Scope_ID								
ORDER BY ISNULL(task.S_N,1) ASC, task.Project_Scope_Task_ID ASC							

                       ");

            parameters.Add(new SqlParameter("@Project_Scope_ID", Request.QueryString["Project_Scope_ID"]));
            DataTable dt = con.FillDatatable(sql,parameters.ToArray());

            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
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

    }
}