using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Initiation.Periodicity;
using Misp.Initiation.Archives;

namespace Misp.Initiation.Base
{
    public class InitiationControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of InitiationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public InitiationControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {            
            this.ServiceFactory = new InitiationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {
            if (fonctionality == InitiationFunctionalitiesCode.INITIATION_MODEL)
            {
                InitiationController initiationController = new InitiationController();
                initiationController.ApplicationManager = this.ApplicationManager;
                initiationController.Service = ((InitiationServiceFactory)ServiceFactory).GetInitiationService();
                return initiationController;
            }

            if (fonctionality == InitiationFunctionalitiesCode.INITIATION_PERIOD)
            {
                PeriodNameController periodNameController = new PeriodNameController();
                periodNameController.ApplicationManager = this.ApplicationManager;
                periodNameController.Service = ((InitiationServiceFactory)ServiceFactory).GetPeriodNameService();
                return periodNameController;
            }

            if (fonctionality == InitiationFunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY)
            {
                ArchiveController archiveController = new ArchiveController();
                archiveController.ApplicationManager = this.ApplicationManager;
                archiveController.isSimpleArchive = true;
                archiveController.fileService = ((InitiationServiceFactory)ServiceFactory).GetFileService();
                return archiveController;
            }

            if (fonctionality == InitiationFunctionalitiesCode.BACKUP_AUTOMATIC_FUNCTIONALITY)
            {
                ArchiveController archiveController = new ArchiveController();
                archiveController.ApplicationManager = this.ApplicationManager;
                archiveController.isSimpleArchive = false;
                archiveController.fileService = ((InitiationServiceFactory)ServiceFactory).GetFileService();
                return archiveController;
            }
            return null;
        }

    }
}
