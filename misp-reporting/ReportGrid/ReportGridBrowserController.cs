using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.ReportGrid
{
    public class ReportGridBrowserController : InputGridBrowserController
    {
        public ReportGridBrowserController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.NEW_REPORT_GRID_FUNCTIONALITY; }

    }
}
