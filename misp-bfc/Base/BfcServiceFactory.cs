using Misp.Bfc.Service;
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

        private ReviewService reviewService;
        private PrefundingAccountService prefundingAccountService;
        private BfcItemService memberBankService;

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public BfcServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>reviewService</returns>
        public ReviewService GetReviewService()
        {
            if (reviewService == null)
            {
                reviewService = new ReviewService();
                reviewService.ResourcePath = BfcResourcePath.BFC_REVIEW_RESOURCE_PATH;
                reviewService.RestClient = ApplicationManager.RestClient;
                reviewService.PrefundingAccountService = GetPrefundingAccountService();
                reviewService.MemberBankService = GetMemberBankService();
            }
            return reviewService;
        }

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

        /// <summary>
        /// memberBankService
        /// </summary>
        public BfcItemService GetMemberBankService()
        {
            if (memberBankService == null)
            {
                memberBankService = new BfcItemService();
                memberBankService.ResourcePath = BfcResourcePath.BFC_MEMBER_BANK_RESOURCE_PATH;
                memberBankService.RestClient = ApplicationManager.RestClient;
            }
            return memberBankService;
        }


    }
}
