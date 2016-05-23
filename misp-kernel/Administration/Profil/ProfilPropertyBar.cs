using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilPropertyBar : PropertyBar
    {
        public LayoutAnchorable ProfilLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation()
        {
            this.ProfilLayoutAnchorable = new LayoutAnchorable();
            this.ProfilLayoutAnchorable.Title = "Properties";
            this.ProfilLayoutAnchorable.CanClose = false;
            this.ProfilLayoutAnchorable.CanFloat = false;
            this.ProfilLayoutAnchorable.CanAutoHide = false;
            this.ProfilLayoutAnchorable.CanHide = false;
            Pane = new LayoutAnchorablePane();
            Pane.Children.Add(ProfilLayoutAnchorable);
            this.Panes.Add(Pane);
        }

    }
}
