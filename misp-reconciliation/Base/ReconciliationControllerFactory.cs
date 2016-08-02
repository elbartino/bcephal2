using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Reconciliation.Reconciliation;
using Misp.Reconciliation.Posting;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Profil;
using Misp.Reporting.ReportGrid;
using Misp.Reconciliation.ReconciliationContext;

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
                ReconciliationEditorController recoEditorController = new ReconciliationEditorController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationService();
                return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.LIST_RECONCILIATION_FILTERS_FUNCTIONALITY)
            {
                ReconciliationBrowserController recoBrowserController = new ReconciliationBrowserController();
                recoBrowserController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoBrowserController.Functionality = fonctionality;
                recoBrowserController.ApplicationManager = this.ApplicationManager;
                recoBrowserController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationService();
                return recoBrowserController;
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

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_CONTEXT_FUNCTIONALITY)
            {

                ReconciliationContextEditorController controller = new ReconciliationContextEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationContextService();
                return controller;
            }





            if (fonctionality == ReconciliationFunctionalitiesCode.ADMINISTRATION_ROLE)
            {
                RoleBrowserController recoEditorController = new RoleBrowserController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetRoleService();
                return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                UserEditorController recoEditorController = new UserEditorController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetUserService();
                return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.ADMINISTRATION_LIST_USER)
            {
                UserBrowserController recoEditorController = new UserBrowserController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetUserService();
                return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL)
            {
                ProfilEditorController recoEditorController = new ProfilEditorController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetProfilService();
                return recoEditorController;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL)
            {
                ProfilBrowserController recoEditorController = new ProfilBrowserController();
                recoEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                recoEditorController.Functionality = fonctionality;
                recoEditorController.ApplicationManager = this.ApplicationManager;
                recoEditorController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetProfilService();
                return recoEditorController;
            }

            return null;
        }

    }
}
