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
    public partial class Transport_Request : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProjectList();
                BindTransportList();
            }
            if (Request.Browser.IsMobileDevice)
            {
            }
        }

        private void BindProjectList()
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

                    //// Add default "Select Reason" option at the top
                    ddlProject.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        private void BindTransportList()
        {

            string query = "SELECT Transport_Mode_ID, Transport_Mode FROM Transport_Mode WHERE Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlTransport.DataSource = reader;
                    ddlTransport.DataTextField = "Transport_Mode"; // Text displayed in dropdown
                    ddlTransport.DataValueField = "Transport_Mode_ID"; // Value for each item
                    ddlTransport.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlTransport.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int ManpowerSeat = 0, MaterialSeat = 0;
            string dateTxt = txtDate.Text;
            string timeTxt = txtTime.Text;
            string goBackSelection = rblGoBack.SelectedValue;
            if (!string.IsNullOrEmpty(ManpowerSeatTB.Text))
            {
                ManpowerSeat = Convert.ToInt32(ManpowerSeatTB.Text);
            }
            else
            {
                ManpowerSeat = 0;
            }

            if (!string.IsNullOrEmpty(ManpowerSeatTB.Text))
            {
                MaterialSeat = Convert.ToInt32(MaterialSeatTB.Text);
            }
            else
            {
                MaterialSeat = 0;
            }

            int totalToText;
            if (!int.TryParse(hdnTotal.Value, out totalToText))
            {
                totalToText = 0; // Or handle error
            }


            string remarkToTxt = txtRemark.Text;
            string Project_ID = ddlProject.SelectedValue;
            string Transport_ID = ddlTransport.SelectedValue;
            string user = Session["EmpID"].ToString();
            int ProjectValidation = int.Parse(con.ExecuteSQLQueryWithOneReturn(@"
                SELECT COUNT(1) FROM Transport_Request 
                WHERE Request_Date = '" + dateTxt +
                @"' AND Project_ID = '" + Project_ID +
                @"' AND Go_Back = '" + goBackSelection +
                @"' AND Status IN ('Submitted','Taken')").ToString());

            if (ProjectValidation > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Submit Failed.This transport request has been create for today.');", true);
            }
            else
            {
                string insertQuery = string.Format(@"

        IF NOT EXISTS (SELECT * FROM Transport_Request WHERE Request_Date = '" + dateTxt + @"' AND Project_ID = '" + Project_ID + @"' AND Go_Back = '" + goBackSelection + @"' AND Status IN ('Submitted','Taken')))
        BEGIN
            INSERT INTO Transport_Request			
            (			
	            Creation_Time,		
	            Request_Date,		
	            Request_Time,		
	            Go_Back,		
	            Employee_Profile_ID,		
	            Project_ID,		
	            Manpower_Seat,		
	            Material_Seat,		
	            No_of_Seat,	
                Transport_Mode_ID,
	            Remarks,		
	            Status,		
	            Employee_Profile_ID1,		
	            Vehicle_Profile_ID,		
	            Is_Driver,
                Approval_Time
            )	
            (			
	            SELECT		
		            GETDATE(),	
		            '" + dateTxt + @"',	
		            '" + timeTxt + @"',	
		            '" + goBackSelection + @"',	
		            Employee_Profile.Employee_Profile_ID,	
		            '" + Project_ID + @"',	
		            '" + ManpowerSeat.ToString() + @"',	
		            '" + MaterialSeat.ToString() + @"',	
		            '" + totalToText + @"',
		            '" + Transport_ID + @"',	
		            '" + remarkToTxt + @"',	
		            CASE WHEN (driver.Vehicle_Profile_ID IS NOT NULL) THEN 'Taken' ELSE 'Submitted' END,	
		            CASE WHEN (driver.Vehicle_Profile_ID IS NOT NULL) THEN Employee_Profile.Employee_Profile_ID ELSE NULL END,	
		            driver.Vehicle_Profile_ID,	
		            CASE WHEN (driver.Vehicle_Profile_ID IS NOT NULL) THEN 'Y' ELSE 'N' END,
                    CASE WHEN (driver.Vehicle_Profile_ID IS NOT NULL) THEN GETDATE() ELSE NULL END
	            FROM Employee_Profile		
	            LEFT JOIN		
	            (		
		            SELECT	
			            ROW_NUMBER() OVER (PARTITION BY Employee_Profile_ID ORDER BY Vehicle_Profile_ID DESC) AS RowNumber,
			            Employee_Profile_ID,
			            Vehicle_Profile_ID
		            FROM Vehicle_Profile	
		            WHERE Status = 'Active'	
	            ) driver ON driver.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID AND driver.RowNumber = 1		
	            WHERE Employee_Profile.Employee_Profile_ID = '" + user + @"'		
            )			
        END
                                        ");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Submit Successfully');", true);
                        }
                    }
                }

                string targetUrl = "~/Modules/Menu.aspx";
                Response.Redirect(targetUrl);
            }
        }
    }
}