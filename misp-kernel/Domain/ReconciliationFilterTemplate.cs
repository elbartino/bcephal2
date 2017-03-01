using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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

        public Measure leftMeasure { get; set; }

        public Measure rightMeasure { get; set; }

        public Measure writeoffMeasure { get; set; }
        
        [ScriptIgnore]
        public WriteOffFieldValueType writeoffDefaultMeasureTypeEnum { get; set; }

        public String writeoffDefaultMeasureType
        {
            get { return this.writeoffDefaultMeasureTypeEnum != null ? this.writeoffDefaultMeasureTypeEnum.name : null; }
            set { this.writeoffDefaultMeasureTypeEnum = WriteOffFieldValueType.getByName(value); }
        }
        
        public String balanceFormula 
        {
            get { return this.balanceFormulaEnum != null ? this.balanceFormulaEnum.name : null; }
            set { this.balanceFormulaEnum = BalanceFormula.getByName(value); }
        }

        public bool? useDebitCredit { get; set; }
                
        public bool acceptWriteOff { get; set; }
                
	    public WriteOffConfiguration writeOffConfig;

        [ScriptIgnore]
        public BalanceFormula balanceFormulaEnum { get; set; }
        
	
	    /// <summary>
	    /// 
	    /// </summary>
	    public ReconciliationFilterTemplate() {
		    this.visibleInShortcut = true;
		    this.acceptWriteOff = true;
            this.leftGrid = new Grille();            
            this.rigthGrid = new Grille();
            this.bottomGrid = new Grille();
            this.leftGrid.name = "Left";
            this.rigthGrid.name = "Right";
            this.bottomGrid.name = "Bottom";
            this.leftGrid.report = true;
            this.rigthGrid.report = true;
            this.bottomGrid.report = true;
            this.leftGrid.reconciliation = true;
            this.rigthGrid.reconciliation = true;
            this.bottomGrid.reconciliation = true;

            this.balanceFormulaEnum = BalanceFormula.LEFT_MINUS_RIGHT;
	    }
        
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Grille)) return 1;
            return this.name.CompareTo(((Grille)obj).name);
        }    
    
    }

}
