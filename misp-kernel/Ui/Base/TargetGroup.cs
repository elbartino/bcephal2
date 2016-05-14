using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public class TargetGroup : SideBarExpander
    {

        #region Properties

        public TargetTreeview TargetTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public TargetGroup() : base() { }

        public TargetGroup(string header) : base(header) { }

        public TargetGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.TargetTreeview = new TargetTreeview();
            this.ContentPanel.Children.Add(this.TargetTreeview);
        }

        #endregion

    }

}
