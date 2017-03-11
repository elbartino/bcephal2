using DevExpress.Xpf.Core;
using Misp.Bfc.Model;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Bfc.Advisements
{
    /// <summary>
    /// Interaction logic for AdvisementForm.xaml
    /// </summary>
    public partial class AdvisementForm : Grid, IEditableView<Advisement>
    {

        #region Properties

        public AdvisementType AdvisementType { get; set; }

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        public Advisement EditedObject { get; set; }

        public bool IsModify { get; set; }

        #endregion


        #region Constructors

        public AdvisementForm(SubjectType subjectType, AdvisementType advisementType)
        {
            ThemeManager.SetThemeName(this, "Office2016White");
            this.AdvisementType = advisementType;
            this.SubjectType = subjectType;
            InitializeComponent();
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

            //this.EditedObject.alreadyRequestedAmount = decimal. this.AlreadyRequestedPrefundingTextBox.Text.Trim();
            //this.EditedObject.amount;
            //this.EditedObject.balance;
            this.EditedObject.valueDateTime = this.ValueDatePicker.SelectedDate;
            this.EditedObject.message = this.MessageTextBlock.Text;
            this.EditedObject.structuredMessage = this.StructuredMessageTextBox.Text;
            if (string.IsNullOrEmpty(this.EditedObject.creator)) this.EditedObject.creator = ApplicationManager.Instance.User.login;
        }

        public void displayObject()
        {

        }

        public bool validateEdition()
        {
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



        #region Operations

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

    }
}
