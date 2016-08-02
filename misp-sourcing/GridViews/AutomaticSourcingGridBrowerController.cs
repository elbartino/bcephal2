﻿using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class AutomaticSourcingGridBrowerController : AutomaticSourcingBrowserController
    {
        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality()
        {
            return Misp.Sourcing.Base.SourcingFunctionalitiesCode.NEW_AUTOMATIC_GRID_FUNCTIONALITY;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new AutomaticSourcingGridBrowser(); }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_GRID;
        }
    }
}