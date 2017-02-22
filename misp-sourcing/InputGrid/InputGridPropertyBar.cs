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

        public LayoutAnchorablePane Pane { get; set; }

        public LayoutAnchorable UserRightLayoutAnchorable { get; set; }

        protected override void UserInitialisation()
        {
            this.DesignLayoutAnchorable = new LayoutAnchorable();
            this.DesignLayoutAnchorable.Title = "Grid Properties";
            this.DesignLayoutAnchorable.CanClose = false;
            this.DesignLayoutAnchorable.CanFloat = false;
            this.DesignLayoutAnchorable.CanAutoHide = false;
            this.DesignLayoutAnchorable.CanHide = false;

            this.UserRightLayoutAnchorable = new LayoutAnchorable();
            this.UserRightLayoutAnchorable.Title = "Administration";
            this.UserRightLayoutAnchorable.CanAutoHide = false;
            this.UserRightLayoutAnchorable.CanClose = false;
            this.UserRightLayoutAnchorable.CanFloat = false;
            this.UserRightLayoutAnchorable.CanHide = false;
            
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(DesignLayoutAnchorable);
            Pane.Children.Add(UserRightLayoutAnchorable);
            this.Panes.Add(Pane);

        }

    }
}
