using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Reporting.Report
{
    public class ReportForm : InputTableForm
    {

        #region Constructors

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            this.TablePropertiesPanel.CustomizeForReport();
            this.TableCellParameterPanel.CustomizeForReport();     
        }

        protected override bool isReport()
        {
            return true;
        }

        #endregion

    }
}
