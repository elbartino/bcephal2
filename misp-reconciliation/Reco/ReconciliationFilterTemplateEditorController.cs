using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateEditorController : EditorController<ReconciliationFilterTemplate, BrowserData>
    {
        #region Properties

        protected System.Windows.Threading.DispatcherTimer runTimer;
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        private bool IsRenameOnDoubleClick = false;
        private object OnChanged;
        #endregion


        #region Constructor

        public ReconciliationFilterTemplateEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.RECONCILIATION_FILTER;
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
            ApplicationManager.MainWindow.IsBussy = true;
            Kernel.Application.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    try
                    {
                        ReconciliationFilterTemplate template = GetNewTemplate();
                        ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.AddTemplate(template);
                        ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().addOrSelectPage(template);
                        initializePageHandlers(page);
                        page.Title = template.name;
                        getEditor().ListChangeHandler.AddNew(template);
                        page.SearchAll();
                    }
                    catch (Exception e)
                    {
                        MessageDisplayer.DisplayError("Error", e.Message);
                    }
                    finally
                    {
                        ApplicationManager.MainWindow.IsBussy = false;
                    }
                }));

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

        protected override void AfterClose()
        {
            base.AfterClose();
            foreach(EditorItem<ReconciliationFilterTemplate> page in getEditor().getPages()){
                removePageHandlers(page);
            }
            getEditor().Children.Clear();
            removeViewHandlers();
        }

        protected virtual void removePageHandlers(EditorItem<ReconciliationFilterTemplate> page)
        {
            //base.removePageHandlers(page);
            ReconciliationFilterTemplateEditorItem editorPage = (ReconciliationFilterTemplateEditorItem)page;
            editorPage.getForm().SelectionChanged -= OnSelectedTabChange;
            if (ApplicationManager.User != null && ApplicationManager.User.IsAdmin())
            {
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.Changed -= onGroupFieldChange;
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.ItemChanged -= OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.WriteOffConfigPanel.ItemChanged -= OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.ItemChanged -= OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.KeyUp -= onNameTextChange;
                editorPage.getForm().FormChanged -= OnFormChanged;
            }
            if (editorPage.getForm().AdministrationBar != null)
            {
                editorPage.getForm().AdministrationBar.Changed -= OnChangeEventHandler;
            }
        }
        
        protected override void initializePageHandlers(EditorItem<ReconciliationFilterTemplate> page)
        {
            base.initializePageHandlers(page);
            ReconciliationFilterTemplateEditorItem editorPage = (ReconciliationFilterTemplateEditorItem)page;
            editorPage.getForm().SelectionChanged += OnSelectedTabChange;
            if (ApplicationManager.User != null && ApplicationManager.User.IsAdmin())
            {                
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.GroupService = GetService().GroupService;
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.subjectType = SubjectTypeFound();
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.Changed += onGroupFieldChange;
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.ItemChanged += OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.WriteOffConfigPanel.ItemChanged += OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.ItemChanged += OnConfigurationChanged;
                editorPage.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
                editorPage.getForm().FormChanged += OnFormChanged;
            }
            if (editorPage.getForm().AdministrationBar != null)
            {
                editorPage.getForm().AdministrationBar.Changed += OnChangeEventHandler;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<ReconciliationFilterTemplate> page)
        {
            if (page == null) return;
            base.OnPageSelected(page);
            ReconciliationFilterTemplateForm form = ((ReconciliationFilterTemplateEditorItem)page).getForm();
            ReconciliationFilterTemplatePropertyBar bar = (ReconciliationFilterTemplatePropertyBar)this.PropertyBar;
            if (bar.AdministratorLayoutAnchorable != null) bar.AdministratorLayoutAnchorable.Content = form.AdministrationBar;
            PerformSelectionChange();
        }

        private void OnChangeEventHandler()
        {
            OnChange();
        }

        private void OnFormChanged()
        {
            OnChange();
        }

        

        private void onNameTextChange(object sender, KeyEventArgs args)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEditedNewName();
                OnChange();
            }
        }

        private void OnConfigurationChanged(object item)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (page == null) return;
            if (item is ReconciliationFilterTemplate)
            {
                ReconciliationFilterTemplate recoFilterTemplate = (ReconciliationFilterTemplate)item;
                page.EditedObject.group = recoFilterTemplate.group;
                page.EditedObject.visibleInShortcut = recoFilterTemplate.visibleInShortcut;
                page.EditedObject.balanceFormulaEnum = recoFilterTemplate.balanceFormulaEnum;
                page.EditedObject.useDebitCredit = recoFilterTemplate.useDebitCredit;
            }
            else if (item is WriteOffConfiguration) 
            {
                page.EditedObject.writeOffConfig = (WriteOffConfiguration)item;
            }
            OnChange();
        }

        protected void onGroupFieldChange()
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            string name = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.textBox.Text;
            BGroup group = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.groupField.Group;
            ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.updateTemplate(name, page.Title, true);
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
            ((ReconciliationFilterTemplateSideBar)SideBar).MeasureGroup.InitializeMeasure();

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

        protected override void Rename(string name)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Text = name;
            //page.EditedObject.name = name;
            base.Rename(name);
        }


        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();

            return ValidateEditedNewName(page.Title);
        }


        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            ReconciliationFilterTemplate table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Filter Template name can't be mepty!");
                page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.SelectAll();
                page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Focus();
                return OperationState.STOP;
            }


            foreach (ReconciliationFilterTemplateEditorItem unInputTable in getEditor().getPages())
            {
                if (unInputTable != getEditor().getActivePage() && newName == unInputTable.Title)
                {
                    DisplayError("Duplicate Name", "There is another Filter Template  named: " + newName);
                    page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Text = page.Title;
                    page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.SelectAll();
                    page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel.NameTextBox.Focus();
                    return OperationState.STOP;
                }
            }
            if (!IsRenameOnDoubleClick)
                if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((ReconciliationFilterTemplateSideBar)SideBar).TemplateGroup.TemplateTreeview.updateTemplate(newName, table.name, false);
            table.name = newName;
            page.Title = newName;
            return OperationState.CONTINUE;
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
                ReconciliationFilterTemplatePropertyBar bar = (ReconciliationFilterTemplatePropertyBar)this.PropertyBar;
                page.SetPeriod(sender, bar.FilterLayoutAnchorable.IsSelected);
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
            ReconciliationFilterTemplatePropertyBar bar = (ReconciliationFilterTemplatePropertyBar)this.PropertyBar;
            page.SetTarget((Target)target, bar.FilterLayoutAnchorable.IsSelected);
            
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
            ReconciliationFilterTemplatePropertyBar bar = (ReconciliationFilterTemplatePropertyBar)this.PropertyBar;
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            if (page.getForm().SelectedIndex == 0)
            {
                ApplicationManager.MainWindow.displayPropertyBar(null);
                if (page.getForm().LeftGrid.GrilleBrowserForm.gridBrowser.RebuildGrid) UpdateGridForm(page.getForm().LeftGrid);
                if (page.getForm().RightGrid.GrilleBrowserForm.gridBrowser.RebuildGrid) UpdateGridForm(page.getForm().RightGrid);
                if (page.getForm().BottomGrid.GridBrowser.RebuildGrid) UpdateGridForm(page.getForm().BottomGrid);

                page.getForm().LeftGrid.CustomizeDC();
                page.getForm().RightGrid.CustomizeDC();
            }
            else if (ApplicationManager.Instance.User.IsAdmin())
            {
               ApplicationManager.MainWindow.displayPropertyBar(this.PropertyBar);
                //if (!bar.Pane.Children.Contains(bar.DesignLayoutAnchorable)) bar.Pane.Children.Add(bar.DesignLayoutAnchorable);
                //if (bar.AdministratorLayoutAnchorable != null && !bar.Pane.Children.Contains(bar.AdministratorLayoutAnchorable)) bar.Pane.Children.Add(bar.AdministratorLayoutAnchorable);

                bar.Pane.Children.Remove(bar.FilterLayoutAnchorable);
                if (page.getForm().SelectedIndex == 1)
                {
                    ConfigurationPropertiesPanel configPane = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel;
                    configPane.displayObject();
                    bar.DesignLayoutAnchorable.Content = page.getForm().ConfigurationPanel.ConfigurationPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Filter Properties";
                }
                else if (page.getForm().SelectedIndex == 2)
                {
                    bar.DesignLayoutAnchorable.Content = page.getForm().LeftGridProperties.InputGridPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Left Grid Properties";
                    bar.FilterLayoutAnchorable.Content = page.getForm().LeftGrid.GrilleBrowserForm.filterForm;
                    bar.FilterLayoutAnchorable.Title = "Left Filter Properties";

                    bar.Pane.Children.Insert(1, bar.FilterLayoutAnchorable);
                }
                else if (page.getForm().SelectedIndex == 3)
                {
                    bar.DesignLayoutAnchorable.Content = page.getForm().RightGridProperties.InputGridPropertiesPanel;
                    bar.DesignLayoutAnchorable.Title = "Right Grid Properties";
                    bar.FilterLayoutAnchorable.Content = page.getForm().RightGrid.GrilleBrowserForm.filterForm;
                    bar.FilterLayoutAnchorable.Title = "Right Filter Properties";

                    bar.Pane.Children.Insert(1, bar.FilterLayoutAnchorable);
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

        protected virtual void UpdateGridForm(ReconciliationFilterTemplateBottomGrid grid)
        {
            ReconciliationFilterTemplateEditorItem page = (ReconciliationFilterTemplateEditorItem)getEditor().getActivePage();
            page.getForm().EditedObject = page.EditedObject;
            grid.displayObject();
        }


        public override OperationState Save(EditorItem<ReconciliationFilterTemplate> page)
        {
            if (!page.IsReadOnly && page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                page.fillObject();
                ReconciliationFilterTemplate editedObject = page.EditedObject;
                try
                {
                    page.EditedObject = GetService().Save(editedObject);
                    //page.EditedObject = GetService().getByOid(editedObject.oid.Value);
                    ((ReconciliationFilterTemplateEditorItem)page).getForm().displayObject();
                    page.IsModify = false;
                }
                catch (Misp.Kernel.Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + editedObject.ToString());
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }
        #endregion


        #region Utils

        protected override IView getNewView()
        {
            ReconciliationFilterTemplateEditor editor = new ReconciliationFilterTemplateEditor(this.SubjectType, this.FunctionalityCode);
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
                if (grid == null)
                {
                   grid = ((ReconciliationFilterTemplateSideBar)getNewSideBar()).TemplateGroup.TemplateTreeview.getTemplateByName(name);
                   if (grid == null)
                   {
                       grid = GetService().getByName(name);
                       if (grid == null) return name;
                   }
                }
                i++;
            }
            return name;
        }

        #endregion

    }
}

