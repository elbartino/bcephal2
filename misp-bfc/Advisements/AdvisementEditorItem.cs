using Misp.Bfc.Model;
using Misp.Bfc.Service;
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
        
        public AdvisementType AdvisementType { get; set; }

        public AdvisementEditorItem(Kernel.Domain.SubjectType subjectType, AdvisementType advisementType, AdvisementService service)
            : base(subjectType)
        {
            this.AdvisementType = advisementType;
            if (getAdvisementForm() != null)
            {
                getAdvisementForm().AdvisementType = advisementType;
                getAdvisementForm().Service = service;
                getAdvisementForm().CustomizeForType();
            }
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Advisement> getNewEditorItemForm()
        {
            this.CanFloat = false;          
            return new AdvisementForm(this.SubjectType, this.AdvisementType);
        }

        public AdvisementForm getAdvisementForm()
        {
            return (AdvisementForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }

        public Kernel.Service.GroupService GroupService { get; set; }
    }
}
