// File: App_Code/InvoiceNoGenerator.cs  or  FruitUnitedMobile/InvoiceNoGenerator/InvoiceNoGenerator.cs
using System;
using System.Data.SqlClient;

namespace FruitUnitedMobile.InvoiceNoGenerator
{
    public static class InvoiceNoGenerate
    {
        /// <summary>
        /// Generates mobile-style invoice number: FU{CustomerCode(2)}{yyMMdd}{POS_ID(1)}{###}
        /// Example: FUFP250101A001
        /// Sequence is per customer + POS + date (max 999/day)
        /// </summary>
        public static string GenerateMobileStyleInvoiceNo(
    SqlConnection con,
    SqlTransaction trans,
    string customerCode,           // docDate removed → always uses today
    int posProfileId)
        {
            DateTime today = DateTime.Today;

            if (string.IsNullOrWhiteSpace(customerCode) || customerCode.Trim().Length < 1)
                throw new ArgumentException("Customer code must be at least 1 character.", nameof(customerCode));

            if (posProfileId <= 0)
                throw new ArgumentException("Invalid POS Profile ID.", nameof(posProfileId));

            string posId = GetPosId(con, trans, posProfileId);

            string cust = customerCode.Trim().ToUpper().Substring(0, 1);
            string pos = posId.Trim().ToUpper();

            if (string.IsNullOrEmpty(pos) || pos.Length != 1 || !char.IsLetter(pos[0]))
                throw new InvalidOperationException($"POS_ID must be exactly one letter (A-Z). Got '{pos}'");

            // Date part: DDMMyy (today)
            string datePart = today.ToString("yyMMdd");

            // No hyphen anymore
            //string prefixPart = $"FU{cust}";
            string prefixPart = $"{cust}";
            string searchPrefix = $"{prefixPart}{datePart}{pos}";  

            const int EXPECTED_LENGTH = 10;   // 1 + 6 + 1 + 2 = 10 chars

            string sql = @"
        SELECT MAX(Doc_No)
        FROM Invoice
        WHERE Doc_No LIKE @Prefix + '%'
          AND LEN(Doc_No) = @ExpectedLen";

            string maxDocNo = null;
            using (var cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@Prefix", searchPrefix);
                cmd.Parameters.AddWithValue("@ExpectedLen", EXPECTED_LENGTH);

                maxDocNo = cmd.ExecuteScalar() as string;
            }

            int nextSeq = 1;
            if (!string.IsNullOrEmpty(maxDocNo) && maxDocNo.Length >= 2)
            {
                string seqPart = maxDocNo.Substring(maxDocNo.Length - 2);
                if (int.TryParse(seqPart, out int current))
                    nextSeq = current + 1;
            }

            if (nextSeq > 99)
                throw new InvalidOperationException(
                    $"Daily invoice limit (99) reached for {cust}/{pos} on {today:yyyy-MM-dd}");

            // Final number – no hyphen
            return $"{prefixPart}{datePart}{pos}{nextSeq:D2}";
        }

        private static string GetPosId(SqlConnection con, SqlTransaction trans, int posProfileId)
        {
            const string sql = @"
        SELECT POS_ID
        FROM POS_Profile
        WHERE POS_Profile_ID = @PosProfileID";

            using (var cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@PosProfileID", posProfileId);
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException($"POS_Profile not found for ID {posProfileId}");

                string posId = result.ToString().Trim();

                if (string.IsNullOrEmpty(posId))
                    throw new InvalidOperationException("POS_ID is empty in POS_Profile");

                // ────────────────────────────────────────────────
                // Take only the FIRST character and uppercase it
                string firstChar = posId.Substring(0, 1).ToUpper();

                // Optional: extra safety – ensure it's actually a letter
                if (!char.IsLetter(firstChar[0]))
                    throw new InvalidOperationException(
                        $"POS_ID first character must be a letter (A-Z). Got '{firstChar}' from '{posId}'");

                return firstChar;
                // ────────────────────────────────────────────────
            }
        }
    }
}