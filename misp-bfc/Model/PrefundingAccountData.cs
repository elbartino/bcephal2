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
        public decimal? otherPaidReconcilied { get; set; }
        public decimal? otherPaidNotYetReconcilied { get; set; }
        public decimal? totalToPayReconcilied { get; set; }
        public decimal? totalToPayNotYetReconcilied { get; set; }
        public decimal? expectedPFBalanceReconcilied { get; set; }
        public decimal? expectedPFBalanceNotYetReconcilied { get; set; }
        public decimal? pfAccountDebitReconcilied { get; set; }
        public decimal? pfAccountDebitNotYetReconcilied { get; set; }
        public decimal? pfAccountCreditReconcilied { get; set; }
        public decimal? pfAccountCreditNotYetReconcilied { get; set; }
        public decimal? pfAccountBalanceReconcilied { get; set; }
        public decimal? pfAccountBalanceNotYetReconcilied { get; set; }
        public decimal? deltaReconcilied { get; set; }
        public decimal? deltaNotYetReconcilied { get; set; }
        public decimal? balancePF { get; set; }
        public decimal? peakDayLast24 { get; set; }
        public decimal? ratioPFPeak { get; set; }

    }
}
