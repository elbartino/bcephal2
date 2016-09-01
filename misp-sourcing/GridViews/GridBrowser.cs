using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Sourcing.GridViews
{
    public class GridBrowser : Grid
    {

        public Misp.Kernel.Ui.Base.ChangeItemEventHandler SortEventHandler { get; set; }

        public delegate bool EditCellEventHandler(GrilleEditedElement element);
        public EditCellEventHandler EditEventHandler { get; set; }

        public DeleteEventHandler DeleteEventHandler { get; set; }
        public ChangeItemEventHandler DuplicateEventHandler { get; set; }

        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;

        private List<string> columnNames = new List<string>(0);

        public DataGrid grid;

        public bool RebuildGrid = true;
        
        public Grille Grille { get; set; }

        public InputGridService Service { get; set; }

        public BrowserGridContextMenu BrowserGridContextMenu { get; set; }

        public GridBrowser()
        {
            
        }

        public List<long> GetSelectedOis()
        {
            List<long> oids = new List<long>(0);
            foreach (Object row in grid.SelectedItems)
            {
                if (row is GridItem)
                {
                    GridItem item = (GridItem)row;
                    int? oid = item.GetOid();
                    if (oid.HasValue) oids.Add(oid.Value);
                }
            }
            return oids;
        }

        protected void buildGrid()
        {
            if (grid != null)
            {
                grid.CellEditEnding -= OnCellEditEnding;
                grid.Sorting -= OnSort;
                grid.SelectionChanged -= onSelectionchange;
                if (this.BrowserGridContextMenu != null)
                {
                    //this.BrowserGridContextMenu.NewMenuItem.Click -= OnNewMenuClick;
                    this.BrowserGridContextMenu.SaveAsMenuItem.Click -= OnDuplicateMenuClick;
                    this.BrowserGridContextMenu.DeleteMenuItem.Click -= OnDeleteMenuClick;
                    grid.ContextMenuOpening -= OnContextMenuOpening;
                    grid.ContextMenu = null;
                }
            }
            this.Children.Clear();
            
            grid = new DataGrid();
            initializeContextMenu();
            grid.SelectionChanged += onSelectionchange;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.White.Color.ToString());
            grid.Background = bruch;
            grid.BorderBrush = bruch;
            grid.HeadersVisibility = DataGridHeadersVisibility.All;
            grid.GridLinesVisibility = DataGridGridLinesVisibility.All;
            grid.RowHeaderWidth = 20;
            grid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            grid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            grid.CanUserReorderColumns = false;
            grid.CanUserResizeColumns = true;
            grid.CanUserSortColumns = true;
            grid.AutoGenerateColumns = false;
            grid.CanUserAddRows = false;
            grid.IsReadOnly = false;
            grid.MinColumnWidth = 70;
            grid.MaxColumnWidth = 1000;
            grid.SelectionUnit = DataGridSelectionUnit.FullRow;

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            grid.RowHeaderTemplate = template;

            grid.CellEditEnding += OnCellEditEnding;
            grid.Sorting += OnSort;
        }

        private void onSelectionchange(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender) return;
            if (ChangeHandler != null) ChangeHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void initializeContextMenu()
        {
            if (!this.Grille.report)
            {
                this.BrowserGridContextMenu = new BrowserGridContextMenu();
                this.BrowserGridContextMenu.NewMenuItem.Header = "New line";
                this.BrowserGridContextMenu.SaveAsMenuItem.Header = "Duplicate";
                this.BrowserGridContextMenu.NewMenuItem.IsEnabled = false;
                this.BrowserGridContextMenu.SaveAsMenuItem.IsEnabled = false;
                this.BrowserGridContextMenu.DeleteMenuItem.IsEnabled = false;

                this.BrowserGridContextMenu.Items.Clear();
                //this.BrowserGridContextMenu.Items.Add(this.BrowserGridContextMenu.NewMenuItem);
                this.BrowserGridContextMenu.Items.Add(this.BrowserGridContextMenu.SaveAsMenuItem);
                //this.BrowserGridContextMenu.Items.Add(new Separator());
                this.BrowserGridContextMenu.Items.Add(this.BrowserGridContextMenu.DeleteMenuItem);

                //this.BrowserGridContextMenu.NewMenuItem.Click += OnNewMenuClick;
                this.BrowserGridContextMenu.SaveAsMenuItem.Click += OnDuplicateMenuClick;
                this.BrowserGridContextMenu.DeleteMenuItem.Click += OnDeleteMenuClick;
                grid.ContextMenuOpening += OnContextMenuOpening;

                grid.ContextMenu = BrowserGridContextMenu;
            }
        }

        private void OnDeleteMenuClick(object sender, RoutedEventArgs e)
        {
            if (DeleteEventHandler != null) DeleteEventHandler(GetSelectedOis());
        }

        private void OnDuplicateMenuClick(object sender, RoutedEventArgs e)
        {
            if (DuplicateEventHandler != null) DuplicateEventHandler(GetSelectedOis());
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.BrowserGridContextMenu != null)
            {
                this.BrowserGridContextMenu.NewMenuItem.IsEnabled = this.grid.SelectedItems.Count > 0;
                this.BrowserGridContextMenu.SaveAsMenuItem.IsEnabled = this.grid.SelectedItems.Count > 0;
                this.BrowserGridContextMenu.DeleteMenuItem.IsEnabled = this.grid.SelectedItems.Count > 0;
            }
        }
        
        private void OnSort(object sender, DataGridSortingEventArgs e)
        {
            DataGridColumn col = e.Column;
            GrilleColumn column = this.Grille.GetColumn(col.Header.ToString());
            e.Handled = true;
            if (SortEventHandler != null) SortEventHandler(column);
        }
        
        protected void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs args)
        {
            GridItem item = (GridItem)this.grid.SelectedItem;
            if (args.EditAction == DataGridEditAction.Commit && item != null)
            {
                int? oid = item.GetOid();
                DataGridColumn col = args.Column;
                GrilleColumn column = this.Grille.GetColumn(col.Header.ToString());
                string oldValue = item.Datas[column.position] != null ? item.Datas[column.position].ToString() : "";
                string newValue = "";
                if (args.EditingElement is TextBox) newValue = ((TextBox)args.EditingElement).Text.Trim();
                else if (args.EditingElement is ComboBox)
                {
                    if (((ComboBox)args.EditingElement).SelectedItem != null) newValue = ((ComboBox)args.EditingElement).SelectedItem.ToString();
                }
                GrilleEditedElement element = new GrilleEditedElement();
                element.column = column;
                element.oid = oid;

                if (newValue.Equals(oldValue)) return;
                if (column.type.Equals(ParameterType.MEASURE.ToString())){
                    decimal val = 0;
                    if (string.IsNullOrWhiteSpace(newValue)) val = 0;
                    else if (decimal.TryParse(newValue, out val)) element.measure = val;
                    else if (decimal.TryParse(newValue.Replace(".", ","), out val)) element.measure = val;
                    else
                    {
                        MessageDisplayer.DisplayError("Wromg measure", "'" + newValue + "'" + " is not a decimal!");
                        args.Cancel = true;
                        return;
                    }
                }
                else if (column.type.Equals(ParameterType.SCOPE.ToString()))
                {
                    if (string.IsNullOrWhiteSpace(newValue)) element.value = null;
                    else element.value = column.getValue(newValue);
                }
                if (column.type.Equals(ParameterType.PERIOD.ToString()))
                {
                    DateTime date;
                    if (string.IsNullOrWhiteSpace(newValue)) element.date = null;
                    else if (DateTime.TryParse(newValue, out date))
                    {
                        element.date = date.ToShortDateString();
                    }
                    else
                    {
                        MessageDisplayer.DisplayError("Wromg date", "'" + newValue + "'" + " is not a date!");
                        args.Cancel = true;
                        return;
                    }
                }

                if (this.EditEventHandler != null)
                {
                    if (!EditEventHandler(element)) args.Cancel = true;
                }
            }
        }

        public void buildColumns(Grille grid)
        {
            this.Grille = grid;
            buildGrid();
            foreach (GrilleColumn column in grid.columnListChangeHandler.Items)
            {
                if (column.show) this.AddColumn(column);
            }
            this.Children.Add(this.grid);
            RebuildGrid = false;
        }
                
        public void displayPage(GrillePage page)
        {
            displayRows(page.rows);
        }

       
        public void displayRows(List<object[]> rows)
        {
            List<GridItem> items = new List<GridItem>(0);
            foreach (object[] row in rows)
            {
                items.Add(new GridItem(row));                
            }
            if (!this.Grille.report)
            {
                items.Add(new GridItem(new object[this.grid.Columns.Count]));
            }

            this.grid.ItemsSource = items;
        }


        public void AddColumn(GrilleColumn grilleColumn)
        {
            String name = grilleColumn.name;
            DataGridColumn column = getColumn(grilleColumn);
            column.Header = name;
            column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column.IsReadOnly = this.Grille.report || this.Grille.reconciliation;
            grid.Columns.Add(column);
            columnNames.Add(name);
        }

        private String getBindingName(GrilleColumn grilleColumn)
        {
            return "Datas[" + grilleColumn.position + "]";
        }

        private DataGridColumn getColumn(GrilleColumn grilleColumn)
        {
            
            if(!this.Grille.report && grilleColumn.type.Equals(ParameterType.SCOPE.ToString())){
                DataGridComboBoxColumn column = new DataGridComboBoxColumn();
                try
                {
                    grilleColumn.values = Service.ModelService.getLeafAttributeValues(grilleColumn.valueOid.Value);
                }catch(Exception){}
                column.ItemsSource = grilleColumn.getValueNames();
                //column.SelectedItemBinding = new Binding("SelectedValue");
                column.SelectedValueBinding = new Binding(getBindingName(grilleColumn));

                return column;
            }

            DataGridTextColumn col = new DataGridTextColumn();
            col.Binding = new Binding(getBindingName(grilleColumn));
            return col;
        }
        

        public void RemoveColumn(String name, int position = -1)
        {
            for (int i = this.grid.Columns.Count - 1; i >= 0; i--)
            {
                DataGridColumn col = this.grid.Columns[i];
                if (!col.Header.Equals(name)) continue;
                this.grid.Columns.Remove(col);
                columnNames.Remove(name);
                break;
            }
        }


        
    }
}
