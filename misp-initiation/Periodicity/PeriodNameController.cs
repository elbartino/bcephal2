using Misp.Initiation.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Initiation.Periodicity
{
    public class PeriodNameController : EditorItemController<PeriodName>
    {

        /// <summary>
        /// 
        /// </summary>
        public PeriodNameController()
        {
            Functionality = InitiationFunctionalitiesCode.PERIOD_FUNCTIONALITY;
            ModuleName = PlugIn.MODULE_NAME;
        }

        public PeriodNameEditor getPeriodNameEditor()
        {
            return (PeriodNameEditor)this.View;
        }

        public override EditorItem<PeriodName> getEditorItem()
        {
            return getPeriodNameEditorItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public PeriodNameEditorItem getPeriodNameEditorItem()
        {
            return getPeriodNameEditor().periodNameEditorItem;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux mesures.
        /// </summary>
        /// <returns>MeasureService</returns>
        public PeriodNameService GetPeriodNameService()
        {
            return (PeriodNameService)base.Service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            Kernel.Domain.PeriodName root = GetPeriodNameService().getRootMeasure();
            getPeriodNameEditorItem().EditedObject = root;
            getPeriodNameEditorItem().ListChangeHandler = root.childrenListChangeHandler;
            getPeriodNameEditorItem().displayObject();
            return OperationState.CONTINUE;
        }
                
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new PeriodNameEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new PeriodNameToolBar(); }

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

        public override OperationState Create() { return OperationState.CONTINUE; }
        public override OperationState Open(object oid) { return OperationState.CONTINUE; }
        public override OperationState TryToSaveBeforeClose()
        {
            if (!IsModify) return OperationState.CONTINUE;
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoCancelQuestion("", "Do you want to save change before close?");
            if (result == MessageBoxResult.Cancel) return OperationState.STOP;
            if (result == MessageBoxResult.No) return OperationState.CONTINUE;
            return Save();
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.DEFAULT; 
        }
        public override OperationState Edit(object oid) { return OperationState.CONTINUE; }
        public override OperationState SaveAs() { return OperationState.CONTINUE; }
        public override OperationState Rename() { return OperationState.CONTINUE; }
        public override OperationState Delete() { return OperationState.CONTINUE; }
        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }
        public override OperationState Search(object oid) { return OperationState.CONTINUE; }
        protected override PropertyBar getNewPropertyBar() { return null; }

        protected override void initializePropertyBarData() { }
        protected override void initializeViewData() { }
        protected override void initializeSideBarData() { }
        protected override void initializePropertyBarHandlers() { }
        protected override void initializeViewHandlers() { }
        protected override void initializeSideBarHandlers() { }

    }
}
