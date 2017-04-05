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
            this.SentPrefundingTextBox.Text = NumberUtil.ToGermanFormat(data.sentPrefundingReconcilied);
            this.SentPrefundingNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentPrefundingNotYetReconcilied);

            this.SentReplenishmentTextBox.Text = NumberUtil.ToGermanFormat(data.sentReplenishmentReconcilied);
            this.SentReplenishmentNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentReplenishmentNotYetReconcilied);

            this.WriteOffReceiveTextBox.Text = NumberUtil.ToGermanFormat(data.writeoffReceived);

            this.TotalToReceiveTextBox.Text = NumberUtil.ToGermanFormat(data.totalToReceiveReconcilied);
            this.TotalToReceiveNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.totalToReceiveNotYetReconcilied);

            this.MemberAdvisementTextBox.Text = NumberUtil.ToGermanFormat(data.sentMemberAdvisementReconcilied);
            this.MemberAdvisementNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.sentMemberAdvisementNotYetReconcilied);

            this.WriteOffPaidTextBox.Text = NumberUtil.ToGermanFormat(data.writeoffPaid);

            this.TotalToPayTextBox.Text = NumberUtil.ToGermanFormat(data.totalToPayReconcilied);
            this.TotalToPayNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.totalToPayNotYetReconcilied);

            this.ExpectedPFTextBox.Text = NumberUtil.ToGermanFormat(data.expectedPFBalanceReconcilied);
            this.ExpectedPFNotRecoTextBox.Text = NumberUtil.ToGermanFormat(data.expectedPFBalanceNotYetReconcilied);


            this.PFCreditTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountCredit);
            this.PFDebitTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountDebit);
            this.PFBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.pfAccountBalance);

            this.RICreditTextBox.Text = NumberUtil.ToGermanFormat(data.riAccountCredit);
            this.RIDebitTextBox.Text = NumberUtil.ToGermanFormat(data.riAccountDebit);
            this.RIBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.riAccountBalance);

            this.MACreditTextBox.Text = NumberUtil.ToGermanFormat(data.maAccountCredit);
            this.MADebitTextBox.Text = NumberUtil.ToGermanFormat(data.maAccountDebit);
            this.MABalanceTextBox.Text = NumberUtil.ToGermanFormat(data.maAccountBalance);

            this.OtherCreditTextBox.Text = NumberUtil.ToGermanFormat(data.otherAccountCredit);
            this.OtherDebitTextBox.Text = NumberUtil.ToGermanFormat(data.otherAccountDebit);
            this.OtherBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.otherAccountBalance);

            this.NonRecoTransactionsCreditTextBox.Text = NumberUtil.ToGermanFormat(data.notRecoTransactionCredit);
            this.NonRecoTransactionsDebitTextBox.Text = NumberUtil.ToGermanFormat(data.notRecoTransactionDebit);
            this.NonRecoTransactionsBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.notRecoTransactionBalance);                       

            this.TotalCreditTextBox.Text = NumberUtil.ToGermanFormat(data.totalAccountCredit);
            this.TotalDebitTextBox.Text = NumberUtil.ToGermanFormat(data.totalAccountDebit);
            this.TotalBalanceTextBox.Text = NumberUtil.ToGermanFormat(data.totalAccountBalance);


            this.DeltaNotReconciliatedTextBox.Text = NumberUtil.ToGermanFormat(data.deltaNotYetReconcilied);
            this.DeltaReconciliatedTextBox.Text = NumberUtil.ToGermanFormat(data.deltaReconcilied);

            
            this.RatioPFPeakDayTextBox.Text = NumberUtil.ToGermanFormat(data.ratioPFPeak);

            this.RatioPFPeakMa24MonthsTextBox.Text = NumberUtil.ToGermanFormat(data.ratioPFPeak);
            this.TotalBalancePFAccountTextBox.Text = NumberUtil.ToGermanFormat(data.totalBalancePFAccount);
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
