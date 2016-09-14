using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserDataFilter
    {

        public static int DEFAULT_PAGE_SIZE = 5;

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? groupOid { get; set; }

        public List<BrowserDataFilterItem> items { get; set; }

        public BrowserDataFilter()
        {
            pageSize = DEFAULT_PAGE_SIZE;
            items = new List<BrowserDataFilterItem>(0);
        }

    }
}
