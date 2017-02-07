using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateEditorController : EditorController<ReconciliationFilterTemplate, BrowserData>
    {
        #region Properties

        protected System.Windows.Threading.DispatcherTimer runTimer;
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
                
        #endregion


        #region Constructor

        public ReconciliationFilterTemplateEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        #endregion


        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ReconciliationFilterTemplateEditor getTemplateEditor()
        {
            return (ReconciliationFilterTemplateEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux ReconciliationFilterTemplates.
        /// </summary>
        /// <returns>ReconciliationFilterTemplateService</returns>
        public ReconciliationFilterTemplateService GetService()
        {
            return (ReconciliationFilterTemplateService)base.Service;
        }

        #endregion


        #region Operations

        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            ReconciliationFilterTemplate template = GetNewTemplate();
            //((ReconciliationFilterTemplateSideBar)SideBar).GrilleGroup.GrilleTreeview.AddGrille(template);
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().addOrSelectPage(template);
            initializePageHandlers(page);
            page.Title = template.name;
            getEditor().ListChangeHandler.AddNew(template);
            page.SearchAll();
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_GRID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(ReconciliationFilterTemplate template)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().addOrSelectPage(template);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(template);
            page.SearchAll();
            return OperationState.CONTINUE;
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        #endregion


        #region Handles

        protected override void initializePageHandlers(EditorItem<ReconciliationFilterTemplate> page)
        {
            base.initializePageHandlers(page);
            ReconciliationFilterTemplateEditorItem editorPage = (ReconciliationFilterTemplateEditorItem)page;
            editorPage.getForm().SelectionChanged += OnSelectedTabChange;
        }

        protected override void initializeSideBarHandlers()
        {
            /*((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.SelectionChanged += onSelectGridFromSidebar;

            ((InputGridSideBar)SideBar).MeasureGroup.Tree.Click += onSelectMeasureFromSidebar;
            ((InputGridSideBar)SideBar).EntityGroup.Tree.Click += onSelectTargetFromSidebar;
            ((InputGridSideBar)SideBar).EntityGroup.Tree.DoubleClick += onDoubleClickSelectTargetFromSidebar;
            ((InputGridSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;

            ((InputGridSideBar)SideBar).PeriodGroup.Tree.Click += onSelectPeriodFromSidebar;*/
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            /*List<BrowserData> designs = Service.getBrowserDatas();
            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.fillTree(new ObservableCollection<BrowserData>(designs));

            ((InputGridSideBar)SideBar).EntityGroup.InitializeData();
            ((InputGridSideBar)SideBar).MeasureGroup.InitializeMeasure(false);

            ((InputGridSideBar)SideBar).PeriodGroup.InitializeData();

            //PeriodName rootPeriodName = GetInputGridService().PeriodNameService.getRootPeriodName();
            //((InputGridSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);

            Target targetAll = GetInputGridService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((InputGridSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            BGroup group = GetInputGridService().GroupService.getDefaultGroup();*/
        }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        protected override void initializeViewData() { }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }


        protected virtual void OnSelectedTabChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            if (e == null) return;
            if (!(e.Source is ReconciliationFilterTemplateForm)) return;
            PerformSelectionChange();
            e.Handled = true;
        }

        protected virtual void PerformSelectionChange()
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (page.getForm().SelectedIndex == 0)
            {
                ApplicationManager.MainWindow.displayPropertyBar(null);
                //if (page.getForm().LeftGrid.gridBrowser.RebuildGrid) UpdateGridForm();
            }
            else
            {
                ApplicationManager.MainWindow.displayPropertyBar(this.PropertyBar);
                if (page.getForm().SelectedIndex == 1)
                {
                    //((ReconciliationFilterTemplatePropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = page.getForm().InputGridSheetForm.InputGridPropertiesPanel;
                }
                else if (page.getForm().SelectedIndex == 2)
                {
                    ((ReconciliationFilterTemplatePropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = page.getForm().LeftGridProperties.InputGridPropertiesPanel;
                }
                else if (page.getForm().SelectedIndex == 3)
                {
                    ((ReconciliationFilterTemplatePropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = page.getForm().RightGridProperties.InputGridPropertiesPanel;
                }
                else if (page.getForm().SelectedIndex == 4)
                {
                    ((ReconciliationFilterTemplatePropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = page.getForm().BottomGridProperties.InputGridPropertiesPanel;
                }
            }            
        }

        protected virtual void UpdateGridForm()
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            page.getForm().EditedObject = page.EditedObject;
            /*page.getForm().displayObjectInGridForm();
            Search(page.EditedObject.GrilleFilter != null ? page.EditedObject.GrilleFilter.page : 1);*/
        }

        #endregion


        #region Utils

        protected override IView getNewView()
        {
            ReconciliationFilterTemplateEditor editor = new ReconciliationFilterTemplateEditor();
            editor.Service = GetService();
            return editor;
        }

        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new ReconciliationFilterTemplateToolBar(); }

        protected override SideBar getNewSideBar() { return new ReconciliationFilterTemplateSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new ReconciliationFilterTemplatePropertyBar(); }

        protected virtual ReconciliationFilterTemplate GetNewTemplate()
        {
            ReconciliationFilterTemplate template = new ReconciliationFilterTemplate();
            template.name = getNewPageName("Filter");
            template.group = GetService().GroupService.getDefaultGroup();
            template.visibleInShortcut = true;
            return template;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                ReconciliationFilterTemplate grid = GetObjectByName(name);
                if (grid == null) return name;
                i++;
            }
            return name;
        }

        #endregion

    }
}

