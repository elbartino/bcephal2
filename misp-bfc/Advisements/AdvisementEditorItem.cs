using Misp.Bfc.Model;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Advisements
{
    public class AdvisementEditorItem : EditorItem<Advisement>
    {
        public string DEFAULT_NAME;

        public AdvisementEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Advisement> getNewEditorItemForm()
        {
            this.CanFloat = false;
          
            return new AdvisementForm(this.SubjectType);
        }

        public virtual AdvisementForm getInputTableForm()
        {
            return (AdvisementForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }

        public Kernel.Service.GroupService GroupService { get; set; }
    }
}
