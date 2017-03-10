﻿using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Base;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridBrowserController : InputGridBrowserController
    {

        public PostingGridBrowserController()
            : base()
        {
            this.SubjectType = Kernel.Domain.SubjectType.POSTING_GRID;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new PostingGridBrowser(this.SubjectType, this.FunctionalityCode); }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.POSTING_GRID;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return ReconciliationFunctionalitiesCode.POSTING_GRID_EDIT; }

    }
}