using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.Base;
using Misp.Sourcing.GridViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridEditorController : EditorController<Grille, BrowserData>
    {
        #region Properties
        protected System.Windows.Threading.DispatcherTimer runTimer;
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }

        PeriodName defaultPeriodName { get; set; }

        public static RoutedCommand ExportCommand = new RoutedCommand();
        public static MenuItem ExportMenuItem = BuildContextMenuItem("Export to Excel", ExportCommand);
        public CommandBinding ExportCommandBinding { get; set; }
        
        #endregion

        #region Constructor

        public InputGridEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        #endregion


        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public InputGridEditor getInputGridEditor()
        {
            return (InputGridEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public InputGridService GetInputGridService()
        {
            return (InputGridService)base.Service;
        }

        #endregion


        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Grille grid = GetNewGrid();
            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.AddGrille(grid);
            InputGridEditorItem page = (InputGridEditorItem)getEditor().addOrSelectPage(grid);
            initializePageHandlers(page);
            page.Title = grid.name;
            getEditor().ListChangeHandler.AddNew(grid);
            DisplayActiveColumn();           
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_GRID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Grille grid)
        {
            InputGridEditorItem page = (InputGridEditorItem)getEditor().addOrSelectPage(grid);
            UpdateStatusBar();
            UpdateToolBar(page.EditedObject);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(grid);
            DisplayActiveColumn();
            Search();
            return OperationState.CONTINUE;
        }

        public virtual OperationState LoadGrid()
        {
            OperationState state = OperationState.CONTINUE;
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return state;
            if (!page.EditedObject.oid.HasValue) return state;
            bool result = GetInputGridService().Load(page.EditedObject.oid.Value);
            if (result)
            {
                page.EditedObject.loaded = true;
                UpdateToolBar(page.EditedObject);
            }
            return state;
        }

        public virtual OperationState ClearGrid()
        {
            OperationState state = OperationState.CONTINUE;
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return state;
            if (!page.EditedObject.oid.HasValue) return state;
            bool result = GetInputGridService().Clear(page.EditedObject.oid.Value);
            if (result)
            {
                page.EditedObject.loaded = false;
                UpdateToolBar(page.EditedObject);
            }
            return state;
        }

        protected void UpdateToolBar(Grille grid)
        {
            if (grid == null || !grid.oid.HasValue) ((InputGridToolBar)ToolBar).SetNew();
            else ((InputGridToolBar)ToolBar).SetLoaded(grid.loaded);
        }

        protected virtual Grille GetNewGrid()
        {
            Grille grid = new Grille();
            grid.name = getNewPageName("Input Grid");
            grid.group = GetInputGridService().GroupService.getDefaultGroup();
            grid.visibleInShortcut = true;
            return grid;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Grille grid = GetObjectByName(name);
                if (grid == null) return name; 
                i++;
            }
            return name;
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Save(EditorItem<Grille> page)
        {
            try
            {
                InputGridEditorItem currentPage = (InputGridEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Unable to save Grid", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            Grille editedObject = page.EditedObject;
            
            return OperationState.CONTINUE;
        }

        public OperationState Create(string name, Grille gridInEdition)
        {
            Grille grid = null;//gridInEdition.getCopy(name);
            if (grid == null) return OperationState.STOP;

            EditorItem<Grille> page = getEditor().addOrSelectPage(grid);

            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.AddGrille(grid);
            return Open(grid);
        }

        public bool isSaveAs = false;

        private void updateRunProgress(AllocationRunInfo info)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();

            if (info == null || info.runEnded == true)
            {
                page.IsModify = true;
                OnChange();
                Mask(false);
            }
            else
            {
                int rate = info.totalCellCount != 0 ? (Int32)(info.runedCellCount * 100 / info.totalCellCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";                
            }
        }

        protected void Mask(bool mask, string content = "Saving...")
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page != null) page.getInputGridForm().InputGridSheetForm.Mask(mask);
            ApplicationManager.MainWindow.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = 0;
                ApplicationManager.MainWindow.LoadingLabel.Content = content;

                ApplicationManager.MainWindow.LoadingProgressBar.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingLabel.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingImage.Visibility = Visibility.Hidden;
            }
        }    

        private Grille GetGrid(string name)
        {
            if (!IsNameUsed(name))
            {
                Grille grid = new Grille();
                grid.name = name;
                grid.group = GetInputGridService().GroupService.getDefaultGroup();
                return grid;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Grille obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another Grid named: " + name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();
            foreach (InputGridEditorItem page in getInputGridEditor().getPages())
            {
                if (page.getInputGridForm().InputGridSheetForm.SpreadSheet != null)
                {
                    page.getInputGridForm().InputGridSheetForm.SpreadSheet.Close();
                }
                page.getInputGridForm().SelectionChanged -= OnSelectedTabChange;
            }
            if (getInputGridEditor().NewPage != null && ((InputGridEditorItem)getInputGridEditor().NewPage).getInputGridForm().InputGridSheetForm.SpreadSheet != null)
                ((InputGridEditorItem)getInputGridEditor().NewPage).getInputGridForm().InputGridSheetForm.SpreadSheet.Close();
            ApplicationManager.MainWindow.StatusLabel.Content = "";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosing(object sender, CancelEventArgs args)
        {
            base.OnPageClosing(sender, args);
            if (!args.Cancel)
            {
                InputGridEditorItem page = (InputGridEditorItem)sender;
                if (page.getInputGridForm().InputGridSheetForm.SpreadSheet != null && OperationState.STOP == page.getInputGridForm().InputGridSheetForm.SpreadSheet.Close())
                {
                    try
                    {
                        args.Cancel = true;
                    }
                    catch (Exception)
                    {
                        DisplayError("Unable to save Grid", "Unable to save Excel file.");
                     
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosed(object sender, EventArgs args)
        {
            base.OnPageClosed(sender, args);
            InputGridEditorItem page = (InputGridEditorItem)sender;
            page.getInputGridForm().SelectionChanged -= OnSelectedTabChange; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Grille> page)
        {
            if (page == null) return;
            InputGridForm form = ((InputGridEditorItem)page).getInputGridForm();
            ((InputGridPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = form.InputGridSheetForm.InputGridPropertiesPanel;
            OnSelectedTabChange(null, null);
        }
        
        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState OnChange()
        {
            base.OnChange();
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            page.getInputGridForm().GridForm.gridBrowser.RebuildGrid = true;
            UpdateStatusBar();
            return OperationState.CONTINUE;
        }

                             

        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();

            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Text = name;
            page.EditedObject.name = name;
            base.Rename(name);
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        {
            InputGridEditor editor = new InputGridEditor();
            editor.Service = GetInputGridService();
            return editor; 
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new InputGridToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new InputGridSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new InputGridPropertyBar(); }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {
            //DimensionField.Periodicity = GetStructuredReportService().PeriodicityService.getPeriodicity(); 
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((InputGridToolBar)this.ToolBar).LoadButton.Click += OnLoad;
            ((InputGridToolBar)this.ToolBar).ClearButton.Click += OnClear;
        }

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Grille> page)
        {
            base.initializePageHandlers(page);
            InputGridEditorItem editorPage = (InputGridEditorItem)page;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.GroupService = GetInputGridService().GroupService;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.OnSetTableVisible += OnSetTableVisible;

            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.visibleInShortcutCheckbox.Checked += OnVisibleInShortcutCheck;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.visibleInShortcutCheckbox.Unchecked += OnVisibleInShortcutCheck;

            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.Changed += OnInputGridPropertiesChange;
            editorPage.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.selectionColumnChanged += OnInputGridPropertiesSelectionColumnChange;
            editorPage.getInputGridForm().InputGridSheetForm.SpreadSheet.SelectionChanged += OnSpreadSheetSelectionChanged;
        
            initializeGridFormHandlers(editorPage.getInputGridForm().GridForm);

            editorPage.getInputGridForm().SelectionChanged += OnSelectedTabChange;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            OperationState result = LoadGrid();
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            OperationState result = ClearGrid();
        }

        private void OnSelectedTabChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            if (e == null) return;
            if (!(e.Source is InputGridForm)) return;
            InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
            if (page.getInputGridForm().SelectedIndex == 1) ApplicationManager.MainWindow.displayPropertyBar(this.PropertyBar);
            else
            {
                ApplicationManager.MainWindow.displayPropertyBar(null);
                if (page.getInputGridForm().GridForm.gridBrowser.RebuildGrid) UpdateGridForm();
            }
            e.Handled = true;
        }

        private void UpdateGridForm()
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            page.getInputGridForm().GridForm.EditedObject = page.EditedObject;
            page.getInputGridForm().GridForm.displayObject();
            Search(page.EditedObject.GrilleFilter != null ? page.EditedObject.GrilleFilter.page : 1);
        }

        protected void initializeGridFormHandlers(GridViews.GrilleBrowserForm form)
        {
 	        form.filterForm.periodFilter.DefaultPeriodName = defaultPeriodName;
            form.filterForm.periodFilter.DisplayPeriod(null);
            form.filterForm.periodFilter.ItemChanged += OnPeriodItemChanged;
            form.filterForm.targetFilter.ItemChanged += OnTargetItemChanged;
            form.filterForm.periodFilter.ItemDeleted += OnPeriodItemDeleted;
            form.filterForm.targetFilter.ItemDeleted += OnTargetItemDeleted;
            form.filterForm.searchButton.Click += OnSearchClick;
            form.filterForm.resetButton.Click += OnResetClick;
            form.filterForm.ChangeHandler += OnFilterChange;
            form.toolBar.ChangeHandler += OnPageChange;
            form.EditEventHandler += OnEditColumn;

            form.gridBrowser.DuplicateEventHandler += OnDuplicateRows;
            form.gridBrowser.DeleteEventHandler += OnDeleteRows;
        }


        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            base.RemoveCommands();
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ExportMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ExportCommandBinding);
            }
        }

        protected override void initializeCommands()
        {
            base.initializeCommands();
            this.ExportCommandBinding = new CommandBinding(ExportCommand, ExportCommandExecuted, ExportCommandEnabled);

            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, ExportMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(ExportCommandBinding);
            }
        }


        private void OnDuplicateRows(object obj)
        {
            String message = "You are about to duplicate " + ((List<int>)obj).Count + " row(s).\nDo you want to continue?";
            if (MessageDisplayer.DisplayYesNoQuestion("Duplicate", message) == MessageBoxResult.Yes)
            {
                if (this.GetInputGridService().duplicateGridRows((List<int>)obj))
                {
                    InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
                    Search(page.getInputGridForm().GridForm.toolBar.total + 1);
                }
            }
        }

        private void OnDeleteRows(object obj)
        {
            String message = "You are about to delete " + ((List<int>)obj).Count + " row(s).\nDo you want to continue?";
            if (MessageDisplayer.DisplayYesNoQuestion("Delete", message) == MessageBoxResult.Yes)
            {
                if (this.GetInputGridService().deleteGridRows((List<int>)obj))
                {
                    InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
                    Search(page.getInputGridForm().GridForm.toolBar.current);
                }
            }
        }

        private bool OnEditColumn(GrilleEditedElement element)
        {
            try
            {
                return this.GetInputGridService().editCell(element);
            }
            catch (ServiceExecption e) { }
            return false;
        }

        private void OnPageChange(object item)
        {
            Search((int)item);
        }

        private void OnPeriodItemDeleted(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
            GrilleFilter grilleFilter = page.getInputGridForm().GridForm.filterForm.GrilleFilter;
            if (grilleFilter == null) grilleFilter = new GrilleFilter();
            if (grilleFilter.filterPeriod == null) grilleFilter.filterPeriod = new Period();
            grilleFilter.filterPeriod.SynchronizeDeletePeriodItem(periodItem);
            page.getInputGridForm().GridForm.filterForm.Display(grilleFilter);
            OnChange();
        }

        private void OnTargetItemDeleted(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
            GrilleFilter grilleFilter = page.getInputGridForm().GridForm.filterForm.GrilleFilter;
            if (grilleFilter == null) grilleFilter = new GrilleFilter();
            if (grilleFilter.filterScope == null) grilleFilter.filterScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            grilleFilter.filterScope.SynchronizeDeleteTargetItem(targetItem);
            page.getInputGridForm().GridForm.filterForm.Display(grilleFilter);
            OnChange();
        }

        private void OnTargetItemChanged(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
            GrilleFilter grilleFilter = page.getInputGridForm().GridForm.filterForm.GrilleFilter;
            if (grilleFilter == null) grilleFilter = new GrilleFilter();
            if (grilleFilter.filterScope == null) grilleFilter.filterScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            grilleFilter.filterScope.SynchronizeTargetItems(targetItem);
            page.getInputGridForm().GridForm.filterForm.Display(grilleFilter);
            OnChange();
        }

        private void OnPeriodItemChanged(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
            GrilleFilter grilleFilter = page.getInputGridForm().GridForm.filterForm.GrilleFilter;
            if (grilleFilter == null) grilleFilter = new GrilleFilter();
            if (grilleFilter.filterPeriod == null) grilleFilter.filterPeriod = new Period();
            PeriodItem itemUpdated = grilleFilter.filterPeriod.SynchronizePeriodItems(periodItem);
            page.getInputGridForm().GridForm.filterForm.Display(grilleFilter);
            OnChange();
        }

        public virtual void Search(int currentPage = 0)
        {
            try
            {
                InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
                GrilleFilter filter = page.getInputGridForm().GridForm.filterForm.Fill();
                filter.grid = new Grille();
                filter.grid.code = page.EditedObject.code;
                filter.grid.columnListChangeHandler = page.EditedObject.columnListChangeHandler;
                filter.grid.report = page.EditedObject.report;
                filter.grid.oid = page.EditedObject.oid;
                filter.grid.name = page.EditedObject.name;
                //filter.grid = page.EditedObject;
                filter.page = currentPage;
                filter.pageSize = (int)page.getInputGridForm().GridForm.toolBar.pageSizeComboBox.SelectedItem;
                GrillePage rows = this.GetInputGridService().getGridRows(filter);
                page.getInputGridForm().GridForm.displayPage(rows);
                OnChange();
            }
            catch (ServiceExecption e) { }
        }

        public void Export()
        {
            try
            {
                string filePath = openFileDialog("Export to Excel", null);
                if (filePath == null) return;
                InputGridEditorItem page = (InputGridEditorItem)getEditor().getActivePage();
                GrilleFilter filter = page.getInputGridForm().GridForm.filterForm.Fill();
                filter.grid = page.EditedObject;
                filter.file = filePath;
                bool response = this.GetInputGridService().exportToExcel(filter);
                if (response) MessageDisplayer.DisplayInfo("Export to Excel", "Grid exported!");
            }
            catch (ServiceExecption e) { }
        }

        protected virtual string openFileDialog(string title, string initialDirectory)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = title;
            if (!string.IsNullOrWhiteSpace(initialDirectory)) fileDialog.InitialDirectory = initialDirectory;

            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_EXCEL;
            string listeExtension = "Excel files (";
            string listeExtension1 = "";
            foreach (string ext in HistoryHandler.TAB_FILE_EXTENSION_EXCEL)
            {
                listeExtension += "*" + ext + ",";
                listeExtension1 += "*" + ext + ";";
            }
            int lastComa = listeExtension.LastIndexOf(",");
            listeExtension = listeExtension.Remove(lastComa);
            listeExtension += ")|";

            int lastSemiColon = listeExtension1.LastIndexOf(";");
            listeExtension1 = listeExtension1.Remove(lastSemiColon);

            fileDialog.Filter = listeExtension + listeExtension1;
            Nullable<bool> result = fileDialog.ShowDialog(this.ApplicationManager.MainWindow);

            var filePath = fileDialog.FileName;

            return result == true ? filePath : null;
        }

        private void OnFilterChange()
        {
            Search();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            ((InputGridEditorItem)getEditor().getActivePage()).getInputGridForm().GridForm.filterForm.reset();
            Search();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            Search();
        }





        private void OnSetTableVisible(object item)
        {
            if (!(item is bool)) return;
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (item is Grille) ((Grille)page.EditedObject).visibleInShortcut = (bool)item;
            OnChange();
        }

        private void OnVisibleInShortcutCheck(object sender, RoutedEventArgs e)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (!page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.throwEvent) return;
            page.EditedObject.visibleInShortcut = page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.visibleInShortcutCheckbox.IsChecked.Value;
            OnChange();
        }


        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<BrowserData> designs = Service.getBrowserDatas();
            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.fillTree(new ObservableCollection<BrowserData>(designs));

            ((InputGridSideBar)SideBar).EntityGroup.ModelService = GetInputGridService().ModelService;
            ((InputGridSideBar)SideBar).EntityGroup.InitializeTreeViewDatas();

            Measure rootMeasure = GetInputGridService().MeasureService.getRootMeasure();

            List<CalculatedMeasure> CalculatedMeasures = GetInputGridService().CalculatedMeasureService.getAllCalculatedMeasure();
            ((InputGridSideBar)SideBar).MeasureGroup.MeasureTreeview.DisplayRoot(rootMeasure, CalculatedMeasures);


            PeriodName rootPeriodName = GetInputGridService().PeriodNameService.getRootPeriodName();
            defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            ((InputGridSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);

            Target targetAll = GetInputGridService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((InputGridSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            BGroup group = GetInputGridService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.SelectionChanged += onSelectGridFromSidebar;

            ((InputGridSideBar)SideBar).MeasureGroup.MeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            ((InputGridSideBar)SideBar).EntityGroup.OnSelectTarget += onSelectTargetFromSidebar;
            ((InputGridSideBar)SideBar).EntityGroup.EntityTreeview.SelectionDoubleClick += onDoubleClickSelectTargetFromSidebar;
            ((InputGridSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;

            ((InputGridSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodFromSidebar;
            
        }

        private void ExportCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }

        private void ExportCommandExecuted(object sender, ExecutedRoutedEventArgs e) 
        {
            Export(); 
        }
        
        private void onRightClickFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Ui.Popup.EntityPopup)
            {
                Kernel.Ui.Popup.EntityPopup popup = (Kernel.Ui.Popup.EntityPopup)sender;
                popup.OnValidate += OnValidate;
                Kernel.Domain.Attribute attribute = null;

                if (popup.Tag is Kernel.Domain.Attribute)
                {
                    attribute = (Kernel.Domain.Attribute)popup.Tag;
                    popup.selectedItem.Clear();
                    popup.selectedNames.Clear();


                    popup.ItemSource.Clear();
                    List<Kernel.Domain.AttributeValue> values = GetInputGridService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    values.BubbleSortByName();
                    popup.ItemSource.AddRange(values);
                    popup.selectedItem.AddRange(attribute.FilterAttributeValues);
                    popup.FillSelectedNames();
                    popup.Tag = attribute;
                }
                //else if (popup.Tag is Kernel.Domain.AttributeValue) 
                //{
                //    popup.IsChildren = true;
                //    Kernel.Domain.AttributeValue value = (Kernel.Domain.AttributeValue)popup.Tag;
                //    popup.ItemSource.AddRange(value.childrenListChangeHandler.Items);
                //    popup.Tag = value;
                //}
                popup.IsOpen = true;
                popup.Display();
            }
        }

        private void OnValidate(object sender)
        {
            if (sender == null) return;
            if (!(sender is Array)) return;
            object[] senderArray = (object[])sender;
            bool isAttribute;
            Kernel.Domain.Attribute attribute = null;
            Kernel.Domain.AttributeValue value = null;
            List<Kernel.Domain.AttributeValue> listValues = new List<AttributeValue>(0);

            isAttribute = senderArray[1] is Kernel.Domain.Attribute;
            if (senderArray[0] is IList && senderArray[1] is Kernel.Domain.Target)
            {
                List<object> liste = (List<object>)senderArray[0];
                listValues.AddRange(liste.Cast<Kernel.Domain.AttributeValue>().ToList());
                attribute = isAttribute ? (Kernel.Domain.Attribute)senderArray[1] : null;
                value = !isAttribute ? (Kernel.Domain.AttributeValue)senderArray[1] : null;
            }

            if (isAttribute)
            {
                attribute.valueListChangeHandler.Items.Clear();
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }
            else
            {
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }

            foreach (Kernel.Domain.AttributeValue avalue in listValues)
            {
                attribute.valueListChangeHandler.Items.Add(avalue);
            }
        }

        private Kernel.Domain.Attribute currentAttribute;



        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectGridFromSidebar(object sender)
        {
            if (sender != null && sender is Grille)
            {
                Grille grid = (Grille)sender;
                EditorItem<Grille> page = getInputGridEditor().getPage(grid.name);
                if (page != null)
                {
                    ((InputGridEditorItem)page).getInputGridForm().InputGridSheetForm.SpreadSheet.protectSheet(false);
                    page.fillObject();
                    getInputGridEditor().selectePage(page);
                    ((InputGridEditorItem)page).getInputGridForm().InputGridSheetForm.SpreadSheet.protectSheet();
                    
                }
                else if (grid.oid != null && grid.oid.HasValue)
                {
                    this.Open(grid.oid.Value);

                }
                else
                {
                    ((InputGridEditorItem)page).getInputGridForm().InputGridSheetForm.SpreadSheet.protectSheet(false);
                    page = getInputGridEditor().addOrSelectPage(grid);
                    initializePageHandlers(page);
                    page.Title = grid.name;

                    getInputGridEditor().ListChangeHandler.AddNew(grid);
                    ((InputGridEditorItem)page).getInputGridForm().InputGridSheetForm.SpreadSheet.protectSheet();
                }
                InputGridEditorItem pageOpen = (InputGridEditorItem)getInputGridEditor().getActivePage();
                UpdateStatusBar();
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une mesure sur la sidebar.
        /// Cette opération a pour but d'assigner la mesure sélectionnée 
        /// aux cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La mesure sélectionnée</param>
        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && (sender is Measure || sender is CalculatedMeasure))
            {
                InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
                if (page == null) return;
                if (page.getInputGridForm().SelectedIndex == 1)
                    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(sender);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void onSelectPeriodFromSidebar(object sender)
        {
            if (sender != null)
            {
                InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
                if (page == null) return;
                if (page.getInputGridForm().SelectedIndex == 1)
                    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(sender);
                else
                {
                    if (sender is PeriodInterval)
                    {
                        PeriodInterval periodInterval = (PeriodInterval)sender;
                        page.getInputGridForm().GridForm.filterForm.periodFilter.SetPeriodInterval(periodInterval);
                    }
                    else if (sender is PeriodName)
                    {
                        page.getInputGridForm().GridForm.filterForm.periodFilter.SetPeriodItemName(((PeriodName)sender).name);
                    }
                }
            }
            
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTargetFromSidebar(object target)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return;
            if (page.getInputGridForm().SelectedIndex == 1 && target is Kernel.Domain.Attribute)
                page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(target);
            else if (page.getInputGridForm().SelectedIndex != 1 && !(target is Kernel.Domain.Attribute)) 
                page.getInputGridForm().GridForm.filterForm.targetFilter.SetTargetValue((Target)target);
          
        }

        protected void onDoubleClickSelectTargetFromSidebar(object sender)
        {
            onSelectTargetFromSidebar(sender);
        }
                
        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEditedNewName();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextLostFocus(object sender, RoutedEventArgs args)
        {
            ValidateEditedNewName();
        }

        protected void onGroupFieldChange()
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            string name = page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.groupField.textBox.Text;
            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.updateGrille(name, page.Title, true);
            OnChange();
        }
        
        /// <summary>
        /// Cette méthode est éxécutée lorsque la selection change dans le SpreadSheet.
        /// On affiche le nom de la cellule active.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSpreadSheetSelectionChanged(Kernel.Ui.Office.ExcelEventArg args)
        {
            DisplayActiveColumn();
        }

        protected void DisplayActiveColumn()
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Cell activeCell = page.getInputGridForm().InputGridSheetForm.SpreadSheet.getActiveCell();
            String activeCellName = activeCell != null ? activeCell.Name : null;
            page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SelecteColumn(activeCell);
        }


        private void OnInputGridPropertiesSelectionColumnChange(object obj)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return;
            if (obj is GrilleColumn)
            {
                GrilleColumn column = (GrilleColumn)obj;
                
                if (column.type.Equals(ParameterType.SCOPE.ToString()))
                {
                    //Target target = column.scope;
                    //if (target.typeName.Equals(typeof(AttributeValue).Name, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    Kernel.Domain.AttributeValue value = GetInputGridService().ModelService.getAttributeValue(target.oid.Value, target.name);
                    //   column.SetValue(value);
                    //   page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.ColumnForms.Display(column);
                    //}

                    //if (target.typeName.Equals(typeof(Misp.Kernel.Domain.Attribute).Name, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    Misp.Kernel.Domain.Attribute value = GetInputGridService().ModelService.getAttributeByOid(target.oid.Value);
                    //    column.SetValue(value);
                    //    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.ColumnForms.Display(column);
                    //}
                    //if (target.typeName.Equals(typeof(Entity).Name, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    Misp.Kernel.Domain.Entity value = GetInputGridService().ModelService.getEntityByOid(target.oid.Value);
                    //    column.SetValue(value);
                    //    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.ColumnForms.Display(column);
                    //}

                }

            }
           
        }

        private void OnInputGridPropertiesChange(object obj)
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return;
            if(obj is bool && (bool)obj) BuildSheetTable();
            OnChange();
        }

        protected void BuildSheetTable()
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            if (page == null) return;
            page.getInputGridForm().InputGridSheetForm.BuildSheetTable();
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateStatusBar()
        {
            
        }


        #endregion

        public override bool validateName(EditorItem<Grille> page, string name)
        {
            if(!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        private bool IsRenameOnDoubleClick = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
            InputGridEditorItem page = (InputGridEditorItem)getInputGridEditor().getActivePage();
            Grille table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Grid name can't be mepty!");
                page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.SelectAll();
                page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Focus();
                return OperationState.STOP;
            }


            foreach (InputGridEditorItem unInputTable in getInputGridEditor().getPages())
            {
                if (unInputTable != getInputGridEditor().getActivePage() && newName == unInputTable.Title)
                {
                    DisplayError("Duplicate Name", "There is another Grid named: " + newName);
                    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Text = page.Title;
                    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.SelectAll();
                    page.getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.NameTextBox.Focus();
                    return OperationState.STOP;
                }
                    }
            if(!IsRenameOnDoubleClick)
            if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.updateGrille(newName, table.name, false);
            table.name = newName;
            page.getInputGridForm().InputGridSheetForm.SpreadSheet.ChangeTitleBarCaption(newName);
            page.Title = newName;
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Grille GetObjectByName(string name)
        {
            return ((InputGridSideBar)SideBar).GrilleGroup.GrilleTreeview.getGrilleByName(name);
            //return GetInputGridService().getByName(name);
        }

        public override Kernel.Application.OperationState Search(object oid) { return OperationState.CONTINUE; }
        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}

