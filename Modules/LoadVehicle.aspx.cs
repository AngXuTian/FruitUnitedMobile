using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace FruitUnitedMobile.Modules
{
    public partial class LoadVehicle : System.Web.UI.Page
    {
        private int vehicleProfileID = 0;
        private int driver1ID = 0;
        private int driver2ID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            RetrieveSessionIDs();
            if (!IsPostBack)
            {
                RefreshPageState();
            }
        }

        private void RefreshPageState()
        {
            if (vehicleProfileID == 0)
            {
                Toast1.ShowError("Vehicle session expired.");
                return;
            }

            string status = GetVehicleStatus(vehicleProfileID);

            if (status == "Loaded")
            {
                h4StatusHeader.InnerText = "Status: Vehicle Loaded";
                btnLoadVehicle.Visible = false;
                btnTriggerComplete.Visible = true;

                bool hasBalance = CheckHasBalance(vehicleProfileID);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setBalance", $"setHasBalance({hasBalance.ToString().ToLower()});", true);

                LoadItemsGrid(GetVehicleLoadedItemsData(vehicleProfileID));
            }
            else if (status == "Ready")
            {
                h4StatusHeader.InnerText = "Status: Ready for Loading";
                btnLoadVehicle.Visible = true;
                btnTriggerComplete.Visible = false;
                ClearGrid();
            }
            else
            {
                h4StatusHeader.InnerText = "Status: No Active Batch";
                btnLoadVehicle.Visible = false;
                btnTriggerComplete.Visible = false;
                ClearGrid();
            }
        }

        protected void btnLoadVehicle_Click(object sender, EventArgs e)
        {
            string result = ExecuteLoadVehicle(vehicleProfileID, driver1ID, driver2ID);
            if (result.StartsWith("Success")) Toast1.ShowSuccess(result);
            else Toast1.ShowError(result,null,10000);
            RefreshPageState();
        }

        protected void btnCompleteDelivery_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE Daily_Loading SET Status = 'Completed', Completion_Date = GETDATE() WHERE Vehicle_Profile_ID = @VehicleID AND Status = 'Loaded'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleProfileID);
                    if (cmd.ExecuteNonQuery() > 0) Toast1.ShowSuccess("Delivery completed successfully.");
                    else Toast1.ShowError("Error: No loaded batch found.");
                }
            }
            // After completion, refresh to clear list and hide buttons
            RefreshPageState();
        }

        private void ClearGrid()
        {
            gvLoadingItems.DataSource = null;
            gvLoadingItems.DataBind();
        }

        private bool CheckHasBalance(int vehicleID)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            string query = @"SELECT COUNT(1) FROM Daily_Loading_Items DLI 
                             INNER JOIN Daily_Loading DL ON DLI.Daily_Loading_ID = DL.Daily_Loading_ID
                             WHERE DL.Vehicle_Profile_ID = @VID AND DL.Status = 'Loaded' AND ISNULL(DLI.Balance,0) > 0";
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@VID", vehicleID);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private string GetVehicleStatus(int vehicleID)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT Status FROM Daily_Loading WHERE Vehicle_Profile_ID = @VID AND (Status='Ready' OR Status='Loaded')", con);
                cmd.Parameters.AddWithValue("@VID", vehicleID);
                con.Open();
                return cmd.ExecuteScalar()?.ToString() ?? "None";
            }
        }

        private void LoadItemsGrid(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) { ClearGrid(); return; }
            dt.Columns.Add("No.", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["No."] = i + 1;
            dt.Columns["No."].SetOrdinal(0);
            gvLoadingItems.DataSource = dt;
            gvLoadingItems.DataBind();
        }

        private DataTable GetVehicleLoadedItemsData(int vehicleID)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;
            string sql = @"SELECT PP.Abbreviation, PP.Product_Name, UOM.UOM, SUM(DLI.Balance) AS Balance 
                           FROM Daily_Loading_Items DLI 
                           INNER JOIN Daily_Loading DL ON DLI.Daily_Loading_ID = DL.Daily_Loading_ID 
                           INNER JOIN Product_Profile PP ON PP.Product_Profile_ID = DLI.product_PROFILE_ID 
                           INNER JOIN UOM_Profile UOM ON UOM.UOM_Profile_ID = PP.UOM_Profile_ID 
                           WHERE DL.Vehicle_Profile_ID = @VID AND DL.Status = 'Loaded' 
                           GROUP BY PP.Abbreviation, PP.Product_Name, UOM.UOM ORDER BY PP.Product_Name ASC";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, connStr))
            {
                da.SelectCommand.Parameters.AddWithValue("@VID", vehicleID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public string ExecuteLoadVehicle(int vehicleProfileID, int driver1ID, int driver2ID)
        {
            string connStr = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

            if (Session["Pos_Profile_ID"] == null ||
            !int.TryParse(Session["Pos_Profile_ID"].ToString(), out int posProfileId) ||
            posProfileId == 0)
            {

                // Optional: stop further processing
                return "Invalid POS Profile.\n\n" +
                    "Please do of the following:\n" +
                    "• Ask your admin to assign POS device in your Employee Profile";
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Find the 'Ready' loading batch for this vehicle
                        SqlCommand cmdReady = new SqlCommand(
                            "SELECT Daily_Loading_ID FROM Daily_Loading " +
                            "WHERE Vehicle_Profile_ID = @VID AND Status = 'Ready'",
                            con, trans);

                        cmdReady.Parameters.AddWithValue("@VID", vehicleProfileID);
                        object readyID = cmdReady.ExecuteScalar();

                        if (readyID == null)
                        {
                            trans.Rollback();
                            return "Error: No ready batch found for this vehicle.";
                        }

                        int dailyLoadingId = Convert.ToInt32(readyID);

                        // 2. Update the record - now also setting pos_profile_ID
                        SqlCommand cmdUpd = new SqlCommand(
                            @"UPDATE Daily_Loading 
                      SET Status = 'Loaded', 
                          Loaded_Date = GETDATE(), 
                          Employee_Profile_ID = @D1, 
                          pos_profile_ID = @PosProfileID
                      WHERE Daily_Loading_ID = @ID",
                            con, trans);

                        cmdUpd.Parameters.AddWithValue("@ID", dailyLoadingId);
                        cmdUpd.Parameters.AddWithValue("@D1", driver1ID);
                        cmdUpd.Parameters.AddWithValue("@PosProfileID", posProfileId);

                        int rowsAffected = cmdUpd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            trans.Rollback();
                            return "Error: Update failed - record may have been modified.";
                        }

                        trans.Commit();
                        return "Success: Vehicle loaded.";
                    }
                    catch (Exception ex)
                    {
                        try { trans.Rollback(); } catch { }
                        return "Error: " + ex.Message;
                    }
                }
            }
        }

        private void RetrieveSessionIDs()
        {
            if (Session["Vehicle_Profile_ID"] != null) int.TryParse(Session["Vehicle_Profile_ID"].ToString(), out vehicleProfileID);
            if (Session["EmpID"] != null) int.TryParse(Session["EmpID"].ToString(), out driver1ID);
            if (Session["Emp2ID"] != null) int.TryParse(Session["Emp2ID"].ToString(), out driver2ID);
        }
    }
}