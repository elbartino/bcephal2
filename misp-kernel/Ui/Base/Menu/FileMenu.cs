using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;
using System.Collections.Specialized;

namespace Misp.Kernel.Ui.Base.Menu
{
    public class FileMenu : ApplicationMenu
    {

        private ApplicationMenu newFile;
        private ApplicationMenu openFile;
        private ApplicationMenu saveFile;
        private ApplicationMenu saveAsFile;
        private ApplicationMenu recentFiles;
        private ApplicationMenu backupMenu;
        private ApplicationMenu backupSimpleMenu;
        private ApplicationMenu backupAutomaticMenu;
        private ApplicationMenu quitApplication;

        public ApplicationMenu NewFile { get { return newFile; } }
        public ApplicationMenu OpenFile { get { return openFile; } }
        public ApplicationMenu SaveAsFile { get { return saveAsFile; } }
        public ApplicationMenu SaveFile { get { return saveFile; } }
        public ApplicationMenu RecentFiles { get { return recentFiles; } }
        public ApplicationMenu ArchiveMenu { get { return backupMenu; } }

        public ApplicationMenu BackupSimpleMenu { get { return backupSimpleMenu; } }
        public ApplicationMenu BackupAutomaticMenu { get { return backupAutomaticMenu; } }
        public ApplicationMenu QuitApplication { get { return quitApplication; } }

        public virtual bool isDefaultMenu() { return true; }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(NewFile);
            menus.Add(OpenFile);
            menus.Add(RecentFiles);
            menus.Add(SaveAsFile);
            menus.Add(SaveFile);
            ArchiveMenu.Items.Add(backupSimpleMenu);
            ArchiveMenu.Items.Add(backupAutomaticMenu);
            menus.Add(ArchiveMenu);
            menus.Add(QuitApplication);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.FILE_MENU_CODE;
            this.Header = "File";
            newFile = BuildMenu(FunctionalitiesCode.PROJECT, "New Project", NavigationToken.GetCreateViewToken(FunctionalitiesCode.PROJECT_EDIT));
            openFile = BuildMenu(FunctionalitiesCode.PROJECT, "Open Project", NavigationToken.GetSearchViewToken(FunctionalitiesCode.PROJECT_OPEN));
            recentFiles = BuildMenu(FunctionalitiesCode.PROJECT, "Open Recent Project", FunctionalitiesCode.PROJECT_RECENTS);
            BuildRecentOpenedFiles();
            BuildSaveAsMenu();
            NavigationToken token =  NavigationToken.GetCreateViewToken(FunctionalitiesCode.FILE_SAVE);
            token.FunctionalityType = FunctionalityType.SUB_FONCTIONALITY;
            saveFile = BuildMenu(FunctionalitiesCode.PROJECT, "Save Project", token);
            saveFile.IsEnabled = false;
            saveAsFile.IsEnabled = false;

            backupMenu = BuildMenu(FunctionalitiesCode.PROJECT, FunctionalitiesLabel.BACKUP_LABEL, FunctionalitiesCode.BACKUP_FUNCTIONALITY);
            //backupMenu.IsEnabled = false;
            backupSimpleMenu = BuildMenu(FunctionalitiesCode.PROJECT, FunctionalitiesLabel.BACKUP_SIMPLE_LABEL, NavigationToken.GetCreateViewToken(FunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY));
            backupAutomaticMenu = BuildMenu(FunctionalitiesCode.PROJECT, FunctionalitiesLabel.BACKUP_AUTOMATIC_LABEL, NavigationToken.GetCreateViewToken(FunctionalitiesCode.BACKUP_AUTOMATIC_FUNCTIONALITY));


            quitApplication = BuildMenu(FunctionalitiesCode.PROJECT, "Close B-Cephal", new NavigationToken(FunctionalitiesCode.FILE_QUIT, ViewType.LOGOUT));
        
        }
        public string lastFilePath { get; set; }
        public void  BuildSaveAsMenu()
        {
            NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.FILE_SAVE_AS, lastFilePath);
            this.saveAsFile = BuildMenu(FunctionalitiesCode.PROJECT, "Save As", token);
        }

        /// <summary>
        /// Build Recent Opened Files menu
        /// </summary>
        public void BuildRecentOpenedFiles()
        {
            this.RecentFiles.Items.Clear();
            bool isMonouser = ApplicationManager.Instance.ApplcationConfiguration.IsMonouser();
            if (isMonouser)
            {
                StringCollection files = Util.UserPreferencesUtil.GetRecentFiles();
                foreach (string filePath in files)
                {
                    this.RecentFiles.Items.Add(BuildRecentFileMenuItem(filePath));
                }
                Application.ApplicationManager.Instance.MainWindow.FileClosedView.BuildRecentOpenedFiles(files);
            }
            else
            {
                List<String> projects = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetFileService().getRecentOpenedProjects();
                foreach (string project in projects)
                {
                    this.RecentFiles.Items.Add(BuildRecentFileMenuItem(project));
                }
                Application.ApplicationManager.Instance.MainWindow.FileClosedView.BuildRecentOpenedFiles(projects);
            }
        }

        public void BuildRecentOpenedFilesForSaveAs()
        {
            this.RecentFiles.Items.Clear();
            StringCollection files = Util.UserPreferencesUtil.GetRecentFiles();
            foreach (string filePath in files)
            {
                this.RecentFiles.Items.Add(BuildRecentFileMenuItem(filePath));
            }
           // Application.ApplicationManager.Instance.MainWindow.FileClosedView.BuildRecentOpenedFiles(files);
        }

        public void EnableSaveAsMenu(bool enable)
        {
            this.SaveAsFile.IsEnabled = enable;
        }

        public void EnableSaveMenu(bool enable)
        {
            this.SaveFile.IsEnabled = enable;
            this.SaveAsFile.IsEnabled = enable;
        }

        /// <summary>
        /// Construit un element de menu
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected ApplicationMenu BuildRecentFileMenuItem(string filePath)
        {
            NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.PROJECT, filePath);
            int n = filePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            string header = filePath.Substring(n + 1);
            ApplicationMenu menu = BuildMenu(this.RecentFiles.Code, header, token);
            menu.ToolTip = filePath;
            return menu;
        }


        public void RemoveAllRecentFiles()
        {
            Util.UserPreferencesUtil.RemoveAllRecentFiles();
            BuildRecentOpenedFiles();
        }
        
    }
}
