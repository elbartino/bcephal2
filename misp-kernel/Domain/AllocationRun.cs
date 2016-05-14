using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class AllocationRun : Persistent
    {
        public enum AllocationRunType {ALLOCATION,PARTIAL_CLEARRING, ALLOCATION_WITH_ERROR}

        public string name { get; set; }

        public string executionDate { get; set; }

        public string allocationRunType { get; set; }

        public long runDuration { get; set; }

        public int nbrNullScope { get; set; }

        public int nbrNullPeriod { get; set; }

        public int nbrNoMeasure { get; set; }

        public int nbrNoNumericValue { get; set; }

        public int nullNumericValue { get; set; }

        public long allocationId { get; set; }


        public AllocationRun() 
        {
            allocationRunType = AllocationRunType.ALLOCATION.ToString();
        }

        [ScriptIgnore]
        public bool IsAllocation
        {
            get
            {
                return allocationRunType != null ? allocationRunType == AllocationRunType.ALLOCATION.ToString() : false;
            }
            set
            {
                allocationRunType = value ? AllocationRunType.ALLOCATION.ToString() : AllocationRunType.PARTIAL_CLEARRING.ToString();
            }
        }
       
    }
}
