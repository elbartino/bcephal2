using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeGroup : SideBarExpander
    {

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public TransformationTreeGroup() : base() { }

        public TransformationTreeGroup(string header) : base(header) { }

        public TransformationTreeGroup(string header, bool expanded) : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.TransformationTreeTreeview = new TransformationTreeTreeview();
            this.ContentPanel.Children.Add(this.TransformationTreeTreeview);
        }

        #endregion
        
        public TransformationTreeTreeview TransformationTreeTreeview { get; set; }
    }
}
