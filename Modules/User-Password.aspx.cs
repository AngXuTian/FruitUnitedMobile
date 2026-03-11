using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBConnection;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.DynamicData;
using iTextSharp.text;
using ForSessionValue;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using ZXing.QrCode;
using Image = System.Drawing.Image;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using ZXing.Common;
using ZXing;
using System.IO;

namespace FruitUnitedMobile.Modules
{
    public partial class User_Password : System.Web.UI.Page
    {
        Connection con = new Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Employee_Name"] != null && !string.IsNullOrEmpty(Session["Employee_Name"].ToString()))
            {
                UserName.InnerText = "Hi, " + Session["Employee_Name"].ToString();
            }

            if (!IsPostBack)
            {        
            }  
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string CurrentPassword = CurrentPasswordText.Text;
            string NewPassword = ConfirmPasswordText1.Text;
            string ConfirmNewPassword = ConfirmPasswordText2.Text;
            string DBPassword = con.ExecuteSQLQueryWithOneReturn(string.Format(@"SELECT Mobile_Password FROM Employee_Profile WHERE Employee_Profile_ID = '{0}'", Session["EmpID"].ToString())).ToString();
            if (DBPassword.ToLower() != CurrentPassword.ToLower())
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Incorrect Current Password.');", true);
            }
            else if (NewPassword.Length < 4)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('New Password must be atleast 4.');", true);
            }
            else if (NewPassword != ConfirmNewPassword)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('New Password and Confirm New Password is not match.');", true);
            }
            else
            {
                con.ExecuteSQLQuery(string.Format(@"
                UPDATE Employee_Profile
                SET Mobile_Password = '{0}'
                WHERE Employee_Profile_ID = {1} "
                , NewPassword, Session["EmpID"]));
                ClientScript.RegisterStartupScript(this.GetType(), "alert", @"alert('Password Changed.');window.location=""./Menu.aspx"";", true);
            }
        }
    }
}