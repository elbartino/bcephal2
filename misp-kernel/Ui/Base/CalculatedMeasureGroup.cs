using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Ui.Base
{
    public class CalculatedMeasureGroup : SidebarGroup
    {
         #region Properties

        public CalculatedMeasureTreeview CalculatedMeasureTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public CalculatedMeasureGroup() : base() { }

        public CalculatedMeasureGroup(string header) : base(header) { }

        public CalculatedMeasureGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.CalculatedMeasureTreeview = new CalculatedMeasureTreeview();
            this.ContentPanel.Children.Add(this.CalculatedMeasureTreeview);
        }

        #endregion

    }
}
