using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reconciliation.Reconciliation
{
    public class ReconciliationPropertyBar : PropertyBar
    {

        public LayoutAnchorable ReconciliationLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.ReconciliationLayoutAnchorable = new LayoutAnchorable();
            this.ReconciliationLayoutAnchorable.Title = "Properties";
            this.ReconciliationLayoutAnchorable.CanClose = false;
            this.ReconciliationLayoutAnchorable.CanFloat = false;
            this.ReconciliationLayoutAnchorable.CanAutoHide = false;
            this.ReconciliationLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(ReconciliationLayoutAnchorable);
            this.Panes.Add(Pane);
        }

    }
}
