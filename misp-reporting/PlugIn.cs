﻿using System;
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
using Misp.Kernel.Ui.Dashboard;
using Misp.Reporting.Dashboard;


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
            functionalities.Add(new ReportingFunctionality());
            return functionalities;
        }

        /// <summary>
        /// Les DashboardCategories du plugin
        /// </summary>
        /// <returns></returns>
        protected override List<NavCategory> GetNavDashboardCategories()
        {
            List<NavCategory> categories = new List<NavCategory>(0);
            NavCategory reportCategory = BuildCategory("Daily Controls", FunctionalitiesCode.REPORT);
            reportCategory.Block = BuildBlock("Daily Controls", NavigationToken.GetSearchViewToken(FunctionalitiesCode.REPORT));
            reportCategory.Block = new ReportBlock();
            categories.Add(reportCategory);
            return categories;
        }

        /// <summary>
        /// Le ControllerFactory du plugin
        /// </summary>
        /// <returns></returns>
        protected override ControllerFactory GetPluinControllerFactory() { return new ReportingControllerFactory(ApplicationManager.Instance); }

    }
}
