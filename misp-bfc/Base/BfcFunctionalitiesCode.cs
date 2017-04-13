using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcFunctionalitiesCode : FunctionalitiesCode
    {

        public static string ADVISEMENT = "functionality.advisement";
        
        public static string PREFUNDING_ADVISEMENT = "functionality.prefunding.advisement";
        public static string PREFUNDING_ADVISEMENT_LIST = "functionality.prefunding.advisement.list";
        
        public static string MEMBER_ADVISEMENT = "functionality.member.advisement";
        public static string MEMBER_ADVISEMENT_LIST = "functionality.member.advisement.list";

        public static string REPLENISHMENT_INSTRUCTION_ADVISEMENT = "functionality.replenishment.instruction.advisement";
        public static string REPLENISHMENT_INSTRUCTION_ADVISEMENT_LIST = "functionality.replenishment.instruction.advisement.list";

        public static string SETTLEMENT_ADVISEMENT = "functionality.settlement.advisement";
        public static string SETTLEMENT_ADVISEMENT_LIST = "functionality.settlement.advisement.list";
        
        
        public static string REVIEW = "functionality.review";
        public static string REVIEW_PF_ACCOUNT = "functionality.review.pf.account";
        public static string REVIEW_SETTLEMENT_EVOLUTION = "functionality.review.settlement.evolution";
        public static string REVIEW_AGEING_BALANCE = "functionality.review.ageing.balance";

    }
}
