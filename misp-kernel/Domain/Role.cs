using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Role : Persistent
    {
        public String name { get;set;}
        public String description { get; set; }
    }
}
