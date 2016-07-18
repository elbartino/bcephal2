using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrillePage
    {

        public int pageSize { get; set; }

        public int pageFirstItem { get; set; }

        public int pageLastItem { get; set; }

        public int totalItemCount { get; set; }

        public int pageCount { get; set; }

        public int currentPage { get; set; }

        public List<Object[]> rows { get; set; }

    }
}
