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

        public BrowserDataFilterItem()
        {
            
        }

        public BrowserDataFilterItem(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public BrowserDataFilterItem(FilterData data)
        {
            this.name = data.ValuePropertyBindingPath;
            this.value = data.QueryString;
        }

    }
}
