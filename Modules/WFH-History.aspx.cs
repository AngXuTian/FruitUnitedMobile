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
    public partial class WFH_History : System.Web.UI.Page
    {
        Connection con = new Connection();
        SessionValue sessionValue = new SessionValue();
        protected void Page_Load(object sender, EventArgs e)
        {
            //string date = DateSearch.Text.Trim();

            if (!IsPostBack)
            {

                object FromDateObj = Session["WFHHistory_FromDate"];
                if (FromDateObj != null)
                {
                    DateFromTextBox.Text = FromDateObj.ToString();
                }

                object ToDateObj = Session["WFHHistory_ToDate"];
                if (ToDateObj != null)
                {
                    DateToTextBox.Text = ToDateObj.ToString();
                }

                object EmployeeObj = Session["WFHHistory_Employee"];
                if (EmployeeObj != null)
                {
                    EmployeeTextBox.Text = EmployeeObj.ToString();
                }

                object DepartmentObj = Session["WFHHistory_Department"];
                if (DepartmentObj != null)
                {
                    DepartmentTextBox.Text = DepartmentObj.ToString();
                }

                BindWFHHistoryGrid();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            Session["WFHHistory_FromDate"] = DateFromTextBox.Text.Trim();
            Session["WFHHistory_ToDate"] = DateToTextBox.Text.Trim();
            Session["WFHHistory_Employee"] = EmployeeTextBox.Text.Trim();
            Session["WFHHistory_Department"] = DepartmentTextBox.Text.Trim();
            BindWFHHistoryGrid();
        }
        protected void WFHHistoryGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Check if the LinkButton clicked has the "UpdateRow" command
            if (e.CommandName == "CancelWFH")
            {
                // Get the ID from CommandArgument
                int WFH_ID = Convert.ToInt32(e.CommandArgument);
                string SQLCancelQuery = @"
                UPDATE WFH		
                SET Status = 'Cancelled'		
                WHERE WFH_ID = '"+ WFH_ID + @"'
                ";

                con.ExecuteSQLQuery(SQLCancelQuery);
                BindWFHHistoryGrid();
            }
        }
        protected void BindWFHHistoryGrid()
        {
            string fromDate = DateFromTextBox.Text.Trim();
            string toDate = DateToTextBox.Text.Trim();
            string Employee = EmployeeTextBox.Text.Trim();
            string Department = DepartmentTextBox.Text.Trim();

            string filterCondition = "";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                filterCondition += " AND CONVERT(date, WFH.WFH_Date) BETWEEN @FromDate AND @ToDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.Date) { Value = DateTime.Parse(fromDate) });
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.Date) { Value = DateTime.Parse(toDate) });
            }
            else if (!string.IsNullOrEmpty(fromDate))
            {
                filterCondition += " AND CONVERT(date, WFH.WFH_Date) >= Convert(date, '" + fromDate + "')";
            }
            else if (!string.IsNullOrEmpty(toDate))
            {
                filterCondition += " AND CONVERT(date, WFH.WFH_Date) <= Convert(date, '" + toDate + "')";
            }
            else
            {
                filterCondition += " AND CONVERT(date, WFH.WFH_Date) >= Convert(date, GETDATE())";
            }

            if (!string.IsNullOrEmpty(Employee))
            {
                filterCondition += @"
                AND Employee_Profile.Dispay_Name LIKE '%" + Employee + @"%'
                ";
            }

            if (!string.IsNullOrEmpty(Department))
            {
                filterCondition += @"
                AND Department_Profile.Department LIKE '%" + Department + @"%'
                ";
            }

            string sql = @"
            SELECT								
	            WFH.WFH_ID,							
	            Employee_Profile.Display_Name,
                Employee_Profile.Employee_Profile_ID,
                REPLACE(convert(varchar, WFH.WFH_Date, 106),' ','-') AS WFH_Date,					
	            Department_Profile.Department,							
	            WFH.Remarks,							
	            WFH.Status							
            FROM WFH								
            INNER JOIN Employee_Profile ON Employee_Profile.Employee_Profile_ID = WFH.Employee_Profile_ID								
            INNER JOIN Department_Profile ON Department_Profile.Department_Profile_ID = WFH.Department_Profile_ID								
            WHERE WFH.Status IN ('Submitted','Cancelled') " + filterCondition + @" ORDER BY WFH.WFH_Date DESC 													
            ";

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            WFHHistoryGrid.DataSource = dt;
            WFHHistoryGrid.DataBind();
        }

        protected void WFHHistoryGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[4].Style.Add("text-align", "center");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField StatusHF = (HiddenField)e.Row.FindControl("StatusHF");
                HiddenField Employee_Profile_IDHF = (HiddenField)e.Row.FindControl("Employee_Profile_IDHF");
                string Employee_Profile_ID = Employee_Profile_IDHF.Value;
                if (StatusHF.Value == "Cancelled")
                {
                    e.Row.Cells[0].Style.Add("color", "Red");
                }
                if (!Employee_Profile_IDHF.Value.Contains(Session["EmpID"].ToString()))
                {
                    e.Row.Cells[4].Style.Add("display", "none");
                }
            }
        }

    }
}