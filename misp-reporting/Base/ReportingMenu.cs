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
            
            menus.Add(ReportGroupMenu);
            menus.Add(GridGroupMenu);
            menus.Add(StructuredReportGrouMenu);
            menus.Add(CalculatedMeasureGroupMenu);
            menus.Add(PivotTableGroupMenu);
            
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.REPORTING;
            this.Header = FunctionalitiesLabel.REPORTING_LABEL;

            ReportGroupMenu = BuildMenu(FunctionalitiesCode.REPORTING, FunctionalitiesLabel.REPORT_LABEL, FunctionalitiesCode.REPORT);
            newReportMenu = BuildMenu(FunctionalitiesCode.REPORT, FunctionalitiesLabel.NEW_REPORT_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.REPORT_EDIT), Kernel.Domain.RightType.CREATE);
            listReportMenu = BuildMenu(FunctionalitiesCode.REPORT, FunctionalitiesLabel.LIST_REPORT_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.REPORT_LIST), Kernel.Domain.RightType.VIEW);

            StructuredReportGrouMenu = BuildMenu(FunctionalitiesCode.REPORTING, FunctionalitiesLabel.STRUCTURED_REPORT_LABEL, FunctionalitiesCode.STRUCTURED_REPORT);
            newStructuredReportMenu = BuildMenu(FunctionalitiesCode.STRUCTURED_REPORT, FunctionalitiesLabel.NEW_STRUCTURED_REPORT_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.STRUCTURED_REPORT_EDIT), Kernel.Domain.RightType.CREATE);
            listStructuredReportMenu = BuildMenu(FunctionalitiesCode.STRUCTURED_REPORT, FunctionalitiesLabel.LIST_STRUCTURED_REPORT_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.STRUCTURED_REPORT_LIST), Kernel.Domain.RightType.VIEW);

            CalculatedMeasureGroupMenu = BuildMenu(FunctionalitiesCode.REPORTING, FunctionalitiesLabel.CALCULATED_MEASURE_LABEL, FunctionalitiesCode.CALCULATED_MEASURE);
            calculatedMeasureMenu = BuildMenu(FunctionalitiesCode.CALCULATED_MEASURE, FunctionalitiesLabel.NEW_CALCULATED_MEASURE_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.CALCULATED_MEASURE_EDIT), Kernel.Domain.RightType.CREATE);
            listCalculatedMeasureMenu = BuildMenu(FunctionalitiesCode.CALCULATED_MEASURE, FunctionalitiesLabel.LIST_CALCULATED_MEASURE_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.CALCULATED_MEASURE_LIST), Kernel.Domain.RightType.VIEW);

            PivotTableGroupMenu = BuildMenu(FunctionalitiesCode.REPORTING, FunctionalitiesLabel.PIVOT_TABLE_LABEL, FunctionalitiesCode.PIVOT_TABLE);
            newPivotTableMenu = BuildMenu(FunctionalitiesCode.PIVOT_TABLE, FunctionalitiesLabel.NEW_PIVOT_TABLE_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.PIVOT_TABLE_EDIT), Kernel.Domain.RightType.CREATE);
            listPivotTableMenu = BuildMenu(FunctionalitiesCode.PIVOT_TABLE, FunctionalitiesLabel.LIST_PIVOT_TABLE_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.PIVOT_TABLE_LIST), Kernel.Domain.RightType.VIEW);

            GridGroupMenu = BuildMenu(FunctionalitiesCode.REPORTING, FunctionalitiesLabel.GRID_LABEL, FunctionalitiesCode.REPORT_GRID);
            NewReportGridMenu = BuildMenu(FunctionalitiesCode.REPORT_GRID, FunctionalitiesLabel.NEW_REPORT_GRID_LABEL, NavigationToken.GetCreateViewToken(ReportingFunctionalitiesCode.REPORT_GRID_EDIT), Kernel.Domain.RightType.CREATE);
            ListReportGridMenu = BuildMenu(FunctionalitiesCode.REPORT_GRID, FunctionalitiesLabel.LIST_REPORT_GRID_LABEL, NavigationToken.GetSearchViewToken(ReportingFunctionalitiesCode.REPORT_GRID_LIST), Kernel.Domain.RightType.VIEW);
            
            ReportGroupMenu.Items.Add(NewReportMenu);
            ReportGroupMenu.Items.Add(ListReportMenu);

            GridGroupMenu.Items.Add(NewReportGridMenu);
            GridGroupMenu.Items.Add(ListReportGridMenu);

            StructuredReportGrouMenu.Items.Add(NewStructuredReportMenu);
            StructuredReportGrouMenu.Items.Add(ListStructuredReportMenu);

            CalculatedMeasureGroupMenu.Items.Add(calculatedMeasureMenu);
            CalculatedMeasureGroupMenu.Items.Add(listCalculatedMeasureMenu);

            PivotTableGroupMenu.Items.Add(newPivotTableMenu);
            PivotTableGroupMenu.Items.Add(listPivotTableMenu);
            
        }

    }
}
