using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Plugin;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.Base;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;


namespace Misp.Reporting
{
    public class PlugIn : AbstractPlugin
    {

        public static string MODULE_NAME = "Reporting";
        public static int REPORTING_PRIORITY = 4;

        /// <summary>
        /// Le nom du plugin
        /// </summary>
        /// <returns></returns>
        protected override string GetPluinName() { return MODULE_NAME; }

        /// <summary>
        /// La priorité du pluin
        /// </summary>
        /// <returns></returns>
        protected override int GetPluinPriority() { return REPORTING_PRIORITY; }

        /// <summary>
        /// Les menus du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<ApplicationMenu> GetPluinMenus()
        {
            List<ApplicationMenu> menus = new List<ApplicationMenu>(0);
            menus.Add(new ReportingMenu());
            return menus;
        }

        /// <summary>
        /// Les fonctionalites du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<Functionality> GetPluinFunctionalities()
        {
            List<Functionality> functionalities = new List<Functionality>(0);
            //functionalities.Add();
            return functionalities;
        }

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected override ControllerFactory GetPluinControllerFactory() { return new ReportingControllerFactory(ApplicationManager.Instance); }

    }
}
