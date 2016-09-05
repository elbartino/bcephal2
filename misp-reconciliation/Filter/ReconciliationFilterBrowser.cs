using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterBrowser : InputGridBrowser
    {

        protected override string getTitle()
        {
            return "Reconciliation Filters";
        }

    }
}

