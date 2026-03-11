using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.Configuration;
using System.Web.UI.HtmlControls;
using ForSessionValue;
using System.Web.Security;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json;

namespace FruitUnitedMobile.Modules
{
    public partial class Transport_History : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        SessionValue sessionValue = new SessionValue();
        protected void Page_Load(object sender, EventArgs e)
        {
            //string date = DateSearch.Text.Trim();

            if (!IsPostBack)
            {

                object FromDateObj = Session["TransactionHistory_FromDate"];
                if (FromDateObj != null)
                {
                    DateFromTextBox.Text = FromDateObj.ToString();
                }

                object ToDateObj = Session["TransactionHistory_ToDate"];
                if (ToDateObj != null)
                {
                    DateToTextBox.Text = ToDateObj.ToString();
                }

                object VehicleNoObj = Session["TransactionHistory_VehicleNo"];
                if (VehicleNoObj != null)
                {
                    VehicleTextBox.Text = VehicleNoObj.ToString();
                }

                object ProjectObj = Session["TransactionHistory_Project"];
                if (ProjectObj != null)
                {
                    ProjectTextBox.Text = ProjectObj.ToString();
                }

                BindDriverGrid();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            Session["TransactionHistory_FromDate"] = DateFromTextBox.Text.Trim();
            Session["TransactionHistory_ToDate"] = DateToTextBox.Text.Trim();
            Session["TransactionHistory_VehicleNo"] = VehicleTextBox.Text.Trim();
            Session["TransactionHistory_Project"] = ProjectTextBox.Text.Trim();
            BindDriverGrid();
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string json = "";

            if (!string.IsNullOrEmpty(json))
            {
                var driverAssignments = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                foreach (var pair in driverAssignments)
                {
                    string requestId = pair.Key;
                    string driverId = pair.Value;

                    string sql = @"
                UPDATE Transport_Request
                SET 
                    Employee_Profile_ID1 = '"+ driverId + @"',
                    Edit_Time = GETDATE(),
                    Vehicle_Profile_ID = (SELECT Vehicle_Profile_ID FROM Vehicle_Profile WHERE Status = 'Active' AND Employee_Profile_ID = '" + driverId + @"'),
                    Status = CASE WHEN ('" + driverId + @"' <> '') THEN 'Taken' ELSE Status END
                WHERE Transport_Request_ID = '" + requestId + @"'";

                    

                    con.ExecuteSQLQuery(sql);
                }
            }
        }

        protected void BindDriverGrid()
        {
            string fromDate = DateFromTextBox.Text.Trim();
            string toDate = DateToTextBox.Text.Trim();
            string vehicleNo = VehicleTextBox.Text.Trim();
            string projectName = ProjectTextBox.Text.Trim();

            string filterCondition = "";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                filterCondition += " AND CONVERT(date, req.Request_Date) BETWEEN @FromDate AND @ToDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.Date) { Value = DateTime.Parse(fromDate) });
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.Date) { Value = DateTime.Parse(toDate) });
            }

            if (!string.IsNullOrEmpty(vehicleNo) && vehicleNo != "0")
            {
                filterCondition += " AND vehicle.Vehicle_No like '%"+ vehicleNo + "%'";
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                filterCondition += @"
                AND EXISTS (
                    SELECT 1 FROM Transport_Request t
                    LEFT JOIN  Project p on t.Project_ID= p.Project_ID
                    WHERE p.Project_Name like '%" + projectName + @"%'
                    AND t.Vehicle_Profile_ID = req.Vehicle_Profile_ID
                    AND CONVERT(date, t.Request_Date) = CONVERT(date, req.Request_Date)
                    AND t.Go_Back = req.Go_Back
                )";
            }

            string sql = @"
    SELECT				
        REPLACE(CONVERT(varchar(11), req.Request_Date, 106), ' ', '-') AS Request_Date,
        SUBSTRING(CONVERT(varchar(20), req.Request_Time, 8), 1, 5) AS Request_Time,				
        req.Go_Back,			
        vehicle.Vehicle_No,			
        STUFF((
            SELECT DISTINCT ', ' + Project.Project_Initial 
            FROM Project 
            INNER JOIN Transport_Request history 
                ON history.Project_ID = Project.Project_ID 
            WHERE history.Status = 'Taken' 
            AND history.Vehicle_Profile_ID = req.Vehicle_Profile_ID
            AND CONVERT(nvarchar, history.Request_Date, 106) = CONVERT(nvarchar, req.Request_Date, 106) 
            AND history.Go_Back = req.Go_Back
            FOR XML PATH('')
        ),1,2,'') AS Project			
    FROM Transport_Request req				
    INNER JOIN Vehicle_Profile vehicle ON vehicle.Vehicle_Profile_ID = req.Vehicle_Profile_ID				
    WHERE req.Is_Driver = 'Y' AND req.Status <> 'Cancelled'
    " + filterCondition + @"
    GROUP BY 
    req.Request_Date, 
    req.Request_Time, 
    req.Go_Back, 
    vehicle.Vehicle_No, 
    req.Vehicle_Profile_ID
    ORDER BY req.Request_Date DESC, req.Request_Time ASC";


            try
            {
                DataTable dt = con.FillDatatable(sql, parameters.ToArray());
                DriverGrid.DataSource = dt;
                DriverGrid.DataBind();
            }
            catch
            {
                throw new Exception(sql);
            }
        }
    
        private DataTable GetDriverList()
        {
            string query = @"
                            DECLARE @employeeID as INT = '" + Session["EmpID"] + @"'

                            SELECT Employee_Profile_ID, Schedule_Display_Name
                            FROM Employee_Profile
                            WHERE Status = 'Active'
                            AND (
                                (EXISTS(SELECT 1 FROM Vehicle_Profile WHERE Employee_Profile_ID = @employeeID)
                                 AND Employee_Profile_ID = @employeeID)
                                OR
                                (NOT EXISTS(SELECT 1 FROM Vehicle_Profile WHERE Employee_Profile_ID = @employeeID)
                                 AND Employee_Profile_ID IN (SELECT Employee_Profile_ID FROM Vehicle_Profile WHERE Status = 'Active'))
                            )";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        protected void DriverGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string goBack = DataBinder.Eval(e.Row.DataItem, "Go_Back")?.ToString();

                // Set background color of the Request_Time cell
                if (goBack == "Go")
                {
                    int timeCellIndex = 1; 
                    e.Row.Cells[timeCellIndex].BackColor = System.Drawing.Color.LawnGreen;
                }
                else
                {
                    int timeCellIndex = 1; 

                    e.Row.Cells[timeCellIndex].BackColor = System.Drawing.Color.DarkOrange;
                }
            }
        }

    }
}