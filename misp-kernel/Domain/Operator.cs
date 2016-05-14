using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Operator
    {
        public static Operator AND = new Operator("AND");

        public static Operator OR = new Operator("OR");

        public static Operator NOT = new Operator("NOT");

        public String sign { get; set; }
        public String name { get; set; }

        private Operator(String name)
        {
            this.name = name;
		    this.sign = sign;
	    }

        public static Operator getBySign(String sign)
        {
            if (string.IsNullOrEmpty(sign)) return null;
            if (AND.sign.Equals(sign)) return AND;
            if (OR.sign.Equals(sign)) return OR;
            if (NOT.sign.Equals(sign)) return NOT;
            return null;
        }

        public static Operator getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (AND.name.Equals(name)) return AND;
            if (OR.name.Equals(name)) return OR;
            if (NOT.name.Equals(name)) return NOT;
            return null;
        }
    }
}
