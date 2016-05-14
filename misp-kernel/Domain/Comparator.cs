using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Comparator
    {
        public static Comparator EQUALS = new Comparator("EQUALS", "=");

        public static Comparator LOWER = new Comparator("LOWER", "<");

        public static Comparator LOWER_OR_EQUALS = new Comparator("LOWER_OR_EQUALS", "<=");

        public static Comparator GREATER = new Comparator("GREATER", ">");

        public static Comparator GREATER_OR_EQUALS = new Comparator("GREATER_OR_EQUALS", ">=");

        public String sign { get; set; }
        public String name { get; set; }

        private Comparator(String name, String sign)
        {
            this.name = name;
		    this.sign = sign;
	    }

        public static Comparator getBySign(String sign)
        {
            if (string.IsNullOrEmpty(sign)) return null;
            if (LOWER.sign.Equals(sign)) return LOWER;
            if (GREATER.sign.Equals(sign)) return GREATER;
            if (EQUALS.sign.Equals(sign)) return EQUALS;
            if (LOWER_OR_EQUALS.sign.Equals(sign)) return LOWER_OR_EQUALS;
            if (GREATER_OR_EQUALS.sign.Equals(sign)) return GREATER_OR_EQUALS;
            return null;
        }

        public static Comparator getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (LOWER.name.Equals(name)) return LOWER;
            if (GREATER.name.Equals(name)) return GREATER;
            if (EQUALS.name.Equals(name)) return EQUALS;
            if (LOWER_OR_EQUALS.name.Equals(name)) return LOWER_OR_EQUALS;
            if (GREATER_OR_EQUALS.name.Equals(name)) return GREATER_OR_EQUALS;
            return null;
        }
    }
}
