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
                this.AddColumn(column, true);
                foreach (Kernel.Domain.Attribute attribute in grid.attribute.childrenListChangeHandler.Items)
                {
                    column = new GrilleColumn(attribute, position++);
                    grid.AddColumn(column);
                    this.AddColumn(column);
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
            
            return column;
        }

        protected override void OnCellValueChanged(object sender, CellValueChangedEventArgs args)
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

                bool isKey = column.position == 0;

                if (isKey && string.IsNullOrWhiteSpace(newValue)) {
                    MessageDisplayer.DisplayError("Wromg value", column.name + " can't be empty!");
                    args.Handled = true;
                    //args.Value = oldValue;
                    return;
                }
                else
                {
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
        }

    }
}
