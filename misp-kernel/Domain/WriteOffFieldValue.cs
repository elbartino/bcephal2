using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class WriteOffFieldValue : Persistent
    {
   
	    public WriteOffFieldValueType defaultValueType;
	

	    public int position;
	
	
	    public Attribute attribute;
	
	
	    public AttributeValue attributeValue;
	
	
	    public PeriodInterval period;
	
	    public decimal measure;
	
	
	
	    /**
	     * Default constructor
	     */
	    public WriteOffFieldValue() {
	
	    }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is WriteOffFieldValue)) return 1;
            return this.position.CompareTo(((WriteOffFieldValue)obj).position);
        }


        public void setValue(AttributeValue value)
        {
            this.attribute = null;
            this.attributeValue = value;
            this.period = null;
        }

        public void setAttribute(Attribute attribute)
        {
            this.attribute = attribute;
            this.attributeValue = null;
            this.period = null;
        }

        public void setPeriodInterval(PeriodInterval interval)
        {
            this.attribute = null;
            this.attributeValue = null;
            this.period = interval;
        }

        public void setDecimalMeasure(decimal decimalMeasure)
        {

        }

    }
}
