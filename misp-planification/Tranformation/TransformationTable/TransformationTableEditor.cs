using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableEditor : StructuredReportEditor
    {
        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<StructuredReport> getNewPage() { return new TransformationTableEditorItem(); }
    }
}
