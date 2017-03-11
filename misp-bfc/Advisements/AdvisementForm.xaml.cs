using DevExpress.Xpf.Core;
using Misp.Bfc.Model;
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

            this.EditedObject.alreadyRequestedAmount = decimal.Parse(this.AlreadyRequestedPrefundingTextEdit.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            this.EditedObject.amount = decimal.Parse(this.AmountTextEdit.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            this.EditedObject.balance = decimal.Parse(this.BalanceTextEdit.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

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

                if (!string.IsNullOrWhiteSpace(this.AlreadyRequestedPrefundingTextEdit.Text)) this.AlreadyRequestedPrefundingTextEdit.Text = this.EditedObject.alreadyRequestedAmount.HasValue ? this.EditedObject.alreadyRequestedAmount.Value.ToString() : "";
                if (!string.IsNullOrWhiteSpace(this.AmountTextEdit.Text)) this.AmountTextEdit.Text = this.EditedObject.amount.HasValue ? this.EditedObject.amount.Value.ToString() : "";
                if (!string.IsNullOrWhiteSpace(this.BalanceTextEdit.Text)) this.BalanceTextEdit.Text = this.EditedObject.balance.HasValue ? this.EditedObject.balance.Value.ToString() : "";

                if (this.EditedObject.valueDateTime.HasValue) this.ValueDatePicker.SelectedDate = this.EditedObject.valueDateTime;
                this.MessageTextBlock.Text = this.EditedObject.message != null ? this.EditedObject.message : "";
                this.StructuredMessageTextBox.Text = this.EditedObject.structuredMessage != null ? this.EditedObject.structuredMessage : "";

                if (this.EditedObject.oid.HasValue)
                {
                    this.MemberBankComboBox.IsEnabled = false;
                    this.SchemeComboBox.IsEnabled = false;
                    this.PlatformComboBox.IsEnabled = false;
                    this.PmlComboBox.IsEnabled = false;
                    this.AmountTextEdit.IsEnabled = false;                    
                    this.ValueDatePicker.IsEnabled = false;
                    this.MessageTextBlock.IsEnabled = false;
                    this.StructuredMessageTextBox.IsEnabled = false;
                    this.OkButton.Visibility = Visibility.Collapsed;
                    this.CancelButton.Visibility = Visibility.Collapsed;
                }
                this.IsModify = !this.EditedObject.oid.HasValue;
            }
            throwHandlers = true;
        }

        public bool validateEdition()
        {
            string amountText = this.AmountTextEdit.Text.Trim();
            if (string.IsNullOrWhiteSpace(amountText))
            {
                MessageDisplayer.DisplayWarning("Wrong " + AmountLabel.Content, AmountLabel.Content + " can't be empty!");
                return false;
            }
            decimal amount = decimal.Parse(amountText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
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
            if (string.IsNullOrWhiteSpace(this.MessageTextBlock.Text))
            {
                MessageDisplayer.DisplayWarning("Wrong Message", "Message can't be empty!");
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
            InitializeHandlers();
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

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null && sender is ComboBox)
            {
                BfcItem item = (BfcItem)((ComboBox)sender).SelectedItem;
                if (sender == this.MemberBankComboBox)
                {
                    this.MemberBank = item;
                    this.MemberBankTextBox.Text = item != null ? item.id : "";
                }
                else if (sender == this.SchemeComboBox)
                {
                    this.Scheme = item;
                    this.SchemeTextBox.Text = item != null ? item.id : "";
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
