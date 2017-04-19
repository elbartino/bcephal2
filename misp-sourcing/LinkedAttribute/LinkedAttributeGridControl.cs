using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Misp.Sourcing.LinkedAttribute
{
    public class LinkedAttributeGridControl : GridBrowser
    {

        public LinkedAttributeGridControl()
        {
            this.AllowUnknowValueForScope = true;
        }
        

        public override void buildColumns(Grille grille)
        {
            if (grille is LinkedAttributeGrid)
            {
                LinkedAttributeGrid grid = (LinkedAttributeGrid)grille;
                this.Grille = grid;
                buildGrid();
                int position = 0;
                GrilleColumn column = new GrilleColumn(grid.attribute, position++);
                grid.AddColumn(column);
                this.AddColumn(column, grid.attribute.incremental);
                foreach (Kernel.Domain.Attribute attribute in grid.attribute.childrenListChangeHandler.Items)
                {
                    column = new GrilleColumn(attribute, position++);
                    grid.AddColumn(column);
                    this.AddColumn(column, attribute.incremental);
                }
                this.Children.Add(this.gridControl);
                RebuildGrid = false;
            }
        }

        protected override GridColumn getColumn(GrilleColumn grilleColumn, bool readOnly = false)
        {
            DevExpress.Xpf.Grid.GridColumn column = new DevExpress.Xpf.Grid.GridColumn();
            column.FieldName = grilleColumn.name;
            column.IsSmart = true;
            column.ReadOnly = this.IsReadOnly || this.Grille.IsReadOnly() || readOnly;
            column.ColumnFilterMode = ColumnFilterMode.DisplayText;
            Binding b = new Binding(getBindingName(grilleColumn));
            b.Mode = BindingMode.TwoWay;
            column.Binding = b;

            column.Style = this.gridControl.FindResource("GridColumn") as Style;
            column.Width = new GridColumnWidth(1, GridColumnUnitType.Star);

            setColumnEditSettings(column, grilleColumn, readOnly);

            return column;
        }

        protected override void setColumnEditSettings(GridColumn column, GrilleColumn grilleColumn, bool readOnly = false)
        {
            bool isIncremental = grilleColumn.attribute.incremental;
            bool isKey = ((LinkedAttributeGrid)Grille).attribute.oid == grilleColumn.attribute.oid;
            if (!isIncremental)
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

                combo.AllowNullInput = !isKey; 
                combo.AutoComplete = true;
                combo.IncrementalFiltering = true;
                combo.ImmediatePopup = true;

                column.AllowIncrementalSearch = true;
                column.EditSettings = combo;
            }
        }


        protected override void OnCellValueChanged(object sender, CellValueChangedEventArgs args)
        {
            GridItem item = (GridItem)this.gridControl.SelectedItem;
            if (item == null) item = (GridItem)this.gridControl.CurrentItem;
            if (item != null)
            {
                GridColumn col = args.Column;
                GrilleColumn column = this.Grille.GetColumn(col.FieldName);
                string newValue = args.Value != null ? args.Value.ToString() : "";

                GrilleEditedElement element = new GrilleEditedElement();
                element.column = column;
                element.oid = item.GetOid();
                element.value = new Kernel.Domain.Browser.BrowserData();
                element.value.name = newValue;
                if (this.EditEventHandler != null)
                {
                    Object[] row = EditEventHandler(element);
                    if (row == null) args.Handled = true;
                    else item.Datas = row;
                    Refresh();
                }
            }
        }

        protected override void OnValidateCell(object sender, GridCellValidationEventArgs args)
        {
            GridItem item = (GridItem)this.gridControl.SelectedItem;
            if (item == null) item = (GridItem)this.gridControl.CurrentItem;
            if (item != null)
            {
                ColumnBase col = args.Column;
                GrilleColumn column = this.Grille.GetColumn(col.FieldName);
                string oldValue = item.Datas[column.position] != null ? item.Datas[column.position].ToString() : "";
                string newValue = args.Value != null ? args.Value.ToString() : "";

                if (!IsEditionValid(item, column, oldValue, newValue, args))
                {                    
                    args.Handled = true;
                    args.IsValid = false;
                    return;
                }
            }
        }

        protected bool IsEditionValid(GridItem item, GrilleColumn column, string oldValue, string newValue, GridCellValidationEventArgs args)
        {
            int? oid = item.GetOid();
            bool isKey = column.position == 0;
            if (isKey)
            {
                if (string.IsNullOrWhiteSpace(newValue))
                {
                    args.ErrorContent = column.name + " can't be empty!";
                    args.IsValid = false;
                    return false;
                }
            }

            else if (!oid.HasValue)
            {
                
                LinkedAttributeGrid grid = (LinkedAttributeGrid)this.Grille;
                Object keyValue = item.Datas[0];
                bool emptyKeyValue = keyValue == null || string.IsNullOrWhiteSpace(keyValue.ToString());
                if (emptyKeyValue && !grid.attribute.incremental)
                {
                    args.ErrorContent = "You have to set " + grid.attribute.name + " before set " + column.name;
                    args.IsValid = false;
                    return false;
                }
            }
            return true;
        }



    }
}
