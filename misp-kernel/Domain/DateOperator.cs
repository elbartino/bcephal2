using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class DateOperator
    {

        public static DateOperator EQUALS = new DateOperator("EQUALS", "=");
	
	    public static DateOperator BEFORE = new DateOperator("BEFORE", "<");
	
	    public static DateOperator BEFORE_OR_EQUALS = new DateOperator("BEFORE_OR_EQUALS", "<=");

        public static DateOperator AFTER = new DateOperator("AFTER", ">");
	
	    public static DateOperator AFTER_OR_EQUALS = new DateOperator("AFTER_OR_EQUALS", ">=");


        public static DateOperator DURING_PERIOD = new DateOperator("DURING_PERIOD", "During period");
        public static DateOperator BEFORE_START_PERIOD = new DateOperator("BEFORE_START_PERIOD", "Before start period");
        public static DateOperator BEFORE_END_PERIOD = new DateOperator("BEFORE_END_PERIOD", "Before end period");
        public static DateOperator AFTER_START_PERIOD = new DateOperator("AFTER_START_PERIOD", "After Start period");
        public static DateOperator AFTER_END_PERIOD = new DateOperator("AFTER_END_PERIOD", "After End period");

        

        public String sign { get; set; }
        public String name { get; set; }

        private DateOperator(String name, String sign) {
            this.name = name;
		    this.sign = sign;
	    }

        public static DateOperator getBySign(String sign)
        {
            if (string.IsNullOrEmpty(sign)) return null;
            if (AFTER.sign.Equals(sign)) return AFTER;
            if (BEFORE.sign.Equals(sign)) return BEFORE;
            if (EQUALS.sign.Equals(sign)) return EQUALS;
            if (BEFORE_OR_EQUALS.sign.Equals(sign)) return BEFORE_OR_EQUALS;
            if (AFTER_OR_EQUALS.sign.Equals(sign)) return AFTER_OR_EQUALS;
            if (DURING_PERIOD.sign.Equals(sign)) return DURING_PERIOD;
            if (BEFORE_START_PERIOD.sign.Equals(sign)) return BEFORE_START_PERIOD;
            if (BEFORE_END_PERIOD.sign.Equals(sign)) return BEFORE_END_PERIOD;
            if (AFTER_START_PERIOD.sign.Equals(sign)) return AFTER_START_PERIOD;
            if (AFTER_END_PERIOD.sign.Equals(sign)) return AFTER_END_PERIOD;
            return null;
        }

        public static DateOperator getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (AFTER.name.Equals(name)) return AFTER;
            if (BEFORE.name.Equals(name)) return BEFORE;
            if (EQUALS.name.Equals(name)) return EQUALS;
            if (BEFORE_OR_EQUALS.name.Equals(name)) return BEFORE_OR_EQUALS;
            if (AFTER_OR_EQUALS.name.Equals(name)) return AFTER_OR_EQUALS;
            if (DURING_PERIOD.name.Equals(name)) return DURING_PERIOD;
            if (BEFORE_START_PERIOD.name.Equals(name)) return BEFORE_START_PERIOD;
            if (BEFORE_END_PERIOD.name.Equals(name)) return BEFORE_END_PERIOD;
            if (AFTER_START_PERIOD.name.Equals(name)) return AFTER_START_PERIOD;
            if (AFTER_END_PERIOD.name.Equals(name)) return AFTER_END_PERIOD;
            return null;
        }
    }
}
