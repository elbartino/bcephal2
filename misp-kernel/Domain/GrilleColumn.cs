using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class GrilleColumn : Persistent
    {

        public String type { get; set; }

        public String name { get; set; }

        public int position { get; set; }

        public bool show { get; set; }

        public int? valueOid { get; set; }

        [ScriptIgnore]
        public bool isAdded { get; set; }

        [ScriptIgnore]
        public List<BrowserData> values { get; set; }

        
        public GrilleColumn()
        {
            show = true;
            values = new List<BrowserData>(0);
        }

        public void SetValue(object value)
        {
            if (value is Measure)
            {
                this.type = ParameterType.MEASURE.ToString();
                this.valueOid = ((Measure)value).oid.Value;
                this.name = ((Measure)value).name;
            }            
            else if (value is Target)
            {
                this.type = ParameterType.SCOPE.ToString();
                this.valueOid = ((Target)value).oid.Value;
                this.name = ((Target)value).name;
            }
            else if (value is PeriodName)
            {
                this.type = ParameterType.PERIOD.ToString();
                this.valueOid = ((PeriodName)value).oid.Value;
                this.name = ((PeriodName)value).name;
            }
            else if (value is PeriodInterval)
            {
                this.type = ParameterType.PERIOD.ToString();
                this.valueOid = ((PeriodInterval)value).periodName.oid.Value;
                this.name = ((PeriodInterval)value).periodName.name;
            }
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is GrilleColumn)) return 1;
            if (this.name != null && this.name.Equals(((GrilleColumn)obj).name)) return 0;
            return this.position.CompareTo(((GrilleColumn)obj).position);
        }


        public List<string> getValueNames()
        {
            List<string> names = new List<string>(0);
            foreach (BrowserData value in values) names.Add(value.name);
            names.Insert(0, "");
            return names;
        }

        public BrowserData getValue(String name)
        {
            if(string.IsNullOrWhiteSpace(name)) return null;
            foreach (BrowserData value in values) if (value.name.Equals(name)) return value;
            return null;
        }

    }
}
