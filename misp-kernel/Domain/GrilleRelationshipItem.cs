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
        public bool exclusive { get; set; }
        public bool primary { get; set; }


        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is GrilleRelationshipItem)) return 1;
            return this.position.CompareTo(((GrilleRelationshipItem)obj).position);
        }
    }
}
