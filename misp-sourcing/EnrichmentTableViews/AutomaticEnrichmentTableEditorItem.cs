using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.EnrichmentTableViews
{
    public class AutomaticEnrichmentTableEditorItem : AutomaticSourcingEditorItem
    {

        public AutomaticEnrichmentTableEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<AutomaticSourcing> getNewEditorItemForm()
        {
            return new AutomaticEnrichmentTableForm(this.SubjectType);
        }

        public override AutomaticSourcingForm getAutomaticSourcingForm()
        {
            return (AutomaticEnrichmentTableForm)editorItemForm;
        }

        public AutomaticEnrichmentTableForm getAutomaticEnrichmentTableForm()
        {
            return (AutomaticEnrichmentTableForm)editorItemForm;
        }
    }
}
