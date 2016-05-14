using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.Table;
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

namespace Misp.Sourcing.MultipleFilesUpload
{
    /// <summary>
    /// Interaction logic for Step2.xaml
    /// </summary>
    public partial class Step2 : Grid
    {
        private bool alreadyDisplay;

        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid TableGrid { get; set; }

        public Step2()
        {
            alreadyDisplay = false;
            InitializeComponent();
            InitializeExcelFilesGrid();
        }

        public void DisplayTales(InputTableService service)
        {
            if (alreadyDisplay || service == null) return;
            TableGrid.ItemsSource = service.getBrowserDatas();
            alreadyDisplay = true;
        }


        public bool Validate()
        {
            if (TableGrid.SelectedItem == null)
            {
                MessageDisplayer.DisplayWarning("Select input table template", "You have to select an input table template before continue!");
                return false;
            }
             if(TableGrid.SelectedItems!=null && TableGrid.SelectedItems.Count>1)
                
             {
                 MessageDisplayer.DisplayWarning("Select input table template", "you cannot select more than one table template!");
                 return false;
             }
             return true;
        }

        private void InitializeExcelFilesGrid()
        {
            TableGrid = new BrowserGrid();
            TableGrid.hideContextMenu();

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            TableGrid.RowHeaderTemplate = template;

            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = "Name";
            column.Width = new DataGridLength(1,DataGridLengthUnitType.Star);
            column.Binding = new System.Windows.Data.Binding("name");
            TableGrid.Columns.Add(column);

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            TableGrid.AlternatingRowBackground = bruch;
            TableGrid.AlternatingRowBackground.Opacity = 0.3;

            this.GridPanel.Content = TableGrid;
            this.TableGrid.SelectionChanged += TableGrid_SelectionChanged;
        }

        void TableGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableGrid.SelectedItem!=null)
            this.TableTextBox.Text = ((InputTableBrowserData)TableGrid.SelectedItem).name;

        }
    }
}
