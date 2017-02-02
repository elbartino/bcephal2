using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Sourcing.Table;
using Misp.Reporting.Report;
using Misp.Reporting.Calculated_Measure;
using Misp.Reporting.StructuredReport;
using Misp.Sourcing.InputGrid;
using Misp.Reporting.ReportGrid;

namespace Misp.Reporting.Base
{
    public class ReportingControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of SourcingControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReportingControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new ReportingServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {
            if (fonctionality == ReportingFunctionalitiesCode.REPORT_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                ReportBrowserController reportController = new ReportBrowserController();
                reportController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                reportController.FunctionalityCode = fonctionality;
                reportController.ApplicationManager = this.ApplicationManager;
                reportController.Service = ((ReportingServiceFactory)ServiceFactory).GetReportService();
                return reportController;
            } 
            if (fonctionality == ReportingFunctionalitiesCode.REPORT_EDIT && editionMode.HasValue)
            {
                ReportEditorController reportController = new ReportEditorController();
                reportController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                reportController.FunctionalityCode = fonctionality;
                reportController.ApplicationManager = this.ApplicationManager;
                reportController.Service = ((ReportingServiceFactory)ServiceFactory).GetReportService();
                return reportController;
            }

            if (fonctionality == ReportingFunctionalitiesCode.STRUCTURED_REPORT_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                StructuredReportBrowserController reportController = new StructuredReportBrowserController();
                reportController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                reportController.FunctionalityCode = fonctionality;
                reportController.ApplicationManager = this.ApplicationManager;
                reportController.Service = ((ReportingServiceFactory)ServiceFactory).GetStructuredReportService();
                return reportController;
            }
            if (fonctionality == ReportingFunctionalitiesCode.STRUCTURED_REPORT_EDIT && editionMode.HasValue)
            {
                StructuredReportEditorController reportController = new StructuredReportEditorController();
                reportController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                reportController.FunctionalityCode = fonctionality;
                reportController.ApplicationManager = this.ApplicationManager;
                reportController.Service = ((ReportingServiceFactory)ServiceFactory).GetStructuredReportService();
                return reportController;
            }

            if (fonctionality == ReportingFunctionalitiesCode.CALCULATED_MEASURE_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                CalculatedMeasureBrowserController calculatedMeasureBrowserController = new CalculatedMeasureBrowserController();
                calculatedMeasureBrowserController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                calculatedMeasureBrowserController.FunctionalityCode = fonctionality;
                calculatedMeasureBrowserController.ApplicationManager = this.ApplicationManager;
                calculatedMeasureBrowserController.Service = ((ReportingServiceFactory)ServiceFactory).GetCalculatedMeasureService2();
                return calculatedMeasureBrowserController;
            }
            if (fonctionality == ReportingFunctionalitiesCode.CALCULATED_MEASURE_EDIT && editionMode.HasValue)
            {
                CalculatedMeasureEditorController calculatedMeasureEditorController = new CalculatedMeasureEditorController();
                calculatedMeasureEditorController.ModuleName = Misp.Reporting.PlugIn.MODULE_NAME;
                calculatedMeasureEditorController.FunctionalityCode = fonctionality;
                calculatedMeasureEditorController.ApplicationManager = this.ApplicationManager;
                calculatedMeasureEditorController.Service = ((ReportingServiceFactory)ServiceFactory).GetCalculatedMeasureService2();
                return calculatedMeasureEditorController;
            }
            
            if (fonctionality == ReportingFunctionalitiesCode.REPORT_GRID_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                ReportGridBrowserController controller = new ReportGridBrowserController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReportingServiceFactory)ServiceFactory).GetReportGridService();
                return controller;
            }
            if (fonctionality == ReportingFunctionalitiesCode.REPORT_GRID_EDIT && editionMode.HasValue)
            {
                ReportGridEditorController controller = new ReportGridEditorController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReportingServiceFactory)ServiceFactory).GetReportGridService();
                return controller;
            }

            return null;
        }

    }
}
