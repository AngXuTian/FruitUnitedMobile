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

namespace FruitUnitedMobile.Modules
{
    public partial class Worker_Timesheet_Out : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;
        string workerTimesheetID = "";
        string dateIn = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                workerTimesheetID = Request.QueryString["worker_timesheet_id"];
                if (!string.IsNullOrEmpty(workerTimesheetID))
                {
                    BindDisplay();
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                fileUpload1.Attributes.Add("capture", "environment"); // Use back camera
                fileUpload1.Attributes.Add("accept", "image/*");
            }
        }

        private void BindDisplay()
        {
            string work_timesheet_out = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT Worker_Timesheet_ID FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Date_Out IS NULL ", Session["EmpID"])
                )?.ToString();

            string query = @"
            SELECT 
            timesheet.Worker_Timesheet_ID, 
            Project.Project_ID, 
            Project.Project_No, 
            Project.Project_Name, 
            timesheet.Shift_Type, 
            timesheet.Office, 
            timesheet.Date_In, 
            timesheet.Time_In, 
            timesheet.Remarks,
            timesheet.Type,
            Project_Site.Postal_Code 
            FROM Worker_Timesheet timesheet 
            LEFT JOIN Project ON Project.Project_ID = timesheet.Project_ID 
            LEFT JOIN Project_Site ON Project.Project_Site_ID = Project_Site.Project_Site_ID 
            WHERE timesheet.Worker_Timesheet_ID = @Worker_Timesheet_ID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Worker_Timesheet_ID", workerTimesheetID);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            txtShiftType.Text = reader["Shift_Type"]?.ToString() ?? string.Empty;
                            txtType.Text = reader["Type"]?.ToString() ?? string.Empty;
                            txtProject.Text = reader["Project_No"]?.ToString() + " / " + reader["Project_Name"]?.ToString();
                            txtRemark.Text = reader["Remarks"]?.ToString() ?? string.Empty;
                            PostCode.Text = reader["Postal_Code"]?.ToString() ?? string.Empty;
                            dateIn = reader["Date_In"]?.ToString() ?? string.Empty;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this ,this.GetType(), "alert", "alert('No data found for the given Worker_Timesheet_ID.');", true);
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            //DateTime today = DateTime.ParseExact("2025-05-15 23:58:00,000", "yyyy-MM-dd HH:mm:ss,fff",
            //System.Globalization.CultureInfo.InvariantCulture);
            string projectID = "";
            string type = "";
            string clockOutType = ddlClockType.SelectedValue;
            string employeeID = "";
            string clockInType = "";
            string inputTime = txtTimeIn.Text;
            DateTime inputTimeD = DateTime.ParseExact(inputTime, "H:mm", null, System.Globalization.DateTimeStyles.None);

            int gracePeriod = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                                string.Format(@"SELECT ISNULL(Check_Out_Grace_Period,0) FROM Working_Hour WHERE CONVERT(date, GETDATE()) BETWEEN CONVERT(Date, From_Date) AND CONVERT(Date, ISNULL(To_Date, GETDATE()))", today))?.ToString());
            DateTime graceTime = today.AddMinutes(gracePeriod);
            graceTime = graceTime.AddSeconds(-graceTime.Second);

            if (inputTimeD > graceTime)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Input time must be within actual time grace period.');", true);
                return;
            }

            //To change the date to next day if the selected time falls within grace period & is next day
            //Cater for scenario where worker check out at 23:58 but select the time to be 00:00, this should be accepted if within grace period and check out date, system to default to next day
            if (today.Date < graceTime.Date)
            {
                if (inputTimeD.Hour < 1 && inputTimeD.Minute <= gracePeriod)
                {
                    inputTimeD = inputTimeD.AddDays(1);
                }
            }

            string TimeValidation = con.ExecuteSQLQueryWithOneReturn(@"
            SELECT COUNT(1) from Worker_Timesheet 
            WHERE Worker_Timesheet_ID = '"+ Request.QueryString["worker_timesheet_id"] + @"' 
            AND CONVERT(DATETIME, CAST(CAST(Date_In AS date) AS nvarchar(10)) +' '+ CAST(CAST(Time_In AS time) AS nvarchar(8))) < CONVERT(DATETIME,'"+ inputTimeD.ToString("yyyy-MM-dd") + " " + txtTimeIn.Text + @"')
            ").ToString();

            if (TimeValidation == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Check Out cannot early than Check In.');", true);
                return;
            }

            string query = "SELECT Project.Project_ID, timesheet.Type, timesheet.Employee_Profile_ID, timesheet.Clock_Type_In FROM Worker_Timesheet timesheet LEFT JOIN Project ON Project.Project_ID = timesheet.Project_ID WHERE timesheet.Worker_Timesheet_ID = @Worker_Timesheet_ID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Worker_Timesheet_ID", Request.QueryString["worker_timesheet_id"]);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            projectID = reader["Project_ID"]?.ToString() ?? string.Empty;
                            type = reader["Type"]?.ToString() ?? string.Empty;
                            employeeID = reader["Employee_Profile_ID"]?.ToString() ?? string.Empty;
                            clockInType = reader["Clock_Type_In"]?.ToString() ?? string.Empty;
                        }
                    }
                }
            }

            //check if any overlap timing
            //int overlapTime = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
            //        string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{1}' AND CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106) + ' {0}') BETWEEN CONVERT(datetime, CONVERT(nvarchar, Date_In, 106) + ' ' + CONVERT(nvarchar, Actual_Time_In, 108)) AND CONVERT(datetime, CONVERT(nvarchar, Date_Out, 106) + ' ' + CONVERT(nvarchar, Actual_Time_Out, 108))", inputTime, employeeID))?.ToString());

            int overlapTime = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{1}' AND CONVERT(datetime, '{0}') BETWEEN CONVERT(datetime, CONVERT(nvarchar, Date_In, 106) + ' ' + CONVERT(nvarchar, Actual_Time_In, 108)) AND CONVERT(datetime, CONVERT(nvarchar, Date_Out, 106) + ' ' + CONVERT(nvarchar, Actual_Time_Out, 108))", inputTimeD.ToString("yyyy-MM-dd HH:mm:ss"), employeeID))?.ToString());

            if (overlapTime > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Overlap time');", true);
                return;
            }

            int overpassLogoutTime = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                 string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE CAST(DATEADD(day, 1, DATEADD(hour,4, Date_In)) AS VARCHAR(30)) < CONVERT(datetime, '{0}') AND Worker_Timesheet_ID  = '{1}'", inputTimeD.ToString("yyyy-MM-dd HH:mm:ss"), Request.QueryString["worker_timesheet_id"]))?.ToString());

            if (overpassLogoutTime > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Overpass Logout Time.');", true);
                return;
            }


            //Only 1 OT-Out / Normal-Out per day only
            if (clockOutType == "OT-Out" || clockOutType == "Normal-Out")
            {
                int noOfOTNormalOut = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Clock_Type_Out = '{1}' 
                                        AND CONVERT(nvarchar, Date_In, 106) = CONVERT(nvarchar, Date_Out, 106) AND CONVERT(nvarchar, Date_Out, 106) = CONVERT(nvarchar, GETDATE(), 106)", employeeID, clockOutType))?.ToString());

                if (noOfOTNormalOut > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Maximum of 1 OT-Out / Normal-Out is allowed for each day only');", true);
                    return;
                }
            }

            //Validation 2
            //If project requires scope, check if there is any verified scope and all submitted scope must be verified
            if (projectID != "")
            {
                //int verifiedProject = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                //   string.Format(@"SELECT COUNT(1) FROM Project_Task_Completion completion INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID WHERE Project_Scope.Project_ID = '{0}' AND CONVERT(nvarchar, completion.Verified_On, 106) = CONVERT(nvarchar, GETDATE(), 106)", projectID)
                //)?.ToString());

                int verifiedProject = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Project_Task_Completion completion INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID 
                                        INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID WHERE Project_Scope.Project_ID = '{0}' 
                                                AND CONVERT(nvarchar, completion.Verified_On, 106) = CONVERT(nvarchar, GETDATE(), 106)", projectID)
                )?.ToString());

                int submittedProject = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Project_Task_Completion completion INNER JOIN Project_Scope_Task task ON task.Project_Scope_Task_ID = completion.Project_Scope_Task_ID 
                                        INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID WHERE Project_Scope.Project_ID = '{0}' 
                                                AND completion.Verified_On IS NULL", projectID)
                )?.ToString());

                string requireScope = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT Mobile_Visibility FROM Project WHERE Project_ID = '{0}'", projectID)
                )?.ToString();

                int workScopeCount = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Project_Scope_Task task INNER JOIN Project_Scope ON Project_Scope.Project_Scope_ID = task.Project_Scope_ID WHERE Project_Scope.Project_ID = '{0}'", projectID)
                )?.ToString());

                if ((verifiedProject == 0 || submittedProject > 0) && type == "Project" && requireScope == "Y")
                {
                    ClientScript.RegisterStartupScript( this.GetType(), "alert", "alert('Please verify all task completion.');", true);
                    return;
                }

                //Validation 3
                string subconValidation = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT TOP 1 Subcon FROM Project_with_Subcon WHERE Project_ID = '{0}' AND Task_Date = '{1}'", projectID, today.ToString("yyyy-MM-dd"))
                )?.ToString();

                if (subconValidation == "Y")
                {
                    int subcon = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Subcon_Timesheet WHERE Project_ID = '{0}' AND Date_Out = '{1}'", projectID, today.ToString("yyyy-MM-dd"))
                       )?.ToString());

                    if (subcon == 0 && type == "Project")
                    {
                        ClientScript.RegisterStartupScript( this.GetType(), "alert", "alert('Please make sure subcon has been check out');", true);
                        return;
                    }
                }
            }

            // Initialize image variable
            string imagePath = null;

            // Handle file upload
            if (fileUpload1.HasFile)
            {
                string baseDir = ConfigurationManager.AppSettings["SystemPath"];
                string uploadFolder = baseDir + @"Source\UploadImages\Worker_Timesheet\";

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Validate file type
                string fileExtension = Path.GetExtension(fileUpload1.FileName).ToLower();
                string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid file type. Please upload an image file.');", true);
                    return;
                }

                string employeeName = con.ExecuteSQLQueryWithOneReturn(@"SELECT Employee_Name FROM Employee_Profile WHERE Employee_Profile_ID ='" + Session["EmpID"] + @"' ").ToString();

                // Generate a unique filename
                string fileName = Path.GetFileNameWithoutExtension(fileUpload1.FileName);
                string uniqueName = employeeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_OUT" + fileExtension;
                string savePath = Path.Combine(uploadFolder, uniqueName);

                int fileSize = fileUpload1.PostedFile.ContentLength;
                int maxFileSize = 2 * 1024 * 1024; // 2MB

                try
                {
                    if (fileSize > maxFileSize)
                    {
                        // Compress and resize the image if it is larger than 2MB
                        using (Stream fileStream = fileUpload1.PostedFile.InputStream)
                        {
                            using (Image originalImage = Image.FromStream(fileStream))
                            {
                                using (Bitmap resizedImage = ResizeImage(originalImage, 1024, 1024))
                                {
                                    SaveCompressedImage(resizedImage, savePath, 75); // Compress with 75% quality
                                }
                            }
                        }
                    }
                    else
                    {
                        // Save original file if it's within size limit
                        fileUpload1.SaveAs(savePath);
                    }

                    imagePath = uniqueName; // Store file name for database
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error uploading file: " + ex.Message + "');", true);
                    return;
                }
            }

            string overlapQuery = @"
            SELECT Time_In, Time_Out
            FROM Worker_Timesheet
            WHERE Date_Out = @TodayDate AND Time_IN IS NOT NULL AND Time_Out IS NOT NULL AND Employee_Profile_ID= @Employee_Profile_ID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmdCheck = new SqlCommand(overlapQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@TodayDate", inputTimeD.ToString("yyyy-MM-dd"));
                    //cmdCheck.Parameters.AddWithValue("@TodayDate", today.ToString("yyyy-MM-dd"));
                    cmdCheck.Parameters.AddWithValue("@Employee_Profile_ID", Session["EmpID"]);

                    using (SqlDataReader reader = cmdCheck.ExecuteReader())
                    {
                        DateTime newTimeIn;
                        if (!DateTime.TryParse(txtTimeIn.Text, out newTimeIn))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Input times cannot overlap with other timesheets.');", true);
                            return;
                        }

                        while (reader.Read())
                        {
                            DateTime existingTimeIn = Convert.ToDateTime(reader["Time_In"]);
                            DateTime existingTimeOut = Convert.ToDateTime(reader["Time_Out"]);

                            if (newTimeIn >= existingTimeIn && newTimeIn <= existingTimeOut)
                            {
                                ClientScript.RegisterStartupScript( this.GetType(), "alert", "alert('Time overlaps with an existing entry.');", true);
                                return; 
                            }
                        }
                    }
                }
            }

            if (type == "Attend SIC")
            {
                string insertQuery2 = @"

                INSERT INTO Project_SIC
                (
                    Creation_Time,
                    Project_ID,
                    Employee_Profile_ID,
                    From_Date
                )
                (
                    SELECT
                        GETDATE(),
                        Project_ID,
                        Employee_Profile_ID,
                        @Date_Out
                    FROM Worker_Timesheet
                    WHERE Worker_Timesheet_ID = @Worker_Timesheet_ID	
                    AND Type = 'Attend SIC'
                    AND (
                        SELECT COUNT(1) FROM Project_SIC SIC 
                        WHERE SIC.Project_ID = Worker_Timesheet.Project_ID 
                        AND SIC.Employee_Profile_ID = Worker_Timesheet.Employee_Profile_ID
                        AND SIC.From_Date <= Worker_Timesheet.Date_Out
                        AND ISNULL(SIC.To_Date, Worker_Timesheet.Date_Out) >= Worker_Timesheet.Date_Out
                    ) = 0
                )

                ";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery2, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Worker_Timesheet_ID", Convert.ToInt32(Request.QueryString["worker_timesheet_id"]));
                        cmdInsert.Parameters.AddWithValue("@Date_Out", inputTimeD.ToString("yyyy-MM-dd"));
                        cmdInsert.ExecuteNonQuery();
                    }
                }
            }


            string insertQuery = @"

	        UPDATE Worker_Timesheet					
	        SET Clock_Type_Out = @Clock_Type,					
		        Date_Out = @DateOut,				
		        Actual_Time_Out = @Time,
                System_Time_Out = GETDATE(),
		        Break_Min = @Break,
                Break_Hour = CASE WHEN @Break = 0 THEN 0 ELSE CAST(@Break AS FLOAT) / 60 END,
		        Remarks = @Remark,				
		        Image_Out = @Image,
		        CheckOut_Latitude = @Latitude,
		        CheckOut_Longitude = @Longitude,
                Destination_Distance_CheckOut = @Distance
	        WHERE Worker_Timesheet_ID = @Worker_Timesheet_ID		

            ";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@DateOut", inputTimeD.ToString("yyyy-MM-dd"));
                    //cmdInsert.Parameters.AddWithValue("@DateOut", today.ToString("yyyy-MM-dd"));
                    cmdInsert.Parameters.AddWithValue("@Clock_Type", ddlClockType.SelectedValue);
                    cmdInsert.Parameters.AddWithValue("@Time", txtTimeIn.Text);
                    if (string.IsNullOrWhiteSpace(txtBreak.Text))
                    {
                        cmdInsert.Parameters.AddWithValue("@Break", DBNull.Value);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@Break", Convert.ToInt32(txtBreak.Text));
                    }
                    cmdInsert.Parameters.AddWithValue("@Remark", txtRemark.Text);
                    cmdInsert.Parameters.AddWithValue("@Image", imagePath ?? DBNull.Value.ToString());
                    cmdInsert.Parameters.AddWithValue("@Worker_Timesheet_ID", Convert.ToInt32(Request.QueryString["worker_timesheet_id"]));
                    cmdInsert.Parameters.AddWithValue("@Latitude", Latitude.Text);
                    cmdInsert.Parameters.AddWithValue("@Longitude", Longitude.Text);
                    cmdInsert.Parameters.AddWithValue("@Distance", Distance.Text);
                    cmdInsert.ExecuteNonQuery();
                }

                // Call stored procedure
                using (SqlCommand collectionCmd = new SqlCommand("Update_WorkerTimesheet", conn))
                {
                    collectionCmd.CommandType = CommandType.StoredProcedure;
                    collectionCmd.Parameters.AddWithValue("@timesheetID", Convert.ToInt32(Request.QueryString["worker_timesheet_id"]));
                    collectionCmd.ExecuteNonQuery();
                }

                using (SqlCommand collectionCmd = new SqlCommand("Update_WorkerTimesheetAllowance", conn))
                {
                    collectionCmd.CommandType = CommandType.StoredProcedure;
                    collectionCmd.Parameters.AddWithValue("@timesheetID", Convert.ToInt32(Request.QueryString["worker_timesheet_id"]));
                    collectionCmd.ExecuteNonQuery();
                }

                // Success message
                //ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Timesheet submitted successfully!');", true);
                string targetUrl = "~/Modules/Menu.aspx";
                Response.Redirect(targetUrl);
                //            string script = $@"
                //    alert('Timesheet submitted successfully!');
                //    window.location.href = '{targetUrl}';
                //";
                //            ClientScript.RegisterStartupScript( this.GetType(), "success", script, true);
            }
        }
        private Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
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