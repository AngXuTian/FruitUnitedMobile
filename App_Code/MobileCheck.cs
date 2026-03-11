using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FruitUnitedMobile.MobileCheck
{
    public class Extentions
    {
        public static bool IsMobile(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;
            //tablet
            if (Regex.IsMatch(userAgent, "(tablet|ipad|playbook|silk)|(android(?!.*mobile))", RegexOptions.IgnoreCase))
                return true;
            //mobile
            const string mobileRegex =
                "blackberry|iphone|mobile|windows ce|opera mini|htc|sony|palm|symbianos|ipad|ipod|blackberry|bada|kindle|symbian|sonyericsson|android|samsung|nokia|wap|motor";

            if (Regex.IsMatch(userAgent, mobileRegex, RegexOptions.IgnoreCase)) return true;
            //not mobile 
            return false;
        }
    }
}