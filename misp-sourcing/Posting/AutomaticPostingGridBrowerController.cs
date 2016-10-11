using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Posting
{
    public class AutomaticPostingGridBrowerController : AutomaticSourcingGridBrowerController
    {

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
