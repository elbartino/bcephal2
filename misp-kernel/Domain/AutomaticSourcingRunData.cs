using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class AutomaticSourcingRunData
    {
        public int oid { get; set; }

        public String excelFilePath { get; set; }

        public String tableName { get; set; }

        public bool runAllocation { get; set; }
    }
}
