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
    public partial class WFH : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
            if (Request.Browser.IsMobileDevice)
            {
            }
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string loginEmpID = Session["EmpID"].ToString();
            DateTime enteredDate = DateTime.Parse(txtDateFrom.Text);
            string enteredRemarks = txtRemark.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd;
                object result;

                // Get total applied of the month
                cmd = new SqlCommand(@"
                SELECT COUNT(1) 
                FROM WFH 
                WHERE Status = 'Submitted' 
                AND MONTH(WFH_Date) = @Month 
                AND YEAR(WFH_Date) = @Year 
                AND Employee_Profile_ID = @EmpID", conn);
                cmd.Parameters.AddWithValue("@Month", enteredDate.Month);
                cmd.Parameters.AddWithValue("@Year", enteredDate.Year);
                cmd.Parameters.AddWithValue("@EmpID", loginEmpID);
                int totalApplied = (int)cmd.ExecuteScalar();

                //  Get available days
                cmd = new SqlCommand(@"
                SELECT No_of_Day 
                FROM Employee_WFH 
                WHERE Employee_Profile_ID = @EmpID 
                AND @EnteredDate BETWEEN Start_Date AND End_Date", conn);
                cmd.Parameters.AddWithValue("@EmpID", loginEmpID);
                cmd.Parameters.AddWithValue("@EnteredDate", enteredDate);
                result = cmd.ExecuteScalar();
                int availableDays = result != null ? Convert.ToInt32(result) : 0;

                // Check individual limit
                if ((totalApplied + 1) > availableDays)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Total WFH for the month exceeded the available limit.');", true);
                    return;
                }

                // Get department ID
                cmd = new SqlCommand("SELECT Department_Profile_ID FROM Employee_Profile WHERE Employee_Profile_ID = @EmpID", conn);
                cmd.Parameters.AddWithValue("@EmpID", loginEmpID);
                object deptIDResult = cmd.ExecuteScalar();
                if (deptIDResult == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Department Not Found.');", true);
                    return;
                }
                string deptID = deptIDResult.ToString();

                // Get WFH percentage limit for department
                cmd = new SqlCommand(@"
                SELECT Percentage 
                FROM Department_WFH 
                WHERE @EnteredDate BETWEEN Start_Date AND End_Date 
                AND Department_Profile_ID = @DeptID", conn);
                cmd.Parameters.AddWithValue("@EnteredDate", enteredDate);
                cmd.Parameters.AddWithValue("@DeptID", deptID);
                result = cmd.ExecuteScalar();
                double allowedPercentage = result != null ? Convert.ToDouble(result) : 0;

                // Get total active employees in department
                cmd = new SqlCommand("SELECT COUNT(1) FROM Employee_Profile WHERE Department_Profile_ID = @DeptID AND Status = 'Active'", conn);
                cmd.Parameters.AddWithValue("@DeptID", deptID);
                int totalEmployees = (int)cmd.ExecuteScalar();

                // Get how many already applied WFH on that day
                cmd = new SqlCommand("SELECT COUNT(1) FROM WFH WHERE Status = 'Submitted' AND WFH_Date = @EnteredDate AND Department_Profile_ID = @DeptID", conn);
                cmd.Parameters.AddWithValue("@EnteredDate", enteredDate);
                cmd.Parameters.AddWithValue("@DeptID", deptID);
                int deptWFHApplied = (int)cmd.ExecuteScalar();

                // Validation 2: Check percentage limit
                double actualPercentage = ((deptWFHApplied + 1) / (double)totalEmployees) * 100.0;
                if (actualPercentage > allowedPercentage)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Total WFH for the month exceeded the available limit.');", true);
                    return;
                }

                // Passed All Validation: Insert WFH Record
                cmd = new SqlCommand(@"
                INSERT INTO WFH (Creation_Time, Status, Employee_Profile_ID, Department_Profile_ID, WFH_Date, Remarks)
                SELECT GETDATE(), 'Submitted', Employee_Profile_ID, Department_Profile_ID, @WFHDate, @Remarks
                FROM Employee_Profile
                WHERE Employee_Profile_ID = @EmpID", conn);
                cmd.Parameters.AddWithValue("@WFHDate", enteredDate);
                cmd.Parameters.AddWithValue("@EmpID", loginEmpID);
                cmd.Parameters.AddWithValue("@Remarks", enteredRemarks);
                int rowsInserted = cmd.ExecuteNonQuery();

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Submission successful.');", true);
            }
        }



    }
}