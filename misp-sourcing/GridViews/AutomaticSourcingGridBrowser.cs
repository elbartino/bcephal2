using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class AutomaticSourcingGridBrowser : AutomaticSourcingBrowser
    {
        public AutomaticSourcingGridBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        protected override bool isAutomaticGrid()
        {
            return true;
        }


    }
}
