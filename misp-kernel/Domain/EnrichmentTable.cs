using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class EnrichmentTable : Persistent
    {

        public EnrichmentTable()
        {
            
        }
        public String name { get; set; }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is EnrichmentTable)) return 1;
            return this.name.CompareTo(((EnrichmentTable)obj).name);
        }

    }
}
