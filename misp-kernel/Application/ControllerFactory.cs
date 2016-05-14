using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Group;


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
            
            foreach (Plugin.IPlugin plugin in ApplicationManager.Plugins)
            {
                Controller.Controllable controller = plugin.ControllerFactory.GetController(fonctionality);
                if (controller != null) return controller;
            }
            return null;
        }
        

    }
}
