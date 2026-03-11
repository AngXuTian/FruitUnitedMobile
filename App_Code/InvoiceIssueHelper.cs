// File: App_Code/InvoiceProcessor.cs
// Place this file in the App_Code folder of your ASP.NET project. It will be automatically compiled and available.

using System;
using System.Configuration;
using System.Data.SqlClient;

namespace FruitUnitedMobile.InvoiceIssueHelper
{
    public class InvoiceIssue
    {
        private static string connString = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

        // Note: These methods assume a connection and transaction are passed in, as they are part of a larger transaction.
        // They execute the provided SQL scripts as batches.

        public static int InsertInvoice(
    string docNo,
    int dailyLoadingId,
    int outletId,
    DateTime docDate,
    int creator,
    decimal subTotal,       // ← keep parameter name for clarity
    decimal taxAmount,
    decimal grandTotal,     // ← keep parameter name
    int vehicleId,
    int driverId,
	int driver2Id,
	int currencyId,
    double exchangeRate,
    int taxId,
    string recipient,
    string poNo,
    DateTime? poDate,
    int customerId,
    SqlConnection con,
    SqlTransaction trans)
        {
            string sql = @"
        INSERT INTO Invoice
        (
            Doc_No, 
            Daily_Loading_ID, 
            Outlet_Profile_ID, 
            Doc_Date, 
            Doc_Time,
            Creator, 
            Creation_Time, 
            Status, 
            Amount,           -- ← changed from Sub_Total
            Tax_Amount, 
            Total_Amount,     -- ← changed from Grand_Total
            Vehicle_Profile_ID, 
            Employee_Profile_ID, 
			Employee_Profile_ID1,
            Currency_Profile_ID, 
            Exchange_Rate,
            Tax_Profile_ID, 
            Received_By, 
            PO_No, 
            PO_Date, 
            Customer_Profile_ID,
			Doc_Type,
			Revision
        )
        VALUES
        (
            @DocNo, 
            @DailyLoadingID, 
            @OutletID, 
            @DocDate, 
            GETDATE(),
            @Creator, 
            GETDATE(), 
            'Issued', 
            @SubTotal,        -- maps to Amount column
            @TaxAmount, 
            @GrandTotal,      -- maps to Total_Amount column
            @VehicleID, 
            @DriverID, 
			@Driver2ID, 
            @CurrencyID, 
            @ExchangeRate,
            @TaxID, 
            @Recipient, 
            @PONo, 
            @PODate, 
            @CustomerID,
			'Product',
			0
        );
        SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@DocNo", docNo);
                cmd.Parameters.AddWithValue("@DailyLoadingID", dailyLoadingId);
                cmd.Parameters.AddWithValue("@OutletID", outletId);
                cmd.Parameters.AddWithValue("@DocDate", docDate);
                cmd.Parameters.AddWithValue("@Creator", creator);
                cmd.Parameters.AddWithValue("@SubTotal", subTotal);     // goes into Amount
                cmd.Parameters.AddWithValue("@TaxAmount", taxAmount);
                cmd.Parameters.AddWithValue("@GrandTotal", grandTotal); // goes into Total_Amount
                cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                cmd.Parameters.AddWithValue("@DriverID", driverId);
				cmd.Parameters.AddWithValue("@Driver2ID", driver2Id == 0 ? (object)DBNull.Value : driver2Id);
				cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                cmd.Parameters.AddWithValue("@ExchangeRate", exchangeRate);
                cmd.Parameters.AddWithValue("@TaxID", taxId);
                cmd.Parameters.AddWithValue("@Recipient", recipient ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PONo", poNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PODate", poDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static int InsertInvoiceItem(
            SqlConnection con,                      // required - move up
            SqlTransaction trans,                   // required - move up
            int invoiceId,
            int productId,
            decimal quantity,
            decimal unitPrice,
            decimal amount,
            string itemType,
            string status,
            decimal balance,
            int? offsetOutstanding = null,
            int? exchangeReasonId = null,
            int? productProfileId1 = null,
            decimal? replacementQuantity = null)
        {
            string sql = @"
                INSERT INTO Invoice_Items
                (
                    Invoice_ID, Product_Profile_ID, Quantity, Unit_Price, Amount, Item_Type,
                    Status, Balance, Offset_Outstanding, Exchange_Reason_ID,
                    Product_Profile_ID1, Replacement_Quantity, Creation_Time
                )
                VALUES
                (
                    @InvoiceID, @ProductID, @Quantity, @UnitPrice, @Amount, @ItemType,
                    @Status, @Balance,
                    @OffsetOutstanding, @ExchangeReasonID,
                    @ProductProfileID1, @ReplacementQuantity, GETDATE()
                );
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@ItemType", itemType);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Balance", balance);
                cmd.Parameters.AddWithValue("@OffsetOutstanding", offsetOutstanding ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExchangeReasonID", exchangeReasonId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductProfileID1", productProfileId1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReplacementQuantity", replacementQuantity ?? (object)DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }



        public static void UpdateInvoiceOutstanding(int invoiceItemId, SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
/**Execute with Item Type = Outstanding**/

UPDATE Invoice_Items
SET Status = 'Outstanding',
	Balance = Quantity
WHERE Invoice_Items_ID = @InvoiceItemID
    AND Item_Type = 'Outstanding'";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@InvoiceItemID", invoiceItemId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertIntoLoadingMovement(int invoiceItemId, int userId, SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
DECLARE @itemID int;
	DECLARE @InvoiceID int;
	DECLARE @productID int;
	DECLARE @loadingID int;
	DECLARE @outletID int;
	DECLARE @movementID int;
	DECLARE @quantity decimal(20,2);
	DECLARE @balanceQty decimal(20,2);
	DECLARE @ledgerID int;
	DECLARE @fifoQty decimal(20,2);

    SET @itemID = @InvoiceItemID;
	SET @InvoiceID = (SELECT Invoice_ID FROM Invoice_Items WHERE Invoice_Items_ID = @itemID);
	SET @quantity = (SELECT Quantity FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)
	SET @productID = (SELECT Product_Profile_ID FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)

	IF ((SELECT Product_Profile_ID1 FROM Invoice_Items WHERE Invoice_Items_ID = @itemID AND Item_Type = 'Exchange') IS NOT NULL)
		BEGIN
			SET @quantity = (SELECT Replacement_Quantity FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)
			SET @productID = (SELECT Product_Profile_ID1 FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)
		END

	SET @loadingID = (SELECT Daily_Loading_ID FROM Invoice WHERE Invoice_ID = @InvoiceID)

	IF ((SELECT Item_Type FROM Invoice_Items WHERE Invoice_Items_ID = @itemID) NOT IN ('Outstanding','Miscellaneous'))
		BEGIN
			SET @balanceQty = @quantity

			DECLARE fifoCursor CURSOR FOR 
			SELECT item.Balance, item.Daily_Loading_Items_ID, item.Product_Movement_ID
			FROM Daily_Loading_Items item WITH(ROWLOCK,XLOCK)
			LEFT JOIN Product_Movement movement ON movement.Product_Movement_ID = item.Product_Movement_ID
			INNER JOIN Daily_Loading loading WITH(ROWLOCK,XLOCK) ON loading.Daily_Loading_ID = item.Daily_Loading_ID
			WHERE loading.Daily_Loading_ID = @loadingID
					AND item.Product_Profile_ID = @productID
					AND item.Balance > 0
			ORDER BY ISNULL(movement.Doc_Date,GETDATE()) ASC

			OPEN fifoCursor;

			FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID, @movementID;

			WHILE @@FETCH_STATUS = 0
			BEGIN

				IF (@balanceQty = 0)
					BREAK;

				IF (@balanceQty > (SELECT Balance FROM Daily_Loading_Items WHERE Daily_Loading_Items_ID = @ledgerID))
					BEGIN
						FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID, @movementID;
					END

				IF (@balanceQty <= @fifoQty)
					BEGIN
						INSERT INTO Daily_Loading_Movement
						(
							Creator,
							Creation_Time,
							Daily_Loading_ID,
							Doc_Date,
							Doc_Time, 
							Doc_Type,
							Direction,
							Reference_No,
							Outlet_Profile_ID,
							Reference_ID,
							Product_Movement_ID,
							Product_Profile_ID,
							Quantity,
							Outgoing_Batch_ID
						)
						(
							SELECT
								@UserID,
								GETDATE(),
								Invoice.Daily_Loading_ID,
								Invoice.Doc_Date,
								Invoice.Doc_Time,
								'Invoice - ' + Invoice_Items.Item_Type,
								'OUT',
								Invoice.Doc_No,
								Invoice.Outlet_Profile_ID,
								Invoice_Items.Invoice_Items_ID,
								@movementID,
								Invoice_Items.Product_Profile_ID,
								@balanceQty,
								@ledgerID
							FROM Invoice_Items
							INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
							WHERE Invoice_Items.Invoice_Items_ID = @itemID
						)

						UPDATE Daily_Loading_Items
						SET Balance = @fifoQty - @balanceQty
						WHERE Daily_Loading_Items_ID = @ledgerID
                    
						SET @balanceQty = 0

						BREAK;
					END

				IF (@balanceQty > @fifoQty)
					BEGIN
						INSERT INTO Daily_Loading_Movement
						(
							Creator,
							Creation_Time,
							Daily_Loading_ID,
							Doc_Date, 
							Doc_Time,
							Doc_Type,
							Direction,
							Reference_No,
							Outlet_Profile_ID,
							Reference_ID,
							Product_Movement_ID,
							Product_Profile_ID,
							Quantity,
							Outgoing_Batch_ID
						)
						(
							SELECT
								@UserID,
								GETDATE(),
								Invoice.Daily_Loading_ID,
								Invoice.Doc_Date,
								Invoice.Doc_Time,
								'Invoice - ' + Invoice_Items.Item_Type,
								'OUT',
								Invoice.Doc_No,
								Invoice.Outlet_Profile_ID,
								Invoice_Items.Invoice_Items_ID,
								@movementID,
								Invoice_Items.Product_Profile_ID,
								@fifoQty,
								@ledgerID
							FROM Invoice_Items
							INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
							WHERE Invoice_Items.Invoice_Items_ID = @itemID
						)

						UPDATE Daily_Loading_Items
						SET Balance = 0
						WHERE Daily_Loading_Items_ID = @ledgerID

						SET @balanceQty = @balanceQty - @fifoQty

						FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID, @movementID;
					END

			FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID, @movementID;
			END
			CLOSE fifoCursor;
			DEALLOCATE fifoCursor;

			IF (@balanceQty > 0)
				BEGIN
					INSERT INTO Daily_Loading_Movement
					(
						Creator,
						Creation_Time,
						Daily_Loading_ID,
						Doc_Date,
						Doc_Time, 
						Doc_Type,
						Direction,
						Reference_No,
						Outlet_Profile_ID,
						Reference_ID,
						Product_Profile_ID,
						Quantity
					)
					(
						SELECT
							@UserID,
							GETDATE(),
							Invoice.Daily_Loading_ID,
							Invoice.Doc_Date,
							Invoice.Doc_Time,
							'Invoice - ' + Invoice_Items.Item_Type,
							'OUT',
							Invoice.Doc_No,
							Invoice.Outlet_Profile_ID,
							Invoice_Items.Invoice_Items_ID,
							Invoice_Items.Product_Profile_ID,
							@balanceQty
						FROM Invoice_Items
						INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
						WHERE Invoice_Items.Invoice_Items_ID = @itemID
					)
				END
	END";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@InvoiceItemID", invoiceItemId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeductOffsetOutstandingDelivery(int invoiceItemId, int userId, SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
DECLARE @itemID int;
DECLARE @InvoiceID int;
DECLARE @productID int;
DECLARE @outletID int;
DECLARE @quantity decimal(20,2);
DECLARE @balanceQty decimal(20,2);
DECLARE @ledgerID int;
DECLARE @fifoQty decimal(20,2);

SET @itemID = @InvoiceItemID;
SET @InvoiceID = (SELECT Invoice_ID FROM Invoice_Items WHERE Invoice_Items_ID = @itemID);
SET @quantity = (SELECT Quantity FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)
SET @productID = (SELECT Product_Profile_ID FROM Invoice_Items WHERE Invoice_Items_ID = @itemID)
SET @outletID = (SELECT Outlet_Profile_ID FROM Invoice WHERE Invoice_ID = @InvoiceID)

IF ((SELECT COUNT(1) FROM Invoice_Items WHERE Invoice_Items_ID = @itemID AND Item_Type = 'Exchange' AND Offset_Outstanding = 1) > 0)
	BEGIN
		SET @balanceQty = @quantity

		DECLARE fifoCursor CURSOR FOR 
		SELECT Invoice_Items.Balance, Invoice_Items.Invoice_Items_ID 
		FROM Invoice_Items WITH(ROWLOCK,XLOCK)
		INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
		WHERE Invoice_Items.Item_Type = 'Outstanding'
				AND Invoice_Items.Status = 'Outstanding'
				AND Invoice_Items.Balance > 0
				AND Invoice_Items.Product_Profile_ID = @productID
				AND Invoice.Outlet_Profile_ID = @outletID
		ORDER BY Invoice.Doc_Date ASC, Invoice_Items.Invoice_Items_ID ASC

		OPEN fifoCursor;

		FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID;

		WHILE @@FETCH_STATUS = 0
		BEGIN

			IF (@balanceQty = 0)
				BREAK;

			IF (@balanceQty <= @fifoQty)
				BEGIN
					INSERT INTO Return_Batch
					(
						Creator,
						Creation_Time,
						Doc_No,
						Doc_Date,
						Doc_Type,
						Main_Reference_ID,
						Quantity,
						Reference_ID,
						Invoice_Items_ID
					)
					(
						SELECT
							@UserID,
							GETDATE(),
							Invoice.Doc_No,
							Invoice.Doc_Date,
							'Invoice',
							Invoice_Items.Invoice_ID,
							@balanceQty,
							Invoice_Items.Invoice_Items_ID,
							@ledgerID
						FROM Invoice_Items
						INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
						WHERE Invoice_Items.Invoice_Items_ID = @itemID
					)

					UPDATE Invoice_Items
					SET Balance = @fifoQty - @balanceQty
					WHERE Invoice_Items_ID = @ledgerID

					UPDATE Invoice_Items
					SET Status = 'Completed'
					WHERE Invoice_Items_ID = @ledgerID
							AND Balance = 0
                    
					SET @balanceQty = 0

					BREAK;
				END

			IF (@balanceQty > @fifoQty)
				BEGIN
					INSERT INTO Return_Batch
					(
						Creator,
						Creation_Time,
						Doc_No,
						Doc_Date,
						Doc_Type,
						Main_Reference_ID,
						Quantity,
						Reference_ID,
						Invoice_Items_ID
					)
					(
						SELECT
							@UserID,
							GETDATE(),
							Invoice.Doc_No,
							Invoice.Doc_Date,
							'Invoice',
							Invoice_Items.Invoice_ID,
							@fifoQty,
							Invoice_Items.Invoice_Items_ID,
							@ledgerID
						FROM Invoice_Items
						INNER JOIN Invoice ON Invoice.Invoice_ID = Invoice_Items.Invoice_ID
						WHERE Invoice_Items.Invoice_Items_ID = @itemID
					)

					UPDATE Invoice_Items
					SET Balance = 0
					WHERE Invoice_Items_ID = @ledgerID

					UPDATE Invoice_Items
					SET Status = 'Completed'
					WHERE Invoice_Items_ID = @ledgerID
							AND Balance = 0

					SET @balanceQty = @balanceQty - @fifoQty

					FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID;
				END

		FETCH NEXT FROM fifoCursor INTO @fifoQty, @ledgerID;
		END
		CLOSE fifoCursor;
		DEALLOCATE fifoCursor;
END";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@InvoiceItemID", invoiceItemId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateProductBatchBalance(int dailyLoadingId, SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
DECLARE @outQty decimal(20,4);
DECLARE @batchID int;
DECLARE @productID int;
DECLARE @locationID int;
DECLARE @balance decimal(20,4);

DECLARE itemCursor CURSOR FOR 
SELECT Product_Movement_ID FROM Daily_Loading_Items
WHERE Daily_Loading_ID = @DailyLoadingID

OPEN itemCursor;

FETCH NEXT FROM itemCursor INTO @batchID

WHILE @@FETCH_STATUS = 0
BEGIN

SET @outQty = ISNULL((SELECT SUM(Out_Qty) FROM Product_Movement WITH(ROWLOCK,XLOCK) 
                WHERE Outgoing_Batch_ID = @batchID
                        AND Out_Qty IS NOT NULL),0)
            
UPDATE Product_Movement
SET Balance = In_Qty - @outQty
WHERE Product_Movement_ID = @batchID

FETCH NEXT FROM itemCursor INTO @batchID
END
CLOSE itemCursor;
DEALLOCATE itemCursor;

DECLARE movementCursor CURSOR FOR 
SELECT DISTINCT Product_Profile_ID, Location_Profile_ID FROM Daily_Loading_Items
WHERE Daily_Loading_ID = @DailyLoadingID

OPEN movementCursor;

FETCH NEXT FROM movementCursor INTO @productID, @locationID

WHILE @@FETCH_STATUS = 0
BEGIN

SET @balance = ISNULL((SELECT SUM(Product_Movement.Balance) 
                                    FROM Product_Movement WITH(ROWLOCK,XLOCK)
                                    INNER JOIN Transaction_Type ON Transaction_Type.Transaction_Type_ID = Product_Movement.Transaction_Type_ID 
                                    WHERE Product_Movement.Product_Profile_ID = @productID 
                                            AND Product_Movement.Location_Profile_ID = @locationID 
                                            AND Transaction_Type.Direction = 'IN'),0)
            
UPDATE Product_Balance
SET Edit_Time = GETDATE(),
    Quantity = @balance
WHERE Product_Profile_ID = @productID 
        AND Location_Profile_ID = @locationID

FETCH NEXT FROM movementCursor INTO @productID, @locationID
END
CLOSE movementCursor;
DEALLOCATE movementCursor;";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@DailyLoadingID", dailyLoadingId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertProductMovement(int dailyLoadingId, SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
INSERT INTO Product_Movement
    (
    Creation_Time,
    Creator,
    Availability,
    Doc_Date,
    Doc_Time,
    Employee_Profile_ID,
    Vehicle_Profile_ID,
    Employee_Profile_ID1,
    Transaction_Type_ID,
    Product_Profile_ID,
    Doc_No,
    Out_Qty,
    Quantity,
    Outgoing_Batch_ID,
    Outgoing_Batch,
    Reference_ID,
    Reference_Table
)
(
    SELECT
        GETDATE(),
        loading.Creation_Time,
        loading.Creator,
        loading.Doc_Date,
        loading.Doc_Time,
        (SELECT Employee_Profile_ID FROM Users WHERE Users_ID = loading.Creator),
        loading.Vehicle_Profile_ID,
        loading.Employee_Profile_ID,
        (SELECT Transaction_Type_ID FROM Transaction_Type WHERE Default_Daily_Loading = 1),
        item.Product_Profile_ID,
        loading.Doc_No,
        item.Quantity,
        -item.Quantity,
        item.Product_Movement_ID,
        movement.Doc_No,
        item.Daily_Loading_Items_ID,
        'Daily_Loading_Items'
    FROM Daily_Loading_Items item WITH(ROWLOCK,XLOCK)
    INNER JOIN Product_Movement movement WITH(ROWLOCK,XLOCK) ON movement.Product_Movement_ID = item.Product_Movement_ID
    INNER JOIN Daily_Loading loading ON loading.Daily_Loading_ID = item.Daily_Loading_ID
    WHERE item.Daily_Loading_ID = @DailyLoadingID
)";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@DailyLoadingID", dailyLoadingId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}