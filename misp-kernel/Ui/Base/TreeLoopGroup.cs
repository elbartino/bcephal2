using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public class TreeLoopGroup : SidebarGroup
    {
        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public TreeLoopGroup() : base() { }

        public TreeLoopGroup(string header) : base(header) { }

        public TreeLoopGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.TransformationTreeLoopTreeview = new TransformationTreeLoopTreeview();
            this.ContentPanel.Children.Add(this.TransformationTreeLoopTreeview);
        }

        #endregion

        #region properties

        public TransformationTreeLoopTreeview TransformationTreeLoopTreeview { get; set; }

        #endregion

    }
}
