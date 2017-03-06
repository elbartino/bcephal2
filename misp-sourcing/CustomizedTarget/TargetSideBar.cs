using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.CustomizedTarget
{
    public class TargetSideBar : SideBar
    {

        
        #region Properties

        public CustomizedTargetGroup TargetGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }
        public TargetGroup StandardTargetGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.StandardTargetGroup = new TargetGroup("Standards Target", true);
            this.EntityGroup = new ModelSidebarGroup("Entities", true);
            this.TargetGroup = new CustomizedTargetGroup("Targets", true);

            this.StandardTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.StandardTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.EntityGroup);
            //this.AddGroup(this.StandardTargetGroup);
        }


        public override void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            this.EntityGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        #endregion

    }
}
