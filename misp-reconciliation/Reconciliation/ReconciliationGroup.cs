using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reconciliation
{
    public class ReconciliationGroup : SideBarExpander
    {

        public ReconciliationTreeview ReconciliationTreeview { get; set; }

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public ReconciliationGroup() : base() { }

        public ReconciliationGroup(string header) : base(header) { }

        public ReconciliationGroup(string header, bool expanded) : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.ReconciliationTreeview = new ReconciliationTreeview();
            this.ContentPanel.Children.Add(this.ReconciliationTreeview);
        }

        #endregion
        
    }
}
