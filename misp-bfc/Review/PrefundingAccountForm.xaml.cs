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
    public partial class PrefundingAccountForm : ScrollViewer
    {
        public PrefundingAccountForm()
        {
            InitializeComponent();
        }

        public void Display(PrefundingAccountData data)
        {
            this.SentPrefundingTextBox.Text = NumberUtil.setGermanFormatter(data.sentPrefundingReconcilied);
            this.SentPrefundingNotRecoTextBox.Text =NumberUtil.setGermanFormatter(data.sentPrefundingNotYetReconcilied);

            this.SentReplenishmentTextBox.Text = NumberUtil.setGermanFormatter(data.sentReplenishmentReconcilied);
            this.SentReplenishmentNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.sentReplenishmentNotYetReconcilied);

            this.TotalToReceiveTextBox.Text = NumberUtil.setGermanFormatter(data.totalToReceiveReconcilied);
            this.TotalToReceiveNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.totalToReceiveNotYetReconcilied);

            this.MemberAdvisementTextBox.Text = NumberUtil.setGermanFormatter(data.sentMemberAdvisementReconcilied);
            this.MemberAdvisementNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.sentMemberAdvisementNotYetReconcilied);

            this.TotalToPayTextBox.Text =NumberUtil.setGermanFormatter(data.totalToPayReconcilied);
            this.TotalToPayNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.totalToPayNotYetReconcilied);

            this.ExpectedPFTextBox.Text = NumberUtil.setGermanFormatter(data.expectedPFBalanceReconcilied);
            this.ExpectedPFNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.expectedPFBalanceNotYetReconcilied);

            this.PFAccountDebitTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountDebitReconcilied);
            this.PFAccountDebitNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountDebitNotYetReconcilied);

            this.PFAccountCreditTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountCreditReconcilied);
            this.PFAccountDebitNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountCreditNotYetReconcilied);

            this.PFAccountBalanceTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountBalanceReconcilied);
            this.PFAccountBalanceNotRecoTextBox.Text = NumberUtil.setGermanFormatter(data.pfAccountCreditNotYetReconcilied);

            this.DeltaTextBox.Text = NumberUtil.setGermanFormatter(data.deltaReconcilied);
            this.DeltaNotRecoTextBox.Text =NumberUtil.setGermanFormatter(data.deltaNotYetReconcilied);
        }
    }
}
