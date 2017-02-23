using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Designer;
using Misp.Sourcing.CustomizedTarget;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Sourcing.Table
{
    public class InputTableSideBar : SideBar
    {

        #region Constructor


        
        #endregion


        #region Properties

        public InputTableGroup InputTableGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }        
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }     
        public DesignerGroup DesignerGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public TreeLoopGroup TreeLoopGroup { get; set; }

        #endregion


        #region Initialization

        public override void SetReadOnly(bool readOnly)
        {
            EntityGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            TargetGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            MeasureGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            PeriodGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            DesignerGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            CustomizedTargetGroup.Visibility = readOnly ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;            
        }

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.InputTableGroup = new InputTableGroup("Input Tables", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.PeriodGroup = new PeriodSidebarGroup();
            this.DesignerGroup = new DesignerGroup("Designs", true);
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Targets", true);
            this.TreeLoopGroup = new TreeLoopGroup("Loops", true);

            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.InputTableGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            
            this.DesignerGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.InputTableGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;            
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.DesignerGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            
            this.AddGroup(this.InputTableGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            //this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.PeriodGroup);
            this.AddGroup(this.DesignerGroup);
        }

        public override void customize(List<Right> listeRights)
        {
            this.EntityGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, listeRights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.CustomizedTargetGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, listeRights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.MeasureGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, listeRights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.PeriodGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, listeRights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.DesignerGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, listeRights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        
        #endregion

    }
}
