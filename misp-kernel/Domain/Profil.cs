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

        public Boolean active { get; set; }
	  
        public PersistentListChangeHandler<Rights> rightsListChangeHandler;

        [ScriptIgnore]
        public bool visibleInShortcut { get; set; }
            
        [ScriptIgnore]
        public BGroup group { get; set; }

        public Profil()
        {
            rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
        }
    }
}
