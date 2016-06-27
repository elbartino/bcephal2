using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class ProfilBrowserData : BrowserData
    {

        public bool active { get; set; }

        public ProfilBrowserData() : base() { }

        public ProfilBrowserData(BrowserData data) : base(data) { }

    }
}
