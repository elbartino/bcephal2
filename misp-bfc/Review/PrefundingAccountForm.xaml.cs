using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #region Properties

        public ChangeEventHandler FilterChanged { get; set; }

        public List<BfcItem> Schemes { get; private set; }

        public List<BfcItem> Pmls { get; private set; }

        public bool IsAlreadyLoaded { get; private set; }

        bool throwHandlers;

        #endregion


        #region Constructors

        public PrefundingAccountForm()
        {
            this.IsAlreadyLoaded = false;
            InitializeComponent();
            this.Schemes = new List<BfcItem>(0);
            this.Pmls = new List<BfcItem>(0);
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(PrefundingAccountData data)
        {
            throwHandlers = false;
            this.SentPrefundingTextBox.Text = NumberUtil.ToStandardFormat(data.sentPrefundingReconcilied);
            this.SentPrefundingNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.sentPrefundingNotYetReconcilied);

            this.SentReplenishmentTextBox.Text = NumberUtil.ToStandardFormat(data.sentReplenishmentReconcilied);
            this.SentReplenishmentNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.sentReplenishmentNotYetReconcilied);

            this.WriteOffReceiveTextBox.Text = NumberUtil.ToStandardFormat(data.writeoffReceived);

            this.TotalToReceiveTextBox.Text = NumberUtil.ToStandardFormat(data.totalToReceiveReconcilied);
            this.TotalToReceiveNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.totalToReceiveNotYetReconcilied);

            this.MemberAdvisementTextBox.Text = NumberUtil.ToStandardFormat(data.sentMemberAdvisementReconcilied);
            this.MemberAdvisementNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.sentMemberAdvisementNotYetReconcilied);

            this.WriteOffPaidTextBox.Text = NumberUtil.ToStandardFormat(data.writeoffPaid);

            this.TotalToPayTextBox.Text = NumberUtil.ToStandardFormat(data.totalToPayReconcilied);
            this.TotalToPayNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.totalToPayNotYetReconcilied);

            this.ExpectedPFTextBox.Text = NumberUtil.ToStandardFormat(data.expectedPFBalanceReconcilied);
            this.ExpectedPFNotRecoTextBox.Text = NumberUtil.ToStandardFormat(data.expectedPFBalanceNotYetReconcilied);


            this.PFCreditTextBox.Text = NumberUtil.ToStandardFormat(data.pfAccountCredit);
            this.PFDebitTextBox.Text = NumberUtil.ToStandardFormat(data.pfAccountDebit);
            this.PFBalanceTextBox.Text = NumberUtil.ToStandardFormat(data.pfAccountBalance);

            this.RICreditTextBox.Text = NumberUtil.ToStandardFormat(data.riAccountCredit);
            this.RIDebitTextBox.Text = NumberUtil.ToStandardFormat(data.riAccountDebit);
            this.RIBalanceTextBox.Text = NumberUtil.ToStandardFormat(data.riAccountBalance);

            this.MACreditTextBox.Text = NumberUtil.ToStandardFormat(data.maAccountCredit);
            this.MADebitTextBox.Text = NumberUtil.ToStandardFormat(data.maAccountDebit);
            this.MABalanceTextBox.Text = NumberUtil.ToStandardFormat(data.maAccountBalance);

            this.OtherCreditTextBox.Text = NumberUtil.ToStandardFormat(data.otherAccountCredit);
            this.OtherDebitTextBox.Text = NumberUtil.ToStandardFormat(data.otherAccountDebit);
            this.OtherBalanceTextBox.Text = NumberUtil.ToStandardFormat(data.otherAccountBalance);

            this.NonRecoTransactionsCreditTextBox.Text = NumberUtil.ToStandardFormat(data.notRecoTransactionCredit);
            this.NonRecoTransactionsDebitTextBox.Text = NumberUtil.ToStandardFormat(data.notRecoTransactionDebit);
            this.NonRecoTransactionsBalanceTextBox.Text = NumberUtil.ToStandardFormat(data.notRecoTransactionBalance);

            this.TotalCreditTextBox.Text = NumberUtil.ToStandardFormat(data.totalAccountCredit);
            this.TotalDebitTextBox.Text = NumberUtil.ToStandardFormat(data.totalAccountDebit);
            this.TotalBalanceTextBox.Text = NumberUtil.ToStandardFormat(data.totalAccountBalance);


            this.DeltaNotReconciliatedTextBox.Text = NumberUtil.ToStandardFormat(data.deltaNotYetReconcilied);
            this.DeltaReconciliatedTextBox.Text = NumberUtil.ToStandardFormat(data.deltaReconcilied);


            this.RatioPFPeakDayTextBox.Text = NumberUtil.ToStandardFormat(data.ratioPFPeak);

            this.RatioPFPeakMa24MonthsTextBox.Text = NumberUtil.ToStandardFormat(data.ratioPFPeak);
            this.TotalBalancePFAccountTextBox.Text = NumberUtil.ToStandardFormat(data.totalBalancePFAccount);
            throwHandlers = true;
            this.IsAlreadyLoaded = true;
        }

        public void FillFilter(ReviewFilter filter)
        {
            if (filter == null) filter = new ReviewFilter();
            foreach (BfcItem scheme in this.Schemes)
            {
                filter.schemeIdOids.Add(scheme.oid.Value);
            }
            foreach (BfcItem pml in this.Pmls)
            {
                filter.pmlIdOids.Add(pml.oid.Value);
            }
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.SchemeComboBoxEdit.PopupClosed += OnSchemePopupClosed;
            this.PmlComboBoxEdit.PopupClosed += OnPmlPopupClosed;
        }

        private void OnSchemePopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Normal)
            {
                this.Schemes = new List<BfcItem>(0);
                SchemeTextBox.Text = "";
                ObservableCollection<object> SelectedItems = this.SchemeComboBoxEdit.SelectedItems;
                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    String coma = "";
                    foreach (object obj in SelectedItems)
                    {
                        if (obj is BfcItem)
                        {
                            BfcItem item = (BfcItem)obj;
                            this.Schemes.Add(item);
                            SchemeTextBox.Text += coma + item.id;
                            coma = ";";
                        }
                    }
                }
                if (throwHandlers && FilterChanged != null) FilterChanged();
            }
        }

        private void OnPmlPopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Normal)
            {
                this.Pmls = new List<BfcItem>(0);
                PmlTextBox.Text = "";
                ObservableCollection<object> SelectedItems = this.PmlComboBoxEdit.SelectedItems;
                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    String coma = "";
                    foreach (object obj in SelectedItems)
                    {
                        if (obj is BfcItem)
                        {
                            BfcItem item = (BfcItem)obj;
                            this.Pmls.Add(item);
                            PmlTextBox.Text += coma + item.id;
                            coma = ";";
                        }
                    }
                }
                if (throwHandlers && FilterChanged != null) FilterChanged();
            }
        }
        
        #endregion

    }
}
