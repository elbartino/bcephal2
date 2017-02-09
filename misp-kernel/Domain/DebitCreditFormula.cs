using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
 public class DebitCreditFormula
    {
        public static String DEBIT_NEGATIVE_NAME = "DEBIT_NEGATIVE";
        public static String DEBIT_NOT_NEGATIVE_NAME = "DEBIT_NOT_NEGATIVE";

        public static DebitCreditFormula DEBIT_NEGATIVE = new DebitCreditFormula(DEBIT_NEGATIVE_NAME, "Consider 'D' as negative amount");
        public static DebitCreditFormula DEBIT_NOT_NEGATIVE = new DebitCreditFormula(DEBIT_NOT_NEGATIVE_NAME, "Do not consider 'D' as negative amount");

        public String label { get; set; }
        public String name { get; set; }

        private DebitCreditFormula(String name, String label)
        {
            this.name = name;
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static DebitCreditFormula getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (DEBIT_NEGATIVE.name.Equals(name)) return DEBIT_NEGATIVE;
            if (DEBIT_NOT_NEGATIVE.name.Equals(name)) return DEBIT_NOT_NEGATIVE;
            return null;
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
