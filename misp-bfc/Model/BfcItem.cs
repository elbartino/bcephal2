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

    }
}
