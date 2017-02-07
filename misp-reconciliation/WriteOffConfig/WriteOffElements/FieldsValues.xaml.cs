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
    /// Interaction logic for FieldsValues.xaml
    /// </summary>
    public partial class FieldsValues : StackPanel
    {

        public WriteOffField writeOffField { get; set; }

        public event AddEventHandler OnAddField;

        public event DeleteEventHandler OnDeleteField;

        public event ActivateEventHandler ActivateFieldPanel;

        public FieldsValues()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            this.NewButton.Click += OnHandledButton;
            this.DeleteButton.Click += OnHandledButton;
        }

        private void OnHandledButton(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ButtonActions((Button)sender);
            }
        }       

        private void ButtonActions(Button button)
        {
            if (button == NewButton)
            {
                if (OnAddField != null) OnAddField(null);
            }
            if (button == DeleteButton)
            {
                if (OnDeleteField != null) OnDeleteField(null);
            }
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
             string name = writeOffField.attributeField != null ? writeOffField.attributeField.name :
                 writeOffField.periodField != null ? writeOffField.periodField.name :
                 writeOffField.measureField != null ? writeOffField.measureField.name :"" ;
             this.ValueTypeTextBox.Text = name;       
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.writeOffField.setAttribute(attribute);
            display();
        }

        public void setPeriodName(Kernel.Domain.PeriodName PeriodName)
        {
            this.writeOffField.setPeriodName(PeriodName);
            display();
        }

        public void setMeasure(Kernel.Domain.Measure measure)
        {
            this.writeOffField.setMeasure(measure);
            display();
        }

    }
}
