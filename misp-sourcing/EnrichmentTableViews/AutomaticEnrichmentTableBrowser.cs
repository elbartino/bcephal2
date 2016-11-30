using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.EnrichmentTableViews
{
    public class AutomaticEnrichmentTableBrowser : AutomaticSourcingBrowser
    {
        protected override bool isAutomaticGrid()
        {
            return false;
        }

        protected override string getTitle()
        {
            return "Automatic Enrichment Table";
        }
    }
}
