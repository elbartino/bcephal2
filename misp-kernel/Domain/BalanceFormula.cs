using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class BalanceFormula
    {

        public static BalanceFormula LEFT_MINUS_RIGHT = new BalanceFormula("Left - Right");

        public static BalanceFormula LEFT_PLUS_RIGHT = new BalanceFormula("Left + Right");

        public String label { get; set; }

        private BalanceFormula(String label)
        {
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static BalanceFormula getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (LEFT_MINUS_RIGHT.label.Equals(label)) return LEFT_MINUS_RIGHT;
            if (LEFT_PLUS_RIGHT.label.Equals(label)) return LEFT_PLUS_RIGHT;
            return null;
        }
    }
}