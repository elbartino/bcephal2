using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class GrilleGroup : SideBarExpander
    {

        #region Properties

        public GrilleTreeview GrilleTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public GrilleGroup() : base() { }

        public GrilleGroup(string header) : base(header) { }

        public GrilleGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.GrilleTreeview = new GrilleTreeview();
            this.ContentPanel.Children.Add(this.GrilleTreeview);
        }

        #endregion

    }
}
