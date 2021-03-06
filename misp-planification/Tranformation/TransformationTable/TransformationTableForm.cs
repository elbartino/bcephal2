﻿using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableForm : StructuredReportForm
    {

        #region Constructors

        public TransformationTableForm(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        #endregion


        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            StructuredReportPropertiesPanel.ColumnForms.DisplayCellRef = true;
        }

    }
}
