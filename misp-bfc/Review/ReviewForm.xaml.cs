using DevExpress.Xpf.Core;
using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
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
    /// Interaction logic for ReviewForm.xaml
    /// </summary>
    public partial class ReviewForm : UserControl
    {

        #region Properties

        public ChangeEventHandler MemberBankChanged { get; set; }

        public List<BfcItem> MemberBanks { get; private set; }
        bool throwHandlers;

        public bool IsBussy { 
            set {this.LoadingDecorator.IsSplashScreenShown = value; }
            get { return this.LoadingDecorator.IsSplashScreenShown.Value; }
        }

        #endregion


        #region Constructors

        public ReviewForm()
        {
            this.MemberBanks = new List<BfcItem>(0);
            ThemeManager.SetThemeName(this, "Office2016White");
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(PrefundingAccountData data)
        {
            throwHandlers = false;
            this.PrefundingAccountForm.Display(data);
            throwHandlers = true;
        }

        public void Display(List<SettlementEvolutionData> datas)
        {
            throwHandlers = false;
            this.SettlementEvolutionForm.Display(datas);
            throwHandlers = true;
        }

        public void DisplayTotal(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.AgeingBalanceForm.DisplayTotal(datas);
            throwHandlers = true;
        }

        public void DisplayDetails(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.AgeingBalanceForm.DisplayDetails(datas);
            throwHandlers = true;
        }

        public ReviewFilter GetFilter()
        {
            ReviewFilter filter = new ReviewFilter();
            foreach (BfcItem bank in this.MemberBanks)
            {
                filter.memberBankIdOids.Add(bank.oid.Value);
            }
            if (this.TabControl.SelectedIndex == 0) { }
            else if (this.TabControl.SelectedIndex == 1)
            {
                this.SettlementEvolutionForm.FillFilter(filter);
            }
            else if (this.TabControl.SelectedIndex == 2)
            {
                this.AgeingBalanceForm.FillFilter(filter);
            }
            return filter;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.MemberBankComboBoxEdit.PopupClosed += OnMemberBankPopupClosed;
        }

        private void OnMemberBankPopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Normal)
            {
                this.MemberBanks = new List<BfcItem>(0);
                MemberBankTextBox.Text = "";
                ObservableCollection<object> SelectedItems = this.MemberBankComboBoxEdit.SelectedItems;
                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    String coma = "";
                    foreach (object obj in SelectedItems)
                    {
                        if (obj is BfcItem)
                        {
                            BfcItem item = (BfcItem)obj;
                            this.MemberBanks.Add(item);
                            MemberBankTextBox.Text += coma + item.id;
                            coma = ";";
                        }
                    }
                }
                if (throwHandlers && MemberBankChanged != null) MemberBankChanged();
            }
        }
                
        #endregion

        
    }
}
