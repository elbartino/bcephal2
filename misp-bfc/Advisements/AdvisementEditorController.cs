using Misp.Bfc.Model;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
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
           
        public AdvisementType advisementType { get; set; }

        #endregion


        #region Constructors

        public AdvisementEditorController(AdvisementType advisementType)
            : base()
        {
            this.advisementType = advisementType;
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = SubjectType.ADVISEMENT;
        }

        #endregion


        #region Operations

        public override Kernel.Application.OperationState Create()
        {
            Advisement advisement = new Advisement();
            advisement.advisementType = advisementType.ToString();
            advisement.creator = ApplicationManager.User.login;
            try
            {
                AdvisementEditorItem page = (AdvisementEditorItem)getAdvisementEditor().addOrSelectPage(advisement);
                initializePageHandlers(page);
                page.Title = advisementType.ToString();
                getAdvisementEditor().ListChangeHandler.AddNew(advisement);
            }
            catch (Exception) { }
            return OperationState.CONTINUE;
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
            return new AdvisementEditor(this.SubjectType, this.FunctionalityCode, this.advisementType);
        }

        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            return new Kernel.Ui.Base.ToolBar();
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

        #endregion


        #region Handlers

        protected override void initializeSideBarData()
        {

        }

        protected override void initializeSideBarHandlers()
        {
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.ADVISEMENT;
        }

        #endregion

    }
}
