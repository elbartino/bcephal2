using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class LinkedAttributeGridColumn
    {
        public Attribute attribute { get; set; }

        public bool isKey { get; set; }

        public int position { get; set; }

        public LinkedAttributeGridColumn(Attribute attribute, int position, bool isKey = false)
        {
            this.attribute = attribute;
            this.position = position;
            this.isKey = isKey;
        }

        public override string ToString()
        {
            return this.attribute != null ? this.attribute.ToString() : base.ToString();
        }

    }
}
