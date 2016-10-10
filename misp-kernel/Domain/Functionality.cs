using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
   public class Functionality
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<Functionality> children { get; set; }

        public Functionality(string code, string name)
        {
            this.code = code;
            this.name = name;
            children = new List<Functionality>(0);
        }
    }
}
