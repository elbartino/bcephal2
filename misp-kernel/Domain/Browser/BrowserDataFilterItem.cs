using DataGridFilterLibrary.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserDataFilterItem
    {
        
        public string name { get; set; }

        public string value { get; set; }

        public string value2 { get; set; }

        public String operation { get; set; }

        public BrowserDataFilterItem() { }

        public BrowserDataFilterItem(string name, string value) : this(name, value, "=") { }

        public BrowserDataFilterItem(String name, String value, String operation)
        {
            this.name = name;
            this.value = value;
            this.operation = operation;
        }

        public BrowserDataFilterItem(String name, String value, FilterOperator operation) : this(name, value)
        {
            setOperation(operation);
        }

        public BrowserDataFilterItem(FilterData data) : this(data.ValuePropertyBindingPath, data.QueryString, data.Operator) { }

        public void setOperation(FilterOperator operation)
        {
            if (operation == null) return;
            if (operation == FilterOperator.Between) this.operation = "Between";
            else if (operation == FilterOperator.Equals) this.operation = "=";
            else if (operation == FilterOperator.GreaterThan) this.operation = ">";
            else if (operation == FilterOperator.GreaterThanOrEqual) this.operation = ">=";
            else if (operation == FilterOperator.LessThan) this.operation = "<";
            else if (operation == FilterOperator.LessThanOrEqual) this.operation = "<=";
            else if (operation == FilterOperator.Like) this.operation = "Like";
            //else if (operation == FilterOperator.Undefined) this.operation = "Undefined";
        }

    }
}
