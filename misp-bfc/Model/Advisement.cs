using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Model
{
    public class Advisement : Persistent
    {
        public BrowserData SentPrefunding { get; set; }
        public BrowserData SentReplenistment { get; set; }
        public BrowserData SentMemberAdvisement { get; set; }
        public decimal Amount { get; set; }
        public BrowserData PFAmoutDebit { get; set; }
        public BrowserData PFAmountCredit { get; set; }
        public BrowserData Scheme { get; set; }
        public BrowserData MemberBank { get; set; }
        public string Message { get; set; }
        public string StructuredMessage { get; set; }
        public DateTime ValueDate { get; set; }

        public decimal AlreadyRequestedPrefunding { get; set; }
        public BrowserData Pml { get; set; }
        public BrowserData PlatForm { get; set; }

    }
}
