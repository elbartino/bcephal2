using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class StructuredReportColumnItem : Persistent
    {

        public PeriodInterval periodInterval { get; set; }

        public String tagValue { get; set; }

        public Target scope { get; set; }

        public Measure measure { get; set; }

        public void SetValue(object value)
        {
            if (value is Measure)
            {
                this.measure = (Measure)value;
                this.periodInterval = null;
                this.tagValue = null;
                this.scope = null;
            }
            else if (value is Target)
            {
                this.measure = null;
                this.periodInterval = null;
                this.tagValue = null;
                this.scope = (Target)value;
            }
            else if (value is String)
            {
                this.measure = null;
                this.periodInterval = null;
                this.tagValue = (String)value;
                this.scope = null;
            }
            else if (value is PeriodInterval)
            {
                this.measure = null;
                this.periodInterval = (PeriodInterval)value;
                this.tagValue = null;
                this.scope = null;
            }
        }

        public object GetValue()
        {
            if(measure != null) return measure;
            if(scope != null) return scope;
            if(tagValue != null) return tagValue;
            if (periodInterval != null) return periodInterval;
            return null;
        }

        public String GetValueString()
        {
            object value = GetValue();
            return value != null ? value.ToString() : null;
        }

        public bool EqualToValueString(String name)
        {
            String value = GetValueString();
            return value == null ? false : value.Equals(name);
        }

        public override string ToString()
        {
            String value = GetValueString();
            return value != null ? value : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is StructuredReportColumnItem)) return 1;
            return this.GetValueString().CompareTo(((StructuredReportColumnItem)obj).GetValueString());
        }

    }
}
