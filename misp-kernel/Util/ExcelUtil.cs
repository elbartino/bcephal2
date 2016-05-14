using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class ExcelUtil
    {
        public static string excelVersion;
        public static ExcelExtension GetDefaultExcelExtenstion()
        {
            Microsoft.Office.Interop.Excel._Application appl = new Microsoft.Office.Interop.Excel.Application();
            string sVersion = appl.Version;
            excelVersion = sVersion;
            switch (sVersion.ToString())
            {
                case "7.0": return ExcelExtension.XLS;
                case "8.0": return ExcelExtension.XLS;
                case "9.0": return ExcelExtension.XLS;
                case "10.0": return ExcelExtension.XLS;
                case "11.0": return ExcelExtension.XLS;
                case "12.0": return ExcelExtension.XLSX ;
                case "14.0": return ExcelExtension.XLSX;
                case "15.0": return ExcelExtension.XLSX;
                default: return null;
            }
        }

        public static ExcelExtension GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            String ext = Path.GetExtension(filePath);
            return ExcelExtension.Get(ext);
        }

        public static String GetFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            String name = Path.GetFileName(filePath);
            return name;
        }
    }
}
