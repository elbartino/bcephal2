using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.User
{
    public class UserSideBar : SideBar
    {
        #region Properties

        public UserGroup UserGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.UserGroup = new UserGroup("Users", true);

            this.UserGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.UserGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.UserGroup);
        }

        #endregion
    }
}
