using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Bfc.Base
{
    public class AdvisementMenu : ApplicationMenu
    {
        public ApplicationMenu ReviewMenu { get; private set; }
        public ApplicationMenu ReviewPFAccountMenu { get; private set; }
        public ApplicationMenu ReviewSettlementEvolutionMenu { get; private set; }
        public ApplicationMenu ReviewAgeingBalanceMenu { get; private set; }

        public ApplicationMenu PrefundingAdvisementMenu { get; private set; }
        public ApplicationMenu NewPrefundingAdvisementMenu { get; private set; }
        public ApplicationMenu ListPrefundingAdvisementMenu { get; private set; }

        public ApplicationMenu MemberAdvisementMenu { get; private set; }
        public ApplicationMenu NewMemberAdvisementMenu { get; private set; }
        public ApplicationMenu ListMemberAdvisementMenu { get; private set; }

        public ApplicationMenu ExceptionalAdvisementMenu { get; private set; }
        public ApplicationMenu NewExceptionalAdvisementMenu { get; private set; }
        public ApplicationMenu ListExceptionalAdvisementMenu { get; private set; }

        public ApplicationMenu SettlementAdvisementMenu { get; private set; }
        public ApplicationMenu NewSettlementAdvisementMenu { get; private set; }
        public ApplicationMenu ListSettlementAdvisementMenu { get; private set; }


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(PrefundingAdvisementMenu);
            menus.Add(MemberAdvisementMenu);
            menus.Add(ExceptionalAdvisementMenu);
            menus.Add(SettlementAdvisementMenu);
            menus.Add(ReviewMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = BfcFunctionalitiesCode.ADVISEMENT;
            this.Header = BfcFunctionalitiesLabel.ADVISEMENT_LABEL;
            PrefundingAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.ADVISEMENT, BfcFunctionalitiesLabel.PREFUNDING_ADVISEMENT_LABEL, BfcFunctionalitiesCode.ADVISEMENT);
            NewPrefundingAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT, BfcFunctionalitiesLabel.NEW_PREFUNDING_ADVISEMENT_LABEL, NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT), Kernel.Domain.RightType.CREATE);
            ListPrefundingAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT, BfcFunctionalitiesLabel.LIST_PREFUNDING_ADVISEMENT_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT_LIST), Kernel.Domain.RightType.VIEW);
            PrefundingAdvisementMenu.Items.Add(NewPrefundingAdvisementMenu);
            PrefundingAdvisementMenu.Items.Add(ListPrefundingAdvisementMenu);

            MemberAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.ADVISEMENT, BfcFunctionalitiesLabel.MEMBER_ADVISEMENT_LABEL, BfcFunctionalitiesCode.ADVISEMENT);
            NewMemberAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.MEMBER_ADVISEMENT, BfcFunctionalitiesLabel.NEW_MEMBER_ADVISEMENT_LABEL, NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.MEMBER_ADVISEMENT), Kernel.Domain.RightType.CREATE);
            ListMemberAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.MEMBER_ADVISEMENT, BfcFunctionalitiesLabel.LIST_MEMBER_ADVISEMENT_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.MEMBER_ADVISEMENT_LIST), Kernel.Domain.RightType.VIEW);
            MemberAdvisementMenu.Items.Add(NewMemberAdvisementMenu);
            MemberAdvisementMenu.Items.Add(ListMemberAdvisementMenu);

            ExceptionalAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.ADVISEMENT, BfcFunctionalitiesLabel.REPLENISHMENT_INSTRUCTION_ADVISEMENT_LABEL, BfcFunctionalitiesCode.ADVISEMENT);
            NewExceptionalAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT, BfcFunctionalitiesLabel.NEW_REPLENISHMENT_INSTRUCTION_ADVISEMENT_LABEL, NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT), Kernel.Domain.RightType.CREATE);
            ListExceptionalAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT, BfcFunctionalitiesLabel.LIST_REPLENISHMENT_INSTRUCTION_ADVISEMENT_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT_LIST), Kernel.Domain.RightType.VIEW);
            ExceptionalAdvisementMenu.Items.Add(NewExceptionalAdvisementMenu);
            ExceptionalAdvisementMenu.Items.Add(ListExceptionalAdvisementMenu);

            SettlementAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.ADVISEMENT, BfcFunctionalitiesLabel.SETTLEMENT_ADVISEMENT_LABEL, BfcFunctionalitiesCode.ADVISEMENT);
            NewSettlementAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT, BfcFunctionalitiesLabel.NEW_SETTLEMENT_ADVISEMENT_LABEL, NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT), Kernel.Domain.RightType.CREATE);
            ListSettlementAdvisementMenu = BuildMenu(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT, BfcFunctionalitiesLabel.LIST_SETTLEMENT_ADVISEMENT_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT_LIST), Kernel.Domain.RightType.VIEW);
            SettlementAdvisementMenu.Items.Add(NewSettlementAdvisementMenu);
            SettlementAdvisementMenu.Items.Add(ListSettlementAdvisementMenu);

            ReviewMenu = BuildMenu(BfcFunctionalitiesCode.REVIEW, BfcFunctionalitiesLabel.REVIEW_LABEL, BfcFunctionalitiesCode.ADVISEMENT);
            ReviewPFAccountMenu = BuildMenu(BfcFunctionalitiesCode.REVIEW, BfcFunctionalitiesLabel.REVIEW_PF_ACCOUNT_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.REVIEW_PF_ACCOUNT));
            ReviewSettlementEvolutionMenu = BuildMenu(BfcFunctionalitiesCode.REVIEW, BfcFunctionalitiesLabel.REVIEW_SETTLEMENT_EVOLUTION_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.REVIEW_SETTLEMENT_EVOLUTION));
            ReviewAgeingBalanceMenu = BuildMenu(BfcFunctionalitiesCode.REVIEW, BfcFunctionalitiesLabel.REVIEW_AGEING_BALANCE_LABEL, NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.REVIEW_AGEING_BALANCE));
            ReviewMenu.Items.Add(ReviewPFAccountMenu);
            ReviewMenu.Items.Add(ReviewSettlementEvolutionMenu);
            ReviewMenu.Items.Add(ReviewAgeingBalanceMenu);
        }
    }
}
