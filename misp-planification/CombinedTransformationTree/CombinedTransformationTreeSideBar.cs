using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeSideBar : SideBar
    {

        
        #region Properties

        public CombinedTransformationTreeGroup CombineTransformationTreeGroup { get; set; }

        public Planification.Tranformation.TransformationTreeGroup TransformationTreeGroup { get; set; }
        public EntityGroup EntityGroup { get; set; }
        public TargetGroup StandardTargetGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.CombineTransformationTreeGroup = new CombinedTransformationTreeGroup("Combined Transformation trees", true);
            this.TransformationTreeGroup = new Tranformation.TransformationTreeGroup("Transformation Trees",true);
            
            this.TransformationTreeGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CombineTransformationTreeGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.TransformationTreeGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CombineTransformationTreeGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.CombineTransformationTreeGroup);
            this.AddGroup(this.TransformationTreeGroup);
        }

        #endregion

    }
}
