using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class CellPropertyBrowserData : BrowserData
    {

        public string nameColumn { get; set; }

        public string nameRow { get; set; }

        public string nameSheet { get; set; }

        public string cellScope { get; set; }

        public string cellMeasure { get; set; }

        public string periodFrom { get; set; }

        public string periodTo { get; set; }

        public decimal decimalMeasure { get; set; }

    }
}
