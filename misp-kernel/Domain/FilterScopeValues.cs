using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class FilterScopeValues
    {
        
        public static FilterScopeValues IS_BLANCK = new FilterScopeValues("Is blanck");

        public static FilterScopeValues NOT_BLANCK = new FilterScopeValues("Is not blanck");

        public static FilterScopeValues BEGIN_WITH = new FilterScopeValues("Begin with");

        public static FilterScopeValues NOT_BEGIN_WITH = new FilterScopeValues("Does not begin with");

        public static FilterScopeValues CONTAINS = new FilterScopeValues("Contains");

        public static FilterScopeValues NOT_CONTAINS = new FilterScopeValues("Does not Contains");

        public String label { get; set; }

        private FilterScopeValues(String label)
        {
            this.label = label;
        }
        
        public override string ToString()
        {
            return label;
        }

        public static FilterScopeValues getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (IS_BLANCK.label.Equals(label)) return IS_BLANCK;
            if (NOT_BLANCK.label.Equals(label)) return NOT_BLANCK;
            if (BEGIN_WITH.label.Equals(label)) return BEGIN_WITH;
            if (NOT_BEGIN_WITH.label.Equals(label)) return NOT_BEGIN_WITH;
            if (CONTAINS.label.Equals(label)) return CONTAINS;
            if (NOT_CONTAINS.label.Equals(label)) return NOT_CONTAINS;
            return null;
        }
    }
}
