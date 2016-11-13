using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Application;
using Misp.Initiation.Base;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Input;
using Misp.Kernel.Task;
using Misp.Kernel.Util;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Initiation.Model
{
    public class ModelsEditorController : EditorController<Misp.Kernel.Domain.Model, Misp.Kernel.Domain.Browser.BrowserData>
    {

        public InitiationController InitiationController { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ModelsEditorController(InitiationController InitiationController)
        {
            this.InitiationController = InitiationController;
            this.FunctionalityCode = InitiationFunctionalitiesCode.INITIATION_MODEL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ModelEditor getModelEditor()
        {
            return (ModelEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux Models.
        /// </summary>
        /// <returns>PeriodicityService</returns>
        public ModelService GetModelService()
        {
            return (ModelService)base.Service;
        }

        BusyAction action;
        List<Kernel.Domain.Model> models;

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            List<Kernel.Domain.Model> models = GetModelService().getAll();
            foreach (Kernel.Domain.Model model in models)
            {
                EditorItem<Kernel.Domain.Model> page = getModelEditor().addOrSelectPage(model);
                ((ModelEditorItem)page).GetModelForm().ModelService = GetModelService();
            }
            return OperationState.CONTINUE;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.DEFAULT; 
        }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Search(object oid)
        {
            try
            {
                int idModel = int.Parse(oid.ToString());
                Kernel.Domain.Model model = GetModelService().getByOid(idModel);
                getModelEditor().getPageAndSelect(model.name);
            }
            catch (Exception) { }
            return OperationState.CONTINUE;
        }
        /// <summary>
        /// Cette methode permet de créer un nouveau Model.
        /// 
        /// 
        ///    
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
         
            Misp.Kernel.Domain.Model model = new Misp.Kernel.Domain.Model();
            model.name = getNewPageName("Model");
            model.modelFilename = Misp.Kernel.Util.UserPreferencesUtil.GetRecentFiles()[0];
            try
            {
                EditorItem<Misp.Kernel.Domain.Model> page = getModelEditor().addOrSelectPage(model);
                initializePageHandlers(page);
                page.Title = model.name;
                getModelEditor().ListChangeHandler.AddNew(model);
            }
            catch (Exception)
            {
            }
            return OperationState.CONTINUE;
        }

         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(object oid)
        {
            return OperationState.CONTINUE;
        }

        public override OperationState TryToSaveBeforeClose() { return OperationState.CONTINUE; }

        public override OperationState Edit(object oid) 
        {
             return  base.Open(oid);
        }
        
        public override OperationState SaveAs() { return OperationState.CONTINUE; }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }


        protected override void Rename(string name)
        {
            EditorItem<Kernel.Domain.Model> page = getModelEditor().getActivePage();
            page.EditedObject.name = name;
            base.Rename(name);
        }
        
        public override OperationState Delete() 
        {
            EditorItem<Misp.Kernel.Domain.Model> page = getModelEditor().getActivePage();
            Kernel.Domain.Model deletedModel = page.EditedObject;
            GetModelService().Delete(deletedModel.oid.Value);
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        {
            ModelEditor editor = new ModelEditor();
            Style LayoutDocumentTabItemStyle = new Style(typeof(LayoutDocumentTabItem));
            LayoutDocumentTabItemStyle.Setters.Add(new EventSetter(LayoutDocumentTabItem.MouseDoubleClickEvent, new MouseButtonEventHandler(OnPageTabDoubleClick)) { });
            LayoutDocumentTabItemStyle.Setters.Add(new EventSetter(LayoutDocumentTabItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPageTabMouseLeftButtonDown)) { });
            editor.manager.Resources.Add(typeof(LayoutDocumentTabItem), LayoutDocumentTabItemStyle);

            return editor; 
        }

        private void OnPageTabMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.Clear();
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override ToolBar getNewToolBar() { return new ModelToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ModelToolBarHandlerBuilder(this.InitiationController, this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new SideBar(); }

        protected override PropertyBar getNewPropertyBar() { return null; }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData() { }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers() { }

        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected override void initializeViewHandlers() 
        {
            base.initializeViewHandlers();
            getModelEditor().RenameMenuItem.Click += OnRename;
            getModelEditor().DeleteMenuItem.Click += OnDelete;
            getModelEditor().SaveMenuItem.Click += OnSave;
            getModelEditor().SaveAsMenuItem.Click += OnSavsAs;
        }

        protected  void OnPageTabDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Rename();
        }

        public override OperationState OnChange()
        {
            EditorItem<Kernel.Domain.Model> page = getModelEditor().getActivePage();
            page.EditedObject.refreshUnivWhenRun = true;
            
            return base.OnChange();
        }

        /// <summary>
        /// "Enregistre sous" le modèle selectionné
        /// </summary>
        /// <returns></returns>
        private void OnSavsAs(object sender, System.Windows.RoutedEventArgs e)
        {
            toolBarHandlerBuilder.onRenameButtonClic(sender, e);
        }

        /// <summary>
        /// Enregistre le modèle selectionné
        /// </summary>
        /// <returns></returns>
        private void OnSave(object sender, System.Windows.RoutedEventArgs e)
        { 
            bool disableSaveButton = true;
            Save(getModelEditor().getActivePage());
            foreach (EditorItem<Misp.Kernel.Domain.Model> page in getModelEditor().getPages())
                if (page.IsModify)
                {
                    disableSaveButton = false;
                    break;
                }
            
            if (disableSaveButton && this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = false; 
        }
        /// <summary>
        /// Supprime le modèle selectionné
        /// </summary>
        /// <returns></returns>
        private void OnDelete(object sender, System.Windows.RoutedEventArgs e)
        {
            EditorItem<Misp.Kernel.Domain.Model> page = getModelEditor().getActivePage();
            if (page == null) return;
            Kernel.Domain.Model deletedModel = page.EditedObject;

            if (deletedModel != null && deletedModel.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to delete this model." + "\n" + "You have to clear allocation before delete.";
                Kernel.Util.MessageDisplayer.DisplayWarning("delete Model", message);
                return;
            }

            if (getModelEditor().getPages().Count == 1)
            {
                string message = "You're not allowed to delete the last model";
                Kernel.Util.MessageDisplayer.DisplayWarning("delete Model", message);
                return;
            }

            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Model", "Do you really want to delete model?");
            if (result == MessageBoxResult.Yes)
            {
                page.Close();
                if (deletedModel.oid.HasValue) GetModelService().Delete(deletedModel.oid.Value);
            }
                /*
                Misp.Kernel.Domain.Model model=new Misp.Kernel.Domain.Model();
                model.name= page.Title;
                getModelEditor().ListChangeHandler.Items.Remove(model);
                Service.Delete2(page.EditedObject);
                  */
        }

        /// <summary>
        /// Methode qu permet de renommer un modèle
        /// </summary>
        /// <returns></returns>
        private void OnRename(object sender, System.Windows.RoutedEventArgs e)
        {
            toolBarHandlerBuilder.onRenameButtonClic(sender, e);            
        }
    }
}
