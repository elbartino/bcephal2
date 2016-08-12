using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Sourcing.Table;
using Misp.Kernel.Service;
using Misp.Kernel.Administration.Base;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationServiceFactory : ServiceFactory
    {

        private ReconciliationService reconciliationService;
        private PostingService postingService;
        private AutomaticPostingGridService automaticPostingGridService;

        /// <summary>
        /// Build a new instance of InitiationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReconciliationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets InitiationService
        /// </summary>
        public ReconciliationService GetReconciliationService()
        {
            if (reconciliationService == null)
            {
                reconciliationService = new ReconciliationService();
                reconciliationService.ResourcePath = ReconciliationResourcePath.RECONCILIATION_RESOURCE_PATH;
                reconciliationService.GroupService = GetGroupService();
                reconciliationService.FileService = GetFileService();
                reconciliationService.ModelService = GetModelService();
                reconciliationService.periodNameService = GetPeriodNameService();
                reconciliationService.measureService = GetMeasureService();
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
                postingService.ResourcePath = ReconciliationResourcePath.RECONCILIATON_POSTING_RESOURCE_PATH;
                postingService.GroupService = GetGroupService();
                postingService.FileService = GetFileService();
                postingService.ModelService = GetModelService();
                postingService.periodNameService = GetPeriodNameService();
                postingService.measureService = GetMeasureService();
                configureService(postingService);
            }
            return postingService;
        }

        public AutomaticPostingGridService GetAutomaticPostingGridService()
        {
            if (automaticPostingGridService == null)
            {
                automaticPostingGridService = new AutomaticPostingGridService();
                automaticPostingGridService.ResourcePath = ResourcePath.AUTOMATIC_POSTING_GRID_RESOURCE_PATH;
                automaticPostingGridService.SocketResourcePath = ResourcePath.SOCKET_AUTOMATIC_POSTING_GRID_RESOURCE_PATH;
                automaticPostingGridService.FileService = GetFileService();
                automaticPostingGridService.ModelService = GetModelService();
                automaticPostingGridService.MeasureService = GetMeasureService();
                automaticPostingGridService.PeriodicityService = GetPeriodicityService();
                automaticPostingGridService.GroupService = GetGroupService();
                //automaticPostingGridService.InputTableService = GetInputTableService();
                automaticPostingGridService.CalculatedMeasureService = GetCalculatedMeasureService2();
                automaticPostingGridService.PeriodNameService = GetPeriodNameService();
                configureService(automaticPostingGridService);
            }
            return automaticPostingGridService;
        }

       
    }
}
