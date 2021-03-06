﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Group;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Base;
using Misp.Kernel.Administration.Profil;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.UserProfile;


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
        public virtual Controller.Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {
            if (fonctionality == FunctionalitiesCode.HOME_PAGE)
            {
                HomePageController homeController = new HomePageController();
                homeController.ApplicationManager = this.ApplicationManager;
                homeController.Service = ServiceFactory.GetFileService();
                return homeController;
            }
            if (fonctionality == FunctionalitiesCode.PROJECT || fonctionality == FunctionalitiesCode.PROJECT_EDIT || fonctionality == FunctionalitiesCode.PROJECT_OPEN)
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
                controller.FunctionalityCode = fonctionality;
                return controller;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE)
            {
                RoleEditorController roleEditorController = new RoleEditorController();
                roleEditorController.ModuleName = "Administration";
                roleEditorController.FunctionalityCode = fonctionality;
                roleEditorController.ApplicationManager = this.ApplicationManager;
                roleEditorController.Service = ServiceFactory.GetRoleService();
                return roleEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_USER_EDIT)
            {
                UserEditorController userEditorController = new UserEditorController();
                userEditorController.ModuleName = "Administration";
                userEditorController.FunctionalityCode = fonctionality;
                userEditorController.ApplicationManager = this.ApplicationManager;
                userEditorController.Service = ServiceFactory.GetUserService();
                return userEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_USER_LIST)
            {
                UserBrowserController userBrowserController = new UserBrowserController();
                userBrowserController.ModuleName = "Administration";
                userBrowserController.FunctionalityCode = fonctionality;
                userBrowserController.ApplicationManager = this.ApplicationManager;
                userBrowserController.Service = ServiceFactory.GetUserService();
                return userBrowserController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_EDIT)
            {
                ProfilEditorController profilEditorController = new ProfilEditorController();
                profilEditorController.ModuleName = "Administration";
                profilEditorController.FunctionalityCode = fonctionality;
                profilEditorController.ApplicationManager = this.ApplicationManager;
                profilEditorController.Service = ServiceFactory.GetProfilService();
                return profilEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_LIST)
            {
                ProfilBrowserController profilBrowserController = new ProfilBrowserController();
                profilBrowserController.ModuleName = "Administration";
                profilBrowserController.FunctionalityCode = fonctionality;
                profilBrowserController.ApplicationManager = this.ApplicationManager;
                profilBrowserController.Service = ServiceFactory.GetProfilService();
                return profilBrowserController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_CONNECTED_USER_PROFILE)
            {
                UserProfileEditorController userProfileEditorController = new UserProfileEditorController();
                userProfileEditorController.ModuleName = "Administration";
                userProfileEditorController.FunctionalityCode = fonctionality;
                userProfileEditorController.ApplicationManager = this.ApplicationManager;
                userProfileEditorController.Service = ServiceFactory.GetUserService();
                return userProfileEditorController;
            }

            
            foreach (Plugin.IPlugin plugin in ApplicationManager.Plugins)
            {
                Controller.Controllable controller = plugin.ControllerFactory.GetController(fonctionality, viewType, editionMode);
                if (controller != null) return controller;
            }
            return null;
        }
        

    }
}
