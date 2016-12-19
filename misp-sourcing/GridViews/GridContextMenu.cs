using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Misp.Sourcing.GridViews
{
    public class GridContextMenu
    {
        public BarButtonItem DuplicateItem;
        public BarButtonItem DeleteItem;

        public GridContextMenu(GridViewBase view)
        {
            DuplicateItem = new BarButtonItem();
            DuplicateItem.Content = "Duplicate";
            DuplicateItem.Glyph = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative));
            DeleteItem = new BarButtonItem();
            DeleteItem.Content = "Delete";
            DeleteItem.Glyph = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative));
            view.RowCellMenuCustomizations.Add(DuplicateItem);
            view.RowCellMenuCustomizations.Add(DeleteItem);
        }

    }
}
