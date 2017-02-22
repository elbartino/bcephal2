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

        public string name { get; set; }

        public string firstName { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool? active { get; set; }

        public bool? administrator { get; set; }

        public bool? visibleInShortcut { get; set; }

        public Profil profil { get; set; }
        
        public PersistentListChangeHandler<Right> rightsListChangeHandler { get; set; }

        public PersistentListChangeHandler<Relation> relationsListChangeHandler { get; set; }


        public User()
        {
            this.active = true;
            this.administrator = false;
            rightsListChangeHandler = new PersistentListChangeHandler<Right>();
            relationsListChangeHandler = new PersistentListChangeHandler<Relation>();
        }

        public bool IsAdmin()
        {
            return this.administrator.HasValue && this.administrator.Value;
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

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;

            if (obj is Kernel.Domain.User)
            {
                Kernel.Domain.User objm = (Kernel.Domain.User)obj;
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
