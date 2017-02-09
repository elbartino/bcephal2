using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class BalanceFormula
    {
        public static String LEFT_MINUS_RIGHT_NAME = "LEFT_MINUS_RIGHT";
        public static String LEFT_PLUS_RIGHT_NAME = "LEFT_PLUS_RIGHT";

        public static BalanceFormula LEFT_MINUS_RIGHT = new BalanceFormula(LEFT_MINUS_RIGHT_NAME, "Left - Right = 0");

        public static BalanceFormula LEFT_PLUS_RIGHT = new BalanceFormula(LEFT_PLUS_RIGHT_NAME, "Left + Right = 0");

        public String label { get; set; }

        public String name { get; set; }

        private BalanceFormula(String name, String label)
        {
            this.name = name;
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static BalanceFormula getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (LEFT_MINUS_RIGHT.name.Equals(name)) return LEFT_MINUS_RIGHT;
            if (LEFT_PLUS_RIGHT.name.Equals(name)) return LEFT_PLUS_RIGHT;
            return null;
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