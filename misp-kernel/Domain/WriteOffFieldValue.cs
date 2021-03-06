﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class WriteOffFieldValue : Persistent
    {
        
        public int position { get; set; }


        public Attribute attribute { get; set; }


        public AttributeValue attributeValue { get; set; }


        public PeriodInterval period { get; set; }

        public decimal measure { get; set; }
	
	
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

        public override string ToString()
        {
            if (this.attributeValue != null) return this.attributeValue.name;
            return "";
        }
    }
}
