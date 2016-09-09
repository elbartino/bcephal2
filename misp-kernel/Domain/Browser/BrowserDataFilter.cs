using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserDataFilter
    {

        public List<BrowserDataFilterItem> items { get; set; }

        public BrowserDataFilter()
        {
            items = new List<BrowserDataFilterItem>(0);
        }

    }
}
