using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleEditedResult
    {

        public bool isError { get; set; }
        public String error { get; set; }
        public Object[] datas { get; set; }

        public GrilleEditedResult()
        {
            this.isError = false;
        }

    }
}
