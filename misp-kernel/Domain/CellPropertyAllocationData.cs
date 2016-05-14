using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class CellPropertyAllocationData : Persistent
    {

        public enum AllocationType { Scope2Scope, Linear, Template, NoAllocation }


         /// <summary>
        /// Construit une npouvelle instance de CellPropertyAllocationData.
        /// </summary>
        public CellPropertyAllocationData() {
            type = AllocationType.NoAllocation.ToString();
        }

        public string type { get; set; }

        public long sequence { get; set; }

        //public AllocationTemplate allocationTemplate { get; set; }

        public Measure measureRef { get; set; }

        public Measure outputMeasure { get; set; }


        public CellPropertyAllocationData GetCopy()
        {
            CellPropertyAllocationData data = new CellPropertyAllocationData();
            data.type = this.type;
            data.sequence = this.sequence;
            data.measureRef = this.outputMeasure;
            return data;
        }

    }
}
