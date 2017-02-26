using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableBrowser : StructuredReportBrowser
    {

        public TransformationTableBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

    }
}
