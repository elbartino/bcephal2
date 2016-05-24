using Misp.Kernel.Application;
using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Base
{
    public class AdministrationServiceFactory : ServiceFactory
    {
        private SecurityService securityService;
      
        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AdministrationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public SecurityService GetSecurityService()
        {
            if (securityService == null)
            {
                securityService = new SecurityService();
                securityService.ResourcePath = AdministrationResourcePath.SECURITY_RESOURCE_PATH;
                securityService.GroupService = GetGroupService();
                securityService.FileService = GetFileService();
                configureService(securityService);
            }
            return securityService;
        }
        
    }
}
