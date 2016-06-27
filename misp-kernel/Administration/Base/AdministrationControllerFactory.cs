using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Kernel.Administration.Base;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Profil;

namespace Misp.Reconciliation.Base
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
            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE)
            {
                RoleBrowserController roleEditorController = new RoleBrowserController();
                roleEditorController.ModuleName = "Administration";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                roleEditorController.Functionality = fonctionality;
                roleEditorController.ApplicationManager = this.ApplicationManager;
                roleEditorController.Service = ((AdministrationServiceFactory)ServiceFactory).GetRoleService();
                return roleEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                UserEditorController userEditorController = new UserEditorController();
                userEditorController.ModuleName = "Administration";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                userEditorController.Functionality = fonctionality;
                userEditorController.ApplicationManager = this.ApplicationManager;
                userEditorController.Service = ((AdministrationServiceFactory)ServiceFactory).GetUserService();
                return userEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_USER)
            {
                UserBrowserController userBrowserController = new UserBrowserController();
                userBrowserController.ModuleName = "Administration";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                userBrowserController.Functionality = fonctionality;
                userBrowserController.ApplicationManager = this.ApplicationManager;
                userBrowserController.Service = ((AdministrationServiceFactory)ServiceFactory).GetUserService();
                return userBrowserController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL)
            {
                ProfilEditorController profilEditorController = new ProfilEditorController();
                profilEditorController.ModuleName = "Administration";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                profilEditorController.Functionality = fonctionality;
                profilEditorController.ApplicationManager = this.ApplicationManager;
                profilEditorController.Service = ((AdministrationServiceFactory)ServiceFactory).GetProfilService();
                return profilEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL)
            {
                ProfilBrowserController profilBrowserController = new ProfilBrowserController();
                profilBrowserController.ModuleName = "Administration";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                profilBrowserController.Functionality = fonctionality;
                profilBrowserController.ApplicationManager = this.ApplicationManager;
                profilBrowserController.Service = ((AdministrationServiceFactory)ServiceFactory).GetProfilService();
                return profilBrowserController;
            }
            return null;
        }

    }
}
