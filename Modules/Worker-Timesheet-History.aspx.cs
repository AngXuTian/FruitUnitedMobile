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
    public partial class Worker_Timesheet_History : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWorkerTimeSheetHistoryData();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string soNo = SONoTextBox.Text.Trim();
            string projectName = ProjectNameTextBox.Text.Trim();
            string FromDate = DateFromTextBox.Text.Trim();
            string ToDate = DateToTextBox.Text.Trim();
            BindWorkerTimeSheetHistoryData(soNo, projectName, FromDate ,ToDate);
        }


        protected void BindWorkerTimeSheetHistoryData(string soNo, string projectName, string FromDate, string ToDate)
        {
            string filterCondition = "";
            string sql = string.Format(@"
                        	            SELECT					
                                            REPLACE(convert(varchar, timesheet.Date_In, 106),' ','-') AS Date_In,
                                            FORMAT(timesheet.Time_In,'HH:mm') AS Time_In,
                                            REPLACE(convert(varchar, timesheet.Date_Out, 106),' ','-') AS Date_Out,
                                            FORMAT(timesheet.Time_Out,'HH:mm') AS Time_Out,			
	                                        timesheet.Type,				
	                                        Project.Project_Name,				
	                                        'Project No : ' + Project.Project_No AS Project_No,
                                            timesheet.Remarks
                                        FROM Worker_Timesheet timesheet					
                                        LEFT JOIN Project ON Project.Project_ID = timesheet.Project_ID					
                                        WHERE timesheet.Employee_Profile_ID = '" + Session["EmpID"] + @"'				
                       ");

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(soNo))
            {
                filterCondition += " AND Project.Project_No LIKE '%" + soNo + "%'";
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                filterCondition += " AND Project.Project_Name LIKE '%" + projectName + "%'";
            }

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                DateTime FromDateD;
                DateTime ToDateD;
                FromDateD = DateTime.ParseExact(FromDate.Replace("-", " "), "d MMM yyyy", CultureInfo.InvariantCulture);
                ToDateD = DateTime.ParseExact(ToDate.Replace("-", " "), "d MMM yyyy", CultureInfo.InvariantCulture);
                filterCondition += " AND ((timesheet.Date_In BETWEEN '" + FromDateD.ToString("yyyy-MM-dd") + "T00:00:00.000' AND '" + ToDateD.ToString("yyyy-MM-dd") + "T23:59:59.999')  OR (timesheet.Date_Out BETWEEN '" + FromDateD.ToString("yyyy-MM-dd") + "T00:00:00.000' AND '" + ToDateD.ToString("yyyy-MM-dd") + "T23:59:59.999'))";
            }
            else if (!string.IsNullOrEmpty(FromDate))
            {
                DateTime FromDateD;
                FromDateD = DateTime.ParseExact(FromDate.Replace("-", " "), "d MMM yyyy", CultureInfo.InvariantCulture);
                filterCondition += " AND (timesheet.Date_In >= '" + FromDateD.ToString("yyyy-MM-dd") + "T00:00:00.000' OR timesheet.Date_Out >= '" + FromDateD.ToString("yyyy-MM-dd") + "T00:00:00.000')";
            }
            else if (!string.IsNullOrEmpty(ToDate))
            {
                DateTime ToDateD;
                ToDateD = DateTime.ParseExact(ToDate.Replace("-", " "), "d MMM yyyy", CultureInfo.InvariantCulture);
                filterCondition += " AND (timesheet.Date_In <= '" + ToDateD.ToString("yyyy-MM-dd") + "T23:59:59.999' OR timesheet.Date_Out <= '" + ToDateD.ToString("yyyy-MM-dd") + "T23:59:59.999')";
            }


            if (!string.IsNullOrEmpty(filterCondition))
            {
                sql = sql + filterCondition;
            }

            sql = sql + "ORDER BY ISNULL(timesheet.Date_Out, GETDATE()) DESC, ISNULL(timesheet.Time_Out, GETDATE()) DESC";
            DataTable dt = con.FillDatatable(sql);
            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }



        protected void InvoiceGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "left");
            e.Row.Cells[2].Style.Add("text-align", "left");
            e.Row.Cells[3].Style.Add("text-align", "left");
            e.Row.Cells[4].Style.Add("text-align", "left");
            e.Row.Cells[0].Style.Add("vertical-align", "middle");
            e.Row.Cells[1].Style.Add("vertical-align", "middle");
            e.Row.Cells[2].Style.Add("vertical-align", "middle");
            e.Row.Cells[3].Style.Add("vertical-align", "middle");
            e.Row.Cells[4].Style.Add("vertical-align", "middle");


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField lbl_Code = (HiddenField)e.Row.FindControl("LabelType");
                if (lbl_Code.Value == "Office")
                {
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#ADD8E6");
                }
                else if (lbl_Code.Value == "Training")
                {
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFE0");
                }
                else if (lbl_Code.Value == "Attend SIC")
                {
                    e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#90EE90");
                }
            }
        }

        protected void BindWorkerTimeSheetHistoryData()
        {
            string sql = string.Format(@"
                        	            SELECT
                                            REPLACE(convert(varchar, timesheet.Date_In, 106),' ','-') AS Date_In,
                                            FORMAT(timesheet.Time_In,'HH:mm') AS Time_In,
                                            REPLACE(convert(varchar, timesheet.Date_Out, 106),' ','-') AS Date_Out,
                                            FORMAT(timesheet.Time_Out,'HH:mm') AS Time_Out,			
	                                        timesheet.Type,				
	                                        Project.Project_Name,				
	                                        'Project No : ' + Project.Project_No AS Project_No,
                                            timesheet.Remarks
                                        FROM Worker_Timesheet timesheet					
                                        LEFT JOIN Project ON Project.Project_ID = timesheet.Project_ID					
                                        WHERE timesheet.Employee_Profile_ID = '" + Session["EmpID"] + @"'
                                        ORDER BY ISNULL(timesheet.Date_Out, GETDATE()) DESC, ISNULL(timesheet.Time_Out, GETDATE()) DESC
                       ");

            DataTable dt = con.FillDatatable(sql);

            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }
    }
}