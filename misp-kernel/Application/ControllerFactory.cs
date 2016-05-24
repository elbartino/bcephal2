using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Group;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Base;


namespace Misp.Kernel.Application
{
    public class ControllerFactory
    {

        /// <summary>
        /// Build a new instance of ControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ControllerFactory(ApplicationManager applicationManager)
        {
            this.ApplicationManager = applicationManager;
            this.ServiceFactory = new ServiceFactory(applicationManager);
        }

        /// <summary>
        /// Gets or sets the ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Gets or sets the ServiceFactory
        /// </summary>
        public ServiceFactory ServiceFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public virtual Controller.Controllable GetController(string fonctionality)
        {
            if (fonctionality == FunctionalitiesCode.HOME_PAGE_FUNCTIONALITY)
            {
                HomePageController homeController = new HomePageController();
                homeController.ApplicationManager = this.ApplicationManager;
                homeController.Service = ServiceFactory.GetFileService();
                return homeController;
            }
            if (fonctionality == FunctionalitiesCode.FILE_FUNCTIONALITY)
            {
                FileController fileController = new FileController();
                fileController.ApplicationManager = this.ApplicationManager;
                fileController.Service = ServiceFactory.GetFileService();
                return fileController;
            }
            if (fonctionality == FunctionalitiesCode.LIST_GROUP_FUNCTIONALITY)
            {
                GroupBrowserController controller = new GroupBrowserController();
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ServiceFactory.GetGroupService();
                controller.Functionality = fonctionality;
                return controller;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                //UserEditorController userEditorController = new UserEditorController();
                //userEditorController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //userEditorController.Functionality = fonctionality;
                //userEditorController.ApplicationManager = this.ApplicationManager;
                //userEditorController.Service = ServiceFactory.GetReconciliationService();
                //return recoEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_USER)
            {
                UserBrowserController userBrowserController = new UserBrowserController();
                // userBrowserController.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                userBrowserController.Functionality = fonctionality;
                userBrowserController.ApplicationManager = this.ApplicationManager;
                userBrowserController.Service = ServiceFactory.GetSecurityService();
                return userBrowserController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL)
            {
                //PostingBrowserController controller = new PostingBrowserController();
                //controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                //controller.Functionality = fonctionality;
                //controller.ApplicationManager = this.ApplicationManager;
                //controller.Service = ServiceFactory.GetPostingService();
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
            
            foreach (Plugin.IPlugin plugin in ApplicationManager.Plugins)
            {
                Controller.Controllable controller = plugin.ControllerFactory.GetController(fonctionality);
                if (controller != null) return controller;
            }
            return null;
        }
        

    }
}
