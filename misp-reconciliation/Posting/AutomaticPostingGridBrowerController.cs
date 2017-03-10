﻿using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class AutomaticPostingGridBrowerController : AutomaticSourcingGridBrowerController
    {

        public AutomaticPostingGridBrowerController()
            : base()
        {
            this.SubjectType = Kernel.Domain.SubjectType.AUTOMATIC_POSTING_GRID;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality()
        {
            return Misp.Sourcing.Base.SourcingFunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT;
        }
        
        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_POSTING_GRID;
        }

    }
}