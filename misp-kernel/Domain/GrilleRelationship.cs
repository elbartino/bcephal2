using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class GrilleRelationship : Persistent
    {

        public int position { get; set; }
        public GrilleColumn primaryColumn { get; set; }
        public PersistentListChangeHandler<GrilleRelationshipItem> itemListChangeHandler { get; set; }

        /**
         * Constructor
         */
        public GrilleRelationship() {
            itemListChangeHandler = new PersistentListChangeHandler<GrilleRelationshipItem>();
	    }

    }
}
