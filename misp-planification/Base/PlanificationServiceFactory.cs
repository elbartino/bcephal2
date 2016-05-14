using Misp.Kernel.Application;
using Misp.Kernel.Service;
using Misp.Planification.Tranformation.TransformationTable;
using Misp.Reporting.Base;
using Misp.Reporting.Report;
using Misp.Sourcing.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Base
{
    public class PlanificationServiceFactory : ServiceFactory
    {
        private TransformationTreeService transformationTreeService;

        private TransformationTableService transformationTableService;

        private CombineTransformationTreeService combinedtransformationTreeService;

        private PresentationService presentationService;

        private ReportService reportService;

        public PlanificationServiceFactory(ApplicationManager applicationManager) : base(applicationManager) { }

        /// <summary>
        /// Gets TransformationTreeService
        /// </summary>
        public TransformationTreeService GetTransformationTreeService()
        {
            if (transformationTreeService == null)
            {
                transformationTreeService = new TransformationTreeService();
                transformationTreeService.ResourcePath = PlanificationResourcePath.TRANSFORMATION_TREE_RESOURCE_PATH;
                transformationTreeService.SocketResourcePath = PlanificationResourcePath.SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH;
                transformationTreeService.FileService = GetFileService();
                transformationTreeService.GroupService = GetGroupService();
                transformationTreeService.MeasureService = GetMeasureService();
                transformationTreeService.ModelService = GetModelService();
                transformationTreeService.PeriodicityService = GetPeriodicityService();
                transformationTreeService.DesignService = GetDesignService();
                transformationTreeService.AuditService = GetAuditAllocationService();
                transformationTreeService.TargetService = GetTargetService();
                transformationTreeService.CalculatedMeasureService = GetCalculatedMeasureService2();
                transformationTreeService.PeriodNameService = GetPeriodNameService();
                configureService(transformationTreeService);
            }
            return transformationTreeService;
        }

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
                reportService.TargetService = GetTargetService();
                reportService.PeriodNameService = GetPeriodNameService();

                configureService(reportService);
            }
            return reportService;
        }

        public TransformationTableService GetTransformationTableService()
        {
            if (transformationTableService == null)
            {
                transformationTableService = new TransformationTableService();
                transformationTableService.ResourcePath = PlanificationResourcePath.TRANSFORMATION_TABLE_RESOURCE_PATH;
                transformationTableService.SocketResourcePath = PlanificationResourcePath.TRANSFORMATION_TABLE_RESOURCE_PATH;
                transformationTableService.FileService = GetFileService();
                transformationTableService.GroupService = GetGroupService();
                transformationTableService.MeasureService = GetMeasureService();
                transformationTableService.ModelService = GetModelService();
                transformationTableService.PeriodicityService = GetPeriodicityService();
                transformationTableService.CalculatedMeasureService = GetCalculatedMeasureService2();
                transformationTableService.TargetService = GetTargetService();
                transformationTableService.PeriodNameService = GetPeriodNameService();
                configureService(transformationTableService);
            }
            return transformationTableService;
        }

        public PresentationService GetPresentationService()
        {
            if (presentationService == null)
            {
                presentationService = new PresentationService();
                presentationService.ResourcePath = PlanificationResourcePath.TRANSFORMATION_SLIDE_RESOURCE_PATH;
                presentationService.SocketResourcePath = PlanificationResourcePath.SOCKET_TRANSFORMATION_SLIDE_RESOURCE_PATH;
                presentationService.FileService = GetFileService();
                presentationService.GroupService = GetGroupService();
                presentationService.MeasureService = GetMeasureService();
                presentationService.ModelService = GetModelService();
                presentationService.PeriodicityService = GetPeriodicityService();
                presentationService.TargetService = GetTargetService();
                presentationService.ReportService = GetReportService();
                presentationService.PeriodNameService = GetPeriodNameService();
                configureService(presentationService);
            }
            return presentationService;
        }


        public CombineTransformationTreeService GetCombinedTransformationTreeService()
        {
            if (combinedtransformationTreeService == null)
            {
                combinedtransformationTreeService = new CombineTransformationTreeService();
                combinedtransformationTreeService.ResourcePath = PlanificationResourcePath.TRANSFORMATION_COMBINED_RESOURCE_PATH;
                combinedtransformationTreeService.FileService = GetFileService();
                combinedtransformationTreeService.GroupService = GetGroupService();
                combinedtransformationTreeService.MeasureService = GetMeasureService();
                combinedtransformationTreeService.ModelService = GetModelService();
                combinedtransformationTreeService.PeriodicityService = GetPeriodicityService();
                combinedtransformationTreeService.TargetService = GetTargetService();
                combinedtransformationTreeService.CalculatedMeasureService = GetCalculatedMeasureService2();
                combinedtransformationTreeService.PeriodNameService = GetPeriodNameService();
                combinedtransformationTreeService.TransformationTreeService = GetTransformationTreeService();
                configureService(combinedtransformationTreeService);
            }
            return combinedtransformationTreeService;
        }

    }
}
