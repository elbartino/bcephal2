using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserDataPage<B>
    {
        
        public int pageSize { get; set; }

        public int pageFirstItem { get; set; }

        public int pageLastItem { get; set; }

        public int totalItemCount { get; set; }

        public int pageCount { get; set; }

        public int currentPage { get; set; }

        public List<B> rows { get; set; }

        public BrowserDataPage()
        {
            rows = new List<B>(0);
        }

    }
}
