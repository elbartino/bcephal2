using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misp_view.Models
{
    public class PrefundingAccountData
    {
        public decimal sentPrefundingReconcilied;
        public decimal sentPrefundingNotYetReconcilied;
        public decimal sentReplenishmentReconcilied;
        public decimal sentReplenishmentNotYetReconcilied;
        public decimal totalToReceiveReconcilied;
        public decimal totalToReceiveNotYetReconcilied;
        public decimal sentMemberAdvisementReconcilied;
        public decimal sentMemberAdvisementNotYetReconcilied;
        public decimal totalToPayReconcilied;
        public decimal totalToPayNotYetReconcilied;
        public decimal expectedPFBalanceReconcilied;
        public decimal expectedPFBalanceNotYetReconcilied;
        public decimal pfAccountDebitReconcilied;
        public decimal pfAccountDebitNotYetReconcilied;
        public decimal pfAccountCreditReconcilied;
        public decimal pfAccountCreditNotYetReconcilied;
        public decimal pfAccountBalanceReconcilied;
        public decimal pfAccountBalanceNotYetReconcilied;
        public decimal deltaReconcilied;
        public decimal deltaNotYetReconcilied;
        public decimal balancePF;
        public decimal peakDayLast24;
        public decimal ratioPFPeak; 


        
    }
}
