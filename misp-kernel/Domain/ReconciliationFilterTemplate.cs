using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationFilterTemplate
    {
	    public Grille leftGrid;

	
	    public Grille rigthGrid;
	
	
	    public Grille bottomGrid;
	
	
	    public Boolean visibleInShortcut;
	
	
	    public BGroup group;
	

	    public Attribute reconciliationType;
	
	
	    public Measure amountMeasure;
	

	    public BalanceFormula balanceFormula;
	

	    public DebitCreditFormula debitCreditFormula;
	

	    public bool acceptWriteOff;
	
	
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
