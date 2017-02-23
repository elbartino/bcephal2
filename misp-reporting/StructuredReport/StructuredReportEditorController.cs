using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Group;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Application;
using System.Windows.Controls;
using System.Threading;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Ui.Office;
using System.Windows.Navigation;
using Misp.Kernel.Util;
using Misp.Kernel.Service;
using System.Windows.Data;
using System.Collections;
using Misp.Kernel.Domain.Browser;
using Misp.Sourcing.Table;
using Misp.Kernel.Task;
using System.IO;

namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportEditorController : EditorController<Misp.Kernel.Domain.StructuredReport, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        protected System.Windows.Threading.DispatcherTimer runTimer;
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        #endregion

        #region Constructor

        public StructuredReportEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.STRUCTURED_REPORT;
        }

        #endregion


        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public StructuredReportEditor getStructuredReportEditor()
        {
            return (StructuredReportEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux StructuredReports.
        /// </summary>
        /// <returns>StructuredReportService</returns>
        public StructuredReportService GetStructuredReportService()
        {
            return (StructuredReportService)base.Service;
        }

        #endregion


        #region Operations

        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Kernel.Domain.StructuredReport report = GetNewStructuredReport();
            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.AddStructuredReport(report);
            StructuredReportEditorItem page = (StructuredReportEditorItem)getEditor().addOrSelectPage(report);
            initializePageHandlers(page);
            page.Title = report.name;
            getEditor().ListChangeHandler.AddNew(report);
            page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.ItemForm.PeriodicityService = GetStructuredReportService().PeriodicityService;
            DisplayActiveColumn();
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.STRUCTURED_REPORT;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Kernel.Domain.StructuredReport report)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getEditor().addOrSelectPage(report);
            UpdateStatusBar();
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(report);
            page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.ItemForm.PeriodicityService = GetStructuredReportService().PeriodicityService;
            DisplayActiveColumn();
            return OperationState.CONTINUE;
        }

        protected virtual Kernel.Domain.StructuredReport GetNewStructuredReport()
        {
            Kernel.Domain.StructuredReport report = new Kernel.Domain.StructuredReport();
            report.name = getNewPageName("Structured Report");
            report.group = GetStructuredReportService().GroupService.getDefaultGroup();
            report.visibleInShortcut = true;
            return report;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Kernel.Domain.StructuredReport report = GetObjectByName(name);
                if (report == null) return name;
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
        public override OperationState Save(EditorItem<Kernel.Domain.StructuredReport> page)
        {
            try
            {
                StructuredReportEditorItem currentPage = (StructuredReportEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Unable to save Structured Report", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            Kernel.Domain.StructuredReport editedObject = page.EditedObject;

            return OperationState.CONTINUE;
        }

        public OperationState Create(string name, Kernel.Domain.StructuredReport reportInEdition)
        {
            Kernel.Domain.StructuredReport report = null;//reportInEdition.getCopy(name);
            if (report == null) return OperationState.STOP;

            EditorItem<Kernel.Domain.StructuredReport> page = getEditor().addOrSelectPage(report);

            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.AddStructuredReport(report);
            return Open(report);
        }

        public bool isSaveAs = false;



        protected StructuredReportRunData nextRunData;
        public virtual OperationState Run()
        {
            OperationState state = OperationState.CONTINUE;
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page == null) return state;
            nextRunData = null;
            String filePath = openSaveDialog();
            if (string.IsNullOrEmpty(filePath)) return state;

            StructuredReportRunData data = new StructuredReportRunData();
            data.filePath = filePath;
            if (page.EditedObject.oid.HasValue) data.oid = page.EditedObject.oid.Value;

            //if (page.IsModify)
            //{
            //    nextRunData = data;
            //    return Save(page);
            //}

            GetStructuredReportService().RunHandler += updateRunProgress;
            Mask(true, "Running...");
            GetStructuredReportService().Run(data);
            return state;
        }

        private void updateRunProgress(AllocationRunInfo info)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();

            if (info == null || info.runEnded == true)
            {
                GetStructuredReportService().RunHandler -= updateRunProgress;
                page.IsModify = true;
                nextRunData = null;
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
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page != null) page.getStructuredReportForm().Mask(mask);
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


        /// <summary>
        /// open dialog for export file
        /// </summary>
        /// <returns></returns>
        public String openSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = "Run Structured Report";
            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_CSV;
            fileDialog.Filter = "Excel files (*" + HistoryHandler.FILE_EXTENSION_CSV + ")|*" + HistoryHandler.FILE_EXTENSION_CSV;
            Nullable<bool> result = fileDialog.ShowDialog();
            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;
            if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return null;
            return filePath;
        }


        private Kernel.Domain.StructuredReport GetStructuredReport(string name)
        {
            if (!IsNameUsed(name))
            {
                Kernel.Domain.StructuredReport design = new Kernel.Domain.StructuredReport();
                design.name = name;
                design.group = GetStructuredReportService().GroupService.getDefaultGroup();
                return design;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Kernel.Domain.StructuredReport obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another Structured Report named: " + name);
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

            foreach (StructuredReportEditorItem page in getStructuredReportEditor().getPages())
            {
                if (page.getStructuredReportForm().SpreadSheet != null)
                {
                    //page.getInputTableForm().SpreadSheet.Close(page.getInputTableForm().SpreadSheet.DocumentUrl);
                    page.getStructuredReportForm().SpreadSheet.Close();
                }
            }
            if (getStructuredReportEditor().NewPage != null && ((StructuredReportEditorItem)getStructuredReportEditor().NewPage).getStructuredReportForm().SpreadSheet != null)
                ((StructuredReportEditorItem)getStructuredReportEditor().NewPage).getStructuredReportForm().SpreadSheet.Close();
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
                StructuredReportEditorItem page = (StructuredReportEditorItem)sender;
                if (page.getStructuredReportForm().SpreadSheet != null && OperationState.STOP == page.getStructuredReportForm().SpreadSheet.Close())
                {
                    try
                    {
                        args.Cancel = true;
                    }
                    catch (Exception)
                    {
                        DisplayError("Unable to save Structured Report", "Unable to save Excel file.");

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Kernel.Domain.StructuredReport> page)
        {
            if (page == null) return;
            StructuredReportForm form = ((StructuredReportEditorItem)page).getStructuredReportForm();
            ((StructuredReportPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = form.StructuredReportPropertiesPanel;
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
            UpdateStatusBar();
            return OperationState.CONTINUE;
        }


        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();

            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Text = name;
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
        protected override IView getNewView() { return new StructuredReportEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new StructuredReportToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new StructuredReportSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new StructuredReportPropertyBar(); }

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
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Kernel.Domain.StructuredReport> page)
        {
            base.initializePageHandlers(page);
            StructuredReportEditorItem editorPage = (StructuredReportEditorItem)page;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.groupField.GroupService = GetStructuredReportService().GroupService;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.OnAllocateEach += OnAllocateEach;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.OnSetTableVisible += OnSetTableVisible;

            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.visibleInShortcutCheckbox.Checked += OnVisibleInShortcutCheck;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.visibleInShortcutCheckbox.Unchecked += OnVisibleInShortcutCheck;

            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.Changed += OnStructuredReportPropertiesChange;
            editorPage.getStructuredReportForm().StructuredReportPropertiesPanel.selectionColumnChanged += OnStructuredReportPropertiesSelectionColumnChange;
            editorPage.getStructuredReportForm().SpreadSheet.SelectionChanged += OnSpreadSheetSelectionChanged;
        }

        private void OnSetTableVisible(object item)
        {
            if (!(item is bool)) return;
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (item is Kernel.Domain.TransformationTable) ((Kernel.Domain.TransformationTable)page.EditedObject).visibleInShortcut = (bool)item;
            if (item is Kernel.Domain.StructuredReport) ((Kernel.Domain.StructuredReport)page.EditedObject).visibleInShortcut = (bool)item;
            OnChange();
        }

        private void OnVisibleInShortcutCheck(object sender, RoutedEventArgs e)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (!page.getStructuredReportForm().StructuredReportPropertiesPanel.throwEvent) return;
            if (page.EditedObject is Kernel.Domain.StructuredReport)
            {
                ((Kernel.Domain.StructuredReport)page.EditedObject).visibleInShortcut = page.getStructuredReportForm().StructuredReportPropertiesPanel.visibleInShortcutCheckbox.IsChecked.Value;
            }
            else if (page.EditedObject is Kernel.Domain.TransformationTable)
            {
                ((Kernel.Domain.TransformationTable)page.EditedObject).visibleInShortcut = page.getStructuredReportForm().StructuredReportPropertiesPanel.visibleInShortcutCheckbox.IsChecked.Value;
            }
            OnChange();
        }

        private void OnAllocateEach(bool value)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            ((Kernel.Domain.TransformationTable)page.EditedObject).allocateEachLoop = value;
            OnChange();
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((StructuredReportToolBar)this.ToolBar).RunButton.Click += OnRun;
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            if (Run() == OperationState.CONTINUE)
                this.AfterSave();
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<BrowserData> designs = Service.getBrowserDatas();
            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.fillTree(new ObservableCollection<BrowserData>(designs));

            ((StructuredReportSideBar)SideBar).EntityGroup.InitializeData();
            
            ((StructuredReportSideBar)SideBar).MeasureGroup.InitializeMeasure(true);

            ((StructuredReportSideBar)SideBar).PeriodGroup.InitializeData();

            
            Target targetAll = GetStructuredReportService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((StructuredReportSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            BGroup group = GetStructuredReportService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.SelectionChanged += onSelectStructuredReportFromSidebar;

            ((StructuredReportSideBar)SideBar).MeasureGroup.Tree.Click += onSelectMeasureFromSidebar;
            ((StructuredReportSideBar)SideBar).EntityGroup.Tree.Click += onSelectTargetFromSidebar;
            ((StructuredReportSideBar)SideBar).EntityGroup.Tree.DoubleClick += onDoubleClickSelectTargetFromSidebar;
            ((StructuredReportSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
            ((StructuredReportSideBar)SideBar).PeriodGroup.Tree.Click += onSelectPeriodFromSidebar;
       
            ((StructuredReportSideBar)SideBar).TreeLoopGroup.TransformationTreeLoopTreeview.SelectionChanged += onSelectLoopFromSidebar;
            ((StructuredReportSideBar)SideBar).SpecialGroup.SelectionChanged += onSelectSpecialFromSidebar;
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectStructuredReportFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Domain.StructuredReport)
            {
                Kernel.Domain.StructuredReport design = (Kernel.Domain.StructuredReport)sender;
                EditorItem<Kernel.Domain.StructuredReport> page = getStructuredReportEditor().getPage(design.name);
                if (page != null)
                {
                    page.fillObject();
                    getStructuredReportEditor().selectePage(page);
                }
                else if (design.oid != null && design.oid.HasValue)
                {
                    this.Open(design.oid.Value);
                }
                else
                {
                    page = getStructuredReportEditor().addOrSelectPage(design);
                    initializePageHandlers(page);
                    page.Title = design.name;

                    getStructuredReportEditor().ListChangeHandler.AddNew(design);
                }
                StructuredReportEditorItem pageOpen = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
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
                //Measure measure = (Measure)sender;
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
                if (page == null) return;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.SetValue(sender);
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
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
                if (page == null) return;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.SetValue(sender);
            }

        }


        protected void onSelectLoopFromSidebar(object sender)
        {
            if (sender != null && sender is TransformationTreeItem)
            {
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
                if (page == null) return;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.SetValue(sender);
            }
        }

        protected void onSelectSpecialFromSidebar(object sender)
        {
            if (sender != null && sender is StructuredReportColumn.Type)
            {
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
                if (page == null) return;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.SetValue(sender);
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
                if (page == null) return;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.SetValue(sender);
            }
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
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Text = page.Title;
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
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            string name = page.getStructuredReportForm().StructuredReportPropertiesPanel.groupField.textBox.Text;
            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.updateStructuredReport(name, page.Title, true);
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
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Cell activeCell = page.getStructuredReportForm().SpreadSheet.getActiveCell();
            String activeCellName = activeCell != null ? activeCell.Name : null;
            page.getStructuredReportForm().StructuredReportPropertiesPanel.SelecteColumn(activeCell);
        }


        private void OnStructuredReportPropertiesSelectionColumnChange(object obj)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page == null) return;
            if (obj is StructuredReportColumn)
            {
                StructuredReportColumn column = (StructuredReportColumn)obj;

                if (column.type.Equals(StructuredReportColumn.Type.TARGET.ToString()))
                {
                    Target target = column.scope;
                    if (target.typeName.Equals(typeof(AttributeValue).Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Kernel.Domain.AttributeValue value = GetStructuredReportService().ModelService.getAttributeValue(target.oid.Value, target.name);
                        column.SetValue(value);
                        page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.Display(column);
                    }

                    if (target.typeName.Equals(typeof(Misp.Kernel.Domain.Attribute).Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Misp.Kernel.Domain.Attribute value = GetStructuredReportService().ModelService.getAttributeByOid(target.oid.Value);
                        column.SetValue(value);
                        page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.Display(column);
                    }
                    if (target.typeName.Equals(typeof(Entity).Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Misp.Kernel.Domain.Entity value = GetStructuredReportService().ModelService.getEntityByOid(target.oid.Value);
                        column.SetValue(value);
                        page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.Display(column);
                    }

                }

            }

        }

        private void OnStructuredReportPropertiesChange(object obj)
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page == null) return;
            if (obj is bool && (bool)obj) BuildSheetTable();
            OnChange();
        }

        protected void BuildSheetTable()
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            if (page == null) return;
            page.getStructuredReportForm().BuildSheetTable();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateStatusBar()
        {
            //StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            //Kernel.Ui.Office.Range range = page.getStructuredReportForm().SpreadSheet.GetSelectedRange();
        }


        #endregion

        public override bool validateName(EditorItem<Kernel.Domain.StructuredReport> page, string name)
        {
            foreach (char c in name.ToCharArray())
            {
                if (Path.GetInvalidFileNameChars().Contains(c))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Invalid Name", "The name can't containt: " + c);
                    return false;
                }
            }
            if (!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        public bool validateName(string name)
        {
            foreach (char c in name.ToCharArray())
            {
                if (Path.GetInvalidFileNameChars().Contains(c))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Invalid Name", "The name can't containt: " + c);
                    return false;
                }
            }
            return true;
        }

        private bool IsRenameOnDoubleClick = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
            StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();
            Kernel.Domain.StructuredReport table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Text.Trim();

            if (!validateName(newName)) return OperationState.STOP;

            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Structured Report name can't be mepty!");
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.SelectAll();
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetStructuredReportService().getByName(newName) != null) found = true;

            foreach (StructuredReportEditorItem unInputTable in getStructuredReportEditor().getPages())
            {
                if ((found && newName != getStructuredReportEditor().getActivePage().Title) || (unInputTable != getStructuredReportEditor().getActivePage() && newName == unInputTable.Title))
                {
                    DisplayError("Duplicate Name", "There is another Structured Report named: " + newName);
                    page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Text = page.Title;
                    page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.SelectAll();
                    page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Focus();
                    return OperationState.STOP;
                }
            }
            if (!IsRenameOnDoubleClick)
                if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.updateStructuredReport(newName, table.name, false);
            table.name = newName;
            page.getStructuredReportForm().SpreadSheet.ChangeTitleBarCaption(newName);
            page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Kernel.Domain.StructuredReport GetObjectByName(string name)
        {
            return ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.getStructuredReportByName(name);
        }

        public override Kernel.Application.OperationState Search(object oid) { return OperationState.CONTINUE; }
        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
