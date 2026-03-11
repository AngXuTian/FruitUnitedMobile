using System;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Connection = DBConnection.Connection;

namespace FruitUnitedMobile.Modules
{
    public partial class Login : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDriver1Dropdown();
                BindDriver2Dropdown();
                BindVehicleDropdown();
                ClearSessionAndCache();
            }
        }

        private void ClearSessionAndCache()
        {
            Session.Clear();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            FormsAuthentication.SignOut();
        }

        private void BindDriver1Dropdown()
        {
            Driver1DDL.Items.Clear();
            Driver1DDL.Items.Add(new ListItem("-- Select Primary Driver --", "0"));

            string query = @"
                SELECT Employee_Profile_ID, Employee_Name, Employee_Code 
                FROM employee_profile
                WHERE (driver IN (1) AND Status = 'Active' AND Mobile_Username IS NOT NULL AND Password_For_Mobile IS NOT NULL)
                   OR (mobile_admin IN (1) AND Status = 'Active')
                ORDER BY Employee_Name";

            // Same connection logic as before...
            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string code = reader["Employee_Code"].ToString();
                        string name = reader["Employee_Name"].ToString();
                        Driver1DDL.Items.Add(new ListItem($"{name} - {code}", code));
                    }
                }
            }
        }

        private void BindDriver2Dropdown()
        {
            Driver2DDL.Items.Clear();
            Driver2DDL.Items.Add(new ListItem("-- No Secondary Driver --", "")); // Empty value = optional

            string query = @"
                SELECT employee_profile_ID, employee_name, employee_code 
                FROM employee_profile 
                WHERE Designation = 'Driver' AND status = 'Active'
                ORDER BY employee_name";

            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string code = reader["employee_code"].ToString();
                        string name = reader["employee_name"].ToString();
                        Driver2DDL.Items.Add(new ListItem($"{name} - {code}", code));
                    }
                }
            }
        }

        private void BindVehicleDropdown()
        {
            // unchanged - same as original
            VehicleDDL.Items.Clear();
            VehicleDDL.Items.Add(new ListItem("-- Select Vehicle --", "0"));

            string query = "SELECT Vehicle_Profile_ID, Vehicle_No FROM vehicle_profile WHERE Status = 'Active' ORDER BY Vehicle_No";

            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        VehicleDDL.Items.Add(new ListItem(reader["Vehicle_No"].ToString(), reader["Vehicle_Profile_ID"].ToString()));
                }
            }
        }

        protected void LoginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string primaryCode = Driver1DDL.SelectedValue?.Trim() ?? "";
                string password = Password.Value?.Trim() ?? "";
                string secondaryCode = Driver2DDL.SelectedValue?.Trim() ?? "";
                string vehicleId = VehicleDDL.SelectedValue;

                // === Validations === (unchanged)
                if (string.IsNullOrEmpty(primaryCode) || primaryCode == "0")
                {
                    Toast1.ShowError("Please select Primary Driver.");
                    //ErrorLabel.Text = "Please select Primary Driver.";
                    return;
                }
                if (string.IsNullOrEmpty(password))
                {
                    Toast1.ShowError("Password is required.");
                    //ErrorLabel.Text = "Password is required.";
                    return;
                }
                if (string.IsNullOrEmpty(vehicleId) || vehicleId == "0")
                {
                    Toast1.ShowError("Please select a vehicle.");
                    //ErrorLabel.Text = "Please select a vehicle.";
                    return;
                }

                // === Authenticate Primary Driver only === (unchanged)
                string valid = con.ExecuteSQLQueryWithOneReturn($@"
            SELECT 1 FROM Employee_Profile
            WHERE employee_code = '{primaryCode}'
              AND Password_For_Mobile = '{password}'
              AND Status = 'Active'")?.ToString() ?? "0";

                if (valid != "1")
                {
                    Toast1.ShowError("Incorrect password for the primary driver.");
                    //ErrorLabel.Text = "Incorrect password for the primary driver.";
                    return;
                }

                // === Get Primary Driver details ===
                string empID = con.ExecuteSQLQueryWithOneReturn($"SELECT Employee_Profile_ID FROM Employee_Profile WHERE employee_code = '{primaryCode}'")?.ToString() ?? "0";
                string empName = con.ExecuteSQLQueryWithOneReturn($"SELECT Employee_Name FROM Employee_Profile WHERE employee_code = '{primaryCode}'")?.ToString() ?? "";
                string userID = con.ExecuteSQLQueryWithOneReturn($"SELECT Users_ID FROM Employee_Profile LEFT JOIN Users ON Users.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID WHERE employee_code = '{primaryCode}'")?.ToString() ?? "0";
                string pos1 = con.ExecuteSQLQueryWithOneReturn($"SELECT Pos_Profile_ID1 FROM Employee_Profile LEFT JOIN Users ON Users.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID WHERE employee_code = '{primaryCode}'")?.ToString() ?? "0";

                // === Get Secondary Driver details (if selected) ===
                string emp2ID = "";
                string emp2Name = "";
                string emp2Code = "";
                string pos2 = "0";

                if (!string.IsNullOrEmpty(secondaryCode) && secondaryCode != "0")
                {
                    emp2ID = con.ExecuteSQLQueryWithOneReturn($"SELECT Employee_Profile_ID FROM Employee_Profile WHERE employee_code = '{secondaryCode}'")?.ToString() ?? "";
                    emp2Name = con.ExecuteSQLQueryWithOneReturn($"SELECT Employee_Name FROM Employee_Profile WHERE employee_code = '{secondaryCode}'")?.ToString() ?? "";
                    emp2Code = secondaryCode;

                    pos2 = con.ExecuteSQLQueryWithOneReturn($"SELECT Pos_Profile_ID1 FROM Employee_Profile LEFT JOIN Users ON Users.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID WHERE employee_code = '{secondaryCode}'")?.ToString() ?? "0";
                }

                // ────────────────────────────────────────────────
                //     Determine final Pos_Profile_ID according to rules
                // ────────────────────────────────────────────────
                string finalPosProfileID;

                if (pos1 != "0" && !string.IsNullOrEmpty(pos1))
                {
                    finalPosProfileID = pos1;           // Rule 1 & 2: Primary has priority
                }
                else if (!string.IsNullOrEmpty(secondaryCode) && pos2 != "0" && !string.IsNullOrEmpty(pos2))
                {
                    finalPosProfileID = pos2;           // Rule 3: Primary doesn't have → use secondary
                }
                else
                {
                    finalPosProfileID = "0";            // Rule 4: neither has valid value
                }

                // === Store everything in Session ===
                Session["EmpCode"] = primaryCode;
                Session["EmpID"] = empID;
                Session["Employee_Name"] = empName;
                Session["UserID"] = userID;
                Session["Pos_Profile_ID"] = finalPosProfileID;    // ← the new decided value
                Session["Emp2Code"] = emp2Code;
                Session["Emp2ID"] = emp2ID;
                Session["Emp2Name"] = emp2Name;
                Session["Vehicle_Profile_ID"] = vehicleId;
                Session["Vehicle_No"] = VehicleDDL.SelectedItem.Text;

                // === Redirect ===
                FormsAuthentication.RedirectFromLoginPage(primaryCode, false);
                Response.Redirect($"~/Modules/Menu.aspx?EmployeeID={empID}", false);
            }
            catch (Exception ex)
            {
                Toast1.ShowError("Login failed. Please try again.");
                //ErrorLabel.Text = "Login failed. Please try again.";
                // Consider logging ex.Message / ex.StackTrace in production
            }
        }
    }
}