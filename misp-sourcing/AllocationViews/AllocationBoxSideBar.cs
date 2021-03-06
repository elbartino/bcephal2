﻿using System;
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

namespace Misp.Sourcing.AllocationViews
{
    public class AllocationBoxSideBar : SideBar
    {


        #region Constructor


        #endregion


        #region Properties

        public ModelSidebarGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }
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
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.PeriodGroup = new PeriodSidebarGroup();
            this.CustomizedTargetGroup = new CustomizedTargetGroup("Customized Target", true);
            this.CustomizedTargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CustomizedTargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.TreeLoopGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.AddGroup(this.TreeLoopGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.CustomizedTargetGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.PeriodGroup);
        }

        #endregion
        

    }
}
