using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Role
{
    public class RoleSideBar : BrowserSideBar
    {

        #region Properties

        
        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.RemoveGroup(this.GroupGroup);
        }

        #endregion

    }
}
