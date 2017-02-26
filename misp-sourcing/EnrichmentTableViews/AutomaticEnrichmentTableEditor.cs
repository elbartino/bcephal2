using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.EnrichmentTableViews
{
    public class AutomaticEnrichmentTableEditor : AutomaticSourcingEditor
    {
        public AutomaticEnrichmentTableEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        protected override EditorItem<Misp.Kernel.Domain.AutomaticSourcing> getNewPage() { return new AutomaticEnrichmentTableEditorItem(this.SubjectType); }
    }
}
