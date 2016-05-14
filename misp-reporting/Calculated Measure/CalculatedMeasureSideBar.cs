using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Reporting.Calculated_Measure
{
    public class CalculatedMeasureSideBar :  SideBar    
    {

        #region Properties

        public MeasureGroup MeasureGroup { get; set; }
        public CalculatedMeasureGroup CalculatedMeasureGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// initialise les groupes d'objet qui apparaissent sur la side bar
        /// </summary>
        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.MeasureGroup = new MeasureGroup("Measure", true);
            this.CalculatedMeasureGroup = new CalculatedMeasureGroup("Calculated Measure",true);

            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculatedMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.CalculatedMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.CalculatedMeasureGroup);
            this.AddGroup(this.MeasureGroup);
           
        }

        #endregion
    }
}
