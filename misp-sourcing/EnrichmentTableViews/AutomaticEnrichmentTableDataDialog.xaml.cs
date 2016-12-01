using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
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

namespace Misp.Sourcing.EnrichmentTableViews
{
    /// <summary>
    /// Interaction logic for AutomaticEnrichmentTableDataDialog.xaml
    /// </summary>
    public partial class AutomaticEnrichmentTableDataDialog : Window
    {
        //public EnrichmentTableService EnrichmentTableService { get; set; }

        public AutomaticEnrichmentTableDataDialog()
        {
            InitializeComponent();
            InitializeHandlers();
            NewTableRadioButton.IsChecked = true;
        }
        
        public AutomaticGridData Fill()
        {
            AutomaticGridData data = new AutomaticGridData();
            if (NewTableRadioButton.IsChecked.Value) data.action = AutomaticGridAction.NEW;
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
                    MessageDisplayer.DisplayError("Wromg Table", "Select the table to modify");
                    return null;
                }
            }
            data.tableName = NewTableNameTextBox.Text.Trim();
            data.overrideExisting = overrideCheckBox.IsChecked.Value;
            return data;
        }

        public void loadTables()
        {
            //gridComboBox.ItemsSource = InputGridService.getBrowserDatas();
        }

        private void InitializeHandlers()
        {
            NewTableRadioButton.Checked += OnChecked;
            ModifyTableRadioButton.Checked += OnChecked;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            NewTableNameTextBox.IsEnabled = NewTableRadioButton.IsChecked.Value;
            gridComboBox.IsEnabled = ModifyTableRadioButton.IsChecked.Value;
            attemptRadioButton.IsEnabled = ModifyTableRadioButton.IsChecked.Value;
            replaceRadioButton.IsEnabled = ModifyTableRadioButton.IsChecked.Value;
            if (NewTableRadioButton.IsChecked.Value)
            {
                NewTableNameTextBox.SelectAll();
                NewTableNameTextBox.Focus();
            }
            else
            {
                gridComboBox.Focus();
            }
        }

    }
}