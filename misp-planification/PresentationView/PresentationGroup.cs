using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.PresentationView
{
    public class PresentationGroup : SideBarExpander
    {
         #region Properties

        public PresentationTreeView PresentationTreeView { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public PresentationGroup() : base() { }

        public PresentationGroup(string header) : base(header) { }

        public PresentationGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.PresentationTreeView = new PresentationTreeView();
            this.ContentPanel.Children.Add(this.PresentationTreeView);
        }

        #endregion

    }
}
