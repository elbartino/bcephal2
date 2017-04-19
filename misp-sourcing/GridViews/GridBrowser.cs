using DataGridFilterLibrary.Support;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Core.ConditionalFormatting;
using System.Windows.Media;

namespace Misp.Sourcing.GridViews
{
    public class GridBrowser : Grid
    {
        public FormatConditions FormatConditionsEventHandler;

        public ChangeEventHandler FilterEventHandler { get; set; }

        public ChangeItemEventHandler SortEventHandler { get; set; }

        public delegate Object[] EditCellEventHandler(GrilleEditedElement element);
        public EditCellEventHandler EditEventHandler { get; set; }

        public DeleteEventHandler DeleteEventHandler { get; set; }
        public ChangeItemEventHandler DuplicateEventHandler { get; set; }

        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;
        public Kernel.Ui.Base.SelectedItemChangedEventHandler SelectedItemChangedHandler;
        public Kernel.Ui.Base.SelectedItemChangedEventHandler DeselectedItemChangedHandler;

        private List<string> columnNames = new List<string>(0);
        
        public GridControl gridControl;

        public bool RebuildGrid = true;
        
        public Grille Grille { get; set; }

        public InputGridService Service { get; set; }

        public bool IsReadOnly { get; set; }

        public bool AllowEmptyValue = false;
        public bool AllowUnknowValueForScope = false;

        public GridBrowser()
        {
            ThemeManager.SetThemeName(this, "Office2016White");
        }

        public List<long> GetSelectedOis()
        {
            List<long> oids = new List<long>(0);
            foreach (Object row in gridControl.SelectedItems)
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
            if (gridControl != null)
            {
                gridControl.EndSorting -= OnEndSorting;
                gridControl.FilterChanged -= OnFilterChanged;
                gridControl.SelectionChanged -= OnSelectionChanged;
                ((GridTableView)gridControl.View).CellValueChanged -= OnCellValueChanged;
                ((GridTableView)gridControl.View).ValidateCell -= OnValidateCell;
                ((GridTableView)gridControl.View).SortEventHandler -= OnSort;
                ((GridTableView)gridControl.View).Menu.DeleteItem.ItemClick -= OnDelete;
                ((GridTableView)gridControl.View).Menu.DuplicateItem.ItemClick -= OnDuplicate;
                gridControl.View = null;
            }

            gridControl = new GridControl();
            GridControl.AllowInfiniteGridSize = true;
            GridTableView view = new GridTableView(gridControl);            
            gridControl.SelectionMode = MultiSelectMode.MultipleRow;
            gridControl.View = view; 
            view.ShowGroupPanel = Grille.report && !Grille.reconciliation;
            view.AllowEditing = !this.IsReadOnly && !Grille.report && !Grille.reconciliation;

            gridControl.EndSorting += OnEndSorting;

            gridControl.FilterChanged += OnFilterChanged;
            gridControl.SelectionChanged += OnSelectionChanged;
            view.SortEventHandler += OnSort;
            view.CellValueChanged += OnCellValueChanged;
            view.ValidateCell += OnValidateCell;
            
            view.Menu.DeleteItem.ItemClick += OnDelete;
            view.Menu.DuplicateItem.ItemClick += OnDuplicate;
                        
            view.IsRowCellMenuEnabled = !this.IsReadOnly && !Grille.IsReadOnly();

            if(FormatConditionsEventHandler != null) 
            {
                List<FormatCondition> conditions = FormatConditionsEventHandler();
                if(conditions != null)
                {
                    foreach(FormatCondition condition in conditions)
                    {
                        view.FormatConditions.Add(condition);
                    }
                }
            }

        }

        

        private bool allowSort = true;
        private void OnEndSorting(object sender, RoutedEventArgs e)
        {
            
            if (allowSort && this.Grille != null && gridControl.SortInfo != null)
            {
                OnClearSort();
                foreach (GridSortInfo info in gridControl.SortInfo)
                {
                    GrilleColumn column = this.Grille.GetColumn(info.FieldName);
                    if (column != null)
                    {
                        column.orderAsc = info.SortOrder.Equals(ListSortDirection.Ascending);
                    }
                }

                if (allowSort && SortEventHandler != null)
                {
                    allowSort = false;
                    SortEventHandler(null);
                }
                allowSort = true;
            }
        }

        

        private void OnSort(object columnName)
        {            
            if (this.Grille != null && columnName != null)
            {                
                GrilleColumn column = this.Grille.GetColumn(columnName.ToString());
                if (column != null)
                {
                    bool orderAsc = column.orderAsc.HasValue && column.orderAsc.Value;
                    OnClearSort();
                    column.orderAsc = !orderAsc;
                }
                if (SortEventHandler != null) SortEventHandler(column);
            }
        }

