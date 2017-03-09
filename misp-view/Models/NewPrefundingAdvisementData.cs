using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misp_view.Models
{
    public class NewPrefundingAdvisementData
    {
        public decimal schemeID;
        public decimal schemeName;
        public decimal alreadyRequestedPrefunding;
        public decimal newPrefundingRequest;
        public string debitCredit;
        public decimal newBalance;
        public decimal valueDate;
        public string message;
        public string structuredMessage;
    }
}
