using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Initiation.Service;
using Misp.Initiation.Model;
using Misp.Initiation.Measure;
using Misp.Initiation.Periodicity;

namespace Misp.Initiation.Base
{
    public class InitiationServiceFactory : ServiceFactory
    {

        

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public InitiationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        

    }
}
