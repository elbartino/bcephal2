﻿using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportSideBar : SideBar
    {
        
        #region Constructor


        #endregion


        #region Properties

        public StructuredReportGroup StructuredReportGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
     
        public SpecialGroup SpecialGroup { get; set; }
        public TreeLoopGroup TreeLoopGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }
        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.StructuredReportGroup = new StructuredReportGroup("Structured Reports", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.PeriodGroup = new PeriodSidebarGroup();

            this.StructuredReportGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.StructuredReportGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;           
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;


            this.SpecialGroup = new SpecialGroup("Specials ", true);
            this.SpecialGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.SpecialGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup = new TreeLoopGroup("Loops ", true);
            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.StructuredReportGroup);
            this.AddGroup(this.SpecialGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.PeriodGroup);
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
