using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Reporting.Report;
using Misp.Kernel.Service;
using Misp.Reporting.StructuredReport;

namespace Misp.Reporting.Base
{
    public class ReportingServiceFactory : ServiceFactory
    {

        private ReportService reportService;
        private StructuredReportService structuredReportService;
        public CalculatedMeasureService calculatedMeasureService { get; set; }

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReportingServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets ReportService
        /// </summary>
        public ReportService GetReportService()
        {
            if (reportService == null)
            {
                reportService = new ReportService();
                reportService.ResourcePath = ReportingResourcePath.REPORT_RESOURCE_PATH;
                reportService.SocketResourcePath = ReportingResourcePath.SOCKET_REPORT_RESOURCE_PATH;
                reportService.MeasureService = GetMeasureService();
                reportService.GroupService = GetGroupService();
                reportService.FileService = GetFileService();
                reportService.ModelService = GetModelService();
                reportService.PeriodicityService = GetPeriodicityService();
                reportService.DesignService = GetDesignService();
                reportService.AuditService = GetAuditReportService();
                reportService.calculatedMeasureService = GetCalculatedMeasureService2();
                reportService.TargetService = GetTargetService();
                reportService.PeriodNameService = GetPeriodNameService();

                configureService(reportService);
            }
            return reportService;
        }

        /// <summary>
        /// Gets StructuredReportService
        /// </summary>
        public StructuredReportService GetStructuredReportService()
        {
            if (structuredReportService == null)
            {
                structuredReportService = new StructuredReportService();
                structuredReportService.ResourcePath = ReportingResourcePath.STRUCTURED_REPORT_RESOURCE_PATH;
                structuredReportService.SocketResourcePath = ReportingResourcePath.SOCKET_REPORT_RESOURCE_PATH;
                structuredReportService.MeasureService = GetMeasureService();
                structuredReportService.CalculatedMeasureService = GetCalculatedMeasureService2();
                structuredReportService.GroupService = GetGroupService();
                structuredReportService.FileService = GetFileService();
                structuredReportService.ModelService = GetModelService();
                structuredReportService.PeriodicityService = GetPeriodicityService();
                structuredReportService.TargetService = GetTargetService();
                structuredReportService.PeriodNameService = GetPeriodNameService();

                configureService(structuredReportService);
            }
            return structuredReportService;
        }

              
     
    }
}
