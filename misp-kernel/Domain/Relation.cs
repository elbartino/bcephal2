using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Relation : Persistent
    {
        [ScriptIgnore]
        public User owner { get; set; }

        [ScriptIgnore]
        public User user { get; set; }

        [ScriptIgnore]
        public Role role { get; set; }

        private String ownername;

        private String rolename;

        public String ownerName { get { return this.owner != null ? this.owner.name : this.ownername; }
            set { ownername = value; }
        }

        public String roleName { get { return this.role != null ? this.role.name : this.rolename ;}
            set { rolename = value; }
        }
	

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Relation)) return 1;
            return 1;
        }
    }
}
