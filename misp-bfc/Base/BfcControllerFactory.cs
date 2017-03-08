using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of ReconciliationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public BfcControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new BfcServiceFactory(applicationManager);
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
