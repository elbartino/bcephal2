using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;
using Misp.Kernel.Application;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Misp.Kernel.Task;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using Misp.Kernel.Domain.Browser;


namespace Misp.Kernel.Controller
{

    /// <summary>
    /// Un BrowserController est un controller qui gère la navigation.
    /// L'EditorController possède un navigateur (Browser).
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par ce controller</typeparam>
    public abstract class BrowserController<T, B> : Controller<T, B>
        where T : Domain.Persistent
        where B : Domain.Browser.BrowserData
    {

        #region Constructors
        
        /// <summary>
        /// Construit une nouvelle instance de BrowserController
        /// </summary>
        public BrowserController() : base() { }

        #endregion


        #region Not used methods

        public override OperationState Save() { return OperationState.CONTINUE; }
        public override OperationState SaveAll() { return OperationState.CONTINUE; }
        public override OperationState TryToSaveBeforeClose() 
        {
            RemoveCommands();
            return OperationState.CONTINUE; 
        }

        protected override void initializeCommands()
        {
            base.initializeCommands();
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, RefreshMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(RefreshCommandBinding);                
            }
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RefreshMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RefreshCommandBinding);
            }
        }


        #endregion


        #region Operations

        /// <summary>
        /// Crée un nouvel objet et affiche la view d'édition 
        /// </summary>
        /// <returns></returns>
        public override OperationState Create()
        {
            return HistoryHandler.Instance.openPage(NavigationToken.GetCreateViewToken(GetEditorFuntionality()));
        }

        /// <summary>
        /// Ouvre l'objet sélectionné dans l'éditeur
        /// </summary>
        /// <returns></returns>
        public override OperationState Open()
        {
            List<object> ids = new List<object>(0);
            foreach (Object item in GetBrowser().Grid.SelectedItems)
            {
                ids.Add(((B)item).oid);
               
            }
            return Open(ids);
           
        }

        /// <summary>
        /// Affiche un objet dans la vue d'édition
        /// </summary>
        /// <param name="oid">Identifiant de l'objet à afficher</param>
        /// <returns></returns>
        public override OperationState Open(object oid)
        {
            return HistoryHandler.Instance.openPage(NavigationToken.GetModifyViewToken(GetEditorFuntionality(), oid));
        }

        public OperationState Open(List<object> oids)
        {
            return HistoryHandler.Instance.openPage(NavigationToken.GetModifyViewToken(GetEditorFuntionality(), oids));
        }
        
        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            try
            {
                return Search(0);
                //List<B> items = this.Service.getBrowserDatas();
                //items.BubbleSort();
                //GetBrowser().Grid.ItemsSource = items;
                //return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
               DisplayError("error", e.Message);
            }
            
            return OperationState.STOP;
        }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public override OperationState Search(object item)
        {
            try
            {
                int p = 0;
                BGroup group = null;
                if (item != null && item is int) p = (int)item;
                else if (item != null && item is Kernel.Domain.BGroup) group = (BGroup)item;
                BrowserDataFilter filter = GetBrowser().BuildFilter(p);
                if (group != null && group.oid.HasValue) filter.groupOid = group.oid;
                BrowserDataPage<B> page = this.Service.getBrowserDatas(filter);
                GetBrowser().DisplayPage(page);
                return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
                DisplayError("error", e.Message);
            }

            return OperationState.STOP;
        }

        public override OperationState SaveAs(string name)
        {
            Object selection = GetBrowser().Grid.SelectedItem;
            if (GetBrowser().Grid.ItemsSource != null && GetBrowser().Grid.ItemsSource is List<B>)
            {
                List<B> items = (List<B>)GetBrowser().Grid.ItemsSource;
                foreach (B item in items)
                {
                    if (item.name.ToUpper().Equals(name.ToUpper()) && item != ((B)selection))
                    {
                        DisplayError("Save as", "There is another item named : " + name);
                        return OperationState.STOP;
                    }
                }
            }
            
            if (selection != null)
            {
                try
                {

                    Service.SaveAs(((B)selection).oid, name);
                    Search();
                }
                catch (Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + name);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        public virtual OperationState RenameItem(B item, string newName)
        {
            if (item == null) return OperationState.CONTINUE;
            if (String.IsNullOrWhiteSpace(newName))
            {
                DisplayError("Unable to rename item", "The name can't be empty!");
                return OperationState.STOP;
            }
            if (item.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            Boolean isDuplicate = Service.IsDuplicateName(item.oid, newName);
            if (isDuplicate)
            {
                DisplayError("Rename ", "There is another item named : " + newName);
                return OperationState.STOP;
            }

            try
            {
                Service.Rename(item.oid, newName);
                item.name = newName;
            }
            catch (Domain.BcephalException)
            {
                DisplayError("Unable rename item", "Unable rename : " + newName);
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName)
        {
            Object selection = GetBrowser().Grid.SelectedItem;
            if (GetBrowser().Grid.ItemsSource != null && GetBrowser().Grid.ItemsSource is List<B>)
            {
                List<B> items = (List<B>)GetBrowser().Grid.ItemsSource;
                foreach (B item in items)
                {
                    if (item.name.ToUpper().Equals(newName.ToUpper()) && item != ((B)selection))
                    {
                        DisplayError("Rename ", "There is another item named : " + newName);
                        return OperationState.STOP;
                    }
                }
            }

            if (selection != null)
            {
                try
                {

                    Service.Rename(((B)selection).oid, newName);
                    Search();
                }
                catch (Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + newName);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public virtual OperationState FilterByGroup(int groupOid)
        {
            try
            {
                List<B> items = this.Service.getBrowserDatasByGroup(groupOid);
                GetBrowser().Grid.ItemsSource = items;
                return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
                DisplayError("error", e.Message);
            }

            return OperationState.STOP;
        }

        /// <summary>
        /// Renomme l'objet sélectionné.
        /// </summary>
        /// <returns></returns>
        public override OperationState Rename()
        {
            Object selection = GetBrowser().Grid.SelectedItem;
            if (selection == null) return OperationState.STOP;
            GetBrowser().Grid.IsReadOnly = false;
            GetBrowser().Grid.BeginEdit();
            return OperationState.CONTINUE;
        }

        protected BusyAction action;
        
        /// <summary>
        /// Supprime les objets sélectionnés
        /// </summary>
        /// <returns></returns>
        public override OperationState Delete()
        {
            System.Collections.IList items = GetBrowser().Grid.SelectedItems;
            if (items == null || items.Count == 0) return OperationState.STOP;
            int count = items.Count;
            string message = "You are about to delete " + count + " items.\nDo you want to continue?";
            if (count == 1)
            {
                object item = GetBrowser().Grid.SelectedItem;
                if(item != null) message = "You are about to delete " + item.ToString() + " .\nDo you want to continue?";
            }
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete", message);
            if (result == MessageBoxResult.Yes)
            {

                action = new BusyAction(false)
                {
                    DoWork = () =>
                    {
                        try
                        {
                            action.ReportProgress(0, message);
                            if (!Service.Delete(items)) Kernel.Util.MessageDisplayer.DisplayError("Delete", "Delete fail!");
                            else System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Search()));
                            action.ReportProgress(100, message);                            
                        }
                        catch (BcephalException e)
                        {
                            MessageDisplayer.DisplayError("Error", e.Message);
                            action = null;
                            return OperationState.STOP;
                        }
                        return OperationState.CONTINUE;
                    }

                };
                action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.MainWindow.OnBusyPropertyChanged);
                action.Run();                
            }
            return OperationState.CONTINUE;
        }

        #endregion


        #region View

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public abstract string GetEditorFuntionality();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Le Browser géré par ce controller</returns>
        public Browser<B> GetBrowser()
        {
            return (Browser<B>)this.View;
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar() { return new BrowserToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new BrowserToolBarHandlerBuilder(this); }

        protected override PropertyBar getNewPropertyBar() { return null; }
        protected override void initializePropertyBarData() { }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new BrowserSideBar(); }

        #endregion


        #region Handlers
        
        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected override void initializeViewHandlers() 
        {
            this.GetBrowser().Grid.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChange);
            this.GetBrowser().Grid.PreviewKeyDown += new KeyEventHandler(OnKeyPress);
            this.GetBrowser().Grid.MouseDoubleClick += new MouseButtonEventHandler(OnDoubleClick);
            this.GetBrowser().Grid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
            //this.GetBrowser().Grid.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;

            this.GetBrowser().Grid.FilterChanged += OnFilterChanged;
            this.GetBrowser().NavigationBar.ChangeHandler += OnPageChange;
        }


        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            this.GetBrowser().Grid.BrowserGridContextMenu.NewMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onNewButtonClic);
            this.GetBrowser().Grid.BrowserGridContextMenu.OpenMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onOpenButtonClic);
            this.GetBrowser().Grid.BrowserGridContextMenu.RenameMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onRenameButtonClic);
            this.GetBrowser().Grid.BrowserGridContextMenu.SaveAsMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onSaveAsButtonClic);
            //this.GetBrowser().Grid.BrowserGridContextMenu.CopyMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onco);
            //this.GetBrowser().Grid.BrowserGridContextMenu.PasteMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onNewButtonClic);
            this.GetBrowser().Grid.BrowserGridContextMenu.DeleteMenuItem.Click += new RoutedEventHandler(toolBarHandlerBuilder.onDeleteButtonClic);

        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers() {
            if (this.SideBar != null)
            {
                ((BrowserSideBar)this.SideBar).GroupGroup.GroupTreeview.SelectionChanged += OnGroupSelected;
            }
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            if (this.SideBar != null && this.Service != null)
            {
                Kernel.Domain.BGroup rootGroup = this.Service.GroupService.getRootGroup(SubjectTypeFound());
                ((BrowserSideBar)SideBar).GroupGroup.GroupTreeview.DisplayRoot(rootGroup);
            }            
        }

        protected override void initializePropertyBarHandlers() { }

        protected virtual void OnSelectionChange(object sender, SelectionChangedEventArgs args)
        {
            bool itemsSelected = this.GetBrowser().Grid.SelectedItems.Count > 0;
            this.ToolBar.NewButton.IsEnabled = true;
            this.ToolBar.OpenButton.IsEnabled = itemsSelected;
            this.ToolBar.RenameButton.IsEnabled = this.GetBrowser().Grid.SelectedItems.Count == 1;
            this.ToolBar.DeleteButton.IsEnabled = itemsSelected;

            this.GetBrowser().Grid.BrowserGridContextMenu.NewMenuItem.IsEnabled = this.ToolBar.NewButton.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.OpenMenuItem.IsEnabled = this.ToolBar.OpenButton.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.RenameMenuItem.IsEnabled = this.ToolBar.RenameButton.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.SaveAsMenuItem.IsEnabled = this.GetBrowser().Grid.BrowserGridContextMenu.RenameMenuItem.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.CopyMenuItem.IsEnabled = this.ToolBar.DeleteButton.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.PasteMenuItem.IsEnabled = this.ToolBar.DeleteButton.IsEnabled;
            this.GetBrowser().Grid.BrowserGridContextMenu.DeleteMenuItem.IsEnabled = this.ToolBar.DeleteButton.IsEnabled;

            customizeContextMenu();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            customizeContextMenu();
        }

        protected virtual void customizeContextMenu()
        {
           
        }

        private void OnFilterChanged()
        {
            Search(0);
        }

        private void OnPageChange(object item)
        {
            Search((int)item);
        }

        protected virtual void OnKeyPress(object sender, KeyEventArgs args)
        {
            bool itemsSelected = this.GetBrowser().Grid.SelectedItems.Count > 0;
            if (this.GetBrowser().Grid.IsReadOnly)
            {
                if (itemsSelected && args.Key == Key.Enter)
                {
                    this.Open();
                    args.Handled = true;
                }
                else if (itemsSelected && args.Key == Key.Delete)
                {
                    this.Delete();
                    args.Handled = true;
                }
            }
        }

        protected virtual void OnDoubleClick(object sender, MouseButtonEventArgs args)
        {
            bool itemsSelected = this.GetBrowser().Grid.SelectedItems.Count > 0;
            if (itemsSelected) this.Open();
        }

        protected virtual void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs args)
        {
            B item = (B)this.GetBrowser().Grid.SelectedItem;
            if(args.EditAction == DataGridEditAction.Commit) EditProperty(item, args);            
        }

        private void EditProperty(B item, DataGridCellEditEndingEventArgs args)
        {
            String header = args.Column.Header.ToString();
            Object value = null;
            if (args.EditingElement is TextBox) value = ((TextBox)args.EditingElement).Text;
            else if (args.EditingElement is CheckBox) value = ((CheckBox)args.EditingElement).IsChecked;
            else if (args.EditingElement is ComboBox) value = ((ComboBox)args.EditingElement).SelectedItem;
            OperationState state = EditProperty(item, header, value);
            if (state == OperationState.STOP)
            {
                args.Cancel = true;
                //GetBrowser().Grid.CancelEdit();
                if (args.EditingElement is TextBox) ((TextBox)args.EditingElement).SelectAll();
            }            
        }

        protected virtual OperationState EditProperty(B item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                return RenameItem(item, (String)value);
            }
            return OperationState.CONTINUE;
        }

        private void OnGroupSelected(object newSelection)
        {
            if(newSelection == null) return;
            Kernel.Domain.BGroup group = (Kernel.Domain.BGroup) newSelection;
            Search(group);
            //if (group.oid == null || !group.oid.HasValue) Search();
            //else FilterByGroup(group.oid.Value);
        }

        #endregion

    }
}
