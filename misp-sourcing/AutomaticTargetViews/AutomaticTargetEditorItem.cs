using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetEditorItem : AutomaticSourcingEditorItem
    {

        public AutomaticTargetEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<AutomaticSourcing> getNewEditorItemForm()
        {
            return new AutomaticTargetForm(this.SubjectType);
        }

        public override AutomaticSourcingForm getAutomaticSourcingForm()
        {
            return (AutomaticTargetForm)editorItemForm;
        }

        public AutomaticTargetForm getAutomaticTargetForm()
        {
            return (AutomaticTargetForm)editorItemForm;
        }
    }
}
