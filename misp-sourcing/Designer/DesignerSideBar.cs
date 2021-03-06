﻿using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Sidebar;
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
        public ModelSidebarGroup EntityGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public CalculatedMeasureGroup CalculateMeasureGroup { get; set; }
        public CustomizedTargetGroup CustomizedTargetGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.DesignerGroup = new DesignerGroup("Designs", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.CalculateMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);
            this.PeriodGroup = new PeriodSidebarGroup();
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Targets", true);

            this.DesignerGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.DesignerGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CalculateMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;


            this.AddGroup(this.DesignerGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.CalculateMeasureGroup);
            this.AddGroup(this.PeriodGroup);
            //this.AddGroup(this.TagGroup);

        }

        public override void Customize(List<Right> rights, bool readOnly = false)
        {
            this.EntityGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.MeasureGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            this.PeriodGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        #endregion

    }
}
