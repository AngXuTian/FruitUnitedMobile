using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ForValidation
{
    public class Validation
    {        
        public string ValidateMandatory(string val, string fieldName)
        {
            string err = string.Empty;

            if (string.IsNullOrEmpty(val))
            {
                err = string.Format(@"{0} is mandatory", val);
            }

            return err;
        }

        public void DeletePDFFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.pdf");

            foreach(string file in files)
            {
                File.Delete(file);
            }
        }
    }   
}