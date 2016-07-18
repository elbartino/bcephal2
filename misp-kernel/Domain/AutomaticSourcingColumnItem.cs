using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class AutomaticSourcingColumnItem : Persistent
    {

        public int position { get; set; }

        public String type { get; set; }

        public String value { get; set; }

        [ScriptIgnore]
        public AutomaticSourcingColumn column { get; set; }


        public override string ToString()
        {
            return this.value != null ? this.value : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is AutomaticSourcingColumnItem)) return 1;
            if (this.value != null && this.value.Equals(((AutomaticSourcingColumnItem)obj).value)) return 0;
            return this.position.CompareTo(((AutomaticSourcingColumnItem)obj).position);
        }

    }
}
