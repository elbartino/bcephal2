using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportEditorItem : EditorItem<Misp.Kernel.Domain.StructuredReport>
    {

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.StructuredReport> getNewEditorItemForm() { return new StructuredReportForm(); }

        public virtual StructuredReportForm getStructuredReportForm()
        {
            return (StructuredReportForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args) {}

        public Kernel.Service.GroupService GroupService { get; set; }

    }
}
