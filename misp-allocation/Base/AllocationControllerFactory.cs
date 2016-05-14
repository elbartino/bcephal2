using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Allocation.Clear;
using Misp.Allocation.Run;
using Misp.Allocation.Audit;

namespace Misp.Allocation.Base
{
    public class AllocationControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of SourcingControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public AllocationControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new AllocationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality)
        {
            if (fonctionality == FunctionalitiesCode.CLEAR_ALL_ALLOCATION_FUNCTIONALITY)
            {
                ClearAllocationController controller = new ClearAllocationController();
                controller.ModuleName = Misp.Allocation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((AllocationServiceFactory)ServiceFactory).GetClearAllocationService();
                return controller;
            }
            if (fonctionality == FunctionalitiesCode.RUN_ALL_ALLOCATION_FUNCTIONALITY)
            {
                RunAllAllocationsController controller = new RunAllAllocationsController();
                controller.ModuleName = Misp.Allocation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((AllocationServiceFactory)ServiceFactory).GetAllocationService();
                return controller;
            }
            if (fonctionality == FunctionalitiesCode.ALLOCATION_LOG_FUNCTIONALITY)
            {
                AllocationLogController controller = new AllocationLogController();
                controller.ModuleName = Misp.Allocation.PlugIn.MODULE_NAME;
                controller.Functionality = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((AllocationServiceFactory)ServiceFactory).GetAllocationLogService();
                return controller;
            }
            return null;
        }

    }
}
