using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.CustomizedTarget
{
    public class CustomizedTargetGroup : SideBarExpander
    {

        #region Properties

        public CustomizedTargetTreeview TargetTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public CustomizedTargetGroup() : base() { }

        public CustomizedTargetGroup(string header) : base(header) { }

        public CustomizedTargetGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.TargetTreeview = new CustomizedTargetTreeview();
            this.ContentPanel.Children.Add(this.TargetTreeview);
        }

        #endregion

    }
}

