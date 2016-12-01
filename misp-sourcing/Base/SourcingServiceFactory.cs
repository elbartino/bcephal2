using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Sourcing.Table;
using Misp.Kernel.Service;

namespace Misp.Sourcing.Base
{
    public class SourcingServiceFactory : ServiceFactory
    {

        private InputTableService inputTableService;
        private AutomaticSourcingService automaticSourcingService;
        private AutomaticSourcingGridService automaticSourcingGridService;
        private AutomaticTargetService automaticTargetService;
        private UploadMultipleFilesService uploadMultipleFilesService;
        private AutomaticEnrichmentTableService automaticEnrichmentTableService;

        
        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public SourcingServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public InputTableService GetInputTableService()
        {
            if (inputTableService == null)
            {
                inputTableService = new InputTableService();
                inputTableService.ResourcePath = SourcingResourcePath.INPUT_TABLE_RESOURCE_PATH;
                inputTableService.SocketResourcePath = SourcingResourcePath.SOCKET_INPUT_TABLE_RESOURCE_PATH;
                inputTableService.FileService = GetFileService();
                inputTableService.GroupService = GetGroupService();
                inputTableService.MeasureService = GetMeasureService();
                inputTableService.ModelService = GetModelService();
                inputTableService.PeriodicityService = GetPeriodicityService();
                inputTableService.DesignService = GetDesignService();
                inputTableService.AuditService = GetAuditAllocationService();
                inputTableService.TargetService = GetTargetService();
                inputTableService.PeriodNameService = GetPeriodNameService();
                inputTableService.TransformationTreeService = GetTransformationTreeService();
                configureService(inputTableService);
            }
            return inputTableService;
        }

        public AutomaticSourcingService GetAutomaticSourcingService()
        {
            if (automaticSourcingService == null)
            {
                automaticSourcingService = new AutomaticSourcingService();
                automaticSourcingService.ResourcePath = ResourcePath.AUTOMATIC_SOURCING_RESOURCE_PATH;
                automaticSourcingService.SocketResourcePath = ResourcePath.SOCKET_AUTOMATIC_SOURCING_RESOURCE_PATH;
                automaticSourcingService.FileService = GetFileService();
                automaticSourcingService.ModelService = GetModelService();
                automaticSourcingService.MeasureService = GetMeasureService();
                automaticSourcingService.PeriodicityService = GetPeriodicityService();
                automaticSourcingService.GroupService = GetGroupService();
                automaticSourcingService.InputTableService = GetInputTableService();
                automaticSourcingService.CalculatedMeasureService = GetCalculatedMeasureService2();
                automaticSourcingService.PeriodNameService = GetPeriodNameService();
                automaticSourcingService.InputGridService = GetInputGridService();
                automaticSourcingService.TargetService = GetTargetService();
                configureService(automaticSourcingService);
            }
            return automaticSourcingService;
        }

        public AutomaticTargetService GetAutomaticTargetService()
        {
            if (automaticTargetService == null)
            {
                automaticTargetService = new AutomaticTargetService();
                automaticTargetService.ResourcePath = ResourcePath.AUTOMATIC_TARGET_RESOURCE_PATH;
                automaticTargetService.SocketResourcePath = ResourcePath.SOCKET_AUTOMATIC_SOURCING_RESOURCE_PATH;
                automaticTargetService.FileService = GetFileService();
                automaticTargetService.ModelService = GetModelService();
                automaticTargetService.MeasureService = GetMeasureService();
                automaticTargetService.PeriodicityService = GetPeriodicityService();
                automaticTargetService.GroupService = GetGroupService();
                automaticTargetService.InputTableService = GetInputTableService();
                automaticTargetService.CalculatedMeasureService = GetCalculatedMeasureService2();
                automaticTargetService.PeriodNameService = GetPeriodNameService();
                automaticTargetService.TargetService = GetTargetService();
                configureService(automaticTargetService);
            }
            return automaticTargetService;
        }

        public UploadMultipleFilesService GetUploadMultipleFilesService()
        {
            if (uploadMultipleFilesService == null)
            {
                uploadMultipleFilesService = new UploadMultipleFilesService();
                uploadMultipleFilesService.ResourcePath = ResourcePath.UPLOAD_MULTIPE_FILES_RESOURCE_PATH;
                uploadMultipleFilesService.FileService = GetFileService();
                uploadMultipleFilesService.GroupService = GetGroupService();
                configureService(uploadMultipleFilesService);
            }
            return uploadMultipleFilesService;
        }

        public AutomaticSourcingGridService GetAutomaticSourcingGridService()
        {
            if (automaticSourcingGridService == null)
            {
                automaticSourcingGridService = new AutomaticSourcingGridService();
                automaticSourcingGridService.ResourcePath = ResourcePath.AUTOMATIC_SOURCING_GRID_RESOURCE_PATH;
                automaticSourcingGridService.SocketResourcePath = ResourcePath.SOCKET_AUTOMATIC_SOURCING_GRID_RESOURCE_PATH;
                automaticSourcingGridService.FileService = GetFileService();
                automaticSourcingGridService.ModelService = GetModelService();
                automaticSourcingGridService.MeasureService = GetMeasureService();
                automaticSourcingGridService.PeriodicityService = GetPeriodicityService();
                automaticSourcingGridService.GroupService = GetGroupService();
                automaticSourcingGridService.InputTableService = GetInputTableService();
                automaticSourcingGridService.CalculatedMeasureService = GetCalculatedMeasureService2();
                automaticSourcingGridService.PeriodNameService = GetPeriodNameService();
                configureService(automaticSourcingGridService);
            }
            return automaticSourcingGridService;
        }

        public AutomaticEnrichmentTableService GetAutomaticEnrichmentTableService()
        {
            if (automaticEnrichmentTableService == null)
            {
                automaticEnrichmentTableService = new AutomaticEnrichmentTableService();
                automaticEnrichmentTableService.ResourcePath = ResourcePath.AUTOMATIC_ENRICHMENT_TABLE_RESOURCE_PATH;
                automaticEnrichmentTableService.SocketResourcePath = ResourcePath.SOCKET_AUTOMATIC_ENRICHMENT_TABLE_RESOURCE_PATH;
                automaticEnrichmentTableService.FileService = GetFileService();
                automaticEnrichmentTableService.ModelService = GetModelService();
                automaticEnrichmentTableService.MeasureService = GetMeasureService();
                automaticEnrichmentTableService.PeriodicityService = GetPeriodicityService();
                automaticEnrichmentTableService.GroupService = GetGroupService();
                automaticEnrichmentTableService.InputTableService = GetInputTableService();
                automaticEnrichmentTableService.CalculatedMeasureService = GetCalculatedMeasureService2();
                automaticEnrichmentTableService.PeriodNameService = GetPeriodNameService();
                configureService(automaticEnrichmentTableService);
            }
            return automaticEnrichmentTableService;
        }

        
    }
}
