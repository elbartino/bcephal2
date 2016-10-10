using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Planification.Tranformation;
using Misp.Planification.Tranformation.TransformationTable;
using Misp.Planification.PresentationView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Planification.Tranformation.Run_all;

namespace Misp.Planification.Base
{
    public class PlanificationControllerFactory : ControllerFactory
    {
        /// <summary>
        /// Build a new instance of PlanificationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public PlanificationControllerFactory(ApplicationManager applicationManager) : base(applicationManager)
        {
            this.ServiceFactory = new PlanificationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality)
        {
            if (fonctionality == PlanificationFunctionalitiesCode.NEW_TRANSFORMATION_TREE_FUNCTIONALITY)
            {
                TransformationTreeEditorController tableController = new TransformationTreeEditorController();
                tableController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                tableController.FunctionalityCode = fonctionality;
                tableController.ApplicationManager = this.ApplicationManager;
                tableController.Service = ((PlanificationServiceFactory)ServiceFactory).GetTransformationTreeService();
                return tableController;
            }
            if (fonctionality == PlanificationFunctionalitiesCode.LIST_TRANSFORMATION_TREE_FUNCTIONALITY)
            {
                TransformationTreeBrowserController tableController = new TransformationTreeBrowserController();
                tableController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                tableController.FunctionalityCode = fonctionality;
                tableController.ApplicationManager = this.ApplicationManager;
                tableController.Service = ((PlanificationServiceFactory)ServiceFactory).GetTransformationTreeService();
                return tableController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.NEW_TRANSFORMATION_TABLE_FUNCTIONALITY)
            {
                TransformationTableController transformationTableController = new TransformationTableController();
                transformationTableController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                transformationTableController.FunctionalityCode = fonctionality;
                transformationTableController.ApplicationManager = this.ApplicationManager;
                transformationTableController.Service = ((PlanificationServiceFactory)ServiceFactory).GetTransformationTableService();
                return transformationTableController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.NEW_SLIDE_FUNCTIONALITY) 
            {
                PresentationEditorController presentationEditorController = new PresentationEditorController();
                presentationEditorController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                presentationEditorController.FunctionalityCode = fonctionality;
                presentationEditorController.ApplicationManager = this.ApplicationManager;
                presentationEditorController.Service = ((PlanificationServiceFactory)ServiceFactory).GetPresentationService();
                return presentationEditorController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.LOAD_TRANSFORMATION_TREES_FUNCTIONALITY)
            {
                RunAllTransformationTreesController RunAllTransformationTreesController = new RunAllTransformationTreesController();
                RunAllTransformationTreesController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                RunAllTransformationTreesController.FunctionalityCode = fonctionality;
                RunAllTransformationTreesController.ApplicationManager = this.ApplicationManager;
                RunAllTransformationTreesController.Service = ((PlanificationServiceFactory)ServiceFactory).GetTransformationTreeService();
                return RunAllTransformationTreesController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.CLEAR_TRANSFORMATION_TREES_FUNCTIONALITY)
            {
                RunAllTransformationTreesController RunAllTransformationTreesController = new RunAllTransformationTreesController();
                RunAllTransformationTreesController.isClearOption = true;
                RunAllTransformationTreesController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                RunAllTransformationTreesController.FunctionalityCode = fonctionality;
                RunAllTransformationTreesController.ApplicationManager = this.ApplicationManager;
                RunAllTransformationTreesController.Service = ((PlanificationServiceFactory)ServiceFactory).GetTransformationTreeService();
                return RunAllTransformationTreesController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.NEW_COMBINED_TRANSFORMATION_TREES_FUNCTIONALITY)
            {
                CombinedTransformationTree.CombinedTransformationTreeEditorController CombineTransformationTreeEditorController = new CombinedTransformationTree.CombinedTransformationTreeEditorController();
                CombineTransformationTreeEditorController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                CombineTransformationTreeEditorController.FunctionalityCode = fonctionality;
                CombineTransformationTreeEditorController.ApplicationManager = this.ApplicationManager;
                CombineTransformationTreeEditorController.Service = ((PlanificationServiceFactory)ServiceFactory).GetCombinedTransformationTreeService();
                return CombineTransformationTreeEditorController;
            }

            if (fonctionality == PlanificationFunctionalitiesCode.LIST_COMBINED_TRANSFORMATION_TREES_FUNCTIONALITY)
            {
                CombinedTransformationTree.CombinedTransformationTreeBrowserController CombineTransformationTreeEditorBrowserController = new CombinedTransformationTree.CombinedTransformationTreeBrowserController();
                CombineTransformationTreeEditorBrowserController.ModuleName = Misp.Planification.PlugIn.MODULE_NAME;
                CombineTransformationTreeEditorBrowserController.FunctionalityCode = fonctionality;
                CombineTransformationTreeEditorBrowserController.ApplicationManager = this.ApplicationManager;
                CombineTransformationTreeEditorBrowserController.Service = ((PlanificationServiceFactory)ServiceFactory).GetCombinedTransformationTreeService();
                return CombineTransformationTreeEditorBrowserController;
            }

            return null;
        }

    }
}
