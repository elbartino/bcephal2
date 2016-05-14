using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Allocation.Run
{
    public class RunAllAllocationsController : Controllable
    {

        List<CellAllocationRunInfoBrowserData> liste { get; set; }
        int MAX_ELEMENTS = 75;
        int countElements { get; set; }

        public RunAllAllocationsController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// Assigne ou retourne le nom (ou code) de la fonctionnalité contrôlée. 
        /// </summary>
        public string Functionality { get; set; }

        /// <summary>
        /// Assigne ou retourne le nom du module auquel appartient la fonctionnalité contrôlée. 
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Retourne la barre d'outils liée à la fonctionnalité contrôlée.  
        /// </summary>
        public ToolBar ToolBar { get; set; }

        /// <summary>
        /// Retourne la barre de gauche liée à la fonctionnalité contrôlée. 
        /// </summary>
        public SideBar SideBar { get; set; }

        /// <summary>
        /// Retourne la barre de droite liée à la fonctionnalité contrôlée. 
        /// </summary>
        public PropertyBar PropertyBar { get; set; }

        /// <summary>
        /// Retourne la vue (ou écran) liée à la fonctionnalité contrôlée. 
        /// </summary>
        public IView View { get; set; }

        /// <summary>
        /// Assigne ou retourne le controller à partir duquel on a activé ce controller.
        /// </summary>
        public Controllable ParentController { get; set; }

        /// <summary>
        /// Assigne ou retourne l'ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Assigne ou retourne le NavigationToken lié à la fonctionnalité contrôlée.
        /// </summary>
        public NavigationToken NavigationToken { get; set; }

        /// <summary>
        ///  Assigne ou retourne la valeur indiquant
        ///  qu'une modification est survenue dans la vue liée à la fonctionnalité contrôlée.
        /// </summary>
        public bool IsModify { get; set; }

        public AllocationService Service { get; set; }

        /// <summary>
        /// Effectue l'initialisation
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Initialize() { return OperationState.CONTINUE; }

        private RunWindow runWindow { get; set; }

        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si la création a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Create() 
        {
            runWindow = new RunWindow();
            runWindow.Owner = ApplicationManager.MainWindow;
            runWindow.initializeGroup(this.Service);
            runWindow.SetRunClearLabel("Run");
            runWindow.Controller = this;
            runWindow.DisplayDatas(Service.getTableBrowserDatas());
            runWindow.ShowDialog();
            return OperationState.CONTINUE; 
        }
        
        protected System.Windows.Threading.DispatcherTimer runTimer;
        //protected DialogAllocationRun allocationRunDialog;

        
        public virtual OperationState Run(System.Collections.IList tables)
        {
            OperationState state = OperationState.CONTINUE;            
            TableActionData data = new TableActionData(tables);
            Mask(true);
            Service.RunAllocationTableHandler += updateRunProgress;
            Service.RunAllocationTable(data);
            runWindow.Close();
            return state;
        }

        private void updateRunProgress(AllocationRunInfo runInfo)
        {            
            if (runInfo == null || runInfo.runEnded == true)
            {
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.RunAllocationTableHandler -= updateRunProgress;
                Service.FileService.SaveCurrentFile();
                Kernel.Util.MessageDisplayer.DisplayInfo("Load Tables", "Load Tables ended!");
                Mask(false);
                ApplicationManager.MainWindow.treeDetails.Visibility = Visibility.Hidden;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "Tables loading: " + rate + " %"
                    + " (" + runInfo.runedCellCount + "/" + runInfo.totalCellCount + ")";

                if (runInfo.currentInfo != null)
                {
                    rate = runInfo.currentInfo.totalCellCount != 0 ? (Int32)(runInfo.currentInfo.runedCellCount * 100 / runInfo.currentInfo.totalCellCount) : 0;
                    if (rate > 100) rate = 100;

                    if (runInfo.currentInfo.runedCellCount != 0)
                    {
                        ApplicationManager.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = runInfo.currentInfo.totalCellCount;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Value = runInfo.currentInfo.runedCellCount;
                        ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = runInfo.currentInfo.tableName + ": " + rate + " %"
                            + " (" + runInfo.currentInfo.runedCellCount + "/" + runInfo.currentInfo.totalCellCount + ")";
                    }
                }

            }
        }

        protected void Mask(bool mask, string content = "Running...")
        {            
            ApplicationManager.MainWindow.BusyBorder2.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (!mask)
            {
                ApplicationManager.MainWindow.ProgressBarTree.Maximum = 100;
                ApplicationManager.MainWindow.ProgressBarTree.Value = 0;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "";
            }
        }    
               

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'ouverture a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Open() { return Create(); }

        /// <summary>
        /// Affiche un objet dans la vue d'édition
        /// </summary>
        /// <param name="oid">L'identifian de l'objet</param>
        /// <returns>
        /// OperationState.CONTINUE si l'ouverture a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Open(object oid) { return Create(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Edit(object oid) { return Open(oid); }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Search() { return OperationState.CONTINUE; }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <param name="oid"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Search(object oid) { return Search(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState TryToSaveBeforeClose() { return OperationState.CONTINUE; }

        /// <summary>
        /// Ferme ce controller
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Close() { return OperationState.CONTINUE; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Save() { return OperationState.CONTINUE; }
        public void AfterSave() {  }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState SaveAll() { return OperationState.CONTINUE; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState SaveAs() { return OperationState.CONTINUE; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Rename() { return OperationState.CONTINUE; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Delete() { return OperationState.CONTINUE; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState OnChange() { return OperationState.CONTINUE; }

    }
}
