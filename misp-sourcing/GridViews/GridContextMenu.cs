using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class GridContextMenu
    {
        public BarButtonItem DuplicateItem;
        public BarButtonItem DeleteItem;

        public GridContextMenu(GridViewBase view)
        {
            DuplicateItem = new BarButtonItem();
            //DuplicateItem.
            DuplicateItem.Content = "Duplicate";
            DeleteItem = new BarButtonItem();
            DeleteItem.Content = "Delete";
            view.RowCellMenuCustomizations.Add(DuplicateItem);
            view.RowCellMenuCustomizations.Add(DeleteItem);
        }

    }
}
