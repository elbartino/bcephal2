using Microsoft.Office.Core;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Util;
using Misp.Planification.Tranformation;
using Misp.Planification.Tranformation.InstructionControls;
using Misp.Reporting.Report;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Misp.Planification.PresentationView
{
    public class PresentationEditorController : EditorController<Presentation, Misp.Kernel.Domain.Browser.BrowserData>
    {

        #region 
        public static RoutedCommand ImportCommand = new RoutedCommand();
        public static RoutedCommand ExportCommand = new RoutedCommand();
         
        public TransformationSlideDialog slideDialog { get; set; }
        public ObservableCollection<TransformationTreeItem> Loops { get; set; }

        public static MenuItem ImportMenuItem = BuildContextMenuItem("Import", ImportCommand);
        public static MenuItem ExportMenuItem = BuildContextMenuItem("Export", ExportCommand);
      
        public CommandBinding ImportCommandBinding { get; set; }
        public CommandBinding ExportCommandBinding { get; set; }
      
        private void ImportCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ImportCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onImportButtonClic(sender, e); }

        private void ExportCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ExportCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onExportButtonClic(sender, e); }

        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }

        public ReportEditorController reportEditorController { get; set; }

        protected int? currentTreeOid { get; set; }
        #endregion


        #region Events

        public event OnSelectionChangeEventHandler OnSelectionChange;
        public delegate void OnSelectionChangeEventHandler(string statusBarText);

        public event OnRemoveNewPageEventHandler OnRemoveNewPage;
        public delegate void OnRemoveNewPageEventHandler(bool remove = false);

        #endregion

        public override void CustomizeForUser(EditorItem<Presentation> page)
        {
            page.CanRename = true;
            page.CanSave = true;
        }

        public void RemoveMenuCommands()
        {
            RemoveCommands();
        }

        public void InitialiazeMenuCommand() 
        {
            initializeCommands();
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            if (slideDialog != null)
            {
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(ImportMenuItem);
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(ExportMenuItem);

                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(DeleteMenuItem);
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(SaveAsMenuItem);
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(SaveMenuItem);
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(RenameMenuItem);
                slideDialog.dockingManager.DocumentContextMenu.Items.Remove(NewMenuItem);
                

                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(ImportCommandBinding);
                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(ExportCommandBinding);

                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(NewCommandBinding);
                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(RenameCommandBinding);
                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveCommandBinding);
                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveAsCommandBinding);
                slideDialog.dockingManager.DocumentContextMenu.CommandBindings.Remove(DeleteCommandBinding);
                
            }
        }

        protected override void initializeCommands()
        {
            base.initializeCommands();
            this.ImportCommandBinding = new CommandBinding(ImportCommand, ImportCommandExecuted, ImportCommandEnabled);
            this.ExportCommandBinding = new CommandBinding(ExportCommand, ExportCommandExecuted, ExportCommandEnabled);
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputTables.
        /// </summary>
        /// <returns>PresentationService</returns>
        public PresentationService GetPresentationService()
        {
            return (PresentationService)base.Service;
        }

        public override Kernel.Application.OperationState Delete()
        {
            return OperationState.CONTINUE;
        }

        public override Kernel.Application.OperationState Create()
        {
            Presentation presentation = GetNewPresentation();
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.AddPresentation(presentation);
            PresentationEditorItem page = (PresentationEditorItem)getEditor().addOrSelectPage(presentation);
            OperationState sate = page.getPresentationForm().SlideView.Open();
            if (sate == OperationState.STOP)
            {
                MessageDisplayer.DisplayError("Bcephal - MS PowerPoint Error", "Unable to create powerPoint file!");
                return OperationState.STOP;
            }
            String fileName = page.getPresentationForm().SlideView.DocumentName;
            
            presentation.slideFileName = presentation.name + Path.GetExtension(fileName);
            presentation.slideFileExtension = Path.GetExtension(fileName);
            initializePageHandlers(page);
            page.Title = presentation.name;
            getPresentationEditor().ListChangeHandler.AddNew(presentation);
            page.getPresentationForm().EditedObject = presentation;
            page.getPresentationForm().listeBrowserData = GetPresentationService().ReportService.getBrowserDatas();            
            if(!String.IsNullOrEmpty(Kernel.Util.UserPreferencesUtil.GetPowerPowerPointSavingRepository()))
            page.EditedObject.userSavingDir = Kernel.Util.UserPreferencesUtil.GetPowerPowerPointSavingRepository();
            page.getPresentationForm().displayObject();
            page.IsModify = true;
            refreshInReportSidebar(page);
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.PRESENTATION;
        }

        /// <summary>
        /// Ouvre la table passée en parametre dans l'éditeur.
        /// </summary>
        /// <param name="table">La table à ouvrir</param>
        /// <returns>
        /// - CONTINUE si l'ouverture de la table se termine avec succès. 
        /// - STOP sinon
        /// </returns>
        public override OperationState Open(Presentation presentation)
        {
            string filePath = GetPresentationService().FileService.FileTransferService.downloadPresentation(presentation.name + EdrawSlide.POWER_POINT_EXT);
            if (string.IsNullOrWhiteSpace(filePath))
            {

            }

            filePath = filePath + presentation.name + EdrawSlide.POWER_POINT_EXT;
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.AddPresentationIfNatExist(presentation);
            PresentationEditorItem page = (PresentationEditorItem)getEditor().addOrSelectPage(presentation);
            OperationState sate = page.getPresentationForm().SlideView.Open(filePath);
            if (sate == OperationState.STOP)
            {
                MessageDisplayer.DisplayError("Bcephal - MS PowerPoint Error", "Unable to create powerPoint file!");
                return OperationState.STOP;
            }
            refreshInReportSidebar(page);
            ((PresentationEditorItem)page).getPresentationForm().displayObject();
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(presentation);
            return OperationState.CONTINUE;
        }


        protected override Kernel.Ui.Base.IView getNewView()
        {
            return new PresentationEditor(this.SubjectType, this.FunctionalityCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public PresentationEditor getPresentationEditor()
        {
            return (PresentationEditor)this.View;
        }


        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            return new  PresentationToolBar();
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new PresentationToolBarHandlerBuilder(this);
        }

        protected override SideBar getNewSideBar()
        {
            return new PresentationSideBar();
        }

        protected override Kernel.Ui.Base.PropertyBar getNewPropertyBar()
        {
            return new PresentationPropertyBar();
        }

        public virtual OperationState ImportSlide()
        {
            EditorItem<Presentation> editorItem = getEditor().getActivePage();
            if (editorItem == null)
            {
                return OperationState.STOP;
            }

            PresentationEditorItem page = (PresentationEditorItem)editorItem;
            if (page.getPresentationForm().SlideView.Import() != OperationState.CONTINUE) return OperationState.STOP;
            page.getPresentationForm().SlideView.RemoveTempFiles();

            string nameAfterImport;
            nameAfterImport = page.getPresentationForm().SlideView.DocumentName;
            if (page.EditedObject.oid.HasValue)
            {
                if (page.EditedObject.name == page.DEFAULT_NAME)
                {
                        ChangePowerPointFileName(nameAfterImport, page, page.EditedObject);
                }
            }
            else
            {
                if (!validateName(page, nameAfterImport))
                {
                    nameAfterImport = getNewPageName(nameAfterImport);
                }
                    ChangePowerPointFileName(nameAfterImport, page, page.EditedObject);
            }
            OnChange();
            return OperationState.CONTINUE;
        }

        public virtual OperationState ExportSlide()
        {
            EditorItem<Presentation> editorItem = getPresentationEditor().getActivePage();
            if (editorItem == null) return OperationState.STOP;

            PresentationEditorItem page = (PresentationEditorItem)editorItem;
            if (page.getPresentationForm().SlideView != null)
            {
                if (page.getPresentationForm().SlideView.Export(openSaveDialog()) != OperationState.CONTINUE) return OperationState.STOP;
                page.getPresentationForm().SlideView.RemoveTempFiles();
                Save(page);
            }
            return OperationState.CONTINUE;
        }

        protected string openSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = "Export PowerPoint Presentation";
            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = ApplicationManager.Instance.DefaultPowertPointExtension.Extension; ;
            fileDialog.Filter = "PowerPoint files (*" + ApplicationManager.Instance.DefaultPowertPointExtension.Extension + ")|*" + ApplicationManager.Instance.DefaultPowertPointExtension.Extension;
            Nullable<bool> result = fileDialog.ShowDialog();

            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;

            if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return null;

            return filePath;
        }

        protected override void initializePageHandlers(Misp.Kernel.Ui.Base.EditorItem<Misp.Kernel.Domain.Presentation> page)
        {
            base.initializePageHandlers(page);
            PresentationEditorItem editorPage = (PresentationEditorItem)page;
            editorPage.getPresentationForm().PresentationPropertiesPanel.groupField.GroupService = GetPresentationService().GroupService;
            editorPage.getPresentationForm().PresentationPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getPresentationForm().PresentationPropertiesPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getPresentationForm().PresentationPropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getPresentationForm().PresentationPropertiesPanel.OnOpenAfterRun += OnOpenAfterRun;
            editorPage.getPresentationForm().PresentationPropertiesPanel.OnNewReport += OnNewReport;
            editorPage.getPresentationForm().PresentationPropertiesPanel.OnFolderNameChange += OnFolderSaveNameChange;
            editorPage.getPresentationForm().PresentationPropertiesPanel.OnInsertReport += OnInsertReport;
            editorPage.getPresentationForm().PresentationPropertiesPanel.EditReport += OnEditReport;
            if (editorPage.getPresentationForm().SlideView != null) 
            {
                editorPage.getPresentationForm().SlideView.ShapeAdded += OnShapeChanged;
                editorPage.getPresentationForm().SlideView.ShapeUpdated += OnShapeChanged;
                editorPage.getPresentationForm().SlideView.ShapeDeleted += OnShapeDeleted;
                editorPage.getPresentationForm().SlideView.Imported += OnImported;
                editorPage.getPresentationForm().SlideView.SlideFileChanged += OnSlideFileChanged;
            }
        }

        private void OnSlideFileChanged()
        {
            Change();
        }

        private void OnImported() {
            Change();
        }

        private void OnShapeDeleted(int slidePosition, String slideName, int shapePosition, String shapeValue, SlideItemType type,String shapeName)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            PresentationSlide presentationSlide = page.EditedObject.getSlide(slidePosition);
            if (presentationSlide == null) return;
            PresentationSlideItem item = presentationSlide.getShape(shapePosition);
            if (item == null) return;
            presentationSlide.DeleteShape(item);
            page.EditedObject.UpdateSlide(presentationSlide);
            Change();
        }
        
        private void OnShapeChanged(int slidePosition, String slideName, int shapePosition, String shapeValue, SlideItemType type,String shapeName)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            bool added = false;
            PresentationSlide presentationSlide = page.EditedObject.getSlide(slidePosition);
            if (presentationSlide == null)
            {
                presentationSlide = new PresentationSlide(slidePosition, slideName);
                added = true;
            }

            if (added) page.EditedObject.AddSlide(presentationSlide);
            else page.EditedObject.UpdateSlide(presentationSlide);
            
            PresentationSlideItem item = presentationSlide.getShape(shapePosition);
            added = false;
            if (item == null)
            {
                item = new PresentationSlideItem();
                added = true;
            }
            item.index = shapePosition;
            item.value = shapeValue;
            item.type = type;
            item.name = shapeName;
            if (added) presentationSlide.AddShape(item);
            else presentationSlide.UpdateShape(item);
            
            Change();
        }

        private void OnEditReport()
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            PowerPoint.Shape shape = page.getPresentationForm().SlideView.GetActiveShape();
            if (shape == null) return;
            if (shape.Type != MsoShapeType.msoEmbeddedOLEObject) return;
            //page.getPresentationForm().SlideView.CloseShapeWorkbook(shape);
            string shapeValue = shape.Name;           
            OnEditReport(shapeValue);
        }

        private void OnEditReport(string shapeValue)
        {            
            try
            {
                int reportOid;
                bool ok = int.TryParse(shapeValue, out reportOid);
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => displayReportView(reportOid,-1))); 
            }
            catch (Exception) 
            {
                
            }
        }

      

        private void OnShapeActivated(object paramActiveShape, object paramActiveSlide)
        {
            if (paramActiveShape == null || paramActiveSlide == null) return;
            if (!(paramActiveShape is Kernel.Domain.PresentationSlideItem) || !(paramActiveSlide is Kernel.Domain.PresentationSlide)) return;

             PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
             if (page == null) return;
           
             Kernel.Domain.PresentationSlide activeSlide = page.EditedObject.containsSlide(((Kernel.Domain.PresentationSlide)paramActiveSlide).position);
             if (activeSlide == null)
             {
                 activeSlide = ((Kernel.Domain.PresentationSlide)paramActiveSlide);
                 page.EditedObject.AddSlide(activeSlide);
             }
             else 
             {
                 page.EditedObject.UpdateSlide(activeSlide);
             }

             Kernel.Domain.PresentationSlideItem activeShape = activeSlide.containsShape(((Kernel.Domain.PresentationSlideItem)paramActiveShape).index);
             if (activeShape == null)
             {
                 activeShape = ((Kernel.Domain.PresentationSlideItem)paramActiveShape);
                 activeSlide.AddShape(activeShape);
             }
             else 
             {
                 activeSlide.UpdateShape((Kernel.Domain.PresentationSlideItem)paramActiveShape);
             }

            Change();
        }

        private void OnOpenAfterRun(bool OpenAfterRun)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.openPresentationAfterRun = OpenAfterRun;
        
            Change();
        }

        public void updateReportList(int? reportOid) 
        {
            if (reportOid == null) return;
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if(page == null) return;
            Kernel.Domain.Report report = (Kernel.Domain.Report)GetPresentationService().ReportService.getByOid(reportOid.Value);
            page.getPresentationForm().PresentationPropertiesPanel.updateReportList((Kernel.Domain.Report)report);
        }


        private void OnFillShapeWithText(object hasText)
        {
            if ((bool)hasText)
                Change();
        }

        private OperationState Change() 
        {
            OperationState result = OperationState.STOP;
            Action action = new Action((Action)(() =>
            {
                result = OnChange();
            }
            )
            );
            System.Windows.Application.Current.Dispatcher.Invoke(action);

            return result;
        }
      
      
        Controllable controllable { get; set; }
        BusyAction action;
        public void displayReportView(int reportoid = 0,int treeOid = -1)
        {
            if (slideDialog == null) return;
            TreeActionDialog.IsActionReportView = false;
            slideDialog.IsReportView = true;
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;

            NavigationToken token = NavigationToken.GetCreateViewToken(Reporting.Base.ReportingFunctionalitiesCode.REPORT_EDIT);
            if (reportoid > 0) token = NavigationToken.GetModifyViewToken(Reporting.Base.ReportingFunctionalitiesCode.REPORT_EDIT, reportoid);

            controllable = ApplicationManager.ControllerFactory.GetController(token.Functionality, token.ViewType, token.EditionMode);
            controllable.NavigationToken = token;
            controllable.Initialize();

            ReportEditorController controller = (ReportEditorController)controllable;

            controller.getInputTableEditor().NewPageSelected -= controller.NewPageSelectedHandler;
            controller.getInputTableEditor().OnRemoveNewPage += OnRemoveNewReportPage;
            controller.listeTotalReport = BlockPanel.listeTotalReport;
            

            TreeLoopGroup LoopGroup = ((ReportSideBar)controller.SideBar).TreeLoopGroup;
            controller.SideBar.RemoveGroup(((ReportSideBar)controller.SideBar).StatusGroup);
            controller.SideBar.RemoveGroup(0);
            if (LoopGroup != null)
            {
                LoopGroup.TransformationTreeLoopTreeview.fillTree(this.Loops);
                controller.SideBar.AddGroup(LoopGroup, 0);
            }

            controller.CustomizeMenuForTree(slideDialog.dockingManager);
            slideDialog.displaySideBar(controller.SideBar);
            slideDialog.displayPropertyBar(controller.PropertyBar);
            slideDialog.displayView(controller.View);

            controller.treeOid = treeOid;
            if (reportoid > 0)
            {
                controller.Open(reportoid);
                if (slideDialog != null) slideDialog.SaveButton.IsEnabled = false;
            }
            else
            {
                controller.Create();
                if (slideDialog != null) slideDialog.SaveButton.IsEnabled = true;
            }
            controller.ChangeEventListener += onChange;
            
        }

        private void onChange()
        {
            if (slideDialog != null) slideDialog.OnChange();
        }
        
        public void SaveReport() 
        {
            if (controllable == null) return;
            if (controllable is ReportEditorController)
            {
                ReportEditorController controller = (ReportEditorController)controllable;
                InputTableEditorItem page = (InputTableEditorItem)controller.getInputTableEditor().getActivePage();
                if (page != null)
                {
                    page.IsModify = true;
                    controller.treeOid = 0;
                    controller.GetInputTableService().SaveTableHandler += UpdateSaveTableInfo;
                    controller.Save(controller.getInputTableEditor().getActivePage());
                }
                else CloseReportWithoutSave();
            }
        }

        public bool CloseReportWithoutSave()
        {
            if (controllable != null)
            {
                foreach (InputTableEditorItem page in ((ReportEditorController)controllable).getInputTableEditor().getPages())
                {
                    page.getInputTableForm().SpreadSheet.Close();
                    page.IsModify = false;
                    page.Close();
                }
                ((ReportEditorController)controllable).GetInputTableService().SaveTableHandler -= UpdateSaveTableInfo;
                ((ReportEditorController)controllable).RemoveMenuForTree(this.slideDialog.dockingManager);
                this.removeReportView();
                this.InitialiazeMenuCommand();
            }
            return true;
        }


        protected void UpdateSaveTableInfo(SaveInfo info, Object table)
        {
            if (info == null || info.isEnd == true)
            {
                //((ReportEditorController)controllable).GetInputTableService().SaveTableHandler -= UpdateSaveTableInfo;
                CloseReportWithoutSave();
                if (table != null)
                {
                    InsertReportWithoutCopy((InputTable)table);
                    refreshInReportSidebar();
                }
            }
        }
          
        private void OnRemoveNewReportPage(bool remove = false)
        {
            
        }


        public void removeReportView()
        {
            if (slideDialog == null) return;
            slideDialog.IsReportView = false;
            controllable = null;
            Action action = new Action((Action)(() =>
            {
                slideDialog.displayPage(slideDialog.PresentationEditorController);
                slideDialog.SaveButton.IsEnabled = slideDialog.PresentationEditorController.IsModify;
            }));
            System.Windows.Application.Current.Dispatcher.Invoke(action);
        }

        private void OnInsertReport(object selectedReport)
        {
            if (selectedReport == null) return;
            if (!(selectedReport is Kernel.Domain.InputTable)) return;
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            int reportOid = ((Kernel.Domain.InputTable)selectedReport).oid.Value;
            string name = ((Kernel.Domain.InputTable)selectedReport).name + DateTime.Now.ToString().Replace(":", "_").Replace("/","_").Replace(" ","");
            var reportCopySaved = GetPresentationService().ReportService.SaveAs(reportOid, name,-1);
            if (reportCopySaved == null) return;
            InsertReportWithoutCopy(reportCopySaved);
        }

        


        private void InsertReportWithoutCopy(InputTable report)
        {
            if (report == null) return;
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            string reportFilePath = GetPresentationService().FileService.FileTransferService.downloadTable(report.name + EdrawOffice.EXCEL_EXT);
            if (reportFilePath == null) return;
            reportFilePath += report.excelFileName;
            page.getPresentationForm().SlideView.AddOrUpdateExcelShape(reportFilePath, (report).oid.Value);
            OnChange();
        }

        private void OnFolderSaveNameChange(string newFolderName)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page == null) return;
            Kernel.Util.UserPreferencesUtil.SetPowerPointSavingFolder(newFolderName);
            page.EditedObject.userSavingDir = newFolderName;
            OnChange();
        }

        private void OnNewReport()
        {
            currentTreeOid = 0;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => displayReportView(0,0))); 
        }

        public void refreshInReportSidebar(PresentationEditorItem page = null)
        {
            page = page == null ? (PresentationEditorItem)getPresentationEditor().getActivePage() : page;
            if(page == null) return;
            List<Kernel.Domain.Browser.InputTableBrowserData> browserData = GetPresentationService().ReportService.getBrowserDatasForTree();
            page.getPresentationForm().PresentationPropertiesPanel.fillReportList(browserData);
        }

        private void onGroupFieldChange()
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            string name = page.getPresentationForm().PresentationPropertiesPanel.groupField.textBox.Text;
            BGroup group = page.getPresentationForm().PresentationPropertiesPanel.groupField.Group;

            if (page.EditedObject == null) return;
            page.EditedObject.group = group;
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.updatePresentation(name, page.Title, true);
            page.getPresentationForm().PresentationPropertiesPanel.displayPresentation(page.EditedObject);
            page.EditedObject.isModified = true;
            OnChange();
        }

        private void onNameTextChange(object sender, KeyEventArgs args)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getPresentationForm().PresentationPropertiesPanel.nameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                String name = page.getPresentationForm().PresentationPropertiesPanel.nameTextBox.Text;
                Rename(name);
            }
        }

        protected override void initializeSideBarData()
        {
            List<Kernel.Domain.Browser.BrowserData> datas = this.Service.getBrowserDatas();
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.fillTree(new ObservableCollection<Kernel.Domain.Browser.BrowserData>(datas));

            ((PresentationSideBar)SideBar).MeasureGroup.InitializeMeasure();

            ((PresentationSideBar)SideBar).SpecialGroup.SpecialTreeView.Items.RemoveAt(1);

            BGroup group = GetPresentationService().GroupService.getDefaultGroup();
        }

        protected override void initializeSideBarHandlers()
        {
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.SelectionChanged += onSelectPresentationFromSidebar;
            ((PresentationSideBar)SideBar).SpecialGroup.SelectionSpecialChanged += OnSelecteSpecialFromSideBar;
            ((PresentationSideBar)SideBar).TreeLoopGroup.TransformationTreeLoopTreeview.SelectionChanged += onSelectLoopFromSidebar;
        }

        private void OnSelecteSpecialFromSideBar(object specialType)
        {
            if (specialType == null) return;
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            page.getPresentationForm().SlideView.AddOrUpdateTextShape(SlideItemType.INCREMENTAL, specialType.ToString(), 0);
        }

        private void onSelectLoopFromSidebar(object selectedLoop)
        {
            if (selectedLoop == null) return;
            if (selectedLoop is Kernel.Domain.TransformationTreeItem)
            {
                Kernel.Domain.TransformationTreeItem transformationItemLoop = (TransformationTreeItem)selectedLoop;
                PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
                page.getPresentationForm().SlideView.AddOrUpdateTextShape(SlideItemType.LOOP, transformationItemLoop.name, transformationItemLoop.oid.Value);
                OnChange();
            }
        }
        
        private void onSelectPresentationFromSidebar(object sender)
        {
            if (sender != null && sender is Presentation)
            {
                Presentation table = (Presentation)sender;
                EditorItem<Presentation> page = getPresentationEditor().getPage(table.name);
                Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
                string excelDir = getPowerPointFolder();
                string filePath = excelDir + table.name + EdrawOffice.EXCEL_EXT;
                if (page != null)
                {
                    page.fillObject();
                    getPresentationEditor().selectePage(page);
                }
                else if (table.oid != null && table.oid.HasValue && table.oid.Value > 0)
                {
                    this.Open(table.oid.Value);
                }
                else
                {
                    page = getPresentationEditor().addOrSelectPage(table);
                    initializePageHandlers(page);
                    page.Title = table.name;
                    getPresentationEditor().ListChangeHandler.AddNew(table);
                }
                PresentationEditorItem pageOpen = (PresentationEditorItem)getPresentationEditor().getActivePage();
                //pageOpen.getInputTableForm().SpreadSheet.Open(filePath, EdrawOffice.EXCEL_ID);
               /// UpdateStatusBar(null);
            }
        }

        protected override void Rename(string name)
        {
            PresentationEditorItem page = (PresentationEditorItem)getEditor().getActivePage();
            
            if (!Kernel.Util.FileUtil.isValidFileName(name)) 
            {
                MessageDisplayer.DisplayInfo("Presentation", "The slide name "+name+" is not valid");
                return;
            }
            if (validateName(page, name))
            {
                    ChangePowerPointFileName(name, page, page.EditedObject);
                    base.Rename(name);
            }
        }

      

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Save(EditorItem<Presentation> page)
        {
            if (page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                try
                {
                    Mask(true);
                    PresentationEditorItem currentPage = (PresentationEditorItem)page;
                    if (currentPage.getPresentationForm().SlideView != null)
                    {

                        String name = currentPage.getPresentationForm().PresentationPropertiesPanel.nameTextBox.Text;
                        Rename(name);
                        String savingFolder =  currentPage.getPresentationForm().PresentationPropertiesPanel.savingFolderTextBox.Text;
                        String filePath = buildPowerPointFilePath(page.EditedObject.name);
                        String tempFolder = GetPresentationService().FileService.GetFileDirs().TempPresentationFolder;
                        page.EditedObject.userSavingDir = buildPowerPointSavingFolderPath(savingFolder,page.EditedObject.oid);
                        page.EditedObject.slideFileName = Path.GetFileName(filePath);
                        page.EditedObject.slideFileExtension = Path.GetExtension(filePath);
                        filePath = tempFolder + Path.DirectorySeparatorChar + Path.GetFileName(filePath); 
                        if (currentPage.getPresentationForm().SlideView.SaveAs(filePath) != OperationState.CONTINUE)
                        {
                            MessageDisplayer.DisplayError("Unable to save " + page.EditedObject.name, "Unable to save file :\n" + filePath);
                            OnChange();
                            Mask(false);
                            return OperationState.STOP;
                        }
                        String fileName = currentPage.getPresentationForm().SlideView.DocumentName + Path.GetExtension(filePath);
                        String path = currentPage.getPresentationForm().SlideView.DocumentUrl;
                        GetPresentationService().FileService.FileTransferService.uploadPresentation(fileName, tempFolder);
                        page.EditedObject.slideFileName = fileName;            
                    }
                    
                    GetPresentationService().SavePresentationHandler += UpdateSaveInfo;
                    GetPresentationService().Save(page.EditedObject);
                }
                catch (Exception)
                {
                    DisplayError("Unable to save " + page.EditedObject.name, "Unable to save " + page.EditedObject.name + " named : " + page.EditedObject.name);
                    OnChange();
                    Mask(false);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        private void UpdateSaveInfo(SaveInfo info, object presentation)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page != null && presentation != null && presentation is Presentation)
            {
                page.EditedObject = (Presentation)presentation;
                page.displayObject();
                page.IsModify = false;
                return;
            }

            if (info == null || info.isEnd == true)
            {
                GetPresentationService().SavePresentationHandler -= UpdateSaveInfo;
                Mask(false);
                Service.FileService.SaveCurrentFile();
            }
            else
            {
                int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.stepCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.stepRuned;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
            }
        }

        protected void Mask(bool mask, string content = "Saving...")
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            if (page != null) page.getPresentationForm().Mask(mask);
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

        public override void OnPageSelected(EditorItem<Presentation> page)
        {
            if (page == null) return;
            PresentationForm form = ((PresentationEditorItem)page).getPresentationForm();
            ((PresentationPropertyBar)this.PropertyBar).PresentationLayoutAnchorable.Content = form.PresentationPropertiesPanel;
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();

            foreach (PresentationEditorItem page in getPresentationEditor().getPages())
            {
                if (page.getPresentationForm().SlideView != null)
                {
                    page.getPresentationForm().SlideView.Close();
                }
            }
            if (getPresentationEditor().NewPage != null && ((PresentationEditorItem)getPresentationEditor().NewPage).getPresentationForm().SlideView != null)
                ((PresentationEditorItem)getPresentationEditor().NewPage).getPresentationForm().SlideView.Close();
            ApplicationManager.MainWindow.StatusLabel.Content = "";
            Kernel.Util.ClipbordUtil.ClearClipboard();
        }

        protected virtual Presentation GetNewPresentation()
        {
            Presentation presentation = new Presentation();
            presentation.name = getNewPageName("Slide");
            presentation.group = GetPresentationService().GroupService.getDefaultGroup();
            return presentation;
        }

        protected override Presentation GetObjectByName(string name)
        {
            return ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.getPresentationByName(name);
        }

        protected virtual void ChangePowerPointFileName(string newName, PresentationEditorItem page, Presentation presentation)
        {
            UpdatePresentationSidebarName(newName, page.Title, false);
            page.getPresentationForm().SlideView.DocumentName = newName;
            page.getPresentationForm().SlideView.ChangeTitleBarCaption(newName);
            page.Title = newName;
            presentation.slideFileName = newName + EdrawOffice.EXCEL_EXT;
            page.getPresentationForm().PresentationPropertiesPanel.nameTextBox.Text = newName;
            presentation.name = newName;
            presentation.isModified = true;
        }

        protected virtual void UpdatePresentationSidebarName(string newName, string presentationName, bool updateGroup)
        {
            ((PresentationSideBar)SideBar).PresentationGroup.PresentationTreeView.updatePresentation(newName, presentationName, updateGroup);
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Presentation obj = GetPresentationService().getByName(name);
                if (obj == null)
                {
                    string fileName = buildPowerPointFilePath(name);
                    if (!System.IO.File.Exists(fileName)) return name;
                }
                i++;
            }
            return name;
        }

        protected virtual string buildPowerPointFilePath(string name)
        {
            PresentationEditorItem page = (PresentationEditorItem)getPresentationEditor().getActivePage();
            string powerPointDir = getPowerPointFolder();
            string filePath = powerPointDir + name + EdrawSlide.POWER_POINT_EXT;
            
            int i = 0;
            foreach (PresentationEditorItem slideItem in getPresentationEditor().getPages())
            {
                i++;
                if (slideItem != page && filePath == slideItem.EditedObject.slideFileName)
                {
                    filePath = powerPointDir + name + i + EdrawSlide.POWER_POINT_EXT;
                }
            }
            return filePath;
        }

        protected virtual string buildPowerPointSavingFolderPath(string savingFolder,int? oid)
        {
            if (string.IsNullOrEmpty(savingFolder)) return "";
            return savingFolder + (savingFolder.EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString());            
        }

        protected virtual string getPowerPointFolder()
        {
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            return fileDirs != null ? fileDirs.PresentationDir : "";
        }

        protected virtual string getReportFolder()
        {
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            return fileDirs != null ? fileDirs.InputTableDir : "";
        }


        public void customiseSlideMenu(DockingManager dockingManager)
        {
            dockingManager.DocumentContextMenu.Items.Clear();

            dockingManager.DocumentContextMenu.Items.Insert(0, SaveMenuItem);
            dockingManager.DocumentContextMenu.Items.Insert(0, SaveAsMenuItem);
            dockingManager.DocumentContextMenu.Items.Insert(0, ExportMenuItem);
            dockingManager.DocumentContextMenu.Items.Insert(0, ImportMenuItem);
            dockingManager.DocumentContextMenu.Items.Insert(0, RenameMenuItem);
            
            dockingManager.DocumentContextMenu.CommandBindings.Add(ImportCommandBinding);
            dockingManager.DocumentContextMenu.CommandBindings.Add(ExportCommandBinding);
            dockingManager.DocumentContextMenu.CommandBindings.Add(SaveCommandBinding);
            dockingManager.DocumentContextMenu.CommandBindings.Add(SaveAsCommandBinding);
            dockingManager.DocumentContextMenu.CommandBindings.Add(RenameCommandBinding);
        }
    }
}
