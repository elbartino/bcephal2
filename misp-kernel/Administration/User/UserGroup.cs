using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.User
{
    public class UserGroup : SideBarExpander
    {
        public UserTreeview UserTreeview { get; set; }

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public UserGroup() : base() { }

        public UserGroup(string header) : base(header) { }

        public UserGroup(string header, bool expanded) : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.UserTreeview = new UserTreeview();
            this.ContentPanel.Children.Add(this.UserTreeview);
        }

        #endregion
        
    }
}
