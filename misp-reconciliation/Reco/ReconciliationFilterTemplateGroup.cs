using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateGroup : SidebarGroup
    {
        
        #region Properties

        public ReconciliationFilterTemplateTreeView TemplateTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public ReconciliationFilterTemplateGroup() : base() { }

        public ReconciliationFilterTemplateGroup(string header) : base(header) { }

        public ReconciliationFilterTemplateGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.TemplateTreeview = new ReconciliationFilterTemplateTreeView();
            this.ContentPanel.Children.Add(this.TemplateTreeview);
        }

        #endregion

    }
}
