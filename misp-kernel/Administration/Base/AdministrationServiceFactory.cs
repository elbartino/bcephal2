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

        /// <summary>
        /// Build a new instance of AdministrationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AdministrationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        
        
    }
}
