using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Base;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterBrowserController : InputGridBrowserController
    {

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new ReconciliationFilterBrowser(); }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.RECONCILIATION_FILTER;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return ReconciliationFunctionalitiesCode.NEW_RECONCILIATION_FILTER_FUNCTIONALITY; }

    }
}

