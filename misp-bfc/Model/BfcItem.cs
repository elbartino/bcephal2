using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Model
{
    public class BfcItem : IComparable
    {
        public int? oid { get; set; }

        public string id { get; set; }

        public string name { get; set; }


        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.name) ? this.name : base.ToString();
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null || !(obj is BfcItem)) return 1;
            if (this == obj) return 0;
            return this.oid.Value.CompareTo(((BfcItem)obj).oid.Value);
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;

            if (obj is BfcItem)
            {
                BfcItem objm = (BfcItem)obj;
                if (objm.oid.HasValue && this.oid.HasValue)
                {
                    if (objm.oid == this.oid) return true;
                }
                if (objm.name != null && objm.name.Equals(this.name)) return true;
            }
            return false;
        }

    }
}
