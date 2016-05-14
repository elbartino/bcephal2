using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public class PeriodNameGroup : SideBarExpander
    {

        #region Properties

        public PeriodNameTreeview PeriodNameTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public PeriodNameGroup() : base() { }

        public PeriodNameGroup(string header) : base(header) { }

        public PeriodNameGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.PeriodNameTreeview = new PeriodNameTreeview();
            this.ContentPanel.Children.Add(this.PeriodNameTreeview);
        }

        #endregion

    }
}
