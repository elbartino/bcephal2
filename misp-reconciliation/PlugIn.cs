using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Plugin;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Ui.Dashboard;
using Misp.Reconciliation.Base;
using Misp.Reconciliation.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation
{
    public class PlugIn : AbstractPlugin
    {

        public static string MODULE_NAME = "Reconciliation";
        public static int RECONCILIATION_PRIORITY = 6;

        /// <summary>
        /// Le nom du plugin
        /// </summary>
        /// <returns></returns>
        protected override string GetPluinName() { return MODULE_NAME; }

        /// <summary>
        /// La priorité du pluin
        /// </summary>
        /// <returns></returns>
        protected override int GetPluinPriority() { return RECONCILIATION_PRIORITY; }

        /// <summary>
        /// Les menus du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<ApplicationMenu> GetPluinMenus()
        {
            List<ApplicationMenu> menus = new List<ApplicationMenu>(0);
            menus.Add(new ReconciliationMenu());
            return menus;
        }

        /// <summary>
        /// Les fonctionalites du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<Functionality> GetPluinFunctionalities()
        {
            List<Functionality> functionalities = new List<Functionality>(0);
            functionalities.Add(new ReconciliationFunctionality());
            return functionalities;
        }

        /// <summary>
        /// Les DashboardCategories du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<NavCategory> GetNavDashboardCategories()
        {
            List<NavCategory> categories = new List<NavCategory>(0);
            NavCategory recoCategory = BuildCategory("Reconciliation", ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER);
            recoCategory.Block = BuildBlock("Reconciliation", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER));
            recoCategory.Block = new ReconciliationBlock();
            categories.Add(recoCategory);
            return categories;
        }

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected override ControllerFactory GetPluinControllerFactory() { return new ReconciliationControllerFactory(ApplicationManager.Instance); }

    }
}
