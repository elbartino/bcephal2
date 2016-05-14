using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class AuditInfo
    {

        public long pageCount { get; set; }

        public long currentPage { get; set; }

        public long pageSize { get; set; }

        public long totalItemCount { get; set; }

        public List<AuditInfoItem> items { get; set; }

        public List<AllocationRunBrowserData> allocationRunLogInfos { get; set; }

    }
}
