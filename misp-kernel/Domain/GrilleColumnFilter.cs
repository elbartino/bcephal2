using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleColumnFilter
    {
        public GrilleColumn column { get; set; }

        public string filterOperation { get; set; }

        public string filterValue { get; set; }

        public string filterOperator { get; set; }

        public List<GrilleColumnFilter> items { get; set; }

        public bool isGroup { get; set; }


        public GrilleColumnFilter()
        {
            this.items = new List<GrilleColumnFilter>(0);
            isGroup = false;
        }
        
    }
}
