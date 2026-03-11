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
    public partial class GRTN_MaterialList : System.Web.UI.Page
    {
        Connection con = new Connection();
        string connectionString = ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString; // Replace with your actual connection string

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Session["GRTN_Project_ID"].ToString()) || !string.IsNullOrEmpty(Session["GRTN_Location_ID"].ToString()) || !string.IsNullOrEmpty(Session["GRTN_Quotation_ID"].ToString()))
            {
                ProjectTitle.Text = con.ExecuteSQLQueryWithOneReturn(@"SELECT Project_Name FROM Project WHERE Project_ID = " + Session["GRTN_Project_ID"].ToString()).ToString();
                if (!IsPostBack)
                {
                    Session["Material_Profile_ID"] = null;
                    BindAllDDL();
                    MaterialListGrid.DataSource = GRTN_Materialst_GV;
                    MaterialListGrid.DataBind();
                }
            }
            else
            {
                string targetUrl = "~/Modules/GRTN-Project.aspx";
                Response.Redirect(targetUrl);
            }
        }

        private void BindAllDDL()
        {
            BindCategoryDDL();
            BindBrandDDL();
            BindPartDDL();
            BindSNDDL();
            BindUOMDDL();
            Material_Profile_ID_TB.Value = "0";
            Inventory_Ledger_ID_TB.Value = "0";
            QtyTB.Text = "1";
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

            string query = string.Format(@"
            SELECT DISTINCT(Brand.Brand_Profile_ID),(Brand) 
            FROM Qtn_Consolidated_BOM BOM
            LEFT JOIN Material_Profile MP on BOM.Material_Profile_ID = MP.Material_Profile_ID
            LEFT JOIN Brand_Profile Brand on MP.Brand_Profile_ID = Brand.Brand_Profile_ID
            WHERE BOM.BOM_Type IN('Material', 'Consumable')
            AND BOM.Issued_Qty > 0
            AND BOM.Quotation_ID = {0} ORDER BY Brand.Brand ASC"
            , Session["GRTN_Quotation_ID"].ToString());

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

        private void BindUOMDDL()
        {

            string query = "select DISTINCT UOM_Profile_ID,UOM from UOM_Profile WHERE Status = 'Active' Order BY UOM ASC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlUOM.DataSource = reader;
                    ddlUOM.DataTextField = "UOM"; // Text displayed in dropdown
                    ddlUOM.DataValueField = "UOM_Profile_ID"; // Value for each item
                    ddlUOM.DataBind();

                    //// Add default "Select Reason" option at the top
                    ddlUOM.Items.Insert(0, new ListItem("-- UOM --", "0"));
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string GRTN_Project_ID = Session["GRTN_Project_ID"] as string;
            string GRTN_Quotation_ID = Session["GRTN_Quotation_ID"] as string;
            string GRTN_Location_ID = Session["GRTN_Location_ID"] as string;
            string EmployeeID = Session["EmpID"] as string;
            string AlertMessage1 = "Insuffince Balance!", AlertMessage2 = "";
            foreach (DataRow row in GRTN_Materialst_GV.Rows)
            {
                string MaterialBalanceSQLResult = con.ExecuteSQLQueryWithOneReturn(string.Format(@"
				SELECT SUM(BOM.Issued_Qty) AS Issued_Qty
                FROM Qtn_Consolidated_BOM BOM
                LEFT JOIN Material_Profile MP on BOM.Material_Profile_ID = MP.Material_Profile_ID
                LEFT JOIN Brand_Profile Brand on MP.Brand_Profile_ID = Brand.Brand_Profile_ID
                WHERE BOM.BOM_Type IN('Material', 'Consumable')
                AND BOM.Issued_Qty > 0
                AND BOM.Quotation_ID = {0} AND BOM.Material_Profile_ID = {1}"
                , GRTN_Quotation_ID, row["Material_Profile_ID"].ToString())).ToString();

                try
                {
                    decimal MaterialBalanceQTY = decimal.Parse(MaterialBalanceSQLResult);
                    if (MaterialBalanceQTY < (decimal.Parse(row["Qty"].ToString()) * decimal.Parse(row["Qty_Per_Pack"].ToString())))
                    {
                        AlertMessage2 += "\\nIssued Qty for " + row["Part_No"].ToString() + " : " + MaterialBalanceSQLResult;
                    }
                }
                catch
                {
                    throw new Exception(MaterialBalanceSQLResult);
                }
            }

            if (!string.IsNullOrEmpty(AlertMessage2))
            {
                DataTable dt = GRTN_Materialst_GV;
                MaterialListGrid.DataSource = dt;
                MaterialListGrid.DataBind();
                //BindAllDDL();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + AlertMessage1 + AlertMessage2 + "');", true);
            }
            else
            {
                string imagePath = "";

                string INSERT_NEW_GRTN_SQLScript = string.Format(@"  
                    DECLARE @newGRTNID int
                    DECLARE @EmployeeID int = {0}
                    DECLARE @ProjectID int = {1}
                    DECLARE @Remark nvarchar(4000) = '{2}'

                    INSERT INTO GRTN							
                    (							
	                    Creation_Time,						
	                    Status,						
	                    Doc_Date,						
	                    Employee_Profile_ID,						
	                    Employee_Profile_ID1,						
	                    Quotation_ID,						
	                    Remark						
                    )							
                    (							
	                    SELECT						
		                    GETDATE(),					
		                    'Submitted',					
		                    GETDATE(),					
		                    '{0}',					
		                    '{0}',					
		                    Project.Quotation_ID,					
		                    '{2}'					
	                    FROM Project						
	                    INNER JOIN Quotation ON Quotation.Quotation_ID = Project.Quotation_ID						
	                    WHERE Project.Project_ID = '{1}'					
                    )							
							
                    SET @newGRTNID = SCOPE_IDENTITY()							
                    SELECT @newGRTNID
                    ", EmployeeID, GRTN_Project_ID, Remark_TB.Text);

                string New_GRTN_ID = con.ExecuteSQLQueryWithOneReturn(INSERT_NEW_GRTN_SQLScript).ToString();

                if (fileUpload1.HasFile)
                {
                    string baseDir = ConfigurationManager.AppSettings["SystemPath"];
                    string uploadFolder = baseDir + @"Document\GRTN\File\" + New_GRTN_ID + @"\";
                    string xmlFilePath = Path.Combine(uploadFolder, "user_" + New_GRTN_ID + ".xml");

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

                string GRTNNo = con.ExecuteSQLQueryWithOneReturn(@"SELECT Doc_No FROM GRTN WHERE GRTN_ID = '" + New_GRTN_ID + @"'").ToString();

                if (string.IsNullOrEmpty(GRTNNo))
                {
                    CommonFunctions.CommonFunctions.generateGRTNNo(New_GRTN_ID, Session["EmpID"].ToString());
                }

                foreach (DataRow row in GRTN_Materialst_GV.Rows)
                {
                    string GRTN_Inventory_Ledger_ID = "NULL";
                    if (row["Inventory_Ledger_ID"].ToString() != "0")
                    {
                        GRTN_Inventory_Ledger_ID = row["Inventory_Ledger_ID"].ToString();
                    }

                    string GRTN_Material_Profile_ID = "";
                    if (row["Material_Profile_ID"].ToString() != "0")
                    {
                        GRTN_Material_Profile_ID = row["Material_Profile_ID"].ToString();
                    }

                    string GRTN_Qty = "";
                    if (row["Qty"].ToString() != "0")
                    {
                        GRTN_Qty = row["Qty"].ToString();
                    }

                    string GRTN_Qty_Per_Pack = "";
                    if (row["Qty_Per_Pack"].ToString() != "0")
                    {
                        GRTN_Qty_Per_Pack = row["Qty_Per_Pack"].ToString();
                    }

                    string GRTN_UOM_Profile_ID = "";
                    if (row["UOM_Profile_ID"].ToString() != "0")
                    {
                        GRTN_UOM_Profile_ID = row["UOM_Profile_ID"].ToString();
                    }

                    string Insert_GI_Item_SQLScript = string.Format(@"
                    DECLARE @newGRTNID int = {0}
                    DECLARE @Material_Profile_ID int = {1}
                    DECLARE @Inventory_Ledger_ID int = {2}
                    DECLARE @Qty int = {3}
                    DECLARE @Qty_Per_Pack int = {4}
                    DECLARE @UOM_Profile_ID int = {5}
                    DECLARE @Location_Profile_ID int = {6}
                       

                    INSERT INTO GRTN_Item											
                    (											
	                    Creation_Time,										
	                    GRTN_ID,										
	                    Material_Profile_ID,										
	                    Inventory_Ledger_ID,										
	                    Packaged_Qty,													
	                    Qty_Per_Pack,										
	                    Qty,
	                    UOM_Profile_ID,
	                    Location_Profile_ID										
                    )											
                    (											
                        SELECT											
                            GETDATE(),											
                            @newGRTNID,											
                            @Material_Profile_ID,											
                            @Inventory_Ledger_ID,											
                            @Qty,											
                            @Qty_Per_Pack,											
                            @Qty*@Qty_Per_Pack,											
                            @UOM_Profile_ID,											
                            @Location_Profile_ID										
                        FROM Material_Profile mat																						
                        WHERE mat.Material_Profile_ID = @Material_Profile_ID									
                    )											
                    
                   



                    ", New_GRTN_ID, GRTN_Material_Profile_ID, GRTN_Inventory_Ledger_ID, GRTN_Qty, GRTN_Qty_Per_Pack, GRTN_UOM_Profile_ID, GRTN_Location_ID);

                    con.ExecuteSQLQuery(Insert_GI_Item_SQLScript);

                }


                con.ExecuteSQLQuery(string.Format(@" 
                    INSERT INTO Material_Balance
                    (
                        Creation_Time,
                        Material_Profile_ID,
                        Qty_Per_Pack,
                        UOM_Profile_ID
                    )
                    (   
                        SELECT DISTINCT
                            GETDATE(),
                            item.Material_Profile_ID,
                            item.Qty_Per_Pack,
                            item.UOM_Profile_ID
                        FROM GRTN_Item item
                        WHERE 
                            (SELECT COUNT(1) 
                            FROM Material_Balance WITH(ROWLOCK,XLOCK) 
                            WHERE Material_Balance.Material_Profile_ID = item.Material_Profile_ID 
                            AND Material_Balance.Qty_Per_Pack = item.Qty_Per_Pack 
                            AND Material_Balance.UOM_Profile_ID = item.UOM_Profile_ID) = 0
                        AND GRTN_ID = {0}
                    )", New_GRTN_ID));

                con.ExecuteSQLQuery(string.Format(@" 
                    DECLARE @returnID int;											
                    DECLARE @matID int;											
                    DECLARE @SOID int;											
                    DECLARE @ledgerID int;											
                    DECLARE @UOMID int;											
                    DECLARE @returnQty decimal(20,2);											
                    DECLARE @issuedBatchID int;											
                    DECLARE @matUOMID int;											
											
                    DECLARE @ledgerTable TABLE											
                    (											
	                    LOFICount int,										
	                    ledgerBatchID int,										
	                    sourceBatchID int,										
	                    LOFIBalance decimal(20,2),										
	                    matBalanceID int,										
	                    costID int										
                    )											
											
                    DECLARE @balance decimal(20,2);											
                    DECLARE @LOFINumberRecords int;											
                    DECLARE @LOFIRowCounter int;											
                    DECLARE @LOFIQty decimal(20,2);											
                    DECLARE @ledgerBatchID int;											
                    DECLARE @sourceBatchID int;											
                    DECLARE @matBalanceID int;											
                    DECLARE @costID int;											
                    DECLARE @ledgerQty decimal(20,2);											
                    DECLARE @LOFIBalance decimal(20,2);											
                    DECLARE @qtyPerPack decimal(20,2);											
                    DECLARE @batchBalance decimal(20,2);											
											
                    DECLARE @numberRecords int											
                    DECLARE @rowCounter int											
											
                    DECLARE @returnTable TABLE											
                    (											
	                    RowNumber int,										
	                    GRTN_Item_ID int,										
	                    Material_Profile_ID int,										
	                    Quotation_ID int,										
	                    Inventory_Ledger_ID int,										
	                    UOM_Profile_ID int,										
	                    Return_Qty decimal(20,2),										
	                    Qty_Per_Pack decimal(20,2)										
                    )											
											
                    INSERT INTO @returnTable											
                    (											
	                    RowNumber,										
	                    GRTN_Item_ID,										
	                    Material_Profile_ID,										
	                    Quotation_ID,										
	                    Inventory_Ledger_ID,										
	                    UOM_Profile_ID,										
	                    Return_Qty,										
	                    Qty_Per_Pack										
                    )											
                    (											
	                    SELECT										
		                    ROW_NUMBER() OVER (ORDER BY item.Inventory_Ledger_ID DESC, item.GRTN_Item_ID ASC),									
		                    item.GRTN_Item_ID,									
		                    item.Material_Profile_ID,									
		                    GRTN.Quotation_ID,									
		                    item.Inventory_Ledger_ID,									
		                    item.UOM_Profile_ID,									
		                    item.Packaged_Qty,									
		                    item.Qty_Per_Pack									
	                    FROM GRTN_Item item										
	                    INNER JOIN GRTN ON GRTN.GRTN_ID = item.GRTN_ID										
	                    WHERE item.GRTN_ID = {0}										
                    ) ORDER BY item.Inventory_Ledger_ID DESC, item.GRTN_Item_ID ASC											
											
                    SET @numberRecords = @@RowCount 											
                    SET @rowCounter = 1											
											
                    WHILE @rowCounter <= @numberRecords											
                    BEGIN											
	                    SELECT 										
		                    @returnID = rtn.GRTN_Item_ID,									
		                    @matID = rtn.Material_Profile_ID,									
		                    @SOID = rtn.Quotation_ID,									
		                    @ledgerID = rtn.Inventory_Ledger_ID,									
		                    @UOMID = rtn.UOM_Profile_ID,									
		                    @returnQty = rtn.Return_Qty,									
		                    @qtyPerPack = rtn.Qty_Per_Pack,									
		                    @matUOMID = (SELECT UOM_Profile_ID FROM Material_Profile WHERE Material_Profile_ID = rtn.Material_Profile_ID)									
	                    FROM @returnTable rtn										
	                    WHERE RowNumber = @rowCounter										
											
	                    IF (ISNULL(@ledgerID,0) <> 0)										
		                    BEGIN									
			                    SET @issuedBatchID = (SELECT Batch_ID FROM Inventory_Ledger WITH(ROWLOCK,XLOCK) WHERE Inventory_Ledger_ID = @ledgerID)								
											
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
				                    UOM_Profile_ID,							
				                    Qty_Per_Pack,							
				                    Qty,							
				                    Balance,							
				                    Packaged_Balance,							
				                    Material_Balance_ID,							
				                    Material_Cost_ID							
			                    )								
			                    (								
				                    SELECT							
					                    GETDATE(),						
					                    'IN',						
					                    'Goods Return',						
					                    GRTN.Doc_No,						
					                    GRTN.Doc_Date,						
					                    item.GRTN_Item_ID,						
					                    GRTN.GRTN_ID,						
					                    GRTN.Quotation_ID,						
					                    GRTN.Service_Maintenance_ID,						
					                    item.Material_Profile_ID,						
					                    item.Location_Profile_ID,						
					                    ledger.Serial_No,						
					                    item.Packaged_Qty,						
					                    item.UOM_Profile_ID,						
					                    item.Qty_Per_Pack,						
					                    item.Qty,						
					                    0,						
					                    0,						
					                    ledger.Material_Balance_ID,						
					                    ledger.Material_Cost_ID						
				                    FROM GRTN_Item item							
				                    INNER JOIN Inventory_Ledger ledger WITH(ROWLOCK,XLOCK) ON ledger.Inventory_Ledger_ID = item.Inventory_Ledger_ID							
				                    INNER JOIN GRTN ON GRTN.GRTN_ID = item.GRTN_ID							
				                    WHERE item.GRTN_Item_ID = @returnID							
			                    )								
											
			                    INSERT INTO Inventory_Location_History								
			                    (								
				                    Creation_Time,							
				                    Inventory_Ledger_ID,							
				                    Location_Profile_ID							
			                    )								
			                    (								
				                    SELECT							
					                    GETDATE(),						
					                    Inventory_Ledger_ID,						
					                    Location_Profile_ID						
				                    FROM Inventory_Ledger							
				                    WHERE Inventory_Ledger_ID = @issuedBatchID							
						                    AND ISNULL(Location_Profile_ID,0) <> (SELECT Location_Profile_ID FROM GRTN_Item WHERE GRTN_Item_ID = @returnID)					
			                    )								
											
			                    UPDATE Inventory_Ledger								
			                    SET Balance = @returnQty * Qty_Per_Pack,								
				                    Packaged_Balance = @returnQty,							
				                    Location_Profile_ID = (SELECT Location_Profile_ID FROM GRTN_Item WHERE GRTN_Item_ID = @returnID)							
			                    WHERE Inventory_Ledger_ID = @issuedBatchID								
											
			                    UPDATE Inventory_Ledger								
			                    SET Actual_Qty = Actual_Qty - @returnQty								
			                    WHERE Inventory_Ledger_ID = @ledgerID								
		                    END									
											
	                    IF (ISNULL(@ledgerID,0) = 0)										
		                    BEGIN									
			                    --- For inventorize item ---								
			                    IF (@UOMID = @matUOMID)								
				                    BEGIN							
					                    SET @balance = @returnQty						
											
					                    DELETE FROM @ledgerTable						
											
					                    INSERT INTO @ledgerTable						
					                    (						
						                    LOFICount,					
						                    ledgerBatchID,					
						                    sourceBatchID,					
						                    LOFIBalance,					
						                    matBalanceID,					
						                    costID					
					                    )						
					                    (						
						                    SELECT					
							                    ROW_NUMBER() OVER (ORDER BY ledger.Inventory_Ledger_ID DESC),				
							                    ledger.Inventory_Ledger_ID,				
							                    ledger.Batch_ID,				
							                    ledger.Actual_Qty,				
							                    ledger.Material_Balance_ID,				
							                    ledger.Material_Cost_ID				
						                    FROM Inventory_Ledger ledger WITH(ROWLOCK,XLOCK)					
						                    INNER JOIN Material_Profile mat ON mat.UOM_Profile_ID = ledger.UOM_Profile_ID AND mat.Material_Profile_ID = ledger.Material_Profile_ID					
						                    WHERE ledger.Material_Profile_ID = @matID					
								                    AND ledger.Quotation_ID = @SOID			
								                    AND ledger.In_Out = 'OUT'			
								                    AND ledger.Actual_Qty > 0			
					                    ) ORDER BY ledger.Inventory_Ledger_ID DESC						
											
					                    SET @LOFINumberRecords = @@RowCount 						
					                    SET @LOFIRowCounter = 1						
											
					                    WHILE @balance > 0 AND @LOFIRowCounter <= @LOFINumberRecords						
					                    BEGIN						
						                    SELECT					
							                    @LOFIQty = LOFIBalance,				
							                    @ledgerBatchID = ledgerBatchID,				
							                    @sourceBatchID = sourceBatchID,				
							                    @matBalanceID = matBalanceID,				
							                    @costID = costID				
						                    FROM @ledgerTable					
						                    WHERE LOFICount = @LOFIRowCounter					
											
						                    IF (@balance > @LOFIQty)					
							                    BEGIN				
								                    SET @batchBalance = @LOFIQty			
								                    SET @ledgerQty = @LOFIQty			
								                    SET @balance = @balance - @LOFIQty			
								                    SET @LOFIBalance = 0			
							                    END				
						                    ELSE					
							                    BEGIN				
								                    SET @batchBalance = @balance			
								                    SET @ledgerQty = @balance			
								                    SET @LOFIBalance = @LOFIQty - @balance			
								                    SET @balance = 0			
							                    END				
											
						                    UPDATE Inventory_Ledger					
						                    SET Actual_Qty = @LOFIBalance					
						                    WHERE Inventory_Ledger_ID = @ledgerBatchID					
											
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
							                    Packaged_Qty,				
							                    UOM_Profile_ID,				
							                    Qty_Per_Pack,				
							                    Qty,				
							                    Balance,				
							                    Packaged_Balance,				
							                    Material_Balance_ID,				
							                    Material_Cost_ID,				
							                    Batch_ID				
						                    )					
						                    (					
							                    SELECT				
								                    GETDATE(),			
								                    'IN',			
								                    'Goods Return',			
								                    GRTN.Doc_No,			
								                    GRTN.Doc_Date,			
								                    item.GRTN_Item_ID,			
								                    GRTN.GRTN_ID,			
								                    GRTN.Quotation_ID,			
								                    GRTN.Service_Maintenance_ID,			
								                    item.Material_Profile_ID,			
								                    item.Location_Profile_ID,			
								                    @ledgerQty,			
								                    item.UOM_Profile_ID,			
								                    item.Qty_Per_Pack,			
								                    @ledgerQty * item.Qty_Per_Pack,			
								                    @ledgerQty * item.Qty_Per_Pack,			
								                    @ledgerQty,			
								                    @matBalanceID,			
								                    @costID,			
								                    @ledgerBatchID			
							                    FROM GRTN_Item item				
							                    INNER JOIN GRTN ON GRTN.GRTN_ID = item.GRTN_ID				
							                    WHERE item.GRTN_Item_ID = @returnID				
						                    )					
											
						                    SET @LOFIRowCounter = @LOFIRowCounter + 1					
					                    END						
				                    END							
			                    ELSE								
				                    BEGIN							
					                    SET @matBalanceID = (SELECT Material_Balance_ID FROM Material_Balance WHERE Material_Profile_ID = @matID AND Qty_Per_Pack = @qtyPerPack AND UOM_Profile_ID = @UOMID)						
											
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
						                    Packaged_Qty,					
						                    UOM_Profile_ID,					
						                    Qty_Per_Pack,					
						                    Qty,					
						                    Balance,					
						                    Packaged_Balance,					
						                    Material_Balance_ID					
					                    )						
					                    (						
						                    SELECT					
							                    GETDATE(),				
							                    'IN',				
							                    'Goods Return',				
							                    GRTN.Doc_No,				
							                    GRTN.Doc_Date,				
							                    item.GRTN_Item_ID,				
							                    GRTN.GRTN_ID,				
							                    GRTN.Quotation_ID,				
							                    GRTN.Service_Maintenance_ID,				
							                    item.Material_Profile_ID,				
							                    item.Location_Profile_ID,				
							                    item.Packaged_Qty,				
							                    item.UOM_Profile_ID,				
							                    item.Qty_Per_Pack,				
							                    item.Qty,				
							                    item.Qty,				
							                    item.Packaged_Qty,				
							                    @matBalanceID				
						                    FROM GRTN_Item item					
						                    INNER JOIN GRTN ON GRTN.GRTN_ID = item.GRTN_ID					
						                    WHERE item.GRTN_Item_ID = @returnID					
					                    )						
				                    END							
		                    END									
											
	                    UPDATE GRTN_Item										
	                    SET Material_Balance_ID = balance.Material_Balance_ID										
	                    FROM GRTN_Item item										
	                    INNER JOIN Material_Balance balance ON balance.UOM_Profile_ID = item.UOM_Profile_ID 
                        AND balance.Qty_Per_Pack = item.Qty_Per_Pack AND balance.Material_Profile_ID = item.Material_Profile_ID										
	                    WHERE item.GRTN_Item_ID = @returnID										
											
	                    SET @rowCounter = @rowCounter + 1										
                    END											
                    ", New_GRTN_ID));

                string Quotation_ID = con.ExecuteSQLQueryWithOneReturn("SELECT Quotation_ID FROM GRTN WHERE GRTN_ID =" + New_GRTN_ID).ToString();
                string Combine_Material_ID = con.ExecuteSQLQueryWithOneReturn("SELECT STUFF((SELECT DISTINCT ', ' + CONVERT(nvarchar(50), Material_Profile_ID) FROM GRTN_Item WHERE GRTN_ID = " + New_GRTN_ID + " FOR XML PATH('')), 1, 1, '')").ToString();
                Insert_SOConsolidatedBOM_Procedure(New_GRTN_ID);
                Insert_ProjectConsolidatedBOM_Procedure(New_GRTN_ID);
                Update_SOBOMPRQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMIssuedReturnedPOQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMReservedQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_SOBOMQty_Procedure(Quotation_ID, Combine_Material_ID);
                Update_ProjectBOMQty_Procedure(GRTN_Project_ID);

                string GetAllInfoFROMGRTNQuery = string.Format(@"
                    SELECT 
                    Material_Balance_ID, 
                    Location_Profile_ID,  
                    Material_Profile_ID, 
                    UOM_Profile_ID, 
                    Qty_Per_Pack 
                    FROM GRTN_Item WHERE GRTN_ID = {0}"
                , New_GRTN_ID);

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
                string targetUrl = "~/Modules/Menu.aspx";
                Response.Redirect(targetUrl);
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string GRTN_Quotation_ID = Session["GRTN_Quotation_ID"] as string;
            string Material_Profile_ID = Material_Profile_ID_TB.Value;
            string Select_UOM_Profile_ID = Select_UOM_Profile_ID_TB.Value;
            string Inventory_Ledger_ID = Inventory_Ledger_ID_TB.Value;
            string Part_No_D = con.ExecuteSQLQueryWithOneReturn("Select Part_No FROM Material_Profile WHERE Material_Profile_ID = " + Material_Profile_ID).ToString();
            string AlertMessage1 = "Insuffince Balance!", AlertMessage2 = "";
            bool duplicateFound = false;

            string MaterialBalanceSQLResult = con.ExecuteSQLQueryWithOneReturn(string.Format(@"
				SELECT SUM(BOM.Issued_Qty) AS Issued_Qty
                FROM Qtn_Consolidated_BOM BOM
                LEFT JOIN Material_Profile MP on BOM.Material_Profile_ID = MP.Material_Profile_ID
                LEFT JOIN Brand_Profile Brand on MP.Brand_Profile_ID = Brand.Brand_Profile_ID
                WHERE BOM.BOM_Type IN('Material', 'Consumable')
                AND BOM.Issued_Qty > 0
                AND BOM.Quotation_ID = {0} AND BOM.Material_Profile_ID = {1} "
               , GRTN_Quotation_ID, Material_Profile_ID)).ToString();

            try
            {
                decimal MaterialBalanceQTY = decimal.Parse(MaterialBalanceSQLResult);
                if (MaterialBalanceQTY < (decimal.Parse(QtyTB.Text) * decimal.Parse(QtyPerPackTB.Text)))
                {
                    AlertMessage2 += "\\nIssued Qty for " + Part_No_D + " : " + MaterialBalanceSQLResult;
                }
            }
            catch
            {
                throw new Exception(MaterialBalanceSQLResult);
            }

            if (!string.IsNullOrEmpty(AlertMessage2))
            {
                DataTable dt = GRTN_Materialst_GV;
                MaterialListGrid.DataSource = dt;
                MaterialListGrid.DataBind();
                //BindAllDDL();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + AlertMessage1 + AlertMessage2 + "');", true);
            }
            else
            {

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
                    DataTable dt = GRTN_Materialst_GV;
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
                    string QTY_Per_Pack = QtyPerPackTB.Text;
                    string UOM = con.ExecuteSQLQueryWithOneReturn("select UOM from UOM_Profile LEFT JOIN Material_Profile on  UOM_Profile.UOM_Profile_ID = Material_Profile.UOM_Profile_ID WHERE Material_Profile_ID = " + Material_Profile_ID).ToString();
                    string Selected_UOM = con.ExecuteSQLQueryWithOneReturn("Select UOM FROM UOM_Profile WHERE UOM_Profile_ID = " + Select_UOM_Profile_ID).ToString();

                    if (Inventory_Ledger_ID != "0" || Inventory_Ledger_ID == "")
                    {
                        Serial_No = con.ExecuteSQLQueryWithOneReturn(@"Select 'SN : ' + Serial_No FROM Inventory_Ledger WHERE Inventory_Ledger_ID = " + Inventory_Ledger_ID).ToString();
                    }

                    DataTable dt = GRTN_Materialst_GV;
                    dt.Rows.Add(Part_No, Part_Description, Serial_No, QTY, QTY_Per_Pack, UOM, Selected_UOM, Material_Profile_ID, Inventory_Ledger_ID, Select_UOM_Profile_ID);
                    GRTN_Materialst_GV = dt;
                    MaterialListGrid.DataSource = dt;
                    MaterialListGrid.DataBind();
                    Session["Material_Profile_ID"] = Material_Profile_ID;
                    //BindAllDDL();
                    btnSubmit.Visible = true;
                }
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetddlPart(string BrandID)
        {
            string GRTN_Quotation_ID = HttpContext.Current.Session["GRTN_Quotation_ID"] as string;
            List<object> Material_Part = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"

                SELECT Material_Profile_ID, Part_No From (
                    SELECT DISTINCT(MP.Material_Profile_ID), (MP.Part_No + ' / ' + CONVERT(nvarchar(max), MP.Description) + '[' + UP.UOM + ']') AS Part_No
                    FROM Qtn_Consolidated_BOM BOM
                    LEFT JOIN Material_Profile MP on BOM.Material_Profile_ID = MP.Material_Profile_ID
                    LEFT JOIN UOM_Profile UP ON MP.UOM_Profile_ID = UP.UOM_Profile_ID
                    LEFT JOIN Brand_Profile Brand on MP.Brand_Profile_ID = Brand.Brand_Profile_ID
                    WHERE BOM.BOM_Type IN('Material', 'Consumable')
                    AND BOM.Issued_Qty > 0
                    AND BOM.Quotation_ID = '{0}' 
                    AND Brand.Brand_Profile_ID = '{1}' 
                ) A
                ORDER BY A.Part_No ASC
                ", GRTN_Quotation_ID, BrandID);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Material_Part.Add(new
                    {
                        Material_Profile_ID = reader["Material_Profile_ID"].ToString(),
                        Part_No = reader["Part_No"].ToString()
                    });
                }
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(Material_Part);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetddlSerialNo(string PartID)
        {
            string GRTN_Quotation_ID = HttpContext.Current.Session["GRTN_Quotation_ID"] as string;
            List<object> Material_SerialNo = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"
                SELECT 
	                Inventory_Ledger.Inventory_Ledger_ID, 
	                Inventory_Ledger.Serial_No 
                FROM Inventory_Ledger 
                WHERE Inventory_Ledger.Material_Profile_ID = {1}
                AND Inventory_Ledger.In_Out = 'Out' 
                AND Inventory_Ledger.Quotation_ID = {0}
                AND Inventory_Ledger.Actual_Qty > 0
                AND Inventory_Ledger.Serial_No IS NOT NULL
                ", GRTN_Quotation_ID, PartID);
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
            string GR_Location_ID = HttpContext.Current.Session["GRTN_Location_ID"] as string;
            string GRTN_Quotation_ID = HttpContext.Current.Session["GRTN_Quotation_ID"] as string;
            List<object> Material_SerialNo = new List<object>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Comnet"].ConnectionString))
            {
                string sql = string.Format(@"
                SELECT 
	                UOM_Profile.UOM_Profile_ID, 
	                UOM_Profile.UOM,
                    UOM_Profile.Loose_Packaging,
            		UOMTable.Sequence
	                FROM UOM_Profile 
	                INNER JOIN 
		                (SELECT Material_Profile.UOM_Profile_ID, 1 AS Sequence 
		                FROM GI_Item 
                        LEFT JOIN GI ON GI.GI_ID = GI_Item.GI_ID
                        LEFT JOIN Material_Profile ON GI_Item.Material_Profile_ID = Material_Profile.Material_Profile_ID
		                WHERE Material_Profile.Material_Profile_ID = {0} and GI.Project_Quotation_ID = {1}
		                UNION ALL 
		                SELECT looseUOM.UOM_Profile_ID, 2 AS Sequence 
		                FROM UOM_Profile looseUOM 
		                CROSS JOIN 
			                (SELECT UOM_Profile.UOM_Profile_ID, UOM_Profile.Loose_Packaging 
			                FROM Material_Profile 
			                INNER JOIN UOM_Profile ON UOM_Profile.UOM_Profile_ID = Material_Profile.UOM_Profile_ID 
			                WHERE Material_Profile.Material_Profile_ID = {0}) matTable 
		                WHERE looseUOM.Standard_Loose = CASE WHEN (matTable.Loose_Packaging = 'Y') THEN 'Loose' ELSE NULL END) UOMTable 
		                ON UOMTable.UOM_Profile_ID = UOM_Profile.UOM_Profile_ID 
	                WHERE (UOM_Profile.UOM IS NOT NULL) 
	                AND (ISNULL(UOM_Profile.Availability, 1) = 1) 
                ORDER BY UOMTable.Sequence ASC, UOM_Profile.UOM ASC		

                ", PartID, GRTN_Quotation_ID);
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Material_SerialNo.Add(new
                    {
                        UOM_Profile_ID = reader["UOM_Profile_ID"].ToString(),
                        UOM = reader["UOM"].ToString(),
                        Sequence = reader["Sequence"].ToString(),
                        Loose_Packaging = reader["Loose_Packaging"].ToString()
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
                foreach (DataRow row in GRTN_Materialst_GV.Rows)
                {
                    if (row["Material_Profile_ID"].ToString() == id)
                    {
                        GRTN_Materialst_GV.Rows.Remove(row);
                        break;
                    }
                }

                MaterialListGrid.DataSource = GRTN_Materialst_GV;
                MaterialListGrid.DataBind();
            }
        }

        private DataTable GRTN_Materialst_GV
        {
            get
            {
                if (ViewState["GRTN_Materialst_GV"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Part_No");
                    dt.Columns.Add("Part_Description");
                    dt.Columns.Add("Serial_No");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("Qty_Per_Pack");
                    dt.Columns.Add("UOM");
                    dt.Columns.Add("Selected_UOM");
                    dt.Columns.Add("Material_Profile_ID");
                    dt.Columns.Add("Inventory_Ledger_ID");
                    dt.Columns.Add("UOM_Profile_ID");
                    DataColumn customerNoCol = new DataColumn("S_N", typeof(int));
                    customerNoCol.AutoIncrement = true;
                    customerNoCol.AutoIncrementSeed = 1;
                    customerNoCol.AutoIncrementStep = 1;
                    dt.Columns.Add(customerNoCol);
                    ViewState["GRTN_Materialst_GV"] = dt;
                }
                return (DataTable)ViewState["GRTN_Materialst_GV"];
            }
            set
            {
                ViewState["GRTN_Materialst_GV"] = value;
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
            decimal decimalValue;
            if (decimal.TryParse(Qty_Per_Pack, out decimalValue))
            {
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
                            command.Parameters.Add(new SqlParameter("@qtyPerPack", SqlDbType.Decimal)).Value = decimalValue;
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

        private void Update_ProjectBOMQty_Procedure(string Project_ID)
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

    }
}