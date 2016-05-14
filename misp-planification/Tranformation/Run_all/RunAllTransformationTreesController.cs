using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Planification.Tranformation.Run_all
{
   public  class RunAllTransformationTreesController : Controllable
    {
        int MAX_ELEMENTS = 75;
        int countElements { get; set; }
        public bool isClearOption { get; set; }
        

        public RunAllTransformationTreesController()
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

        public TransformationTreeService Service { get; set; }

        /// <summary>
        /// Effectue l'initialisation
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Initialize() { return OperationState.CONTINUE; }

        private Misp.Planification.Tranformation.Run_all.LoadTransformationTreesDialog loadTransformationTreesDialog { get; set; }

        protected TransformationTreeService GetTransformationTreeService() { return (TransformationTreeService)Service; }
        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si la création a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Create() 
        {
            loadTransformationTreesDialog = new LoadTransformationTreesDialog();
            loadTransformationTreesDialog.Owner = ApplicationManager.MainWindow;
            loadTransformationTreesDialog.initializeGroup(this.Service);
            loadTransformationTreesDialog.setDialogTitle(getDialogTitle());
            loadTransformationTreesDialog.SetRunClearLabel(getRunButtonText());
            loadTransformationTreesDialog.RunAllTransformationTreesController = this;

            List<Misp.Kernel.Domain.Browser.TransformationTreeBrowserData> listeToDisplay =
            isClearOption ? Service.getRunnedTransformationTreeBrowserDatas() : Service.getTransformationTreeBrowserDatas();
            if (listeToDisplay == null || listeToDisplay.Count == 0)
            {
                Kernel.Util.MessageDisplayer.DisplayInfo(getNoItemsTitle(), getNoItemsMessage());
            }
            else
            {
                loadTransformationTreesDialog.DisplayDatas(listeToDisplay);
                loadTransformationTreesDialog.ShowDialog();
            }
            return OperationState.CONTINUE; 
        }

        protected System.Windows.Threading.DispatcherTimer runTimer;
       
        public virtual OperationState Run(System.Collections.IList trees)
        {
            OperationState state = OperationState.CONTINUE;
            List<int> stringOid = new List<int>();
            Misp.Kernel.Domain.Browser.TransformationTreeBrowserData bData;
            TableActionData tableActionData = new TableActionData();
            List<Misp.Kernel.Domain.Browser.TransformationTreeBrowserData> ListDatas = new List<Kernel.Domain.Browser.TransformationTreeBrowserData>(0);
            foreach (object tree in trees)
            {
                bData = (Misp.Kernel.Domain.Browser.TransformationTreeBrowserData)tree;
                int oid = (int)bData.oid;
                if (isClearOption) tableActionData.oids.Add(oid);
                else stringOid.Add(oid);
            }

            if (isClearOption)
            {
                if (ListDatas.Count > 0) tableActionData = new TableActionData(ListDatas);
                Service.ClearTreeHandler += updateClearProgress;
                GetTransformationTreeService().ClearTree(tableActionData);
            }
            else
            {
                Service.RunHandler += updateRunProgress;
                Service.PowerpointHandler += loadPowerpoint;
                GetTransformationTreeService().Run(stringOid, true);
            }
            
            Mask(true, getMaskStartText());
            return state;
        }

        private void loadPowerpoint(Kernel.Ui.Office.PowerpointLoadInfo info)
        {
            if (info == null) return;
            PowerpointLoader.Load(info);
        }

        private void updateClearProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                GetTransformationTreeService().ClearTreeHandler -= updateClearProgress;
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
            }
        }
    
        private void updateRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                Service.RunHandler -= updateRunProgress;
                Mask(false, getMaskEndedText());
                Service.FileService.SaveCurrentFile();
                loadTransformationTreesDialog.treeDetails.Visibility = Visibility.Hidden;
                loadTransformationTreesDialog.ProgressBarTreeContent.Maximum = 0;
                loadTransformationTreesDialog.ProgressBarTreeContent.Value = 0;
                loadTransformationTreesDialog.statusTextBlockTreeContent.Content = "";
                loadTransformationTreesDialog.DisplayDatas(Service.getTransformationTreeBrowserDatas());  
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;

                loadTransformationTreesDialog.ProgressBarTree.Maximum = info.totalCount;
                loadTransformationTreesDialog.ProgressBarTree.Value = info.runedCount;
                loadTransformationTreesDialog.statusTextBlockTree.Content = "" + rate + " % " + " (" + info.runedCount + " / " + info.totalCount + ")";

                if (info.currentTreeRunInfo != null)
                {
                    rate = info.currentTreeRunInfo.runedCount != 0 ? (Int32)(info.currentTreeRunInfo.runedCount * 100 / info.currentTreeRunInfo.totalCount) : 0;
                    if (rate > 100) rate = 100;

                    if (info.currentTreeRunInfo.runedCount != 0)
                    {
                        loadTransformationTreesDialog.treeDetails.Visibility = Visibility.Visible;
                        loadTransformationTreesDialog.ProgressBarTreeContent.Maximum = info.currentTreeRunInfo.totalCount;
                        loadTransformationTreesDialog.ProgressBarTreeContent.Value = info.currentTreeRunInfo.runedCount;
                        loadTransformationTreesDialog.statusTextBlockTreeContent.Content = ""+ info.currentTreeRunInfo.item +" :  "+ rate + " %";
                    }
                }

            }
        }

        protected void Mask(bool mask, string content = "Saving...")
        {
            loadTransformationTreesDialog.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
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

        #region Message Utils
            private string getRunButtonText()
            {
                return isClearOption ? "Clear" : "Run";
            }

            private string getMaskStartText()
            {
                return isClearOption ? "Clear..." : "Running...";
            }

            private string getMaskEndedText()
            {
                return isClearOption ? "Clear ended" : "Run ended";
            }

            private string getDialogTitle() {
                return isClearOption ? "Clear Transformation trees - Select items" : "Run Transformation trees - Select items";
            }

            private string getNoItemsTitle() 
            {
                return isClearOption ? "Clear All TransformationTrees " : "Run All TransformationTrees";
            }

            private string getNoItemsMessage()
            {
                return isClearOption ? "No TransformationTree has been runned" : "No TransformationTree created ";
            }
        #endregion

    }
}
