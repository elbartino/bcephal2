using Misp.Bfc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Service
{
    public class AdvisementService : Misp.Kernel.Service.Service<Advisement, AdvisementBrowserData>
    {
        public BfcItemService MemberBankService { get; set; }
        public BfcItemService SchemeService { get; set; }
        public BfcItemService PlatformService { get; set; }
        public BfcItemService PmlService { get; set; }             

    }
}
