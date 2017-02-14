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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for WriteOffValueItem.xaml
    /// </summary>
    public partial class WriteOffValueItem : Grid
    {
        public event AddEventHandler OnAddFieldValue;
        public event DeleteEventHandler OnDeleteFieldValue;
        public event ActivateEventHandler ActivateFiedValue;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        public Kernel.Domain.WriteOffFieldValue WriteOffFieldValue { get; set; }

        public int Index { get; set; }

        public WriteOffValueItem()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public WriteOffValueItem(Kernel.Domain.WriteOffFieldValue valueField) : base()
        {
            display(valueField);
        }
              
        public void showRowLabel(bool show = true) 
        {
            this.PossibleValues.showRowLabel(show);
            this.DefaultValues.showRowLabel(show);
        }

        public void display()
        {
            this.PossibleValues.display();
        }

        public void display(Kernel.Domain.WriteOffFieldValue valueField)
        {
            this.PossibleValues.writeOffValueField = valueField;
            this.PossibleValues.display();

            this.DefaultValues.writeOffValueField = valueField;
            this.DefaultValues.display();
        }

        public void InitializeHandlers()
        {
            this.GotFocus += OnGotFocus;
            this.MouseLeftButtonDown += OnGotFocus;
            this.PossibleValues.OnAddFieldValue += OnAddFieldsValue;
            this.PossibleValues.OnDeleteFieldValue += OnDeleteFieldsValue;
            this.PossibleValues.ActivateFiedValue += OnActivateFieldsValue;
            this.DefaultValues.ActivateFiedValue += OnActivateFieldsValue;
            this.DefaultValues.ItemChanged += OnDefaultValuesChanged;
        }

        private void OnDefaultValuesChanged(object item)
        {
            getCurrentFieldValue().setDefaultValue(item);
            if (ItemChanged != null) ItemChanged(this.WriteOffFieldValue);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivateFiedValue != null) ActivateFiedValue(this);
        }

        private void OnActivateFieldsValue(object item)
        {
            if (ActivateFiedValue != null) ActivateFiedValue(this);
        }

        private void OnAddFieldsValue(object item)
        {
            if (OnAddFieldValue != null) OnAddFieldValue(null);
        }

        private void OnDeleteFieldsValue(object item)
        {
            if (OnDeleteFieldValue != null) OnDeleteFieldValue(this);
        }

        public Kernel.Domain.WriteOffFieldValue FillWriteOffField()
        {
            return new Kernel.Domain.WriteOffFieldValue();
        }

        public void setPeriodInterval(Kernel.Domain.PeriodInterval periodInterval)
        {
            this.getCurrentFieldValue().setPeriodInterval(periodInterval);
            if (ItemChanged != null) ItemChanged(this.WriteOffFieldValue);
            display();
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.getCurrentFieldValue().setAttribute(attribute);
            if (ItemChanged != null) ItemChanged(this.WriteOffFieldValue);
            display();
        }

        public void setAttributeValue(Kernel.Domain.AttributeValue value)
        {
            this.getCurrentFieldValue().setValue(value);
            if (ItemChanged != null) ItemChanged(this.WriteOffFieldValue);
            display();
        }

        public WriteOffFieldValue getCurrentFieldValue() 
        {
            if (this.WriteOffFieldValue == null)
            {
                this.WriteOffFieldValue = new WriteOffFieldValue();
                this.WriteOffFieldValue.position = -1;
            }
            this.PossibleValues.writeOffValueField = this.WriteOffFieldValue;
            return this.WriteOffFieldValue;                
        }

        public void setDateView() 
        {
            this.PossibleValues.setDateView();
            this.DefaultValues.setDateView();
        }

        public void removeDateView() 
        {
            this.PossibleValues.removeDateView();
            this.DefaultValues.removeDateView();
        }


        public void UpdateObject(WriteOffFieldValue fieldValue)
        {
            this.WriteOffFieldValue = fieldValue;
        }
    }
}
