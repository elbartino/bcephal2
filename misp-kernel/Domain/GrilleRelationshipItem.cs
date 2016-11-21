using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleRelationshipItem : Persistent
    {

        public int position { get; set; }
        public GrilleColumn column { get; set; }
        public bool isExclusive { get; set; }

    }
}
