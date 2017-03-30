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
using Misp.Kernel.Ui.Sidebar;
using System.Windows.Threading;


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
        public BrowserController() : base() {}

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
            foreach (Object item in GetBrowser().Form.Grid.SelectedItems)
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
            ApplicationManager.MainWindow.IsBussy = true;
            Kernel.Application.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
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
                   }
                   catch (ServiceExecption e)
                   {
                       DisplayError("error", e.Message);
                   }
                   finally
                   {
                       ApplicationManager.MainWindow.IsBussy = false;
                   }
               }));
            return OperationState.CONTINUE;
        }

        public override OperationState SaveAs(string name)
        {
            Object selection = GetBrowser().Form.Grid.SelectedItem;
            if (GetBrowser().Form.Grid.ItemsSource != null && GetBrowser().Form.Grid.ItemsSource is List<B>)
            {
                List<B> items = (List<B>)GetBrowser().Form.Grid.ItemsSource;
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
            Object selection = GetBrowser().Form.Grid.SelectedItem;
            if (GetBrowser().Form.Grid.ItemsSource != null && GetBrowser().Form.Grid.ItemsSource is List<B>)
            {
                List<B> items = (List<B>)GetBrowser().Form.Grid.ItemsSource;
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
                GetBrowser().Form.Grid.ItemsSource = items;
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
            //Object selection = GetBrowser().Form.Grid.SelectedItem;
            //if (selection == null) return OperationState.STOP;
            //GetBrowser().Form.Grid.IsReadOnly = false;
            //GetBrowser().Form.Grid.BeginEdit();
            return OperationState.CONTINUE;
        }

        protected BusyAction action;
        
        /// <summary>
        /// Supprime les objets sélectionnés
        /// </summary>
        /// <returns></returns>
        public override OperationState Delete()
        {
            System.Collections.IList items = GetBrowser().Form.Grid.SelectedItems;
            if (items == null || items.Count == 0) return OperationState.STOP;
            int count = items.Count;
            string message = "You are about to delete " + count + " items.\nDo you want to continue?";
            if (count == 1)
            {
                object item = GetBrowser().Form.Grid.SelectedItem;
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

        /// <summary>
        /// Effectue l'initialisation du controller.
        /// On initialize la vue et on met this.IsModify = false
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Initialize()
        {
            base.Initialize();
            List<Right> rights = null;
            PrivilegeObserver observer = new PrivilegeObserver();            
            if (this.ToolBar != null) this.ToolBar.Customize(this.FunctionalityCode, observer, rights, false);
            bool edit = observer.hasPrivilege(this.FunctionalityCode, Domain.RightType.CREATE);
            GetBrowser().SetReadOnly(!edit);
            return OperationState.CONTINUE;
        }

        #endregion


        #region Handlers
        
        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected override void initializeViewHandlers() 
        {
            this.GetBrowser().Form.Grid.SelectionChanged += OnSelectionChange;
            this.GetBrowser().Form.Grid.PreviewKeyDown += new KeyEventHandler(OnKeyPress);
            this.GetBrowser().Form.Grid.MouseDoubleClick += new MouseButtonEventHandler(OnDoubleClick);
            //this.GetBrowser().Form.Grid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
            
            //this.GetBrowser().Form.Grid.FilterChanged += OnFilterChanged;
            this.GetBrowser().Form.PaginationBar.ChangeHandler += OnPageChange;
        }
        

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            this.GetBrowser().Form.Grid.NewMenuItem.ItemClick += toolBarHandlerBuilder.onNewButtonClic;
            this.GetBrowser().Form.Grid.OpenMenuItem.ItemClick += toolBarHandlerBuilder.onOpenButtonClic;
            this.GetBrowser().Form.Grid.RenameMenuItem.ItemClick += toolBarHandlerBuilder.onRenameButtonClic;
            this.GetBrowser().Form.Grid.SaveAsMenuItem.ItemClick += toolBarHandlerBuilder.onSaveAsButtonClic;
            this.GetBrowser().Form.Grid.DeleteMenuItem.ItemClick += toolBarHandlerBuilder.onDeleteButtonClic;

            this.GetBrowser().Form.Grid.View.ContextMenuOpening += OnContextMenuOpening;
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            customizeContextMenuForSelection();
        }

        private void customizeContextMenuForSelection()
        {
            int count = this.GetBrowser().Form.Grid.SelectedItems.Count;
            bool itemsSelected = count > 0;

            bool create = true;
            bool saveAs = count == 1;
            bool delete = count == 1;

            if (count == 1)
            {
                BrowserData item = (BrowserData)this.GetBrowser().Form.Grid.SelectedItem;
                List<Right> rights = null;
                PrivilegeObserver observer = new PrivilegeObserver();
                if (!ApplicationManager.User.IsAdmin())
                {
                    RightService service = ApplicationManager.ControllerFactory.ServiceFactory.GetRightService();
                    rights = service.getUserRights(this.SubjectType.label, item.oid);
                }
                saveAs = RightsUtil.HasRight(Domain.RightType.SAVE_AS, rights);
                delete = RightsUtil.HasRight(Domain.RightType.DELETE, rights);
                create = observer.hasPrivilege(this.FunctionalityCode, Domain.RightType.CREATE);
            }
            this.GetBrowser().Form.Grid.NewMenuItem.IsEnabled = create;
            this.GetBrowser().Form.Grid.OpenMenuItem.IsEnabled = itemsSelected;
            this.GetBrowser().Form.Grid.RenameMenuItem.IsEnabled = saveAs && count == 1;
            this.GetBrowser().Form.Grid.SaveAsMenuItem.IsEnabled = saveAs && count == 1;
            //this.GetBrowser().Form.Grid.CopyMenuItem.IsEnabled = itemsSelected && create;
            //this.GetBrowser().Form.Grid.PasteMenuItem.IsEnabled = create;
            this.GetBrowser().Form.Grid.DeleteMenuItem.IsEnabled = itemsSelected && delete;
            customizeContextMenu();
        }

        protected virtual void OnSelectionChange(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
        {
            customizeContextMenuForSelection();
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
            if (this.SideBar != null && this.Service != null && this.Service.GroupService != null)
            {
                Kernel.Domain.BGroup rootGroup = this.Service.GroupService.getRootGroup(SubjectTypeFound());
                ((BrowserSideBar)SideBar).GroupGroup.GroupTreeview.DisplayRoot(rootGroup);
            }            
        }

        protected override void initializePropertyBarHandlers() { }

        
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
            bool itemsSelected = this.GetBrowser().Form.Grid.SelectedItems.Count > 0;
            if (this.GetBrowser().IsReadOnly)
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
            bool itemsSelected = this.GetBrowser().Form.Grid.SelectedItems.Count > 0;
            if (itemsSelected) this.Open();
        }

        protected virtual void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs args)
        {
            B item = (B)this.GetBrowser().Form.Grid.SelectedItem;
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
