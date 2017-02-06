using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.WriteOffConfig
{
    public class ReconciliationFilterTemplateEditorItem : EditorItem<Kernel.Domain.ReconciliationFilterTemplate>
    {
          /// <summary>
        /// Contruit une nouvelle instance de ReconciliationEditorItem
        /// </summary>
        public ReconciliationFilterTemplateEditorItem()
            : base()
        {
            this.Title = "Reconciliation Filter Template";
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Kernel.Domain.ReconciliationFilterTemplate> getNewEditorItemForm()
        {
            return new ReconciliationFilterTemplateForm();
        }

        /// <summary>
        /// ModelForm
        /// </summary>
        public ReconciliationFilterTemplateForm getReconciliationFilterTemplateForm() 
        {
            return (ReconciliationFilterTemplateForm)editorItemForm; 
        }
        
    }
}
