using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Reporting.Base
{
    public class ReportingMenu : ApplicationMenu
    {

        private ApplicationMenu newReportMenu;
        private ApplicationMenu listReportMenu;
        private ApplicationMenu newStructuredReportMenu;
        private ApplicationMenu listStructuredReportMenu;
        private ApplicationMenu calculatedMeasureMenu;
        private ApplicationMenu listCalculatedMeasureMenu;
        private ApplicationMenu exportBudgetMenu;

        public ApplicationMenu NewReportMenu { get { return newReportMenu; } }
        public ApplicationMenu ListReportMenu { get { return listReportMenu; } }

        public ApplicationMenu NewStructuredReportMenu { get { return newStructuredReportMenu; } }
        public ApplicationMenu ListStructuredReportMenu { get { return listStructuredReportMenu; } }

        public ApplicationMenu CalculatedMeasureMenu { get { return calculatedMeasureMenu; } }
        public ApplicationMenu ListCalculatedMeasureMenu { get { return listCalculatedMeasureMenu; } }
        public ApplicationMenu ExportBudgetMenu { get { return exportBudgetMenu; } }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(NewReportMenu);
            menus.Add(ListReportMenu);
            menus.Add(new Separator());
            menus.Add(NewStructuredReportMenu);
            menus.Add(ListStructuredReportMenu);
            menus.Add(new Separator());
            menus.Add(calculatedMeasureMenu);
            menus.Add(listCalculatedMeasureMenu);
          /*  menus.Add(new Separator());
            menus.Add(exportBudgetMenu);*/
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.REPORTING_MENU_CODE;
            this.Header = "Reporting";
            newReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "New Report", NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_REPORT_FUNCTIONALITY));
            listReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "List Reports", NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_REPORT_FUNCTIONALITY));
            newStructuredReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "New Structured Report", NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_STRUCTURED_REPORT_FUNCTIONALITY));
            listStructuredReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "List Structured Reports", NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_STRUCTURED_REPORT_FUNCTIONALITY));
            calculatedMeasureMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "New Calculated Measure", NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_CALCULATED_MEASURE_FUNCTIONALITY));
            listCalculatedMeasureMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "List Calculated Measure", NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_CALCULATED_MEASURE_FUNCTIONALITY));
            exportBudgetMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, "Export Budget", NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.EXPORT_BUDGET));
        }

    }
}
