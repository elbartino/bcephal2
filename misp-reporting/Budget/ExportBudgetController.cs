using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.Budget
{
   public  class ExportBudgetController : Controllable
    {
        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureEditorController.
        /// </summary>
        public ExportBudgetController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }
        #endregion

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
        public Misp.Kernel.Ui.Base.ToolBar ToolBar { get; set; }

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


        /// <summary>
        /// Effectue l'initialisation
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Initialize() { return OperationState.CONTINUE; }

        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si la création a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Create()
        {
            return openSaveDialog();
        }
       /// <summary>
       /// open dialog for export file
       /// </summary>
       /// <returns></returns>
        public OperationState openSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = "Export Budget";
            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_EXCEL;
            fileDialog.Filter = "Excel files (*" + HistoryHandler.FILE_EXTENSION_EXCEL+ ")|*" + HistoryHandler.FILE_EXTENSION_EXCEL;
            Nullable<bool> result = fileDialog.ShowDialog();

            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;

            if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return OperationState.STOP;

            Service.exportBudget(filePath);
            return OperationState.CONTINUE;
        }


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
        public void AfterSave() { }

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

        public ReportService Service { get; set; }

    }
}
