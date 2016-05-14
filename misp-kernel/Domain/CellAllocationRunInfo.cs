using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class CellAllocationRunInfo : Persistent
    {

        double _remainingAmountBeforeAlloc;

        public CellProperty cellProperty {get; set; }
        public double initialAmount { get; set; }
        public double allocatedAmount { get; set; }
        public double remainingAmount { get; set; }
        public double remainingAmountBeforeAlloc 
        { 
            get { return _remainingAmountBeforeAlloc; } 
            set 
            { 
                _remainingAmountBeforeAlloc = value; 
            } 
        }
        public bool alreadyAdded { get; set; }

    }
}
