using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class ExcelExtension
    {

        public static ExcelExtension XLS = new ExcelExtension(".xls");
        public static ExcelExtension XLSX = new ExcelExtension(".xlsx");

        public string Extension { get; set; }

        public ExcelExtension(string extension) 
        {
            this.Extension = extension;
        }

        public override  string ToString() 
        {
            return this.Extension;
        }


        public static ExcelExtension Get(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return null;
            if(ext.Trim().ToUpper().Equals(XLSX.Extension.ToUpper())) return XLSX;
            else if(ext.Trim().ToUpper().Equals(XLS.Extension.ToUpper())) return XLS;
            return null;
        }
    }
}
