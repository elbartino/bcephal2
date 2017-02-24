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
        public AutomaticTargetBrowser(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override string getTitle()
        {
            return "Automatic Target";
        }
    }
}
