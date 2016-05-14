﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Reporting.Report
{
    public class ReportEditorItem : InputTableEditorItem
    {

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<InputTable> getNewEditorItemForm()
        {
            return new ReportForm();
        }

        public override InputTableForm getInputTableForm()
        {
            return (ReportForm)editorItemForm;
        }

        public ReportForm getReportForm()
        {
            return (ReportForm)editorItemForm;
        }

    }
}
