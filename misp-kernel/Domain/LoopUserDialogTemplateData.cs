using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class LoopUserDialogTemplateData
    {
        public String message { get; set; }
        public String help { get; set; }
        public List<Value> values { get; set; }
        public bool stop { get; set; }
        public bool onePossibleChoice { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoopUserDialogTemplateData() {
		    values = new List<Value>(0);
            stop = false;
	    }

    }
}
