using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingEditorItem : EditorItem<Misp.Kernel.Domain.AutomaticSourcing>
    {
        public AutomaticSourcingEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override IEditableView<Misp.Kernel.Domain.AutomaticSourcing> getNewEditorItemForm()
        {
            return new AutomaticSourcingForm(this.SubjectType);
        }

        public virtual AutomaticSourcingForm getAutomaticSourcingForm()
        {

            return (AutomaticSourcingForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {
        }

        public Kernel.Service.GroupService GroupService { get; set; }
    }

    
}
