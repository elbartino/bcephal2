using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationFilterTemplate : Persistent
    {

        public String name { get; set; }

        public Grille leftGrid { get; set; }

        public Grille rigthGrid { get; set; }

        public Grille bottomGrid { get; set; }

        public Boolean visibleInShortcut { get; set; }

        public BGroup group { get; set; }

        public Attribute reconciliationType { get; set; }

        public Measure amountMeasure { get; set; }

        public BalanceFormula balanceFormula { get; set; }

        public DebitCreditFormula debitCreditFormula { get; set; }

        public bool acceptWriteOff { get; set; }
	
	
	    public WriteOffConfiguration writeOffConfig;
	
	
	    /**
	        * Default constructor
	        */
	    public ReconciliationFilterTemplate() {
		    this.visibleInShortcut = true;
		    this.acceptWriteOff = true;
	    }
    }
}
