using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class AllocationRunBrowserDataPage
    {
        public static long DEFAULT_PAGE_SIZE = 25;

        public long totalCount { get; set; }
        public long pageCount { get; set; }
        public long pageSize { get; set; }
        public long page { get; set; }
        public List<AllocationRunBrowserData> datas { get; set; }

    }
}
