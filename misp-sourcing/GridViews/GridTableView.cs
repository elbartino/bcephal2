using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class GridTableView : TableView
    {
        public ChangeItemEventHandler SortEventHandler { get; set; }

        GridControl grid;
        public GridContextMenu Menu;

        public GridTableView(GridControl grid)
        {
            this.grid = grid;
            this.ShowGroupPanel = true;
            this.ShowAutoFilterRow = true;
            this.ShowCriteriaInAutoFilterRow = true;
           // this.AutoWidth = true;
            this.IsEnabled = true;
            this.AllowEditing = true;
            this.UseIndicatorForSelection = true;
            this.ShowCheckBoxSelectorColumn = true;
            //this.ShowCheckBoxSelectorInGroupRow = true;
            this.NavigationStyle = GridViewNavigationStyle.Cell;
            //this.OptionsClipboard.
            this.BestFitColumns();
                     
            this.Menu = new GridContextMenu(this);
        }

        protected override void OnColumnHeaderClick(ColumnBase column, bool isShift, bool isCtrl)
        {
            if (column != null && SortEventHandler != null) SortEventHandler(column.FieldName);
        }

    }
}
