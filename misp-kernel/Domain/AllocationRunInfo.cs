using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class AllocationRunInfo
    {
        public bool runEnded { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public int totalCellCount { get; set; }
        public int runedCellCount { get; set; }
        public long pageCount { get; set; }
        public long currentPage { get; set; }
        public long pageSize { get; set; }
        public String tableName { get; set; }
        
        public List<CellAllocationRunInfoBrowserData> infos { get; set; }
        public AllocationRunInfo currentInfo { get; set; }

    }
}
