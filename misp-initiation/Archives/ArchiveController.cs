using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Initiation.Archives
{
   public class ArchiveController : Controllable
    {
       public ArchiveController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// Assigne ou retourne le nom (ou code) de la fonctionnalité contrôlée. 
        /// </summary>
        public string FunctionalityCode { get; set; }

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

        public FileService fileService { get; set; }
       

        /// <summary>
        /// Effectue l'initialisation
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Initialize() { return OperationState.CONTINUE; }


        SimpleArchiveDialog simpleArchiveDialog { get; set; }

        AutomaticArchiveDialog automaticArchiveDialog { get; set; }

        public bool isSimpleArchive { get; set; }
        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si la création a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Create() 
        {
            if (isSimpleArchive)
            {
                configureSimpleArchive();
            }
            else
            {
                configureAutomaticArchive();
            }
           
            return OperationState.CONTINUE;
        }


        private void configureSimpleArchive()
        {
            simpleArchiveDialog = new SimpleArchiveDialog();
            simpleArchiveDialog.Owner = ApplicationManager.MainWindow;
            simpleArchiveDialog.fileService = fileService;
            Kernel.Domain.SimpleArchive simpleFileArchive = new Kernel.Domain.SimpleArchive();

            simpleFileArchive.name = this.ApplicationManager.File != null ? this.ApplicationManager.File.name : "Archive";
            simpleFileArchive.name += " " + DateTime.Now.ToString().Replace(":", "-").Replace("/", "-");
            simpleFileArchive.repository = Kernel.Util.UserPreferencesUtil.GetArchiveRepository();
            simpleFileArchive.repository = null;
            string defaultRepository = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + System.IO.Path.DirectorySeparatorChar + ".bcephal" + System.IO.Path.DirectorySeparatorChar + "Archives";
            if (string.IsNullOrWhiteSpace(simpleFileArchive.repository))
            {
                simpleFileArchive.repository = defaultRepository;
            }
            simpleFileArchive.isDefaultRepository = simpleFileArchive.repository == defaultRepository;
            simpleArchiveDialog.Display(simpleFileArchive);
            simpleArchiveDialog.ShowDialog();
        }


        private void configureAutomaticArchive()
        {
            automaticArchiveDialog = new AutomaticArchiveDialog();
            automaticArchiveDialog.Owner = ApplicationManager.MainWindow;
            automaticArchiveDialog.fileService = fileService;
            Kernel.Domain.ArchiveConfiguration automaticFileArchiveConfig = fileService.getArchiveConfiguration();
            if (string.IsNullOrWhiteSpace(automaticFileArchiveConfig.repository))
            {
                string defaultRepository = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) 
                    + System.IO.Path.DirectorySeparatorChar + ".bcephal" 
                    + System.IO.Path.DirectorySeparatorChar + "Archives";            
                automaticFileArchiveConfig.repository = defaultRepository;
            }
            
            automaticArchiveDialog.Display(automaticFileArchiveConfig);
            automaticArchiveDialog.ShowDialog();
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
        public OperationState Open(object oid) {
            isSimpleArchive = oid != null && oid is bool ? (bool)oid : false;
            return Create(); 
        }

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
