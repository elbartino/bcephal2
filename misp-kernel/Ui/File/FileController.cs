using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Misp.Kernel.Controller;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using Misp.Kernel.Task;

namespace Misp.Kernel.Ui.File
{
    public class FileController : Controller<Misp.Kernel.Domain.File, Domain.Browser.BrowserData>
    {

        public static string FILE_EXTENSION = ".bcp";

        public FileController()
        {
            FunctionalityCode = FunctionalitiesCode.FILE_FUNCTIONALITY;
        }


        public FileService GetFileInfoService()
        {
            return (FileService)base.Service;
        }

        public void RefreshDashboard()
        {
            try
            {
                //DashboardView.FileService = this.GetFileInfoService();
                //FileOpenedView.FileService = this.GetFileInfoService();
                //this.ApplicationManager.MainWindow.FileOpenedView.BuildRecentOpenedContols();
            }
            catch (Exception) { }
            this.ApplicationManager.MainWindow.FileClosedView.Visibility = ApplicationManager.Instance.File == null ? Visibility.Visible : Visibility.Collapsed;
            //this.ApplicationManager.MainWindow.FileOpenedView.Visibility = ApplicationManager.Instance.File != null ? Visibility.Visible : Visibility.Collapsed;
            this.ApplicationManager.MainWindow.DashboardView.DashBoardService = this.GetFileInfoService().DashBoardService;
            this.ApplicationManager.MainWindow.DashboardView.Visibility = ApplicationManager.Instance.File != null ? Visibility.Visible : Visibility.Collapsed;
            if (ApplicationManager.Instance.File != null)
            {
                this.ApplicationManager.MainWindow.DashboardView.Refresh();
            }
        }

        /**
         * return a reconciliation service
         * 
         */
        public void getReconciliationService()
        {
            this.ApplicationManager.MainWindow.DashboardView.DashBoardService = this.GetFileInfoService().DashBoardService;
            
        }


        public override OperationState Open()
        {            
            return Search();
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.DEFAULT;
        }

        /// <summary>
        /// Cette methode permet de créer un nouveau File.
        /// 
        /// 1. On affiche le dialog de fichier pour pemettre à l'utilisateur 
        ///    d'indiquer le nom du fichier et le répertoire dans lequel le créer.
        ///    
        /// 2. On valide le nom et le répertoire pour se rassurer 
        ///    qu'il n'existe pas un fichier de même nom dans ce répertoire.
        ///    
        /// 3. On transmet le nom et le répertoire au serveur 
        ///    qui se charge de créer effectivement le fichier 
        ///    et renvoie les références du nouveau fichier.
        ///    
        /// 4. Si durant ce processus une erreur survient, 
        ///    un message d'erreur est afficher et le processus est interromu.
        ///    
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau fichier se termine avec succès. STOP sinon</returns>
        public override OperationState Create() {
            try
            {
                string pathDirectory = Kernel.Util.UserPreferencesUtil.GetFileOpeningRepository();
                string filePath = openFileDialogForFolders("New Project", pathDirectory);
                if (filePath == null || string.IsNullOrWhiteSpace(filePath)) return OperationState.STOP;
                return OpenOrCreate(filePath, true);
            }
            catch(BcephalException e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return OperationState.STOP;
            }
        }

