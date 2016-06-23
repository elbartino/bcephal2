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

        [ScriptIgnore]
        public String firstName { get; set; }

        public String login { get; set; }

        public String password { get; set; }

        public String email { get; set; }

        public bool? active { get; set; }

        public Profil profil { get; set; }

        public bool? admin { get; set; }

        public bool? visibleInShortcut { get; set; }

        public BGroup group { get; set; }

        public PersistentListChangeHandler<Rights> rightsListChangeHandler { get; set; }

        public PersistentListChangeHandler<Relation> relationsListChangeHandler { get; set; }


        public User()
        {
            this.active = true;
            rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
            relationsListChangeHandler = new PersistentListChangeHandler<Relation>();
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is User)) return 1;
            return this.name.CompareTo(((User)obj).name);
        }
    }
    
}
