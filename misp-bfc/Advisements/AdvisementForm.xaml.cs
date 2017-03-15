using DevExpress.Xpf.Core;
using Misp.Bfc.Model;
using Misp.Bfc.Service;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Misp.Bfc.Advisements
{
    /// <summary>
    /// Interaction logic for AdvisementForm.xaml
    /// </summary>
    public partial class AdvisementForm : Grid, IEditableView<Advisement>
    {

        #region Properties

        public ChangeEventHandler SelectionChanged { get; set; }

        public AdvisementType AdvisementType { get; set; }

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        public Advisement EditedObject { get; set; }

        public bool IsModify { get; set; }

        public BfcItem MemberBank { get; private set; }
        public BfcItem Scheme { get; private set; }
        public BfcItem Platform { get; private set; }
        public BfcItem Pml { get; private set; }

        public AdvisementService Service { get; set; }

        bool throwHandlers;

        #endregion


        #region Constructors

        public AdvisementForm(SubjectType subjectType, AdvisementType advisementType)
        {
            ThemeManager.SetThemeName(this, "Office2016White");
            this.AdvisementType = advisementType;
            this.SubjectType = subjectType;
            InitializeComponent();
            throwHandlers = true;
        }
        
        #endregion


        #region Operations
        
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.EditedObject.memberBank = (BfcItem)this.MemberBankComboBox.SelectedItem;
            this.EditedObject.scheme = (BfcItem)this.SchemeComboBox.SelectedItem;
            this.EditedObject.pml = (BfcItem)this.PmlComboBox.SelectedItem;
            this.EditedObject.platform = (BfcItem)this.PlatformComboBox.SelectedItem; 
            this.EditedObject.dc = (BfcItem)this.DCComboBox.SelectedItem;

            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            if (!string.IsNullOrWhiteSpace(this.AlreadyRequestedPrefundingTextEdit.Text)) this.EditedObject.alreadyRequestedAmount = decimal.Parse(this.AlreadyRequestedPrefundingTextEdit.Text.Trim(), format);
            if (!string.IsNullOrWhiteSpace(this.AmountTextEdit.Text)) this.EditedObject.amount = decimal.Parse(this.AmountTextEdit.Text.Trim(), format);
            if (!string.IsNullOrWhiteSpace(this.BalanceTextEdit.Text)) this.EditedObject.balance = decimal.Parse(this.BalanceTextEdit.Text.Trim(), format);

            this.EditedObject.valueDateTime = this.ValueDatePicker.SelectedDate;
            this.EditedObject.message = this.MessageTextBlock.Text;
            this.EditedObject.structuredMessage = this.StructuredMessageTextBox.Text;
            if (string.IsNullOrEmpty(this.EditedObject.creator)) this.EditedObject.creator = ApplicationManager.Instance.User.login;
        }

        public void displayObject()
        {
            throwHandlers = false;
            if (this.EditedObject != null)
            {
                this.MemberBankComboBox.SelectedItem = this.EditedObject.memberBank;
                this.SchemeComboBox.SelectedItem = this.EditedObject.scheme;
                this.PlatformComboBox.SelectedItem = this.EditedObject.platform;
                this.PmlComboBox.SelectedItem = this.EditedObject.pml;
                if (this.EditedObject.dc != null) this.DCComboBox.SelectedItem = this.EditedObject.dc;
                else this.DCComboBox.SelectedIndex = 0;

                this.AlreadyRequestedPrefundingTextEdit.Text = this.EditedObject.alreadyRequestedAmount.HasValue ? this.EditedObject.alreadyRequestedAmount.Value.ToString() : "";
                this.AmountTextEdit.Text = this.EditedObject.amount.HasValue ? this.EditedObject.amount.Value.ToString() : "";
                this.BalanceTextEdit.Text = this.EditedObject.balance.HasValue ? this.EditedObject.balance.Value.ToString() : "";

                if (this.EditedObject.valueDateTime.HasValue) this.ValueDatePicker.SelectedDate = this.EditedObject.valueDateTime;
                this.MessageTextBlock.Text = this.EditedObject.message != null ? this.EditedObject.message : "";
                this.StructuredMessageTextBox.Text = this.EditedObject.structuredMessage != null ? this.EditedObject.structuredMessage : "";
                this.CreatorTextBox.Text = this.EditedObject.creator != null ? this.EditedObject.creator : ApplicationManager.Instance.User.login;

                if (this.EditedObject.oid.HasValue)
                {
                    this.MemberBankComboBox.IsEnabled = false;
                    this.SchemeComboBox.IsEnabled = false;
                    this.PlatformComboBox.IsEnabled = false;
                    this.PmlComboBox.IsEnabled = false;
                    this.DCComboBox.IsEnabled = false;
                    this.AmountTextEdit.IsEnabled = false;                    
                    this.ValueDatePicker.IsEnabled = false;
                    this.MessageTextBlock.IsEnabled = false;
                    this.StructuredMessageTextBox.IsEnabled = false;
                    this.CreatorGrid.Visibility = Visibility.Visible;
                    this.OkButton.Visibility = Visibility.Collapsed;
                    this.CancelButton.Visibility = Visibility.Collapsed;
                }
                else this.CreatorGrid.Visibility = Visibility.Collapsed;
                this.IsModify = !this.EditedObject.oid.HasValue;
            }
            throwHandlers = true;
        }


        protected void DisplayAlreadyRequestedPrefundingAmount()
        {
            if (this.Service != null && isPrefunding() && this.EditedObject != null && !this.EditedObject.oid.HasValue
                && this.MemberBank != null && this.Scheme != null)
            {
                decimal amount = this.Service.getAlreadyRequestedPrefundingAmount(this.MemberBank.oid.Value, this.Scheme.oid.Value);
                this.AlreadyRequestedPrefundingTextEdit.Text = amount.ToString();
                DisplayBalanceAmount();
            }
        }

        protected void DisplayBalanceAmount()
        {
            if (this.Service != null && isPrefunding() && this.EditedObject != null && !this.EditedObject.oid.HasValue
                && this.MemberBank != null && this.Scheme != null)
            {
                var format = new NumberFormatInfo();
                format.NegativeSign = "-";
                decimal alreadyRequested = 0;
                decimal amount = 0;
                if (!string.IsNullOrWhiteSpace(this.AlreadyRequestedPrefundingTextEdit.Text)) alreadyRequested = decimal.Parse(this.AlreadyRequestedPrefundingTextEdit.Text.Trim(), format);
                if (!string.IsNullOrWhiteSpace(this.AmountTextEdit.Text)) amount = decimal.Parse(this.AmountTextEdit.Text.Trim(), format);
                if (DCComboBox.SelectedItem != null && DCComboBox.SelectedItem is BfcItem)
                {
                    BfcItem item = (BfcItem)DCComboBox.SelectedItem;
                    bool isDebit = item.name.ToUpper().Equals("D", StringComparison.InvariantCultureIgnoreCase)
                        || item.name.ToUpper().Equals("DEBIT", StringComparison.InvariantCultureIgnoreCase);
                    amount = isDebit ? 0 - amount : amount;
                }
                
                decimal balance = alreadyRequested + amount;                
                this.BalanceTextEdit.Text = balance.ToString();
            }
        }

        public bool validateEdition()
        {
            string amountText = this.AmountTextEdit.Text.Trim();
            if (string.IsNullOrWhiteSpace(amountText))
            {
                MessageDisplayer.DisplayWarning("Wrong " + AmountLabel.Content, AmountLabel.Content + " can't be empty!");
                return false;
            }
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            decimal amount = decimal.Parse(amountText, format);
            if (amount == 0)
            {
                MessageDisplayer.DisplayWarning("Wrong " + AmountLabel.Content, AmountLabel.Content + " can't be 0!");
                return false;
            }
            if (!ValueDatePicker.SelectedDate.HasValue)
            {
                MessageDisplayer.DisplayWarning("Wrong Value Date", "Value Date can't be empty!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.StructuredMessageTextBox.Text))
            {
                MessageDisplayer.DisplayWarning("Wrong Structured Message", "Structured Message can't be empty!");
                return false;
            }
            return true;
        }

        public Advisement getNewObject()
        {
            Advisement advisement = new Advisement();
            advisement.advisementType = this.AdvisementType.ToString();
            return advisement;
        }

        public List<object> getEditableControls()
        {
            return new List<object>();
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler) { }

        public void SetReadOnly(bool readOnly) { }

        public void Customize(List<Kernel.Domain.Right> rights, bool readOnly) { }

        #endregion


        #region Initialization

        public void CustomizeForType()
        {
            if (isPrefunding())
            {
                this.AmountLabel.Content = "New Pre-funding Requested";
                this.PmlGrid.Visibility = Visibility.Collapsed;
                this.PlatformGrid.Visibility = Visibility.Collapsed;
            }
            else if (isExceptional())
            {
                this.AmountLabel.Content = "Replenishment Amount";
                this.PlatformGrid.Visibility = Visibility.Collapsed;
                this.AlreadyRequestedPrefundingGrid.Visibility = Visibility.Collapsed;
                this.BalanceGrid.Visibility = Visibility.Collapsed;
            }
            else if (isMember())
            {
                this.AmountLabel.Content = "Member Advisement Amount";
                this.AlreadyRequestedPrefundingGrid.Visibility = Visibility.Collapsed;
                this.BalanceGrid.Visibility = Visibility.Collapsed;
            }
            else if (isSettlement())
            {
                this.AmountLabel.Content = "Settlement Advisement Amount";
                this.MemberBankGrid.Visibility = Visibility.Collapsed;
                this.AlreadyRequestedPrefundingGrid.Visibility = Visibility.Collapsed;
                this.BalanceGrid.Visibility = Visibility.Collapsed;
            }
            InitializeData();
            InitializeHandlers();
        }

        protected void InitializeData()
        {
            if (this.Service != null)
            {
                List<BfcItem> dcs = Service.DebitCreditService.getAll();
                this.DCComboBox.ItemsSource = dcs;

                List<BfcItem> schemes = Service.SchemeService.getAll();
                this.SchemeComboBox.ItemsSource = schemes;

                if (!isSettlement())
                {
                    List<BfcItem> banks = Service.MemberBankService.getAll();
                    this.MemberBankComboBox.ItemsSource = banks;
                }
                if (isSettlement() || isMember())
                {
                    List<BfcItem> platforms = Service.PlatformService.getAll();
                    this.PlatformComboBox.ItemsSource = platforms;
                }
                if (!isPrefunding())
                {
                    List<BfcItem> pmls = Service.PmlService.getAll();
                    this.PmlComboBox.ItemsSource = pmls;
                }
            }
        }

        protected bool isPrefunding()
        {
            return this.AdvisementType == AdvisementType.PREFUNDING;
        }

        protected bool isMember()
        {
            return this.AdvisementType == AdvisementType.MEMBER;
        }

        protected bool isExceptional()
        {
            return this.AdvisementType == AdvisementType.EXCEPTIONAL;
        }

        protected bool isSettlement()
        {
            return this.AdvisementType == AdvisementType.SETTLEMENT;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            if (!isSettlement())
            {
                this.MemberBankComboBox.SelectionChanged += OnComboBoxSelectionChanged;
                this.AmountTextEdit.EditValueChanged += OnAmountChanged;
                this.DCComboBox.SelectionChanged += OnDCComboBoxSelectionChanged;
            }

            this.SchemeComboBox.SelectionChanged += OnComboBoxSelectionChanged;

            if (isSettlement() || isMember())
            {
                this.PlatformComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            }
            if (!isPrefunding())
            {
                this.PmlComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            }
            
        }

        private void OnDCComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (throwHandlers) DisplayBalanceAmount();
        }

        private void OnAmountChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (throwHandlers) DisplayBalanceAmount();
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null && sender is ComboBox)
            {
                BfcItem item = (BfcItem)((ComboBox)sender).SelectedItem;
                if (sender == this.MemberBankComboBox)
                {
                    this.MemberBank = item;
                    this.MemberBankTextBox.Text = item != null ? item.id : "";
                    DisplayAlreadyRequestedPrefundingAmount();
                }
                else if (sender == this.SchemeComboBox)
                {
                    this.Scheme = item;
                    this.SchemeTextBox.Text = item != null ? item.id : "";
                    DisplayAlreadyRequestedPrefundingAmount();
                }
                else if (sender == this.PlatformComboBox)
                {
                    this.Platform = item;
                    this.PlatformTextBox.Text = item != null ? item.id : "";
                }
                else if (sender == this.PmlComboBox)
                {
                    this.Pml = item;
                    this.PmlTextBox.Text = item != null ? item.id : "";
                }
                OnChange();
                if (throwHandlers && SelectionChanged != null) SelectionChanged();                
            } 
        }
        
        private void OnChange()
        {
            if (throwHandlers) this.IsModify = !this.EditedObject.oid.HasValue;
        }

        #endregion

    }
}
