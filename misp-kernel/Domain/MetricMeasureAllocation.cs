using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class MetricMeasureAllocation
    {
        public long nbrCellAllocated { get; set; }

        public String timeAllocation { get; set; }

        public int nbrCellWithError { get; set; }

        public int totalItemCount { get; set; }

        public int nbrErrorNullScope { get; set; }

        public int nbrErrorNoMeasure { get; set; }

        public int nbrErrorNotNumericValue { get; set; }

        public int nbrErrorNullPeriod { get; set; }


        public List<MetricMeasureAllocationItem> metricNumberOfCellPerMeasure { get; set; }
        public List<MetricMeasureAllocationItem> metricAmountToAllocatePerMeasure { get; set; }
        public List<MetricMeasureAllocationItem> metricAllocatedAmountPerMeasure { get; set; }
        public List<MetricMeasureAllocationItem> metricRemainingAmountPerMeasure { get; set; }

       
    }
}
