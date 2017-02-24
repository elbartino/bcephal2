using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Sourcing.Table;

namespace Misp.Reporting.Report
{
    public class ReportBrowserController : InputTableBrowserController
    {

        public ReportBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.REPORT;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Reporting.Base.ReportingFunctionalitiesCode.REPORT_EDIT; }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.REPORT;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new ReportBrowser(this.SubjectType); }                

    }
}
