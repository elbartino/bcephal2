using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.Calculated_Measure;
using Misp.Sourcing.CustomizedTarget;

namespace Misp.Reporting.Report
{
    public class ReportSideBar : InputTableSideBar
    {

        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de ReportSideBar.
        /// </summary>
        public ReportSideBar() { }

        #endregion


        #region Properties

        public InputTableGroup ReportGroup { get { return InputTableGroup; } set { InputTableGroup = value; } }
        public CalculatedMeasureGroup CalculatedMeasureGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.CalculatedMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);
            this.CalculatedMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.CalculatedMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
                       
            this.RemoveGroup(this.DesignerGroup);
                       
            this.AddGroup(this.CalculatedMeasureGroup);

            this.AddGroup(this.DesignerGroup);
           
            this.InputTableGroup.Header = "Reports";
        }

        #endregion

    }
}
