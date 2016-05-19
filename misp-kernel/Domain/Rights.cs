using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Domain
{
    public class Rights : Persistent
    {
    	public Profil profil { get; set;}

    	public User user { get; set;}
	
	    public String functionnality { get; set;}

        public UserAction action { get; set; }

        public override string ToString()
        {
		    return functionnality + " - " + action;
        }
    }
}
