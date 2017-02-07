using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
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
            return new ReconciliationFilterTemplateSideBar();
        }

        protected override Kernel.Ui.Base.PropertyBar getNewPropertyBar()
        {
            return new ReconciliationContextPropertyBar();
        }

        protected override void initializeSideBarData()
        {
            ((ReconciliationFilterTemplateSideBar)SideBar).EntityGroup.InitializeData();
            ((ReconciliationFilterTemplateSideBar)SideBar).MeasureGroup.InitializeMeasure(false);
            ((ReconciliationFilterTemplateSideBar)SideBar).PeriodGroup.InitializeData();
        }

        protected override void initializeSideBarHandlers()
        {
            ((ReconciliationFilterTemplateSideBar)SideBar).MeasureGroup.Tree.Click += onSelectMeasureFromSidebar;
            ((ReconciliationFilterTemplateSideBar)SideBar).EntityGroup.Tree.Click += OnSelectTarget;
            ((ReconciliationFilterTemplateSideBar)SideBar).PeriodGroup.Tree.Click += onSelectPeriodNameFromSidebar;
        }

        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is Measure)
            {
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getReconciliationFilterTemplateEditor().getActivePage();
                if (page == null) return;
                Measure measure = (Measure)sender;
                page.getReconciliationFilterTemplateForm().setMeasure(measure);
            }
        }

        protected void OnSelectTarget(object sender)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getReconciliationFilterTemplateEditor().getActivePage();
            if (page == null) return;
            if(sender is Kernel.Domain.Attribute)
            {
                page.getReconciliationFilterTemplateForm().setAttribute(sender as Kernel.Domain.Attribute);
            }
            else if(sender is Kernel.Domain.AttributeValue)
            {}
        }

        protected virtual void onSelectPeriodNameFromSidebar(object sender)
        {
            if (sender == null) return;
            if (sender is PeriodName)
            {
                PeriodName periodName = (PeriodName)sender;
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getReconciliationFilterTemplateEditor().getActivePage();
                if (page == null) return;
                page.getReconciliationFilterTemplateForm().setPeriodName(periodName);
            }
            else if (sender is PeriodInterval)
            {
                PeriodInterval periodInterval = (PeriodInterval)sender;
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getReconciliationFilterTemplateEditor().getActivePage();
                if (page == null) return;
                page.getReconciliationFilterTemplateForm().setPeriodInterval(periodInterval);
            }
        }
    }
}
