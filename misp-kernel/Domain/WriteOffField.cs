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
        
	    public bool mandatory;
	
	    public Attribute attributeField;
	
	    public PeriodName periodField;
	
	    public Measure measureField;

        public PersistentListChangeHandler<WriteOffFieldValue> valueListChangeHandler;

        public WriteOffField() 
        {
            valueListChangeHandler = new PersistentListChangeHandler<WriteOffFieldValue>();
        }

        public void setMeasure(Measure measure)
        {
            this.measureField = measure;
            this.attributeField = null;
            this.periodField = null;
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.measureField = null;
            this.attributeField = attribute;
            this.periodField = null;
        }

        public void setPeriodName(Kernel.Domain.PeriodName periodName)
        {
            this.measureField = null;
            this.attributeField = null;
            this.periodField = periodName;
        }

        public void AddFieldValue(WriteOffFieldValue fieldValue) 
        {
            fieldValue.position = valueListChangeHandler.Items.Count;
            valueListChangeHandler.AddNew(fieldValue);
        }

        public void UpdateFieldValue(WriteOffFieldValue fieldValue)
        {
            valueListChangeHandler.AddUpdated(fieldValue);
        }

        public void DeleteFieldValue(WriteOffFieldValue fieldValue)
        {
            fieldValue.position = -1;
            valueListChangeHandler.AddDeleted(fieldValue);
            foreach(WriteOffFieldValue fvalue in valueListChangeHandler.Items)
            {
                if(fvalue.position > fieldValue.position) fvalue.position = fvalue.position -1;
            }
        }


        public void ForgetFieldValue(WriteOffFieldValue fieldValue)
        {
            valueListChangeHandler.forget(fieldValue);
            foreach (WriteOffFieldValue fvalue in valueListChangeHandler.Items)
            {
                if (fvalue.position > fieldValue.position) fvalue.position = fvalue.position - 1;
            }
            fieldValue.position = -1;
        }
        

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is WriteOffField)) return 1;
            return this.position.CompareTo(((WriteOffField)obj).position);
        }

    }
}
