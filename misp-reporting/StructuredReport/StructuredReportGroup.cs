using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportGroup : SidebarGroup
    {

        #region Properties

        public StructuredReportTreeview StructuredReportTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public StructuredReportGroup() : base() { }

        public StructuredReportGroup(string header) : base(header) { }

        public StructuredReportGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.StructuredReportTreeview = new StructuredReportTreeview();
            this.ContentPanel.Children.Add(this.StructuredReportTreeview);
        }

        #endregion

    }
}