        /// <summary>
        /// Cette methode permet d'ouvrir un fichier existant.
        /// 
        /// 1. On affiche le dialog de fichier pour pemettre à l'utilisateur 
        ///    de sélectionner un fichier à ouvrir.
        ///    
        /// 2. On valide le nom pour se rassurer que ce fichier existe bien.
        ///    
        /// 3. On transmet le nom et le répertoire au serveur 
        ///    qui se charge d'effectuer l'ouverture du fichier 
        ///    et renvoie les références du fichier ainsi ouvert.
        ///    
        /// 4. Si durant ce processus une erreur survient, 
        ///    un message d'erreur est afficher et le processus est interromu.
        ///    
        /// </summary>
        /// <returns>CONTINUE si l'ouverture du fichier se termine avec succès. STOP sinon</returns>
        public override OperationState Search()
        {
            try
            {
                string pathDirectory = Kernel.Util.UserPreferencesUtil.GetFileOpeningRepository();
                string filePath = openFileDialogForMispFiles("Open File", pathDirectory);
                if (filePath == null || string.IsNullOrWhiteSpace(filePath)) return OperationState.STOP;
                return Open(filePath);
            }
            catch (BcephalException e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return OperationState.STOP;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(object oid)
        {
            try
            {
                if (oid == null || !(oid is string))
                {
                    MessageDisplayer.DisplayError("Error", "");
                    return OperationState.STOP;
                }
                bool isMonouser = ApplicationManager.ApplcationConfiguration.IsMonouser();
                string filePath = (string)oid;
                if (isMonouser && ApplicationManager.useZip() && !System.IO.File.Exists(filePath))
                {
                    MessageDisplayer.DisplayError("Error", "File not found: " + filePath);
                    return OperationState.STOP;
                }
                if (isMonouser && !ApplicationManager.useZip() && !System.IO.Directory.Exists(filePath))
                {
                    MessageDisplayer.DisplayError("Error", "Directory not found: " + filePath);
                    return OperationState.STOP;
                }
                return OpenOrCreate(filePath, false);
            }
            catch (BcephalException e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return OperationState.STOP;
            }
        }

        BusyAction action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Chemin absolue vers le fichier à ouvrir ou à créer</param>
        /// <param name="create">Est ce la creation</param>
        /// <returns></returns>
        public OperationState OpenOrCreate(string filePath, bool create)
        {
            action = new BusyAction(false)
            {
                DoWork = () =>
                {
                    try
                    {
                        bool isMonouser = ApplicationManager.ApplcationConfiguration.IsMonouser();
                        String message = create ? "File creation..." : "File loading...";
                        action.ReportProgress(0, message);
                        if (isMonouser && !System.IO.File.Exists(filePath) && !create && ApplicationManager.useZip())
                        {
                            MessageDisplayer.DisplayError("Error", "File not found: " + filePath);
                            return OperationState.STOP;
                        }
                        if (isMonouser && !System.IO.Directory.Exists(filePath) && !create && !ApplicationManager.useZip())
                        {
                            MessageDisplayer.DisplayError("Error", "Directory not found: " + filePath);
                            return OperationState.STOP;
                        }

                        string fileDir = System.IO.Path.GetDirectoryName(filePath);
                        string fileName = System.IO.Path.GetFileName(filePath);
                        //string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);

                        action.ReportProgress(10, message);
                        
                        Misp.Kernel.Domain.File file = create ?
                            this.GetFileInfoService().CreateFile(fileDir, fileName) :
                            this.GetFileInfoService().OpenFile(fileDir, fileName);

                        message = create ? "File created!" : "File loaded!";
                        action.ReportProgress(99, message);

                        if (file == null) return OperationState.STOP;
                        this.ApplicationManager.File = file;
                        this.ApplicationManager.AllocationCount = this.GetFileInfoService().GetAllocationCount();
                        
                        Util.UserPreferencesUtil.AddRecentFile(filePath);
                        Util.UserPreferencesUtil.SetFileOpeningRepository(filePath);
                       action.ReportProgress(100, message);                       
                    }
                    catch (BcephalException e)
                    {
                        MessageDisplayer.DisplayError(create ? "Create file" : "Open file", e.Message);
                        action = null;
                        return OperationState.STOP;
                    }
                    return OperationState.CONTINUE;
                },


                EndWork = () =>
                {
                    try
                    {
                        string fileNameWithoutExtension = "";
                        if (filePath.EndsWith(FILE_EXTENSION)) fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        else fileNameWithoutExtension = System.IO.Path.GetFileName(filePath);
                        ApplicationManager.MainWindow.Title = fileNameWithoutExtension + " - B-Cephal";
                        ApplicationManager.MainWindow.MenuBar.customizeForFileOpened();
                        this.ToolBar.DisplayAllControls();
                        ApplicationManager.MainWindow.MenuBar.GetFileMenu().lastFilePath = filePath;
                        ApplicationManager.MainWindow.MenuBar.GetFileMenu().BuildSaveAsMenu(); 
                        ApplicationManager.MainWindow.MenuBar.GetFileMenu().BuildRecentOpenedFiles();
                        RefreshDashboard();                        
                    }
                    catch (Exception e)
                    {
                        MessageDisplayer.DisplayError("Error", e.Message);
                        return OperationState.STOP;
                    }
                    finally
                    {
                        action = null;
                    }

                    return OperationState.CONTINUE;
                }

            };

            action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.MainWindow.OnBusyPropertyChanged);
            action.Run();
            return OperationState.CONTINUE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState SaveAs() 
        {
            action = new BusyAction(false)
            {
                DoWork = () =>
                {
                    try
                    {
                        String message = "Saving File ";
                        action.ReportProgress(0, message);
                        string filePath = "";
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            filePath = openFileDialogForFolders("Save as", null);
                        }
                        ));
                        
                        if (filePath == null || string.IsNullOrWhiteSpace(filePath)) return OperationState.STOP;

                        string fileDir = System.IO.Path.GetDirectoryName(filePath);
                        string fileName = System.IO.Path.GetFileName(filePath);
                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);

                        message = "Saving File as "+fileName;
                        action.ReportProgress(10, message);

                        Misp.Kernel.Domain.File file = this.GetFileInfoService().SaveAs(fileDir, fileName);
                        if (file == null) return OperationState.STOP;
                        this.ApplicationManager.File = file;
                        this.ApplicationManager.AllocationCount = this.GetFileInfoService().GetAllocationCount();
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            ApplicationManager.MainWindow.Title = fileNameWithoutExtension + " - B-Cephal";
                        }
                        ));
                        
