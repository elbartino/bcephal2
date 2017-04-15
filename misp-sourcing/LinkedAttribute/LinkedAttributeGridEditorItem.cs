using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.LinkedAttribute
{
    public class LinkedAttributeGridEditorItem : EditorItem<LinkedAttributeGrid>
    {

        public LinkedAttributeGridEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<LinkedAttributeGrid> getNewEditorItemForm() { return new LinkedAttributeGridForm(this.SubjectType); }

        public virtual LinkedAttributeGridForm getLinkedAttributeGridForm()
        {
            return (LinkedAttributeGridForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args) { }
        
    }
}
