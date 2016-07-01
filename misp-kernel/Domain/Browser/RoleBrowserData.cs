using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class RoleBrowserData : BrowserData
    {

        public String name { get; set; }

        public String delete { get; set; }

        public RoleBrowserData() : base() { }

        public RoleBrowserData(BrowserData data) : base(data) { }

    }
}
