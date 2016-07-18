using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingGroup : SideBarExpander
    {

        #region Properties

        public AutomaticSourcingTreeview AutomaticSourcingTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public AutomaticSourcingGroup() : base() { }

        public AutomaticSourcingGroup(string header) : base(header) { }

        public AutomaticSourcingGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.AutomaticSourcingTreeview = new AutomaticSourcingTreeview();
            this.ContentPanel.Children.Add(this.AutomaticSourcingTreeview);
        }


        #endregion

    }
}
