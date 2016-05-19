using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class User : Persistent
    {

        public String name { get; set; }

        public String login { get; set; }

        public String password { get; set; }

        public String email { get; set; }

        public bool? active { get; set; }

        public PersistentListChangeHandler<Rights> rightsListChangeHandler { get; set; }

        public PersistentListChangeHandler<Relation> relationsListChangeHandler { get; set; }


        public User()
        {
            this.active = true;
            rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
            relationsListChangeHandler = new PersistentListChangeHandler<Relation>();
        }

        public String ToString()
        {
            return this.name;
        }
    }
    
}
