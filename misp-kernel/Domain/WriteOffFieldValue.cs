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
    }
}
