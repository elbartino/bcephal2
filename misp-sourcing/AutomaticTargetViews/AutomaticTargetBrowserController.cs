using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetBrowserController : AutomaticSourcingBrowserController
    {
        public AutomaticTargetBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Misp.Sourcing.Base.SourcingFunctionalitiesCode.AUTOMATIC_TARGET_EDIT; }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_TARGET;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new AutomaticTargetBrowser(); }
        
    }
}
