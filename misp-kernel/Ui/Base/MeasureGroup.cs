using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Kernel.Ui.Base
{
    public class MeasureGroup : SideBarExpander
    {

        #region Properties

        public MeasureTreeview MeasureTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public MeasureGroup() : base() { }

        public MeasureGroup(string header) : base(header) { }

        public MeasureGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.MeasureTreeview = new MeasureTreeview();
            this.ContentPanel.Children.Add(this.MeasureTreeview);
        }

        #endregion

    }
}
