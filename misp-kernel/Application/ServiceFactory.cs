using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Service;
using RestSharp;
using Misp.Kernel.Administration.Base;


namespace Misp.Kernel.Application
{
    public class ServiceFactory
    {

        private FileService fileService;
        private GroupService groupService;
        private PeriodNameService periodNameService;
        private InitiationService initiationService;
        private ModelService modelService;
        private PeriodicityService periodicityService;
        private MeasureService measureService;
        private DashBoardService dashBoardService;
        private TargetService targetService;
        private DesignService designService;
        private AuditService auditAllocationService;
        private AuditService auditReportService;
        private AllocationLogService allocationLogService;
        private AutomaticSourcingService automaticSourcingService;
        private CalculatedMeasureService calculatedMeasureService;
        private ReconciliationService reconciliationService;
        private PostingService postingService;

        private SecurityService securityService;
        private RoleService roleService;

        /// <summary>
        /// Build a new instance of ServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ServiceFactory(ApplicationManager applicationManager)
        {
            this.ApplicationManager = applicationManager;
        }

        /// <summary>
        /// Gets or sets the ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public SecurityService GetSecurityService()
        {
            if (securityService == null)
            {
                securityService = new SecurityService();
                securityService.ResourcePath = AdministrationResourcePath.SECURITY_RESOURCE_PATH;
                securityService.GroupService = GetGroupService();
                securityService.FileService = GetFileService();
                configureService(securityService);
            }
            return securityService;
        }

        /// <summary>
        /// Gets or sets FileInfoService
        /// </summary>
        public FileService GetFileService() 
        {
            if (fileService == null)
            {
                fileService = new FileService();
                fileService.ResourcePath = ResourcePath.FILE_RESOURCE_PATH;
                fileService.FileService = fileService;
                fileService.DashBoardService = GetDashBoardService();
                configureService(fileService);
            }
            return fileService;
        }
        
        /// <summary>
        /// Gets PeriodNameService
        /// </summary>
        public PeriodNameService GetPeriodNameService()
        {
            if (periodNameService == null)
            {
                periodNameService = new PeriodNameService();
                periodNameService.ResourcePath = ResourcePath.PERIOD_NAME_RESOURCE_PATH;
                periodNameService.FileService = GetFileService();
                configureService(periodNameService);
            }
            return periodNameService;
        }
        
        /// <summary>
        /// Gets or sets GroupService
        /// </summary>
        public GroupService GetGroupService()
        {
            if (groupService == null)
            {
                groupService = new GroupService();
                groupService.ResourcePath = ResourcePath.GROUP_RESOURCE_PATH;
                groupService.FileService = GetFileService();
                configureService(groupService);
            }
            return groupService;
        }

        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public InitiationService GetInitiationService()
        {
            if (initiationService == null)
            {
                initiationService = new InitiationService();
                initiationService.ResourcePath = ResourcePath.INITIATION_RESOURCE_PATH;
                configureService(initiationService);
                initiationService.ModelService = GetModelService();
                initiationService.MeasureService = GetMeasureService();
                initiationService.PeriodicityService = GetPeriodNameService();
                initiationService.FileService = GetFileService();
            }
            return initiationService;
        }


        /// <summary>
        /// Gets InputTableService
        /// </summary>
        public DashBoardService GetDashBoardService()
        {
            if (dashBoardService == null)
            {
                dashBoardService = new DashBoardService();
                dashBoardService.ResourcePath = ResourcePath.DASHBOARD_RESOURCE_PATH;
                configureService(dashBoardService);
            }
            return dashBoardService;
        }

        /// <summary>
        /// Gets ModelService
        /// </summary>
        public ModelService GetModelService()
        {
            if (modelService == null)
            {
                modelService = new ModelService();
                modelService.ResourcePath = ResourcePath.MODEL_RESOURCE_PATH;
                configureService(modelService);
            }
            return modelService;
        }

        /// <summary>
        /// Gets TargetService
        /// </summary>
        public TargetService GetTargetService()
        {
            if (targetService == null)
            {
                targetService = new TargetService();
                targetService.ResourcePath = ResourcePath.TARGET_RESOURCE_PATH;
                targetService.FileService = GetFileService();
                targetService.ModelService = GetModelService();
                targetService.GroupService = GetGroupService();
                configureService(targetService);
            }
            return targetService;
        }


        /// <summary>
        /// Gets PeriodicityService
        /// </summary>
        public PeriodicityService GetPeriodicityService()
        {
            if (periodicityService == null)
            {
                periodicityService = new PeriodicityService();
                periodicityService.ResourcePath = ResourcePath.PERIODICITY_RESOURCE_PATH;
                configureService(periodicityService);
            }
            return periodicityService;
        }

