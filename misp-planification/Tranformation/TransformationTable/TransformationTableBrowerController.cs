﻿using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableBrowerController : StructuredReportBrowserController
    {
        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }
    }
}
