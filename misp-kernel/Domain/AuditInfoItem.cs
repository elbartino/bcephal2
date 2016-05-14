using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class AuditInfoItem
    {
        public String cellName { get; set; }

        public String cellTargetName { get; set; }

        public String cellMeasureName { get; set; }

        public String cellSheetName { get; set; } 

        public String cellPeriod { get; set; }

        public int cellID { get; set; }

        public String cellTableName { get; set; }

        public String cellTableFilterName { get; set; }

        public String cellTablePeriod { get; set; }

        public double cellInitialAmount { get; set; }

        public String allocationType { get; set; }

        public double cellAllocatedAmount { get; set; }

        public double cellRemainingAmount { get; set; }

        public double cellAmountInReport { get; set; }

        public double universeItemAmount { get; set; }

    }
}
