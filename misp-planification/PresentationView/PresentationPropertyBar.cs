using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Planification.PresentationView
{
    public class PresentationPropertyBar : PropertyBar
    {
        public LayoutAnchorable PresentationLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.PresentationLayoutAnchorable = new LayoutAnchorable();

            this.PresentationLayoutAnchorable.Title = "Slide Properties";

            this.PresentationLayoutAnchorable.CanClose = false;
            this.PresentationLayoutAnchorable.CanAutoHide = false;
            this.PresentationLayoutAnchorable.CanFloat = false;
            this.PresentationLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            
            Pane.Children.Add(PresentationLayoutAnchorable);
            
            this.Panes.Add(Pane);

        }
    }
}
