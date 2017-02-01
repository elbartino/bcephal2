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
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {
            return null;
        }

    }
}
