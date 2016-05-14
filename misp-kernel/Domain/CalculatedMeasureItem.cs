using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class CalculatedMeasureItem : Persistent
    {
        
        public enum MeasureType {MEASURE,CALCULATED_MEASURE, AMOUNT}

    

        public bool openPar { get; set; }
    
        public bool closePar { get; set; }

        public int position { get; set; }

        public decimal amount { get; set; }

        public string sign { get; set; }

        public bool ignoreAll { get; set; }
        public bool ignoreTableObject { get; set; }
        public bool ignoreTableVc { get; set; }
        public bool ignoreTablePeriod { get; set; }
        public bool ignoreCellObject { get; set; }
        public bool ignoreCellVc { get; set; }
        public bool ignoreCellPeriod { get; set; }

        public string measureType { get; set; }

        public Measure measure { get; set; }

        [ScriptIgnore]
        public CalculatedMeasure calculatedMeasure { get; set; }


        public CalculatedMeasureItem() 
        {
            measureType = MeasureType.MEASURE.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is CalculatedMeasureItem)) return 1;
            return this.position.CompareTo(((CalculatedMeasureItem)obj).position);
        }

        /// <summary>
        /// retourne la valeur a affiché selon le type
        /// </summary>
        /// <returns>valueToDisplay</returns>
        public string GetValue()
        {

            if (measure != null)
            {
                if (measureType == MeasureType.MEASURE.ToString() || measureType == MeasureType.CALCULATED_MEASURE.ToString())
                {
                    if (measure.name.Equals("NOT_FOUND_MEASURE")) return "empty measure";
                    return measure.name;
                }
            }
            else
            {
                return amount != 0 ? Math.Round(amount).ToString() : ""; 
            }
           
            return "";
        }
        public bool isASign()
        {
            bool isasign = (this.sign != null) && ((this.sign.Equals("+") || this.sign.Equals("-") || this.sign.Equals("/") || this.sign.Equals("^") || this.sign.Equals("x"))) ? true : false;
           return isasign;
        }



        /// <summary>
        /// Définit le parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(CalculatedMeasure parent) { this.calculatedMeasure = parent; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetParent() { return this.calculatedMeasure; }

        /// <summary>
        /// Définit la position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position) { this.position = position; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPosition() { return this.position; }

        /// <summary>
        /// copie exacte
        /// </summary>
        /// <returns></returns>
        public CalculatedMeasureItem GetCopy(CalculatedMeasureItem item)
        {
            if(item==null)
            item = new CalculatedMeasureItem();
            item.amount = this.amount;

            item.measure = this.measure;
            item.measureType = this.measureType;
            item.sign = this.sign;
            return item;
        }



        public string getItemExpression()
        {
            string op = "";

            /*if (this.openPar == true && this.closePar == true)
            {
                op = "(";
                if (measure != null) op = op + measure.name;
                else if (amount != 0) op = op + amount.ToString();
                return op+")";
            }
            if (this.openPar == true){
                op = "(";
                if (measure != null) op = op + measure.name; 
                else if (amount != 0) op = op + amount.ToString();
                return op;
            }

            if (this.closePar == true)
            { 
                op = ")"; return op;
            }

            if (this.sign != null && measure != null)
            { 
                op = this.sign + measure.name; return op; 
            }
            if (this.sign != null && amount != 0)
            { 
                op = this.sign + amount.ToString(); return op; 
            }

            if (this.sign != null && measure == null && amount == 0 && !this.sign.Equals("="))
            { 
                op = this.sign; return op; 
            }

            if (this.sign == null && measure != null && !openPar && !closePar)
            { 
                op = measure.name; return op; 
            }
            if (this.sign == null && amount != 0 && !openPar && !closePar)
            { 
                op = amount.ToString(); return op; 
            }

            if (this.sign != null && measure == null && amount == 0)
            {
                op = this.sign; return op; 
            }*/

            
            if (this.sign != null)
            {
                op += this.sign;
                if (this.openPar)
                    op += "(";
            }
            else
            {
                if (this.openPar)
                    op += "(";
            }

            if (this.measure != null)
                op = op + measure.name;
            if (this.amount != 0)
                op = op + Math.Round(amount);
            if (this.closePar)
                op = op + ")";
           
            return op;
        }
    }
}
