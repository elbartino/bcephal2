using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class TransformationTreeLoopValue : Persistent
    {
        
        public int position { get; set; }
        
        public Target scope { get; set; }

        public Measure measure { get; set; }

        public PeriodInterval periodInterval { get; set; }

        public PeriodName periodName { get; set; }
        
        [ScriptIgnore]
        public TransformationTreeItem loop { get; set; }

        public TransformationTreeLoopValue() { }

        public TransformationTreeLoopValue(int position, object value)
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
            return value != null && (value is PeriodName || value is Period);
        }

        public bool IsAnAttribute() 
        {
            object value = GetValue();
            return (this.scope != null && (value is Kernel.Domain.Attribute || ((Persistent)value).typeName.Equals(typeof(Kernel.Domain.Attribute).Name)));
        }

        public bool IsPeriodName()
        {
            object value = GetValue();
            return value != null && (value is PeriodName);
        }

        public void SetValue(object value)
        {
            this.scope = null;
            this.measure = null;
            this.periodInterval = null;
            this.periodName = null;
            if (value != null && value is Target) this.scope = (Target)value;
            else if (value != null && value is Measure) this.measure = (Measure)value;
            else if (value != null && value is PeriodInterval) this.periodInterval = (PeriodInterval)value;
            else if (value != null && value is PeriodName) this.periodName = (PeriodName)value;

        }

        public object GetValue()
        {
            if (this.scope != null) return this.scope;
            if (this.measure != null) return this.measure;
            if (this.periodInterval != null) return this.periodInterval;
            if (this.periodName != null) return this.periodName;
            return "NOT FOUND";
        }
                
        
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is TransformationTreeLoopValue)) return 1;
            return this.position.CompareTo(((TransformationTreeLoopValue)obj).position);
        }

        public TransformationTreeLoopValue GetCopy()
        {
            TransformationTreeLoopValue value = new TransformationTreeLoopValue();
            value.scope = this.scope;
            value.position = this.position;
            value.measure = this.measure;
            value.periodInterval = this.periodInterval;
            value.periodName = this.periodName;
            value.loop = this.loop;
            return value;
        }
        
    }
}
