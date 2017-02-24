using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Designer
{
    public class DesignerEditorItem : EditorItem<Design>
    {

        public DesignerEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Design> getNewEditorItemForm()
        {

            return new DesignerForm(this.SubjectType);
        }

        public virtual DesignerForm getDesignerForm()
        {
            return (DesignerForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {
        }
        
        public Kernel.Service.GroupService GroupService { get; set; }

    }
}

