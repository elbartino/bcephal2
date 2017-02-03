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
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Planification.Tranformation
{
    public class LoopDialogSideBar : SideBar
    {


        #region Constructor


        #endregion


        #region Properties

        public ModelSidebarGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CalculateMeasureGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public TreeLoopGroup TreeLoopGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }
        #endregion


        #region Initialization

        public override void SetReadOnly(bool readOnly)
        {
            EntityGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            TargetGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            MeasureGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            PeriodGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            CalculateMeasureGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            CustomizedTargetGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            TreeLoopGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            this.TreeLoopGroup = new TreeLoopGroup("Loops", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.CalculateMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);
            this.PeriodGroup = new PeriodSidebarGroup();
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Target", true);
            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
           

            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
        
            this.AddGroup(this.TreeLoopGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.CalculateMeasureGroup);
            this.AddGroup(this.PeriodGroup);
        }

        #endregion
        

    }
}
