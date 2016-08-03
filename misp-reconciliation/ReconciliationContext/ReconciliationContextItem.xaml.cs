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
    /// Interaction logic for ReconciliationContextItem.xaml
    /// </summary>
    public partial class ReconciliationContextItem : Grid
    {
        public event Misp.Kernel.Ui.Base.ActivateEventHandler ActivatedItem;

        public event Misp.Kernel.Ui.Base.ChangeEventHandler Changed;        

        public ReconciliationContextItem()
        {
            InitializeComponent();
            this.contextItemLabel.MouseLeftButtonDown += OnMouseDown;
            this.contextItemTextbox.MouseLeftButtonDown += OnMouseDown;
            this.contextItemTextbox.GotFocus += OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivatedItem != null) ActivatedItem(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ActivatedItem != null) ActivatedItem(this);
        }

        public void setContextItemContent(String name) 
        {
            this.contextItemLabel.Content = name;
        }
        
        public void setValue(object value)
        {
            string currentValue = this.contextItemTextbox.Text.Trim();
            if (currentValue != null && currentValue.Equals(value != null ? value.ToString() : "")) return;
            this.contextItemTextbox.Text = value != null ? value.ToString() : "";
            if (Changed != null) Changed();
        }

    }
}
