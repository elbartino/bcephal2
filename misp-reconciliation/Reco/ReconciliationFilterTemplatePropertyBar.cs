using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplatePropertyBar : PropertyBar
    {

        public LayoutAnchorable DesignLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        public LayoutAnchorable FilterLayoutAnchorable { get; set; }

        public LayoutAnchorable AdministratorLayoutAnchorable { get; set; }

        protected override void UserInitialisation()
        {
            this.DesignLayoutAnchorable = new LayoutAnchorable();
            this.DesignLayoutAnchorable.Title = "Grid Properties";
            this.DesignLayoutAnchorable.CanClose = false;
            this.DesignLayoutAnchorable.CanFloat = false;
            this.DesignLayoutAnchorable.CanAutoHide = false;
            this.DesignLayoutAnchorable.CanHide = false;

            this.FilterLayoutAnchorable = new LayoutAnchorable();
            this.FilterLayoutAnchorable.Title = "Filter";
            this.FilterLayoutAnchorable.CanClose = false;
            this.FilterLayoutAnchorable.CanFloat = false;
            this.FilterLayoutAnchorable.CanAutoHide = false;
            this.FilterLayoutAnchorable.CanHide = false;

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
            Pane.Children.Add(DesignLayoutAnchorable);
            Pane.Children.Add(FilterLayoutAnchorable);
            if (AdministratorLayoutAnchorable != null) Pane.Children.Add(AdministratorLayoutAnchorable);
            this.Panes.Add(Pane);

        }

    }
}
