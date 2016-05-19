using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Relation : Persistent
    {
        public User owner { get; set; }
        public User user { get; set; }
        public Role role { get; set; }
    }
}
