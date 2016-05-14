using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Sourcing.Table
{
    public class InputTableGroup : SideBarExpander
    {

        #region Properties

        public InputTableTreeview InputTableTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public InputTableGroup() : base() { }

        public InputTableGroup(string header) : base(header) { }

        public InputTableGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.InputTableTreeview = new InputTableTreeview();
            this.ContentPanel.Children.Add(this.InputTableTreeview);
        }

        #endregion

    }
}
