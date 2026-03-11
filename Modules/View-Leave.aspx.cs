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
using System.Xml.Linq;

namespace FruitUnitedMobile.Modules
{
    public partial class View_Leave : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Action"].ToString() == "Approval")
            {
                string accessible = con.ExecuteSQLQueryWithOneReturn(@"SELECT access.Available FROM Employee_Mobile_Access access INNER JOIN Mobile_Module ON Mobile_Module.Mobile_Module_ID = access.Mobile_Module_ID WHERE Mobile_Module.Module_Name = 'Leave Approval' AND access.Employee_Profile_ID = '"+ Session["EmpID"] + @"'").ToString();
            
                if (accessible == "N")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid access.');", true);
                    return;
                }
            }

            if (!IsPostBack)
            {
                BindData("","","");
            }
            if (Request.Browser.IsMobileDevice)
            {
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string employeeName = employeeTextBox.Text.Trim();
            string dateFrom = DateFromTextBox.Text.Trim();
            string dateTo = DateToTextBox.Text.Trim();

            BindData(employeeName, dateFrom, dateTo);
        }

        protected void BindData(string employeeName = null, string dateFrom = null, string dateTo = null)
        {
            string action = Request.QueryString["Action"].ToString();
            string filterCondition = "";

            //Display only submitted leave for approval
            if (action == "Approval")
            {
                filterCondition += " AND app.Status = 'Submitted'";
            }

            if (action == "Submitted")
            {
                filterCondition += " AND app.Employee_Profile_ID = @employeeID";
            }

            if (!string.IsNullOrEmpty(employeeName) || !string.IsNullOrEmpty(dateFrom))
            {
                if (!string.IsNullOrEmpty(employeeName))
                {
                    filterCondition += " AND (employee.Display_Name LIKE '%' + @employeeName + '%' OR employee.Employee_Name LIKE '%' + @employeeName + '%')";
                }

                if (!string.IsNullOrEmpty(dateFrom))
                {
                    if (string.IsNullOrEmpty(dateTo))
                    {
                        dateTo = dateFrom;
                    }

                    filterCondition += " AND CONVERT(date, app.Leave_Date) BETWEEN CONVERT(date, @dateFrom) AND CONVERT(date, @dateTo)";
                }
            }

            string sql = $@"
                SELECT
                    app.Leave_Application_ID,
                    employee.Display_Name,
                    'Date : ' + REPLACE(CONVERT(nvarchar, app.From_Date, 106), ' ', '-') + ' to ' + REPLACE(CONVERT(nvarchar, ISNULL(app.To_Date, app.From_Date), 106), ' ', '-') AS Leave_Date,
                    CASE WHEN (app.Session = 'Full Day') THEN '' ELSE '   (' + app.Session + ')' END AS Leave_Session,
                    'Leave Type : ' + leave.Leave_Type AS Leave_Type,
                    ISNULL('Remarks : ' + CONVERT(nvarchar(max), app.Remarks), '') AS Remarks,
                    app.Status AS Status
                FROM Leave_Application app
                INNER JOIN Employee_Profile employee ON employee.Employee_Profile_ID = app.Employee_Profile_ID
                INNER JOIN Leave_Profile leave ON leave.Leave_Profile_ID = app.Leave_Profile_ID
                WHERE app.Status IS NOT NULL
                        AND (DATEDIFF(Month, app.From_Date, GETDATE()) BETWEEN 0 AND 3
                                OR CONVERT(date, app.From_Date) >= CONVERT(date, GETDATE())
                                OR CONVERT(date, ISNULL(app.To_Date, app.From_Date)) >= CONVERT(date, GETDATE()))
                {filterCondition}
                ORDER BY app.From_Date DESC";

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@employeeID", Session["EmpID"]));

            if (!string.IsNullOrEmpty(employeeName))
            {
                parameters.Add(new SqlParameter("@employeeName", employeeName));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                parameters.Add(new SqlParameter("@dateFrom", dateFrom));
            }

            if (!string.IsNullOrEmpty(dateTo))
            {
                parameters.Add(new SqlParameter("@dateTo", dateTo));
            }

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            DataGrid.DataSource = dt;
            DataGrid.DataBind();
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string action = Request.QueryString["Action"].ToString();

            if (action != "Approval")
            {
                DataGrid.Columns[1].Visible = false;
            }

            if (action != "Submitted")
            {
                DataGrid.Columns[2].Visible = false;
            }

            DataGrid.Columns[3].Visible = false;
            DataGrid.Columns[4].Visible = false;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var status = (e.Row.FindControl("lblStatus") as Label).Text;
                if (status == "Cancelled")
                {
                    ((LinkButton)e.Row.FindControl("lnkCancel")).Visible = false;
                    ((LinkButton)e.Row.FindControl("lnkApprove")).Visible = false;
                    Label lblDate = (Label)e.Row.FindControl("lblDate");
                    lblDate.ForeColor = Color.Red;
                }

                if (status == "Submitted")
                {
                    Label lblDate = (Label)e.Row.FindControl("lblDate");
                    lblDate.ForeColor = Color.Green;
                }
            }
        }

        //Command for approval / cancellation
        protected void MyGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Get the CommandArgument (e.g., ID of the row)
            string id = e.CommandArgument.ToString();
            GridViewRow row = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;

            Label lblLeaveApplicationID = (Label)row.FindControl("Leave_Application_ID");
            string leaveApplicationID = lblLeaveApplicationID?.Text;

            // Call your backend function or logic here
            if (row != null)
            {
                if (e.CommandName == "Approve")
                {
                    approvalAction(id, leaveApplicationID);
                }

                if (e.CommandName == "CancelDD")
                {
                    cancelAction(id, leaveApplicationID);
                }

                // Optionally, update the UI or rebind the GridView
                BindData();
            }
        }

        private void approvalAction(string id, string leaveApplicationID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Leave_Application SET Status = 'Approved', Approval_Date = GETDATE(), Employee_Profile_ID1 = @employeeID " +
                    "WHERE Leave_Application_ID = @leaveApplicationID AND Status = 'Submitted'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@employeeID", Session["EmpID"]);
                    cmd.Parameters.AddWithValue("@leaveApplicationID", leaveApplicationID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void cancelAction(string id, string leaveApplicationID)
        {
            DateTime fromDate = DateTime.Parse(con.ExecuteSQLQueryWithOneReturn(@"SELECT From_Date FROM Leave_Application WHERE Leave_Application_ID = '"+ leaveApplicationID +@"'").ToString());
            
            if (fromDate < DateTime.Today)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Applicable for upcoming leave only.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Leave_Application SET Status = 'Cancelled', Cancelled_On = GETDATE(), Employee_Profile_ID2 = @employeeID " +
                    "WHERE Leave_Application_ID = @leaveApplicationID AND Status <> 'Cancelled'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@employeeID", Session["EmpID"]);
                    cmd.Parameters.AddWithValue("@leaveApplicationID", leaveApplicationID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}