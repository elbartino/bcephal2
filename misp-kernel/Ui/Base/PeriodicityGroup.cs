using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Kernel.Ui.Base
{
    public class PeriodicityGroup : SideBarExpander
    {

        #region Properties

        public PeriodicityTreeview PeriodicityTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public PeriodicityGroup() : base() { }

        public PeriodicityGroup(string header) : base(header) { }

        public PeriodicityGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.PeriodicityTreeview = new PeriodicityTreeview();
            this.ContentPanel.Children.Add(this.PeriodicityTreeview);
        }

        #endregion

    }
}
