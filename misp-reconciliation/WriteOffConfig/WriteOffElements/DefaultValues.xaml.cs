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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DefaultValues : Grid
    {
        public Kernel.Domain.WriteOffFieldValue writeOffValueField { get; set; }

        public event ActivateEventHandler ActivateFiedValue;

        public event ChangeItemEventHandler ItemChanged;

        public DefaultValues()
        {
            InitializeComponent();
            this.DefaultValuesCombobox.ItemsSource = new String[] 
            {
                  WriteOffFieldValueType.LEFT_SIDE.label,
                  WriteOffFieldValueType.RIGHT_SIDE.label,
                  WriteOffFieldValueType.CUSTOM.label,
                  ""
            };
            InitializeHandlers();
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            if (this.writeOffValueField == null) return;
            this.DefaultValuesCombobox.SelectedItem = writeOffValueField.defaultValueType.ToString();
        }

        public void InitializeHandlers()
        {
            this.DefaultValuesCombobox.SelectionChanged += OnChooseDefaultValues;
            this.DefaultValuesCombobox.GotFocus += OnGotFocus;
            this.DefaultValuesCombobox.MouseLeftButtonDown += OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivateFiedValue != null) ActivateFiedValue(null);
        }

        private void OnChooseDefaultValues(object sender, SelectionChangedEventArgs e)
        {
            if (ItemChanged != null) ItemChanged(DefaultValuesCombobox.SelectedItem);
        }
    }
}
