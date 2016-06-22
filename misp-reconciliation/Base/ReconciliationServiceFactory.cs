using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Sourcing.Table;
using Misp.Kernel.Service;
using Misp.Reconciliation.Reconciliation;
using Misp.Kernel.Administration.Base;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationServiceFactory : ServiceFactory
    {

        private ReconciliationService reconciliationService;
        private PostingService postingService;

        private RoleService roleService;
        private UserService userService;
        private ProfilService profilService;

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReconciliationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public ReconciliationService GetReconciliationService()
        {
            if (reconciliationService == null)
            {
                reconciliationService = new ReconciliationService();
                reconciliationService.ResourcePath = ReconciliationResourcePath.RECONCILIATION_RESOURCE_PATH;
                reconciliationService.GroupService = GetGroupService();
                reconciliationService.FileService = GetFileService();
                reconciliationService.ModelService = GetModelService();
                reconciliationService.periodNameService = GetPeriodNameService();
                reconciliationService.measureService = GetMeasureService();
                reconciliationService.postingService = GetPostingService();
                configureService(reconciliationService);
            }
            return reconciliationService;
        }

        /// <summary>
        /// Gets PostingService
        /// </summary>
        public PostingService GetPostingService()
        {
            if (postingService == null)
            {
                postingService = new PostingService();
                postingService.ResourcePath = ReconciliationResourcePath.RECONCILIATON_POSTING_RESOURCE_PATH;
                postingService.GroupService = GetGroupService();
                postingService.FileService = GetFileService();
                postingService.ModelService = GetModelService();
                postingService.periodNameService = GetPeriodNameService();
                postingService.measureService = GetMeasureService();
                configureService(postingService);
            }
            return postingService;
        }


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
