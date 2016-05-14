using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class ReconciliationTemplate : Persistent
    {
        
        public string name { get; set; }

        public BGroup group { get; set; }

        public bool visibleInShortcut { get; set; }

        public PostingFilter leftPostingFilter { get; set; }

        public PostingFilter rigthPostingFilter { get; set; }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is ReconciliationTemplate)) return 1;
            return this.name.CompareTo(((ReconciliationTemplate)obj).name);
        }


    }
}
