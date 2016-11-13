using Misp.Kernel.Administration.Profil;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.User
{
    public class UserBrowserSideBar : SideBar
    {

        #region Constructor


        #endregion


        #region Properties

        public ProfilGroup ProfilGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.ProfilGroup = new ProfilGroup("Profils", true);
            this.ProfilGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.ProfilGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.AddGroup(this.ProfilGroup);
        }


        #endregion

    }
}
