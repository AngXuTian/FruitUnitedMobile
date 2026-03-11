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
using System.Configuration;

namespace FruitUnitedMobile.Modules
{
    public partial class Verification_Task : System.Web.UI.Page
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
			SELECT CONCAT(ISNULL(Project.Project_Name, 'N/A'), ' / ', ISNULL(Project.Project_No, 'N/A')) AS ProjectName FROM Project_Scope INNER JOIN Project ON Project.Project_ID = Project_Scope.Project_ID WHERE Project_Scope_ID={0}										
 													
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
                BindDOData();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            //string soNo = SONoTextBox.Text.Trim();

            //BindDOData(soNo);
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
	completion.Project_Task_Completion_ID,										
	Task_Profile.Task,										
	completion.Qty AS Pending_Qty,										
	completedBy.Display_Name AS Completed_By,										
	FORMAT(completion.Completion_Date, 'dd-MMM-yyyy HH:mm:ss') AS Completion_Date,								
	completion.Remark,										
	Exceeded_Reason.Reason AS Exceeded_Reason,										
	Tool_Profile.Tool,										
	task.Completed_Qty,										
	Project_Scope.Qty AS Scope_Qty,										
	task.Actual_Percent,										
	task.Remark AS Task_Remark,										
	task.Project_Scope_Task_ID,										
	task.Project_Scope_ID,										
	Task_Profile.Score_Point AS Ops_Point,										
	Task_Profile.Average_Score_Point AS Average_Point,										
	Task_Profile.Highest_Score_Point AS Highest_Point										
FROM Project_Task_Completion completion											
INNER JOIN Employee_Profile completedBy ON completedBy.Employee_Profile_ID = completion.Employee_Profile_ID											
LEFT JOIN Exceeded_Reason ON Exceeded_Reason.Exceeded_Reason_ID = completion.Exceeded_Reason_ID											
INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID											
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID											
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID											
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID											
WHERE completion.Verified_On IS NULL											
		AND task.Project_Scope_ID = @Project_Scope_ID		
{filterCondition}
ORDER BY ISNULL(task.S_N,1) ASC, task.Project_Scope_Task_ID ASC, completion.Project_Task_Completion_ID ASC

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
            string Project_Task_Completion_ID = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
	            SELECT
    task.S_N,
	completion.Project_Task_Completion_ID									
FROM Project_Task_Completion completion											
INNER JOIN Employee_Profile completedBy ON completedBy.Employee_Profile_ID = completion.Employee_Profile_ID											
LEFT JOIN Exceeded_Reason ON Exceeded_Reason.Exceeded_Reason_ID = completion.Exceeded_Reason_ID											
INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID											
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID											
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID											
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID											
WHERE completion.Verified_On IS NULL											
		AND task.Project_Scope_ID = {0}
ORDER BY ISNULL(task.S_N,1) ASC, task.Project_Scope_Task_ID ASC, completion.Project_Task_Completion_ID ASC
									
 													
             ", Request.QueryString["Project_Scope_ID"])
                           )?.ToString();

            //SessionValue sessionValue = new SessionValue();
            InvoiceGrid.Columns[4].Visible = false;
            InvoiceGrid.Columns[5].Visible = false;

            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "center");
            e.Row.Cells[2].Style.Add("text-align", "center");
            e.Row.Cells[3].Style.Add("text-align", "center");



            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var linkButton = (LinkButton)e.Row.FindControl("viewInfo");
                if (linkButton != null)
                {
                    var projectScopeTaskId = DataBinder.Eval(e.Row.DataItem, "Project_Scope_Task_ID");
                    var projectTaskId = DataBinder.Eval(e.Row.DataItem, "Project_Task_Completion_ID");

                    linkButton.PostBackUrl = $"~/Modules/Verification-Completion.aspx?Project_Scope_Task_ID={projectScopeTaskId}&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"] + "&Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Task_Completion_ID=" + projectTaskId;
                }

                var linkButton2 = (LinkButton)e.Row.FindControl("viewImage");
                if (linkButton2 != null)
                {

                    var projectScopeTaskId = DataBinder.Eval(e.Row.DataItem, "Project_Scope_Task_ID");
                    var projectTaskId = DataBinder.Eval(e.Row.DataItem, "Project_Task_Completion_ID");

                    linkButton2.PostBackUrl = $"~/Modules/UploadedImages.aspx?Project_Scope_Task_ID={projectScopeTaskId}&Project_Scope_ID=" + Request.QueryString["Project_Scope_ID"] + "&Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Task_Completion_ID=" + projectTaskId ;
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
	completion.Project_Task_Completion_ID,										
	Task_Profile.Task,										
	completion.Qty AS Pending_Qty,										
	completedBy.Display_Name AS Completed_By,										
	FORMAT(completion.Completion_Date, 'dd-MMM-yyyy HH:mm:ss') AS Completion_Date,										
	completion.Remark,										
	Exceeded_Reason.Reason AS Exceeded_Reason,										
	Tool_Profile.Tool,										
	task.Completed_Qty,										
	Project_Scope.Qty AS Scope_Qty,										
	task.Actual_Percent,										
	task.Remark AS Task_Remark,										
	task.Project_Scope_Task_ID,										
	task.Project_Scope_ID,										
	Task_Profile.Score_Point AS Ops_Point,										
	Task_Profile.Average_Score_Point AS Average_Point,										
	Task_Profile.Highest_Score_Point AS Highest_Point										
FROM Project_Task_Completion completion											
INNER JOIN Employee_Profile completedBy ON completedBy.Employee_Profile_ID = completion.Employee_Profile_ID											
LEFT JOIN Exceeded_Reason ON Exceeded_Reason.Exceeded_Reason_ID = completion.Exceeded_Reason_ID											
INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID											
INNER JOIN Task_Profile ON Task_Profile.Task_Profile_ID = task.Task_Profile_ID											
INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID											
LEFT JOIN Tool_Profile ON Tool_Profile.Tool_Profile_ID = task.Tool_Profile_ID											
WHERE completion.Verified_On IS NULL											
		AND task.Project_Scope_ID = @Project_Scope_ID
ORDER BY ISNULL(task.S_N,1) ASC, task.Project_Scope_Task_ID ASC, completion.Project_Task_Completion_ID ASC
								

                       ");

            parameters.Add(new SqlParameter("@Project_Scope_ID", Request.QueryString["Project_Scope_ID"]));
            DataTable dt = con.FillDatatable(sql, parameters.ToArray());

            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }

        protected void MyGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Verify")
            {
                // Get the CommandArgument (e.g., ID of the row)
                string id = e.CommandArgument.ToString();
                GridViewRow row = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;


                // Call your backend function or logic here
                if (row != null)
                {
                    Label lblProjectTaskCompletionID = (Label)row.FindControl("Project_Task_Completion_ID");
                    string projectTaskId = lblProjectTaskCompletionID?.Text;

                    Label lblProjectScopeTaskID = (Label)row.FindControl("Project_Scope_Task_ID");
                    string projectScopeTskId = lblProjectScopeTaskID?.Text;

                    VerifyAction(id, projectTaskId, projectScopeTskId);

                    // Optionally, update the UI or rebind the GridView
                    BindDOData();
                }
            }
        }

        private void VerifyAction(string id, string projectTaskId, string projectScopeTskId)
        {
           


            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Project_Task_Completion SET Employee_Profile_ID1 = @empId, Verified_On = GETDATE() " +
                    "WHERE Project_Task_Completion_ID = @Project_Task_Completion_ID AND Verified_On IS NULL";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@empId", Session["EmpID"].ToString());
                    cmd.Parameters.AddWithValue("@Project_Task_Completion_ID", projectTaskId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectTaskCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectTaskID", projectScopeTskId);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectScopeCompletion", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectScopeID", Request.QueryString["Project_Scope_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlCommand collectionCmd = new System.Data.SqlClient.SqlCommand("Update_ProjectBillingInfo", conn))
                {
                    collectionCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    collectionCmd.Parameters.AddWithValue("@projectID", Request.QueryString["Project_ID"]);

                    collectionCmd.ExecuteNonQuery();
                }
            }

            //string script = "alert('Confirmed Successfully!');";
            //ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);


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

    }
}