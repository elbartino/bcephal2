using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
 public class DebitCreditFormula
    {
        public static DebitCreditFormula DEBIT_NEGATIVE = new DebitCreditFormula("(-) Debit");

        public static DebitCreditFormula DEBIT_NOT_NEGATIVE = new DebitCreditFormula("(+) Debit");

        public String label { get; set; }

        private DebitCreditFormula(String label)
        {
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static DebitCreditFormula getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (DEBIT_NEGATIVE.label.Equals(label)) return DEBIT_NEGATIVE;
            if (DEBIT_NOT_NEGATIVE.label.Equals(label)) return DEBIT_NOT_NEGATIVE;
            return null;
        }

       
    }
}
