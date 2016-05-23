using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Administration.User
{
    public class UserPropertyBar : PropertyBar
    {
        public LayoutAnchorable UserLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.UserLayoutAnchorable = new LayoutAnchorable();
            this.UserLayoutAnchorable.Title = "Properties";
            this.UserLayoutAnchorable.CanClose = false;
            this.UserLayoutAnchorable.CanFloat = false;
            this.UserLayoutAnchorable.CanAutoHide = false;
            this.UserLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(UserLayoutAnchorable);
            this.Panes.Add(Pane);
        }

    }
}
