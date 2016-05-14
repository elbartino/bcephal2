using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Report : InputTable
    {
        public bool evaluated { get; set; }

        public Report getACopy() 
        {
            Report report = new Report();
            report.oid = this.oid;
            report.evaluated = this.evaluated;
            report.excelFileExtension = this.excelFileExtension;
            report.excelFileName = this.excelFileName;
            report.name = "Copy of_"+this.name;
            report.period = this.period != null ?  this.period.GetCopy() : null;
            report.filter = this.filter != null ? this.filter.GetCopy() : null;
            report.cellPropertyListChangeHandler = new PersistentListChangeHandler<CellProperty>();
            foreach (CellProperty cell in this.cellPropertyListChangeHandler.Items) 
            {
                report.cellPropertyListChangeHandler.AddNew(cell.GetCopy());
            }
            return report;
        }


    }
}
