using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Reporting.Report
{
    public class ReportEditor : InputTableEditor
    {

        public ReportEditor(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<InputTable> getNewPage() { return new ReportEditorItem(this.SubjectType); }

    }
}
