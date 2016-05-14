using Misp.Kernel.Ui.Base;
using Misp.Sourcing.CustomizedTarget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Designer
{
    public class DesignerSideBar : SideBar
    {


        #region Constructor


        #endregion


        #region Properties

        public DesignerGroup DesignerGroup { get; set; }
        public EntityGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CalculateMeasureGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public PeriodicityGroup PeriodicityGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.DesignerGroup = new DesignerGroup("Designs", true);
            this.EntityGroup = new EntityGroup("Entities", true);
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureGroup("Measure", true);
            this.CalculateMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);
            //this.PeriodicityGroup = new PeriodicityGroup("Periods", true);
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Targets", true);

            this.DesignerGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
           // this.PeriodicityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.DesignerGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
           // this.PeriodicityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;


            this.AddGroup(this.DesignerGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.CalculateMeasureGroup);
            this.AddGroup(this.PeriodNameGroup);
            //this.AddGroup(this.TagGroup);

        }


        #endregion

    }
}
