using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class NewValuesBrowserData : BrowserData
    {
        public string nameAttribute { get; set; }

        public string nameEntity { get; set; }

        public string nameModel { get; set; }

        public string nameValue { get; set; }

    }
}
