using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Sourcing.Table;
using Misp.Sourcing.Designer;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.CustomizedTarget;
using Misp.Sourcing.MultipleFilesUpload;
using Misp.Sourcing.AutomaticTargetViews;
using Misp.Sourcing.GridViews;
using Misp.Sourcing.InputGrid;
using Misp.Sourcing.Posting;

namespace Misp.Sourcing.Base
{
    public class SourcingControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of SourcingControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public SourcingControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new SourcingServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality)
        {
            if (fonctionality == SourcingFunctionalitiesCode.INPUT_TABLE_EDIT)
            {
                InputTableEditorController tableController = new InputTableEditorController();
                tableController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                tableController.FunctionalityCode = fonctionality;
                tableController.ApplicationManager = this.ApplicationManager;
                tableController.Service = ((SourcingServiceFactory)ServiceFactory).GetInputTableService();
                return tableController;
            }
            if (fonctionality == SourcingFunctionalitiesCode.INPUT_TABLE_LIST)
            {
                InputTableBrowserController tableController = new InputTableBrowserController();
                tableController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                tableController.FunctionalityCode = fonctionality;
                tableController.ApplicationManager = this.ApplicationManager;
                tableController.Service = ((SourcingServiceFactory)ServiceFactory).GetInputTableService();
                return tableController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.TARGET_EDIT)
            {
                TargetEditorController targetController = new TargetEditorController();
                targetController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                targetController.FunctionalityCode = fonctionality;
                targetController.ApplicationManager = this.ApplicationManager;
                targetController.Service = ((SourcingServiceFactory)ServiceFactory).GetTargetService();
                return targetController;
            }
            if (fonctionality == SourcingFunctionalitiesCode.TARGET_LIST)
            {
                TargetBrowserController targetController = new TargetBrowserController();
                targetController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                targetController.FunctionalityCode = fonctionality;
                targetController.ApplicationManager = this.ApplicationManager;
                targetController.Service = ((SourcingServiceFactory)ServiceFactory).GetTargetService();
                return targetController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.DESIGN_EDIT)
            {
                DesignerEditorController designerController = new DesignerEditorController();
                designerController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                designerController.FunctionalityCode = fonctionality;
                designerController.ApplicationManager = this.ApplicationManager;
                designerController.Service = ((SourcingServiceFactory)ServiceFactory).GetDesignService();
                return designerController;
            }
            if (fonctionality == SourcingFunctionalitiesCode.DESIGN_LIST)
            {
                DesignerBrowserController designerController = new DesignerBrowserController();
                designerController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                designerController.FunctionalityCode = fonctionality;
                designerController.ApplicationManager = this.ApplicationManager;
                designerController.Service = ((SourcingServiceFactory)ServiceFactory).GetDesignService();
                return designerController;
            }
        
            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_SOURCING_EDIT || fonctionality == SourcingFunctionalitiesCode.UPLOAD_STRUCTURED_FILE_FUNCTIONALITY)
            {
                AutomaticSourcingEditorController automaticSourcingController = new AutomaticSourcingEditorController();
                automaticSourcingController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingController.FunctionalityCode = fonctionality;
                automaticSourcingController.ApplicationManager = this.ApplicationManager;
                automaticSourcingController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticSourcingService();
                automaticSourcingController.InputTableService = ((SourcingServiceFactory)ServiceFactory).GetInputTableService();
                return automaticSourcingController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_SOURCING_LIST)
                {
                AutomaticSourcingBrowserController automaticSourcingController = new AutomaticSourcingBrowserController();
                automaticSourcingController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingController.FunctionalityCode = fonctionality;
                automaticSourcingController.ApplicationManager = this.ApplicationManager;
                automaticSourcingController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticSourcingService();
                return automaticSourcingController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.INPUT_TABLE_GRID_LIST)
            {
                InputGridBrowserController controller = new InputGridBrowserController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((SourcingServiceFactory)ServiceFactory).GetInputGridService();
                return controller;
            }

            if (fonctionality == SourcingFunctionalitiesCode.INPUT_TABLE_GRID_EDIT)
            {
                InputGridEditorController controller = new InputGridEditorController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((SourcingServiceFactory)ServiceFactory).GetInputGridService();
                return controller;
            }
            
            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT)
            {
                AutomaticSourcingGridEditorController automaticSourcingGridController = new AutomaticSourcingGridEditorController();
                automaticSourcingGridController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridController.FunctionalityCode = fonctionality;
                automaticSourcingGridController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticSourcingGridService();
                automaticSourcingGridController.InputTableService = ((SourcingServiceFactory)ServiceFactory).GetInputTableService();
                return automaticSourcingGridController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_LIST)
            {
                AutomaticSourcingGridBrowerController automaticSourcingGridBrowerController = new AutomaticSourcingGridBrowerController();
                automaticSourcingGridBrowerController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridBrowerController.FunctionalityCode = fonctionality;
                automaticSourcingGridBrowerController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridBrowerController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticSourcingGridService();
                return automaticSourcingGridBrowerController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_TARGET_EDIT)
            {
                AutomaticTargetEditorController automaticTargetController = new AutomaticTargetEditorController();
                automaticTargetController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticTargetController.FunctionalityCode = fonctionality;
                automaticTargetController.ApplicationManager = this.ApplicationManager;
                automaticTargetController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticTargetService();
                return automaticTargetController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.AUTOMATIC_TARGET_LIST)
            {
                AutomaticTargetBrowserController automaticTargetBrowserController = new AutomaticTargetBrowserController();
                automaticTargetBrowserController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticTargetBrowserController.FunctionalityCode = fonctionality;
                automaticTargetBrowserController.ApplicationManager = this.ApplicationManager;
                automaticTargetBrowserController.Service = ((SourcingServiceFactory)ServiceFactory).GetAutomaticTargetService();
                return automaticTargetBrowserController;
            }

            if (fonctionality == SourcingFunctionalitiesCode.MULTIPLE_FILES_UPLOAD)
            {
                UploadMultipleFilesController uploadMultipleFilesController = new UploadMultipleFilesController();
                uploadMultipleFilesController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                uploadMultipleFilesController.FunctionalityCode = fonctionality;
                uploadMultipleFilesController.ApplicationManager = this.ApplicationManager;
                uploadMultipleFilesController.Service = ((SourcingServiceFactory)ServiceFactory).GetUploadMultipleFilesService();
                uploadMultipleFilesController.InputTableService = ((SourcingServiceFactory)ServiceFactory).GetInputTableService();
                return uploadMultipleFilesController;
            }


            return null;
        }

    }
}
