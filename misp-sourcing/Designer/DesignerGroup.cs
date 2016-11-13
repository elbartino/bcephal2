using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Designer
{
    public class DesignerGroup : SidebarGroup
    {

        #region Properties

        public DesignerTreeview DesignerTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public DesignerGroup() : base() { }

        public DesignerGroup(string header) : base(header) { }

        public DesignerGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.DesignerTreeview = new DesignerTreeview();
            this.ContentPanel.Children.Add(this.DesignerTreeview);
        }

        #endregion

    }
}
