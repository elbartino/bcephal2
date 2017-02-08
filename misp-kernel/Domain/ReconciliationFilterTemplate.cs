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
	
	
	    /// <summary>
	    /// 
	    /// </summary>
	    public ReconciliationFilterTemplate() {
		    this.visibleInShortcut = true;
		    this.acceptWriteOff = true;
            this.leftGrid = new Grille();            
            this.rigthGrid = new Grille();
            this.bottomGrid = new Grille();
            this.leftGrid.report = true;
            this.rigthGrid.report = true;
            this.bottomGrid.report = true;
            this.leftGrid.reconciliation = true;
            this.rigthGrid.reconciliation = true;
            this.bottomGrid.reconciliation = true;
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


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Grille)) return 1;
            return this.name.CompareTo(((Grille)obj).name);
        }    }
}
