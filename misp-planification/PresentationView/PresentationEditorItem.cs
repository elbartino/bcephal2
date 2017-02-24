using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Planification.PresentationView
{
    public class PresentationEditorItem : EditorItem<Presentation>
    {
        public string DEFAULT_NAME;

        public PresentationEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override IEditableView<Presentation> getNewEditorItemForm()
        {
            this.CanFloat = false;
            return new PresentationForm(this.SubjectType);
        }

        public virtual PresentationForm getPresentationForm()
        {
            return (PresentationForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }



    }
}
