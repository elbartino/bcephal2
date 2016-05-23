using Misp.Kernel.Administration.User;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Base
{
    public class AdministrationControllerFactory : ControllerFactory
    {
         /// <summary>
        /// Build a new instance of ReconciliationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AdministrationControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new AdministrationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality)
        {
            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                //UserEditorController userEditorController = new UserEditorController();
                //userEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //userEditorController.Functionality = fonctionality;
                //userEditorController.ApplicationManager = this.ApplicationManager;
                //userEditorController.Service = ((AdministrationServiceFactory)ServiceFactory).GetReconciliationService();
                //return recoEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_USER)
            {
                UserBrowserController userBrowserController = new UserBrowserController();
               // userBrowserController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                userBrowserController.Functionality = fonctionality;
                userBrowserController.ApplicationManager = this.ApplicationManager;
                userBrowserController.Service = ((AdministrationServiceFactory)ServiceFactory).GetSecurityService();
                return userBrowserController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL)
            {
                //PostingBrowserController controller = new PostingBrowserController();
                //controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //controller.Functionality = fonctionality;
                //controller.ApplicationManager = this.ApplicationManager;
                //controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingService();
                //return controller;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL)
            {
                //PostingBrowserController controller = new PostingBrowserController();
                //controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //controller.Functionality = fonctionality;
                //controller.ApplicationManager = this.ApplicationManager;
                //controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingService();
                //return controller;
            }
            return null;
        }
    }
}
