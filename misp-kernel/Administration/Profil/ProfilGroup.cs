using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilGroup : SidebarGroup
    {
        public ProfilTreeview profilTreeview { get; set; }

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public ProfilGroup() : base() { }

        public ProfilGroup(string header) : base(header) { }

        public ProfilGroup(string header, bool expanded) : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.profilTreeview = new ProfilTreeview();
            this.ContentPanel.Children.Add(this.profilTreeview);
        }

        #endregion
        
    }
}
