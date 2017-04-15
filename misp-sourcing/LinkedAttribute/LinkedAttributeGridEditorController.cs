﻿using Misp.Kernel.Application;
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

namespace Misp.Sourcing.LinkedAttribute
{
    public class LinkedAttributeGridEditorController : EditorController<LinkedAttributeGrid, BrowserData>
    {

        #region Properties

        
        #endregion


        #region Constructors

        public LinkedAttributeGridEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.LINKED_ATTRIBUTE_GRID;
        }

        #endregion


        #region Editor and Service

        public LinkedAttributeGridEditor getLinkedAttributeGridEditor()
        {
            return (LinkedAttributeGridEditor)this.View;
        }

        public LinkedAttributeGridService GetLinkedAttributeGridService()
        {
            return (LinkedAttributeGridService)base.Service;
        }

        #endregion
        

        #region Operations

        public override OperationState Open(LinkedAttributeGrid grid)
        {
            //if (getEditor().getPage(grid) == null) grid.loadGrilleFilter();
            LinkedAttributeGridEditorItem page = (LinkedAttributeGridEditorItem)getEditor().addOrSelectPage(grid);
            //UpdateStatusBar();
            //UpdateToolBar(page.EditedObject);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(grid);
            Search();
            return OperationState.CONTINUE;
        }

        public override OperationState Create() { return OperationState.CONTINUE; }

        public override OperationState Delete() { return OperationState.CONTINUE; }
                
        public override SubjectType SubjectTypeFound() { return SubjectType.INPUT_GRID; }                

        #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            LinkedAttributeGridEditor editor = new LinkedAttributeGridEditor(this.SubjectType, this.FunctionalityCode);
            editor.Service = GetLinkedAttributeGridService();
            return editor;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            Misp.Kernel.Ui.Base.ToolBar toolbar = new Kernel.Ui.Base.ToolBar();
            toolbar.Children.Remove(toolbar.NewButton);
            return toolbar;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new SideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new PropertyBar(); }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {
            //DimensionField.Periodicity = GetStructuredReportService().PeriodicityService.getPeriodicity(); 
        }
        
        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<LinkedAttributeGrid> page)
        {
            base.initializePageHandlers(page);
            LinkedAttributeGridEditorItem editorPage = (LinkedAttributeGridEditorItem)page;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.GroupService = GetInputGridService().GroupService;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.LostFocus += onNameTextLostFocus;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.Changed += onGroupFieldChange;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.OnSetTableVisible += OnSetTableVisible;

            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.visibleInShortcutCheckbox.Checked += OnVisibleInShortcutCheck;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.visibleInShortcutCheckbox.Unchecked += OnVisibleInShortcutCheck;

            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.Changed += OnInputGridPropertiesChange;
            //editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.selectionColumnChanged += OnInputGridPropertiesSelectionColumnChange;

            if (editorPage.getLinkedAttributeGridForm().AdministrationBar != null)
            {
                editorPage.getLinkedAttributeGridForm().AdministrationBar.Changed += OnChangeEventHandler;
            }

        }

        protected override void initializeSideBarData()
        {
            
        }

        protected override void initializeSideBarHandlers()
        {
            //((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.SelectionChanged += onSelectGridFromSidebar;
        }

        private void OnChangeEventHandler()
        {
            OnChange();
        }

        #endregion

    }
}
