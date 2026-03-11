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
    public partial class Submitted_Scope : System.Web.UI.Page
    {
        Connection con = new Connection();
        string DONo = string.Empty;
        string Customer = string.Empty;
        string VehicleNo = string.Empty;
        string Status = string.Empty;
        string DODate = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtProject.Text = con.ExecuteSQLQueryWithOneReturn(
                   string.Format(@"														
		SELECT									
		Project.Project_Name										
	FROM Project										
	INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID										
	INNER JOIN Customer_Profile ON Customer_Profile.Customer_Profile_ID = Quotation.Customer_Profile_ID										
	INNER JOIN										
	(										
		SELECT									
			Project_ID,								
			COUNT(1) AS Scope								
		FROM Project_Scope									
		WHERE Mobile_Visibility = 'Y'									
		GROUP BY Project_ID									
	) scopeTable ON scopeTable.Project_ID = Project.Project_ID										
	WHERE Project.Project_ID = {0}											
	ORDER BY Project.Estimated_Start_Date ASC										
 													
 ", Request.QueryString["Project_ID"])
               )?.ToString() ?? "";
                BindFilters();
                BindDOData();
            }

        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string filterQuery = "";

            if (!string.IsNullOrEmpty(ddlFromBuilding.SelectedValue))
            {
                filterQuery += " AND Project_Scope.From_Building = @FromBuilding";
                parameters.Add(new SqlParameter("@FromBuilding", ddlFromBuilding.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlFromFloor.SelectedValue))
            {
                filterQuery += " AND Project_Scope.From_Floor = @FromFloor";
                parameters.Add(new SqlParameter("@FromFloor", ddlFromFloor.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlFromLocation.SelectedValue))
            {
                filterQuery += " AND Project_Scope.From_Location = @FromLocation";
                parameters.Add(new SqlParameter("@FromLocation", ddlFromLocation.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlToBuilding.SelectedValue))
            {
                filterQuery += " AND Project_Scope.To_Building = @ToBuilding";
                parameters.Add(new SqlParameter("@ToBuilding", ddlToBuilding.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlToFloor.SelectedValue))
            {
                filterQuery += " AND Project_Scope.To_Floor = @ToFloor";
                parameters.Add(new SqlParameter("@ToFloor", ddlToFloor.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlToLocation.SelectedValue))
            {
                filterQuery += " AND Project_Scope.To_Location = @ToLocation";
                parameters.Add(new SqlParameter("@ToLocation", ddlToLocation.SelectedValue));
            }

            string sql = string.Format(@"
        select Project_Scope.Project_Scope_ID,
    CASE 
        WHEN COALESCE(Project_Scope.Scope_Info, '') = '' 
        THEN '' 
        ELSE CONCAT('Scope Info: ', Project_Scope.Scope_Info) 
    END AS Scope_Info,
    
    COALESCE(Scope_of_Work.Scope_of_Work, '') AS Scope_of_Work,

    CASE 
        WHEN COALESCE(System_Profile.System_Type, '') = '' 
        THEN '' 
        ELSE CONCAT('System: ', System_Profile.System_Type) 
    END AS System_Type,

    CASE 
        WHEN COALESCE(Scope_Type.Type, '') = '' 
        THEN '' 
        ELSE CONCAT('Type: ', Scope_Type.Type) 
    END AS Type,

    CASE 
        WHEN COALESCE(Ladder_Profile.Height, '') = '' 
        THEN '' 
        ELSE CONCAT('Height: ', Ladder_Profile.Height) 
    END AS Height,

    CASE 
        WHEN COALESCE(Ladder_Profile.Ladder, '') = '' 
        THEN '' 
        ELSE CONCAT('Ladder: ', Ladder_Profile.Ladder) 
    END AS Ladder,

   CONCAT('From: ', 
        COALESCE(Project_Scope.From_Building, 'N/A') + ' -> ' + 
        COALESCE(Project_Scope.From_Floor, 'N/A') + ' -> ' + 
        COALESCE(Project_Scope.From_Location, 'N/A')) AS From_Location,

    CASE 
        WHEN (Project_Scope.To_Building IS NOT NULL OR Project_Scope.To_Floor IS NOT NULL OR Project_Scope.To_Location IS NOT NULL)
        THEN CONCAT('To: ', 
            COALESCE(Project_Scope.To_Building, 'N/A') + ' -> ' + 
            COALESCE(Project_Scope.To_Floor, 'N/A') + ' -> ' + 
            COALESCE(Project_Scope.To_Location, 'N/A'))
        ELSE '' 
    END AS To_Location,

    CASE 
        WHEN COALESCE(Project_Scope.Label_Convention, '') = '' 
        THEN '' 
        ELSE CONCAT('Label Convention: ', Project_Scope.Label_Convention) 
    END AS Label_Convention,

    Project_Scope.Qty,
    Project_Scope.Completion_Percent						
FROM Project_Scope										
INNER JOIN Scope_of_Work ON Scope_of_Work.Scope_of_Work_ID = Project_Scope.Scope_of_Work_ID										
LEFT JOIN System_Profile ON System_Profile.System_Profile_ID = Project_Scope.System_Profile_ID										
LEFT JOIN Scope_Type ON Scope_Type.Scope_Type_ID = Project_Scope.Scope_Type_ID										
LEFT JOIN Ladder_Profile ON Ladder_Profile.Ladder_Profile_ID = Project_Scope.Ladder_Profile_ID										
WHERE 	Project_Scope.Project_ID = @Project_ID										
	{0}								
	AND (SELECT COUNT(1) FROM Project_Task_Completion 									
	INNER JOIN Project_Scope_Task ON Project_Scope_Task.Project_Scope_Task_ID = Project_Task_Completion.Project_Scope_Task_ID									
	WHERE Project_Scope_Task.Project_Scope_ID = Project_Scope.Project_Scope_ID AND Project_Task_Completion.Verified_On IS NULL AND Project_Task_Completion.Employee_Profile_ID = @empID) > 0		
ORDER BY Project_Scope.From_Building ASC, Project_Scope.From_Floor ASC, Project_Scope.From_Location ASC, Project_Scope.To_Building ASC, Project_Scope.To_Floor ASC, Project_Scope.To_Location ASC										

    ", filterQuery);

            parameters.Add(new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]));
            parameters.Add(new SqlParameter("@empID", Session["EmpID"]));

            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }


        private void BindFilters()
        {
            // Bind From Building Dropdown
            ddlFromBuilding.DataSource = con.FillDatatable(
                "SELECT DISTINCT From_Building FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlFromBuilding.DataTextField = "From_Building";
            ddlFromBuilding.DataValueField = "From_Building";
            ddlFromBuilding.DataBind();
            ddlFromBuilding.Items.Insert(0, new ListItem("-- Select From Building --", ""));

            // Bind From Floor Dropdown
            ddlFromFloor.DataSource = con.FillDatatable(
                "SELECT DISTINCT From_Floor FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlFromFloor.DataTextField = "From_Floor";
            ddlFromFloor.DataValueField = "From_Floor";
            ddlFromFloor.DataBind();
            ddlFromFloor.Items.Insert(0, new ListItem("-- Select From Floor --", ""));

            // Bind From Location Dropdown
            ddlFromLocation.DataSource = con.FillDatatable(
                "SELECT DISTINCT From_Location FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlFromLocation.DataTextField = "From_Location";
            ddlFromLocation.DataValueField = "From_Location";
            ddlFromLocation.DataBind();
            ddlFromLocation.Items.Insert(0, new ListItem("-- Select From Location --", ""));

            // Bind To Building Dropdown
            ddlToBuilding.DataSource = con.FillDatatable(
                "SELECT DISTINCT To_Building FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlToBuilding.DataTextField = "To_Building";
            ddlToBuilding.DataValueField = "To_Building";
            ddlToBuilding.DataBind();
            ddlToBuilding.Items.Insert(0, new ListItem("-- Select To Building --", ""));

            // Bind To Floor Dropdown
            ddlToFloor.DataSource = con.FillDatatable(
                "SELECT DISTINCT To_Floor FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlToFloor.DataTextField = "To_Floor";
            ddlToFloor.DataValueField = "To_Floor";
            ddlToFloor.DataBind();
            ddlToFloor.Items.Insert(0, new ListItem("-- Select To Floor --", ""));

            // Bind To Location Dropdown
            ddlToLocation.DataSource = con.FillDatatable(
                "SELECT DISTINCT To_Location FROM Project_Scope WHERE Project_ID = @Project_ID",
                new SqlParameter[] { new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]) });
            ddlToLocation.DataTextField = "To_Location";
            ddlToLocation.DataValueField = "To_Location";
            ddlToLocation.DataBind();
            ddlToLocation.Items.Insert(0, new ListItem("-- Select To Location --", ""));

            // Bind System Dropdown
            ddlSystem.DataSource = con.FillDatatable(
                "SELECT DISTINCT System_Type FROM System_Profile");
            ddlSystem.DataTextField = "System_Type";
            ddlSystem.DataValueField = "System_Type";
            ddlSystem.DataBind();
            ddlSystem.Items.Insert(0, new ListItem("-- Select System --", ""));

            // Bind Type Dropdown
            ddlType.DataSource = con.FillDatatable(
                "SELECT DISTINCT Type FROM Scope_Type");
            ddlType.DataTextField = "Type";
            ddlType.DataValueField = "Type";
            ddlType.DataBind();
            ddlType.Items.Insert(0, new ListItem("-- Select Type --", ""));
        }




        protected void InvoiceGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SessionValue sessionValue = new SessionValue();
            InvoiceGrid.Columns[4].Visible = false;
            e.Row.Cells[0].Style.Add("text-align", "left");
            e.Row.Cells[1].Style.Add("text-align", "center");
            e.Row.Cells[2].Style.Add("text-align", "center");
            e.Row.Cells[3].Style.Add("text-align", "center");


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lb = (LinkButton)e.Row.Cells[3].FindControl("ViewInvoice");
                //HtmlAnchor anchorLink = (HtmlAnchor)e.Row.Cells[4].FindControl("iconLink");

                lb.PostBackUrl = "~/Modules/Submitted-Task.aspx?Project_ID=" + Request.QueryString["Project_ID"] + "&Project_Scope_ID=" + ((Label)e.Row.Cells[4].FindControl("Project_Scope_ID")).Text;
                //anchorLink.HRef = "~/Modules/InvoiceView.aspx?Invoice_ID=" + ((Label)e.Row.Cells[5].FindControl("Invoice_ID")).Text;
            }
        }


        protected void BindDOData()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql = string.Format(@"
       	select Project_Scope.Project_Scope_ID,
    CASE 
        WHEN COALESCE(Project_Scope.Scope_Info, '') = '' 
        THEN '' 
        ELSE CONCAT('Scope Info: ', Project_Scope.Scope_Info) 
    END AS Scope_Info,
    
    COALESCE(Scope_of_Work.Scope_of_Work, '') AS Scope_of_Work,

    CASE 
        WHEN COALESCE(System_Profile.System_Type, '') = '' 
        THEN '' 
        ELSE CONCAT('System: ', System_Profile.System_Type) 
    END AS System_Type,

    CASE 
        WHEN COALESCE(Scope_Type.Type, '') = '' 
        THEN '' 
        ELSE CONCAT('Type: ', Scope_Type.Type) 
    END AS Type,

    CASE 
        WHEN COALESCE(Ladder_Profile.Height, '') = '' 
        THEN '' 
        ELSE CONCAT('Height: ', Ladder_Profile.Height) 
    END AS Height,

    CASE 
        WHEN COALESCE(Ladder_Profile.Ladder, '') = '' 
        THEN '' 
        ELSE CONCAT('Ladder: ', Ladder_Profile.Ladder) 
    END AS Ladder,

    CONCAT('From: ', 
        COALESCE(Project_Scope.From_Building, 'N/A') + ' -> ' + 
        COALESCE(Project_Scope.From_Floor, 'N/A') + ' -> ' + 
        COALESCE(Project_Scope.From_Location, 'N/A')) AS From_Location,

    CASE 
        WHEN (Project_Scope.To_Building IS NOT NULL OR Project_Scope.To_Floor IS NOT NULL OR Project_Scope.To_Location IS NOT NULL)
        THEN CONCAT('To: ', 
            COALESCE(Project_Scope.To_Building, 'N/A') + ' -> ' + 
            COALESCE(Project_Scope.To_Floor, 'N/A') + ' -> ' + 
            COALESCE(Project_Scope.To_Location, 'N/A'))
        ELSE '' 
    END AS To_Location,

    CASE 
        WHEN COALESCE(Project_Scope.Label_Convention, '') = '' 
        THEN '' 
        ELSE CONCAT('Label Convention: ', Project_Scope.Label_Convention) 
    END AS Label_Convention,

    Project_Scope.Qty,
    Project_Scope.Completion_Percent								
	FROM Project_Scope										
	INNER JOIN Scope_of_Work ON Scope_of_Work.Scope_of_Work_ID = Project_Scope.Scope_of_Work_ID										
	LEFT JOIN System_Profile ON System_Profile.System_Profile_ID = Project_Scope.System_Profile_ID										
	LEFT JOIN Scope_Type ON Scope_Type.Scope_Type_ID = Project_Scope.Scope_Type_ID										
	LEFT JOIN Ladder_Profile ON Ladder_Profile.Ladder_Profile_ID = Project_Scope.Ladder_Profile_ID										
	WHERE 	Project_Scope.Project_ID = @Project_ID									
		AND (SELECT COUNT(1) FROM Project_Task_Completion 									
		INNER JOIN Project_Scope_Task ON Project_Scope_Task.Project_Scope_Task_ID = Project_Task_Completion.Project_Scope_Task_ID									
	WHERE Project_Scope_Task.Project_Scope_ID = Project_Scope.Project_Scope_ID AND Project_Task_Completion.Verified_On IS NULL AND Project_Task_Completion.Employee_Profile_ID = @empID) > 0		
	ORDER BY Project_Scope.From_Building ASC, Project_Scope.From_Floor ASC, Project_Scope.From_Location ASC, Project_Scope.To_Building ASC, Project_Scope.To_Floor ASC, Project_Scope.To_Location ASC										

    ");

            parameters.Add(new SqlParameter("@Project_ID", Request.QueryString["Project_ID"]));
            parameters.Add(new SqlParameter("@empID", Session["EmpID"]));
            DataTable dt = con.FillDatatable(sql, parameters.ToArray());
            InvoiceGrid.DataSource = dt;
            InvoiceGrid.DataBind();
        }

        protected void btnToProject_Click(object sender, EventArgs e)
        {
            // Example of passing query parameters
            //lb.PostBackUrl = "~/Modules/Completion-Scope.aspx?Project_ID=" + ((Label)e.Row.Cells[2].FindControl("Project_ID")).Text;

            string targetUrl = "~/Modules/Submitted-WorkScope.aspx";
            Response.Redirect(targetUrl);
        }
    }
}