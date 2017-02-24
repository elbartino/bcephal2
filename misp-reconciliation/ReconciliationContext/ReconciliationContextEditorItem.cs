using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextEditorItem : EditorItem<Kernel.Domain.ReconciliationContext>
    {
         /// <summary>
        /// Contruit une nouvelle instance de ReconciliationEditorItem
        /// </summary>
        public ReconciliationContextEditorItem(Kernel.Domain.SubjectType subjectType)
            : base(subjectType)
        {
            this.Title = "Reconciliation Configuration";
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Kernel.Domain.ReconciliationContext> getNewEditorItemForm()
        {
            return new ReconciliationContexForm(this.SubjectType);
        }

        /// <summary>
        /// ModelForm
        /// </summary>
        public ReconciliationContexForm getReconciliationContextForm() 
        {
            return (ReconciliationContexForm)editorItemForm; 
        }
        
        
    }
}