        private void OnClearSort()
        {
            if (this.Grille != null)
            {
                foreach (GrilleColumn column in this.Grille.columnListChangeHandler.Items)
                {
                    column.orderAsc = null;
                }   
            }
        }

        private void OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (ChangeHandler != null) ChangeHandler();
            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                object obj = gridControl.GetRow(e.ControllerRow);
                if (obj != null && SelectedItemChangedHandler != null) SelectedItemChangedHandler(obj);
            }
            else if (e.Action == System.ComponentModel.CollectionChangeAction.Remove)
            {
                object obj = gridControl.GetRow(e.ControllerRow);
                if (obj != null && DeselectedItemChangedHandler != null) DeselectedItemChangedHandler(obj);
            }
            else if (e.Action == System.ComponentModel.CollectionChangeAction.Refresh)
            {
                if (gridControl.SelectedItems.Count <= 0 && DeselectedItemChangedHandler != null) DeselectedItemChangedHandler(gridControl.ItemsSource);
                if (gridControl.SelectedItems.Count >= gridControl.VisibleRowCount && SelectedItemChangedHandler != null) SelectedItemChangedHandler(gridControl.SelectedItems);
            }
        }
        
        

        private void OnDuplicate(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (DuplicateEventHandler != null) DuplicateEventHandler(GetSelectedOis());
        }

        private void OnDelete(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (DeleteEventHandler != null) DeleteEventHandler(GetSelectedOis());
        }
                        
        protected virtual void OnCellValueChanged(object sender, CellValueChangedEventArgs args)
        {
            GridItem item = (GridItem)this.gridControl.SelectedItem;
            if (item == null) item = (GridItem)this.gridControl.CurrentItem;
            if (item != null)
            {
                int? oid = item.GetOid();
                GridColumn col = args.Column;
                GrilleColumn column = this.Grille.GetColumn(col.FieldName);
                string oldValue = item.Datas[column.position] != null ? item.Datas[column.position].ToString() : "";
                string newValue = args.Value != null ? args.Value.ToString() : "";
                
                GrilleEditedElement element = new GrilleEditedElement();
                element.column = column;
                element.oid = oid;

                //if (newValue.Equals(oldValue)) return;
                if (column.type.Equals(ParameterType.MEASURE.ToString()))
                {
                    decimal val = 0;
                    if (string.IsNullOrWhiteSpace(newValue)) val = 0;
                    else if (decimal.TryParse(newValue, out val)) element.measure = val;
                    else if (decimal.TryParse(newValue.Replace(".", ","), out val)) element.measure = val;
                    else
                    {
                        MessageDisplayer.DisplayError("Wrong measure", "'" + newValue + "'" + " is not a decimal!");
                        args.Handled = true;
                        //args.Value = args.OldValue;
                        return;
                    }
                }
                else if (column.type.Equals(ParameterType.SCOPE.ToString()))
                {
                    if (string.IsNullOrWhiteSpace(newValue)) element.value = null;
                    else
                    {
                        BrowserData data = column.getValue(newValue);
                        if (!AllowUnknowValueForScope && data == null)
                        {
                            MessageDisplayer.DisplayError("Wrong value", "Unknow value : '" + newValue + "'");
                            args.Handled = true;                            
                            return;
                        }
                        else element.value = data;
                    }
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
                        args.Handled = true;
                        return;
                    }
                }

                if (this.EditEventHandler != null)
                {
                    Object[] row = EditEventHandler(element);
                    if (row == null) args.Handled = true;
                    else item.Datas = row;
                    Refresh();
                    //this.grid.SelectedItem = item;
                }
            }
        }

        protected virtual void OnValidateCell(object sender, GridCellValidationEventArgs e)
        {
            
        }





        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            if (e is GridEventArgs && this.Grille.GrilleFilter != null)
            {
                if (this.gridControl.IsFilterEnabled)
                {
                    if (this.gridControl.FilterCriteria != null)
                    {
                        this.Grille.GrilleFilter.filter = buildColumnFilters(this.gridControl.FilterCriteria);
                    }
                    else this.Grille.ClearColumnFilter();
                }
                else this.Grille.ClearColumnFilter();
                if (FilterEventHandler != null) FilterEventHandler();
            }
            e.Handled = true;
        }
        
        private GrilleColumnFilter buildColumnFilters(CriteriaOperator criteria)
        {
            GrilleColumnFilter filter = new GrilleColumnFilter();
            if (criteria != null)
            {
                String link = "And";
                if (criteria is FunctionOperator)
                {
                    FunctionOperator function = (FunctionOperator)criteria;
                    if (function.Operands.Count == 3)
                    {
                        String operation = function.Operands[0].ToString();                        
                        String name = ((OperandProperty)function.Operands[1]).PropertyName;
                        String value = ((OperandValue)function.Operands[2]).Value.ToString();
                        filter = buildColumnFilter(link, name, operation, value);
                    }
                    else if (function.Operands.Count == 2)
                    {
                        String operation = function.OperatorType.ToString();
                        String name = ((OperandProperty)function.Operands[0]).PropertyName;
                        String value = ((OperandValue)function.Operands[1]).Value.ToString();
                        filter = buildColumnFilter(link, name, operation, value);
                    }
                    else if (function.Operands.Count == 1)
                    {
                        String operation = function.OperatorType.ToString();
                        String name = ((OperandProperty)function.Operands[0]).PropertyName;
                        filter = buildColumnFilter(link, name, operation, null);
                    }
                }

                if (criteria is UnaryOperator)
                {
                    UnaryOperator function = (UnaryOperator)criteria;
                    String operation = function.OperatorType.ToString();
                    String name = null;
                    if (function.Operand is OperandProperty)
                    {
                        name = ((OperandProperty)function.Operand).PropertyName;
                        filter = buildColumnFilter(link, name, operation, null);
                    }
                    else if (function.Operand is FunctionOperator)
                    {
                        FunctionOperator functionOperator = (FunctionOperator)function.Operand;
                        String operation2 = functionOperator.OperatorType.ToString();
                        filter = buildColumnFilters(functionOperator);
                        filter.filterOperator = operation + filter.filterOperator;
                    }
                }

                if (criteria is BinaryOperator)
                {
                    BinaryOperator function = (BinaryOperator)criteria;
                    String operation = function.OperatorType.ToString();
                    String name = ((OperandProperty)function.LeftOperand).PropertyName;
                    String value = ((OperandValue)function.RightOperand).Value.ToString();
                    filter = buildColumnFilter(link, name, operation, value);                    
                }

                if (criteria is GroupOperator)
                {                    
                    GroupOperator function = (GroupOperator)criteria;
                    filter.isGroup = true;
                    filter.filterOperation = function.OperatorType.ToString();
                    foreach (CriteriaOperator op in function.Operands)
                    {
                        GrilleColumnFilter item = buildColumnFilters(op);
                        if (item != null) filter.items.Add(item);
                    }
                }
            }
            return filter;
        }

        private GrilleColumnFilter buildColumnFilter(String link, String name, String operation, String value)
        {
            GrilleColumn column = this.Grille.GetColumn(name);
            if (column != null)
            {
                GrilleColumnFilter filter = new GrilleColumnFilter();
                filter.column = column;
                filter.filterOperation = link;
                filter.filterOperator = operation;
                filter.filterValue = value;
                return filter;
            }
            return null;
        }

        protected void Refresh()
        {            
            List<object[]> rows = new List<object[]>(0);
            if (this.gridControl.ItemsSource != null)
            {
                foreach (object row in (List<GridItem>)this.gridControl.ItemsSource)
                {
                    if (row is GridItem)
                    {
                        GridItem item = (GridItem)row;
                        if (item.GetOid().HasValue) rows.Add(item.Datas);
                    }
                }
            }
            displayRows(rows);
        }

        public virtual void buildColumns(Grille grid)
        {
            this.Grille = grid;
            buildGrid();
            foreach (GrilleColumn column in grid.columnListChangeHandler.Items)
            {
                if (column.show) this.AddColumn(column);
            }
            this.Children.Add(this.gridControl);            
            //RemoveLastEmptyColumn();
            RebuildGrid = false;
        }

        public void displayPage(GrillePage page, bool add = false, String side = null)
        {
            if (page != null) displayRows(page.rows, add, side);
        }

        public void displayRows(List<object[]> rows, bool add = false, String side = null) 
        {
            List<GridItem> items = new List<GridItem>(0);
            List<int> positions = this.Grille.getPeriodColumnPositions();
            int count = positions.Count;
            foreach (object[] row in rows)
            {
                if(count > 0)buildDate(row, positions);
                items.Add(new GridItem(row, side));
            }
            if (!this.IsReadOnly && !this.Grille.IsReadOnly())
            {
                items.Add(new GridItem(new object[this.gridControl.Columns.Count], side));
            }

            if (!add || this.gridControl.ItemsSource == null)
            {
                this.gridControl.ItemsSource = items;
            }
            else
            {
                List<GridItem> source = new List<GridItem>((List<GridItem>)this.gridControl.ItemsSource);
                List<int?> oids = new List<int?>(0);
                foreach (GridItem elt in source)
                {
                    oids.Add(elt.GetOid());
                }

                foreach (GridItem elt in items)
                {
                    if (oids.Contains(elt.GetOid())) continue;
                    source.Add(elt);
                }
                this.gridControl.ItemsSource = source;
            }
            
            this.gridControl.View.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            this.gridControl.View.ShowEditor();
        }

        protected void buildDate(object[] row, List<int> positions){
            if(row != null && row.Length > 0){
                foreach (int position in positions) {
                    if (row.Length > position)
                    {
                        Object period = row[position];
                        if (period != null)
                        {
                            if (period is long) row[position] = new DateTime((long)period);
                            else if (period is string)
                            {
                                row[position] = DateTime.ParseExact((string)period, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
			    }
            }
		}

        public void displayItems(List<GridItem> items) 
        {
            this.gridControl.ItemsSource = items;
        }

        public void AddColumn(GrilleColumn grilleColumn, bool readOnly = false)
        {
            DevExpress.Xpf.Grid.GridColumn column = getColumn(grilleColumn, readOnly);
            gridControl.Columns.Add(column);
        }

        protected String getBindingName(GrilleColumn grilleColumn)
        {
            return "Datas[" + grilleColumn.position + "]";
        }

        protected virtual GridColumn getColumn(GrilleColumn grilleColumn, bool readOnly = false) 
        {
            DevExpress.Xpf.Grid.GridColumn column = new DevExpress.Xpf.Grid.GridColumn();
            column.FieldName = grilleColumn.name;
            column.IsSmart = true;
            column.ReadOnly = this.IsReadOnly || this.Grille.IsReadOnly() || readOnly;
            column.ColumnFilterMode = ColumnFilterMode.DisplayText;
            Binding b = new Binding(getBindingName(grilleColumn));
            b.Mode = BindingMode.TwoWay;
            column.Binding = b;

            setColumnEditSettings(column, grilleColumn, readOnly);
            
            if (grilleColumn.type.Equals(ParameterType.PERIOD.ToString()) 
                || grilleColumn.type.Equals(ParameterType.MEASURE.ToString())
                || grilleColumn.type.Equals(ParameterType.SPECIAL_MEASURE.ToString()))
            {
                column.ColumnFilterMode = ColumnFilterMode.Value;
            }
            
            return column;        
        }

        protected virtual void setColumnEditSettings(GridColumn column, GrilleColumn grilleColumn, bool readOnly = false)
        {
            if (grilleColumn.type.Equals(ParameterType.MEASURE.ToString()))
            {
                TextEditSettings settings = new TextEditSettings();
                settings.DisplayFormat = "N2";
                settings.ValidateOnTextInput = true;
                settings.AllowNullInput = true;
                column.EditSettings = settings;
            }

            if (!this.Grille.report && grilleColumn.type.Equals(ParameterType.SCOPE.ToString()))
            {
                try
                {
                    grilleColumn.values = Service.ModelService.getLeafAttributeValues(grilleColumn.valueOid.Value);
                }
                catch (Exception) { }
                ComboBoxEditSettings combo = new ComboBoxEditSettings();
                combo.ItemsSource = grilleColumn.Items;
                combo.IsTextEditable = true;
                combo.ShowText = true;
                combo.ValidateOnTextInput = true;
                combo.AllowNullInput = true;
                column.EditSettings = combo;
            }
            else if (!this.Grille.report && grilleColumn.type.Equals(ParameterType.PERIOD.ToString()))
            {
                DateEditSettings dateSetting = new DateEditSettings();
                dateSetting.IsTextEditable = true;
                dateSetting.ShowText = true;
                dateSetting.ValidateOnTextInput = true;
                dateSetting.AllowNullInput = true;
                column.EditSettings = dateSetting;
            }
        }

        public void RemoveColumn(String name, int position = -1)
        {
            for (int i = this.gridControl.Columns.Count - 1; i >= 0; i--)
            {
                GridColumn col = this.gridControl.Columns[i];
                if (!col.FieldName.Equals(name)) continue;
                this.gridControl.Columns.Remove(col);
                columnNames.Remove(name);
                break;
            }
        }

        public void RemoveLastEmptyColumn()
        {
            int LastColumn = this.gridControl.Columns.Count - 1;
            GridColumn col = this.gridControl.Columns[LastColumn];
            this.gridControl.Columns.Remove(col);            
        }
        
        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

    }
}
