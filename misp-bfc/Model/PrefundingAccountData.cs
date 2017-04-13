using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Model
{
    public class PrefundingAccountData :Persistent
    {

        public decimal? sentPrefundingReconcilied { get; set; }
        public decimal? sentPrefundingNotYetReconcilied { get; set; }
        public decimal? sentReplenishmentReconcilied { get; set; }
        public decimal? sentReplenishmentNotYetReconcilied { get; set; }
        public decimal? writeoffReceived { get; set; }
        public decimal? totalToReceiveReconcilied { get; set; }
        public decimal? totalToReceiveNotYetReconcilied { get; set; }
        public decimal? sentMemberAdvisementReconcilied { get; set; }
        public decimal? sentMemberAdvisementNotYetReconcilied { get; set; }
        public decimal? writeoffPaid { get; set; }
        public decimal? totalToPayReconcilied { get; set; }
        public decimal? totalToPayNotYetReconcilied { get; set; }
        public decimal? expectedPFBalanceReconcilied { get; set; }
        public decimal? expectedPFBalanceNotYetReconcilied { get; set; }

        public decimal? pfAccountDebit { get; set; }
        public decimal? pfAccountCredit { get; set; }
        public decimal? pfAccountBalance { get; set; }

        public decimal? riAccountDebit { get; set; }
        public decimal? riAccountCredit { get; set; }
        public decimal? riAccountBalance { get; set; }

        public decimal? maAccountDebit { get; set; }
        public decimal? maAccountCredit { get; set; }
        public decimal? maAccountBalance { get; set; }

        public decimal? otherAccountDebit { get; set; }
        public decimal? otherAccountCredit { get; set; }
        public decimal? otherAccountBalance { get; set; }

        public decimal? notRecoTransactionDebit { get; set; }
        public decimal? notRecoTransactionCredit { get; set; }
        public decimal? notRecoTransactionBalance { get; set; }

        public decimal? totalAccountDebit { get; set; }
        public decimal? totalAccountCredit { get; set; }
        public decimal? totalAccountBalance { get; set; }

        public decimal? deltaReconcilied { get; set; }
        public decimal? deltaNotYetReconcilied { get; set; }
        public decimal? balancePF { get; set; }
        public decimal? peakDayLast24 { get; set; }
        public decimal? ratioPFPeak { get; set; }
        public decimal? totalBalancePFAccount { get; set; }

    }
}
