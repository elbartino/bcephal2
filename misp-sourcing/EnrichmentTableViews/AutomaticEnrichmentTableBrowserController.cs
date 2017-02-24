using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.EnrichmentTableViews
{
    public class AutomaticEnrichmentTableBrowserController : AutomaticSourcingBrowserController
    {
        public AutomaticEnrichmentTableBrowserController() : base()
        {
            this.SubjectType = Kernel.Domain.SubjectType.AUTOMATIC_ENRICHMENT_TABLE;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality()
        {
            return Misp.Sourcing.Base.SourcingFunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new AutomaticEnrichmentTableBrowser(this.SubjectType); }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_ENRICHMENT_TABLE;
        }
    }
}
