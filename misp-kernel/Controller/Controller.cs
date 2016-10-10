using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;
using Misp.Kernel.Application;
using Misp.Kernel.Util;
using log4net;
using Misp.Kernel.Domain;

namespace Misp.Kernel.Controller
{
    /// <summary>
    /// Un controller est une classe qui gère une fonctionnalité.
    /// Le controller possède une vue et gère toutes les actions et oppérations associés à cette vue.
    /// 
    /// Il existe deux types de controller:
    ///     1. EditorController : gère l'édition d'un objet
    ///     2. BrowserController : gère l'écran de recherche
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par ce controller</typeparam>
    public abstract class Controller<T, B> : Controllable where T : Domain.Persistent where B : Domain.Browser.BrowserData
    {

        #region Commands

        public static RoutedCommand NewCommand = new RoutedCommand();
        public static RoutedCommand RenameCommand = new RoutedCommand();
        public static RoutedCommand SaveCommand = new RoutedCommand();
        public static RoutedCommand SaveAsCommand = new RoutedCommand();
        public static RoutedCommand DeleteCommand = new RoutedCommand();
        public static RoutedCommand RefreshCommand = new RoutedCommand();

        public static MenuItem NewMenuItem = BuildContextMenuItem("New", NewCommand);
        public static MenuItem RenameMenuItem = BuildContextMenuItem("Rename", RenameCommand);
        public static MenuItem SaveMenuItem = BuildContextMenuItem("Save", SaveCommand);
        public static MenuItem SaveAsMenuItem = BuildContextMenuItem("Save As", SaveAsCommand);
        public static MenuItem DeleteMenuItem = BuildContextMenuItem("Delete", DeleteCommand);
        public static MenuItem RefreshMenuItem = BuildContextMenuItem("Refresh", RefreshCommand);

        public CommandBinding RefreshCommandBinding { get; set; }
        public CommandBinding NewCommandBinding { get; set; }
        public CommandBinding RenameCommandBinding { get; set; }
        public CommandBinding SaveCommandBinding { get; set; }
        public CommandBinding SaveAsCommandBinding { get; set; }
        public CommandBinding DeleteCommandBinding { get; set; }

        private void NewCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onNewButtonClic(sender, e); }