        /// <summary>
        /// Gets MeasureService
        /// </summary>
        public MeasureService GetMeasureService()
        {
            if (measureService == null)
            {
                measureService = new MeasureService();
                measureService.ResourcePath = ResourcePath.MEASURE_RESOURCE_PATH;
                configureService(measureService);
            }
            return measureService;
        }

        /// <summary>
        /// Gets MeasureService
        /// </summary>
        public RoleService GetRoleService()
        {
            if (roleService == null)
            {
                roleService = new RoleService();
                roleService.ResourcePath = ResourcePath.SECURITY_ROLE_RESOURCE_PATH;
                configureService(roleService);
            }
            return roleService;
        }

        /// <summary>
        /// Gets ReconciliationService
        /// </summary>
        public ReconciliationService GetReconciliationService()
        {
            if (reconciliationService == null)
            {
                reconciliationService = new ReconciliationService();
                reconciliationService.ResourcePath = ResourcePath.RECONCILIATION_RESOURCE_PATH;
                reconciliationService.measureService = GetMeasureService();
                reconciliationService.ModelService = GetModelService();
                reconciliationService.periodNameService = GetPeriodNameService();
                reconciliationService.postingService = GetPostingService();
                configureService(reconciliationService);
            }
            return reconciliationService;
        }
        
        /// <summary>
        /// Gets PostingService
        /// </summary>
        public PostingService GetPostingService()
        {
            if (postingService == null)
            {
                postingService = new PostingService();
                postingService.ResourcePath = ResourcePath.RECONCILIATON_POSTING_RESOURCE_PATH;
                postingService.GroupService = GetGroupService();
                postingService.FileService = GetFileService();
                postingService.ModelService = GetModelService();
                postingService.periodNameService = GetPeriodNameService();
                postingService.measureService = GetMeasureService();
                configureService(postingService);
            }
            return postingService;
        }

        /// <summary>
        /// Gets DesignService
        /// </summary>
        public DesignService GetDesignService()
        {
            if (designService == null)
            {
                designService = new DesignService();
                designService.ResourcePath = ResourcePath.DESIGN_RESOURCE_PATH;
                designService.FileService = GetFileService();
                designService.ModelService = GetModelService();
                designService.MeasureService = GetMeasureService();
                designService.PeriodicityService = GetPeriodicityService();
                designService.GroupService = GetGroupService();
                designService.CalculatedMeasureService = GetCalculatedMeasureService2();
                designService.TargetService = GetTargetService();
                designService.PeriodNameService = GetPeriodNameService();
                configureService(designService);
            }
            return designService;
        }

        /// <summary>
        /// Gets or sets AuditAllocationService
        /// </summary>
        public AuditService GetAuditAllocationService()
        {
            if (auditAllocationService == null)
            {
                auditAllocationService = new AuditService();
                auditAllocationService.ResourcePath = ResourcePath.AUDIT_ALLOCATION_RESOURCE_PATH;
                auditAllocationService.FileService = GetFileService();
                configureService(auditAllocationService);
            }
            return auditAllocationService;
        }

        /// <summary>
        /// Gets or sets auditReportService
        /// </summary>
        public AuditService GetAuditReportService()
        {
            if (auditReportService == null)
            {
                auditReportService = new AuditService();
                auditReportService.ResourcePath = ResourcePath.AUDIT_REPORT_RESOURCE_PATH;
                auditReportService.FileService = GetFileService();
                configureService(auditReportService);
            }
            return auditReportService;
        }

        /// <summary>
        /// Gets AllocationLogService
        /// </summary>
        public AllocationLogService GetAllocationLogService()
        {
            if (allocationLogService == null)
            {
                allocationLogService = new AllocationLogService();
                allocationLogService.ResourcePath = ResourcePath.ALLOCATION_LOG_RESOURCE_PATH;
                allocationLogService.FileService = GetFileService();
                configureService(allocationLogService);
            }
            return allocationLogService;
        }

        /// <summary>
        /// Gets or sets calculatedMeasureService
        /// </summary>
        public CalculatedMeasureService GetCalculatedMeasureService2()
        {
            if (calculatedMeasureService == null)
            {
                calculatedMeasureService = new CalculatedMeasureService();
                calculatedMeasureService.ResourcePath = ResourcePath.CALCULATED_MEASURE_RESOURCE_PATH;
                calculatedMeasureService.MeasureService = GetMeasureService();
                calculatedMeasureService.GroupService = GetGroupService();
                calculatedMeasureService.FileService = GetFileService();
                calculatedMeasureService.ModelService = GetModelService();
                calculatedMeasureService.PeriodicityService = GetPeriodicityService();
                calculatedMeasureService.DesignService = GetDesignService();
                calculatedMeasureService.AuditService = GetAuditReportService();
                configureService(calculatedMeasureService);
            }
            return calculatedMeasureService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        protected void configureService(IService service)
        {
            service.RestClient = ApplicationManager.RestClient;
        }



       
    }
}
