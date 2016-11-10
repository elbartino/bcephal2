using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;
using Misp.Kernel.Application;
using Misp.Initiation.Base;

namespace Misp.Initiation.Measure
{
    public class MeasureEditorController : EditorItemController<Misp.Kernel.Domain.Measure>
    {

        public InitiationController InitiationController { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MeasureEditorController(InitiationController InitiationController)
        {
            this.InitiationController = InitiationController;
            this.FunctionalityCode = InitiationFunctionalitiesCode.INITIATION_MEASURE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public MeasureEditorItem getMeasureEditor()
        {
            return (MeasureEditorItem)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux mesures.
        /// </summary>
        /// <returns>MeasureService</returns>
        public MeasureService GetMeasureService()
        {
            return (MeasureService)base.Service;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.MEASURE; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            Kernel.Domain.Measure root = GetMeasureService().getRootMeasure();
            getMeasureEditor().EditedObject = root;
            getMeasureEditor().ListChangeHandler = root.childrenListChangeHandler;
            getMeasureEditor().displayObject();
            return OperationState.CONTINUE;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau fichier se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
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

        public override OperationState Edit(object oid) { return OperationState.CONTINUE; }
                
        public override OperationState SaveAs() { return OperationState.CONTINUE; }

        public override OperationState Rename() { return OperationState.CONTINUE; }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE;}



        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new MeasureEditorItem(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new MeasureToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new MeasureToolBarHandlerBuilder(this.InitiationController, this); }

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
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected override void initializeViewHandlers() 
        {
            /*
            getMeasureEditor().getMeasureForm().measureGrid.AddingNewItem += new EventHandler<AddingNewItemEventArgs>(onAddMeasure);
            getMeasureEditor().getMeasureForm().measureGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(onCellEditEnding);
            getMeasureEditor().getMeasureForm().measureGrid.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(onMeasureNameChange);
            */
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers() { }



        protected void onAddMeasure(object sender, AddingNewItemEventArgs e) 
        {
            object item = e.NewItem;
            if (item != null) { }
        }

        protected void onCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridRow row = e.Row;
            DataGridColumn column = e.Column;
            Kernel.Domain.Measure measure = (Kernel.Domain.Measure)row.Item;
            if (measure != null)
            {

            }
        }

        protected void onMeasureNameChange(object sender, DataGridRowEditEndingEventArgs e) 
        {
            DataGridRow row = e.Row;
            Kernel.Domain.Measure measure = (Kernel.Domain.Measure)row.Item;
            if (measure != null) 
            {
            
            }
        }


        public override OperationState Search(object oid)
        {
            return OperationState.CONTINUE;
        }
    }
}
