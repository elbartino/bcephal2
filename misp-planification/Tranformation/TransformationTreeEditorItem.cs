using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeEditorItem : EditorItem<Misp.Kernel.Domain.TransformationTree>
    {
         /// <summary>
        /// Contruit une nouvelle instance de PeriodicityEditorItem
        /// </summary>
        public TransformationTreeEditorItem(Kernel.Domain.SubjectType subjectType)
            : base(subjectType)
        {
            this.Title = "Transformation Tree";
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.TransformationTree> getNewEditorItemForm()
        {
            return new TransformationTreeForm(this.SubjectType);
        }

        /// <summary>
        /// ModelForm
        /// </summary>
        public TransformationTreeForm GetTransformationTreeForm() { return (TransformationTreeForm)editorItemForm; }
        
        
    }
}
