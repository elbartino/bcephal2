﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Reconciliation.Posting;
using Misp.Kernel.Administration.Role;
using Misp.Kernel.Administration.User;
using Misp.Kernel.Administration.Profil;
using Misp.Reconciliation.ReconciliationContext;
using Misp.Reconciliation.RecoGrid;
using Misp.Reconciliation.WriteOffConfig;
using Misp.Reconciliation.Reco;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of ReconciliationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public ReconciliationControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new ReconciliationServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                ReconciliationFilterTemplateBrowserController controller = new ReconciliationFilterTemplateBrowserController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_EDIT && editionMode.HasValue)
            {
                ReconciliationFilterTemplateEditorController controller = new ReconciliationFilterTemplateEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }
                        
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_POSTINGS)
            {
               
                PostingEditorController controller = new PostingEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationGridService();
                return controller;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.POSTING_GRID_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                PostingGridBrowserController controller = new PostingGridBrowserController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingGridService();
                return controller;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.POSTING_GRID_EDIT && editionMode.HasValue)
            {
                PostingGridEditorController controller = new PostingGridEditorController();
                controller.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetPostingGridService();
                return controller;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.AUTOMATIC_POSTING_GRID_LIST && viewType.HasValue && viewType.Value == ViewType.SEARCH)
            {
                AutomaticPostingGridBrowerController automaticSourcingGridBrowerController = new AutomaticPostingGridBrowerController();
                automaticSourcingGridBrowerController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridBrowerController.FunctionalityCode = fonctionality;
                automaticSourcingGridBrowerController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridBrowerController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetAutomaticPostingGridService();
                return automaticSourcingGridBrowerController;
            }
            if (fonctionality == ReconciliationFunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT && editionMode.HasValue)
            {
                AutomaticPostingGridEditorController automaticSourcingGridController = new AutomaticPostingGridEditorController();
                automaticSourcingGridController.ModuleName = Misp.Sourcing.PlugIn.MODULE_NAME;
                automaticSourcingGridController.FunctionalityCode = fonctionality;
                automaticSourcingGridController.ApplicationManager = this.ApplicationManager;
                automaticSourcingGridController.Service = ((ReconciliationServiceFactory)ServiceFactory).GetAutomaticPostingGridService();
                //automaticSourcingGridController.InputTableService = ((ReconciliationServiceFactory)ServiceFactory).GetInputTableService();
                return automaticSourcingGridController;
            }
            
            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_CONFIGURATION)
            {
                ReconciliationContextEditorController controller = new ReconciliationContextEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationContextService();
                return controller;
            }

            if (fonctionality == ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_TEMPLATE_CONFIGURATION)
            {
                ReconciliationFilterTemplateEditorController controller = new ReconciliationFilterTemplateEditorController();
                controller.ModuleName = Misp.Reconciliation.PlugIn.MODULE_NAME;
                controller.FunctionalityCode = fonctionality;
                controller.ApplicationManager = this.ApplicationManager;
                controller.Service = ((ReconciliationServiceFactory)ServiceFactory).GetReconciliationFilterTemplateService();
                return controller;
            }


            return null;
        }

    }
}
