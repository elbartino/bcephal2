using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationFilterTemplate : Persistent
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

        public void setWriteOffConfiguration(WriteOffConfiguration writeoffconfig)
        {
            this.writeOffConfig = writeoffconfig;
        }

        public void setDebitCreditFormula(DebitCreditFormula debCredFor) 
        {
            this.debitCreditFormula = debCredFor;
        }

        public void setMeasure(Measure measure)
        {
            this.amountMeasure = measure;
        }

        public void setReconciliationType(Domain.Attribute attribute)
        {
            this.reconciliationType = attribute;
        }

        public void setBalanceFormula(BalanceFormula balanceFor) 
        {
            this.balanceFormula = balanceFor;
        }

        public void setGroup(BGroup groupe)
        {
            this.group = groupe;
        }

        public void setVisibleInShorcut(bool visibleinShortc) 
        {
            this.visibleInShortcut = visibleinShortc;
        }

        public void setLeftGrid(Grille leftgrid) 
        {
            this.leftGrid = leftgrid;
        }

        public void setRigthGrid(Grille rigthgrid)
        {
            this.rigthGrid = rigthgrid;
        }

        public void setBottomGrid(Grille bottomgrid)
        {
            this.bottomGrid = bottomgrid;
        }

        public void setAcceptWriteOff(bool accept)
        {
            this.acceptWriteOff = accept;
        }

    }
}
