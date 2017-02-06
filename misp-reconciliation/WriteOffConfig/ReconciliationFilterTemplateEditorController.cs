using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Reconciliation.ReconciliationContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.WriteOffConfig
{
    public class ReconciliationFilterTemplateEditorController : EditorController<Kernel.Domain.ReconciliationFilterTemplate, Misp.Kernel.Domain.Browser.BrowserData>
    {

        public ReconciliationFilterTemplateEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        public override Kernel.Application.OperationState Delete()
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Kernel.Domain.SubjectType.RECONCILIATION_FILTER;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ReconciliationFilterTemplateEditor getReconciliationFilterTemplateEditor()
        {
            return (ReconciliationFilterTemplateEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux reconciliation.
        /// </summary>
        /// <returns>ReconciliationService</returns>
        public ReconciliationFilterTemplateService GetReconciliationFilterTemplateService()
        {
            return (ReconciliationFilterTemplateService)base.Service;
        }

        public override Kernel.Application.OperationState Create()
        {
            Kernel.Domain.ReconciliationFilterTemplate reco = new Kernel.Domain.ReconciliationFilterTemplate();


            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getReconciliationFilterTemplateEditor().addOrSelectPage(reco);
            initializePageHandlers(page);
            //page.getReconciliationFilterTemplateForm().ModelService = GetReconciliationContextService().ModelService;
            page.Title = "Reconciliation Filter ";
            getReconciliationFilterTemplateEditor().ListChangeHandler.AddNew(reco);
            return OperationState.CONTINUE;
        }

        protected override Kernel.Ui.Base.IView getNewView()
        {
            return new ReconciliationFilterTemplateEditor();
        }

        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            return new ReconciliationContextToolBar();
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new ToolBarHandlerBuilder(this);
        }

        protected override Kernel.Ui.Sidebar.SideBar getNewSideBar()
        {
            return new ReconciliationContextSideBar();
        }

        protected override Kernel.Ui.Base.PropertyBar getNewPropertyBar()
        {
            return new ReconciliationContextPropertyBar();
        }

        protected override void initializeSideBarData()
        {
           
        }

        protected override void initializeSideBarHandlers()
        {
          
        }
    }
}
