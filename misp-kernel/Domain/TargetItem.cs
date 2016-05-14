using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class TargetItem : Persistent
    {

        public enum Operator { AND, OR, NOT }

        public static Operator getOperatorByStringValue(String operatorString) 
        {
            if (operatorString == Operator.AND.ToString()) return Operator.AND;
            if (operatorString == Operator.NOT.ToString()) return Operator.NOT;
            if (operatorString == Operator.OR.ToString()) return Operator.OR;
            return Operator.AND;
        }
        
        private int _position;
        private string _openingBracket;
        private string _closingBracket;
        private string _operatorType;
        private Target _value;

        
        public String refValueName { get; set; }

        public bool isRefScope { get; set; }

        public bool isDeleted { get; set; }

        public TargetItem() { }

        public TargetItem(int position) { this.position = position; }

        public TargetItem(int position, Target target, string formula, string operatorType)
        {
            this.position = position;
            this._value = target;
            this.formula = formula;
            this.operatorType = operatorType;
            
        }

        public TargetItem(Target target,Kernel.Domain.Attribute attribute ,string formula)
        {
            this.attribute = attribute;
            this._value = target;
            this.formula = formula;
        }


        public string nameSheet { get; set; }

        public string openingBracket
        {
            get { return _openingBracket; }

            set
            {
                _openingBracket = value;
                this.OnPropertyChanged("openingBracket");
            }
        }

        public string closingBracket
        {
            get { return _closingBracket; }

            set
            {
                _closingBracket = value;
                this.OnPropertyChanged("closingBracket");
            }
        }

        public Target value
        {
            get { return _value; }

            set
            {
                _value = value;
                this.OnPropertyChanged("value");
            }
        }

        public TransformationTreeItem loop { get; set; }

        public string operatorType
        {
            get { return _operatorType; }

            set
            {
                _operatorType = value;
                this.OnPropertyChanged("operatorType");
            }
        }
        
        [ScriptIgnore]
        public Target parent { get; set; }

        
        public Attribute attribute { get; set; }
        public String formula { get; set; }

        public int position
        {
            get { return _position; }

            set
            {
                _position = value;
                this.OnPropertyChanged("position");
            }
        }

        [ScriptIgnore]
        public String name
        {
            get
            {
                string text = _position == 0 ? "" : operatorType + " ";
                text = text + (value != null ? value.name : (loop != null ? loop.name : ""));
                return text;
            }
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TargetItem)) return 1;
            return this.position.CompareTo(((TargetItem)obj).position);
        }


        public TargetItem GetCopy()
        {
            TargetItem item = new TargetItem();
            item.value = this.value;
            item.operatorType = this.operatorType;
            item.position = this.position;
            item.openingBracket = this.openingBracket;
            item.closingBracket = this.closingBracket;
            item.formula = this.formula;
            return item;
        }

    }
}
