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


namespace FruitUnitedMobile.Modules
{
    public partial class View_Request : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string requestId = Session["SelectedRequestID"] as string;
                BindDisplay();
            }
        }

        private void BindDisplay()
        {
            string requestId = Session["SelectedRequestID"].ToString();

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@transportReqID", requestId)
            };

            string sql = @"
        SELECT					
            req.Transport_Request_ID,				
            req.Request_Date,				
            SUBSTRING(CONVERT(varchar(20), req.Request_Time, 8), 1, 5) AS Request_Time,				
            req.Go_Back,				
            req.Manpower_Seat,				
            req.Material_Seat,				
            req.No_of_Seat,				
            ISNULL(Project.Project_Initial, Project.Project_Name) AS Project,				
            req.Remarks,				
            driver.Schedule_Display_Name AS Driver,				
            vehicle.Vehicle_No,
            COALESCE(employee.Schedule_Display_Name,employee.Display_Name) AS Requestor
        FROM Transport_Request req					
        INNER JOIN Employee_Profile employee ON employee.Employee_Profile_ID = req.Employee_Profile_ID					
        LEFT JOIN Project ON Project.Project_ID = req.Project_ID					
        LEFT JOIN Employee_Profile driver ON driver.Employee_Profile_ID = req.Employee_Profile_ID1					
        LEFT JOIN Vehicle_Profile vehicle ON vehicle.Vehicle_Profile_ID = req.Vehicle_Profile_ID					
        WHERE req.Status IN ('Submitted','Taken','Cancelled') 
          AND req.Transport_Request_ID = @transportReqID";

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                rptTransportRequests.DataSource = dt;
                rptTransportRequests.DataBind();
            }

            foreach (RepeaterItem item in rptTransportRequests.Items)
            {
                Label tdTime = (Label)item.FindControl("tdTime");
                Label GO_Back = (Label)item.FindControl("GoBack");
                string labelText = GO_Back.Text;
                GO_Back.Style["Display"] = "None";
                if (labelText.ToLower().IndexOf("go") > -1)
                {
                    tdTime.BackColor = Color.LawnGreen;
                }
                else
                {
                    tdTime.BackColor = Color.DarkOrange;
                }
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string employeeId = Request.QueryString["EmployeeID"];

            BindDisplay();
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string employeeId = Session["EmpID"].ToString();
            string requestId = Session["SelectedRequestID"] as string;

            string sql = @"
                        UPDATE Transport_Request
                        SET Status = 'Cancelled',	
                        Edit_Time = GETDATE(),
	                    Employee_Profile_ID1 = NULL,	
	                    Vehicle_Profile_ID = NULL	
                        WHERE Transport_Request_ID = '" + requestId + @"'";

            con.ExecuteSQLQueryWithOneReturn(sql);
            BindDisplay();
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