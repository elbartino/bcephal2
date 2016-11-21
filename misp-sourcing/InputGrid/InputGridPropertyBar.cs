using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridPropertyBar : PropertyBar
    {

        public LayoutAnchorable DesignLayoutAnchorable { get; set; }
        public LayoutAnchorable RelationshipLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.DesignLayoutAnchorable = new LayoutAnchorable();
            this.DesignLayoutAnchorable.Title = "Grid Properties";
            this.DesignLayoutAnchorable.CanClose = false;
            this.DesignLayoutAnchorable.CanFloat = false;
            this.DesignLayoutAnchorable.CanAutoHide = false;
            this.DesignLayoutAnchorable.CanHide = false;

            this.RelationshipLayoutAnchorable = new LayoutAnchorable();
            this.RelationshipLayoutAnchorable.Title = "Relationships";
            this.RelationshipLayoutAnchorable.CanClose = false;
            this.RelationshipLayoutAnchorable.CanFloat = false;
            this.RelationshipLayoutAnchorable.CanAutoHide = false;
            this.RelationshipLayoutAnchorable.CanHide = false;

            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(DesignLayoutAnchorable);
            Pane.Children.Add(RelationshipLayoutAnchorable);
            this.Panes.Add(Pane);

        }

    }
}
