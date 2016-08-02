using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextPropertyBar : PropertyBar
    {

        public LayoutAnchorable ReconciliationContextLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.ReconciliationContextLayoutAnchorable = new LayoutAnchorable();
            this.ReconciliationContextLayoutAnchorable.Title = "Properties";
            this.ReconciliationContextLayoutAnchorable.CanClose = false;
            this.ReconciliationContextLayoutAnchorable.CanFloat = false;
            this.ReconciliationContextLayoutAnchorable.CanAutoHide = false;
            this.ReconciliationContextLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(ReconciliationContextLayoutAnchorable);
            this.Panes.Add(Pane);
        }

    }
}
