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
using ForSessionValue;
using System.Windows.Controls;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using Button = System.Web.UI.WebControls.Button;
using Label = System.Web.UI.WebControls.Label;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FruitUnitedMobile.Modules
{
    public partial class Equipment_List : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        SessionValue sessionValue = new SessionValue();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTypeDropdownList();
                if (Session["EquipmentList_Type"] != null)
                {
                    ddlType.SelectedValue = CommonFunctions.CommonFunctions.Decrypt(Session["EquipmentList_Type"].ToString(), "asdhk876y23");
                    TypeListDDL_Change();
                }
                if (Session["EquipmentList_Equipment"] != null)
                {
                    ddlEquipment.SelectedValue = CommonFunctions.CommonFunctions.Decrypt(Session["EquipmentList_Equipment"].ToString(), "asdhk876y23");
                    EquipmentListDDL_Change();
                }
            }
        }

        protected void TypeListDropDown_Change(object sender, EventArgs e)
        {
            TypeListDDL_Change();
        }

        protected void TypeListDDL_Change()
        {
            BindEquipmentDropdownList();
            ClearFirstWorkerData();
            ClearSecondWorkerData();
            btnViewFile.Enabled = false;
            btnViewFile.BackColor = Color.White;
            btnViewImage.Enabled = false;
            btnViewImage.BackColor = Color.White;
        }

        protected void EquipmentListDropDown_Change(object sender, EventArgs e)
        {
            EquipmentListDDL_Change();
        }

        protected void EquipmentListDDL_Change()
        {
            BindLocatioStatusTB();
            BindWorkerInfo();
            ButtonVisibleValidation();
            if (ddlEquipment.SelectedValue == "0")
            {
                btnViewFile.Enabled = false;
                btnViewFile.BackColor = Color.White;
                btnViewImage.Enabled = false;
                btnViewImage.BackColor = Color.White;
            }
            else
            {
                btnViewFile.Enabled = true;
                btnViewFile.BackColor = Color.Green;
                btnViewImage.Enabled = true;
                btnViewImage.BackColor = Color.Green;
            }
        }

        private void BindEquipmentDropdownList()
        {
            string Type_Value = ddlType.SelectedValue;
            string query = string.Format(@"
                SELECT 
                    Equipment_Profile_ID, 
                    Equipment_Name + ' (' + CASE WHEN (Current_Status = 'Others') THEN 'OTH' ELSE LEFT(Current_Status,1) END + ')' AS Equipment_Name
                FROM Equipment_Profile 
                WHERE 
                Equipment_Type_ID = {0}
                AND Status = 'Active'
            ", Type_Value);
            ddlEquipment.Items.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlEquipment.DataSource = reader;
                    ddlEquipment.DataTextField = "Equipment_Name"; // Text displayed in dropdown
                    ddlEquipment.DataValueField = "Equipment_Profile_ID"; // Value for each item
                    ddlEquipment.Style.Add("background-color", "Black");
                    ddlEquipment.DataBind();
                    //// Add default "Select Reason" option at the top
                    ddlEquipment.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }

           
        }

        private void BindTypeDropdownList()
        {

            string query = "SELECT Equipment_Type_ID, Equipment_Type FROM Equipment_Type WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlType.DataSource = reader;
                    ddlType.DataTextField = "Equipment_Type";
                    ddlType.DataValueField = "Equipment_Type_ID";
                    ddlType.DataBind();
                    ddlType.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        private void BindLocatioStatusTB()
        {
            string EquipmentValue = ddlEquipment.SelectedValue;
            string query = string.Format(@"
                SELECT 
                    Current_Status AS Current_Location, 
                    Status 
                FROM Equipment_Profile 
                WHERE Equipment_Profile_ID = {0}								
            ", EquipmentValue);
            StatusTB.Text = "";
            CurrentLocationTB.Text = "";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        StatusTB.Text = reader["Status"].ToString();
                        CurrentLocationTB.Text = reader["Current_Location"].ToString();
                    }
                }
            }
        }

        private void BindWorkerInfo()
        {
            string EquipmentValue = ddlEquipment.SelectedValue;
            string query = string.Format(@"

                    SELECT COUNT(1) AS TotalRow
                    FROM Equipment_Movement movement
                    INNER JOIN Employee_Profile worker ON worker.Employee_Profile_ID = movement.Employee_Profile_ID
                    LEFT JOIN Project ON Project.Project_ID = movement.Project_ID
                    WHERE movement.Equipment_Profile_ID = {0}

                    SELECT TOP 2
                        worker.Display_Name AS Worker,	
	                    movement.Type AS Movement,	
	                    Project.Project_Name + ' / ' + Project.Project_No AS Project,	
	                    movement.Check_Out_Date,	
	                    movement.Check_Out_Location,	
	                    movement.Check_In_Date,	
	                    movement.Check_In_Location
                    FROM Equipment_Movement movement
                    INNER JOIN Employee_Profile worker ON worker.Employee_Profile_ID = movement.Employee_Profile_ID
                    LEFT JOIN Project ON Project.Project_ID = movement.Project_ID
                    WHERE movement.Equipment_Profile_ID = {0}
                    ORDER BY movement.Check_Out_Date DESC
            ", EquipmentValue);
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    int TotalRow = 0;
                    while (reader.Read())
                    {
                        TotalRow = int.Parse(reader["TotalRow"].ToString());
                    }
                    reader.NextResult();
                    if (TotalRow == 0)
                    {
                        ClearFirstWorkerData();
                        ClearSecondWorkerData();
                    }
                    else if (TotalRow == 1)
                    {
                        int SN = 1;
                        while (reader.Read())
                        {
                            if (SN == 1)
                            {
                                FirstWorkerName.Text = reader["Worker"].ToString();
                                FirstWorkerMovement.Text = reader["Movement"].ToString();
                                FirstWorkerProject.Text = reader["Project"].ToString();
                                FirstWorkerTimeOut.Text = reader["Check_Out_Date"].ToString();
                                FirstWorkerTimeOutLocation.Text = reader["Check_Out_Location"].ToString();
                                FirstWorkerTimeIn.Text = reader["Check_In_Date"].ToString();
                                FirstWorkerTimeInLocation.Text = reader["Check_In_Location"].ToString();
                                ClearSecondWorkerData();
                                SN++;
                            }
                        }
                    }
                    else
                    {
                        int SN = 1;
                        while (reader.Read())
                        {
                            if (SN == 1)
                            {
                                FirstWorkerName.Text = reader["Worker"].ToString();
                                FirstWorkerMovement.Text = reader["Movement"].ToString();
                                FirstWorkerProject.Text = reader["Project"].ToString();
                                FirstWorkerTimeOut.Text = reader["Check_Out_Date"].ToString();
                                FirstWorkerTimeOutLocation.Text = reader["Check_Out_Location"].ToString();
                                FirstWorkerTimeIn.Text = reader["Check_In_Date"].ToString();
                                FirstWorkerTimeInLocation.Text = reader["Check_In_Location"].ToString();

                            }
                            else
                            {
                                SecondWorkerName.Text = reader["Worker"].ToString();
                                SecondWorkerMovement.Text = reader["Movement"].ToString();
                                SecondWorkerProject.Text = reader["Project"].ToString();
                                SecondWorkerTimeOut.Text = reader["Check_Out_Date"].ToString();
                                SecondWorkerTimeOutLocation.Text = reader["Check_Out_Location"].ToString();
                                SecondWorkerTimeIn.Text = reader["Check_In_Date"].ToString();
                                SecondWorkerTimeInLocation.Text = reader["Check_In_Location"].ToString();
                            }
                            SN++;
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string ButtonName = clickedButton.Text;
            string EncryptedType = CommonFunctions.CommonFunctions.Encrypt(ddlType.SelectedValue, "asdhk876y23");
            Session["EquipmentList_Type"] = EncryptedType;
            string EncryptedEquipment = CommonFunctions.CommonFunctions.Encrypt(ddlEquipment.SelectedValue, "asdhk876y23");
            Session["EquipmentList_Equipment"] = EncryptedEquipment;


            if (ButtonName == "View Image")
            {
                string targetUrl = "~/Modules/Equipment-ViewFile.aspx?Type=Image&ID=" + ddlEquipment.SelectedValue;
                Response.Redirect(targetUrl);
            }
            else if (ButtonName == "View File")
            {
                string targetUrl = "~/Modules/Equipment-ViewFile.aspx?Type=File&ID=" + ddlEquipment.SelectedValue;
                Response.Redirect(targetUrl);
            }
        }

        protected void ClearFirstWorkerData()
        {
            FirstWorkerName.Text = "";
            FirstWorkerMovement.Text = "";
            FirstWorkerProject.Text = "";
            FirstWorkerTimeOut.Text = "";
            FirstWorkerTimeOutLocation.Text = "";
            FirstWorkerTimeIn.Text = "";
            FirstWorkerTimeInLocation.Text = "";
        }

        protected void ClearSecondWorkerData()
        {
            SecondWorkerName.Text = "";
            SecondWorkerMovement.Text = "";
            SecondWorkerProject.Text = "";
            SecondWorkerTimeOut.Text = "";
            SecondWorkerTimeOutLocation.Text = "";
            SecondWorkerTimeIn.Text = "";
            SecondWorkerTimeInLocation.Text = "";
        }

        protected void ButtonVisibleValidation()
        {
            string EquipmentValue = ddlEquipment.SelectedValue;
            if (EquipmentValue == "0")
            {
                btnViewFile.Visible = false;
                btnViewImage.Visible = false;
            }
            else
            {
                btnViewFile.Visible = true;
                btnViewImage.Visible = true;
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