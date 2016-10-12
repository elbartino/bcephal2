using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class LoopCondition : Persistent
    {
        public int position { get; set; }
        
        public CellProperty cellProperty { get; set; }

        public string comment { get; set; }

        public string conditions { get; set; }

        [ScriptIgnore]
        public Instruction instructions { get; set; }

        public string openBracket { get; set; }

        public string closeBracket { get; set; }

        public string operatorType { get; set; }

        [ScriptIgnore]
        public String opratorType
        {
            get { return !string.IsNullOrEmpty(operatorType) ? Operator.getBySign(operatorType).sign : Operator.AND.sign; }
            set { this.operatorType = Operator.getBySign(value).name; }
        }
        
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is LoopCondition)) return 1;
            return this.position.CompareTo(((LoopCondition)obj).position);
        }

        public bool isConditionsEmpty()
        {
            string emptyCondition = "BLOCK IF COND START AND Result ENDSTART END END THEN CONTINUE END ENDTHEN ELSE STOP END ENDELSE END";
            return conditions == null || !string.IsNullOrWhiteSpace(conditions) || conditions.Equals(emptyCondition);
        }
    }
}
