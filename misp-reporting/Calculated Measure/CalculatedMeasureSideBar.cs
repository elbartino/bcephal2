using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Reporting.Calculated_Measure
{
    public class CalculatedMeasureSideBar :  SideBar    
    {

        #region Properties

        public MeasureSidebarGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CalculatedMeasureGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// initialise les groupes d'objet qui apparaissent sur la side bar
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.MeasureGroup = new MeasureSidebarGroup("Measure", true);
            this.CalculatedMeasureGroup = new CalculatedMeasureGroup("Calculated Measure",true);

            this.CalculatedMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculatedMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.CalculatedMeasureGroup);
            this.AddGroup(this.MeasureGroup);
           
        }
        public override void Customize(List<Right> rights, bool readOnly = false)
        {
            this.MeasureGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            //this.CalculatedMeasureGroup.Visibility = Kernel.Util.RightsUtil.HasRight(RightType.EDIT, rights) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        #endregion
    }
}
