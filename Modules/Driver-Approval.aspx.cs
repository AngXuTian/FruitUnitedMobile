using DBConnection;
using ForSessionValue;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FruitUnitedMobile.Modules
{
    public partial class Driver_Approval : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!IsPostBack)
            {
                object DateObj = Session["DriverApproval_Date"];
                if (DateObj != null)
                {
                    DateSearch.Text = DateObj.ToString();
                }
                string date = DateSearch.Text.Trim();
                BindDriverGrid(date);
                BindDriverGrid2(date);
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string date = DateSearch.Text.Trim();
            string format = "dd-MMM-yyyy";
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out DateTime parsedDate))
            {
                DateTime today = DateTime.Today;

                if (parsedDate < today)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Back Date is not allowed.');", true);
                }
                else
                {
                    Session["DriverApproval_Date"] = date;
                    BindDriverGrid(date);
                    BindDriverGrid2(date);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Date Format.');", true);
            }
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {

            foreach (GridViewRow row in DriverGrid.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList ddlDriver = (DropDownList)row.FindControl("ddlDriver");
                    Label Transport_Request_ID = (Label)row.FindControl("Transport_Request_ID");
                    UpdateRowInDatabase(Transport_Request_ID.Text, ddlDriver.SelectedItem.Value);
                }
            }

            string date = DateSearch.Text.Trim();
            BindDriverGrid2(date);
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Submit Complete.');", true);
        }

        protected void DDLDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlDriver = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlDriver.NamingContainer;
            string DriverID = ddlDriver.SelectedItem.Value;
            Label SeatUse = (Label)row.FindControl("LabelSeats");
            Label Transport_Request_ID = (Label)row.FindControl("Transport_Request_ID");
            Label Go_Back = (Label)row.FindControl("Go_Back");
            string Go_Back_Text = "";
            if (Go_Back.Text.ToLower().IndexOf("go") > -1)
            {
                Go_Back_Text = "Go_Seat";
            }
            else
            {
                Go_Back_Text = "Back_Seat";
            }
            string filterCondition = " AND Request_Date = CAST(GETDATE() AS DATE)";
            string date = DateSearch.Text.Trim();
            if (!string.IsNullOrEmpty(date))
            {
                filterCondition = " AND CAST(Request_Date AS DATE) = CAST('"+ date + "' AS DATE)";
            }
            if (DriverID == "0")
            {
                UpdateRowInDatabase(Transport_Request_ID.Text, ddlDriver.SelectedItem.Value);
                BindDriverGrid(date);
                BindDriverGrid2(date);
            }
            else
            {
                int DriverTotalSeatUse = int.Parse(con.ExecuteSQLQueryWithOneReturn(@"
                SELECT			
                Vehicle_Profile.No_of_Seat - ISNULL(seatTaken." + Go_Back_Text + @",0) - " + SeatUse.Text + @"					
                FROM Vehicle_Profile				
                INNER JOIN Employee_Profile ON Employee_Profile.Employee_Profile_ID = Vehicle_Profile.Employee_Profile_ID				
                LEFT JOIN (				
                     SELECT			
                        Employee_Profile_ID1 AS Employee_Profile_ID,		
                        Vehicle_Profile_ID,		
                        Request_Date,
                        SUM(CASE WHEN (Go_Back = 'Go') THEN No_of_Seat ELSE 0 END) AS Go_Seat,		
                        SUM(CASE WHEN (Go_Back = 'Back') THEN No_of_Seat ELSE 0 END) AS Back_Seat		
                        FROM Transport_Request			
                        WHERE Employee_Profile_ID1 IS NOT NULL			
                        AND Status NOT IN ('Cancelled','Draft')
                        " + filterCondition + @"
                            GROUP BY Employee_Profile_ID1, Vehicle_Profile_ID, Request_Date			
                        ) seatTaken ON seatTaken.Employee_Profile_ID = Vehicle_Profile.Employee_Profile_ID 
                AND seatTaken.Vehicle_Profile_ID = Vehicle_Profile.Vehicle_Profile_ID				
                WHERE Vehicle_Profile.Status = 'Active' AND Employee_Profile.Employee_Profile_ID = '" + DriverID + "'").ToString());
                if (DriverTotalSeatUse < 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Capacity exceeded.');", true);
                    BindDriverGrid(date);
                    BindDriverGrid2(date);
                }
                else
                {
                    UpdateRowInDatabase(Transport_Request_ID.Text, ddlDriver.SelectedItem.Value);
                    BindDriverGrid(date);
                    BindDriverGrid2(date);
                }
            }
        }

        private void UpdateRowInDatabase(string requestId, string driverId)
        {
            string Approval_Time = "";
            if (driverId == "0")
            {
                Approval_Time = "NULL";
            }
            else
            {
                Approval_Time = "GETDATE()";
            }

            string sql = @"
                        UPDATE Transport_Request
                        SET 
                            Employee_Profile_ID1 = '" + driverId + @"',
                            Approval_Time = "+ Approval_Time + @",
                            Edit_Time = GETDATE(),
                            Vehicle_Profile_ID = (SELECT Vehicle_Profile_ID FROM Vehicle_Profile WHERE Status = 'Active' AND Employee_Profile_ID = '" + driverId + @"'),
                            Status = CASE WHEN ('" + driverId + @"' <> '') THEN 'Taken' ELSE Status END
                        WHERE Transport_Request_ID = '" + requestId + @"'";

            con.ExecuteSQLQuery(sql);
        }

        protected void BindDriverGrid(string date)
        {
            string sql = @"
                            SELECT							
                                req.Transport_Request_ID,
                                REPLACE(CONVERT(varchar(11), req.Request_Date, 106), ' ', '-') AS Request_Date,					
                                SUBSTRING(CONVERT(varchar(20), req.Request_Time, 8), 1, 5) AS Request_Time,						
                                req.Go_Back,						
                                req.Manpower_Seat,						
                                req.Material_Seat,						
                                req.No_of_Seat,						
                                ISNULL(Project.Project_Initial, Project.Project_Name) AS Project,						
                                req.Remarks,
                                employee1.Schedule_Display_Name
                            FROM Transport_Request req							
                            INNER JOIN Employee_Profile employee ON employee.Employee_Profile_ID = req.Employee_Profile_ID
                            LEFT JOIN Employee_Profile employee1 ON employee1.Employee_Profile_ID = req.Employee_Profile_ID1	
                            LEFT JOIN Project ON Project.Project_ID = req.Project_ID							
                            WHERE req.Status IN ('Submitted','Taken') ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            
            string filterCondition = @" 
            AND (CAST(req.Request_Date AS DATE) = CAST(GETDATE() AS DATE) 
            AND CAST(req.Request_Time AS TIME) >= CAST(GETDATE() AS TIME)) 
            OR CAST(req.Request_Date AS DATE) =  CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)
            ";

            if (!string.IsNullOrEmpty(date) && DateTime.Parse(date) != DateTime.Today)
            {
                filterCondition = @"
                AND CAST(req.Request_Date AS DATE) 
                BETWEEN CAST(@RequestDate AS DATE)
                AND CAST(DATEADD(DAY, 1, @RequestDate) AS DATE)
                ";
                parameters.Add(new SqlParameter("@RequestDate", SqlDbType.Date) { Value = DateTime.Parse(date) });
            }
            
            sql = sql + filterCondition + " ORDER BY Request_Date asc,Request_Time ASC, employee1.Schedule_Display_Name ASC";

            DataTable dt = con.FillDatatable(sql, parameters.ToArray()); 
            DriverGrid.DataSource = dt;
            DriverGrid.DataBind();
        }

        protected void BindDriverGrid2(string date)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = @"
              SELECT				
              Employee_Profile.Schedule_Display_Name,
              REPLACE(CONVERT(varchar(11), seatTaken.Request_Date, 106), ' ', '-') AS Request_Date,		
              Vehicle_Profile.No_of_Seat - ISNULL(seatTaken.Go_Seat,0) AS Go_Seat,			
              Vehicle_Profile.No_of_Seat - ISNULL(seatTaken.Back_Seat,0) AS Back_Seat	 FROM
              (				
                  SELECT			
                  Employee_Profile_ID1 AS Employee_Profile_ID,		
                  Vehicle_Profile_ID,		
                  Request_Date,
                  SUM(CASE WHEN (Go_Back = 'Go') THEN No_of_Seat ELSE 0 END) AS Go_Seat,		
                  SUM(CASE WHEN (Go_Back = 'Back') THEN No_of_Seat ELSE 0 END) AS Back_Seat		
                  FROM Transport_Request			
                  WHERE Employee_Profile_ID1 IS NOT NULL			
                  AND Status NOT IN ('Cancelled','Draft')   ";

            if (!string.IsNullOrEmpty(date))
            {
                sql += @"
                AND CAST(Request_Date AS DATE) 
                BETWEEN CAST(@RequestDate AS DATE)
                AND CAST(DATEADD(DAY, 1, @RequestDate) AS DATE)
                ";
                parameters.Add(new SqlParameter("@RequestDate", SqlDbType.Date) { Value = DateTime.Parse(date) });
            }
            else
            {
                sql += @" 
                AND CAST(Request_Date AS DATE) 
                BETWEEN CAST(GETDATE() AS DATE)
                AND CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)
                ";
            }

            sql += @"
                        GROUP BY Employee_Profile_ID1, Vehicle_Profile_ID, Request_Date			
                    ) seatTaken 
                    INNER JOIN Vehicle_Profile ON seatTaken.Employee_Profile_ID = Vehicle_Profile.Employee_Profile_ID 
                    AND seatTaken.Vehicle_Profile_ID = Vehicle_Profile.Vehicle_Profile_ID
                    INNER JOIN Employee_Profile ON Employee_Profile.Employee_Profile_ID = Vehicle_Profile.Employee_Profile_ID
                    WHERE Vehicle_Profile.Status = 'Active'	";

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            DriverGrid2.DataSource = dt;
            DriverGrid2.DataBind();
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

        private DataTable GetDriverListByName(String Driver_ID)
        {
            string query = @"
                            SELECT Employee_Profile_ID, Schedule_Display_Name
                            FROM Employee_Profile
                            WHERE Employee_Profile_ID = " + Driver_ID + @"
                            ";

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
            e.Row.Cells[1].Style.Add("Width", "150px");
            e.Row.Cells[1].Style.Add("text-align", "center");
            e.Row.Cells[2].Style.Add("text-align", "center");
            e.Row.Cells[3].Style.Add("text-align", "center");
            e.Row.Cells[4].Style.Add("text-align", "center");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string requestID = DataBinder.Eval(e.Row.DataItem, "Transport_Request_ID").ToString();

                string DriverID = con.ExecuteSQLQueryWithOneReturn(
                string.Format(@"														
                SELECT Employee_Profile.Employee_Profile_ID 
                FROM Transport_Request 
                LEFT JOIN Employee_Profile on Transport_Request.Employee_Profile_ID1 = Employee_Profile.Employee_Profile_ID 
                WHERE Transport_Request.Transport_Request_ID = '{0}'									
                ", requestID)
                       )?.ToString();

                if (!DriverID.Equals(Session["EmpID"]) && !string.IsNullOrEmpty(DriverID))
                {
                    string Driver_name = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
                    SELECT Employee_Profile.Schedule_Display_Name 
                    FROM Transport_Request 
                    LEFT JOIN Employee_Profile on Transport_Request.Employee_Profile_ID1 = Employee_Profile.Employee_Profile_ID 
                    WHERE Transport_Request.Transport_Request_ID = '{0}'									
                    ", requestID)
                           )?.ToString();

                    DropDownList ddlDriver = (DropDownList)e.Row.FindControl("ddlDriver");
                    if (ddlDriver != null)
                    {
                        DataTable driverList = GetDriverListByName(DriverID);
                        ddlDriver.DataSource = driverList;
                        ddlDriver.DataTextField = "Schedule_Display_Name";
                        ddlDriver.DataValueField = "Employee_Profile_ID";
                        ddlDriver.DataBind();
                        ddlDriver.Items.Insert(0, new ListItem("-- Driver --", "0"));
                    }
                    if (!string.IsNullOrEmpty(Driver_name))
                    {
                        ddlDriver.Items.FindByText(Driver_name).Selected = true;
                        ddlDriver.Enabled = false;
                    }
                }
                else if (DriverID.Equals(Session["EmpID"]) || string.IsNullOrEmpty(DriverID))
                {
                    string Driver_name = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"														
                    SELECT Employee_Profile.Schedule_Display_Name 
                    FROM Transport_Request 
                    LEFT JOIN Employee_Profile on Transport_Request.Employee_Profile_ID1 = Employee_Profile.Employee_Profile_ID 
                    WHERE Transport_Request.Transport_Request_ID = '{0}'									
                    ", requestID)
                           )?.ToString();

                    DropDownList ddlDriver = (DropDownList)e.Row.FindControl("ddlDriver");
                    if (ddlDriver != null)
                    {
                        DataTable driverList = GetDriverList();
                        ddlDriver.DataSource = driverList;
                        ddlDriver.DataTextField = "Schedule_Display_Name";
                        ddlDriver.DataValueField = "Employee_Profile_ID";
                        ddlDriver.DataBind();
                        ddlDriver.Items.Insert(0, new ListItem("-- Driver --", "0"));
                    }
                    if (!string.IsNullOrEmpty(Driver_name))
                    {
                        ddlDriver.Items.FindByText(Driver_name).Selected = true;
                    }
                }


            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string goBack = DataBinder.Eval(e.Row.DataItem, "Go_Back")?.ToString();

                string requestID = DataBinder.Eval(e.Row.DataItem, "Transport_Request_ID").ToString();

                DropDownList ddlDriver = (DropDownList)e.Row.FindControl("ddlDriver");
                if (ddlDriver != null)
                {
                    ddlDriver.Attributes.Add("data-requestid", requestID);
                }

                // Set background color of the Request_Time cell
                if (goBack == "Go")
                {
                    // Replace with correct cell index (0-based) of the "Time" column
                    int timeCellIndex = 2; // Assuming "Time" is the 3rd column

                    e.Row.Cells[timeCellIndex].BackColor = System.Drawing.Color.LawnGreen;
                }
                else
                {
                    int timeCellIndex = 2; // Assuming "Time" is the 3rd column

                    e.Row.Cells[timeCellIndex].BackColor = System.Drawing.Color.DarkOrange;
                }
            }
        }

        protected void DriverGrid2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label goLabel = (Label)e.Row.FindControl("GoLabel");
                if (goLabel != null && int.TryParse(goLabel.Text, out int goValue))
                {
                    if (goValue < 0)
                    {
                        // Set background color red
                        goLabel.BackColor = System.Drawing.Color.Red;
                        goLabel.ForeColor = System.Drawing.Color.White;
                    }
                }
            }
        }

        protected void DriverGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewRequest")
            {
                string requestId = e.CommandArgument.ToString();

                // Store the ID in session
                Session["SelectedRequestID"] = requestId;

                // Redirect to another page
                Response.Redirect("View-Request.aspx");
            }
        }
    }
}