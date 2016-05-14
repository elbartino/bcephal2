using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Allocation.Clear;
using Misp.Allocation.Run;
using Misp.Allocation.Audit;

namespace Misp.Allocation.Base
{
    public class AllocationServiceFactory : ServiceFactory
    {

        private ClearAllocationService clearAllocationService;

        private AllocationService allocationService;
        

        /// <summary>
        /// Build a new instance of AllocationServiceFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AllocationServiceFactory(ApplicationManager applicationManager)
            : base(applicationManager) { }


        /// <summary>
        /// Gets ClearAllocationService
        /// </summary>
        public ClearAllocationService GetClearAllocationService()
        {
            if (clearAllocationService == null)
            {
                clearAllocationService = new ClearAllocationService();
                clearAllocationService.ResourcePath = AllocationResourcePath.ALLOCATION_RESOURCE_PATH;
                clearAllocationService.SocketResourcePath = AllocationResourcePath.SOCKET_ALLOCATION_RESOURCE_PATH;
                clearAllocationService.ClearAllSocketResourcePath = AllocationResourcePath.SOCKET_CLEAR_ALL_ALLOCATION_RESOURCE_PATH;
                clearAllocationService.FileService = GetFileService();
                clearAllocationService.GroupService = GetGroupService();
                configureService(clearAllocationService);
            }
            return clearAllocationService;
        }

        /// <summary>
        /// Gets ClearAllocationService
        /// </summary>
        public AllocationService GetAllocationService()
        {
            if (allocationService == null)
            {
                allocationService = new AllocationService();
                allocationService.ResourcePath = AllocationResourcePath.ALLOCATION_RESOURCE_PATH;
                allocationService.SocketResourcePath = AllocationResourcePath.SOCKET_ALLOCATION_RESOURCE_PATH;
                allocationService.FileService = GetFileService();
                allocationService.GroupService = GetGroupService();
                configureService(allocationService);
            }
            return allocationService;
        }

    }
}
