using Misp.Bfc.Model;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for PrefundingAccountForm.xaml
    /// </summary>
    public partial class PrefundingAccountForm : Grid
    {
        public PrefundingAccountForm()
        {
            InitializeComponent();
        }

        public void Display(PrefundingAccountData data)
        {
            this.SentPrefundingTextBox.Text = NumberUtil.ToGermanFormat(data.sentPrefundingReconcilied);
            this.SentPrefundingNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentPrefundingNotYetReconcilied);

            this.SentReplenishmentTextBox.Text = NumberUtil.ToGermanFormat(data.sentReplenishmentReconcilied);
            this.SentReplenishmentNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentReplenishmentNotYetReconcilied);

            this.OtherReceiveTextBox.Text = NumberUtil.ToGermanFormat(data.otherReceivedReconcilied);
            this.OtherReceiveNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.otherReceivedNotYetReconcilied);

            this.TotalToReceiveTextBox.Text = NumberUtil.ToGermanFormat(data.totalToReceiveReconcilied);
            this.TotalToReceiveNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.totalToReceiveNotYetReconcilied);

            this.MemberAdvisementTextBox.Text = NumberUtil.ToGermanFormat(data.sentMemberAdvisementReconcilied);
            this.MemberAdvisementNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentMemberAdvisementNotYetReconcilied);

            this.OtherPaidTextBox.Text = NumberUtil.ToGermanFormat(data.otherPaidReconcilied);
            this.OtherPaidNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.otherPaidNotYetReconcilied);

            this.TotalToPayTextBox.Text = NumberUtil.ToGermanFormat(data.totalToPayReconcilied);
            this.TotalToPayNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.totalToPayNotYetReconcilied);

            this.ExpectedPFTextBox.Text = NumberUtil.ToGermanFormat(data.expectedPFBalanceReconcilied);
            this.ExpectedPFNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.expectedPFBalanceNotYetReconcilied);

            //this.PFAccountDebitTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountDebitReconcilied);
            //this.PFAccountDebitNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountDebitNotYetReconcilied);

            //this.PFAccountCreditTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountCreditReconcilied);
            //this.PFAccountDebitNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountCreditNotYetReconcilied);

            //this.PFAccountBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountBalanceReconcilied);
            //this.PFAccountBalanceNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountCreditNotYetReconcilied);

            //this.DeltaTextBox.Text = NumberUtil.ToGermanFormat(data.deltaReconcilied);
            //this.DeltaNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.deltaNotYetReconcilied);

            this.RatioPFPeakDayTextBox.Text = NumberUtil.ToGermanFormat(data.ratioPFPeak);
        }
    }
}
