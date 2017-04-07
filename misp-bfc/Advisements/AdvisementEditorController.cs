using Misp.Bfc.Model;
using Misp.Bfc.Service;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Advisements
{
    public class AdvisementEditorController : EditorController<Advisement, AdvisementBrowserData>
    {
        
        #region Properties
           
        public AdvisementType AdvisementType { get; set; }

        #endregion


        #region Constructors

        public AdvisementEditorController(AdvisementType advisementType)
            : base()
        {
            this.AdvisementType = advisementType;
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = SubjectType.ADVISEMENT;
        }

        #endregion


        #region Operations

        public override Kernel.Application.OperationState Create()
        {
            Advisement advisement = new Advisement();
            advisement.advisementType = AdvisementType.ToString();
            advisement.creator = ApplicationManager.User.login;
            advisement.valueDateTime = DateTime.Now;
            try
            {
                AdvisementEditorItem page = (AdvisementEditorItem)getAdvisementEditor().addOrSelectPage(advisement);
                initializePageHandlers(page);
                page.Title = advisement.ToString();
                getAdvisementEditor().ListChangeHandler.AddNew(advisement);
            }
            catch (Exception) { }
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void initializeCommands()
        {
            base.initializeCommands();
            ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveAsMenuItem);
            ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(DeleteMenuItem);
            ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RenameMenuItem);
        }


        public override Kernel.Application.OperationState Delete() { return OperationState.CONTINUE; }

        #endregion
                

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public AdvisementEditor getAdvisementEditor()
        {
            return (AdvisementEditor)this.View;
        }

        protected override Kernel.Ui.Base.IView getNewView()
        {
            return new AdvisementEditor(this.SubjectType, this.FunctionalityCode, this.AdvisementType);
        }

        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            return new AdvisementToolBar();
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new ToolBarHandlerBuilder(this);
        }

        protected override Kernel.Ui.Sidebar.SideBar getNewSideBar()
        {
            return new Kernel.Ui.Sidebar.SideBar();
        }

        protected override Kernel.Ui.Base.PropertyBar getNewPropertyBar()
        {
            return null;
        }

        protected AdvisementService getAdvisementService()
        {
            return (AdvisementService)Service;
        }

        #endregion


        #region Initialization

        public override OperationState Initialize()
        {
            base.Initialize();
            getAdvisementEditor().Service = getAdvisementService();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Advisement> item)
        {
            AdvisementEditorItem page = (AdvisementEditorItem)item;
            base.initializePageHandlers(page);
            page.getAdvisementForm().SelectionChanged += OnSelectionChanged;
            page.getAdvisementForm().OkButton.Click += OnOkClick;
            page.getAdvisementForm().CancelButton.Click += OnCancelClick;
        }


        protected override void initializeSideBarData() { }
        
        public override SubjectType SubjectTypeFound() { return SubjectType.ADVISEMENT; }

        protected bool isPrefunding()
        {
            return this.AdvisementType == AdvisementType.PREFUNDING;
        }

        protected bool isMember()
        {
            return this.AdvisementType == AdvisementType.MEMBER;
        }

        protected bool isReplenishment()
        {
            return this.AdvisementType == AdvisementType.REPLENISHMENT;
        }

        protected bool isSettlement()
        {
            return this.AdvisementType == AdvisementType.SETTLEMENT;
        }

        #endregion


        #region Handlers


        private void OnSelectionChanged()
        {
            AdvisementEditorItem page = (AdvisementEditorItem)getEditor().getActivePage();
            if (isPrefunding())
            {
                //getAdvisementService().getAlready();
            }
        }

        private void OnCancelClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AdvisementEditorItem page = (AdvisementEditorItem)getEditor().getActivePage();
        }

        private void OnOkClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AdvisementEditorItem page = (AdvisementEditorItem)getEditor().getActivePage();
            Save(page);
        }
        
        protected override void initializeSideBarHandlers() { }

        #endregion

    }
}
