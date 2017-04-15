using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class LinkedAttributeGrid : Grille
    {

        public Attribute attribute { get; set; }

        //public override bool Equals(object obj)
        //{
        //    if (base.Equals(obj)) return true;

        //    if (obj is Kernel.Domain.LinkedAttributeGrid)
        //    {
        //        Kernel.Domain.LinkedAttributeGrid objm = (Kernel.Domain.LinkedAttributeGrid)obj;
        //        return this.attribute.Equals(objm.attribute);
        //    }
        //    return false;
        //}

        public override string ToString()
        {
            return this.attribute != null ? this.attribute.ToString() : base.ToString();
        }

    }
}
