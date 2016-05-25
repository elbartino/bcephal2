using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class User : Persistent
    {

        public String name { get; set; }

        public String login { get; set; }

        public String password { get; set; }

        public String email { get; set; }

        public bool? active { get; set; }

        public Profil profil { get; set; }

        public bool? admin { get; set; }

        //[ScriptIgnore]
        //public bool? visibleInShortcut { get; set; }

        //[ScriptIgnore]
        //public BGroup group { get; set; }

        //[ScriptIgnore]
        //public PersistentListChangeHandler<Rights> rightsListChangeHandler { get; set; }

        [ScriptIgnore]
        public PersistentListChangeHandler<Relation> relationsListChangeHandler { get; set; }


        public User()
        {
            this.active = true;
            //rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
            relationsListChangeHandler = new PersistentListChangeHandler<Relation>();
            //this.group = new BGroup();
            //visibleInShortcut = true;
            this.oid = -1;
            this.modificationDate = "01-01-1970 00:00:00";
            this.creationDate = "01-01-1970 00:00:00";
        }

        public String ToString()
        {
            return this.name;
        }
    }
    
}
