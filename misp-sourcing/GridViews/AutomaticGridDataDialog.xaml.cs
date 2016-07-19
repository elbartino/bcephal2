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
using System.Windows.Shapes;

namespace Misp.Sourcing.GridViews
{
    /// <summary>
    /// Interaction logic for AutomaticGridDataDialog.xaml
    /// </summary>
    public partial class AutomaticGridDataDialog : Window
    {
        public InputGridService InputGridService { get; set; }

        public AutomaticGridDataDialog()
        {
            InitializeComponent();
            InitializeHandlers();
            NewGridRadioButton.IsChecked = true;
        }

        public AutomaticGridData Fill()
        {
            AutomaticGridData data = new AutomaticGridData();
            if (NewGridRadioButton.IsChecked.Value) data.action = AutomaticGridAction.NEW;
            else
            {
                if (attemptRadioButton.IsChecked.Value) data.action = AutomaticGridAction.ATTEMPT;
                else if (replaceRadioButton.IsChecked.Value) data.action = AutomaticGridAction.REPLACE;
                Object obj = gridComboBox.SelectedItem;
                if (obj != null && obj is BrowserData)
                {
                    data.tableOid = ((BrowserData)obj).oid;
                }
                else
                {
                    MessageDisplayer.DisplayError("Wromg grid", "Select the grid to modify");
                    return null;
                }
            }
            data.tableName = NewGridNameTextBox.Text.Trim();
            
            return data;
        }

        public void loadGrids()
        {
            gridComboBox.ItemsSource = InputGridService.getBrowserDatas();
        }

        private void InitializeHandlers()
        {
            NewGridRadioButton.Checked += OnChecked;
            ModifyGridRadioButton.Checked += OnChecked;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            NewGridNameTextBox.IsEnabled = NewGridRadioButton.IsChecked.Value;
            gridComboBox.IsEnabled = ModifyGridRadioButton.IsChecked.Value;
            attemptRadioButton.IsEnabled = ModifyGridRadioButton.IsChecked.Value;
            replaceRadioButton.IsEnabled = ModifyGridRadioButton.IsChecked.Value;
            if (NewGridRadioButton.IsChecked.Value)
            {
                NewGridNameTextBox.SelectAll();
                NewGridNameTextBox.Focus();
            }
            else
            {
                gridComboBox.Focus();
            }
        }

    }
}
