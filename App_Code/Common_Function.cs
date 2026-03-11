using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using DBConnection;
using System.Web.Security;
using EASendMail;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace FruitUnitedMobile.CommonFunctions
{

    public class CommonFunctions
    {
        
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        public static bool Generate_Code(string V1, string column, string table, string module, string idtoupdate, string combinationCode, bool variableReset, bool yearly, bool monthly, int len, int yearlen, string mthFormat, string dateResetBy, string docDate , string loginID)
        {
            if (column == "" || column == String.Empty)
                throw new Exception("Column cannot be null. Code didn't generated");

            if (table == "" || table == String.Empty)
                throw new Exception("Table cannot be null. Code didn't generated");

            if (idtoupdate == "" || idtoupdate == String.Empty)
                throw new Exception("ID to update cannot be null. Code didn't generated");

            if (string.IsNullOrEmpty(dateResetBy))
                dateResetBy = "System";

            if (yearly == true && monthly == true)
                throw new Exception("Cannot be both yearly and monthy.");

            if (yearly == true && dateResetBy == "Document" && string.IsNullOrEmpty(docDate))
                throw new Exception("Document date cannot be null or empty if number generation is by document date");

            if (combinationCode == "" || combinationCode == String.Empty)
                throw new Exception("Combination code cannot be null. Code didn't generated");

            if (V1 == "" || V1 == String.Empty)
                V1 = "''";

            if (variableReset == true && V1 == "''")
                throw new Exception("Variable cannot be null if code to reset by variable");

            if (mthFormat == "" || mthFormat == String.Empty)
                mthFormat = "M";

            int yearlymonthly = 0;

            if (yearly == false && monthly == true)
                yearlymonthly = 2;

            if (yearly == true && monthly == false)
                yearlymonthly = 1;

            if (module == "" || module == String.Empty)
                module = table;

            if (dateResetBy == "System")
                docDate = DateTime.Today.ToString();

            string code_sql = @"

            DECLARE @ID_TO_UPDATE VARCHAR(80);
            DECLARE @startNumber int;
            DECLARE @result varchar(80);
            DECLARE @CURRENTNO VARCHAR(80);
            DECLARE @count INT;
            DECLARE @LEN INT;
            DECLARE @YEARLEN INT;
            DECLARE @YEARLYMONTHLY INT; -- 1 FOR YEARLY / 2 FOR MONTHLY
            DECLARE @VARIABLERESET NVARCHAR(10);
            DECLARE @YEAR VARCHAR(80); -- new
            DECLARE @MONTH VARCHAR(80); -- new
            DECLARE @GETLASTYEAR BIT; -- new
            DECLARE @V1 VARCHAR(80);
            DECLARE @COMBINATION NVARCHAR(255);
            DECLARE @RUNNINGNO NVARCHAR(255);
            DECLARE @VARIABLE NVARCHAR(255);
            DECLARE @mthFormat NVARCHAR(10);
            DECLARE @lockTable int;
            DECLARE @docDate datetime;              
            DECLARE @userID int;            

            SET @ID_TO_UPDATE = '" + idtoupdate + @"';
            SET @YEARLYMONTHLY = " + yearlymonthly + @";
            SET @YEARLEN = " + yearlen + @";
            SET @LEN =" + len + @";
            SET @V1 = <V1>
            SET @VARIABLERESET = '" + variableReset + @"';
            SET @mthFormat = '" + mthFormat + @"';
            SET @docDate = GETDATE();

            IF ((SELECT " + column + @" FROM " + table + @" WITH (ROWLOCK, XLOCK) WHERE  " + table + @"_ID = @ID_TO_UPDATE) IS NULL)
                BEGIN
                    SET @YEAR = RIGHT(DATEPART(YEAR,@docDate), @YEARLEN);

                    IF (@mthFormat = 'M' OR @mthFormat = '' OR @mthFormat IS NULL)
                        BEGIN
                            SET @MONTH = DATEPART(MONTH,@docDate);

                            IF (CONVERT(INT, @MONTH) < 10)
                                SET @MONTH = '0' + @MONTH;
                        END

                    IF (@mthFormat = 'MM')
                        SET @MONTH = UPPER(DATENAME(MONTH,@docDate));

                    IF (@mthFormat = 'MMM')
                        SET @MONTH = LEFT(UPPER(DATENAME(MONTH,@docDate)),3);

                    IF (@VARIABLERESET = 'TRUE')
                        SET @VARIABLE = @V1

                    IF NOT EXISTS(SELECT CurrentRunningNo FROM EMS_RunningNos WHERE ColumnName = '" + column + @"' AND TableName = '" + module + @"' AND RN_Year = (CASE WHEN (@YEARLYMONTHLY = 1 OR @YEARLYMONTHLY = 2) THEN @YEAR ELSE '' END) AND RN_Month = (CASE WHEN (@YEARLYMONTHLY = 2) THEN @MONTH ELSE '' END) AND RN_Variable = ISNULL(@VARIABLE,''))
                        BEGIN
                            SET @lockTable = (SELECT COUNT(*) FROM (SELECT CurrentRunningNo FROM EMS_RunningNos WITH(TABLOCKX)) lockTable)
                            SET @CURRENTNO = NULL
                        END
                    ELSE
                        BEGIN    
                            SET @CURRENTNO = (SELECT CurrentRunningNo FROM EMS_RunningNos WITH (ROWLOCK, XLOCK) WHERE ColumnName = '" + column + @"' AND TableName = '" + module + @"' AND RN_Year = (CASE WHEN (@YEARLYMONTHLY = 1 OR @YEARLYMONTHLY = 2) THEN @YEAR ELSE '' END) AND RN_Month = (CASE WHEN (@YEARLYMONTHLY = 2) THEN @MONTH ELSE '' END) AND RN_Variable = ISNULL(@VARIABLE,'')); -- new           
                        END

                    SET @startNumber = 1000000000;

                     IF (@CURRENTNO IS NOT NULL)
                         BEGIN
                            SET @count = CONVERT(INT,@CURRENTNO) + 1 + @startNumber;
                            SET @RUNNINGNO = CONVERT(nvarchar, RIGHT(@count,@LEN))
                            SET @result = <Combination_Code>
                            
                            UPDATE EMS_RunningNos
                            SET CurrentRunningNo = @RUNNINGNO,
                                Last_Modified_Date = GETDATE(),
                                Last_Modified_By = @userID
                            WHERE ColumnName = '" + column + @"' AND TableName = '" + module + @"' 
                                    AND RN_Year = (CASE WHEN (@YEARLYMONTHLY = 1 OR @YEARLYMONTHLY = 2) THEN @YEAR ELSE '' END) 
                                    AND RN_Month = (CASE WHEN (@YEARLYMONTHLY = 2) THEN @MONTH ELSE '' END) 
                                    AND RN_Variable = ISNULL(@VARIABLE,'')
                         END
                     ELSE
                         BEGIN
                            SET @RUNNINGNO = CONVERT(nvarchar, RIGHT(@startNumber+1,@LEN))
                            SET @result = <Combination_Code>
                            
                            INSERT INTO EMS_RunningNos(EMS_RunningNos_GUID, ColumnName, TableName, CurrentRunningNo, RN_Year, RN_Month, RN_Variable, Last_Modified_Date, Last_Modified_By)
                            (SELECT
                                NEWID(),  
                                '" + column + @"', 
                                '" + module + @"', 
                                @RUNNINGNO, 
                                (CASE WHEN (@YEARLYMONTHLY = 1 OR @YEARLYMONTHLY = 2) THEN @YEAR ELSE '' END),
                                (CASE WHEN (@YEARLYMONTHLY = 2) THEN @MONTH ELSE '' END),
                                ISNULL(@VARIABLE,''),
                                GETDATE(),
                                @userID
                            )
                         END

                    UPDATE " + table + @" SET " + column + @" = @result WHERE " + table + @"_ID = @ID_TO_UPDATE;
                END
                
            ";

            code_sql = code_sql.Replace("<Combination_Code>", combinationCode);
            code_sql = code_sql.Replace("<V1>", V1);

            string result = run_SQL_In_Serialized_Transaction(code_sql);
            if (result == "ROLL BACK")
            {
                throw new Exception(code_sql);
            }

            return true;
        }

        public static string run_SQL_In_Serialized_Transaction(string sql)
        {
            string sqlCommand = @"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
                BEGIN TRANSACTION T1
                BEGIN TRY

                -------------------------------------------------------

                " + sql + @"

                -------------------------------------------------------

                COMMIT TRANSACTION T1
                SELECT 'COMMIT';

                END TRY 
                BEGIN CATCH 

                SELECT 'ROLL BACK'
                ROLLBACK TRANSACTION T1 

                END CATCH 
            ";

#pragma warning disable CS0436 // Type conflicts with imported type
            Connection con = new Connection();
#pragma warning restore CS0436 // Type conflicts with imported type
            return con.ExecuteSQLQueryWithOneReturn(sqlCommand).ToString();
        }

        public static void SendMail(string subject, string body, List<string> recipients, List<string> CCs, List<string> BCCs)
        {
            Connection con = new Connection();
            // SEND EMAIL TO RECIPIENT
            string smtpHost = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_Server_Name FROM Global_Settings").ToString();
            string smtpUser = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_Username FROM Global_Settings").ToString();
            string smtpPass = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 SMTP_Password FROM Global_Settings").ToString();
            int smtpPort = int.Parse(con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 Port FROM Global_Settings").ToString());
            bool useSSL = con.ExecuteSQLQueryWithOneReturn("SELECT TOP 1 CASE WHEN SSL = '1' THEN 'Yes' END FROM Global_Settings").ToString() == "Yes";
            int totalRecipients = 0;

            if (smtpPort == 465)
            {
                CDO.Message oMsg = new CDO.Message();
                CDO.IConfiguration iConfg;
                iConfg = oMsg.Configuration;
                ADODB.Fields oFields;
                oFields = iConfg.Fields;
                // Set configuration.
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
                // Html Content
                if (recipients != null && recipients.Count > 0)
                {
                    oMsg.HTMLBody = body;
                    // Email subject, sender, receiver
                    oMsg.Subject = subject;
                    oMsg.From = smtpUser;
                    oMsg.To = string.Join(";", recipients);
                    oMsg.Send();
                    totalRecipients++;
                }

                if (CCs != null && CCs.Count > 0)
                {
                    foreach (string s in CCs)
                    {
                        oMsg.HTMLBody = body;
                        // Email subject, sender, receiver
                        oMsg.Subject = subject;
                        oMsg.From = smtpUser;
                        oMsg.To = s;
                        oMsg.Send();
                        totalRecipients++;
                    }
                }

                if (BCCs != null && BCCs.Count > 0)
                {
                    foreach (string s in BCCs)
                    {
                        oMsg.HTMLBody = body;
                        // Email subject, sender, receiver
                        oMsg.Subject = subject;
                        oMsg.From = smtpUser;
                        oMsg.To = s;
                        oMsg.Send();
                        totalRecipients++;
                    }
                }
            }
            else if (smtpPort == 587)
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                oMail.From = smtpUser;
                oMail.Subject = subject;
                // oMail.TextBody = body;
                oMail.HtmlBody = body;
                oMail.To = string.Join(";", BCCs);
                // oMail.Cc = string.Join(";", CCs);
                // oMail.Bcc = string.Join(";", string.Join(";", BCCs););
                SmtpServer oServer = new SmtpServer(smtpHost);
                oServer.User = smtpUser;
                oServer.Password = smtpPass;
                oServer.Port = 587;
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
                oSmtp.SendMail(oServer, oMail);
            }
            else
            {
                // CONSTRUCT THE MAIL OBJECT
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                System.Net.NetworkCredential basicCredential = new System.Net.NetworkCredential(smtpUser, smtpPass);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress(smtpUser);

                smtpClient.Host = smtpHost;
                if (useSSL)
                    smtpClient.EnableSsl = true;
                smtpClient.Port = smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Timeout = (60 * 5 * 1000);

                message.From = fromAddress;
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;


                if (recipients != null && recipients.Count > 0)
                {
                    foreach (string s in recipients)
                    {
                        message.To.Add(new System.Net.Mail.MailAddress(s));
                        totalRecipients++;
                    }
                }

                if (CCs != null && CCs.Count > 0)
                {
                    foreach (string s in CCs)
                    {
                        message.CC.Add(new System.Net.Mail.MailAddress(s));
                        totalRecipients++;
                    }
                }

                if (BCCs != null && BCCs.Count > 0)
                {
                    foreach (string s in BCCs)
                    {
                        message.Bcc.Add(new System.Net.Mail.MailAddress(s));
                        totalRecipients++;
                    }
                }

                smtpClient.Send(message);
            }
        }

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static void generateGRNNo(string GRNID , string LoginID)
        {
            Connection con = new Connection();
            string initial = "";

            int financialMonth = Int32.Parse(con.ExecuteSQLQueryWithOneReturn(@"SELECT Financial_Month FROM Company_Profile").ToString());

            DateTime GRNDate = DateTime.Parse(con.ExecuteSQLQueryWithOneReturn(@"SELECT Doc_Date FROM GI WHERE GI_ID = '" + GRNID + @"'").ToString());
            int GRNMonth = GRNDate.Month;
            int financialYear;

            if (GRNMonth >= financialMonth)
                financialYear = GRNDate.Year + 1;
            else
                financialYear = GRNDate.Year;

            initial = financialYear.ToString();

            bool newItemNo = CommonFunctions.Generate_Code("'" + initial + @"'", "Doc_No", "GI", "GI", GRNID, "'GRN'+RIGHT('" + initial + @"',2)+@MONTH+@RUNNINGNO", true, false, false, 4, 2, "M", "Document", GRNDate.ToString(), LoginID);

            if (newItemNo == false)
            {
                throw new Exception("Generate GRN No failed.");
            }
        }

        public static void generateGRTNNo(string GRTNID, string LoginID)
        {
            Connection con = new Connection();
            string initial = "";

            int financialMonth = Int32.Parse(con.ExecuteSQLQueryWithOneReturn(@"SELECT Financial_Month FROM Company_Profile").ToString());

            DateTime GRNDate = DateTime.Parse(con.ExecuteSQLQueryWithOneReturn(@"SELECT Doc_Date FROM GRTN WHERE GRTN_ID = '" + GRTNID + @"'").ToString());
            int GRNMonth = GRNDate.Month;
            int financialYear;

            if (GRNMonth >= financialMonth)
                financialYear = GRNDate.Year + 1;
            else
                financialYear = GRNDate.Year;

            initial = financialYear.ToString();

            bool newItemNo = CommonFunctions.Generate_Code("'" + initial + @"'", "Doc_No", "GRTN", "GRTN", GRTNID, "'GRNR'+RIGHT('" + initial + @"',2)+@MONTH+@RUNNINGNO", true, false, false, 4, 2, "M", "Document", GRNDate.ToString(), LoginID);

            if (newItemNo == false)
            {
                throw new Exception("Generate GRN No failed.");
            }
        }

        public static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static void SaveCompressedImage(Bitmap image, string savePath, int quality)
        {
            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            image.Save(savePath, jpgEncoder, encoderParams);
        }

       
    }
}