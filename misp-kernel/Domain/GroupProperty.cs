using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GroupProperty
    {
        public GroupProperty(CellProperty cellProperty, Misp.Kernel.Ui.Office.Range range) 
        {
            this.cellProperty = cellProperty != null ? cellProperty : new CellProperty();
            this.range = range;
            isReset = false;
        }

        public bool isPaste { get; set; }
        public bool isPartialPaste { get; set; }
        public List<string> partialPasteSelections { get; set; }
        public bool isReset { get; set; }
        public bool isValueChanged { get; set; }
        public bool isTarget { get; set; }
        public bool isTag { get; set; }
        public bool isMeasure { get; set; }
        public bool isForAllocation { get; set; }
        public bool isPeriod { get; set; }
        public bool isCellPropertyAllocationData { get; set; }
        public bool isImported { get; set; }
        public Misp.Kernel.Ui.Office.Range range { get; set; }
        public Misp.Kernel.Ui.Office.Range copiedRange { get; set; }

        public int position { get; set; }

        public string excelFileName { get; set; }
        
        public CellProperty cellProperty { get; set; }

        public String name
        {
            get { return range.Name; 
        }

    }
}
}