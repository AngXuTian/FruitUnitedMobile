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
using System.Web.UI.HtmlControls;
using ForSessionValue;

namespace FruitUnitedMobile.Modules
{
    public partial class Submitted_Request : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        SessionValue sessionValue = new SessionValue();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                object FromDateObj = Session["TransportSubmittedLeave_FromDate"];
                if (FromDateObj != null)
                {
                    DateFromTextBox.Text = FromDateObj.ToString();
                }

                object ToDateObj = Session["TransportSubmittedLeave_Todate"];
                if (ToDateObj != null)
                {
                    DateToTextBox.Text = ToDateObj.ToString();
                }

                object ProjectObj = Session["TransportSubmittedLeave_Project"];
                if (ProjectObj != null)
                {
                    ProjectTextBox.Text = ProjectObj.ToString();
                }

                string employeeId = Request.QueryString["EmployeeID"];
                if (!string.IsNullOrEmpty(employeeId))
                {
                    Session["EmployeeID"] = employeeId;
                }
                else
                {
                    employeeId = Session["EmployeeID"] as string;
                }
                BindDisplay(employeeId);
            }
        }

        private void BindDisplay(string employeeId)
        {
            string fromDateText = DateFromTextBox.Text.Trim();
            string toDateText = DateToTextBox.Text.Trim();
            string ProjectText = ProjectTextBox.Text.Trim();
            employeeId = Session["EmpID"] as string;

            string filterCondition = " AND req.Employee_Profile_ID = @EmployeeID";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@EmployeeID", employeeId));

            DateTime parsedDate;
            if (DateTime.TryParse(fromDateText, out parsedDate))
            {
                filterCondition += " AND CAST(req.Request_Date AS DATE) >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", parsedDate));
            }

            if (DateTime.TryParse(toDateText, out parsedDate))
            {
                filterCondition += " AND CAST(req.Request_Date AS DATE) <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", parsedDate));
            }

            if (!string.IsNullOrEmpty(ProjectText))
            {
                filterCondition += " AND Project.Project_Name LIKE '%"+ ProjectText + "%'";
            }

            string sql = @"
    SELECT					
        req.Transport_Request_ID,				
        req.Request_Date,				
        SUBSTRING(CONVERT(varchar(20), req.Request_Time, 8), 1, 5) AS Request_Time,				
        req.Go_Back,				
        req.Manpower_Seat,				
        req.Material_Seat,				
        req.No_of_Seat,
        req.Status,
        ISNULL(Project.Project_Initial, Project.Project_Name) AS Project,
        transport.Transport_Mode,
        req.Remarks,				
        vehicle.Vehicle_No + ' (' + ISNULL(driver.Schedule_Display_Name, driver.Display_Name) + ')' AS Driver,
        vehicle.Vehicle_No,
        COALESCE(employee.Schedule_Display_Name,employee.Display_Name) AS Requestor
    FROM Transport_Request req					
    INNER JOIN Employee_Profile employee ON employee.Employee_Profile_ID = req.Employee_Profile_ID					
    LEFT JOIN Project ON Project.Project_ID = req.Project_ID					
    LEFT JOIN Employee_Profile driver ON driver.Employee_Profile_ID = req.Employee_Profile_ID1					
    LEFT JOIN Vehicle_Profile vehicle ON vehicle.Vehicle_Profile_ID = req.Vehicle_Profile_ID
    LEFT JOIN Transport_Mode transport ON req.Transport_Mode_ID = transport.Transport_Mode_ID		
    WHERE req.Status IN ('Submitted','Taken', 'Cancelled') " + filterCondition + @"
    ORDER BY req.Request_Date DESC, req.Request_Time DESC";
            try
            {
                DataTable dt = con.FillDatatable(sql, parameters.ToArray());

                rptTransportRequests.DataSource = dt;
                rptTransportRequests.DataBind();
            }
            catch
            {
                throw new Exception(sql);
            }

            foreach (RepeaterItem item in rptTransportRequests.Items)
            {
                Label tdTime = (Label)item.FindControl("tdTime");
                Label DateLabel = (Label)item.FindControl("DateLabel");
                Label StatusLabel = (Label)item.FindControl("Status");
                Label GO_Back = (Label)item.FindControl("GoBack");
                string labelText = GO_Back.Text;
                string StatusText = StatusLabel.Text;

                GO_Back.Style["Display"] = "None";
                StatusLabel.Style["Display"] = "None";
                if (labelText.ToLower().IndexOf("go") > -1)
                {
                    tdTime.BackColor = Color.LawnGreen;
                }
                else
                {
                    tdTime.BackColor = Color.DarkOrange;
                }

                if (StatusText.ToLower().IndexOf("cancelled") > -1)
                {
                    DateLabel.Text = DateLabel.Text + " (Cancelled)";
                    DateLabel.ForeColor = Color.Red;
                }
            }
        }
      
        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string employeeId = Request.QueryString["EmployeeID"];
            Session["TransportSubmittedLeave_FromDate"] = DateFromTextBox.Text.Trim();
            Session["TransportSubmittedLeave_Todate"] = DateToTextBox.Text.Trim();
            Session["TransportSubmittedLeave_Project"] = ProjectTextBox.Text.Trim();
            BindDisplay(employeeId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string employeeId = Request.QueryString["EmployeeID"];

            Button btn = (Button)sender;
            string requestId = btn.CommandArgument;

            string sql = @"
                        UPDATE Transport_Request
                    SET Status = 'Cancelled',	
                    Edit_Time = GETDATE(),
                    Approval_Time = NULL,
	                Employee_Profile_ID1 = NULL,	
	                Vehicle_Profile_ID = NULL	
                WHERE Transport_Request_ID = '" + requestId + @"'";


            con.ExecuteSQLQueryWithOneReturn(sql);

            BindDisplay(employeeId);
        }

        protected void rptTransportRequests_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
           
        }

        private void SaveCompressedImage(Bitmap image, string savePath, int quality)
        {
            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            image.Save(savePath, jpgEncoder, encoderParams);
        }
    }
}