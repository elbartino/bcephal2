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
        
        public ReconciliationContextItem ActiveItem { get; set; }

        public Kernel.Domain.ReconciliationContext reconciliationContext { get; set; }

        public ReconciliationContextPanel()
        {
            InitializeComponent();
            this.postingAttributePanel.setContextItemContent("Posting Nbr Attribute");
            this.accountAttributePanel.setContextItemContent("Account Nbr Attribute");
            this.reconciliationAttributePanel.setContextItemContent("Reco Nbr Attribute");
            this.dcAttributePanel.setContextItemContent("Debit/Credit Attribute");
            this.creditValuePanel.setContextItemContent("Credit Value");
            this.debitValuePanel.setContextItemContent("Debit Value");
            this.amountMeasurePanel.setContextItemContent("Amount Measure");
            InitializeHandlers();
        }

        public void display(Kernel.Domain.ReconciliationContext reconciliationcontext) 
        {
            if (reconciliationcontext == null) return;
            this.reconciliationContext = reconciliationcontext;
            this.postingAttributePanel.setValue(reconciliationContext.postingNbreAttribute);
            this.accountAttributePanel.setValue(reconciliationContext.accountNbreAttribute);
            this.reconciliationAttributePanel.setValue(reconciliationContext.recoNbreAttribute);
            this.dcAttributePanel.setValue(reconciliationContext.dcNbreAttribute);
            this.creditValuePanel.setValue(reconciliationContext.creditAttributeValue);
            this.debitValuePanel.setValue(reconciliationContext.debitAttributeValue);
            this.amountMeasurePanel.setValue(reconciliationContext.amountMeasure);
        }
        
        public void InitializeHandlers()
        {
            this.postingAttributePanel.ActivatedItem += OnActivateitem;
            this.dcAttributePanel.ActivatedItem += OnActivateitem;
            this.creditValuePanel.ActivatedItem += OnActivateitem;
            this.debitValuePanel.ActivatedItem += OnActivateitem;
            this.reconciliationAttributePanel.ActivatedItem += OnActivateitem;
            this.accountAttributePanel.ActivatedItem += OnActivateitem;
            this.amountMeasurePanel.ActivatedItem += OnActivateitem;
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
            this.ActiveItem.setValue(measure);
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            if (ActiveItem == null) return;
            if (canSetValue(ActiveItem)) return;
            if (reconciliationContext == null) reconciliationContext = new Kernel.Domain.ReconciliationContext();
            if (ActiveItem == postingAttributePanel) reconciliationContext.postingNbreAttribute = attribute;
            else if (ActiveItem == accountAttributePanel) reconciliationContext.accountNbreAttribute = attribute;
            else if (ActiveItem == reconciliationAttributePanel) reconciliationContext.recoNbreAttribute = attribute;
            else if (ActiveItem == dcAttributePanel) reconciliationContext.dcNbreAttribute = attribute;
            this.ActiveItem.setValue(attribute);
        }

        public void setAttributeValue(Kernel.Domain.AttributeValue value)
        {
            if (ActiveItem == null) return;
            if (!canSetValue(ActiveItem)) return;
            if (reconciliationContext.dcNbreAttribute == null)
            {
                reconciliationContext.dcNbreAttribute = value.attribut;
                dcAttributePanel.setValue(value.attribut);
            }
            else 
            {
                if (reconciliationContext.dcNbreAttribute.Equals(value.attribut)) 
                {
                    reconciliationContext.dcNbreAttribute = value.attribut;
                }
            }
            
            if (ActiveItem == debitValuePanel) reconciliationContext.debitAttributeValue = value;
            else if (ActiveItem == creditValuePanel) reconciliationContext.creditAttributeValue = value;

            this.ActiveItem.setValue(value);
        }

        public bool canSetValue(ReconciliationContextItem item) 
        {
            if (item == creditValuePanel || item == debitValuePanel) return true;
            else return false;
        }


        public List<object> getEditableControls()
        {
            List<object> list = new List<object>();
            list.Add(postingAttributePanel);
            list.Add(reconciliationAttributePanel);
            list.Add(accountAttributePanel);
            list.Add(dcAttributePanel);
            list.Add(debitValuePanel);
            list.Add(creditValuePanel);
            list.Add(amountMeasurePanel);
            return list;
        }
    }
}