                        message = "File saved as " + fileNameWithoutExtension;
                        action.ReportProgress(99, message);
                        UserPreferencesUtil.AddRecentFile(filePath);
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            ApplicationManager.MainWindow.MenuBar.GetFileMenu().BuildRecentOpenedFiles();
                            RefreshDashboard();
                        }
                       ));
                        
                        message = "File " + fileNameWithoutExtension + " opened";
                        action.ReportProgress(100, message); 
                    }
                    catch(BcephalException e)
                    {
                        MessageDisplayer.DisplayError("Saving file as ", e.Message);
                        action = null;
                        return OperationState.STOP;
                    }
                    return OperationState.CONTINUE;
                },

                EndWork = () =>
                {
                    return OperationState.CONTINUE;
                },
            };
            action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.MainWindow.OnBusyPropertyChanged);
            action.Run();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Rename() 
        {
            try
            {
                string filePath = openFileDialogForFolders("Rename", null);
                if (filePath == null || string.IsNullOrWhiteSpace(filePath)) return OperationState.STOP;

                string fileDir = System.IO.Path.GetDirectoryName(filePath);
                string fileName = System.IO.Path.GetFileName(filePath);
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);

                Misp.Kernel.Domain.File file = this.GetFileInfoService().Rename(fileDir, fileName);
                if (file == null) return OperationState.STOP;
                ApplicationManager.MainWindow.Title = fileNameWithoutExtension + " - B-Cephal";
                UserPreferencesUtil.RenameLastOpened(filePath);
                
                ApplicationManager.MainWindow.MenuBar.GetFileMenu().BuildRecentOpenedFiles();
                return OperationState.CONTINUE;
            }
            catch (BcephalException e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return OperationState.STOP;
            }
        }


        public override OperationState RenameItem(string newName)
        {
            return OperationState.CONTINUE;
        }
        public override OperationState Edit(object oid) { return Open(oid); }

        public override OperationState Save()
        { 
        
            return OperationState.CONTINUE;
        }

        public override OperationState SaveAll() { return OperationState.CONTINUE; }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        // <summary>
        /// Cette methode est lorsque la fermeture du controller a réussit. 
        /// Cette methode va déactiver tous les menus sauf le menu file.
        /// </summary>
        /// <returns></returns>
        protected override void AfterClose()
        {
            ApplicationManager.MainWindow.MenuBar.customizeForFileClosed();
            this.ApplicationManager.File = null;
            this.ApplicationManager.AllocationCount = 0;
            this.ApplicationManager.MainWindow.MenuBar.GetFileMenu().BuildRecentOpenedFiles();
            //this.ApplicationManager.MainWindow.FileOpenedView.Visibility = Visibility.Collapsed;
            this.ApplicationManager.MainWindow.DashboardView.Visibility = Visibility.Collapsed;
            this.ApplicationManager.MainWindow.FileClosedView.Visibility = Visibility.Visible;

            this.ApplicationManager.MainWindow.DashboardView.Reset();
        }

        public override OperationState TryToSaveBeforeClose() 
        {
            try
            {
                bool ok = this.GetFileInfoService().CloseFile();
                if (!ok) return OperationState.STOP;
                ApplicationManager.MainWindow.Title = "B-Cephal";

                return OperationState.CONTINUE;
            }
            catch (BcephalException e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return OperationState.STOP;
            }
        }

        protected override IView getNewView() { return new DashBoard(); }

        /// <summary>
        /// The tool bar used to manage file.
        /// </summary>
        /// <returns>New instance of FileToolBar</returns>
        protected override ToolBar getNewToolBar() { return new FileToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new FileToolBarHandlerBuilder(this);
        }

        protected override SideBar getNewSideBar() { return null; }

        protected override PropertyBar getNewPropertyBar() { return null; }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        protected override void initializeViewData(){}        

        protected override void initializeSideBarData(){}

        protected override void initializeViewHandlers() {  }


        protected override void initializeSideBarHandlers() {  }
        
        /// <summary>
        /// Display file dialog to select a file.
        /// </summary>
        /// <param name="title">Title of dialog</param>
        /// <param name="initialDirectory">Initial directory that is displayed by file dialog</param>
        /// <returns>The selected file name</returns>
        private string openFileDialogForMispFiles(string title, string initialDirectory)
        {
            if (ApplicationManager.useZip())
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                fileDialog.Title = title;
                if (!string.IsNullOrWhiteSpace(initialDirectory)) fileDialog.InitialDirectory = initialDirectory;
                fileDialog.DefaultExt = FILE_EXTENSION;
                fileDialog.Filter = "B-Cephal files (*" + FILE_EXTENSION + ")|*" + FILE_EXTENSION;
                Nullable<bool> result = fileDialog.ShowDialog();
                var fileName = fileDialog.SafeFileName;
                var filePath = fileDialog.FileName;
                string[] strings = { fileName, filePath };
                return result == true ? filePath : null;
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
                if (!string.IsNullOrWhiteSpace(initialDirectory)) folderDialog.SelectedPath = initialDirectory;
                System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
                var filePath = folderDialog.SelectedPath;
                if (System.Windows.Forms.DialogResult.OK == result) return filePath;
                return null;

                //Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                //fileDialog.Title = title;

                //fileDialog.ValidateNames = false;
                //fileDialog.CheckFileExists = false;
                //fileDialog.CheckPathExists = true;
                //fileDialog.FileName = "Select Folder";

                //if (!string.IsNullOrWhiteSpace(initialDirectory)) fileDialog.InitialDirectory = initialDirectory;
                //Nullable<bool> result = fileDialog.ShowDialog();
                //if (result == false) return null;
                //string filePath = fileDialog.FileName;
                //filePath = System.IO.Directory.GetParent(filePath).FullName;                
                //return filePath;

                
            }
        }

        private Util.Dialog dialog { get; set; }
        /// <summary>
        /// Display file dialog to select a file.
        /// </summary>
        /// <param name="title">Title of dialog</param>
        /// <param name="initialDirectory">Initial directory that is displayed by file dialog</param>
        /// <returns>The selected file name</returns>
        private string openFileDialogForFolders(string title, string initialDirectory)
        {
            if (ApplicationManager.ApplcationConfiguration.IsMultiuser())
            {
                String name = GetFileInfoService().getDefaultNewProjectName();
                FileNameDialog dialog = new FileNameDialog();
                dialog.Title =title;
                dialog.NameTextBox.Text = name;
                dialog.NameTextBox.SelectAll();
                dialog.NameTextBox.Focus();
                dialog.ValidateNameAction += OnValidateProjectName;
                dialog.ShowCenteredToMouse();
                dialog.ValidateNameAction -= OnValidateProjectName;
                return dialog.Name;                
            }
            else if (ApplicationManager.useZip())
            {
                Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
                fileDialog.Title = title;
                if (!string.IsNullOrWhiteSpace(initialDirectory)) fileDialog.InitialDirectory = initialDirectory;
                fileDialog.DefaultExt = FILE_EXTENSION;
                fileDialog.Filter = "B-Cephal files (*" + FILE_EXTENSION + ")|*" + FILE_EXTENSION;

                Nullable<bool> result = fileDialog.ShowDialog();
                var fileName = fileDialog.SafeFileName;
                var filePath = fileDialog.FileName;
                string[] strings = { fileName, filePath };
                return result == true ? filePath : null;
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderDialog.ShowNewFolderButton = true;
                if (!string.IsNullOrWhiteSpace(initialDirectory)) folderDialog.SelectedPath = initialDirectory;
                System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
                var filePath = folderDialog.SelectedPath;
                if (System.Windows.Forms.DialogResult.OK == result) return filePath;
                return null;
            }
        }

        private bool OnValidateProjectName(String name)
        {
            if (name == null || string.IsNullOrEmpty(name.Trim()))
            {
                Util.MessageDisplayer.DisplayWarning("Empty project name", "The name can't be empty!");
                return false;
            }
            else if (GetFileInfoService().isProjectExist(name.Trim()))
            {
                Util.MessageDisplayer.DisplayWarning("Duplicate project name", "There is another project named: " + name);
                return false;
            }
            return true;
        }


        public override OperationState Search(object oid)
        {
            return OperationState.CONTINUE;
        }
    }
}
