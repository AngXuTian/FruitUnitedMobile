using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.Web.UI.HtmlControls;
using ForSessionValue;
using System.Web.Security;
using System.Data.SqlClient;

namespace FruitUnitedMobile.Modules
{
    public partial class Equipment_Maintenance : System.Web.UI.Page
    {
        Connection con = new Connection();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWorkerTimeSheetHistoryData("");
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string EquipmentName = EquipmentNameTextBox.Text.Trim();
            BindWorkerTimeSheetHistoryData(EquipmentName);
        }


        protected void BindWorkerTimeSheetHistoryData(string EquipmentName)
        {
            string filterCondition = "";

            string sql = string.Format(@"
                        	            SELECT								
	                                        Equipment_Name + ' (' + CASE WHEN (Current_Status = 'Others') THEN 'OTH' ELSE LEFT(Current_Status,1) END + ')' AS Equipment_Name,
                                            REPLACE(convert(varchar, Calibration_Expiry_Date, 106),' ','-') AS Expiry_Date,			
	                                        DATEDIFF(Day, GETDATE(), Calibration_Expiry_Date) AS Remaining_Days							
                                        FROM Equipment_Profile								
                                        WHERE Status = 'Active'								
		                                AND DATEDIFF(Day, GETDATE(), Calibration_Expiry_Date) <= Equipment_Profile.Due_Alert					
                       ");

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(EquipmentName))
            {
                filterCondition += " AND Equipment_Name LIKE '%" + EquipmentName + "%'";
            }

            if (!string.IsNullOrEmpty(filterCondition))
            {
                sql = sql + filterCondition;
            }

            DataTable dt = con.FillDatatable(sql);
            FruitUnitedMobileGrid.DataSource = dt;
            FruitUnitedMobileGrid.DataBind();
        }



        protected void InvoiceGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "left");
            e.Row.Cells[2].Style.Add("text-align", "left");
            e.Row.Cells[0].Style.Add("width", "60%");
            e.Row.Cells[1].Style.Add("width", "25%");
            e.Row.Cells[2].Style.Add("width", "15%");
        }
    }
}