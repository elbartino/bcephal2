using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Reconciliation.Posting;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Profil;
using Misp.Reconciliation.ReconciliationContext;
using Misp.Reconciliation.RecoGrid;

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
        public override Controllable GetController(string fonctionality)
        {
            
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTERS_FUNCTIONALITY)
            {
                //ReconciliationEditorController recoEditorController = new ReconciliationEditorController();
                //recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //recoEditorController.Functionality = fonctionality;
                //recoEditorController.ApplicationManager = this.ApplicationManager;
                //recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationService();
                //return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.LIST_RECONCILIATION_FILTERS_FUNCTIONALITY)
            {
                //ReconciliationBrowserController recoBrowserController = new ReconciliationBrowserController();
                //recoBrowserController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //recoBrowserController.Functionality = fonctionality;
                //recoBrowserController.ApplicationManager = this.ApplicationManager;
                //recoBrowserController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationService();
                //return recoBrowserController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_POSTING_FUNCTIONALITY)
            {
                //PostingBrowserController controller = new PostingBrowserController();
                //controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //controller.Functionality = fonctionality;
                //controller.ApplicationManager = this.ApplicationManager;
                //controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingService();
                //return controller;

                PostingEditorController controller = new PostingEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationGridService();
                return controller;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.LIST_POSTING_GRID_FUNCTIONALITY)
            {
                PostingGridBrowserController controller = new PostingGridBrowserController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingGridService();
                return controller;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.NEW_POSTING_GRID_FUNCTIONALITY)
            {
                PostingGridEditorController controller = new PostingGridEditorController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingGridService();
                return controller;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.NEW_AUTOMATIC_POSTING_GRID_FUNCTIONALITY)
            {
                AutomaticPostingGridEditorController automaticSourcingGridController = new AutomaticPostingGridEditorController();
                automaticSourcingGridController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridController.Functionality = fonctionality;
                automaticSourcingGridController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetAutomaticPostingGridService();
                //automaticSourcingGridController.InputTableService = ((ReconciliationServiceFactory)ServiceFactory).GetInputTableService();
                return automaticSourcingGridController;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.LIST_AUTOMATIC_POSTING_GRID_FUNCTIONALITY)
            {
                AutomaticPostingGridBrowerController automaticSourcingGridBrowerController = new AutomaticPostingGridBrowerController();
                automaticSourcingGridBrowerController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridBrowerController.Functionality = fonctionality;
                automaticSourcingGridBrowerController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridBrowerController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetAutomaticPostingGridService();
                return automaticSourcingGridBrowerController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_CONTEXT_FUNCTIONALITY)
            {

                ReconciliationContextEditorController controller = new ReconciliationContextEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationContextService();
                return controller;
            }


            return null;
        }

    }
}
