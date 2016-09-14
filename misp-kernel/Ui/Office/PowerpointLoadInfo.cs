using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    public class PowerpointLoadInfo
    {

        public String name { get; set; }
        public String filePath { get; set; }
        public String destPath { get; set; }
        public String action { get; set; }
        public int slideIndex { get; set; }
        public int shapeIndex { get; set; }
        public String text { get; set; }
        public decimal value { get; set; }
        public String sheetName { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public List<PowerpointLoadInfo> items { get; set; }

    }
}
