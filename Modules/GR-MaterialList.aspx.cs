using DBConnection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using FruitUnitedMobile.CommonFunctions;
using Image = System.Drawing.Image;
using System.Drawing;
using System.Xml.Linq;
using EASendMail;

namespace FruitUnitedMobile.Modules
{
    public partial class GR_MaterialList : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString; // Replace with your actual connection string

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Session["GR_Project_ID"].ToString()) || !string.IsNullOrEmpty(Session["GR_Location_ID"].ToString()))
            {
                ProjectTitle.Text = con.ExecuteSQLQueryWithOneReturn(@"SELECT Project_Name FROM Project WHERE Project_ID = " + Session["GR_Project_ID"].ToString()).ToString();
                if (!IsPostBack)
                {
                    Session["Material_Profile_ID"] = null;
                    BindAllDDL();
                    MaterialListGrid.DataSource = GR_Materialst_GV;
                    MaterialListGrid.DataBind();
                }
            }
            else
            {
                string targetUrl = "~/Modules/GR-Project.aspx";
                Response.Redirect(targetUrl);
            }
        }

        private void BindAllDDL()
        {
            BindCategoryDDL();
            BindBrandDDL();
            BindPartDDL();
            BindSNDDL();
            Material_Profile_ID_TB.Value = "0";
            Inventory_Ledger_ID_TB.Value = "0";
            QtyTB.Text = "0";
        }

        private void BindCategoryDDL()
        {

            string query = "SELECT Product_Category_ID, Category FROM Product_Category WHERE Status = 'Active' ORDER BY Category ASC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlCategory.DataSource = reader;
                    ddlCategory.DataTextField = "Category"; // Text displayed in dropdown
                    ddlCategory.DataValueField = "Product_Category_ID"; // Value for each item
                    ddlCategory.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlCategory.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        private void BindBrandDDL()
        {

            string query = "SELECT Brand_Profile_ID, Brand FROM Brand_Profile WHERE Status = 'Active' ORDER BY Brand ASC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlBrand.DataSource = reader;
                    ddlBrand.DataTextField = "Brand"; // Text displayed in dropdown
                    ddlBrand.DataValueField = "Brand_Profile_ID"; // Value for each item
                    ddlBrand.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlBrand.Items.Insert(0, new ListItem("-- Please Select --", "0"));
                }
            }
        }

        private void BindPartDDL()
        {
            ddlPart.Items.Clear();
            ddlPart.Items.Insert(0, new ListItem("-- Please Select --", ""));
        }

        private void BindSNDDL()
        {
            ddlSN.Items.Clear();
            ddlSN.Items.Insert(0, new ListItem("-- Please Select --", ""));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string GR_Project_ID = Session["GR_Project_ID"] as string;
            string GR_Location_ID = Session["GR_Location_ID"] as string;
            string EmployeeID = Session["EmpID"] as string;
            string AlertMessage1 = "Insuffince Balance!", AlertMessage2 = "";
            foreach (DataRow row in GR_Materialst_GV.Rows)
            {
                string MaterialBalanceSQLResult = con.ExecuteSQLQueryWithOneReturn(string.Format(@"
                SELECT
                bal.Balance
                FROM Material_Profile 		
                INNER JOIN 
	                (SELECT 
	                bal.Material_Profile_ID, 
	                bal.Material_Balance_ID, 
	                bal.UOM_Profile_ID, 
	                bal.Qty_Per_Pack, 
	                loc.Qty + ISNULL(reservation.Outstanding_Qty,0) AS Balance 
	                FROM Material_Balance bal 
	                INNER JOIN Material_Loc_Balance loc ON loc.Material_Balance_ID = bal.Material_Balance_ID 
	                LEFT JOIN 
		                (SELECT 
		                Material_Balance_ID, 
		                Location_Profile_ID, 
		                SUM(Outstanding_Qty) AS Outstanding_Qty 
		                FROM Reservation 
		                INNER JOIN Quotation ON Quotation.Quotation_ID = Reservation.Quotation_ID 
		                WHERE Quotation.Project_ID = {0}
		                AND Reservation.Location_Profile_ID = {1} GROUP BY Material_Balance_ID, Location_Profile_ID
		                ) reservation ON reservation.Material_Balance_ID = bal.Material_Balance_ID 
	                AND reservation.Location_Profile_ID = loc.Location_Profile_ID 
	                WHERE loc.Location_Profile_ID = {1}
	                ) bal ON bal.Material_Profile_ID = Material_Profile.Material_Profile_ID		
                INNER JOIN UOM_Profile UOM ON UOM.UOM_Profile_ID = bal.UOM_Profile_ID 
                WHERE ISNULL(bal.Balance,0) > 0 AND Material_Profile.Material_Profile_ID = {2}"
                , GR_Project_ID, GR_Location_ID, row["Material_Profile_ID"].ToString())).ToString();

                try
                {
                    decimal MaterialBalanceQTY = decimal.Parse(MaterialBalanceSQLResult);
                    if (MaterialBalanceQTY < decimal.Parse(row["Qty"].ToString()))
                    {
                        AlertMessage2 += "\\nBalance Qty for " + row["Part_No"].ToString() + " : " + MaterialBalanceSQLResult;
                    }
                }
                catch
                {
                    throw new Exception(MaterialBalanceSQLResult);
                }
            }

            if (!string.IsNullOrEmpty(AlertMessage2))
            {
                DataTable dt = GR_Materialst_GV;
                MaterialListGrid.DataSource = dt;
                MaterialListGrid.DataBind();
                //BindAllDDL();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + AlertMessage1 + AlertMessage2 + "');", true);
            }
            else
            {
                string imagePath = "";
                string INSERT_NEW_GI_SQLScript = string.Format(@"  
                    DECLARE @newGIID int
                    DECLARE @EmployeeID int = {0}
                    DECLARE @ProjectID int = {1}
                    DECLARE @Remark nvarchar(4000) = '{2}'

                    INSERT INTO GI
                    (
                    Creation_Time,
                    Status,
                    Doc_Date,
                    Employee_Profile_ID,
                    Employee_Profile_ID1,
                    Quotation_ID,
                    Project_Quotation_ID,
                    Job_Site,
                    Remark,
                    Image_Path
                    )
                    (
                    SELECT
                    GETDATE(),
                    'Received',
                    GETDATE(),
                    @EmployeeID,
                    @EmployeeID,
                    Project.Quotation_ID,
                    Project.Quotation_ID,
                    Quotation.Project_Name,
                    @Remark,
                    '" + imagePath + @"'
                    FROM Project
                    INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID
                    WHERE Project.Project_ID = @ProjectID
                    )

                    SET @newGIID = SCOPE_IDENTITY()
                    SELECT @newGIID
                    ", EmployeeID, GR_Project_ID, Remark_TB.Text);

                string New_GI_ID = con.ExecuteSQLQueryWithOneReturn(INSERT_NEW_GI_SQLScript).ToString();

                if (fileUpload1.HasFile)
                {
                    string baseDir = ConfigurationManager.AppSettings["SystemPath"];
                    string uploadFolder = baseDir + @"Document\GI\File\" + New_GI_ID + @"\";
                    string xmlFilePath = Path.Combine(uploadFolder, "user_" + New_GI_ID + ".xml");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    if (!File.Exists(xmlFilePath))
                    {
                        XDocument xmlDoc4 = new XDocument(
                            new XDeclaration("1.0", "utf-16", "yes"),
                            new XElement("NewDataSet")
                        );
                        xmlDoc4.Save(xmlFilePath);
                    }

                    // Validate file type
                    string fileExtension = Path.GetExtension(fileUpload1.FileName).ToLower();

                    // Generate a unique filename
                    string fileName = Path.GetFileName(fileUpload1.FileName);
                    string savePath = Path.Combine(uploadFolder, fileName);

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
                                    using (Bitmap resizedImage = CommonFunctions.CommonFunctions.ResizeImage(originalImage, 1024, 1024))
                                    {
                                        CommonFunctions.CommonFunctions.SaveCompressedImage(resizedImage, savePath, 75); // Compress with 75% quality
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Save original file if it's within size limit
                            fileUpload1.SaveAs(savePath);
                        }

                        imagePath = fileName; // Store file name for database
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error uploading file: " + ex.Message + "');", true);
                        return;
                    }

                    XDocument xmlDoc = XDocument.Load(xmlFilePath);
                    int fileId = xmlDoc.Descendants("UserFile").Count();
                    FileInfo fileInfo = new FileInfo(savePath);
                    long fileSize1 = fileInfo.Length; // File size in bytes
                    string currentUser = Session["Employee_Name"].ToString(); // Replace with actual user info if available
                    DateTime currentDate = DateTime.Now;

                    // Add file details to XML
                    xmlDoc.Root.Add(new XElement("UserFile",
                        new XElement("File_ID", fileId++),
                        new XElement("Name", fileName),
                        new XElement("User", currentUser),
                        new XElement("Date_Modified", currentDate.ToString("M/d/yyyy h:mm:ss tt")),
                        new XElement("Size", fileSize1),
                        new XElement("Owner", currentUser),
                        new XElement("OwnerID", Session["EmpID"]), // Replace with actual OwnerID
                        new XElement("Remarks"),
                        new XElement("Action", "Upload"),
                        new XElement("Executor", currentUser),
                        new XElement("ExecutorID", Session["EmpID"].ToString()), // Replace with actual ExecutorID
                        new XElement("Path", uploadFolder.Replace(@"\", "/"))
                    ));
                    xmlDoc.Save(xmlFilePath);

                }

                string GINo = con.ExecuteSQLQueryWithOneReturn(@"SELECT Doc_No FROM GI WHERE GI_ID = '" + New_GI_ID + @"'").ToString();

                if (string.IsNullOrEmpty(GINo))
                {
                    CommonFunctions.CommonFunctions.generateGRNNo(New_GI_ID, Session["EmpID"].ToString());
                }

                foreach (DataRow row in GR_Materialst_GV.Rows)
                {
                    string Inventory_Ledger_ID = "NULL";

                    if (row["Inventory_Ledger_ID"].ToString() != "0")
                    {
                        Inventory_Ledger_ID = row["Inventory_Ledger_ID"].ToString();
                    }

                    string 
                    Balance = "",
                    UOM_Profile_ID = "",
                    Material_Balance_ID = "",
                    Qty_Per_Pack = "";

                    DataTable GetAllInfoSQLDataTable = new DataTable();
                    string GetAllInfoSQLQuery = string.Format(@"
                    SELECT
                    Material_Profile.Material_Profile_ID, 
                    Material_Profile.Serial_No, 
                    bal.Balance, 
                    UOM.UOM, 
                    UOM.UOM_Profile_ID, 
                    bal.Material_Balance_ID, 
                    bal.Qty_Per_Pack 
                    FROM Material_Profile 		
                    INNER JOIN 
	                    (SELECT 
	                    bal.Material_Profile_ID, 
	                    bal.Material_Balance_ID, 
	                    bal.UOM_Profile_ID, 
	                    bal.Qty_Per_Pack, 
	                    loc.Qty + ISNULL(reservation.Outstanding_Qty,0) AS Balance 
	                    FROM Material_Balance bal 
	                    INNER JOIN Material_Loc_Balance loc ON loc.Material_Balance_ID = bal.Material_Balance_ID 
	                    LEFT JOIN 
		                    (SELECT 
		                    Material_Balance_ID, 
		                    Location_Profile_ID, 
		                    SUM(Outstanding_Qty) AS Outstanding_Qty 
		                    FROM Reservation 
		                    INNER JOIN Quotation ON Quotation.Quotation_ID = Reservation.Quotation_ID 
		                    WHERE Quotation.Project_ID = {0}
		                    AND Reservation.Location_Profile_ID = {1} GROUP BY Material_Balance_ID, Location_Profile_ID
		                    ) reservation ON reservation.Material_Balance_ID = bal.Material_Balance_ID 
	                    AND reservation.Location_Profile_ID = loc.Location_Profile_ID 
	                    WHERE loc.Location_Profile_ID = {1}
	                    ) bal ON bal.Material_Profile_ID = Material_Profile.Material_Profile_ID		
                    INNER JOIN UOM_Profile UOM ON UOM.UOM_Profile_ID = bal.UOM_Profile_ID 
                    WHERE ISNULL(bal.Balance,0) > 0 AND Material_Profile.Material_Profile_ID = {2}"
                    , GR_Project_ID, GR_Location_ID, row["Material_Profile_ID"].ToString());

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(GetAllInfoSQLQuery, con))
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(GetAllInfoSQLDataTable);
                        }
                    }
                    if (GetAllInfoSQLDataTable.Rows.Count > 0)
                    {
                        Balance = GetAllInfoSQLDataTable.Rows[0]["Balance"].ToString();
                        UOM_Profile_ID = GetAllInfoSQLDataTable.Rows[0]["UOM_Profile_ID"].ToString();
                        Material_Balance_ID = GetAllInfoSQLDataTable.Rows[0]["Material_Balance_ID"].ToString();
                        Qty_Per_Pack = GetAllInfoSQLDataTable.Rows[0]["Qty_Per_Pack"].ToString();
                    }
                    else
                    {
                        throw new Exception("No Record Found");
                    }

                    string Insert_GI_Item_SQLScript = string.Format(@"
                    DECLARE @newGIID int = {4}
                    DECLARE @Qty_GI decimal(20,2) = '{0}'
                    DECLARE @Material_Balance_ID int = {1}
                    DECLARE @Location_ID int = {2}
                    DECLARE @Inventory_Ledger_ID int = {3}
                       

                    INSERT INTO GI_Item							
                    (							
	                    Creation_Time,						
	                    GI_ID,						
	                    Material_Profile_ID,						
	                    Qty,						
	                    Internal_Qty,						
	                    Material_Balance_ID,						
	                    Location_Profile_ID,						
	                    Inventory_Ledger_ID						
                    )							
                    (							
	                    SELECT						
		                    GETDATE(),					
		                    @newGIID,					
		                    Material_Profile_ID,					
		                    @Qty_GI,					
		                    @Qty_GI * Qty_Per_Pack,					
		                    @Material_Balance_ID,					
		                    @Location_ID,					
		                    @Inventory_Ledger_ID				
	                    FROM Material_Balance						
	                    WHERE Material_Balance_ID IN (@Material_Balance_ID)						
                    )							

                   
                    ", row["Qty"].ToString(), Material_Balance_ID, GR_Location_ID, Inventory_Ledger_ID, New_GI_ID);

                    con.ExecuteSQLQuery(Insert_GI_Item_SQLScript);


                    //ErrorMessage.Text = "GR_Project_ID : " + GR_Project_ID + "GR_Location_ID : " + GR_Location_ID + "Material_Balance_ID : " + Material_Balance_ID + "Material_Profile_ID : " + row["Material_Profile_ID"].ToString() + "UOM_Profile_ID : " + UOM_Profile_ID + "Qty_Per_Pack : " + Qty_Per_Pack + " : ";
                    //Update All the Store Procedure.
                    //Update_InventoryLocationBalance_Procedure(Material_Balance_ID, GR_Location_ID);
                    //Update_InventoryBalance_Procedure(row["Material_Profile_ID"].ToString(), UOM_Profile_ID, Qty_Per_Pack);
                }

                con.ExecuteSQLQuery(string.Format(@" 
                    DECLARE @newGIID int = {0}
                    DECLARE @deductTable TABLE
                    (
                        RowNumber int,
                        itemID int,
                        matBalanceID int,
                        matID int,
                        locationID int,
                        qty decimal(20,2)
                    )

                   DECLARE @reservationTable TABLE
                   (
                   FIFOCount int,
                   reservationID int,
                   FIFOBalance decimal(20,2)
                   )

                   DECLARE @numberRecords int
                   DECLARE @rowCounter int
                   DECLARE @itemID int
                   DECLARE @qty decimal(20,2)
                   DECLARE @matBalanceID int
                   DECLARE @matID int
                   DECLARE @locationID int
                   DECLARE @balance decimal(20,2)

                   DECLARE @FIFONumberRecords int
                   DECLARE @FIFORowCounter int
                   DECLARE @FIFOQty decimal(20,2)
                   DECLARE @reservationID int
                   DECLARE @ledgerQty decimal(20,2)
                   DECLARE @FIFOBalance decimal(20,2)

                   INSERT INTO @deductTable
                   (
                       RowNumber,
                       itemID,
                       matBalanceID,
                       matID,
                       locationID,
                       qty            
                   )
                   (
                       SELECT
                           ROW_NUMBER() OVER (ORDER BY item.GI_Item_ID ASC),
                           item.GI_Item_ID,
                           item.Material_Balance_ID,
                           item.Material_Profile_ID,
                           item.Location_Profile_ID,
                           item.Qty
                       FROM GI_Item item
                       INNER JOIN GI ON GI.GI_ID = item.GI_ID
                       WHERE item.GI_ID = @newGIID
                   AND (SELECT COUNT(1) FROM Reservation
                   WHERE Reservation.Material_Balance_ID = item.Material_Balance_ID
                   AND Reservation.Location_Profile_ID = item.Location_Profile_ID
                   AND Reservation.Outstanding_Qty > 0
                   AND Reservation.Quotation_ID = GI.Quotation_ID) > 0
                   ) ORDER BY item.GI_Item_ID ASC
                   
                   SET @numberRecords = @@RowCount
                   SET @rowCounter = 1

                   WHILE @rowCounter <= @numberRecords
                   BEGIN
                   SELECT
                   @itemID = itemID,
                   @qty = qty,
                   @matBalanceID = matBalanceID,
                   @matID = matID,
                   @locationID = locationID
                   FROM @deductTable
                   WHERE RowNumber = @rowCounter

                   SET @balance = @qty

                   DELETE FROM @reservationTable

                   INSERT INTO @reservationTable
                   (
                   FIFOCount,
                   reservationID,
                   FIFOBalance
                   )
                   (
                   SELECT
                   ROW_NUMBER() OVER (ORDER BY Reserved_Date ASC, Reservation_ID ASC),
                   Reservation_ID,
                   Outstanding_Qty
                   FROM Reservation
                   WHERE Material_Balance_ID = @matBalanceID AND Location_Profile_ID = @locationID AND Outstanding_Qty > 0
                   ) ORDER BY Reserved_Date ASC, Reservation_ID ASC

                   SET @FIFONumberRecords = @@RowCount
                   SET @FIFORowCounter = 1

                   WHILE @balance > 0 AND @FIFORowCounter <= @FIFONumberRecords
                   BEGIN
                   SELECT
                   @FIFOQty = FIFOBalance,
                   @reservationID = reservationID
                   FROM @reservationTable
                   WHERE FIFOCount = @FIFORowCounter

                   IF (@balance > @FIFOQty)
                   BEGIN
                   SET @ledgerQty = @FIFOQty
                   SET @balance = @balance - @FIFOQty
                   SET @FIFOBalance = 0
                   END
                   ELSE
                   BEGIN
                   SET @ledgerQty = @balance
                   SET @FIFOBalance = @FIFOQty - @balance
                   SET @balance = 0
                   END

                   INSERT INTO Release_History
                   (
                   Creation_Time,
                   Release_Date,
                   Reservation_ID,
                   Qty,
                   Auto_Manual,
                   Release_Issue
                   )
                   (
                   SELECT
                   GETDATE(),
                   GETDATE(),
                   Reservation_ID,
                   @ledgerQty,
                   'Manual',
                   'Issue'
                   FROM Reservation
                   WHERE Reservation_ID = @reservationID
                   )

                   UPDATE Reservation
                       SET Released_Qty = ISNULL(release.Release_Qty,0),
                       Nett_Reserved_Qty = rvc.Qty - ISNULL(release.Release_Qty,0),
                       Issued_Qty = ISNULL(issue.Issue_Qty,0),
                       Outstanding_Qty = rvc.Qty - ISNULL(release.Release_Qty,0) - ISNULL(issue.Issue_Qty,0),
                       Status = CASE WHEN ((rvc.Qty - ISNULL(release.Release_Qty,0) - ISNULL(issue.Issue_Qty,0)) = 0) THEN 'Closed' ELSE rvc.Status END
                       FROM Reservation rvc
                       LEFT JOIN (
                           SELECT
                           Reservation_ID,
                           SUM(Qty) AS Release_Qty
                           FROM Release_History
                           WHERE Release_Issue = 'Release'
                           GROUP BY Reservation_ID
                       ) release ON release.Reservation_ID = rvc.Reservation_ID
                       LEFT JOIN (
                           SELECT
                           Reservation_ID,
                           SUM(Qty) AS Issue_Qty
                           FROM Release_History
                           WHERE Release_Issue = 'Issue'
                           GROUP BY Reservation_ID
                       ) issue ON issue.Reservation_ID = rvc.Reservation_ID
                       WHERE rvc.Reservation_ID = @reservationID

                   SET @FIFORowCounter = @FIFORowCounter + 1
                   END

                   SET @rowCounter = @rowCounter + 1
                   END

                   DECLARE @deductTable1 TABLE
                   (
                       RowNumber int,
                       itemID int,
                       matBalanceID int,
                       matID int,
                       locationID int,
                       qty decimal(20,2)
                   )

                   DECLARE @ledgerTable TABLE
                   (
                       FIFOCount int,
                       ledgerID int,
                       FIFOBalance decimal(20,2),
                       costID int,
                       batchNo nvarchar(255)
                   )

                   DECLARE @numberRecords1 int
                   DECLARE @rowCounter1 int
                   DECLARE @itemID1 int
                   DECLARE @matBalanceID1 int
                   DECLARE @matID1 int
                   DECLARE @locationID1 int
                   DECLARE @qty1 decimal(20,2)
                   DECLARE @balance1 decimal(20,2)
                   DECLARE @FIFONumberRecords1 int
                   DECLARE @FIFORowCounter1 int
                   DECLARE @FIFOQty1 decimal(20,2)
                   DECLARE @ledgerID int
                   DECLARE @ledgerQty1 decimal(20,2)
                   DECLARE @FIFOBalance1 decimal(20,2)
                   DECLARE @costID int
                   DECLARE @batchNo nvarchar(255)

                   -- For part with SN
                   INSERT INTO Inventory_Ledger
                   (
                       Creation_Time,
                       In_Out,
                       Source,
                       Source_No,
                       Source_Date,
                       Source_ID,
                       Main_Source_ID,
                       Quotation_ID,
                       Service_Maintenance_ID,
                       Material_Profile_ID,
                       Location_Profile_ID,
                       Serial_No,
                       Packaged_Qty,
                       Actual_Qty,
                       UOM_Profile_ID,
                       Qty_Per_Pack,
                       Qty,
                       Material_Cost_ID,
                       Material_Balance_ID,
                       Batch_ID,
                       Batch_No
                   )
                   (
                       SELECT
                           GETDATE(),
                           'OUT',
                           CASE WHEN (GI.Purchase_Receive_ID IS NOT NULL) THEN 'Direct Issue' ELSE 'Goods Issue' END,
                           GI.Doc_No,
                           GI.Received_On,
                           item.GI_Item_ID,
                           GI.GI_ID,
                           GI.Quotation_ID,
                           GI.Service_Maintenance_ID,
                           item.Material_Profile_ID,
                           item.Location_Profile_ID,
                           IL.Serial_No,
                           item.Qty,
                           item.Qty,
                           balance.UOM_Profile_ID,
                           balance.Qty_Per_Pack,
                           item.Internal_Qty,
                           IL.Material_Cost_ID,
                           balance.Material_Balance_ID,
                           IL.Inventory_Ledger_ID,
                           IL.Source_No
                       FROM GI_Item item WITH(ROWLOCK,XLOCK)
                       INNER JOIN Inventory_Ledger IL WITH(ROWLOCK,XLOCK) ON IL.Inventory_Ledger_ID = item.Inventory_Ledger_ID
                       INNER JOIN Material_Balance balance ON balance.Material_Balance_ID = item.Material_Balance_ID
                       INNER JOIN GI ON GI.GI_ID = item.GI_ID
                       WHERE item.GI_ID = @newGIID
                   ) ORDER BY GI.Doc_Date ASC, item.GI_Item_ID ASC

                   UPDATE Inventory_Ledger
                   SET Packaged_Balance = 0,
                       Balance = 0
                   FROM Inventory_Ledger ledger
                   INNER JOIN GI_Item item ON item.Inventory_Ledger_ID = ledger.Inventory_Ledger_ID
                   WHERE item.GI_ID = @newGIID

                   -- For part without SN
                   INSERT INTO @deductTable1
                   (
                       RowNumber,
                       itemID,
                       matBalanceID,
                       matID,
                       locationID,
                       qty            
                   )
                   (
                       SELECT
                           ROW_NUMBER() OVER (ORDER BY item.GI_Item_ID ASC),
                           item.GI_Item_ID,
                           item.Material_Balance_ID,
                           item.Material_Profile_ID,
                           item.Location_Profile_ID,
                           item.Qty
                       FROM GI_Item item WITH(ROWLOCK,XLOCK)
                       INNER JOIN GI ON GI.GI_ID = item.GI_ID
                       WHERE item.GI_ID = @newGIID
                               AND item.Inventory_Ledger_ID IS NULL
                   ) ORDER BY GI.Doc_Date ASC, item.GI_Item_ID ASC

                   SET @numberRecords1 = @@RowCount
                   SET @rowCounter1 = 1

                   WHILE @rowCounter1 <= @numberRecords1
                   BEGIN
                       SELECT
                           @itemID1 = itemID,
                           @qty1 = qty,
                           @matBalanceID1 = matBalanceID,
                           @matID1 = matID,
                           @locationID1 = locationID
                       FROM @deductTable1
                       WHERE RowNumber = @rowCounter1

                       SET @balance1 = @qty1

                       DELETE FROM @ledgerTable

                       INSERT INTO @ledgerTable
                       (
                           FIFOCount,
                           ledgerID,
                           FIFOBalance,
                           costID,
                           batchNo
                       )
                       (
                           SELECT
                               ROW_NUMBER() OVER (ORDER BY ledger.Inventory_Ledger_ID ASC),
                               ledger.Inventory_Ledger_ID,
                               ledger.Packaged_Balance,
                               ledger.Material_Cost_ID,
                               ledger.Source_No
                           FROM Inventory_Ledger ledger WITH(ROWLOCK,XLOCK)
                           WHERE ledger.Material_Balance_ID = @matBalanceID1
                                   AND ledger.Packaged_Balance > 0
                                   AND ledger.Location_Profile_ID = @locationID1
                       ) ORDER BY ledger.Inventory_Ledger_ID ASC

                       SET @FIFONumberRecords1 = @@RowCount
                       SET @FIFORowCounter1 = 1

                       WHILE @balance1 > 0 AND @FIFORowCounter1 <= @FIFONumberRecords1
                       BEGIN
                           SELECT
                               @FIFOQty1 = FIFOBalance,
                               @ledgerID = ledgerID,
                               @costID = costID,
                               @batchNo = batchNo
                           FROM @ledgerTable
                           WHERE FIFOCount = @FIFORowCounter1

                           IF (@balance1 > @FIFOQty1)
                               BEGIN
                               SET @ledgerQty1 = @FIFOQty1
                               SET @balance1 = @balance1 - @FIFOQty1
                               SET @FIFOBalance1 = 0
                               END
                           ELSE
                               BEGIN
                               SET @ledgerQty1 = @balance1
                               SET @FIFOBalance1 = @FIFOQty1 - @balance1
                               SET @balance1 = 0
                               END

                           UPDATE Inventory_Ledger
                           SET Packaged_Balance = @FIFOBalance1,
                               Balance = @FIFOBalance1 * Qty_Per_Pack
                           WHERE Inventory_Ledger_ID = @ledgerID

                   INSERT INTO Inventory_Ledger
                   (
                       Creation_Time,
                       In_Out,
                       Source,
                       Source_No,
                       Source_Date,
                       Source_ID,
                       Main_Source_ID,
                       Material_Profile_ID,
                       Location_Profile_ID,
                       Packaged_Qty,
                       Actual_Qty,
                       UOM_Profile_ID,
                       Qty_Per_Pack,
                       Qty,
                       Material_Cost_ID,
                       Material_Balance_ID,
                       Quotation_ID,
                       Service_Maintenance_ID,
                       Batch_ID,
                       Batch_No
                   )
                   (
                       SELECT
                           GETDATE(),
                           'OUT',
                           'Goods Issue',
                           GI.Doc_No,
                           GI.Doc_Date,
                           item.GI_Item_ID,
                           GI.GI_ID,
                           item.Material_Profile_ID,
                           item.Location_Profile_ID,
                           @ledgerQty1,
                           @ledgerQty1,
                           balance.UOM_Profile_ID,
                           balance.Qty_Per_Pack,
                           @ledgerQty1 * balance.Qty_Per_Pack,
                           @costID,
                           item.Material_Balance_ID,
                           GI.Quotation_ID,
                           GI.Service_Maintenance_ID,
                           @ledgerID,
                           @batchNo
                       FROM GI_Item item WITH(ROWLOCK,XLOCK)
                       INNER JOIN Material_Balance balance ON balance.Material_Balance_ID = item.Material_Balance_ID
                       INNER JOIN GI ON GI.GI_ID = item.GI_ID
                       WHERE item.GI_Item_ID = @itemID1
                   )

                           SET @FIFORowCounter1 = @FIFORowCounter1 + 1
                       END

                       SET @rowCounter1 = @rowCounter1 + 1
                   END", New_GI_ID));

                string Quotation_ID = con.ExecuteSQLQueryWithOneReturn("SELECT Quotation_ID FROM GI WHERE GI_ID =" + New_GI_ID).ToString();
                string Combine_Material_ID = con.ExecuteSQLQueryWithOneReturn("SELECT STUFF((SELECT DISTINCT ', ' + CONVERT(nvarchar(50), Material_Profile_ID) FROM GI_Item WHERE GI_ID = " + New_GI_ID + " FOR XML PATH('')), 1, 1, '')").ToString();
                Insert_SOConsolidatedBOM_Procedure(New_GI_ID);
                Insert_ProjectConsolidatedBOM_Procedure(New_GI_ID);
                Update_SOBOMPRQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMIssuedReturnedPOQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMReservedQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_ProjectBOMQty(GR_Project_ID);

                string GetAllInfoFROMGRTNQuery = string.Format(@"
                 SELECT DISTINCT
	                GI_Item.Material_Balance_ID,
	                GI_Item.Location_Profile_ID,
	                GI_Item.Material_Profile_ID,
	                Material_Balance.UOM_Profile_ID,
	                Material_Balance.Qty_Per_Pack
                FROM GI_Item
                INNER JOIN Material_Balance ON Material_Balance.Material_Balance_ID = GI_Item.Material_Balance_ID
                INNER JOIN GI ON GI.GI_ID = GI_Item.GI_ID
                WHERE GI.GI_ID = {0}"
               ,New_GI_ID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(GetAllInfoFROMGRTNQuery, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            string Material_Balance_ID = reader["Material_Balance_ID"].ToString();
                            string Location_Profile_ID = reader["Location_Profile_ID"].ToString();
                            string Material_Profile_ID = reader["Material_Profile_ID"].ToString();
                            string UOM_Profile_ID = reader["UOM_Profile_ID"].ToString();
                            string Qty_Per_Pack = reader["Qty_Per_Pack"].ToString();
                            Update_InventoryLocationBalance_Procedure(Material_Balance_ID, Location_Profile_ID);
                            Update_InventoryBalance_Procedure(Material_Profile_ID, UOM_Profile_ID, Qty_Per_Pack);
                        }
                    }
                }

                DataTable GetEmailInfoSQLDataTable = new DataTable();
                string GetEmailInfoSQLScript = string.Format(@"
                    DECLARE @newGIID int = {0}

                    SELECT Project.Project_No,
                    Gi.Doc_No,
                    Customer_Profile.Customer_Name,
                    Project.Project_Name,
                    Employee_Profile.Employee_Name,
                    Project.Project_ID
                    FROM GI
                    LEFT JOIN Quotation on GI.Quotation_ID = Quotation.Quotation_ID
                    LEFT JOIN Project on Quotation.Quotation_ID = Project.Quotation_ID
                    LEFT JOIN Customer_Profile on Quotation.Customer_Profile_ID = Customer_Profile.Customer_Profile_ID
                    LEFT JOIN Employee_Profile on Employee_Profile.Employee_Profile_ID = GI.Employee_Profile_ID
                    WHERE GI.GI_ID = @newGIID", New_GI_ID
                );

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(GetEmailInfoSQLScript, con))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(GetEmailInfoSQLDataTable);
                    }
                }
                if (GetEmailInfoSQLDataTable.Rows.Count > 0)
                {
                    string SO_No = GetEmailInfoSQLDataTable.Rows[0]["Project_No"].ToString();
                    string GRN_No = GetEmailInfoSQLDataTable.Rows[0]["Doc_No"].ToString();
                    string Customer = GetEmailInfoSQLDataTable.Rows[0]["Customer_Name"].ToString();
                    string Project_Name = GetEmailInfoSQLDataTable.Rows[0]["Project_Name"].ToString();
                    string Request_Person = GetEmailInfoSQLDataTable.Rows[0]["Employee_Name"].ToString();
                    string Project_ID = GetEmailInfoSQLDataTable.Rows[0]["Project_ID"].ToString();
                    SendNotificationEmail(SO_No, GRN_No, Customer, Project_Name, Request_Person, Project_ID);
                }
                else
                {
                    throw new Exception("No Record Found");
                }

                string targetUrl = "~/Modules/Menu.aspx";
                Response.Redirect(targetUrl);
            }



        }
        
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string Material_Profile_ID = Material_Profile_ID_TB.Value;
            string Inventory_Ledger_ID = Inventory_Ledger_ID_TB.Value;
            bool duplicateFound = false;

            foreach (GridViewRow row in MaterialListGrid.Rows)
            {
                HiddenField Material_Profile_ID_HF = (HiddenField)row.Cells[1].FindControl("Material_Profile_ID_HF");
                HiddenField Inventory_Ledger_ID_HF = (HiddenField)row.Cells[1].FindControl("Inventory_Ledger_ID_HF");
                if (Inventory_Ledger_ID_HF.Value == "0")
                {
                    if (Material_Profile_ID.Contains(Material_Profile_ID_HF.Value))
                    {
                        duplicateFound = true;
                        break;
                    }
                }
                else
                {
                    if (Inventory_Ledger_ID.Contains(Inventory_Ledger_ID_HF.Value))
                    {
                        duplicateFound = true;
                        break;
                    }
                }
            }

            if (duplicateFound)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('This Part already added!');", true);
                DataTable dt = GR_Materialst_GV;
                MaterialListGrid.DataSource = dt;
                MaterialListGrid.DataBind();
                Session["Material_Profile_ID"] = Material_Profile_ID;
                //BindAllDDL();
            }
            else
            {
                string Part_No = con.ExecuteSQLQueryWithOneReturn("Select Part_No FROM Material_Profile WHERE Material_Profile_ID = " + Material_Profile_ID).ToString();
                string Part_Description = con.ExecuteSQLQueryWithOneReturn("Select Description FROM Material_Profile WHERE Material_Profile_ID = " + Material_Profile_ID).ToString();
                string Serial_No = "";
                string QTY = QtyTB.Text;

                if (Inventory_Ledger_ID != "0" || Inventory_Ledger_ID == "")
                {
                    Serial_No = con.ExecuteSQLQueryWithOneReturn(@"Select 'SN : ' + Serial_No FROM Inventory_Ledger WHERE Inventory_Ledger_ID = " + Inventory_Ledger_ID).ToString();
                }

                DataTable dt = GR_Materialst_GV;
                dt.Rows.Add(Part_No, Part_Description, Serial_No, QTY, Material_Profile_ID, Inventory_Ledger_ID);
                GR_Materialst_GV = dt;
                MaterialListGrid.DataSource = dt;
                MaterialListGrid.DataBind();
                Session["Material_Profile_ID"] = Material_Profile_ID;
                //BindAllDDL();
                btnSubmit.Visible = true;
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetddlPart(string BrandID)
        {
            string GR_Project_ID = HttpContext.Current.Session["GR_Project_ID"] as string;
            string GR_Location_ID = HttpContext.Current.Session["GR_Location_ID"] as string;
            List<object> Material_Part = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"
                SELECT 
                Material_Profile.Material_Profile_ID, 
                (Material_Profile.Part_No + ' / ' + CONVERT(nvarchar(max), Material_Profile.Description) + '[' + UOM.UOM + ']') AS Part ,
                Material_Profile.Serial_No, 
                bal.Balance, 
                UOM.UOM, 
                UOM.UOM_Profile_ID, 
                bal.Material_Balance_ID, 
                bal.Qty_Per_Pack 
                FROM Material_Profile 		
                INNER JOIN 
	                (SELECT 
	                bal.Material_Profile_ID, 
	                bal.Material_Balance_ID, 
	                bal.UOM_Profile_ID, 
	                bal.Qty_Per_Pack, 
	                loc.Qty + ISNULL(reservation.Outstanding_Qty,0) AS Balance 
	                FROM Material_Balance bal 
	                INNER JOIN Material_Loc_Balance loc ON loc.Material_Balance_ID = bal.Material_Balance_ID 
	                LEFT JOIN 
		                (SELECT 
		                Material_Balance_ID, 
		                Location_Profile_ID, 
		                SUM(Outstanding_Qty) AS Outstanding_Qty 
		                FROM Reservation 
		                INNER JOIN Quotation ON Quotation.Quotation_ID = Reservation.Quotation_ID 
		                WHERE Quotation.Project_ID = {0} 
		                AND Reservation.Location_Profile_ID = {1} GROUP BY Material_Balance_ID, Location_Profile_ID
		                ) reservation ON reservation.Material_Balance_ID = bal.Material_Balance_ID 
	                AND reservation.Location_Profile_ID = loc.Location_Profile_ID 
	                WHERE loc.Location_Profile_ID = {1}
	                ) bal ON bal.Material_Profile_ID = Material_Profile.Material_Profile_ID		
                INNER JOIN UOM_Profile UOM ON UOM.UOM_Profile_ID = bal.UOM_Profile_ID 
                WHERE ISNULL(bal.Balance,0) > 0 AND Material_Profile.Brand_Profile_ID = {2}
                ", GR_Project_ID, GR_Location_ID, BrandID);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Material_Part.Add(new
                    {
                        Material_Profile_ID = reader["Material_Profile_ID"].ToString(),
                        Part_No = reader["Part"].ToString()
                    });
                }
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(Material_Part); // return as JSON array
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetddlSerialNo(string PartID)
        {
            string GR_Location_ID = HttpContext.Current.Session["GR_Location_ID"] as string;
            List<object> Material_SerialNo = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"
                SELECT
                Inventory_Ledger.Inventory_Ledger_ID,
                Inventory_Ledger.Serial_No
                FROM Inventory_Ledger
                LEFT JOIN Material_Profile on Material_Profile.Material_Profile_ID = Inventory_Ledger.Material_Profile_ID
                LEFT JOIN UOM_Profile on Material_Profile.UOM_Profile_ID = UOM_Profile.UOM_Profile_ID
                WHERE Inventory_Ledger.Location_Profile_ID = {0}
                AND Inventory_Ledger.Material_Profile_ID = {1}
                AND Inventory_Ledger.Packaged_Balance > 0
                AND Material_Profile.Serial_No = 'Y'
                ", GR_Location_ID, PartID);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Material_SerialNo.Add(new
                    {
                        Inventory_Ledger_ID = reader["Inventory_Ledger_ID"].ToString(),
                        Serial_No = reader["Serial_No"].ToString()
                    });
                }
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(Material_SerialNo); // return as JSON array
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetUOMFromPart(string PartID)
        {
            string GR_Location_ID = HttpContext.Current.Session["GR_Location_ID"] as string;
            List<object> Material_SerialNo = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"
                SELECT
                UOM_Profile.UOM
                FROM Material_Profile
                LEFT JOIN UOM_Profile on Material_Profile.UOM_Profile_ID = UOM_Profile.UOM_Profile_ID
                WHERE Material_Profile.Material_Profile_ID = {0}
                ", PartID);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Material_SerialNo.Add(new
                    {
                        UOM = reader["UOM"].ToString()
                    });
                }
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(Material_SerialNo); // return as JSON array
        }

        protected void MaterialListGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString();
                e.Row.Cells[0].Style.Add("text-align", "center");
                e.Row.Cells[2].Style.Add("text-align", "center");
                e.Row.Cells[3].Style.Add("text-align", "center");
            }
        }

        protected void MaterialListGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                string id = e.CommandArgument.ToString();

                // Remove row from DataTable (simulate delete)
                foreach (DataRow row in GR_Materialst_GV.Rows)
                {
                    if (row["Material_Profile_ID"].ToString() == id)
                    {
                        GR_Materialst_GV.Rows.Remove(row);
                        break;
                    }
                }

                MaterialListGrid.DataSource = GR_Materialst_GV;
                MaterialListGrid.DataBind();
            }
        }

        private DataTable GR_Materialst_GV
        {
            get
            {
                if (ViewState["GR_Materialst_GV"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Part_No");
                    dt.Columns.Add("Part_Description");
                    dt.Columns.Add("Serial_No");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("Material_Profile_ID");
                    dt.Columns.Add("Inventory_Ledger_ID");
                    DataColumn customerNoCol = new DataColumn("S_N", typeof(int));
                    customerNoCol.AutoIncrement = true;
                    customerNoCol.AutoIncrementSeed = 1;
                    customerNoCol.AutoIncrementStep = 1;
                    dt.Columns.Add(customerNoCol);
                    ViewState["GR_Materialst_GV"] = dt;
                }
                return (DataTable)ViewState["GR_Materialst_GV"];
            }
            set
            {
                ViewState["GR_Materialst_GV"] = value;
            }
        }

        private void Update_InventoryLocationBalance_Procedure(string Material_Balance_ID, string Location_ID)
        {
            string storedProcName = "Update_InventoryLocationBalance";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@balanceID", Material_Balance_ID);
                        command.Parameters.AddWithValue("@locationID", Location_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Update_InventoryBalance_Procedure(string Material_Profile_ID, string UOM_Profile_ID, string Qty_Per_Pack)
        {
            string storedProcName = "Update_InventoryBalance";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@materialID", Material_Profile_ID);
                        command.Parameters.AddWithValue("@UOM", UOM_Profile_ID);
                        command.Parameters.AddWithValue("@qtyPerPack", Qty_Per_Pack);
                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Insert_SOConsolidatedBOM_Procedure(string GI_ID)
        {
            string storedProcName = "Insert_SOConsolidatedBOM";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineID", GI_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Insert_ProjectConsolidatedBOM_Procedure(string GI_ID)
        {
            string storedProcName = "Insert_ProjectConsolidatedBOM";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineID", GI_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Update_SOBOMPRQty_Procedure(string Quotation_ID, string Combine_Material_Profile_ID)
        {
            string storedProcName = "Update_SOBOMPRQty";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineQtnID", Quotation_ID);
                        command.Parameters.AddWithValue("@combineMatID", Combine_Material_Profile_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Update_SOBOMIssuedReturnedPOQty_Procedure(string Quotation_ID, string Combine_Material_Profile_ID)
        {
            string storedProcName = "Update_SOBOMIssuedReturnedPOQty";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineQtnID", Quotation_ID);
                        command.Parameters.AddWithValue("@combineMatID", Combine_Material_Profile_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void Update_SOBOMReservedQty_Procedure(string Quotation_ID, string Combine_Material_Profile_ID)
        {
            string storedProcName = "Update_SOBOMReservedQty";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineQtnID", Quotation_ID);
                        command.Parameters.AddWithValue("@combineMatID", Combine_Material_Profile_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.StackTrace + Quotation_ID + ":" + Combine_Material_Profile_ID);
            }
        }

        private void Update_SOBOMQty_Procedure(string Quotation_ID, string Combine_Material_Profile_ID)
        {
            string storedProcName = "Update_SOBOMQty";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineQtnID", Quotation_ID);
                        command.Parameters.AddWithValue("@combineMatID", Combine_Material_Profile_ID);

                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.StackTrace + Quotation_ID + ":" + Combine_Material_Profile_ID);
            }
        }

        private void Update_ProjectBOMQty(string Project_ID)
        {
            string storedProcName = "Update_ProjectBOMQty";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@combineProjID", Project_ID);
                        // Open connection and execute
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private void SendNotificationEmail(string SO_No, string GRN_No, string Customer, string Project_Name, string Request_Person, string Project_ID)
        {
            string EmailSubjectTitle = "Goods Issue Notification(Enhanzcom Pte Ltd)";
            string EmployeeEmailAddress = "aquek@enhanzcom.com";
            string BodyEmail = @"Dear PIC, " + Environment.NewLine + Environment.NewLine;
            BodyEmail += @"The following SO requires your attention:" + Environment.NewLine + Environment.NewLine;
            BodyEmail += @"SO No : " + SO_No + Environment.NewLine;
            BodyEmail += @"GRN No : " + GRN_No + Environment.NewLine;
            BodyEmail += @"Customer : " + Customer + Environment.NewLine;
            BodyEmail += @"Project Name : " + Project_Name + Environment.NewLine;
            BodyEmail += @"Request Person : " + Request_Person + Environment.NewLine + Environment.NewLine;
            BodyEmail += @"Please login to the system for more details." + Environment.NewLine + Environment.NewLine;
            BodyEmail += @"Sent By : EMS Admin" + Environment.NewLine;

            // SEND EMAIL TO RECIPIENT
            string smtpHost = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_Server_Name FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4'").ToString();
            string smtpUser = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_User_name FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4'").ToString();
            string smtpPass = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_USER_Password FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4'").ToString();
            int smtpPort = int.Parse(con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 Port FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4'").ToString());
            bool useSSL = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 CASE WHEN SSL = '1' THEN 'Yes' END FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4'").ToString() == "Yes";
            int totalRecipients = 0;


            string GetEmailDetailFromSQL = string.Format(@"
                SELECT DISTINCT
                Employee_Profile.Email
                FROM Employee_Profile
                INNER JOIN
                (
                SELECT Employee_Profile_ID FROM Project WHERE Project_ID = {0}
                UNION ALL
                SELECT Employee_Profile_ID FROM Employee_Profile WHERE Trigger_GI = 'Y'
                ) tempTable ON tempTable.Employee_Profile_ID = Employee_Profile.Employee_Profile_ID
                WHERE Employee_Profile.Status = 'Active' AND Employee_Profile.Email IS NOT NULL
                ", Project_ID);
            DataTable dataTable = new DataTable();

            // Create a connection to the database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Create a SqlDataAdapter to execute the query and fill the DataTable
                SqlDataAdapter dataAdapter = new SqlDataAdapter(GetEmailDetailFromSQL, con);

                try
                {
                    // Open the connection
                    con.Open();

                    // Fill the DataTable with data
                    dataAdapter.Fill(dataTable);

                    // Iterate through each DataRow in the DataTable
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string email = (string)row["Email"];

                        if (smtpPort != 465)
                        {
                            SmtpMail oMail = new SmtpMail("TryIt");
                            oMail.From = smtpUser;
                            oMail.To = email;
                            oMail.Subject = EmailSubjectTitle;
                            oMail.TextBody = BodyEmail;
                            SmtpServer oServer = new SmtpServer(smtpHost);
                            oServer.User = smtpUser;
                            oServer.Password = smtpPass;
                            oServer.Port = smtpPort;
                            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                            SmtpClient oSmtp = new SmtpClient();
                            oSmtp.SendMail(oServer, oMail);
                        }
                        else if (smtpPort == 465)
                        {
                            CDO.Message oMsg = new CDO.Message();
                            CDO.IConfiguration iConfg;
                            iConfg = oMsg.Configuration;
                            ADODB.Fields oFields;
                            oFields = iConfg.Fields;
                            ADODB.Field oField = oFields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
                            oField.Value = CDO.CdoSendUsing.cdoSendUsingPort;
                            oField = oFields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
                            oField.Value = smtpHost;
                            oField = oFields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
                            oField.Value = useSSL;
                            oField = oFields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
                            oField.Value = 1;
                            oField = oFields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
                            oField.Value = smtpUser;
                            oField = oFields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
                            oField.Value = smtpPass;
                            oFields.Update();
                            oMsg.HTMLBody = BodyEmail;
                            oMsg.Subject = EmailSubjectTitle;
                            oMsg.From = smtpUser;
                            oMsg.To = email;
                            oMsg.Send();
                            totalRecipients++;
                        }
                    }
                    InsertEmailLog("Successful", EmailSubjectTitle, EmployeeEmailAddress, "", BodyEmail);
                }
                catch 
                {
                    InsertEmailLog("Failed", EmailSubjectTitle, EmployeeEmailAddress, "Error.", BodyEmail);
                }
            }
        }

        private void InsertEmailLog(string Status, string Subject, string Receiver, string ErrorMessage, string Content)
        {
            string InsertLogSQLScript = string.Format(@"
            DECLARE @USERID int = (SELECT USERS_ID FROM Users where User_Name ='admin' )
            DECLARE @STATUS nvarchar(255) = {0}
            DECLARE @Subject nvarchar(MAX) = {1}
            DECLARE @Receiver nvarchar(MAX) = {2}
            DECLARE @User_Alert_ID int = (SELECT user_alert_ID FROM user_alert WHERE Button_Id = 'Button04227350891348ff9bf5442a8444fac4')
            DECLARE @ErrorMessage nvarchar(MAX) = {3}
            DECLARE @Content nvarchar(MAX) = {4}
           
            INSERT INTO User_Alert_Status (Creator,Creation_Time,Status,Sender,Sent_Time,Subject,Receiver,User_Alert_ID,Error_Message,content) 
            VALUES (@USERID,GETDATE(),@STATUS,@USERID,GETDATE(),@Subject,@Receiver, @User_Alert_ID)
            ", 
            ConvertStringtoSQLValue(Status), 
            ConvertStringtoSQLValue(Subject), 
            ConvertStringtoSQLValue(Receiver), 
            ConvertStringtoSQLValue(ErrorMessage), 
            ConvertStringtoSQLValue(Content)
            );
        }

        private string ConvertStringtoSQLValue(string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return "NULL";
            }
            else
            {
                return "'" + Value + "'";
            }

        }
    }
}