using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleRelationships : Persistent
    {

        public PersistentListChangeHandler<GrilleRelationship> relationshipListChangeHandler { get; set; }

        /**
         * Constructor
         */
        public GrilleRelationships() {
		    relationshipListChangeHandler = new PersistentListChangeHandler<GrilleRelationship>();
	    }

    }
}
