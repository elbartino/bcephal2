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

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public AdvisementEditor getAdvisementEditor()
        {
            return (AdvisementEditor)this.View;
        }
               

        #endregion


        /// <summary>
        /// Default constructor
        /// </summary>
        public AdvisementEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = SubjectType.ADVISEMENT;
        }

        public AdvisementEditorController(AdvisementType advisementType) : base()
        {
            this.advisementType = advisementType;
        }

        public override Kernel.Application.OperationState Delete()
        {
            return OperationState.CONTINUE;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.ADVISEMENT;
        }

        public override Kernel.Application.OperationState Create()
        {
            if (advisementType.ToString().Equals(AdvisementType.PREFUNDING.ToString()))
            {

            }

            if (advisementType.ToString().Equals(AdvisementType.SETTLEMENT.ToString()))
            {

            }

            if (advisementType.ToString().Equals(AdvisementType.EXCEPTIONAL.ToString()))
            {

            }


            return OperationState.CONTINUE;
        }

        protected override Kernel.Ui.Base.IView getNewView()
        {
             return new AdvisementEditor(this.SubjectType, this.FunctionalityCode); 
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

        protected override void initializeSideBarData()
        {

        }

        protected override void initializeSideBarHandlers()
        {
        }
    }
}
