using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class CellAllocationRunInfoBrowserData : BrowserData
    {

        public String table { get; set; }
        public String excelFile { get; set; }
        public String sheet { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public String tableScope { get; set; }
        public String cellScope { get; set; }
        public String measure { get; set; }

        public String dateTag { get; set; }
        public String attributeTag { get; set; }
        public String cellTag { get; set; }
        public String allocation { get; set; }

        public double loadedAmount { get; set; }
        public double cellAmount { get; set; }

        public bool isError { get; set; }
        public String errorMessage { get; set; }
    }

}
