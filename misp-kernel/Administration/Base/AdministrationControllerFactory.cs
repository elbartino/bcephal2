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
                roleEditorController.ModuleName = "Administration_Role";// Misp.Kernel.Administration.PlugIn.MODULE_NAME;
                roleEditorController.Functionality = fonctionality;
                roleEditorController.ApplicationManager = this.ApplicationManager;
                roleEditorController.Service = ((AdministrationServiceFactory)ServiceFactory).GetRoleService();
                return roleEditorController;
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                
            }

            if (fonctionality == AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER)
            {
                
            }
            return null;
        }

    }
}
