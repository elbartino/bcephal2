using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.AddTemplate(template);
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().addOrSelectPage(template);
            initializePageHandlers(page);
            page.Title = template.name;
            getEditor().ListChangeHandler.AddNew(template);
            page.SearchAll();
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            ReconciliationFilterTemplateEditorItem editorPage = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (editorPage == null) return Misp.Kernel.Domain.SubjectType.INPUT_GRID;
            if (editorPage.getForm().SelectedIndex == 0) { }
            else if (editorPage.getForm().SelectedIndex == 1)
            {
                return Misp.Kernel.Domain.SubjectType.RECONCILIATION_FILTER;
            }
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
            editorPage.getForm().ReconciliationFilterTemplateService = GetService();
            editorPage.getForm().SelectionChanged += OnSelectedTabChange;
            editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.GroupService = GetService().GroupService;
            editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.ItemChanged += OnConfigurationChanged;
        }

        private void OnConfigurationChanged(object item)
        {
            if (item is ReconciliationFilterTemplate)
            {
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
                ReconciliationFilterTemplate recoFilterTemplate = (ReconciliationFilterTemplate)item;
                page.EditedObject.group = recoFilterTemplate.group;
                page.EditedObject.visibleInShortcut = recoFilterTemplate.visibleInShortcut;
                page.EditedObject.setBalanceFormula(recoFilterTemplate.balanceFormula);
                page.EditedObject.setDebitCreditFormula(recoFilterTemplate.debitCreditFormula);
            }
            OnChange();
        }

        protected void onGroupFieldChange()
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            string name = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.textBox.Text;
            BGroup group = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.Group;
            page.EditedObject.group = group;
            OnChange();
        }

        protected override void initializeSideBarHandlers()
        {
            ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.SelectionChanged += onSelectTemplateFromSidebar;

            ((ReconciliationFilterTemplateSideBar)SideBar).MeasureGroup.Tree.Click += onSelectMeasureFromSidebar;
            ((ReconciliationFilterTemplateSideBar)SideBar).EntityGroup.Tree.Click += onSelectTargetFromSidebar;
            ((ReconciliationFilterTemplateSideBar)SideBar).EntityGroup.Tree.DoubleClick += onDoubleClickSelectTargetFromSidebar;
            ((ReconciliationFilterTemplateSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;

            ((ReconciliationFilterTemplateSideBar)SideBar).PeriodGroup.Tree.Click += onSelectPeriodFromSidebar;
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<BrowserData> designs = Service.getBrowserDatas();
            ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.fillTree(new ObservableCollection<BrowserData>(designs));

            ((ReconciliationFilterTemplateSideBar)SideBar).EntityGroup.InitializeData();
            ((ReconciliationFilterTemplateSideBar)SideBar).MeasureGroup.InitializeMeasure(false);

            ((ReconciliationFilterTemplateSideBar)SideBar).PeriodGroup.InitializeData();

            Target targetAll = GetService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((ReconciliationFilterTemplateSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            BGroup group = GetService().GroupService.getDefaultGroup();
        }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        protected override void initializeViewData() { }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectTemplateFromSidebar(object sender)
        {
            if (sender != null && sender is ReconciliationFilterTemplate)
            {
                ReconciliationFilterTemplate grid = (ReconciliationFilterTemplate)sender;
                EditorItem<ReconciliationFilterTemplate> page = getEditor().getPage(grid.name);
                if (page != null)
                {
                    page.fillObject();
                    getEditor().selectePage(page);
                }
                else if (grid.oid != null && grid.oid.HasValue)
                {
                    this.Open(grid.oid.Value);

                }
                else
                {
                    page = getEditor().addOrSelectPage(grid);
                    initializePageHandlers(page);
                    page.Title = grid.name;

                    getEditor().ListChangeHandler.AddNew(grid);
                }
                ReconciliationFilterTemplateEditorItem pageOpen = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
                
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une mesure sur la sidebar.
        /// Cette opération a pour but d'assigner la mesure sélectionnée 
        /// aux cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La mesure sélectionnée</param>
        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && (sender is Measure || sender is CalculatedMeasure))
            {
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
                if (page == null) return;
                page.SetMeasure((Measure)sender);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void onSelectPeriodFromSidebar(object sender)
        {
            if (sender != null)
            {
                ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
                if (page == null) return;
                page.SetPeriod(sender);
                OnChange();
            }

        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTargetFromSidebar(object target)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (page == null) return;
            page.SetTarget((Target)target);
            OnChange();

        }

        protected void onDoubleClickSelectTargetFromSidebar(object sender)
        {
            onSelectTargetFromSidebar(sender);
        }

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
            page.getForm().ReconciliationFilterTemplateService = GetService();
            if (page.getForm().SelectedIndex == 0)
            {
                ApplicationManager.MainWindow.displayPropertyBar(null);
                if (page.getForm().LeftGrid.GrilleBrowserForm.gridBrowser.RebuildGrid) UpdateGridForm(page.getForm().LeftGrid);
                if (page.getForm().RightGrid.GrilleBrowserForm.gridBrowser.RebuildGrid) UpdateGridForm(page.getForm().RightGrid);
                if (page.getForm().BottomGrid.GrilleBrowserForm.gridBrowser.RebuildGrid) UpdateGridForm(page.getForm().BottomGrid);
            }
            else
            {
                ApplicationManager.MainWindow.displayPropertyBar(this.PropertyBar);
                ReconciliationFilterTemplatePropertyBar bar = (ReconciliationFilterTemplatePropertyBar)this.PropertyBar;
                if (page.getForm().SelectedIndex == 1)
                {
                    ConfigurationPropertiesPanel configPane = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel;
                    configPane.ReconciliationFilterTemplateService = GetService();
                    configPane.displayObject();
                    bar.DesignLayoutAnchorable.Content = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Filter Properties";
                }
                else if (page.getForm().SelectedIndex == 2)
                {
                    bar.DesignLayoutAnchorable.Content = page.getForm().LeftGridProperties.InputGridPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Left Grid Properties";
                }
                else if (page.getForm().SelectedIndex == 3)
                {
                    bar.DesignLayoutAnchorable.Content = page.getForm().RightGridProperties.InputGridPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Right Grid Properties";
                }
                else if (page.getForm().SelectedIndex == 4)
                {
                    bar.DesignLayoutAnchorable.Content = page.getForm().BottomGridProperties.InputGridPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Bottom Grid Properties";
                }
            }            
        }

        protected virtual void UpdateGridForm(ReconciliationFilterTemplateGrid grid)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            page.getForm().EditedObject = page.EditedObject;
            grid.GrilleBrowserForm.displayObject();
            grid.Search(grid.EditedObject.GrilleFilter != null ? grid.EditedObject.GrilleFilter.page : 1);

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

