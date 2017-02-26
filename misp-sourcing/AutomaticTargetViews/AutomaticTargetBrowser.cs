using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetBrowser : AutomaticSourcingBrowser
    {
        public AutomaticTargetBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        protected override string getTitle()
        {
            return "Automatic Target";
        }
    }
}
