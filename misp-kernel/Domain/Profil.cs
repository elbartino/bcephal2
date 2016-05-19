using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Profil : Persistent
    {
        public String name { get; set; }

        public Boolean active { get; set; }
	  
        public PersistentListChangeHandler<Rights> rightsListChangeHandler;

        public Profil()
        {
            rightsListChangeHandler = new PersistentListChangeHandler<Rights>();
        }
    }
}
