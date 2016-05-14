using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class ReconciliationBrowserData : BrowserData
    {

        public bool template { get; set; }

        public string lastUpdate { get; set; }

        public ReconciliationBrowserData() { }

        public ReconciliationBrowserData(ReconciliationBrowserData data)
            : base(data)
        {
            this.template = data.template;
            this.visibleInShortcut = data.visibleInShortcut;
            this.lastUpdate = data.lastUpdate;
        }

    }
}
