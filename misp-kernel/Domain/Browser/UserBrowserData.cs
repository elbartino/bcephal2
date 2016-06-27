using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class UserBrowserData : BrowserData
    {

        public String firstName { get; set; }

        public String profil { get; set; }

        public bool active { get; set; }

        public UserBrowserData() : base() { }

        public UserBrowserData(BrowserData data) : base(data) { }

    }
}
