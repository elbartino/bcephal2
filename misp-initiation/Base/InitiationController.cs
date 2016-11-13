using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Misp.Kernel.Controller;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using Misp.Initiation.Service;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Initiation.Base
{
    public class InitiationController : Controller<Persistent, Misp.Kernel.Domain.Browser.BrowserData>
    {

        private Model.ModelsEditorController modelController;
        private Measure.MeasureEditorController measureController;

        public Controllable ActiveController { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public InitiationController()
        {
            FunctionalityCode = InitiationFunctionalitiesCode.INITIATION;
            ModuleName = PlugIn.MODULE_NAME;
        }


        public InitiationService GetInitiationService()
        {
            return (InitiationService)base.Service;
        }

        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue
        /// </summary>
        /// <returns></returns>
        public override OperationState OnChange()
        {
            this.IsModify = true;
            this.ActiveController.OnChange();            
            if (getModelController().ToolBar != null) getModelController().ToolBar.SaveButton.IsEnabled = true;
            if (getMeasureController().ToolBar != null) getMeasureController().ToolBar.SaveButton.IsEnabled = true;
            return OperationState.CONTINUE;
        }

      
        /// <summary>
        /// Methode executée quand la sauvegare est effectuée avec succes
        /// </summary>
        public override void AfterSave()
        {
            this.IsModify = false;
            this.getModelController().AfterSave();
            this.getMeasureController().AfterSave();
        }

        public override OperationState Open()
        {
            return Search();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Search() 
        {
            getModelController().Search();
            getMeasureController().Search();
            return OperationState.CONTINUE;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.DEFAULT;
        }

        public override OperationState Search(object oid)
        {
            getModelController().Search(oid);
            return OperationState.CONTINUE; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Create() {
            if (this.Search() == OperationState.STOP) return OperationState.STOP;
            return getModelController().Create();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(object oid)
        {
            return Edit(oid);
        }

        public override OperationState Edit(object oid) 
        {
            if (this.Search() == OperationState.STOP) return OperationState.STOP;
            EditorItem<Kernel.Domain.Model> page = getModelController().getEditor().getPage((int)oid);
            getModelController().getEditor().selectePage(page);
            return OperationState.CONTINUE;
            //return getModelController().Edit(oid); 
        }

        public override OperationState Save() 
        {
            if (getModelController().SaveAll() == OperationState.CONTINUE)
            {
                if (getMeasureController().Save() == OperationState.CONTINUE)
                {
                    Service.FileService.SaveCurrentFile();
                    return OperationState.CONTINUE;
                }
            }
            Service.FileService.SaveCurrentFile();
            return OperationState.STOP; 
        }

        public override OperationState SaveAll() 
        {
            return Save(); 
        }

        public override OperationState SaveAs() { return OperationState.CONTINUE; }

        public override OperationState Rename() { return OperationState.CONTINUE; }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }


        // <summary>
        /// Cette methode est lorsque la fermeture du controller a réussit. 
        /// Cette methode va déactiver tous les menus sauf le menu file.
        /// </summary>
        /// <returns></returns>
        protected override void AfterClose()
        {
            RemoveCommands();
            getModelController().getModelEditor().getLayoutContent().IsActiveChanged -= handler;
            getMeasureController().getMeasureEditor().IsActiveChanged -= handler;
        }

        /// <summary>
        /// Cette méthode est appelée avant de fermer la vue contôlée.
        /// Elle demande à l'utilisateur s'il veut sauver les éventuels 
        /// modifications avant la fermeture de la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState TryToSaveBeforeClose()
        {
            if (!IsModify) return OperationState.CONTINUE;
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoCancelQuestion("", "Do you want to save change before close?");
            if (result == MessageBoxResult.Cancel) return OperationState.STOP;
            if (result == MessageBoxResult.No) return OperationState.CONTINUE;
            return Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IView getNewView() 
        {
            InitiationEditor editor = new InitiationEditor();
            editor.Initializepages(getModelController().getModelEditor(), getMeasureController().getMeasureEditor());
            ActiveController = getModelController();
            this.toolBar = ActiveController.ToolBar;
            this.sideBar = ActiveController.SideBar;
            this.propertyBar = ActiveController.PropertyBar;
            return editor; 
        }

        public InitiationEditor getInitiationEditor()
        {
            return (InitiationEditor)this.view;
        }

        public Model.ModelsEditorController getModelController()
        {
            if (modelController == null)
            {
                modelController = new Model.ModelsEditorController(this);
                modelController.ChangeEventHandler = this.ChangeEventHandler;
                modelController.Initialize();
                modelController.Service = ((InitiationService)this.Service).ModelService;
                modelController.ApplicationManager = this.ApplicationManager;
            }
            return modelController;
        }

        public Measure.MeasureEditorController getMeasureController()
        {
            if (measureController == null)
            {
                measureController = new Measure.MeasureEditorController(this);
                measureController.ChangeEventHandler = this.ChangeEventHandler;
                measureController.Initialize();
                measureController.Service = ((InitiationService)this.Service).MeasureService;
                measureController.ApplicationManager = this.ApplicationManager;
            }
            
            return measureController;
        }
        
        /// <summary>
        /// The tool bar used to manage file.
        /// </summary>
        /// <returns>New instance of FileToolBar</returns>
        protected override ToolBar getNewToolBar() { return null; }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return null; }
        
        protected override SideBar getNewSideBar() { return null; }

        protected override void initializeViewData() {  }

        protected override void initializeSideBarData() {  }


        protected override PropertyBar getNewPropertyBar() { return null; }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        EventHandler handler;
        protected override void initializeViewHandlers() 
        {
            handler = new EventHandler(this.OnActiveViewChenged);
            getModelController().getModelEditor().getLayoutContent().IsActiveChanged += handler;
            getMeasureController().getMeasureEditor().IsActiveChanged += handler;
        }


        protected override void initializeSideBarHandlers() {  }


        protected override void initializeCommands()
        {
            base.initializeCommands();
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(SaveCommandBinding);
            }
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveCommandBinding);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnActiveViewChenged(object sender, EventArgs args) 
        {            
            if ((ActiveController == null || !ActiveController.Equals(getMeasureController())) && sender is Measure.MeasureEditorItem)
            {
                ActiveController = getMeasureController();
                
            }            
            else if ((ActiveController == null || !ActiveController.Equals(getModelController())) && sender.Equals(getModelController().getModelEditor().getLayoutContent()))
            {
                ActiveController = getModelController();
                this.toolBar = ActiveController.ToolBar;
            }

            this.toolBar = ActiveController.ToolBar;
            this.sideBar = ActiveController.SideBar;
            this.propertyBar = ActiveController.PropertyBar;
            this.sideBar.SelectStatus(ModuleName);
            ApplicationManager.MainWindow.displayToolBar(this.toolBar);
            ApplicationManager.MainWindow.displaySideBar(this.sideBar);
            ApplicationManager.MainWindow.displayPropertyBar(this.propertyBar);
        }


        
    }
}
