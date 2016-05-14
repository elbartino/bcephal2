using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Operation
    {
        public static Operation PLUS = new Operation("PLUS", "+");

        public static Operation SUB = new Operation("SUB", "-");

        public static Operation MULT = new Operation("MULT", "*");

        public static Operation DIVIDE = new Operation("DIVIDE", "/");

        public static Operation POWER = new Operation("POWER", "^");

        public String sign { get; set; }
        public String name { get; set; }

        private Operation(String name, String sign)
        {
            this.name = name;
            this.sign = sign;
        }

        public static Operation getBySign(String sign)
        {
            if (string.IsNullOrEmpty(sign)) return null;
            if (PLUS.sign.Equals(sign)) return PLUS;
            if (SUB.sign.Equals(sign)) return SUB;
            if (MULT.sign.Equals(sign)) return MULT;
            if (DIVIDE.sign.Equals(sign)) return DIVIDE;
            if (POWER.sign.Equals(sign)) return POWER;
            return null;
        }

        public static Operation getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (PLUS.name.Equals(name)) return PLUS;
            if (SUB.name.Equals(name)) return SUB;
            if (MULT.name.Equals(name)) return MULT;
            if (DIVIDE.name.Equals(name)) return DIVIDE;
            if (POWER.name.Equals(name)) return POWER;
            return null;
        }
    }
}
