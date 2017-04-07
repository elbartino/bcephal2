using Misp.Bfc.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Plugin;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Ui.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc
{
    public class PlugIn : AbstractPlugin
    {

        public static string MODULE_NAME = "Advisement";
        public static int BFC_PRIORITY = 7;

        /// <summary>
        /// Le nom du plugin
        /// </summary>
        /// <returns></returns>
        protected override string GetPluinName() { return MODULE_NAME; }

        /// <summary>
        /// La priorité du pluin
        /// </summary>
        /// <returns></returns>
        protected override int GetPluinPriority() { return BFC_PRIORITY; }

        /// <summary>
        /// Les menus du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<ApplicationMenu> GetPluinMenus()
        {
            List<ApplicationMenu> menus = new List<ApplicationMenu>(0);
            menus.Add(new AdvisementMenu());
            return menus;
        }

        /// <summary>
        /// Les fonctionalites du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<Functionality> GetPluinFunctionalities()
        {
            List<Functionality> functionalities = new List<Functionality>(0);
            functionalities.Add(new BfcFunctionality());
            return functionalities;
        }

        /// <summary>
        /// Les DashboardCategories du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<NavDashboardCategory> GetNavDashboardCategories()
        {
            List<NavDashboardCategory> categories = new List<NavDashboardCategory>(0);
            categories.Add(BuildCategory("Daily Controls", BfcFunctionalitiesCode.REVIEW));

            NavDashboardCategory newAdvisementCategory = BuildCategory("New Advisement", BfcFunctionalitiesCode.ADVISEMENT);
            newAdvisementCategory.Block = BuildBlock("New Advisement");
            newAdvisementCategory.Block.Children.Add(BuildBlock("New Prefunding Advisement", NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT)));
            newAdvisementCategory.Block.Children.Add(BuildBlock("New Memeber Advisement", NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.MEMBER_ADVISEMENT)));
            newAdvisementCategory.Block.Children.Add(BuildBlock("New Replenishment Instruction", NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT)));
            newAdvisementCategory.Block.Children.Add(BuildBlock("New Settlement Advisement", NavigationToken.GetCreateViewToken(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT)));
            categories.Add(newAdvisementCategory);

            categories.Add(BuildCategory("PF Account Review", BfcFunctionalitiesCode.REVIEW));
            categories.Add(BuildCategory("Settlement Evolution", BfcFunctionalitiesCode.REVIEW));
            categories.Add(BuildCategory("Ageing Balance", BfcFunctionalitiesCode.REVIEW));
            categories.Add(BuildCategory("Bank Account", BfcFunctionalitiesCode.REVIEW));

            NavDashboardCategory listAdvisementCategory = BuildCategory("List Advisements", BfcFunctionalitiesCode.ADVISEMENT);
            listAdvisementCategory.Block = BuildBlock("List Advisement");
            listAdvisementCategory.Block.Children.Add(BuildBlock("List Prefunding Advisements", NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT_LIST)));
            listAdvisementCategory.Block.Children.Add(BuildBlock("List Memeber Advisements", NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.MEMBER_ADVISEMENT_LIST)));
            listAdvisementCategory.Block.Children.Add(BuildBlock("List Replenishment Instructions", NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT_LIST)));
            listAdvisementCategory.Block.Children.Add(BuildBlock("List Settlement Advisements", NavigationToken.GetSearchViewToken(BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT_LIST)));
            categories.Add(listAdvisementCategory);

            return categories;
        }

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected override ControllerFactory GetPluinControllerFactory() { return new BfcControllerFactory(ApplicationManager.Instance); }

    }
}
