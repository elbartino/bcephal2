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

namespace Misp.Sourcing.GridViews
{
    public class GridBrowser : Grid
    {

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
                gridControl.FilterChanged -= OnFilterChanged;
                gridControl.SelectionChanged -= OnSelectionChanged;
                ((GridTableView)gridControl.View).CellValueChanged -= OnCellValueChanged;
                ((GridTableView)gridControl.View).SortEventHandler -= OnSort;
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
            view.AllowEditing = !Grille.report && !Grille.reconciliation;

            gridControl.FilterChanged += OnFilterChanged;
            gridControl.SelectionChanged += OnSelectionChanged;
            view.SortEventHandler += OnSort;
            view.CellValueChanged += OnCellValueChanged;
            
            view.Menu.DeleteItem.ItemClick += OnDelete;
            view.Menu.DuplicateItem.ItemClick += OnDuplicate;

            view.IsRowCellMenuEnabled = !Grille.IsReadOnly();
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
                        
        protected void OnCellValueChanged(object sender, CellValueChangedEventArgs args)
        {
            GridItem item = (GridItem)this.gridControl.SelectedItem;
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
                        MessageDisplayer.DisplayError("Wromg measure", "'" + newValue + "'" + " is not a decimal!");
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
                        if (data == null)
                        {
                            MessageDisplayer.DisplayError("Wromg value", "Unknow value : '" + newValue + "'");
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

        private void OnSort(object columnName)
        {
            if (this.Grille != null && columnName != null)
            {
                GrilleColumn column = this.Grille.GetColumn(columnName.ToString());
                if (SortEventHandler != null) SortEventHandler(column);
            }
        }
        
        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            if (e is DevExpress.Xpf.Grid.GridEventArgs) 
            {
                GridEventArgs filterArgs = (GridEventArgs)e;
                if (filterArgs.Source is GridControl)
                {
                    GridControl filterGrid = (GridControl)filterArgs.Source;
                    OperandValue[] operand = null;
                    CriteriaOperator criteria = null;
                    if (filterGrid.FilterCriteria != null)
                    {
                        Object[] criteriaElements = getOperatorType(filterGrid.FilterCriteria);
                        if (criteriaElements == null) return;
                        string propertyName = criteriaElements[0].ToString();
                        string value = criteriaElements[1].ToString();
                        string valueOperator = criteriaElements[2].ToString();
                      
                        GrilleColumn column = this.Grille.GetColumn(propertyName);
                        if (column != null)
                        {
                            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                            {
                                column.filterValue = null;
                                column.filterOperator = null;
                            }
                            else
                            {
                                column.filterValue = value.ToString();
                                column.filterOperator = valueOperator;
                            }
                            if (FilterEventHandler != null) FilterEventHandler();
                        }
                    }
                    else
                    {
                        this.Grille.ClearColumnFilter();
                        if (FilterEventHandler != null) FilterEventHandler();
                    }
                    
                }
             }
            e.Handled = true;
        }

        protected Object[] getOperatorType(CriteriaOperator criteriaOperator) 
        {
            CriteriaOperator criteria = null;
            OperandValue[] operand = null;
            
            if (criteria == null)
            {
                if (criteriaOperator is FunctionOperator)
                {
                    criteria = FunctionOperator.Parse(criteriaOperator.ToString(), out operand) as FunctionOperator;
                    if (criteria != null) return getOperationItems(criteria as FunctionOperator);
                }

                if (criteriaOperator is UnaryOperator)
                {
                    criteria = UnaryOperator.Parse(criteriaOperator.ToString(), out operand) as UnaryOperator;
                    if (criteria != null) return getOperationItems(criteria as UnaryOperator);
                }

                if (criteriaOperator is BinaryOperator)
                {
                    criteria = BinaryOperator.Parse(criteriaOperator.ToString(), out operand) as BinaryOperator;
                    if (criteria != null) return getOperationItems(criteria as BinaryOperator);
                }

                if (criteriaOperator is GroupOperator)
                {
                    criteria = GroupOperator.Parse(criteriaOperator.ToString(), out operand) as GroupOperator;
                    if (criteria != null) return getOperationItems(criteria as GroupOperator);
                }
            }
             return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A table that contains the column name, the filter value and the operator</returns>
        protected Object[] getOperationItems(CriteriaOperator criteria) 
        {

            if (criteria is FunctionOperator)
            {
                string colName = "";
                string value = "";
                string typeOperator = "";
                if (((FunctionOperator)criteria).Operands.Count == 3)
                {
                   ConstantValue  type = (ConstantValue)((FunctionOperator)criteria).Operands[0];
                   typeOperator = type.Value.ToString();
                   OperandProperty col = (OperandProperty)((FunctionOperator)criteria).Operands[1];
                   colName = col.PropertyName;
                   ConstantValue val = (ConstantValue)((FunctionOperator)criteria).Operands[2];
                   value = val.Value.ToString();
                }
                if (((FunctionOperator)criteria).Operands.Count == 2) 
                {
                    OperandProperty col = (OperandProperty)((FunctionOperator)criteria).Operands[0];
                    colName = col.PropertyName;
                    ConstantValue val = (ConstantValue)((FunctionOperator)criteria).Operands[1];
                    value = val.Value.ToString();
                    FunctionOperatorType type = ((FunctionOperator)criteria).OperatorType;
                    typeOperator = type.ToString();
                }

                return new Object[] { colName, value, typeOperator };
            }
            if (criteria is BinaryOperator) 
            {
                OperandProperty col = (OperandProperty)((BinaryOperator)criteria).LeftOperand;
                ConstantValue val = (ConstantValue)((BinaryOperator)criteria).RightOperand;
                BinaryOperatorType type = ((BinaryOperator)criteria).OperatorType;
                return new Object[] { col.PropertyName, val.Value, type.ToString() };
            }

            if (criteria is UnaryOperator) 
            {
                Object[] firstOperand = getOperationItems(((UnaryOperator)criteria).Operand);
                if (firstOperand == null) return null;
                UnaryOperatorType type = ((UnaryOperator)criteria).OperatorType;
                string operatorType = type.ToString() + firstOperand[2].ToString();
                return new Object[] { firstOperand[0],firstOperand[1],operatorType};
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

        public void buildColumns(Grille grid)
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

        public void displayPage(GrillePage page)
        {
            if(page !=null) displayRows(page.rows);
        }

        public void displayRows(List<object[]> rows) 
        {
            List<GridItem> items = new List<GridItem>(0);
            List<int> positions = this.Grille.getPeriodColumnPositions();
            int count = positions.Count;
            foreach (object[] row in rows)
            {
                if(count > 0)buildDate(row, positions);
                items.Add(new GridItem(row));
            }
            if (!this.Grille.IsReadOnly())
            {
                items.Add(new GridItem(new object[this.gridControl.Columns.Count]));
            }
           
            this.gridControl.ItemsSource = items;
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

        public void AddColumn(GrilleColumn grilleColumn)
        {
            DevExpress.Xpf.Grid.GridColumn column = getColumn(grilleColumn);
            gridControl.Columns.Add(column);
        }

        private String getBindingName(GrilleColumn grilleColumn)
        {
            return "Datas[" + grilleColumn.position + "]";
        }

        private GridColumn getColumn(GrilleColumn grilleColumn) 
        {
            DevExpress.Xpf.Grid.GridColumn column = new DevExpress.Xpf.Grid.GridColumn();
            column.FieldName = grilleColumn.name;
            column.IsSmart = true;
            column.ReadOnly = this.Grille.IsReadOnly();
            column.ColumnFilterMode = ColumnFilterMode.DisplayText;
            Binding b = new Binding(getBindingName(grilleColumn));
            b.Mode = BindingMode.TwoWay;
            column.Binding = b;
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
            if (!this.Grille.report && grilleColumn.type.Equals(ParameterType.PERIOD.ToString()))
            {
                DateEditSettings dateSetting = new DateEditSettings();
                dateSetting.IsTextEditable = true;
                dateSetting.ShowText = true;
                dateSetting.ValidateOnTextInput = true;
                dateSetting.AllowNullInput = true;
                column.EditSettings = dateSetting;
            }
            if (grilleColumn.type.Equals(ParameterType.PERIOD.ToString()) 
                || grilleColumn.type.Equals(ParameterType.MEASURE.ToString())
                || grilleColumn.type.Equals(ParameterType.SPECIAL_MEASURE.ToString()))
            {
                column.ColumnFilterMode = ColumnFilterMode.Value;
            }

            return column;        
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


        
    }
}
