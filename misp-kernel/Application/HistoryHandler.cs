using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Util;
using Misp.Kernel.Ui.General;
using System.Windows.Forms;
using Misp.Kernel.Task;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using System.Windows.Input;

namespace Misp.Kernel.Application
{
    /// <summary>
    /// History Navigation handler
    /// </summary>
    public class HistoryHandler
    {

        /// <summary>
        /// Build a new instance of HistoryHandler.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public HistoryHandler(ApplicationManager applicationManager)
        {
            Instance = this;
            this.ApplicationManager = applicationManager;
            this.OpenedPages = new List<Controllable>(0);
            ActivePage = null;
        }

        public static string FILE_EXTENSION_EXCEL = ".xlsx";
        public static string[] TAB_FILE_EXTENSION_EXCEL ={".xls",".xlsx"};
        public static string FILE_EXTENSION_POWERPOINT = ".pptx";
        public static string[] TAB_FILE_EXTENSION_POWERPOINT = { ".ppt", ".pptx" };
        public static string FILE_EXTENSION_CSV = ".csv";
        public string[] fileAttribute;
        /// <summary>
        /// Gets or sets the PageManager. 
        /// </summary>
        public static HistoryHandler Instance { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Gets or sets opened pages
        /// </summary>
        public List<Controllable> OpenedPages { get; set; }

        /// <summary>
        /// Gets or sets ActivePage
        /// </summary>
        public Controllable ActivePage { get; set; }

        public String InternalErrorMessage { get; set; }

        /// <summary>
        /// Procède à la fermeture de la page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public OperationState closePage(Controllable page)
        {
            OperationState state = page.Close();
            if (state == OperationState.STOP) return OperationState.STOP;

            //Worker worker = new Worker("Closing...");
            //worker.OnWorkWithParameter += OnClosePage;
            //worker.StartWork(page);
            OnClosePage(page);
            return OperationState.CONTINUE;
        }
                
        /// <summary>
        /// Procède à la fermeture de la page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public void OnClosePage(object param)
        {
            Controllable page = (Controllable)param;
            if (page == null) return;
            try
            {
                //OperationState state = page.Close();
                //if (state == OperationState.STOP) return;
                if (OpenedPages.Contains(page)) OpenedPages.Remove(page);
                if (ActivePage != null && ActivePage.Equals(page))
                {
                    ActivePage = null;
                    FunctionalityType functionalityType = page.NavigationToken != null ? page.NavigationToken.FunctionalityType : FunctionalityType.MAIN_FONCTIONALITY;
                    bool isSubFonctionality = functionalityType == FunctionalityType.SUB_FONCTIONALITY;
                    if (isSubFonctionality && page.ParentController != null)
                    {
                        openPage(page.ParentController);
                    }
                    else if (OpenedPages.Count > 0)
                    {
                        /*Controllable pageOpened = OpenedPages[OpenedPages.Count - 1];
                        if (pageOpened.Functionality.Contains("LIST"))
                            openSearchPage(pageOpened.NavigationToken);
                        else 
                         */
                        openPage(OpenedPages[OpenedPages.Count - 1]);
                        return;
                    }
                    else
                    {
                        //ApplicationManager.MainWindow.displayPage(null);
                        openHomePage();
                    }
                }                
            }
            catch (Exception e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return;
            }
            return;
        }


        /// <summary>
        /// Display the page that corresponds to the history token
        /// If there is a valid history token, then navigate to the corresponding page
        /// otherwise open the home page
        /// </summary>
        /// <param name="token"></param>
        public OperationState openPage(NavigationToken token)
        {
            
            if (token == null || token.Functionality.Trim().Length <= 0)
            {
                return openHomePage();
            }

            ViewType viewType = token.ViewType;
            if (viewType == ViewType.LOGOUT)
            {
                return tryToCloseApplication();
            }

            //Worker worker = new Worker("");
            //worker.OnWorkWithParameter += OnOpenPage;
            //worker.StartWork(token);
            OnOpenPage(token);
            return OperationState.CONTINUE ;
        }

        /// <summary>
        /// Display the page that corresponds to the history token
        /// If there is a valid history token, then navigate to the corresponding page
        /// otherwise open the home page
        /// </summary>
        /// <param name="token"></param>
        protected void OnOpenPage(object param)
        {
            NavigationToken token = (NavigationToken)param;
            Controllable page = null;
            try
            {
                string functionality = token.Functionality;
                if (functionality == FunctionalitiesCode.HELP_ABOUT) { OpenAboutDialog(); return; }
                if (functionality == FunctionalitiesCode.RUN_ALL_ALLOCATION_FUNCTIONALITY) { StartRunAllAllocation(); return; }
                if (functionality == FunctionalitiesCode.ALLOCATION_CLEAR_ALL) { StartClearAllAllocation(); return; }
                if (functionality == FunctionalitiesCode.ALLOCATION_LOG) { StartAllocationLog(); return; }
                if (functionality == FunctionalitiesCode.UPLOAD_MULTIPLE_FILES) { UploadMultipleFiles(); return; }
                if (functionality == FunctionalitiesCode.PROPERTIES_FUNCTIONALITY) { createProperties(); return; }
                if (functionality == FunctionalitiesCode.LOAD_TRANSFORMATION_TREES_FUNCTIONALITY) { LoadTransformationTrees(false); return; }
                if (functionality == FunctionalitiesCode.CLEAR_TRANSFORMATION_TREES_FUNCTIONALITY) { LoadTransformationTrees(true); return; }
                if (functionality == FunctionalitiesCode.FILE_SAVE) token.currentActiveFunctionality = ActivePage.FunctionalityCode;
                if (functionality == FunctionalitiesCode.FILE_SAVE_AS) { SaveFileAs(token); return; }
                if (functionality == FunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY || functionality == FunctionalitiesCode.BACKUP_AUTOMATIC_FUNCTIONALITY) 
                {
                    bool isSimpleBackup = functionality == FunctionalitiesCode.BACKUP_SIMPLE_FUNCTIONALITY;
                    loadBackup(functionality,isSimpleBackup); return; 
                }
                
                string tag = token.GetTag();
                FunctionalityType functionalityType = token.FunctionalityType;
                ViewType viewType = token.ViewType;

                bool isMainFonctionality = functionalityType == FunctionalityType.MAIN_FONCTIONALITY;
                
                OperationState state = OperationState.CONTINUE;
                if (isMainFonctionality && !(ActivePage != null && ActivePage is FileController))
                {
                    string activefunctionality = ActivePage != null ? ActivePage.FunctionalityCode : null;
                    page = searchInOpenedPages(activefunctionality);
                    bool tryToSaveActivePage = page != null && activefunctionality != null && !activefunctionality.Equals(functionality);
                    state = tryToSaveActivePage ? page.Close() : OperationState.CONTINUE;

                    if (page != null && state == OperationState.CONTINUE) { OpenedPages.Remove(page); }
                }
                else { }

                if (state == OperationState.CONTINUE)
                {
                    ApplicationManager.MainWindow.SetPogressBar1Visible(false);
                    ApplicationManager.MainWindow.SetPogressBar2Visible(false);
                    if (viewType == ViewType.LOGOUT)
                    {
                        tryToCloseApplication(); return;
                    }
                    else if (viewType == ViewType.EDITION)
                    {
                        openEditionPage(token);
                        if (!String.IsNullOrEmpty(InternalErrorMessage))
                        {
                            MessageDisplayer.DisplayError("Error", InternalErrorMessage);
                            InternalErrorMessage = null;
                        }
                        return;
                    }
                    else
                    {
                        openSearchPage(token); return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageDisplayer.DisplayError("Error", e.Message);
                return;
            }
            return;
        }

        public OperationState createProperties()
        {
            try
            {
                string functionality = FunctionalitiesCode.PROPERTIES_FUNCTIONALITY;
                PropertiesDialog propertiedDialog = new PropertiesDialog();
                propertiedDialog.display();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }
       

        public OperationState  StartAllocationLog()
        {
            try
            {
                string functionality = FunctionalitiesCode.ALLOCATION_LOG;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        public OperationState loadBackup(String functionality, bool backUpOption)
        {
            try
            {
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null)
                {
                    return page.Open(backUpOption);
                }
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

/// <summary>
/// 
/// </summary>
/// <returns></returns>
        public OperationState UploadMultipleFiles()
        {
            try
            {
                string functionality = FunctionalitiesCode.UPLOAD_MULTIPLE_FILES;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        public OperationState StartUpload(object oid=null)
        {
            try
            {
                NavigationToken token;
                if (fileAttribute == null || fileAttribute.Count() <= 0)
                {
                    return OperationState.STOP;

                }
                else
                {
                    if (oid is int)
                        token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.NEW_AUTOMATIC_SOURCING_FUNCTIONALITY, oid);
                    else
                        token = NavigationToken.GetCreateViewToken(FunctionalitiesCode.NEW_AUTOMATIC_SOURCING_FUNCTIONALITY);
                    return openEditionPage(token);
                }
                
             //   return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }
        
        public OperationState OpenAboutDialog() 
        {
            try
            {
                AboutDialog aboutDialog = new AboutDialog();
                aboutDialog.ShowDialog();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        public OperationState StartRunAllAllocation()
        {
            try
            {
                string functionality = FunctionalitiesCode.RUN_ALL_ALLOCATION_FUNCTIONALITY;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }
        public OperationState StartRunAllTransformationTrees()
        {
            try
            {
                string functionality = FunctionalitiesCode.LOAD_TRANSFORMATION_TREES_FUNCTIONALITY;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        public OperationState SaveFileAs(NavigationToken token)
        {
            Controllable page;
            for (int i = OpenedPages.Count - 1; i >= 0; i--)
            {
                page = OpenedPages[i];
                if(page.FunctionalityCode != FunctionalitiesCode.FILE)
                closePage(page);
            }
            page = searchInOpenedPages(FunctionalitiesCode.FILE);
            if (page.SaveAs() == OperationState.STOP) return OperationState.STOP;
            return openPage(page);

            
            //Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            //fileDialog.Title = "B-cephal - Save File As";
            //// Set filter for file extension and default file extension 
            //fileDialog.DefaultExt = FileController.FILE_EXTENSION;
            //fileDialog.Filter = "B-cephal files (*" + FileController.FILE_EXTENSION + ")|*" + FileController.FILE_EXTENSION;
            //Nullable<bool> result = fileDialog.ShowDialog();
            //var fileName = fileDialog.SafeFileName;
            //var filePath = fileDialog.FileName;
            //String copiedPath = ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().lastFilePath;
            
            //if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return OperationState.STOP;
            //if(saveFile(token) == OperationState.STOP) return OperationState.STOP;
           
            //for (int i = OpenedPages.Count - 1; i >= 0; i--)
            //{
            //    closePage(OpenedPages[i]);
            //}
            //if (filePath.EndsWith(FileController.FILE_EXTENSION))
            //{
            //    if (filePath.EndsWith(FileController.FILE_EXTENSION))
            //    {
            //        filePath = filePath.Substring(0, filePath.Length - FileController.FILE_EXTENSION.Length);
            //    }
            //}
            //if (FileUtil.copy(copiedPath, filePath))
            //{
            //    NavigationToken token1 = NavigationToken.GetModifyViewToken(FunctionalitiesCode.FILE_FUNCTIONALITY,filePath);
            //    openEditionPage(token1);
            //    return OperationState.CONTINUE;
            //}
            //else MessageDisplayer.DisplayError("B-cephal - Save as", "Error while saving");
            //return OperationState.CONTINUE;    
        }

        /// <summary>
        /// To clear or run multiple transformationTrees
        /// </summary>
        /// <param name="forClear">forClear = false means it's the run mode
        ///  forClear = true means it's the clear mode</param>
        /// <returns></returns>
        public OperationState LoadTransformationTrees(bool forClear = false)
        {
            try
            {
                string functionality = forClear ? FunctionalitiesCode.CLEAR_TRANSFORMATION_TREES_FUNCTIONALITY :
                    FunctionalitiesCode.LOAD_TRANSFORMATION_TREES_FUNCTIONALITY;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        public OperationState StartClearAllAllocation()
        {
            try
            {
                string functionality = FunctionalitiesCode.ALLOCATION_CLEAR_ALL;
                Controllable page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page != null) return page.Create();
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }

        protected OperationState saveFile(NavigationToken token) 
        {
            Controllable page = searchInOpenedPages(token.currentActiveFunctionality);
            if (page != null)
            {
                if (page.Save() == OperationState.CONTINUE)
                {
                    page.IsModify = false;
                    page.ToolBar.SaveButton.IsEnabled = false;
                }
                return OperationState.CONTINUE;
            }
            return OperationState.CONTINUE;
        }


        /// <summary>
        /// Open a Edition view for a fonctionality
        /// This method shall be called by the PageManager
        /// </summary>
        /// <param name="token">NavigationToken</param>
        protected OperationState openEditionPage(NavigationToken token)
        {
            Controllable page = null;
            try
            {
                String functionality = token.Functionality;

                if (functionality == FunctionalitiesCode.FILE_SAVE)
                    return saveFile(token);

                page = searchInOpenedPages(functionality);

                if (page == null)
                {
                    page = ApplicationManager.ControllerFactory.GetController(functionality);
                    if (page == null)
                    {
                        return openHomePage();
                    }
                    OpenedPages.Add(page);
                    page.NavigationToken = token;
                    page.Initialize();
                }
                openPage(page);
                EditionMode editionMode = token.EditionMode;
                object oid = token.ItemId;
                if (editionMode == EditionMode.CREATE)
                {
                    page.Create();
                }
                else if (editionMode == EditionMode.READ_ONLY)
                {
                    if (token.ItemIds.Count == 0) page.Open(oid);

                    foreach (object id in token.ItemIds)
                    {
                        if (id != null)
                            page.Open(id);
                    }
                }
                else if (editionMode == EditionMode.MODIFY)
                {
                    if (token.ItemIds.Count == 0) page.Edit(oid);
                    foreach (object id in token.ItemIds)
                    {
                        if (id != null)
                            page.Edit(id);
                    }
                }
            }
            catch (Exception ex) 
            {
                InternalErrorMessage = ex.Message;
                OnClosePage(page);
                
            }
            return OperationState.CONTINUE;
        }
                

        /// <summary>
        /// Open the Search view for a functionality in creation mode
        /// </summary>
        /// <param name="token">NavigationToken</param>
        protected OperationState openSearchPage(NavigationToken token)
        {
            String functionality = token.Functionality;
            Controllable page = searchInOpenedPages(functionality);
            if (page == null)
            {
                page = ApplicationManager.ControllerFactory.GetController(functionality);
                if (page == null) { return openHomePage(); }
                OpenedPages.Add(page);
                page.NavigationToken = token;
                page.Initialize();                
            }
            page.Search();

            if (token.ItemId != null)
            {
                    int idmodel = int.Parse(token.ItemId.ToString());
                    page.Search(idmodel);
             }
            

            return openPage(page);
        }
        

        /// <summary>
        /// Open Home page
        /// </summary>
        public OperationState openHomePage()
        {
            Controllable homePage = searchInOpenedPages(FunctionalitiesCode.HOME_PAGE);
            if (homePage == null)
            {
                homePage = ApplicationManager.ControllerFactory.GetController(FunctionalitiesCode.HOME_PAGE);
                homePage.Initialize();
            }
            return openPage(homePage);
        }

        /// <summary>
        /// Display the page and call
        /// </summary>
        /// <param name="page"></param>
        protected OperationState openPage(Controllable page)
        {
            FunctionalityType functionalityType = page.NavigationToken != null ? page.NavigationToken.FunctionalityType : FunctionalityType.MAIN_FONCTIONALITY;
            bool isSubFonctionality = functionalityType == FunctionalityType.SUB_FONCTIONALITY;
            
            if (page == null) return OperationState.CONTINUE;

            if (ActivePage != null && ActivePage is FileController) page.ParentController = ActivePage;
                      
            if (ActivePage == null || (!page.Equals(ActivePage) || !page.FunctionalityCode.Equals(ActivePage.FunctionalityCode)))
            {
                if (isSubFonctionality) { page.ParentController = ActivePage; }
 
                ActivePage = page;                
                ApplicationManager.MainWindow.displayPage(page);
                if (!OpenedPages.Contains(page))
                {
                    OpenedPages.Add(page);
                 
                    
                }
            }
            return OperationState.CONTINUE;
        }

        bool applicationIsClosed = false;
        
        /// <summary>
        /// Try to close
        /// </summary>
        /// <returns></returns>
        public OperationState tryToCloseApplication()
        {
            if (applicationIsClosed) return OperationState.CONTINUE;
            OperationState state = OperationState.CONTINUE;

            if (ActivePage != null && ActivePage.IsModify)
            {
                state = ActivePage.TryToSaveBeforeClose();
                if (state == OperationState.STOP) return OperationState.STOP;
                state = closeApplication();
                applicationIsClosed = state == OperationState.CONTINUE ? true : false;
                return state;
            }

            var response = MessageDisplayer.DisplayYesNoQuestion("Exiting...", "Do you really want to exit B-Cephal ?");
            if (response == MessageBoxResult.No) 
            {
                state = OperationState.STOP; 
            }
            else 
            {
                state = closeApplication();
                applicationIsClosed = state == OperationState.CONTINUE ? true : false;                
            }
            if (ApplicationManager.Instance.MainWindow.MenuBar != null)
            {
                ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(false);
            }
            return state;
        }

        private OperationState closeApplication()
        {
            bool ok = ApplicationManager.StopServer();
            if (ok)
            {
                Application.Current.Shutdown();
            }
            else
            {
                //MessageDisplayer.DisplayError("B-Cephal shutdown", "Unable to shutdown the application.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Try to singout
        /// </summary>
        /// <returns></returns>
        public OperationState tryToSingout()
        {
            OperationState state = OperationState.CONTINUE;
            if (ActivePage != null && ActivePage.IsModify)
            {
                state = ActivePage.TryToSaveBeforeClose();
                if (state == OperationState.STOP) return OperationState.STOP;
                state = closeApplication();
                applicationIsClosed = state == OperationState.CONTINUE ? true : false;
                return state;
            }            
            ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(false);

            ApplicationManager.Instance.User = null;
            ApplicationManager.Instance.MainWindow.ConnectedUserPanel.Visibility = Visibility.Collapsed;
            ApplicationManagerBuilder builder = new ApplicationManagerBuilder();
            builder.loadPlugins();
            tryToLogin();
            ApplicationManager.Instance.MainWindow.displayMenuBar(null);

            return state;
        }

        public void tryToLogin()
        {
            ApplicationManager.Instance.MainWindow.FileClosedView.Visibility = Visibility.Collapsed;
            long userCount = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetSecurityService().getUserCount();
            if (userCount == 0)
            {
                ApplicationManager.Instance.MainWindow.AdministratorPanel.Visibility = Visibility.Visible;
                ApplicationManager.Instance.MainWindow.AdministratorPanel.NameTextBox.Focus();
                ApplicationManager.Instance.MainWindow.AdministratorPanel.SaveButton.Click += onSaveAdminClicked;
            }
            else
            {
                ApplicationManager.Instance.MainWindow.LoginPanel.Visibility = Visibility.Visible;
                ApplicationManager.Instance.MainWindow.LoginPanel.reset();
                ApplicationManager.Instance.MainWindow.LoginPanel.loginTextBox.Focus();
                ApplicationManager.Instance.MainWindow.LoginPanel.LoginButton.Click += onLoginClicked;
                ApplicationManager.Instance.MainWindow.LoginPanel.passwordTextBox.KeyUp += OnValidate;
                ApplicationManager.Instance.MainWindow.LoginPanel.loginTextBox.KeyUp += OnLoginValidate;
            }
        }

        private void OnLoginValidate(object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == Key.Enter) 
            {
                ApplicationManager.Instance.MainWindow.LoginPanel.passwordTextBox.Focus();
            }
        }

        private void OnValidate(object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                connect();   
            }
        }

        private void onSaveAdminClicked(object sender, RoutedEventArgs e)
        {
            if (ApplicationManager.Instance.MainWindow.AdministratorPanel.ValidateEdition())
            {
                SecurityService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetSecurityService();
                User user = service.saveAdministrator(ApplicationManager.Instance.MainWindow.AdministratorPanel.Fill());
                if (user != null)
                {
                    setUser(user);
                    ApplicationManager.Instance.MainWindow.AdministratorPanel.Visibility = Visibility.Collapsed;
                    ApplicationManager.Instance.MainWindow.AdministratorPanel.SaveButton.Click -= onSaveAdminClicked;
                }
                else
                {
                    ApplicationManager.Instance.MainWindow.AdministratorPanel.Console.Text = "Unable to save administrator!";
                    ApplicationManager.Instance.MainWindow.AdministratorPanel.Console.Visibility = Visibility.Visible;
                }
            }
        }

        private void connect() 
        {
            if (ApplicationManager.Instance.MainWindow.LoginPanel.ValidateEdition())
            {
                SecurityService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetSecurityService();
                User user = ApplicationManager.Instance.MainWindow.LoginPanel.Fill();
                user = service.authentificate(user.login, user.password);
                if (user != null)
                {
                    setUser(user);
                    ApplicationManager.Instance.MainWindow.LoginPanel.Visibility = Visibility.Collapsed;
                    ApplicationManager.Instance.MainWindow.LoginPanel.LoginButton.Click -= onLoginClicked;
                    ApplicationManager.Instance.MainWindow.LoginPanel.passwordTextBox.KeyUp -= OnValidate;
                    ApplicationManager.Instance.MainWindow.LoginPanel.loginTextBox.KeyUp -= OnLoginValidate;
                }
                else
                {
                    ApplicationManager.Instance.MainWindow.LoginPanel.Console.Text = "Wrong login or password!";
                    ApplicationManager.Instance.MainWindow.LoginPanel.Console.Visibility = Visibility.Visible;
                    ApplicationManager.Instance.MainWindow.LoginPanel.loginTextBox.Focus();
                    ApplicationManager.Instance.MainWindow.LoginPanel.loginTextBox.SelectAll();
                }
            }
        }

        private void onLoginClicked(object sender, RoutedEventArgs e)
        {
            connect();
        }

        protected void setUser(User user)
        {
            ApplicationManager.Instance.User = user;
            buildUserMenus();
            ApplicationManager.Instance.MainWindow.FileClosedView.ClearTextBlock.Visibility = Visibility.Collapsed;
            ApplicationManager.Instance.MainWindow.FileClosedView.NewFileTextBlock.Visibility = user != null && user.IsAdmin() ? Visibility.Visible : Visibility.Collapsed;
            ApplicationManager.Instance.MainWindow.ConnectedUserPanel.Visibility = user != null ? Visibility.Visible : Visibility.Collapsed;
            if (user != null)
            {
                ApplicationManager.Instance.MainWindow.ConnectedUserPanel.UserService = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetUserService();
                ApplicationManager.Instance.MainWindow.ConnectedUserPanel.UserTextBlock.Text = user.login;
            }
        }

        public void buildUserMenus()
        {
            ApplicationManager.Instance.MainWindow.LoginPanel.Visibility = Visibility.Collapsed;
            ApplicationManager.Instance.MainWindow.FileClosedView.Visibility = Visibility.Visible;
            buildMenus(ApplicationManager.Instance);
            setExcelExtension();
            ApplicationManager.Instance.DefaultPowertPointExtension = PowerPointExtension.PPTX;
            ApplicationManager.Instance.OpenDefaultFile();
        }

        private void setExcelExtension()
        {
            try
            {
                ExcelExtension defaultExtension = ExcelUtil.GetDefaultExcelExtenstion();
                if (defaultExtension == null) MessageDisplayer.DisplayWarning("Bcephal - MS Excel not found", "The MS Excel version of your computer is not supported or there is no MS Excel installed. \n You may not be able to use some functionnalities!");
                else ApplicationManager.Instance.DefaultExcelExtension = defaultExtension;
            }
            catch (Exception)
            {
                //logger.Error("MS Excel checking faild: " + e);
            }
        }

        /// <summary>
        /// Build application menu bar.
        /// </summary>
        /// <param name="manager"></param>
        protected void buildMenus(ApplicationManager manager)
        {
            ApplicationMenusBuilder menuBuilder = new ApplicationMenusBuilder(manager);
            menuBuilder.build();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionality">Functionality short name</param>
        /// <returns></returns>
        protected Controllable searchInOpenedPages(string functionality)
        {
            if (functionality != null && OpenedPages != null && OpenedPages.Count > 0)
            {
                foreach (Controllable page in OpenedPages)
                {
                    if (page.FunctionalityCode == functionality) { return page; }
                }
            }
            return null;
        }
        
    }
}
