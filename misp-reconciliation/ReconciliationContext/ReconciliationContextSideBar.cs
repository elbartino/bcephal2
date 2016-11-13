using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextSideBar : SideBar
    {

        #region Properties

        public ModelSidebarGroup EntityGroup { get; set; }
        public TargetGroup StandardTargetGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.StandardTargetGroup = new TargetGroup("Standards Target", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.MeasureGroup = new MeasureSidebarGroup();

            this.StandardTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;            
            this.StandardTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.MeasureGroup);
        }

        #endregion

    }
}
