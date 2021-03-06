﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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

        public PersistentListChangeHandler<WriteOffFieldValue> writeOffFieldValueListChangeHandler;

        public AttributeValue value;

        public String date;

        [ScriptIgnore]
        public WriteOffFieldValueType defaultValueTypeEnum { get; set; }

        public String defaultValueType
        {
            get { return this.defaultValueTypeEnum != null ? this.defaultValueTypeEnum.name : null; }
            set { this.defaultValueTypeEnum = WriteOffFieldValueType.getByName(value); }
        }

        [ScriptIgnore]
        public DateTime dateTime
        {
            get { return !string.IsNullOrEmpty(date) ? DateTime.Parse(date) : new DateTime(); }
            set { this.date = value.ToShortDateString(); }
        }

        public WriteOffField() 
        {
            writeOffFieldValueListChangeHandler = new PersistentListChangeHandler<WriteOffFieldValue>();
        }

        public void setMeasure(Measure measure)
        {
            this.measureField = measure;
            this.attributeField = null;
            this.periodField = null;
            this.writeOffFieldType = WriteOffFieldType.MEASURE;
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.measureField = null;
            this.attributeField = attribute;
            this.periodField = null;
            this.writeOffFieldType = WriteOffFieldType.ATTRIBUTE;
        }

        public void setPeriodName(Kernel.Domain.PeriodName periodName)
        {
            this.measureField = null;
            this.attributeField = null;
            this.periodField = periodName;
            this.writeOffFieldType = WriteOffFieldType.PERIOD;
        }

        public void AddFieldValue(WriteOffFieldValue fieldValue) 
        {
            fieldValue.position = writeOffFieldValueListChangeHandler.Items.Count;
            writeOffFieldValueListChangeHandler.AddNew(fieldValue);
        }

        public void UpdateFieldValue(WriteOffFieldValue fieldValue)
        {
            writeOffFieldValueListChangeHandler.AddUpdated(fieldValue);
        }

        public void DeleteFieldValue(WriteOffFieldValue fieldValue)
        {
            fieldValue.position = -1;
            writeOffFieldValueListChangeHandler.AddDeleted(fieldValue);
            foreach(WriteOffFieldValue fvalue in writeOffFieldValueListChangeHandler.Items)
            {
                if(fvalue.position > fieldValue.position) fvalue.position = fvalue.position -1;
            }
        }


        public void ForgetFieldValue(WriteOffFieldValue fieldValue)
        {
            writeOffFieldValueListChangeHandler.forget(fieldValue);
            foreach (WriteOffFieldValue fvalue in writeOffFieldValueListChangeHandler.Items)
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
                

        public void SynchronizeDeleteWriteOffFieldValue(WriteOffFieldValue writeOffFieldValue)
        {
            WriteOffFieldValue foundItem = this.GetWriteOffFieldValue(writeOffFieldValue.position);
            if (foundItem == null) return;
            DeleteFieldValue(foundItem);
            this.isModified = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public WriteOffFieldValue SynchronizeWriteOffFieldValue(WriteOffFieldValue writeOffField)
        {
            WriteOffFieldValue foundItem = this.GetWriteOffFieldValue(writeOffField.position);
            if (foundItem == null)
            {
                foundItem = new WriteOffFieldValue();
                foundItem.attribute = writeOffField.attribute;
                foundItem.measure = writeOffField.measure;
                foundItem.attributeValue = writeOffField.attributeValue;
                AddFieldValue(foundItem);
            }
            else
            {
                foundItem.position = writeOffField.position;
                foundItem.attribute = writeOffField.attribute;
                foundItem.measure = writeOffField.measure;
                foundItem.attributeValue = writeOffField.attributeValue;
                UpdateFieldValue(foundItem);
            }
            this.isModified = true;
            return foundItem;
        }

        /// <summary>
        /// Retourne l'item à la position spécifiée.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public WriteOffFieldValue GetWriteOffFieldValue(int position)
        {
            foreach (WriteOffFieldValue item in writeOffFieldValueListChangeHandler.Items)
            {
                if (item.position == position) return item;
            }
            return null;
        }

        public void setDefaultValue(object item)
        {
            this.defaultValueTypeEnum = WriteOffFieldValueType.getByLabel(item.ToString());
        }

        public bool isPeriod()
        {
            return this.periodField != null;
        }

        public bool isAttribute()
        {
            return this.attributeField != null;
        }

        public SubjectType getSubjectType()
        {
            if (isAttribute()) return SubjectType.ATTRIBUTE;
            if (isPeriod()) return SubjectType.PERIOD;
            return SubjectType.ATTRIBUTE;
        }

        public string getName()
        {
            if (isAttribute()) return this.attributeField.name;
            if (isPeriod()) return this.periodField.name;
            return "";
        }

        public bool isIncremental() 
        {
            if (isAttribute() && this.attributeField.incremental)
            return true;
            return false;
        }
    }
}
