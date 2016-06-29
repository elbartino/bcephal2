using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Administration.Role
{
    public class RolePropertyBar : PropertyBar
    {
         public LayoutAnchorable TableLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.TableLayoutAnchorable = new LayoutAnchorable();
            this.TableLayoutAnchorable.Title = "Properties";
            this.TableLayoutAnchorable.CanClose = false;
            this.TableLayoutAnchorable.CanFloat = false;
            this.TableLayoutAnchorable.CanAutoHide = false;
            this.TableLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(TableLayoutAnchorable);
            this.Panes.Add(Pane);
        }
    }
}
