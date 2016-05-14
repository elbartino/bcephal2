using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class PeriodItem : Persistent
    {
        protected static String ITEM_SEPARATOR = "-:-";

        public PeriodItem() { }

        public PeriodItem(int position) { this.position = position >= 0 ? position : 0; }

        public PeriodItem(int position, string name, string value, string formula)
        {
            this.position = position;
            this.name = name;
            this.value = value;
            this.formula = formula;
        }

        public TransformationTreeItem loop { get; set; }

        public string name { get; set; }

        public string value { get; set; }

        public string formula { get; set; }

        public string sheet { get; set; }

        public string operationNumber { get; set; }

        [ScriptIgnore]
        public int numberPeriod { get; set; }

        public int position { get; set; }

        public string operationDate { get; set; }

        public string openBracket { get; set; }

        public string closeBracket { get; set; }

        public string operatorType { get; set; }

        public string comparator { get; set; }

        public string operation { get; set; }

        public string operationGranularity { get; set; }

        [ScriptIgnore]
        public String operatorSign
        {
            get { return !string.IsNullOrEmpty(operationDate) ? DateOperator.getByName(operationDate).sign : DateOperator.EQUALS.sign; }
            set { this.operationDate = DateOperator.getBySign(value).name; }
        }

        [ScriptIgnore]
        public String openBraket
        {
            get { return !string.IsNullOrEmpty(openBracket) ? Bracket.getByLabel(openBracket).label : Bracket.OPEN_BRACKET.label; }
            set { this.openBracket = Bracket.getByLabel(value).label; }
        }

        [ScriptIgnore]
        public String closeBraket
        {
            get { return !string.IsNullOrEmpty(closeBracket) ? Bracket.getByLabel(closeBracket).label : Bracket.CLOSE_BRACKET.label; }
            set { this.closeBracket = Bracket.getByLabel(value).label; }
        }

        [ScriptIgnore]
        public String opratorType
        {
            get { return !string.IsNullOrEmpty(operatorType) ? Operator.getBySign(operatorType).sign : Operator.AND.sign; }
            set { this.operatorType = Operator.getBySign(value).name; }
        }

        [ScriptIgnore]
        public String comprator
        {
            get { return !string.IsNullOrEmpty(comparator) ? Comparator.getBySign(comparator).sign : Comparator.EQUALS.sign; }
            set { this.comparator = Comparator.getBySign(value).name; }
        }

        [ScriptIgnore]
        public String opration
        {
            get { return !string.IsNullOrEmpty(operation) ? Operation.getBySign(operation).sign : Operation.PLUS.sign; }
            set { this.operation = Operation.getBySign(value).name; }
        }

        [ScriptIgnore]
        public String granularity
        {
            get { return !string.IsNullOrEmpty(operationGranularity) ? Granularity.getByName(operationGranularity).ToString() : Granularity.YEAR.ToString(); }
            set { this.operationGranularity = Granularity.getByName(value).ToString(); }
        }

        [ScriptIgnore]
        public Period period { get; set; }

        [ScriptIgnore]
        public DateTime valueDateTime
        {
            get { return !string.IsNullOrEmpty(value) ? DateTime.Parse(value) : new DateTime(); }
            set { this.value = value.ToShortDateString(); }
        }

        [ScriptIgnore]
        public bool isFormula { get { return formula != null && formula.StartsWith("="); } }

        public String getFormuleSubEqual()
        {
            if (isFormula)
            {
                return formula.Substring(1);
            }
            return formula;
        }

        public override string ToString()
        {
            return this.value != null ? this.value : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is PeriodItem)) return 1;
            return this.position.CompareTo(((PeriodItem)obj).position);
        }

        public PeriodItem GetCopy()
        {
            PeriodItem item = new PeriodItem();
            item.name = this.name;
            item.value = this.value;
            item.formula = this.formula;
            item.position = this.position;
            return item;
        }

        public String asString()
        {
            String stringItem = "" + position;
            stringItem += ITEM_SEPARATOR + (name != null ? name : " ")
                    + ITEM_SEPARATOR + (operatorType != null ? operatorSign : " ")
                    + ITEM_SEPARATOR + (value != null ? value : " ")
                    + ITEM_SEPARATOR + (formula != null ? formula : " ")
                    + ITEM_SEPARATOR + (sheet != null ? sheet : " ");
            return stringItem;
        }


        public bool isEmpty()
        {
            return string.IsNullOrEmpty(value) && string.IsNullOrEmpty(formula) && loop == null;
        }


        public bool isInValidDefault()
        {
            return  this.name == PeriodName.DEFAULT_DATE_NAME && this.value == null && this.formula == null;
        }


        [ScriptIgnore]
        public String description
        {
            get
            {
                String desc = name != null ? name : "";
                desc += " = ";
                desc += !String.IsNullOrWhiteSpace(value) ? value : "";
                if (!String.IsNullOrWhiteSpace(operationNumber) && !operationNumber.Equals("null"))
                {
                    desc += " " + operation + " " + operationNumber + " " + operationGranularity;
                }
                return desc;
            }
        }

    }
}
