using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridBrowser : InputGridBrowser
    {

        protected override string getTitle()
        {
            return "Posting Grids";
        }

    }
}
