using Misp.Kernel.Ui.Base;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Posting
{
    public class PostingGridBrowserController : InputGridBrowserController
    {

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new PostingGridBrowser(); }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.POSTING_GRID;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.NEW_POSTING_GRID_FUNCTIONALITY; }

    }
}
