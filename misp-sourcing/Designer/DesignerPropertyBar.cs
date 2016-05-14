﻿using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;

namespace Misp.Sourcing.Designer
{
    public class DesignerPropertyBar : PropertyBar
    {

        public LayoutAnchorable DesignLayoutAnchorable { get; set; }
        
        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.DesignLayoutAnchorable = new LayoutAnchorable();
            this.DesignLayoutAnchorable.Title = "Design Properties";
            this.DesignLayoutAnchorable.CanClose = false;
            this.DesignLayoutAnchorable.CanFloat = false;
            this.DesignLayoutAnchorable.CanAutoHide = false;
            this.DesignLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(DesignLayoutAnchorable);
            this.Panes.Add(Pane);

        }

    }
}
