using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableEditorItem : StructuredReportEditorItem
    {
       /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<StructuredReport> getNewEditorItemForm()
        {
            return new TransformationTableForm();
        }

        public override StructuredReportForm getStructuredReportForm()
        {
            return (TransformationTableForm)editorItemForm;
        }

        public TransformationTableForm getTransformationTableForm()
        {
            return (TransformationTableForm)editorItemForm;
        }
    }
}
