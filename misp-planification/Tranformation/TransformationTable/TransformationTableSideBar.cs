using Misp.Kernel.Ui.Base;
using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableSideBar : StructuredReportSideBar
    {


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de ReportSideBar.
        /// </summary>
        public TransformationTableSideBar() { }

        #endregion


        #region Properties
        
        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.Clear();
            this.AddGroup(this.SpecialGroup);
            this.AddGroup(this.TreeLoopGroup); 
            this.AddGroup(this.EntityGroup); 
            this.AddGroup(this.MeasureGroup);
        }
        #endregion
    }
}

