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
            this.postingAttribute.setContextItemContent("Posting Attribute");
            this.accountAttribute.setContextItemContent("Account Attribute");
            this.reconciliationAttribute.setContextItemContent("Reconciliation Attribute");
            this.dcAttribute.setContextItemContent("Deb/Cred Attribute");
            this.creditValue.setContextItemContent("Credit Value");
            this.debitValue.setContextItemContent("Debit Value");
            InitializeHandlers();
        }

        public void display(Kernel.Domain.ReconciliationContext reconciliationContext) 
        {
            this.postingAttribute.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
            this.accountAttribute.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
            this.reconciliationAttribute.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
            this.dcAttribute.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
            this.creditValue.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
            this.debitValue.setContextItemValue(reconciliationContext.postingNbreAttribute.name);
        }

        public Kernel.Domain.ReconciliationContext Fill() 
        {
            return new Kernel.Domain.ReconciliationContext();
        }

        public void InitializeHandlers()
        {
             this.postingAttribute.ActivatedItem += OnActivateitem;
             this.dcAttribute.ActivatedItem += OnActivateitem;
             this.creditValue.ActivatedItem += OnActivateitem;
             this.debitValue.ActivatedItem += OnActivateitem;
             this.reconciliationAttribute.ActivatedItem += OnActivateitem;
             this.accountAttribute.ActivatedItem += OnActivateitem;
        }

        private void OnActivateitem(object item)
        {
            if (ActivatedItem != null)
            {
                ActiveItem = (ReconciliationContextItem)item;
            } 
        }


        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            if (ActiveItem == null) return;
            if (canSetValue(ActiveItem)) return;
            if (reconciliationContext == null) reconciliationContext = new Kernel.Domain.ReconciliationContext();
            if (ActiveItem == postingAttribute) reconciliationContext.postingNbreAttribute = attribute;
            else if (ActiveItem == accountAttribute) reconciliationContext.accountNbreAttribute = attribute;
            else if (ActiveItem == reconciliationAttribute) reconciliationContext.recoNbreAttribute = attribute;
            else if (ActiveItem == dcAttribute) reconciliationContext.dcNbreAttribute = attribute;
            this.ActiveItem.setAttribute(attribute);
        }

        public void setAttributeValue(Kernel.Domain.AttributeValue value)
        {
            if (ActiveItem == null) return;
            if (!canSetValue(ActiveItem)) return;
            if (reconciliationContext == null) reconciliationContext = new Kernel.Domain.ReconciliationContext();
            if (reconciliationContext.dcNbreAttribute == null)
            {
                reconciliationContext.dcNbreAttribute = value.attribut;
                dcAttribute.setAttribute(value.attribut);
            }
            else 
            {
                if (!reconciliationContext.dcNbreAttribute.Equals(value.attribut)) 
                {
                    return;
                }
            }
            
            if (ActiveItem == debitValue) reconciliationContext.debitAttributeValue = value;
            else if (ActiveItem == creditValue) reconciliationContext.creditAttributeValue = value;

            this.ActiveItem.setAttributeValue(value);
        }

        public bool canSetValue(ReconciliationContextItem item) 
        {
            if (item == creditValue || item == debitValue) return true;
            else return false;
        }

    }
}
