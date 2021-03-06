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

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeSideBar : SideBar
    {


        #region Constructor


        #endregion


        #region Properties

        public TransformationTreeGroup TransformationTreeGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.TransformationTreeGroup = new TransformationTreeGroup("Transformation Trees", true);
            this.TransformationTreeGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TransformationTreeGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.AddGroup(this.TransformationTreeGroup);
        }

        #endregion
        

    }
}
