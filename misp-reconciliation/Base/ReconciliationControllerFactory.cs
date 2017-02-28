using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Profil;
using Misp.Reconciliation.ReconciliationContext;
using Misp.Reconciliation.WriteOffConfig;
using Misp.Reconciliation.Reco;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of ReconciliationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReconciliationControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new ReconciliationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                ReconciliationFilterTemplateBrowserController controller = new ReconciliationFilterTemplateBrowserController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_EDIT && editionMode.HasValue)
            {
                ReconciliationFilterTemplateEditorController controller = new ReconciliationFilterTemplateEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }
                                                 
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_CONFIGURATION)
            {
                ReconciliationContextEditorController controller = new ReconciliationContextEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationContextService();
                return controller;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_TEMPLATE_CONFIGURATION)
            {
                ReconciliationFilterTemplateEditorController controller = new ReconciliationFilterTemplateEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }
            
            return null;
        }

    }
}
