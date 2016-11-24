using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleRelationship : Persistent
    {

        
        public PersistentListChangeHandler<GrilleRelationshipItem> itemListChangeHandler { get; set; }

        /**
         * Constructor
         */
        public GrilleRelationship() {
            itemListChangeHandler = new PersistentListChangeHandler<GrilleRelationshipItem>();
	    }

    }
}
