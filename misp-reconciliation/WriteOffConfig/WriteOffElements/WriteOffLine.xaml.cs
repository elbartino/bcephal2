using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for WriteOffLine.xaml
    /// </summary>
    public partial class WriteOffLine : Grid
    {
        public Kernel.Domain.WriteOffField writeOffField { get; set; }

        public WriteOffLine()
        {
            InitializeComponent();
            this.valueDatePicker.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            if (writeOffField == null) return;
            string name = "";
            if (writeOffField.isAttribute())
            {
                name = this.writeOffField.attributeField.name;
                if (this.writeOffField.defaultValueTypeEnum == null || this.writeOffField.defaultValueTypeEnum == WriteOffFieldValueType.CUSTOM)
                {
                    if (this.writeOffField.writeOffFieldValueListChangeHandler != null && this.writeOffField.writeOffFieldValueListChangeHandler.Items.Count > 0)
                    {
                        this.valueCombobox.ItemsSource = this.writeOffField.writeOffFieldValueListChangeHandler.Items.ToList();                        
                    }
                    else
                    {
                        ModelService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetModelService();
                        this.valueCombobox.ItemsSource = service.getLeafAttributeValues(this.writeOffField.attributeField.oid.Value);
                    }
                }
                else if (this.writeOffField.defaultValueTypeEnum == WriteOffFieldValueType.LEFT_SIDE)
                {
                    this.valueCombobox.IsEnabled = false;
                }
                else if (this.writeOffField.defaultValueTypeEnum == WriteOffFieldValueType.RIGHT_SIDE)
                {
                    this.valueCombobox.IsEnabled = false;
                }
                this.valueDatePicker.Visibility = System.Windows.Visibility.Collapsed;
                this.valueCombobox.Visibility = System.Windows.Visibility.Visible;
            }
            else if (writeOffField.isPeriod())
            {
                name = this.writeOffField.periodField.name;                
                this.valueDatePicker.SelectedDate = DateTime.Now;
                if (this.writeOffField.defaultValueTypeEnum == null || this.writeOffField.defaultValueTypeEnum == WriteOffFieldValueType.TODAY)
                {
                    this.valueDatePicker.IsEnabled = false;
                }
                this.valueDatePicker.Visibility = System.Windows.Visibility.Visible;
                this.valueCombobox.Visibility = System.Windows.Visibility.Collapsed;
            }
            bool addStart = this.writeOffField.mandatory;
            this.nameLabel.Content = name + (addStart ? "*" :"") ;
        }

        public WriteOffField Fill()
        {
            WriteOffField field = null;
            if (writeOffField.isAttribute() && this.valueCombobox.SelectedItem != null)
            {
                field = new WriteOffField();
                field.setAttribute(writeOffField.attributeField);
                Object value = this.valueCombobox.SelectedItem;
                if (value != null && value is AttributeValue) field.value = (AttributeValue)value;
                else if (value != null && value is BrowserData)
                {
                    field.value = new AttributeValue();
                    field.value.oid = ((BrowserData)value).oid;
                    field.value.name = ((BrowserData)value).name;
                }
            }
            else if (writeOffField.isPeriod() && this.valueDatePicker.SelectedDate.HasValue)
            {
                field = new WriteOffField();
                field.setPeriodName(writeOffField.periodField);
                field.dateTime = this.valueDatePicker.SelectedDate.Value; 
            }
            return field;
        }

        public bool Validate()
        {
            if (writeOffField.mandatory)
            {
                if (writeOffField.isAttribute() && this.valueCombobox.SelectedItem == null)
                {
                    MessageDisplayer.DisplayWarning("Write off", "The field '" + this.writeOffField.attributeField.name  + "' is mandatory!");
                    return false;
                }
                else if (writeOffField.isPeriod() && !this.valueDatePicker.SelectedDate.HasValue)
                {
                    MessageDisplayer.DisplayWarning("Write off", "The field '" + this.writeOffField.periodField.name + "' is mandatory!");
                    return false;
                }
            }
            return true;
        }


    }
}
