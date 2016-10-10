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
        private ApplicationMenu newPivotTableMenu;
        private ApplicationMenu listPivotTableMenu;
        public ApplicationMenu NewReportMenu { get { return newReportMenu; } }
        public ApplicationMenu ListReportMenu { get { return listReportMenu; } }

        public ApplicationMenu NewStructuredReportMenu { get { return newStructuredReportMenu; } }
        public ApplicationMenu ListStructuredReportMenu { get { return listStructuredReportMenu; } }

        public ApplicationMenu CalculatedMeasureMenu { get { return calculatedMeasureMenu; } }
        public ApplicationMenu ListCalculatedMeasureMenu { get { return listCalculatedMeasureMenu; } }

        public ApplicationMenu NewPivotTableMenu { get { return newPivotTableMenu; } }
        public ApplicationMenu ListPivotTableMenu { get { return listPivotTableMenu; } }
        
        public ApplicationMenu NewReportGridMenu { get; private set; }
        public ApplicationMenu ListReportGridMenu { get; private set; }
        public ApplicationMenu GridGroupMenu { get; private set; }
        public ApplicationMenu ReportGroupMenu { get; private set; }
        public ApplicationMenu StructuredReportGrouMenu { get; private set; }
        public ApplicationMenu PivotTableGroupMenu { get; private set; }
        public ApplicationMenu CalculatedMeasureGroupMenu { get; private set; }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            ReportGroupMenu.Items.Add(NewReportMenu);
            ReportGroupMenu.Items.Add(ListReportMenu);
            menus.Add(ReportGroupMenu);
            menus.Add(new Separator());
            
            GridGroupMenu.Items.Add(NewReportGridMenu);
            GridGroupMenu.Items.Add(ListReportGridMenu);
            menus.Add(GridGroupMenu);
            menus.Add(new Separator());

            StructuredReportGrouMenu.Items.Add(NewStructuredReportMenu);
            StructuredReportGrouMenu.Items.Add(ListStructuredReportMenu);
            menus.Add(StructuredReportGrouMenu);
            menus.Add(new Separator());

            CalculatedMeasureGroupMenu.Items.Add(calculatedMeasureMenu);
            CalculatedMeasureGroupMenu.Items.Add(listCalculatedMeasureMenu);
            menus.Add(CalculatedMeasureGroupMenu);
            menus.Add(new Separator());

            PivotTableGroupMenu.Items.Add(newPivotTableMenu);
            PivotTableGroupMenu.Items.Add(listPivotTableMenu);
            menus.Add(PivotTableGroupMenu);
            
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
            this.Header = FunctionalitiesLabel.REPORTING_LABEL;
            newReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.NEW_REPORT_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_REPORT_FUNCTIONALITY));
            listReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.LIST_REPORT_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_REPORT_FUNCTIONALITY));
            newStructuredReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE,FunctionalitiesLabel.NEW_STRUCTURED_REPORT_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_STRUCTURED_REPORT_FUNCTIONALITY));
            listStructuredReportMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE,FunctionalitiesLabel.LIST_STRUCTURED_REPORT_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_STRUCTURED_REPORT_FUNCTIONALITY));
            calculatedMeasureMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE,FunctionalitiesLabel.NEW_CALCULATED_MEASURE_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_CALCULATED_MEASURE_FUNCTIONALITY));
            listCalculatedMeasureMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.LIST_CALCULATED_MEASURE_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_CALCULATED_MEASURE_FUNCTIONALITY));
            
            newPivotTableMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.NEW_PIVOT_TABLE_LABEL, null);
            listPivotTableMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.LIST_PIVOT_TABLE_LABEL, null);

            GridGroupMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.GRID_LABEL, null);
            ReportGroupMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.REPORT_LABEL, null);
            StructuredReportGrouMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.STRUCTURED_REPORT_LABEL, null);
            CalculatedMeasureGroupMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.CALCULATED_MEASURE_LABEL, null);
            PivotTableGroupMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.PIVOT_TABLE_LABEL, null);

            NewReportGridMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.NEW_REPORT_GRID_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.NEW_REPORT_GRID_FUNCTIONALITY));
            ListReportGridMenu = BuildMenu(ApplicationMenu.REPORTING_MENU_CODE, FunctionalitiesLabel.LIST_REPORT_GRID_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.LIST_REPORT_GRID_FUNCTIONALITY));

            
            
        }

    }
}
