using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class InputTableBrowserData : BrowserData
    {

        public bool active { get; set; }

        public bool template { get; set; }

        public bool isReport { get; set; }

        public string lastUpdate { get; set; }

        public bool isGrid { get; set; }

        [JsonIgnore]
        public String type
        {
            get
            {
                return this.isGrid ? "Grid" : "InputTable";
            }
        } 

        public InputTableBrowserData() { }

        public InputTableBrowserData(InputTableBrowserData data)
            : base(data)
        {
            this.active = data.active;
            this.template = data.template;
            this.visibleInShortcut = data.visibleInShortcut;
            this.isReport = data.isReport;
            this.lastUpdate = data.lastUpdate;
        }

        public InputTableBrowserData(InputTableBrowserData data,bool isgrid)
            : this(data)
        {
            this.isGrid = isgrid;
        }
    }
}
