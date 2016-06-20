using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Service;
using Misp.Kernel.Administration.Base;

namespace Misp.Administration.Base
{
    public class AdministrationServiceFactory : ServiceFactory
    {

        private RoleService roleService;

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
                roleService.ResourcePath = AdministrationResourcePath.SECURITY_ROLE_RESOURCE_PATH;
                roleService.GroupService = GetGroupService();
                roleService.FileService = GetFileService();
                roleService.ModelService = GetModelService();
                configureService(roleService);
            }
            return roleService;
        }
        
    }
}
