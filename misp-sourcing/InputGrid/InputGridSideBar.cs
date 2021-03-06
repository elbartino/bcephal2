﻿using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridSideBar : SideBar
    {

        #region Constructor


        #endregion


        #region Properties

        public GrilleGroup GrilleGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.GrilleGroup = new GrilleGroup("Grids", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.PeriodGroup = new PeriodSidebarGroup();


            this.GrilleGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.GrilleGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;


            this.AddGroup(this.GrilleGroup);
            this.AddGroup(this.EntityGroup);
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
