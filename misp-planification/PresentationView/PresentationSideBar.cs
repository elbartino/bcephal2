using Misp.Kernel.Ui.Base;
using Misp.Reporting.StructuredReport;
using Misp.Sourcing.CustomizedTarget;
using Misp.Sourcing.Designer;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.PresentationView
{
    public class PresentationSideBar : SideBar
    {
        #region Constructor


        #endregion


        #region Properties

        public PresentationGroup PresentationGroup { get; set; }
        public SpecialGroup SpecialGroup { get; set; }
        public MeasureGroup MeasureGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }
        public DesignerGroup DesignerGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public TreeLoopGroup TreeLoopGroup { get; set; }

        #endregion


        #region Initialization


        public void CustomizeForReport()
        {
            this.RemoveGroup(this.StatusGroup);

            this.RemoveGroup(this.SpecialGroup);
            this.RemoveGroup(this.TreeLoopGroup);
            this.RemoveGroup(this.MeasureGroup);
            this.RemoveGroup(this.PeriodNameGroup);
            this.RemoveGroup(this.DesignerGroup);

            this.AddGroup(this.TreeLoopGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.PeriodNameGroup);
            this.AddGroup(this.DesignerGroup);
        }

        public void CustomizeForPowerpoint()
        {
            this.RemoveGroup(this.StatusGroup);

            this.RemoveGroup(this.SpecialGroup);
            this.RemoveGroup(this.TreeLoopGroup);
            this.RemoveGroup(this.MeasureGroup);
            this.RemoveGroup(this.PeriodNameGroup);
            this.RemoveGroup(this.DesignerGroup);

            this.AddGroup(this.SpecialGroup);
            this.AddGroup(this.TreeLoopGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();

            this.PresentationGroup = new PresentationGroup("Presentations", true);
            this.SpecialGroup = new SpecialGroup("Specials", true); 
            this.MeasureGroup = new MeasureGroup("Measure", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);
            this.DesignerGroup = new DesignerGroup("Designs", true);
            
            this.TreeLoopGroup = new TreeLoopGroup("Loops", true);
            this.PresentationGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.DesignerGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.PresentationGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.DesignerGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            CustomizeForPowerpoint();
        }

        #endregion

    }
}
