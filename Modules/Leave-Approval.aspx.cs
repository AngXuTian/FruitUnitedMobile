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
    public partial class Leave_Approval : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
            if (Request.Browser.IsMobileDevice)
            {
            }
        }

        protected void BindData()
        {
            string sql = $@"
                SELECT
                    app.Leave_Application_ID,
	                employee.Display_Name,
	                'Date : ' + REPLACE(CONVERT(nvarchar, app.From_Date, 106), ' ', '-') + ' to ' + REPLACE(CONVERT(nvarchar, ISNULL(app.To_Date, app.From_Date), 106), ' ', '-') AS Leave_Date,
	                CASE WHEN (app.Session = 'Full Day') THEN '' ELSE '   (' + app.Session + ')' END AS Leave_Session,
	                'Leave Type : ' + leave.Leave_Type AS Leave_Type,
	                ISNULL('Remarks : ' + CONVERT(nvarchar(max), app.Remarks), '') AS Remarks,
                    'Status : ' + app.Status AS Status
                FROM Leave_Application app
                INNER JOIN Employee_Profile employee ON employee.Employee_Profile_ID = app.Employee_Profile_ID
                INNER JOIN Leave_Profile leave ON leave.Leave_Profile_ID = app.Leave_Profile_ID
                WHERE app.Status = 'Submitted'
                ORDER BY app.From_Date DESC";


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataGrid.DataSource = reader;
                    DataGrid.DataBind();
                }
            }
        }

        protected void MyGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve")
            {
                // Get the CommandArgument (e.g., ID of the row)
                string id = e.CommandArgument.ToString();
                GridViewRow row = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;


                // Call your backend function or logic here
                if (row != null)
                {
                    Label lblLeaveApplicationID = (Label)row.FindControl("Leave_Application_ID");
                    string leaveApplicationID = lblLeaveApplicationID?.Text;

                    approvalAction(id, leaveApplicationID);

                    // Optionally, update the UI or rebind the GridView
                    BindData();
                }
            }
        }

        private void approvalAction(string id, string leaveApplicationID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string query = "UPDATE Leave_Application SET Status = 'Approved', Approval_Date = GETDATE(), Employee_Profile_ID1 = @mployeeID " +
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
    }
}