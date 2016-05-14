using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetEditor : AutomaticSourcingEditor
    {
        protected override EditorItem<Misp.Kernel.Domain.AutomaticSourcing> getNewPage() { return new AutomaticTargetEditorItem(); }

    }
}
