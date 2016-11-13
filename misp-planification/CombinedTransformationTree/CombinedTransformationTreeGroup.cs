using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeGroup : SidebarGroup
    {

        #region Properties

        public CombinedTransformationTreeTreeview combinedTransformationTreeTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public CombinedTransformationTreeGroup() : base() { }

        public CombinedTransformationTreeGroup(string header) : base(header) { }

        public CombinedTransformationTreeGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.combinedTransformationTreeTreeview = new CombinedTransformationTreeTreeview();
            this.ContentPanel.Children.Add(this.combinedTransformationTreeTreeview);
        }

        #endregion

    }
}

