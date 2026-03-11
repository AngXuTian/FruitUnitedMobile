using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;

namespace FruitUnitedMobile.Modules
{
    public partial class Delivery_Plan : BasePage
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear previous session data on fresh load if needed
                ClearDeliveryPlanSessions();

                if (Session["EmpID"] == null || string.IsNullOrEmpty(Session["EmpID"].ToString()))
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                txtDeliveryDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadPlans();
            }
        }

        private void LoadPlans()
        {
            DateTime selectedDate;

            if (!DateTime.TryParse(txtDeliveryDate.Text, out selectedDate))
            {
                // If no date yet, just load nothing
                ddlPlanName.Items.Clear();
                ddlPlanName.Items.Add(new ListItem("-- Select Plan --", ""));
                return;
            }

            string query = @"
        SELECT DISTINCT 
            planning.Delivery_Planning_ID,
            planning.Plan_Name,
            planning.Date_From,
            planning.Date_To,
            Vehicle_Profile.Vehicle_No
        FROM Delivery_Planning planning
        LEFT JOIN Vehicle_Profile ON Vehicle_Profile.Vehicle_Profile_ID = planning.Vehicle_Profile_ID
        WHERE planning.Status = 'Active'
            AND planning.Date_From <= @SelectedDate
            AND (planning.Date_To IS NULL OR planning.Date_To >= @SelectedDate)
        ORDER BY planning.Plan_Name";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SelectedDate", selectedDate.Date);

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlPlanName.Items.Clear();
                        ddlPlanName.Items.Add(new ListItem("-- Select Plan --", ""));

                        while (reader.Read())
                        {
                            string planName = reader["Plan_Name"].ToString();
                            //DateTime dateFrom = Convert.ToDateTime(reader["Date_From"]);

                            //string dateRange = dateFrom.ToString("dd MMM yyyy");

                            //if (reader["Date_To"] != DBNull.Value)
                            //{
                            //    dateRange += " - " + Convert.ToDateTime(reader["Date_To"]).ToString("dd MMM yyyy");
                            //}
                            //else
                            //{
                            //    dateRange += " - Ongoing";
                            //}

                            string text = $"{planName}";

                            ddlPlanName.Items.Add(new ListItem(
                                text,
                                reader["Delivery_Planning_ID"].ToString()
                            ));
                        }
                    }
                }
            }
        }

        protected void txtDeliveryDate_TextChanged(object sender, EventArgs e)
        {
            LoadPlans();
        }


        protected void ddlPlanName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlPlanName.SelectedValue))
            {
                LoadPlanInfo();
            }
        }

        private void LoadPlanInfo()
        {
            string query = @"
                SELECT planning.Plan_Name, planning.Date_From, planning.Date_To,
                       Vehicle_Profile.Vehicle_No, COUNT(DISTINCT delDay.Delivery_Day_ID) as DayCount
                FROM Delivery_Planning planning
                LEFT JOIN Vehicle_Profile ON Vehicle_Profile.Vehicle_Profile_ID = planning.Vehicle_Profile_ID
                LEFT JOIN Delivery_Day delDay ON delDay.Delivery_Planning_ID = planning.Delivery_Planning_ID
                WHERE planning.Delivery_Planning_ID = @PlanID
                GROUP BY planning.Plan_Name, planning.Date_From, planning.Date_To, Vehicle_Profile.Vehicle_No";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PlanID", ddlPlanName.SelectedValue);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string info = $"<strong>Plan:</strong> {reader["Plan_Name"]}<br/>";
                            info += $"<strong>Period:</strong> {Convert.ToDateTime(reader["Date_From"]):dd MMM yyyy}";
                            if (reader["Date_To"] != DBNull.Value)
                            {
                                info += $" - {Convert.ToDateTime(reader["Date_To"]):dd MMM yyyy}";
                            }
                            else
                            {
                                info += " - Ongoing";
                            }
                            if (reader["Vehicle_No"] != DBNull.Value)
                            {
                                info += $"<br/><strong>Vehicle:</strong> {reader["Vehicle_No"]}";
                            }
                            info += $"<br/><strong>Delivery Days:</strong> {reader["DayCount"]} days configured";
                        }
                    }
                }
            }
        }

        protected void btnLoadPlan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlPlanName.SelectedValue))
            {
                Toast1.ShowWarning("Please select a plan.");
                return;
            }

            if (string.IsNullOrEmpty(txtDeliveryDate.Text))
            {
                Toast1.ShowWarning("Please select a delivery date.");
                return;
            }

            DateTime selectedDate = DateTime.Parse(txtDeliveryDate.Text);
            hfSelectedPlanID.Value = ddlPlanName.SelectedValue;

            Session["DeliveryDate"] = txtDeliveryDate.Text;
            Session["SelectedDate"] = selectedDate;
            Session["PlanID"] = ddlPlanName.SelectedValue;
            Session["PlanName"] = ddlPlanName.SelectedItem.Text;

            LoadOutlets(selectedDate);

            lblSelectedDate.Text = selectedDate.ToString("dd MMM yyyy (dddd)");
            lblSelectedPlan.Text = ddlPlanName.SelectedItem.Text;

            ClientScript.RegisterStartupScript(this.GetType(), "showStep2", "showStep(2);", true);
        }

        private void LoadOutlets(DateTime deliveryDate)
        {
            string query = @"
                SELECT
                    planning.Plan_Name,
                    Vehicle_Profile.Vehicle_No,
                    DATEADD(day, z.allDate, planning.Date_From) AS Delivery_Date,
                    delDay.Delivery_Day,
                    delDay.Delivery_Day_ID,
                    Outlet_Profile.Outlet_Name,
                    Outlet_Profile.Outlet_Number,
                    Outlet_Profile.Postcode,
                    CONVERT(nvarchar(max), Outlet_Profile.Address) As [Address],
                    outlet.Operating_Hour,
                    outlet.Primary_Contact,
                    outlet.Secondary_Contact,
                    CONVERT(nvarchar(max), outlet.Remarks) As [Remarks],
                    Outlet_Profile.Outlet_Profile_ID,
                    outlet.Delivery_Outlet_ID
                FROM Delivery_Product product
                INNER JOIN Delivery_Outlet outlet ON outlet.Delivery_Outlet_ID = product.Delivery_Outlet_ID
                INNER JOIN Delivery_Day delDay ON delDay.Delivery_Day_ID = outlet.Delivery_Day_ID
                INNER JOIN Delivery_Planning planning ON planning.Delivery_Planning_ID = delDay.Delivery_Planning_ID
                INNER JOIN Product_Profile ON Product_Profile.Product_Profile_ID = product.Product_Profile_ID
                INNER JOIN Outlet_Profile ON Outlet_Profile.Outlet_Profile_ID = outlet.Outlet_Profile_ID
                LEFT JOIN Vehicle_Profile ON Vehicle_Profile.Vehicle_Profile_ID = planning.Vehicle_Profile_ID
                CROSS JOIN
                    (SELECT b12.i + b11.i + b10.i + b9.i + b8.i + b7.i + b6.i + b5.i + b4.i + b3.i + b2.i + b1.i + b0.i allDate
                        FROM 
                        (SELECT 0 i UNION ALL SELECT 1) b0
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 2) b1
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 4) b2
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 8) b3
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 16) b4
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 32) b5
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 64) b6
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 128) b7
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 256) b8
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 512) b9
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 1024) b10
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 2048) b11
                        CROSS JOIN (SELECT 0 i UNION ALL SELECT 4096) b12
                    ) z
                WHERE z.allDate <= DATEDIFF(day, planning.Date_From, ISNULL(planning.Date_To, DATEADD(day, 7, planning.Date_From)))
                    AND DATEPART(WEEKDAY, DATEADD(day, z.allDate, planning.Date_From)) = delDay.Day_In_Number
                    AND DATEADD(day, z.allDate, planning.Date_From) = @DeliveryDate
                    AND planning.Delivery_Planning_ID = @PlanID
                    AND planning.Status = 'Active'
                GROUP BY planning.Plan_Name, Vehicle_Profile.Vehicle_No, DATEADD(day, z.allDate, planning.Date_From),
                    delDay.Delivery_Day, delDay.Delivery_Day_ID, Outlet_Profile.Outlet_Name, Outlet_Profile.Outlet_Number,
                    Outlet_Profile.Postcode, CONVERT(nvarchar(max), Outlet_Profile.Address), outlet.Operating_Hour,
                    outlet.Primary_Contact, outlet.Secondary_Contact, CONVERT(nvarchar(max), outlet.Remarks),
                    Outlet_Profile.Outlet_Profile_ID, outlet.Delivery_Outlet_ID
                ORDER BY Outlet_Profile.Postcode, Outlet_Profile.Outlet_Number";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PlanID", hfSelectedPlanID.Value);
                    cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate.Date);

                    connection.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptOutlets.DataSource = dt;
                        rptOutlets.DataBind();
                        //pnlOutlets.Visible = true;
                        lblNoOutlets.Visible = false;

                        lblDeliveryDay.Text = dt.Rows[0]["Delivery_Day"].ToString();
                        hfSelectedDeliveryDayID.Value = dt.Rows[0]["Delivery_Day_ID"].ToString();
                        //Session["DeliveryDay"] = dt.Rows[0]["Delivery_Day"].ToString();
                        Session["DeliveryDayID"] = dt.Rows[0]["Delivery_Day_ID"].ToString();
                    }
                    else
                    {
                        //pnlOutlets.Visible = false;
                        lblNoOutlets.Visible = true;
                        lblNoOutlets.Text = "No outlets scheduled for delivery on this date.";
                    }
                }
            }
        }

        protected void rptOutlets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectOutlet")
            {
                string[] args = e.CommandArgument.ToString().Split('|');

                string selectedOutletId = args[0];
                string selectedDeliveryOutletId = args[1];

                Session["SelectedOutletID"] = selectedOutletId;
                Session["SelectedDeliveryOutletID"] = selectedDeliveryOutletId;

                // Get PlanID from Session
                int vehicleID = Convert.ToInt32(Session["Vehicle_Profile_ID"]);

                int dailyLoadingId = GetDailyLoadingId(vehicleID);

                Session["DailyLoadingID"] = dailyLoadingId;

                Response.Redirect("~/Modules/Delivery_Plan_Detail.aspx");
            }
        }

        protected void btnBackToStep1_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "showStep1", "showStep(1);", true);
        }

        protected void btnBackToHome_Click(object sender, EventArgs e)
        {
            string url = string.Format("~/Modules/Menu.aspx?EmployeeID={0}", Session["EmpID"].ToString());
            Response.Redirect(url);
        }

        private int GetDailyLoadingId(int vehicle_Profile_ID)
        {
            int dailyLoadingId = 0;
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

            //    string sql = @"
            //SELECT TOP 1 Daily_Loading_ID
            //FROM Daily_Loading
            //WHERE Delivery_Planning_ID = @PlanID
            //  AND Status = 'Loaded'";

            string sql = @"
            select DL.Daily_Loading_ID from Daily_Loading DL 
            LEFT JOIN Vehicle_Profile VP ON VP.Vehicle_Profile_ID = DL.Vehicle_Profile_ID 
            where DL.Vehicle_Profile_ID = @Vehicle_Profile_ID AND DL.Status = 'Loaded' ";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Vehicle_Profile_ID", vehicle_Profile_ID);
                con.Open();

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    dailyLoadingId = Convert.ToInt32(result);
                }
            }

            return dailyLoadingId;
        }


        private void ClearDeliveryPlanSessions()
        {
            Session.Remove("DeliveryDate");
            Session.Remove("SelectedDate");
            Session.Remove("PlanID");
            Session.Remove("PlanName");
            //Session.Remove("DeliveryDay");
            Session.Remove("DeliveryDayID");
            Session.Remove("SelectedOutletID");
            Session.Remove("SelectedDeliveryOutletID");
            Session.Remove("DeliveryProducts");
            Session.Remove("ReturnProducts");
        }
    }
}