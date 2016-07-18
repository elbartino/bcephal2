using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class AutomaticSourcingGridEditorController : AutomaticSourcingEditorController
    {
      
        #region Editor and Service

        //AutomaticGridDataDialog dialog;
        protected override void performRun(AutomaticSourcingEditorItem page)
        {
            //dialog = new AutomaticGridDataDialog();
            //dialog.InputGridService = ApplicationManager.ControllerFactory.ServiceFactory.GetInputGridService();
            //dialog.loadGrids();
            //dialog.NewGridNameTextBox.Text = page.getAutomaticSourcingForm().SpreadSheet.DocumentName;
            //dialog.cancelButton.Click += OnCancelAutomaticGridDataDialog;
            //dialog.runButton.Click += OnRunAutomaticGridDataDialog;
            //dialog.ShowDialog();
            
            AutomaticGridData data = new AutomaticGridData();
            GetAutomaticSourcingService().SaveTableHandler += UpdateSaveInfo;
            GetAutomaticSourcingService().OnUpdateUniverse += OnUpdateUniverse;
            GetAutomaticSourcingService().buildTableNameEventHandler += OnBuildTableName;
            Mask(true, "Running ...");
            data.automaticSourcingOid = page.EditedObject.oid.Value;
            data.excelFilePath = page.getAutomaticSourcingForm().SpreadSheet.DocumentUrl;
            GetAutomaticSourcingService().Run(data);
        }

        private void OnRunAutomaticGridDataDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            //AutomaticGridData data = dialog.Fill();
            AutomaticGridData data = null;

            if (data == null) return;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            GetAutomaticSourcingService().SaveTableHandler += UpdateSaveInfo;
            GetAutomaticSourcingService().OnUpdateUniverse += OnUpdateUniverse;
            GetAutomaticSourcingService().buildTableNameEventHandler += OnBuildTableName;
            Mask(true, "Running ...");            
            data.automaticSourcingOid = page.EditedObject.oid.Value;
            data.excelFilePath = page.getAutomaticSourcingForm().SpreadSheet.DocumentUrl;
            OnCancelAutomaticGridDataDialog(sender, e);
            GetAutomaticSourcingService().Run(data);            
        }

        private void OnCancelAutomaticGridDataDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            //dialog.cancelButton.Click -= OnCancelAutomaticGridDataDialog;
            //dialog.runButton.Click -= OnRunAutomaticGridDataDialog;
            //dialog.Close();
            //dialog = null;
        }
        

        /// <summary>
        /// Service pour acceder aux opérations liés à l'automaticSourcing.
        /// </summary>
        /// <returns>DesignService</returns>
        public virtual AutomaticSourcingGridService GetAutomaticSourcingGridService()
        {
            return (AutomaticSourcingGridService)base.Service;
        }

        public override bool isAutomaticGrid()
        {
            return true;
        }
                    

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            AutomaticSourcingToolBar toolBar = new AutomaticSourcingToolBar();
            toolBar.RunButton.ToolTip = "Run Automatic Grid";
            toolBar.SaveButton.ToolTip = "Save Automatic Grid";
            toolBar.CloseButton.ToolTip = "Exit Automatic Grid";
            return toolBar;
        }

        protected override Kernel.Ui.Base.SideBar getNewSideBar()
        {
            AutomaticSourcingSideBar sideBar = new AutomaticSourcingSideBar();
            sideBar.AutomaticSourcingGroup.Header = "Automatic Grid";
            return sideBar;
        }


        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.AUTOMATIC_GRID;
        }

        #endregion


        protected override Kernel.Domain.AutomaticSourcing GetNewAutomaticSourcing()
        {
            Kernel.Domain.AutomaticSourcing automaticGrid = new Kernel.Domain.AutomaticSourcing();
            automaticGrid.name = getNewPageName("Grid");
            automaticGrid.isGrid = true;
            automaticGrid.isAutomaticGrid = true;
            automaticGrid.group = GetAutomaticSourcingGridService().GroupService.getDefaultGroup();
            return automaticGrid;
        }             

    }
}
