using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain.Browser
{
    public class PostingBrowserData : BrowserData
    {
        public long id { get; set; }

        public String postingNumber { get; set; }

        public String account { get; set; }

        public String accountName { get; set; }

        public String Scheme { get; set; }

        public String date { get; set; }

        public String dc { get; set; }

        public Decimal amount { get; set; }

        public String reconciliationNumber { get; set; }


        public PostingBrowserData() {
            this.oid = -1;
        }

        public override string ToString()
        {
            String text = String.IsNullOrWhiteSpace(account) ? "" : account;
            return text + (String.IsNullOrWhiteSpace(accountName) ? "" : " - " + accountName);
        }

    }
}
