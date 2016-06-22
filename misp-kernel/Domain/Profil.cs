using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Profil : Persistent
    {
        public String name { get; set; }

        public bool active { get; set; }

        public PersistentListChangeHandler<Rights> rightsListChangeHandler { get; set; }

        public bool visibleInShortcut { get; set; }
            
        public BGroup group { get; set; }

        public Profil()
        {
            rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
        }

        public String ToString()
        {
            return this.name;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Profil)) return 1;
            return this.name.CompareTo(((Profil)obj).name);
        }
    }
}
