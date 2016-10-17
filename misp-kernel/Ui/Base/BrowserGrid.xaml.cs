using DataGridFilterLibrary;
using DataGridFilterLibrary.Support;
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

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for BrowserGrid.xaml
    /// </summary>
    public partial class BrowserGrid : DataGrid
    {

        public BrowserGridContextMenu BrowserGridContextMenu { get; set; }
        public FilterHandler FilterHandler { get; set; }
        public FilterDatas FilterData { get; set; }

        public event ChangeEventHandler FilterChanged;

        public BrowserGrid()
        {
            InitializeComponent();
            this.BrowserGridContextMenu = new BrowserGridContextMenu();
            this.ContextMenu = this.BrowserGridContextMenu;


            FilterHandler = new FilterHandler();
            FilterHandler.Handler += OnFilter;
            DataGridExtensions.SetFilterHandler(this, FilterHandler);

            FilterData = new FilterDatas();
            DataGridExtensions.SetFilterDatas(this, FilterData);

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, SelectAll_Executed));
        }
        
        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.ItemsSource != null)
            {
                int itemsCount = dataGrid.Items.Count;
                if (dataGrid.SelectedItems.Count < itemsCount) dataGrid.SelectAll();
                else dataGrid.UnselectAll();
            }
        }

        public virtual void OnFilter(DataGridFilterLibrary.Support.FilterData data)
        {
            if (FilterChanged != null) FilterChanged();
        }

        public void hideContextMenu()
        {
            this.BrowserGridContextMenu.Visibility = System.Windows.Visibility.Collapsed;
        }


        
    }
}
