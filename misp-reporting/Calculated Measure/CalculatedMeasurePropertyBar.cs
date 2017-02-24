using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reporting.CalculatedMeasures
{
    public class CalculatedMeasurePropertyBar : PropertyBar
    {
         public LayoutAnchorable TableLayoutAnchorable { get; set; }

         public LayoutAnchorable AdministratorLayoutAnchorable { get; set; }

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
            if (ApplicationManager.Instance.User.IsAdmin())
            {
                this.AdministratorLayoutAnchorable = new LayoutAnchorable();
                this.AdministratorLayoutAnchorable.Title = "Administration";
                this.AdministratorLayoutAnchorable.CanAutoHide = false;
                this.AdministratorLayoutAnchorable.CanClose = false;
                this.AdministratorLayoutAnchorable.CanFloat = false;
                this.AdministratorLayoutAnchorable.CanHide = false;
            }

            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(TableLayoutAnchorable);
            if (AdministratorLayoutAnchorable != null) Pane.Children.Add(AdministratorLayoutAnchorable);
            this.Panes.Add(Pane);
        }
    }
}
