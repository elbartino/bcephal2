using Misp.Allocation.Run;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Allocation.Clear
{
    public class ClearAllocationController : Controllable
    {

        public ClearAllocationController()
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

        public ClearAllocationService Service { get; set; }

        /// <summary>
        /// Effectue l'initialisation
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Initialize() { return OperationState.CONTINUE; }


        RunWindow runWindow { get; set; }

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
            runWindow.SetRunClearLabel("Clear");
            runWindow.Controller = this;
            if (this.Service.getRunnedTableBrowserDatas().Count > 0)
            {
                runWindow.DisplayDatas(this.Service);
                runWindow.ShowDialog();
            }
            else MessageDisplayer.DisplayInfo("Clear Table", "There is no runned Table!");
            return OperationState.CONTINUE;
         //   return Clear(); 
        }

        protected System.Windows.Threading.DispatcherTimer runTimer;
        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Clear(TableActionData data=null)
        {
            OperationState state = OperationState.CONTINUE;
            string message = data == null ? "You are about to clear loaded tables.\n Do you want to continue ?" : "You are about to clear selected loaded tables.\n Do you want to continue ?";
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Clear allocation", message);
            if (response == MessageBoxResult.Yes)
            {
                Mask(true);
                Service.ClearAllocationTableHandler += updateClearAllocationProgress;
                if (data != null)
                {
                    Service.ClearAllocationTable(data);
                    
                }
                else
                {
                    Service.CleaAllAllocation();
                    runWindow.Close();
                }
            }
            return state;
        }

        private void updateClearAllocationProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                Service.ClearAllocationTableHandler -= updateClearAllocationProgress;
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
                Kernel.Util.MessageDisplayer.DisplayInfo("Clear allocation", "Clear allocation ended!");
                runWindow.DisplayDatas(this.Service);
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "Tables clearing: " + rate + " %"
                    + " (" + runInfo.runedCellCount + "/" + runInfo.totalCellCount + ")";
            }
        }


        protected void Mask(bool mask)
        {
            ApplicationManager.MainWindow.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = 0;
                ApplicationManager.MainWindow.LoadingLabel.Content = "Clear allocation";

                ApplicationManager.MainWindow.LoadingProgressBar.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingLabel.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingImage.Visibility = Visibility.Hidden;
            }
        }    



        private void updateProgressBar(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                runTimer.Stop();

                this.ApplicationManager.MainWindow.StatusBarLabel2.Content = "";
                this.ApplicationManager.MainWindow.SetPogressBar2Visible(false);
                Kernel.Util.MessageDisplayer.DisplayInfo("Clear table", "Clear table ended!");
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                this.ApplicationManager.MainWindow.StatusBarLabel2.Content = rate == 100 ? "Clear table ended!" : "Clear table...";
                this.ApplicationManager.MainWindow.UpdatePogressBar2(runInfo.runedCellCount, runInfo.totalCellCount, "" + rate + " %");
            }
        }

        protected virtual void OnClearTimerTick(object sender, EventArgs e)
        {
            AllocationRunInfo runInfo = Service.GetClearInfo();
            updateProgressBar(runInfo);
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
