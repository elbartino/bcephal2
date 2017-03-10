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
        private SettlementEvolutionService settlementEvolutionService;
        private BfcItemService memberBankService;
        private BfcItemService schemeService;

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
                reviewService.SettlementEvolutionService = GetSettlementEvolutionService();
                reviewService.MemberBankService = GetMemberBankService();
                reviewService.SchemeService = GetSchemeService();
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

        public SettlementEvolutionService GetSettlementEvolutionService()
        {
            if (settlementEvolutionService == null)
            {
                settlementEvolutionService = new SettlementEvolutionService();
                settlementEvolutionService.ResourcePath = BfcResourcePath.BFC_SETTLEMENT_EVOLUTION_RESOURCE_PATH;
                settlementEvolutionService.RestClient = ApplicationManager.RestClient;
            }
            return settlementEvolutionService;
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
