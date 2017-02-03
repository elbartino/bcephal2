using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Kernel.Ui.Base
{
    public class PropertyBar : LayoutAnchorablePaneGroup
    {

        public List<ILayoutAnchorablePane> Panes { get; set; }

        public PropertyBar()
        {
            Panes = new List<ILayoutAnchorablePane>(0);
            InitializeComponent();
            UserInitialisation();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            
        }

        protected void InitializeComponent()
        {
            this.DockWidth = new GridLength(200);
            this.DockMinWidth = 200; 
            this.Orientation = Orientation.Vertical; 
        }

        protected virtual void UserInitialisation() { }

    }
}
