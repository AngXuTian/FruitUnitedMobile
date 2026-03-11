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
    public partial class Worker_Timesheet_In : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdown();
            }
            if (Request.Browser.IsMobileDevice)
            {
                fileUpload1.Attributes.Add("capture", "environment"); // Use back camera
                fileUpload1.Attributes.Add("accept", "image/*");
            }
        }

        private void BindDropdown()
        {

            string query = "SELECT Project_ID, Project_No + ' / ' + Project_Name AS ProjectDisplay FROM Project WHERE Status IN ('Open','Ongoing')";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlProject.DataSource = reader;
                    ddlProject.DataTextField = "ProjectDisplay"; // Text displayed in dropdown
                    ddlProject.DataValueField = "Project_ID"; // Value for each item
                    ddlProject.DataBind();
                    ddlProject.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        protected void ProjectDropDown_Change(object sender, EventArgs e)
        {
            string Project_ID = ddlProject.SelectedValue;

            string query = @"SELECT Project_Site.Postal_Code FROM Project LEFT JOIN Project_Site ON Project.Project_Site_ID = Project_Site.Project_Site_ID WHERE Project.Project_ID = '"+ Project_ID + "'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PostCode.Text = reader["Postal_Code"].ToString();
                    }
                    
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string rbOfficeStr = "N";

            string selectedType = ddlType.SelectedValue;
            string ddlProjectValue = ddlProject.SelectedValue;
            string selectedShiftType = ddlShiftType.SelectedValue;
            string selectedClockType = ddlClockType.SelectedValue;
            string inputTime = txtTimeIn.Text;
            string isLate = "";

            DateTime today = DateTime.Now;
            int d = (int)today.DayOfWeek;

            if (selectedType == "Office")
            {
                rbOfficeStr = "Y";

            }
            else if (selectedType == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select a Type.');", true);
                return;
            }

            if (selectedShiftType == "Flexible Shift" && selectedClockType == "Normal-In")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select either OT-In or Project-In for Flexible Shift.');", true);
                return;
            }

            //Check if worker yet check out
            int validCheckIn = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                                string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Actual_Time_In IS NOT NULL AND Actual_Time_Out IS NULL", Session["EmpID"]))?.ToString());

            if (validCheckIn > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid check in. Please check out the previous record first.');", true);
                return;
            }

            //Check if there is any existing check in time later than selected
            int anyLaterTime = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                                string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{1}' AND CONVERT(nvarchar, Date_In, 106) = CONVERT(nvarchar, GETDATE(), 106) AND Actual_Time_In >= CONVERT(datetime, '{0}')", inputTime, Session["EmpID"]))?.ToString());

            if (anyLaterTime > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid check in. Check in time cannot be earlier than previous check in time.');", true);
                return;
            }

            int isPH = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
            string.Format(@"SELECT COUNT(1) FROM Public_Holiday WHERE CONVERT(nvarchar, PH_Day, 106) = CONVERT(nvarchar, GETDATE(), 106)", today))?.ToString());
            string satWorkingTime = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT DATEADD(Minute, ISNULL(Grace_Period,0), Sat_From_Time) FROM Working_Hour WHERE CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106)) BETWEEN CONVERT(datetime, CONVERT(nvarchar, From_Date, 106)) AND CONVERT(datetime, CONVERT(nvarchar, ISNULL(To_Date, GETDATE()), 106))", today)).ToString();
            string wdWorkingTime = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT DATEADD(Minute, ISNULL(Grace_Period,0), From_Time) FROM Working_Hour WHERE CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106)) BETWEEN CONVERT(datetime, CONVERT(nvarchar, From_Date, 106)) AND CONVERT(datetime, CONVERT(nvarchar, ISNULL(To_Date, GETDATE()), 106))", today)).ToString();
            string wdWorkingTimeTo = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT To_Time FROM Working_Hour WHERE CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106)) BETWEEN CONVERT(datetime, CONVERT(nvarchar, From_Date, 106)) AND CONVERT(datetime, CONVERT(nvarchar, ISNULL(To_Date, GETDATE()), 106))", today)).ToString();
            string satWorkingTimeTo = con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT Sat_To_Time FROM Working_Hour WHERE CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106)) BETWEEN CONVERT(datetime, CONVERT(nvarchar, From_Date, 106)) AND CONVERT(datetime, CONVERT(nvarchar, ISNULL(To_Date, GETDATE()), 106))", today)).ToString();
            int gracePeriod = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
            string.Format(@"SELECT Grace_Period FROM Working_Hour WHERE CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 106)) BETWEEN CONVERT(datetime, CONVERT(nvarchar, From_Date, 106)) AND CONVERT(datetime, CONVERT(nvarchar, ISNULL(To_Date, GETDATE()), 106))", today)).ToString());

            DateTime workingTime = DateTime.ParseExact(DateTime.Parse(wdWorkingTime).ToString("HH:mm"), "H:mm", null, System.Globalization.DateTimeStyles.None);
            DateTime workingTimeTo = DateTime.ParseExact(DateTime.Parse(wdWorkingTimeTo).ToString("HH:mm"), "H:mm", null, System.Globalization.DateTimeStyles.None);
            DateTime inputTimeD = DateTime.ParseExact(inputTime, "H:mm", null, System.Globalization.DateTimeStyles.None);

            //OT-In cannot be falls between working hour for normal shift for Mon to Fri
            if (selectedShiftType == "Normal Shift" && selectedClockType == "OT-In" && inputTimeD >= workingTime && inputTimeD <= workingTimeTo && !(d == 0 || d == 6) && isPH == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select either Flexible Shift / Special Normal if OT-In falls between working hour.');", true);
                return;
            }

            //OT-In cannot be falls between working hour for normal shift for Sat
            if (d == 6 && !string.IsNullOrEmpty(satWorkingTime))
            {
                workingTime = DateTime.ParseExact(DateTime.Parse(satWorkingTime).ToString("HH:mm"), "H:mm", null, System.Globalization.DateTimeStyles.None);
                workingTimeTo = DateTime.ParseExact(DateTime.Parse(satWorkingTimeTo).ToString("HH:mm"), "H:mm", null, System.Globalization.DateTimeStyles.None);

                if (selectedShiftType == "Normal Shift" && selectedClockType == "OT-In" && inputTimeD >= workingTime && inputTimeD <= workingTimeTo && isPH == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select either Flexible Shift / Special Normal if OT-In falls between working hour.');", true);
                    return;
                }
            }

            //Check if Normal-In, not allow worker to reverse the time if over grace period
            if (selectedClockType == "Normal-In")
            {
                if (isPH > 0 || d == 0 || (d ==6 && string.IsNullOrEmpty(satWorkingTime)))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select OT-In / Project-In instead of Normal-In for weekend / PH.');", true);
                    return;
                }

                if (today > workingTime)
                {
                    if(inputTimeD < workingTime)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter the exact check in time');", true);
                        return;
                    }

                    //isLate = "Late In";
                }
            }

            //Cannot over grace period
            if (selectedClockType == "OT-In" || selectedClockType == "Project-In" || selectedClockType == "Normal-In")
            {
                TimeSpan span = today.Subtract(inputTimeD);
                TimeSpan StartTime = TimeSpan.FromHours(0);
                StartTime = StartTime.Add(new TimeSpan(00, gracePeriod, 0));
                if (span > StartTime)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter the exact check in time');", true);
                    return;
                }
            }

            //Each day can has only 1 OT-In and 1 Normal-In for each worker
            if (selectedClockType == "OT-In" || selectedClockType == "Normal-In")
            {
                int noOfOTNormalIn = Convert.ToInt32(con.ExecuteSQLQueryWithOneReturn(
                    string.Format(@"SELECT COUNT(1) FROM Worker_Timesheet WHERE Employee_Profile_ID = '{0}' AND Clock_Type_In = '{1}' AND CONVERT(nvarchar, Date_In, 106) = CONVERT(nvarchar, GETDATE(), 106)", Session["EmpID"], selectedClockType))?.ToString());

                if (noOfOTNormalIn > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Maximum of 1 OT-In / Normal-In is allowed for each day only');", true);
                    return;
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
               string uniqueName = employeeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_IN" + fileExtension;
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
           SELECT Time_In, Time_Out, Clock_Type_Out
           FROM Worker_Timesheet
           WHERE Date_In = @TodayDate AND Time_IN IS NOT NULL AND Time_Out IS NOT NULL AND Employee_Profile_ID= @Employee_Profile_ID ORDER BY Date_Out DESC, Time_Out DESC";

           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               conn.Open();

               using (SqlCommand cmdCheck = new SqlCommand(overlapQuery, conn))
               {
                   cmdCheck.Parameters.AddWithValue("@TodayDate", today.ToString("yyyy-MM-dd"));
                   cmdCheck.Parameters.AddWithValue("@Employee_Profile_ID", Session["EmpID"]);

                   using (SqlDataReader reader = cmdCheck.ExecuteReader())
                   {
                       TimeSpan newTimeIn;
                       if (!TimeSpan.TryParse(txtTimeIn.Text, out newTimeIn))
                       {
                           ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid time format.');", true);
                           return;
                       }

                       while (reader.Read())
                       {
                           TimeSpan existingTimeIn = Convert.ToDateTime(reader["Time_In"]).TimeOfDay;
                           TimeSpan existingTimeOut = Convert.ToDateTime(reader["Time_Out"]).TimeOfDay;

                            if (reader["Clock_Type_Out"].ToString() == "Project-Out" && selectedClockType == "Project-In")
                            {
                                if (newTimeIn >= existingTimeIn && newTimeIn < existingTimeOut)
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Time overlaps with an existing entry.');", true);
                                    return;
                                }
                            }
                           else
                            {
                                if (newTimeIn >= existingTimeIn && newTimeIn <= existingTimeOut)
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Time overlaps with an existing entry.');", true);
                                    return;
                                }
                            }
                       }
                   }
               }
           }

           string timeCheck = con.ExecuteSQLQueryWithOneReturn(@"SELECT COUNT(1)
           FROM Worker_Timesheet
           WHERE ACTUAL_Time_IN IS NOT NULL AND ACTUAL_TIME_OUT IS NULL AND Employee_Profile_ID= '" + Session["EmpID"] + @"'").ToString() ?? "0";
           if (timeCheck != "0")
           {
               ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('The user has already checked in. Kindly return to the main page and proceed with the check-out process.');", true);
               return;
           }
            if (ddlProjectValue == "0")
            {
                ddlProjectValue = "NULL";
            }
            else
            {
                ddlProjectValue = "'"+ ddlProjectValue + "'";
            }

            string insertQuery = @"
           INSERT INTO Worker_Timesheet				
           (				
               Creation_Time,			
               Employee_Profile_ID,			
               Office,			
               Project_ID,			
               Shift_Type,			
               Clock_Type_In,			
               Date_In,			
               Actual_Time_In,
               System_Time_In,
               Image_In,			
               Remarks,
               Type,
               Manual_In_Update,
               Manual_Out_Update,
               CheckIn_Latitude,
               CheckIn_Longitude,
               Destination_Distance_CheckIn
           )				
           VALUES				
           (				
               GETDATE(),			
               '" + Session["EmpID"] + @"',
               '" + rbOfficeStr + @"',
               " + ddlProjectValue + @",
               '" + selectedShiftType + @"',	
               '" + selectedClockType + @"',
               '" + today.ToString("yyyy-MM-dd") + @"',	
               '" + txtTimeIn.Text + @"',
               GETDATE(),
               '" + imagePath + @"',
               '" + txtRemark.Text + @"',
               '" + selectedType + @"',
                'N',
                'N',
               '" + Latitude.Text + @"',
               '" + Longitude.Text + @"',
               '" + Distance.Text + @"'
           )			
           SELECT SCOPE_IDENTITY();		

       ";
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               conn.Open();

               int timesheetID = 0;
               using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
               {
                   cmdInsert.Parameters.AddWithValue("@isLate", isLate);

                    try
                    {
                        timesheetID = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }
                    catch
                    {
                        throw new Exception(insertQuery);
                    }
               }

               //Call stored procedure
               using (SqlCommand collectionCmd = new SqlCommand("Update_WorkerTimesheet", conn))
               {
                   collectionCmd.CommandType = CommandType.StoredProcedure;
                   collectionCmd.Parameters.AddWithValue("@timesheetID", timesheetID);
                   collectionCmd.ExecuteNonQuery();
               }

               //Success message
               //ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Timesheet submitted successfully!');", true);
               string targetUrl = "~/Modules/Menu.aspx";
               Response.Redirect(targetUrl);
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