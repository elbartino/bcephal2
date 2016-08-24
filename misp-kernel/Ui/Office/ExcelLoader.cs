using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Misp.Kernel.Ui.Office
{
    public class ExcelLoader
    {
        public static Dictionary<String, ExcelLoader> Loaders = new Dictionary<string, ExcelLoader>(0); 
        public String FilePath { get; set; }
        public Excel.Application ExcelApplication { get; set; }
        private Excel.Workbook workbook;

        public ExcelLoader() 
        {
            if (ExcelApplication == null) ExcelApplication = new Excel.Application();
        }

        public ExcelLoader(String FilePath)
        {
            this.FilePath = FilePath;
            if (ExcelApplication == null) ExcelApplication = new Excel.Application();
            workbook = ExcelApplication.Workbooks.Open(@FilePath);                        
        }

        public void setValue(string sheetName, int row, int column, Object value)
        {
            if (workbook == null) return;
            Excel.Worksheet worksheet = null;
            foreach (Object sheetItem in workbook.Sheets)
            {
                try
                {
                    Excel.Worksheet sheet = (Excel.Worksheet)sheetItem;
                    if (sheet.Name == sheetName)
                    {
                        worksheet = sheet;
                        break;
                    }
                }
                catch (Exception) { }
            }

            if (worksheet == null) return;
            Excel.Range xlRange = worksheet.Cells[row, column];
            Excel.Range cell = xlRange.Cells[1];
            cell.Value = value;
        }
        
        public void saveAndClose()
        {
            save();
            close();
        }

        public void save()
        {
            if (workbook == null) return;
            workbook.Save();
        }

        public void close()
        {
            if (workbook != null) workbook.Close();
            workbook = null;
            if (ExcelApplication == null) return;
            ExcelApplication.Quit();
            ExcelApplication = null;
        }

    }
}
