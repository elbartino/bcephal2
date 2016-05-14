using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class LineItem : Persistent
    {
        
        public int position { get; set; }
        
        public Target scope { get; set; }

        public Measure measure { get; set; }

        public PeriodInterval periodInterval { get; set; }

        public PeriodName periodName { get; set; }
        
        public String periodFrom { get; set; }

        public String periodTo { get; set; }

        public LineItem() { }

        public LineItem(int position, object value)
            : this()
        {
            this.position = position;
            this.SetValue(value);
        }

        public bool IsMeasure()
        {
            object value = GetValue();
            return value != null && value is Measure;
        }
        
        public bool IsPeriod() 
        {
            object value = GetValue();
            return value != null && value is PeriodInterval;
        }

        public void SetValue(object value)
        {
            this.scope = null;
            this.measure = null;
            this.periodFrom = null;
            this.periodTo = null;
            this.periodInterval = null;
            this.periodName = null;
            if (value != null && value is Target) this.scope = (Target)value;
            else if (value != null && value is Measure) this.measure = (Measure)value;
            else if (value != null && value is PeriodInterval)
            {
                PeriodInterval interval = (PeriodInterval)value;
                this.periodName = interval.periodName;
                this.periodInterval = new PeriodInterval(0, interval.name, interval.periodFromDateTime, interval.periodToDateTime);
                this.periodInterval.periodName = this.periodName;
                this.periodName = this.periodInterval.periodName;
                this.periodFrom = this.periodInterval.fromAsString;
                this.periodTo = this.periodInterval.toAsString;
            }
        }

        public object GetValue()
        {
            if (this.scope != null) return this.scope;
            if (this.measure != null) return this.measure;
            if (this.periodInterval != null) return this.periodInterval;
            if (this.periodFrom != null || this.periodTo != null)
            {
                this.periodInterval = new PeriodInterval();
                this.periodInterval.periodName = this.periodName;
                this.periodInterval.name = this.periodName.name;
                this.periodInterval.periodFrom = this.periodFrom;
                this.periodInterval.periodTo = this.periodTo;
                return this.periodInterval;
            }
            return "";
        }
        

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is LineItem)) return 1;
            return this.position.CompareTo(((LineItem)obj).position);
        }

    }
}
