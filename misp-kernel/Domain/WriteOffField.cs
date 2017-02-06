using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class WriteOffField : Persistent
    {
      
    	public WriteOffFieldType writeOffFieldType;
	
	    public int position;
        
	    public Boolean mandatory;
	
	    public Attribute attributeField;
	
	
	    public PeriodName periodField;
	
	    public Measure measureField;

        public PersistentListChangeHandler<WriteOffFieldValue> valueListChangeHandler;

        public WriteOffField() 
        {
            valueListChangeHandler = new PersistentListChangeHandler<WriteOffFieldValue>();
        }

    }
}
