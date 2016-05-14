using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Kernel.Ui.Base
{
    public class EntityGroup : SideBarExpander
    {

        #region Properties

        public EntityTreeview EntityTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public EntityGroup() : base() { }

        public EntityGroup(string header) : base(header) { }

        public EntityGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.EntityTreeview = new EntityTreeview();
            this.ContentPanel.Children.Add(this.EntityTreeview);
        }

        #endregion

    }
}
