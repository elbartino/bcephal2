using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingPropertyBar : PropertyBar
    {
        public LayoutAnchorable AutomaticSourcingLayoutAnchorable { get; set; }
        public LayoutAnchorable AutomaticTablePropertiesLayoutAnchorable { get; set; }
        
        public LayoutAnchorablePane Pane { get; set; }

        public static bool isAutomaticTarget { get; set; }

        protected override void UserInitialisation()
        {
            this.AutomaticSourcingLayoutAnchorable = new LayoutAnchorable();
            this.AutomaticSourcingLayoutAnchorable.Title = isAutomaticTarget ? "Automatic Target Properties" :  "Automatic Sourcing Properties";
            this.AutomaticSourcingLayoutAnchorable.CanClose = false;
            this.AutomaticSourcingLayoutAnchorable.CanFloat = false;
            this.AutomaticSourcingLayoutAnchorable.CanAutoHide = false;
            this.AutomaticSourcingLayoutAnchorable.CanHide = false;

            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(AutomaticSourcingLayoutAnchorable);
            if (!isAutomaticTarget)
            {
                this.AutomaticTablePropertiesLayoutAnchorable = new LayoutAnchorable();
                this.AutomaticTablePropertiesLayoutAnchorable.Title = "Table Properties";
                this.AutomaticTablePropertiesLayoutAnchorable.CanClose = false;
                this.AutomaticTablePropertiesLayoutAnchorable.CanFloat = false;
                this.AutomaticTablePropertiesLayoutAnchorable.CanAutoHide = false;
                this.AutomaticTablePropertiesLayoutAnchorable.CanHide = false;
                Pane.Children.Add(AutomaticTablePropertiesLayoutAnchorable);
            }
            this.Panes.Add(Pane);
            
        }
    }
}
