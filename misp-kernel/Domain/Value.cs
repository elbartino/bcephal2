using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Value
    {

        public int? oid { get; set;}
	    public String name { get; set;}
	
	    public Value() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="name"></param>
        public Value(int? oid, String name) {
		    this.oid = oid;
		    this.name = name;
	    }

        public override String ToString()
        {
            return name;
        }

    }
}
