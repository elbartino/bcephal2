using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Designer;
using Misp.Sourcing.CustomizedTarget;
using Misp.Kernel.Ui.TreeView;

namespace Misp.Sourcing.AllocationViews
{
    public class AllocationBoxSideBar : SideBar
    {


        #region Constructor


        #endregion


        #region Properties

        public EntityGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CalculateMeasureGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public TreeLoopGroup TreeLoopGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            this.TreeLoopGroup = new TreeLoopGroup("Loops", true);
            this.EntityGroup = new EntityGroup("Entities", true);
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureGroup("Measure", true);
            this.CalculateMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Target", true);
            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.AddGroup(this.TreeLoopGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.CalculateMeasureGroup);
            this.AddGroup(this.PeriodNameGroup);
        }

        #endregion
        

    }
}
