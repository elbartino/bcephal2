using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class TableSaveIssue
    {
        public enum Decision {NULL,ADD, MACHT, STOP }

        public CellMeasure cellMeasure { get; set; }

        public TargetItem targetItem { get; set; }

        public String period { get; set; }

        public bool applyToAll { get; set; }

        public String tableName { get; set; }

        public String excelCellValue { get; set; }

        public String decision { get; set; }
    }
}
