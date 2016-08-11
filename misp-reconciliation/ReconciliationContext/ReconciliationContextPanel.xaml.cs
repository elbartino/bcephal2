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

namespace Misp.Reconciliation.ReconciliationContext
{
    /// <summary>
    /// Interaction logic for ReconciliationContextPanel.xaml
    /// </summary>
    public partial class ReconciliationContextPanel : Grid
    {
        public event Misp.Kernel.Ui.Base.ActivateEventHandler ActivatedItem;

        public event Misp.Kernel.Ui.Base.ChangeEventHandler Change;

        public ReconciliationContextItem ActiveItem { get; set; }

        public Kernel.Service.ModelService ModelService { get; set; }

        public Kernel.Domain.ReconciliationContext reconciliationContext { get; set; }

        public ReconciliationContextPanel()
        {
            InitializeComponent();
            this.postingAttributePanel.setContextItemContent("Posting Nbr Attribute");
            this.accountNbrAttributePanel.setContextItemContent("Account Nbr Attribute");
            this.accountNameAttributePanel.setContextItemContent("Account Name Attribute");
            this.reconciliationAttributePanel.setContextItemContent("Reco Nbr Attribute");
            this.dcAttributePanel.setContextItemContent("Debit/Credit Attribute");
            //this.creditValuePanel.setContextItemContent("Credit Value");
            //this.debitValuePanel.setContextItemContent("Debit Value");
            this.writeOffAccountPanel.setContextItemContent("Write Off Account");
            this.amountMeasurePanel.setContextItemContent("Amount Measure");
            InitializeHandlers();
        }

        public void display(Kernel.Domain.ReconciliationContext reconciliationcontext) 
        {
            if (reconciliationcontext == null) return;
            this.reconciliationContext = reconciliationcontext;
            this.postingAttributePanel.setValue(reconciliationContext.postingNbreAttribute);
            this.accountNbrAttributePanel.setValue(reconciliationContext.accountNbreAttribute);
            this.accountNameAttributePanel.setValue(reconciliationContext.accountNameAttribute);
            this.reconciliationAttributePanel.setValue(reconciliationContext.recoNbreAttribute);
            this.dcAttributePanel.setValue(reconciliationContext.dcNbreAttribute);
            //this.creditValuePanel.setValue(reconciliationContext.creditAttributeValue);
            //this.debitValuePanel.setValue(reconciliationContext.debitAttributeValue);
            this.writeOffAccountPanel.setValue(reconciliationContext.writeOffAccount);
            this.amountMeasurePanel.setValue(reconciliationContext.amountMeasure);
        }
        
        public void InitializeHandlers()
        {
            this.postingAttributePanel.ActivatedItem += OnActivateitem;
            this.dcAttributePanel.ActivatedItem += OnActivateitem;
            //this.creditValuePanel.ActivatedItem += OnActivateitem;
            //this.debitValuePanel.ActivatedItem += OnActivateitem;
            this.writeOffAccountPanel.ActivatedItem += OnActivateitem;
            this.reconciliationAttributePanel.ActivatedItem += OnActivateitem;
            this.accountNbrAttributePanel.ActivatedItem += OnActivateitem;
            this.accountNameAttributePanel.ActivatedItem += OnActivateitem;
            this.amountMeasurePanel.ActivatedItem += OnActivateitem;

            this.postingAttributePanel.Changed += OnChange;
            this.dcAttributePanel.Changed += OnChange;
            //this.creditValuePanel.Changed += OnChange;
            //this.debitValuePanel.Changed += OnChange;
            this.writeOffAccountPanel.Changed += OnChange;
            this.reconciliationAttributePanel.Changed += OnChange;
            this.accountNbrAttributePanel.Changed += OnChange;
            this.accountNameAttributePanel.Changed += OnChange;
            this.amountMeasurePanel.Changed += OnChange;
        }

        private void OnChange()
        {
            if (Change != null) Change();
        }

        private void OnActivateitem(object item)
        {
            if (ActivatedItem != null)
            {
                ActiveItem = (ReconciliationContextItem)item;
            } 
        }

        public void setMeasure(Kernel.Domain.Measure measure)
        {
            if (ActiveItem == null) return;
            if (reconciliationContext == null) reconciliationContext = new Kernel.Domain.ReconciliationContext();
            reconciliationContext.amountMeasure = measure;
            if(cansetMeasure(ActiveItem)) this.ActiveItem.setValue(measure);
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            if (ActiveItem == null) return;
            if (canSetValue(ActiveItem) || cansetMeasure(ActiveItem)) return;
            if (reconciliationContext == null) reconciliationContext = new Kernel.Domain.ReconciliationContext();
            if (ActiveItem == postingAttributePanel) reconciliationContext.postingNbreAttribute = attribute;
            else if (ActiveItem == accountNbrAttributePanel) reconciliationContext.accountNbreAttribute = attribute;
            else if (ActiveItem == accountNameAttributePanel) reconciliationContext.accountNameAttribute = attribute;
            else if (ActiveItem == reconciliationAttributePanel) reconciliationContext.recoNbreAttribute = attribute;
            else if (ActiveItem == dcAttributePanel) reconciliationContext.dcNbreAttribute = attribute;
            this.ActiveItem.setValue(attribute);
        }

        public void setAttributeValue(Kernel.Domain.AttributeValue value)
        {
            if (ActiveItem == writeOffAccountPanel) setWriteOffAccount(value);
        }

        protected void setWriteOffAccount(Kernel.Domain.AttributeValue value)
        {
            Kernel.Domain.Attribute attribute = ModelService.getAttributeByValue(value.oid.Value);
            if (reconciliationContext.accountNbreAttribute == null)
            {
                reconciliationContext.accountNbreAttribute = attribute;
                accountNbrAttributePanel.setValue(attribute);
            }
            else if (reconciliationContext.accountNbreAttribute.oid != attribute.oid)
            {
                MessageDisplayer.DisplayWarning("Wrong write off account ", "Write off Account must be a value of : " + reconciliationContext.accountNbreAttribute.name);
                return;                
            }
            reconciliationContext.writeOffAccount = value;
            this.writeOffAccountPanel.setValue(value);
        }

        public bool canSetValue(ReconciliationContextItem item) 
        {
            //if (item == creditValuePanel || item == debitValuePanel) return true;
            if (item == writeOffAccountPanel) return true;
            else return false;
        }

        public bool cansetMeasure(ReconciliationContextItem item)
        {
            if (item == amountMeasurePanel) return true;
            return false;
        }


        public List<object> getEditableControls()
        {
            List<object> list = new List<object>();
            list.Add(postingAttributePanel);
            list.Add(reconciliationAttributePanel);
            list.Add(accountNbrAttributePanel);
            list.Add(accountNameAttributePanel);
            list.Add(dcAttributePanel);
            //list.Add(debitValuePanel);
            //list.Add(creditValuePanel);
            list.Add(writeOffAccountPanel);
            list.Add(amountMeasurePanel);
            return list;
        }
    }
}
