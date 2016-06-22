using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Service;
using Misp.Kernel.Administration.Base;

namespace Misp.Reconciliation.Base
{
    public class AdministrationServiceFactory : ServiceFactory
    {

        private RoleService roleService;
        private UserService userService;
        private ProfilService profilService;

        /// <summary>
        /// Build a new instance of AdministrationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AdministrationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets RoleService
        /// </summary>
        public RoleService GetRoleService()
        {
            if (roleService == null)
            {
                roleService = new RoleService();
                roleService.ResourcePath = AdministrationResourcePath.SECURITY_RESOURCE_PATH; // SECURITY_ROLE_RESOURCE_PATH;
                roleService.GroupService = GetGroupService();
                roleService.FileService = GetFileService();
                roleService.ModelService = GetModelService();
                configureService(roleService);
            }
            return roleService;
        }

        /// <summary>
        /// Gets UserService
        /// </summary>
        public UserService GetUserService()
        {
            if (userService == null)
            {
                userService = new UserService();
                userService.ResourcePath = AdministrationResourcePath.SECURITY_RESOURCE_PATH;//SECURITY_USER_RESOURCE_PATH;
                userService.GroupService = GetGroupService();
                configureService(userService);
            }
            return userService;
        }

        /// <summary>
        /// Gets ProfilService
        /// </summary>
        public ProfilService GetProfilService()
        {
            if (profilService == null)
            {
                profilService = new ProfilService();
                profilService.ResourcePath = AdministrationResourcePath.SECURITY_RESOURCE_PATH;//SECURITY_PROFIL_RESOURCE_PATH;
                profilService.GroupService = GetGroupService();
                configureService(profilService);
            }
            return profilService;
        }
        
    }
}
