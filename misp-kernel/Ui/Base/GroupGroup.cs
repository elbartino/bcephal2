using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Kernel.Ui.Base
{
    public class GroupGroup : SideBarExpander
    {

        #region Properties

        public GroupTreeview GroupTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public GroupGroup() : base() { }

        public GroupGroup(string header) : base(header) { }

        public GroupGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.GroupTreeview = new GroupTreeview();
            this.ContentPanel.Children.Add(this.GroupTreeview);
        }

        #endregion

    }
}