        private void RenameCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void RenameCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onRenameButtonClic(sender, e); }

        private void SaveCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = IsModify;
        if (ApplicationManager.Instance.MainWindow.MenuBar != null) ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(IsModify);
        }
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onSaveButtonClic(sender, e); }

        private void SaveAsCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void SaveAsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
             if(this.SaveAs() == OperationState.STOP) return;
             this.IsModify = false;
        }

        public virtual void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onDeleteButtonClic(sender, e); }

        private void RefreshCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void RefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.Search(); }

        
        #endregion



        #region Attributes

        protected readonly ILog logger;

        /// <summary>
        /// La vue contrôlée
        /// </summary>
        protected IView view;

        /// <summary>
        /// La barre d'outils liée à la fonctionnalité contrôlée.
        /// </summary>
        protected Misp.Kernel.Ui.Base.ToolBar toolBar;

        /// <summary>
        /// Le constructeur de Handles pour la barre d'outil
        /// </summary>
        protected ToolBarHandlerBuilder toolBarHandlerBuilder;

        /// <summary>
        /// La barre de gauche liée à la fonctionnalité contrôlée.
        /// </summary>
        protected SideBar sideBar;

        /// <summary>
        /// La barre de droite liée à la fonctionnalité contrôlée.
        /// </summary>
        protected PropertyBar propertyBar;

        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de 
        /// </summary>
        public Controller() {
            logger = LogManager.GetLogger(this.GetType());
        }

        #endregion


        #region Properties

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
        public Misp.Kernel.Ui.Base.ToolBar ToolBar { get { return toolBar; } }

        /// <summary>
        /// Retourne la barre de gauche liée à la fonctionnalité contrôlée.  
        /// </summary>
        public SideBar SideBar { get { return sideBar; } }

        /// <summary>
        /// Retourne la barre de droite liée à la fonctionnalité contrôlée. 
        /// </summary>
        public PropertyBar PropertyBar { get { return propertyBar; } }

        /// <summary>
        /// Retourne la vue (ou écran) liée à la fonctionnalité contrôlée. 
        /// Une vue est un Browser ou un Editor.
        /// </summary>
        public IView View { get { return view; } }

        /// <summary>
        /// Assigne ou retourne le service associé au controller.
        /// Ce service est utilisé pour la communication avec le serveur.
        /// </summary>
        public Service<T, B> Service { get; set; }

        /// <summary>
        /// Assigne ou retourne le controller à partir duquel on a activé ce controller.
        /// </summary>
        public Controllable ParentController { get; set; }

        /// <summary>
        /// Assigne ou retourne l'ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }
        
        /// <summary>
        /// Assigne ou retourne la valeur indiquant
        /// qu'une modification est survenue dans la vue liée à la fonctionnalité contrôlée.
        /// </summary>
        public virtual bool IsModify { get; set; }

        /// <summary>
        /// Assigne ou retourne le NavigationToken lié à la fonctionnalité contrôlée.
        /// </summary>
        public NavigationToken NavigationToken { get; set; }

        /// <summary>
        /// Assigne ou retourne le ChangeEventHandler qui spécifie 
        /// la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public ChangeEventHandler ChangeEventListener { get; set; }

        #endregion


        #region Operations

        /// <summary>
        /// Sauve les modification survenues dans la page active de la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Save();

        /// <summary>
        /// Sauve les modification survenues dans toutes les pages de la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState SaveAll();


        private Util.Dialog dialog { get; set; }

        /// <summary>
        /// Sauve l'object courrant sous un autre nom.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState SaveAs() //{ return OperationState.CONTINUE; }
        {
            NamePanel namePanel = new NamePanel();
            namePanel.OnValidate +=namePanel_OnValidate;
            dialog = new Util.Dialog("Save as", namePanel);
            dialog.Height = 110;
            dialog.Width = 300;
            namePanel.NameTextBox.SelectAll();
            namePanel.NameTextBox.Focus();
            if (dialog.ShowCenteredToMouse().Value)
            {
                string name = namePanel.EditedName;
                if(name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Util.MessageDisplayer.DisplayError("Save as", "The name can't be empty!");
                    return OperationState.STOP;
                }
                return SaveAs(name);
            }
            return OperationState.CONTINUE;
        }

        private void namePanel_OnValidate(bool isOk,String editedName)
        {
            if (isOk) {
                if (editedName == null || string.IsNullOrEmpty(editedName) || string.IsNullOrWhiteSpace(editedName))
                {
                    Util.MessageDisplayer.DisplayError("Save as", "The name can't be empty!");
                    return;
                }
                this.SaveAs(editedName);
            }
            this.dialog.Close();
        }

        public virtual OperationState SaveAs(string name)
        {
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Renomme l'objet courant
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Rename();


        /// <summary>
        /// Renomme l'objet courant
        /// </summary>
        /// <param name="newName"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState RenameItem(string newName);

        /// <summary>
        /// Supprime l'objet (ou les objects) courrant.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Delete();

        /// <summary>
        /// permet d'obtenir le module en courrant.
        /// </summary>
        /// <returns>
        /// Domain.SubjectType courrant
        /// </returns>
        public abstract Domain.SubjectType SubjectTypeFound();

     

        /// <summary>
        /// Cette méthode est appelée avant de fermer la vue contôlée.
        /// Elle demande à l'utilisateur s'il veut sauver les éventuels 
        /// modifications avant la fermeture de la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState TryToSaveBeforeClose();

        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState OnChange()
        {
            this.IsModify = true;
            ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(this.IsModify);
            if (this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = true;
            if(ChangeEventListener != null) ChangeEventListener();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Methode executée quand la sauvegare est effectuée avec succes
        /// </summary>
        public virtual void AfterSave()
        {
            this.IsModify = false;
            ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(false);
            if (this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = false;
        }

        /// <summary>
        /// Methode executée quand le run est effectuée avec succes
        /// </summary>
        public virtual void AfterRun()
        {
            this.IsModify = false;
            ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(false);
            if (this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = false;
        }

        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Create();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Open();

        /// <summary>
        /// Affiche un objet dans la vue d'édition
        /// </summary>
        /// <param name="oid">L'identifiant de l'object</param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Open(object oid);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Edit(object oid) { return Open(oid); }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Search();




        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <param name="oid"></param>
        /// /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public abstract OperationState Search(object oid);


        /// <summary>
        /// Cette methode est appelée quand on est en train de fermer le controller.
        /// 
        /// 1. On appelle TryToSaveBeforeClose pour demander à l'utilisateur
        ///    s'il veut sauver ou non les modifications.
        ///    
        /// 2. Si l'éventuel sauvegarde s'est bien passée, on appelle AfterClose.
        /// 
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Close()
        {
            OperationState state = TryToSaveBeforeClose();
            if (state == OperationState.STOP) return state;
            this.IsModify = false;
            if (ApplicationManager.Instance.MainWindow.MenuBar != null)
            {
                ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().EnableSaveMenu(this.IsModify);
            }
            AfterClose();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Cette methode est lorsque la fermeture de ce controller a réussit.
        /// 
        /// On peut redéfinir cette méthode dans une sous classe pour par exemple déactiver certain menus.
        /// Exemple: 
        /// Quand on ferme FileController, il faut déactiver tous les menus sauf le menu file.
        /// </summary>
        protected virtual void AfterClose() 
        {
            RemoveCommands();
        }
        
        /// <summary>
        /// Affiche un message d'erreur dans un dialogue
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        protected virtual void DisplayError(string title, string message)
        {
            Util.MessageDisplayer.DisplayError(title, message);
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Effectue l'initialisation du controller.
        /// On initialize la vue et on met this.IsModify = false
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Initialize()
        {
            initializeView();
            this.IsModify = false;
            return OperationState.CONTINUE;
        }
        
        /// <summary>
        /// Effectue les opérations necessaires pour l'initialisation de la vue.
        /// </summary>
        protected void initializeView()
        {
            initializeChangeEventHandler();
            view = getNewView();
            if(View != null) View.SetChangeEventHandler(this.ChangeEventHandler);
            initializeViewData();
            initializeViewHandlers();
            initializeCommands();

            initializeToolBar();
            initializeSideBar();
            initializePropertyBar();
        }

        protected virtual void initializeCommands()
        {
            this.NewCommandBinding = new CommandBinding(NewCommand, NewCommandExecuted, NewCommandEnabled);        
            this.RenameCommandBinding = new CommandBinding(RenameCommand, RenameCommandExecuted, RenameCommandEnabled);
            this.SaveCommandBinding = new CommandBinding(SaveCommand, SaveCommandExecuted, SaveCommandEnabled);
            this.SaveAsCommandBinding = new CommandBinding(SaveAsCommand, SaveAsCommandExecuted, SaveAsCommandEnabled);
            this.DeleteCommandBinding = new CommandBinding(DeleteCommand, DeleteCommandExecuted, DeleteCommandEnabled);
            this.RefreshCommandBinding = new CommandBinding(RefreshCommand, RefreshCommandExecuted, RefreshCommandEnabled);
           
            RemoveCommands();
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected virtual void RemoveCommands()
        {
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RefreshMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(DeleteMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveAsMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RenameMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(NewMenuItem);

                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(NewCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RenameCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveAsCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(DeleteCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RefreshCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Clear();
            }
        }

        protected static System.Windows.Controls.MenuItem BuildContextMenuItem(string header, System.Windows.Input.RoutedCommand routedCommand)
        {
            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
            menuItem.Header = header;
            menuItem.Command = routedCommand;
            return menuItem;
        }
        

        /// <summary>
        /// Initialisation des Handlers.
        /// </summary>
        protected virtual void initializeChangeEventHandler()
        {
            if (this.ChangeEventHandler == null)
            {
                this.ChangeEventHandler = new ChangeEventHandlerBuilder(this);
                this.ChangeEventHandler.BuildHandlers();
            }            
        }

        /// <summary>
        /// Effectue les opérations necessaires pour l'initialisation de la ToolBar.
        /// </summary>
        protected void initializeToolBar()
        {
            toolBar = getNewToolBar();
            initializeToolBarHandlers();
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected virtual void initializeToolBarHandlers()
        {
            toolBarHandlerBuilder = getNewToolBarHandlerBuilder();
            if (toolBarHandlerBuilder == null) return;
            toolBarHandlerBuilder.buildHandlers();
        }

        /// <summary>
        /// Effectue les opérations necessaires pour l'initialisation de la SideBar.
        /// </summary>
        protected void initializeSideBar()
        {
            sideBar = getNewSideBar();
            if (sideBar != null) sideBar.SelectStatus(ModuleName);
            initializeSideBarData();
            initializeSideBarHandlers();
        }

        /// <summary>
        /// Effectue les opérations necessaires pour l'initialisation de la property bar.
        /// </summary>
        protected void initializePropertyBar()
        {
            propertyBar = getNewPropertyBar();
            initializePropertyBarData();
            initializePropertyBarHandlers();
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected abstract IView getNewView();

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected abstract Misp.Kernel.Ui.Base.ToolBar getNewToolBar();

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected abstract ToolBarHandlerBuilder getNewToolBarHandlerBuilder();

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected abstract SideBar getNewSideBar();

        /// <summary>
        /// Crée et retourne une nouvelle instance de la PropertyBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la PropertyBar</returns>
        protected abstract PropertyBar getNewPropertyBar();

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected abstract void initializeViewData();

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected abstract void initializeSideBarData();

        /// <summary>
        /// Initialisation des donnée sur la PropertyBar.
        /// </summary>
        protected abstract void initializePropertyBarData();

        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected abstract void initializeViewHandlers();

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected abstract void initializeSideBarHandlers();

        /// <summary>
        /// Initialisation des Handlers sur la PropertyBar.
        /// </summary>
        protected abstract void initializePropertyBarHandlers();

        #endregion



        
       
    }
}
