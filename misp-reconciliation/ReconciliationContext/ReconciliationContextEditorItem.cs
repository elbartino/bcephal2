﻿using Misp.Kernel.Domain;
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
        public ReconciliationContextEditorItem()
            : base()
        {
            this.Title = "Reconciliation Context";
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<ReconciliationTemplate> getNewEditorItemForm()
        {
            return new ReconciliationForm();
        }

        /// <summary>
        /// ModelForm
        /// </summary>
        public ReconciliationForm getReconciliationForm() 
        { 
            return (ReconciliationForm)editorItemForm; 
        }
        
        
    }
}
