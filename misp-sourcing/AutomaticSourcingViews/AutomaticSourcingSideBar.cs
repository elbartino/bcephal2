using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Sourcing.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingSideBar : SideBar
    {
        #region Constructor


        #endregion


        #region Properties

        public AutomaticSourcingGroup AutomaticSourcingGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CaculatedMeasureGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }
        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.AutomaticSourcingGroup = new AutomaticSourcingGroup("Automatic Sourcing", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.MeasureGroup = new MeasureSidebarGroup();
            this.CaculatedMeasureGroup = new CalculatedMeasureGroup("Calculate Measure", true);
            this.PeriodGroup = new PeriodSidebarGroup();

            this.AutomaticSourcingGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CaculatedMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
           
            this.AutomaticSourcingGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CaculatedMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
          
            this.AddGroup(this.AutomaticSourcingGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.CaculatedMeasureGroup);
            this.AddGroup(this.PeriodGroup);
           
        }


        #endregion

    }
}
