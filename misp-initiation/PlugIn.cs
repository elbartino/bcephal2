using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Plugin;
using Misp.Kernel.Ui.Base;
using Misp.Initiation.Base;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Dashboard;


namespace Misp.Initiation
{
    public class PlugIn : AbstractPlugin
    {

        public static string MODULE_NAME = "Initiation";
        public static int INITIATION_PRIORITY = 1;
        
        /// <summary>
        /// Le nom du plugin
        /// </summary>
        /// <returns></returns>
        protected override string GetPluinName() { return MODULE_NAME; }

        /// <summary>
        /// La priorité du pluin
        /// </summary>
        /// <returns></returns>
        protected override int GetPluinPriority() { return INITIATION_PRIORITY; }

        /// <summary>
        /// Les menus du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<ApplicationMenu> GetPluinMenus()
        {
            List<ApplicationMenu> menus = new List<ApplicationMenu>(0);
            menus.Add(new InitiationMenu());
            return menus;
        }

        /// <summary>
        /// Les fonctionalites du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<Functionality> GetPluinFunctionalities()
        {
            List<Functionality> functionalities = new List<Functionality>(0);
            functionalities.Add(new InitiationFunctionality());
            return functionalities;
        }

        /// <summary>
        /// Les DashboardCategories du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<NavDashboardCategory> GetNavDashboardCategories()
        {
            List<NavDashboardCategory> categories = new List<NavDashboardCategory>(0);

            return categories;
        }

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected override ControllerFactory GetPluinControllerFactory() { return new InitiationControllerFactory(ApplicationManager.Instance); }        

    }
}
