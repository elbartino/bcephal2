﻿using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextEditorController : EditorController<Kernel.Domain.ReconciliationContext, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
       
        private PeriodName rootPeriodName { get; set; }

        private PeriodName defaultPeriodName { get; set; }
        #endregion

        public ReconciliationContextEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ReconciliationContextEditor getReconciliationContextEditor()
        {
            return (ReconciliationContextEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux reconciliation.
        /// </summary>
        /// <returns>ReconciliationService</returns>
        public ReconciliationContextService GetReconciliationContextService()
        {
            return (ReconciliationContextService)base.Service;
        }

        #endregion

        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle reco.
        /// </summary>
        /// <returns>CONTINUE si la création de la nouvelle reconciliation se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Kernel.Domain.ReconciliationContext reco = getCurrentReconciliationContext();

           ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationContextEditor().addOrSelectPage(reco);
            initializePageHandlers(page);
            page.Title = "Reconciliation Configuration";
            getReconciliationContextEditor().ListChangeHandler.AddNew(reco);
            return OperationState.CONTINUE;
        }

        private Kernel.Domain.ReconciliationContext getCurrentReconciliationContext()
        {
            Kernel.Domain.ReconciliationContext context = GetReconciliationContextService().getReconciliationContext();
            if (context == null) context = new Kernel.Domain.ReconciliationContext();
            return context;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.RECONCILIATION;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Kernel.Domain.ReconciliationContext reco)
        {
            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getEditor().addOrSelectPage(reco);
            initializePageHandlers(page);
            page.getReconciliationContextForm().displayObject();
            getEditor().ListChangeHandler.AddNew(reco);
            return OperationState.CONTINUE;
        }

        

        /**
         * get a new page name
         */
        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Kernel.Domain.ReconciliationContext target = GetObjectByName(name);
                if (target == null) return name;
                i++;
            }
            return name;
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        ReconciliationContextEditorItem currentPage = new ReconciliationContextEditorItem();

        public override OperationState Save(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            try
            {
                currentPage = (ReconciliationContextEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Save Reconciliation", "Unable to save Reconciliation.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

       
        /// <summary>
        /// handler on page selected
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            if (page == null)
            {
                return;
            }
            ReconciliationContexForm form = ((ReconciliationContextEditorItem)page).getReconciliationContextForm();
            if (form.ReconciliationContextPanel != null)
            {
            }
        }

        
        
        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState OnChange()
        {
            base.OnChange();
            return OperationState.CONTINUE;
        }

        
        public override OperationState Rename()
        {
            return OperationState.CONTINUE;

        }

        protected override void Rename(string name)
        {
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

         #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new  ReconciliationContextEditor();  }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new ReconciliationContextToolBar();
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
        protected override SideBar getNewSideBar() { return  new ReconciliationContextSideBar();}

        protected override PropertyBar getNewPropertyBar() { return new ReconciliationContextPropertyBar();}

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {

        }


        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            
            base.initializePageHandlers(page);
            ReconciliationContextEditorItem editorPage = (ReconciliationContextEditorItem)page;
            editorPage.getReconciliationContextForm().ReconciliationContextPanel.ActivatedItem += OnActivatedItem;
            editorPage.getReconciliationContextForm().ReconciliationContextPanel.Change += OnChangeItem;
        }

        private void OnChangeItem()
        {
            OnChange();
        }

        private void OnActivatedItem(object item)
        {
            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationContextEditor().getActivePage();
            if (page == null) return;
         //   page.getReconciliationContextForm().ReconciliationContextPanel.ActiveItem = (ReconciliationContext.ReconciliationContextItem)item;
        }

        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            ((ReconciliationContextSideBar)SideBar).EntityGroup.ModelService = GetReconciliationContextService().ModelService;
            ((ReconciliationContextSideBar)SideBar).EntityGroup.InitializeTreeViewDatas();

            Measure rootMeasure = GetReconciliationContextService().MeasureService.getRootMeasure();
            ((ReconciliationContextSideBar)SideBar).MeasureGroup.MeasureTreeview.DisplayRoot(rootMeasure);
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((ReconciliationContextSideBar)SideBar).EntityGroup.OnSelectAttributeValue += onSelectStandardTargetFromSidebar;
            ((ReconciliationContextSideBar)SideBar).EntityGroup.OnSelectTarget += onSelectStandardTargetFromSidebar;
            ((ReconciliationContextSideBar)SideBar).MeasureGroup.MeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
        }

        private void onSelectMeasureFromSidebar(object sender)
        {
            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationContextEditor().getActivePage();
            if (page == null) return;
            if (sender is Kernel.Domain.Measure)
            {
                page.getReconciliationContextForm().setMeasure((Kernel.Domain.Measure)sender);
            }
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
        }
        

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectStandardTargetFromSidebar(object sender)
        {
            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationContextEditor().getActivePage();
            if (page == null) return;
            if (sender is Kernel.Domain.AttributeValue) 
            {
                page.getReconciliationContextForm().setValue((Kernel.Domain.AttributeValue)sender);
            }
            else if (sender is Kernel.Domain.Attribute)
            {
                page.getReconciliationContextForm().setAttribute((Kernel.Domain.Attribute)sender);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnExpandAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = GetReconciliationContextService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    attribute.valueListChangeHandler.Items.Clear();
                    foreach (Kernel.Domain.AttributeValue value in values)
                    {
                        attribute.valueListChangeHandler.Items.Add(value);
                    }
                    attribute.LoadValues = true;
                }
            }
        }

       

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Kernel.Domain.ReconciliationContext GetObjectByName(string name)
        {
            return new Kernel.Domain.ReconciliationContext();
        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }

}
