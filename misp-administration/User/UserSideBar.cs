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
        public EntityGroup EntityGroup { get; set; }
        public TargetGroup StandardTargetGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.StandardTargetGroup = new TargetGroup("Standards Target", true);
            this.EntityGroup = new EntityGroup("Entities", true);
            this.UserGroup = new UserGroup("Users", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);

            this.StandardTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.UserGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.StandardTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.UserGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.UserGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.PeriodNameGroup);
        }

        #endregion
    }
}
