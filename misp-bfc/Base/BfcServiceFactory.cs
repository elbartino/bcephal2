using Misp.Bfc.Prefunding;
using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcServiceFactory : ServiceFactory
    {

        private PrefundingAccountService prefundingAccountService;

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public BfcServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public PrefundingAccountService GetPrefundingAccountService()
        {
            if (prefundingAccountService == null)
            {
                prefundingAccountService = new PrefundingAccountService();
                prefundingAccountService.ResourcePath = BfcResourcePath.BFC_PREFUNDING_ACCOUNT_RESOURCE_PATH;
                prefundingAccountService.RestClient = ApplicationManager.RestClient;
            }
            return prefundingAccountService;
        }


    }
}
