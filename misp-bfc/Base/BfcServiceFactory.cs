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
        private BfcItemService memberBankService;
        private BfcItemService schemeService;
        private AdvisementService prefundingAdvisementService;
        private AdvisementService memberAdvisementService;
        private AdvisementService exceptionalAdvisementService;
        private AdvisementService settlementAdvisementService;

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
                reviewService.MemberBankService = GetMemberBankService();
                reviewService.SchemeService = GetSchemeService();
            }
            return reviewService;
        }

        public AdvisementService GetPrefundingAdvisementService()
        {
            if (prefundingAdvisementService == null)
            {
                prefundingAdvisementService = new AdvisementService();
                prefundingAdvisementService.ResourcePath = BfcResourcePath.BFC_PREFUNDING_ADVISEMENT_RESOURCE_PATH;
                prefundingAdvisementService.RestClient = ApplicationManager.RestClient;
                prefundingAdvisementService.GroupService = GetGroupService();
            }
            return prefundingAdvisementService;
        }

        public AdvisementService GetMemberAdvisementService()
        {
            if (memberAdvisementService == null)
            {
                memberAdvisementService = new AdvisementService();
                memberAdvisementService.ResourcePath = BfcResourcePath.BFC_MEMBER_ADVISEMENT_RESOURCE_PATH;
                memberAdvisementService.RestClient = ApplicationManager.RestClient;
                memberAdvisementService.GroupService = GetGroupService();
            }
            return memberAdvisementService;
        }

        public AdvisementService GetExceptionalAdvisementService()
        {
            if (exceptionalAdvisementService == null)
            {
                exceptionalAdvisementService = new AdvisementService();
                exceptionalAdvisementService.ResourcePath = BfcResourcePath.BFC_EXCEPTIONAL_ADVISEMENT_RESOURCE_PATH;
                exceptionalAdvisementService.RestClient = ApplicationManager.RestClient;
                exceptionalAdvisementService.GroupService = GetGroupService();
            }
            return exceptionalAdvisementService;
        }

        public AdvisementService GetSettlementAdvisementService()
        {
            if (settlementAdvisementService == null)
            {
                settlementAdvisementService = new AdvisementService();
                settlementAdvisementService.ResourcePath = BfcResourcePath.BFC_SETTLEMENT_ADVISEMENT_RESOURCE_PATH;
                settlementAdvisementService.RestClient = ApplicationManager.RestClient;
                settlementAdvisementService.GroupService = GetGroupService();
            }
            return settlementAdvisementService;
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

        public BfcItemService GetSchemeService()
        {
            if (schemeService == null)
            {
                schemeService = new BfcItemService();
                schemeService.ResourcePath = BfcResourcePath.BFC_SCHEME_RESOURCE_PATH;
                schemeService.RestClient = ApplicationManager.RestClient;
            }
            return schemeService;
        }


    }
}
