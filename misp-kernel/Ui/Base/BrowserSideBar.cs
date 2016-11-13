using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public class BrowserSideBar : SideBar
    {
        
        #region Constructor


        #endregion


        #region Properties

        public GroupGroup GroupGroup { get; set; }

        public GroupCatagoryGroup GroupCatagoryGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.GroupGroup = new GroupGroup("Groups", true);
            this.GroupGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.GroupGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.AddGroup(this.GroupGroup);

            this.GroupCatagoryGroup = new GroupCatagoryGroup("Categories", true);
            this.GroupCatagoryGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.GroupCatagoryGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;            
        }


        #endregion

    }
}
