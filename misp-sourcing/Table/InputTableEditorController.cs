using DiagramDesigner;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Designer;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Group;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Util;
using Misp.Sourcing.Base;
using Misp.Sourcing.Designer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Misp.Sourcing.Table
{

    /// <summary>
    /// 
    /// </summary>
    public class InputTableEditorController : EditorController<InputTable, Misp.Kernel.Domain.Browser.InputTableBrowserData>
    {

        #region Properties
        
        //protected DialogAllocationRun allocationRunDialog;

        public static int LOOP_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

        public static RoutedCommand ImportCommand = new RoutedCommand();
        public static RoutedCommand ExportCommand = new RoutedCommand();
        public static RoutedCommand ApplyToAllCommand = new RoutedCommand();
        public static RoutedCommand RunCommand = new RoutedCommand();
        public static RoutedCommand ClearCommand = new RoutedCommand();

        public static MenuItem ImportMenuItem = BuildContextMenuItem("Import", ImportCommand);
        public static MenuItem ExportMenuItem = BuildContextMenuItem("Export", ExportCommand);
        public static CheckBox ApplyToAllMenuItem = BuildContextCheckBox("Apply To All", ApplyToAllCommand);
        public static MenuItem RunMenuItem = BuildContextMenuItem("Run", RunCommand);
        public static MenuItem ClearMenuItem = BuildContextMenuItem("Clear", ClearCommand);
     

        public CommandBinding ImportCommandBinding { get; set; }
        public CommandBinding ExportCommandBinding { get; set; }
        public CommandBinding ApplyToAllCommandBinding { get; set; }
        public CommandBinding RunCommandBinding { get; set; }
        public CommandBinding ClearCommandBinding { get; set; }

        public int prevColName { get; set; }
        public int prevRowName { get; set; }
        public string prevSheetName { get; set; }
        public int? treeOid { get; set; }
        private void ImportCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ImportCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onImportButtonClic(sender, e); }

        private void ExportCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ExportCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.toolBarHandlerBuilder.onExportButtonClic(sender, e); }

        private void RunCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void RunCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.Run(); }

        private void ClearCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ClearCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.Clear(); }

        private void ApplyToAllCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ApplyToAllCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.ApplyToAll(); }

        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) 
        {
            /*bool canExecute;
            if (sender is bool) canExecute = (bool)sender;
            else
            {
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                canExecute = getInputTableEditor().getAllPages().Count > 2;
            }
            e.CanExecute = canExecute;*/
            e.CanExecute = false;
        }

        protected static System.Windows.Controls.CheckBox BuildContextCheckBox(string header, System.Windows.Input.RoutedCommand routedCommand)
        {
            System.Windows.Controls.CheckBox menuItem = new System.Windows.Controls.CheckBox();
            menuItem.Content = header;
            menuItem.Command = routedCommand;
            return menuItem;
        }

        #endregion


        #region Events

        public event OnSelectionChangeEventHandler OnSelectionChange;
        public delegate void OnSelectionChangeEventHandler(string statusBarText);

        public event OnRemoveNewPageEventHandler OnRemoveNewPage;
        public delegate void OnRemoveNewPageEventHandler(bool remove = false);

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputTableEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        #endregion


        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public InputTableEditor getInputTableEditor()
        {
            return (InputTableEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputTables.
        /// </summary>
        /// <returns>InputTableService</returns>
        public InputTableService GetInputTableService()
        {
            
            return (InputTableService)base.Service;
            
        }

        #endregion


        #region Operations

        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>
        /// - CONTINUE si la création de la table se termine avec succès. 
        /// - STOP sinon
        /// </returns>
        public override OperationState Create()
        {
            InputTable table = GetNewInputTable();
            ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.AddInputTable(table);
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().addOrSelectPage(table);
            page.getInputTableForm().InputTableService = (InputTableService)this.Service;

            page.DEFAULT_NAME = table.name;
            String fileName = page.getInputTableForm().SpreadSheet.CreateNewExcelFile();
            if (fileName == null)
            {
                MessageDisplayer.DisplayError("Bcephal - MS Excel Error", "Unable to create excel file!");
                return OperationState.STOP;
            }
            table.excelFileName = table.name + this.ApplicationManager.DefaultExcelExtension;

            CustomizeSpreedSheet(page);
            initializePageHandlers(page);
            page.Title = table.name;
            getInputTableEditor().ListChangeHandler.AddNew(table);
            GetInputTableService().createTable(table);
            Parameter parameter = new Parameter(table.name);
            if (this.treeOid != null)
            {
                parameter.setTransformationTree(this.treeOid.Value);
                GetInputTableService().parametrizeTable(parameter);
            }
            page.getInputTableForm().EditedObject = table;
            page.getInputTableForm().displayObject();
            OnDisplayActiveCellData();
            return OperationState.CONTINUE;
        }

        
        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
        }

        /// <summary>
        /// Ouvre la table passée en parametre dans l'éditeur.
        /// </summary>
        /// <param name="table">La table à ouvrir</param>
        /// <returns>
        /// - CONTINUE si l'ouverture de la table se termine avec succès. 
        /// - STOP sinon
        /// </returns>
        public override OperationState Open(InputTable table)
        {
            string excelDir = getExcelFolder();
            string filePath = excelDir + table.name + EdrawOffice.EXCEL_EXT;
            string tempPath = GetInputTableService().FileService.FileTransferService.downloadTable(table.name + EdrawOffice.EXCEL_EXT);
            if (string.IsNullOrWhiteSpace(filePath))
            {

            }
           
            filePath = tempPath + table.name + EdrawOffice.EXCEL_EXT;

            ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.AddInputTableIfNatExist(table);
            EditorItem<InputTable> page = getEditor().addOrSelectPage(table);   
            ((InputTableEditorItem)page).getInputTableForm().SpreadSheet.Open(filePath, EdrawOffice.EXCEL_ID);
            ((InputTableEditorItem)page).getInputTableForm().InputTableService = (InputTableService)this.Service;
            UpdateStatusBar(null);
            CustomizeSpreedSheet((InputTableEditorItem)page);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(table);
            GetInputTableService().createTable(table);
            Parameter parameter = new Parameter(table.name);
            
            if (table.tranformationTreeOid == null && this.treeOid != null)
            {
                parameter.setTransformationTree(this.treeOid.Value);
                GetInputTableService().parametrizeTable(parameter);
            }
            bool isNoAllocation = false;
            //if (!isReport())
            //{
            //    ((InputTableEditorItem)page).getInputTableForm().TableCellParameterPanel.allocationPanel.FillAllocationData();
            //    CellPropertyAllocationData data = ((InputTableEditorItem)page).getInputTableForm().TableCellParameterPanel.allocationPanel.AllocationData;
            //    isNoAllocation = data.type == CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
            //}
            ((InputTableEditorItem)page).getInputTableForm().TablePropertiesPanel.displayTable(table, isNoAllocation);
            setActivationTableAction(table);
            setIsTemplateTableAction(table);
            OnDisplayActiveCellData();
            page.IsModify = false;
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// met à jour les actions de désactivation et activation des buttons et menu si la table est active ou pas
        /// </summary>
        /// <param name="table"></param>
        public void setActivationTableAction(InputTable table)
        {
            ((InputTableToolBar)this.ToolBar).RunButton.IsEnabled = table.active;
            RunMenuItem.IsEnabled = table.active;
                //RemoveMenuCommands();
                //initializeCommands();
          
        }

        /// <summary>
        /// met à jour les actions de désactivation et activation des buttons et menu si la table est un template ou pas
        /// </summary>
        /// <param name="table"></param>
        public void setIsTemplateTableAction(InputTable table)
        {
            bool isTemplate = table.template;
            ((InputTableToolBar)this.ToolBar).SaveButton.Visibility = isTemplate ? Visibility.Collapsed : Visibility.Visible;
            ((InputTableToolBar)this.ToolBar).SaveAsButton.Visibility = isTemplate ? Visibility.Visible : Visibility.Collapsed;
            if (isTemplate)
            {
                SaveMenuItem.IsEnabled = !isTemplate;
                SaveAsMenuItem.IsEnabled = isTemplate;
            }
            else
            {
                SaveMenuItem.IsEnabled = IsModify;
                SaveAsMenuItem.IsEnabled = true;
            }
            //RemoveMenuCommands();
            //initializeCommands();
        }


        /// <summary>
        /// Cette methode permet d'importer un fichier excel dans la page active..
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState ImportExcel()
        {
            EditorItem<InputTable> editorItem = getEditor().getActivePage();
            if (editorItem == null)
            {
                return OperationState.STOP;
            }

            InputTableEditorItem page = (InputTableEditorItem)editorItem;
            if (page.getInputTableForm().SpreadSheet.Import() != OperationState.CONTINUE) return OperationState.STOP;
            page.getInputTableForm().SpreadSheet.RemoveTempFiles();

            string nameAfterImport;
            nameAfterImport = page.getInputTableForm().SpreadSheet.DocumentName;
            if (page.EditedObject.oid.HasValue)
            {
                if (page.EditedObject.name == page.DEFAULT_NAME)
                {
                    if (GetInputTableService().renameTable(nameAfterImport, page.EditedObject))
                    {
                        ChangeExcelFileName(nameAfterImport, page, page.EditedObject);
                        GetInputTableService().changeTableExcelFile(page.EditedObject.name, page.EditedObject.excelFileName);
                    }
                }
            }
            else
            {               
                if (!validateName(page, nameAfterImport))
                {
                    nameAfterImport = getNewPageName(nameAfterImport);
                }
                if (GetInputTableService().renameTable(nameAfterImport, page.EditedObject))
                {
          
                    ChangeExcelFileName(nameAfterImport, page, page.EditedObject);
                    GetInputTableService().changeTableExcelFile(page.EditedObject.name, page.EditedObject.excelFileName);
                }
            }
            
            //int index = page.getInputTableForm().SpreadSheet.getActiveSheetIndex();
            //string sheetName = page.getInputTableForm().SpreadSheet.getSheetName(index);

            List<String> sheetNames = page.getInputTableForm().SpreadSheet.getSheetNames();
        
            ObservableCollection<CellProperty> cellproperties = new ObservableCollection<CellProperty>(page.EditedObject.cellPropertyListChangeHandler.Items);
            foreach (CellProperty cellproperty in cellproperties)
            {
                if (sheetNames.Contains(cellproperty.nameSheet))
                {
                    cellproperty.isValueChanged = true;
                    page.EditedObject.UpdateCellProperty(cellproperty, false);
                }
            }
            Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return OperationState.STOP;
            range.Sheet.TableName = page.EditedObject.name;
            int row = range.Items[0].Row1;
            int col = range.Items[0].Column1;
            String sheetName = range.Sheet.Name;

            if (page.groupProperty == null) page.groupProperty = new GroupProperty(new CellProperty(),range);
            page.groupProperty.isImported = true;

            GetInputTableService().getActiveCell(page.EditedObject.name, page.groupProperty, row, col, sheetName);
        
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// Après avoir exporter le fichier l'inputTable est enregistrée.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState ExportExcel()
        {
            EditorItem<InputTable> editorItem = getInputTableEditor().getActivePage();
            if (editorItem == null) return OperationState.STOP;

            InputTableEditorItem page = (InputTableEditorItem)editorItem;
            if (page.getInputTableForm().SpreadSheet != null)
            {
                if (page.getInputTableForm().SpreadSheet.Export(openSaveDialog()) != OperationState.CONTINUE) return OperationState.STOP;
                page.getInputTableForm().SpreadSheet.RemoveTempFiles();
                Save(page);
            }
            return OperationState.CONTINUE;
        }

        protected string openSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = "Export Excel Table";
            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_EXCEL;
            fileDialog.Filter = "Excel files (*" + HistoryHandler.FILE_EXTENSION_EXCEL + ")|*" + HistoryHandler.FILE_EXTENSION_EXCEL;
            Nullable<bool> result = fileDialog.ShowDialog();

            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;

            if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return null;

            return filePath;
        }


        protected bool closeEditorAfterSave = false;
        public override OperationState TryToSaveBeforeClose()
        {
            closeEditorAfterSave = false;
            if (!IsModify) { RemoveCommands(); return OperationState.CONTINUE; }
            MessageBoxResult result = Misp.Kernel.Util.MessageDisplayer.DisplayYesNoCancelQuestion("Close", "Do you want to save change before close?");
            if (result == MessageBoxResult.Cancel) return OperationState.STOP;
            if (result == MessageBoxResult.No)
            {
                RemoveCommands();
                this.IsModify = false;
                return OperationState.CONTINUE;
            }

            closeEditorAfterSave = true;
            OperationState p = Save();
            if (p == OperationState.CONTINUE) RemoveCommands();
            return p;
        }

        
        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Save(EditorItem<InputTable> page)
        {
            if (page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                InputTable table;
                if (page.EditedObject is Report) table = (Report)page.EditedObject;
                else table = page.EditedObject;
                page.EditedObject.excelFileName = page.EditedObject.excelFileName.Replace("\"", "");
                try
                {
                    Mask(true);
                    InputTableEditorItem currentPage = (InputTableEditorItem)page;
                    if (currentPage.getInputTableForm().SpreadSheet != null)
                    {
                        if (saveSpreedSheet(page) == OperationState.STOP)
                        {
                            Mask(false);
                            return OperationState.STOP;
                        }
                    }
                    if (currentPage.groupProperty != null) OnDisplayActiveCellData();
                    if(rootPeriodName != null) GetInputTableService().PeriodNameService.Save(rootPeriodName);
                    GetInputTableService().SaveTableHandler += UpdateSaveInfo;
                    GetInputTableService().Save(table);
                    //if(closeEditorAfterSave) return OperationState.STOP;
                }
                catch (Exception)
                {
                    String name = table is Report ? "Report" : "Input Table";
                    DisplayError("Unable to save " + name, "Unable to save " + name + " named : " + table.name);
                    OnChange();
                    Mask(false);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        protected OperationState saveSpreedSheet(EditorItem<InputTable> page, String fileName = null,bool saveAs = false) 
        {
            InputTableEditorItem currentPage = (InputTableEditorItem)page;
            String excelfileName = saveAs ? buildExcelFileName(page.EditedObject.excelFileName) : page.EditedObject.excelFileName;
            //page.EditedObject.excelFileName = filePath;
            String oldFilePath = currentPage.getInputTableForm().SpreadSheet.DocumentUrl;
            if (String.IsNullOrEmpty(page.EditedObject.excelFileName)) page.EditedObject.excelFileName = page.EditedObject.name + EdrawOffice.EXCEL_EXT;
            String tempFolder = GetInputTableService().FileService.GetFileDirs().TempTableFolder;
            String pathexcel = tempFolder + page.EditedObject.name + EdrawOffice.EXCEL_EXT; //excelfileName;

            if (currentPage.getInputTableForm().SpreadSheet.SaveAs(pathexcel, true) != OperationState.CONTINUE)
            {
                String name = page.EditedObject is Report ? "Report" : "Input Table";
                MessageDisplayer.DisplayError("Unable to save " + name, "Unable to save file :\n" + excelfileName);
                if (currentPage.groupProperty != null) GetInputTableService().buildCellProperty(page.EditedObject.name, currentPage.groupProperty);
                Mask(false);
                return OperationState.STOP;
            }
            String file = currentPage.getInputTableForm().SpreadSheet.DocumentName;
            String path = currentPage.getInputTableForm().SpreadSheet.DocumentUrl;
            GetInputTableService().FileService.FileTransferService.uploadTable(excelfileName, tempFolder);
            page.EditedObject.excelFileName = excelfileName;            

            if (!saveAs)
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(oldFilePath) && !oldFilePath.Equals(excelfileName) && System.IO.File.Exists(oldFilePath))
                    {
                        string bcephalFileName = System.IO.Directory.GetParent(oldFilePath).Parent.Name;
                        if (bcephalFileName.Equals(ApplicationManager.File.name)) System.IO.File.Delete(oldFilePath);
                    }
                }
                catch (Exception) { }
            }

            return OperationState.CONTINUE;
        }

      
        protected void Mask(bool mask, string content = "Saving...")
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page != null) page.getInputTableForm().Mask(mask);
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

        private void MaskDesign(bool mask, string content = "Applying design...")
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page != null) page.getInputTableForm().Mask(mask);
            ApplicationManager.MainWindow.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.MainWindow.LoadingProgressBar.IsIndeterminate = mask;
                ApplicationManager.MainWindow.LoadingLabel.Content = content;

                ApplicationManager.MainWindow.LoadingProgressBar.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingLabel.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingImage.Visibility = Visibility.Hidden;
            }
        }    

        protected void UpdateSaveInfo(SaveInfo info, Object table)
        {            
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page != null && table != null && table is InputTable)
            {
                page.EditedObject = (InputTable)table;
                page.displayObject();
                page.IsModify = false;
                if (closeEditorAfterSave)
                {
                    HistoryHandler.Instance.OnClosePage(this);
                    closeEditorAfterSave = false;
                }
                return;
            }

            if (info == null || info.isEnd == true)
            {
                GetInputTableService().SaveTableHandler -= UpdateSaveInfo;
                Mask(false);
                Service.FileService.SaveCurrentFile();
                if (nextRunActionData != null)
                {
                    if (nextRunActionData.oids.Count == 0) nextRunActionData.oids.Add(page.EditedObject.oid.Value);
                    RunNextRunActionData();
                }
            }
            else
            {
                int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.stepCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.stepRuned;
                ApplicationManager.MainWindow.LoadingLabel.Content = "Saving Table : " + rate + " %" + " (" + info.stepRuned + "/" + info.stepCount + ")";
            }
        }

        protected virtual void RunNextRunActionData()
        {
            if (nextRunActionData == null) return;
            if (nextRunActionData.clearBeforePerformAction)
            {
                GetInputTableService().ClearAllocationTableHandler += updateClearAllocationProgress;
                GetInputTableService().ClearAllocationTable(nextRunActionData);
                Mask(true, "Allocation Clearing...");
            }
            else {
                //allocationRunDialog = new DialogAllocationRun();
                //allocationRunDialog.CloseButton.IsEnabled = false;
                //allocationRunDialog.allocationTabControl.Items.Remove(allocationRunDialog.metricsTabItem);
                //allocationRunDialog.NavigationbarCellsGrid.Visibility = Visibility.Collapsed;

                GetInputTableService().RunAllocationTableHandler += updateRunProgress;
                GetInputTableService().RunAllocationTable(nextRunActionData);
                nextRunActionData = null;
                Mask(true, "Running...");
                //allocationRunDialog.ShowDialog();
            }
        }
                

        public override OperationState SaveAs(string name)
        {
            InputTableEditorItem page = (InputTableEditorItem)getEditor().getActivePage();
            if (page == null || !validateName(page, name)) return OperationState.STOP;
            try
            {
                Mask(true);
                InputTableEditorItem currentPage = (InputTableEditorItem)page;
                if (currentPage.getInputTableForm().SpreadSheet != null)
                {
                    if (saveSpreedSheet(page, name,true) == OperationState.STOP)
                    {
                        Mask(false);
                        return OperationState.STOP;
                    }
                }
                if (currentPage.groupProperty != null) OnDisplayActiveCellData();
                if (rootPeriodName != null) GetInputTableService().PeriodNameService.Save(rootPeriodName);
                GetInputTableService().SaveTableHandler += UpdateSaveAsInfo;
                GetInputTableService().SaveAs(page.EditedObject.name, name);
            }
            catch (Exception)
            {
                String type = page.EditedObject is Report ? "Report" : "Input Table";
                DisplayError("Unable to save " + type, "Unable to save " + type + " as : " + name);
                OnChange();
                Mask(false);
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        protected void UpdateSaveAsInfo(SaveInfo info, Object table)
        {            
            InputTableEditorItem page = (InputTableEditorItem) getInputTableEditor().getActivePage();
            if (page != null && table != null && table is InputTable)
            {
                GetInputTableService().closeTable(page.EditedObject.name);
                page.EditedObject = (InputTable)table;
                page.displayObject();
                page.IsModify = false;
                page.Title = page.EditedObject.name;
                ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.AddInputTableIfNatExist(page.EditedObject);
                GetInputTableService().createTable(page.EditedObject);
                AfterSave();
                return;
            }
            if (info == null || info.isEnd == true) GetInputTableService().SaveTableHandler -= UpdateSaveAsInfo;
            UpdateSaveInfo(info, table);
        }
        
        public override OperationState Delete() { return OperationState.CONTINUE; }

        protected override void Rename(string name)
        {
            InputTableEditorItem page = (InputTableEditorItem)getEditor().getActivePage();
            if (validateName(page, name))
            {
                if (GetInputTableService().renameTable(name, page.EditedObject))
                {
                    ChangeExcelFileName(name, page, page.EditedObject);
                    GetInputTableService().changeTableExcelFile(page.EditedObject.name, page.EditedObject.excelFileName);
                    base.Rename(name);
                    Parameter parameter = new Parameter(page.EditedObject.name);

                    InputTable table = null;
                    Action actionParams = () =>
                    {
                        table = GetInputTableService().parametrizeTable(parameter);
                    };

                    System.Windows.Application.Current.Dispatcher.Invoke(actionParams);
                }
                else
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Rename ", "Unable to rename table " + page.EditedObject.name + " to " + name + " !");
                }

            }
            else
            {
                String oldName = page.EditedObject.name;
                page.getInputTableForm().TablePropertiesPanel.nameTextBox.Text = oldName;
            }
        }
                
        public override bool validateName(EditorItem<InputTable> page, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Name", "Name can't be empty!");
                return false;
            }
            
            if (!base.validateName(page, name))
            {
                String objectName = page.EditedObject is Report ? "Report" : "InputTable";
                Kernel.Util.MessageDisplayer.DisplayError("Duplicate Name", "Another "+objectName+" named " + name + " already exists!");
                return false;
            }
            return true;
        }



        private void modelUpdate_OnValidate(TableSaveIssue tableSaveIssue)
        {
            
        }

        #endregion


        #region Run Allocation, Clear Allocation and Audit
        
        protected virtual bool isReport()
        {
            return false;
        }
        
        /// <summary>
        /// Run audit
        /// </summary>
        /// <returns></returns>
        public virtual void Audit()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null || !page.EditedObject.oid.HasValue || page.EditedObject.oid.Value <= 0) return;

            RunAuditDialogBox runAuditDialog = new RunAuditDialogBox(isReport());
            runAuditDialog.AuditService = GetInputTableService().AuditService;
            
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            runAuditDialog.AuditInfo = GetInputTableService().AuditService.AuditCells(range, page.EditedObject.oid.Value);

            if (runAuditDialog.AuditInfo != null) runAuditDialog.display(); 
            else MessageDisplayer.DisplayInfo("Audit cell", "there is no allocation for the selected cells");
        }


        public virtual OperationState SaveClearRun()
        {
            OperationState state = OperationState.CONTINUE;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return state;            
            nextRunActionData = null;
            TableActionData data = new TableActionData();
            if (page.EditedObject.oid.HasValue) data = new TableActionData(page.EditedObject.oid.Value, null);
            data.saveBeforePerformAction = page.IsModify;
            data.clearBeforePerformAction = page.EditedObject.oid.HasValue;
            data.name = page.EditedObject.name;
            if (((InputTableToolBar)ToolBar).ApplyToAllCheckBox.IsChecked.HasValue && !((InputTableToolBar)ToolBar).ApplyToAllCheckBox.IsChecked.Value)
            {
                Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                if (range == null) return OperationState.STOP;
                range.Sheet.TableName = page.EditedObject.name;
                data.range = range;
            }

            if (data.saveBeforePerformAction)
            {
                nextRunActionData = data;
                return Save(page);
            }
            else if (data.clearBeforePerformAction)
            {
                nextRunActionData = data;
                return Clear();
            }
            return state;
        }
        
        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        protected TableActionData nextRunActionData;
        public virtual OperationState Run()
        {
            OperationState state = OperationState.CONTINUE;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return state;            

            //allocationRunDialog = new DialogAllocationRun();
            //allocationRunDialog.CloseButton.IsEnabled = false;
            //allocationRunDialog.allocationTabControl.Items.Remove(allocationRunDialog.metricsTabItem);
            //allocationRunDialog.NavigationbarCellsGrid.Visibility = Visibility.Collapsed;

            nextRunActionData = null;
            TableActionData data = new TableActionData();
            if (page.EditedObject.oid.HasValue) data = new TableActionData(page.EditedObject.oid.Value, null);
            data.saveBeforePerformAction = page.IsModify;
            data.name = page.EditedObject.name;
            if (((InputTableToolBar)ToolBar).ApplyToAllCheckBox.IsChecked.HasValue && !((InputTableToolBar)ToolBar).ApplyToAllCheckBox.IsChecked.Value)
            {
                Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                if (range == null) return OperationState.STOP;
                range.Sheet.TableName = page.EditedObject.name;
                data.range = range;
            }

            if (data.saveBeforePerformAction)
            {
                nextRunActionData = data;
                return Save(page);
            }

            GetInputTableService().RunAllocationTableHandler += updateRunProgress;
            GetInputTableService().RunAllocationTable(data);
            Mask(true, "Running...");            
            //allocationRunDialog.ShowDialog();
            return state;
        }
        
        private void updateRunProgress(AllocationRunInfo info)
        {
            //allocationRunDialog.UpdateGrid(info);
            if (info == null || info.runEnded == true)
            {               
                //allocationRunDialog.CloseButton.IsEnabled = true;
                //allocationRunDialog.ProgressBar.Visibility = Visibility.Hidden;
                //allocationRunDialog.statusTextBlock.Text = "";
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                GetInputTableService().RunAllocationTableHandler -= updateRunProgress;
                Mask(false);
                nextRunActionData = null;
                Service.FileService.SaveCurrentFile();
            }
            else
            {
                int rate = info.totalCellCount != 0 ? (Int32)(info.runedCellCount * 100 / info.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "Loading Table : " + rate + " %" + " (" + info.runedCellCount + "/" + info.totalCellCount + ")";
            }
        }
        
        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Clear()
        {
            OperationState state = OperationState.CONTINUE;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return state;
            if (page.IsModify) state = Save(page);
            if (state == OperationState.STOP) return state;

            //this.ApplicationManager.MainWindow.UpdatePogressBar2(0, 100, "");

            TableActionData data = null;
            if (page.EditedObject.oid == null || !page.EditedObject.oid.HasValue) return state;
            if (ApplyToAllMenuItem.IsChecked.HasValue && !ApplyToAllMenuItem.IsChecked.Value)
            {
                Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                if (range == null) return OperationState.STOP;
                range.Sheet.TableName = page.EditedObject.name;
                data = new TableActionData(page.EditedObject.oid.Value, range);
            }
            else
            {
                data = new TableActionData(page.EditedObject.oid.Value, null);
            }

            GetInputTableService().ClearAllocationTableHandler += updateClearAllocationProgress;
            GetInputTableService().ClearAllocationTable(data);
            Mask(true, "Allocation Clearing...");
            return state;
        }

        private void updateClearAllocationProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                GetInputTableService().ClearAllocationTableHandler -= updateClearAllocationProgress;
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
                Mask(false);
                if (nextRunActionData != null)
                {
                    InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                    nextRunActionData.clearBeforePerformAction = false;
                    if (nextRunActionData.oids.Count == 0) nextRunActionData.oids.Add(page.EditedObject.oid.Value);
                    RunNextRunActionData();
                }
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
            }
        }

        #endregion


        #region ToolBar Handlers

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((InputTableToolBar)this.ToolBar).SaveClearRunButton.Click += OnSaveClearRunButton;
            ((InputTableToolBar)this.ToolBar).RunButton.Click += OnRun;
            ((InputTableToolBar)this.ToolBar).SaveAsButton.Click += OnSaveAs;
            ((InputTableToolBar)this.ToolBar).ClearButton.Click += OnClear;

           // ((InputTableToolBar)this.ToolBar).AuditButton.Click += OnAudit;
            ((InputTableToolBar)this.ToolBar).ApplyToAllCheckBox.Checked += OnApplyToAll;
            ((InputTableToolBar)this.ToolBar).ApplyToAllCheckBox.Unchecked += OnApplyToAll;
        }

        private void OnSaveAs(object sender, RoutedEventArgs e)
        {
            OperationState result = SaveAs();
            if (result == OperationState.STOP) return;
        }

        private void OnSaveClearRunButton(object sender, RoutedEventArgs e) 
        {
            OperationState result = SaveClearRun();
           if (result == OperationState.CONTINUE) this.AfterSave();

        }
        
        private void OnRun(object sender, RoutedEventArgs e) 
        {
           OperationState result =  Run();
           if (result == OperationState.CONTINUE) this.AfterSave();

        }

        public override OperationState Close()
        {
            return base.Close();
        }

        private void OnClear(object sender, RoutedEventArgs e) { Clear(); }

        
        private void OnApplyToAll(object sender, RoutedEventArgs e)
        { 
            ApplyToAllMenuItem.IsChecked = ((InputTableToolBar)this.ToolBar).ApplyToAllCheckBox.IsChecked; 
        }

        #endregion


        #region Page Handlers

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - TablePropertiesPanel
        /// - CellPropertiesPanel
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<InputTable> page)
        {
            base.initializePageHandlers(page);
            InputTableEditorItem editorPage = (InputTableEditorItem)page;
            if (isReport())
            {
                editorPage.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.DefaultPeriodName = defaultPeriodName;
                editorPage.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.DefaultPeriodName = defaultPeriodName;

                editorPage.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.ItemChanged += OnTablePeriodChange;
                editorPage.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.ItemDeleted += OnTablePeriodDelete;
                editorPage.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.PeriodHyperlink.RequestNavigate += OnNewPeriodName;

                editorPage.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.ItemChanged += OnCellPeriodChange;
                editorPage.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.ItemDeleted += OnCellPeriodDelete;
                editorPage.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.PeriodHyperlink.RequestNavigate += OnNewPeriodName;
            }

            editorPage.getInputTableForm().TablePropertiesPanel.groupField.GroupService = GetInputTableService().GroupService;
            editorPage.getInputTableForm().TablePropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getInputTableForm().TablePropertiesPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getInputTableForm().TablePropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getInputTableForm().TablePropertiesPanel.filterScopePanel.ItemChanged += OnFilterChange;
            editorPage.getInputTableForm().TablePropertiesPanel.filterScopePanel.ItemDeleted += OnFilterDelete;
            
            editorPage.getInputTableForm().TablePropertiesPanel.periodPanel.ItemChanged += OnTablePeriodChange;
            editorPage.getInputTableForm().TablePropertiesPanel.periodPanel.ItemDeleted += OnTablePeriodDelete;
            editorPage.getInputTableForm().TablePropertiesPanel.periodPanel.PeriodHyperlink.RequestNavigate += OnNewPeriodName;

            editorPage.getInputTableForm().TablePropertiesPanel.ResetAllCellsButton.Click += OnResetAllCells;
            
            editorPage.getInputTableForm().TablePropertiesPanel.activeCheckBox.Checked += OnTableActiveOptionChecked;
            editorPage.getInputTableForm().TablePropertiesPanel.activeCheckBox.Unchecked += OnTableActiveOptionChecked;
            editorPage.getInputTableForm().TablePropertiesPanel.templateCheckBox.Checked += OnTableTemplateOptionChecked;
            editorPage.getInputTableForm().TablePropertiesPanel.templateCheckBox.Unchecked += OnTableTemplateOptionChecked;
            editorPage.getInputTableForm().TablePropertiesPanel.visibleInShortcutCheckBox.Checked += OnTableVisibleInShortcutOptionChecked;
            editorPage.getInputTableForm().TablePropertiesPanel.visibleInShortcutCheckBox.Unchecked += OnTableVisibleInShortcutOptionChecked;
            editorPage.getInputTableForm().TableCellParameterPanel.periodPanel.ItemChanged += OnCellPeriodChange;
            editorPage.getInputTableForm().TableCellParameterPanel.periodPanel.ItemDeleted += OnCellPeriodDelete;
            editorPage.getInputTableForm().TableCellParameterPanel.periodPanel.PeriodHyperlink.RequestNavigate += OnNewPeriodName;
          

            editorPage.getInputTableForm().TablePropertiesPanel.Loaded += OnTablePropertiesPanelLoaded;
            editorPage.getInputTableForm().TableCellParameterPanel.Loaded += OnCellPropertiesPanelLoaded;

            editorPage.getInputTableForm().TableCellParameterPanel.filterScopePanel.ItemChanged += OnCellScopeChange;
            editorPage.getInputTableForm().TableCellParameterPanel.filterScopePanel.ItemDeleted += OnCellScopeDelete;
            //editorPage.getInputTableForm().TableCellParameterPanel.allocationPanel.Change += OnAllocationDataChange;
            //editorPage.getInputTableForm().TableCellParameterPanel.ForAllocationChange += OnForallocationChange;
            editorPage.getInputTableForm().TableCellParameterPanel.ResetButton.Click += OnResetCells;
            editorPage.getInputTableForm().AllocationPropertiesPanel.ForAllocationChange += OnForallocationChange;
            editorPage.getInputTableForm().AllocationPropertiesPanel.ResetButton.Click += OnResetCells;
            editorPage.getInputTableForm().TableCellParameterPanel.CellMeasurePanel.ValidateFormula += OnValidateMeasureFormula;
            editorPage.getInputTableForm().AllocationPropertiesPanel.Change += OnAllocationDataChange;
            editorPage.Closed += editorPage_Closed;
            editorPage.getInputTableForm().SpreadSheet.DisableAddingSheet += SpreadSheet_DisableAddingSheet;
            
            if (editorPage.getInputTableForm().SpreadSheet != null)
            {
                editorPage.getInputTableForm().SpreadSheet.SelectionChanged += OnSpreadSheetSelectionChanged;
                editorPage.getInputTableForm().SpreadSheet.Edited += OnSpreadSheetEdited;
                editorPage.getInputTableForm().SpreadSheet.SheetActivated += OnDisplayActiveCellData;
                editorPage.getInputTableForm().SpreadSheet.CopyBcephal += SpreadSheet_CopyBcephal;
                editorPage.getInputTableForm().SpreadSheet.PasteBcephal += SpreadSheet_PasteBcephal;
                editorPage.getInputTableForm().SpreadSheet.PartialPasteBcephal += SpreadSheet_PartialPasteBcephal;
                editorPage.getInputTableForm().SpreadSheet.OnBeforeRightClick +=SpreadSheet_OnBeforeRightClick;
                
                editorPage.getInputTableForm().SpreadSheet.AuditCell += SpreadSheet_AuditCell;
                editorPage.getInputTableForm().SpreadSheet.createDesign += SpreadSheet_CreateDesign;
            }
        }
              

        private void OnCellPropertiesPanelLoaded(object sender, RoutedEventArgs e)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            OnDisplayActiveCellData();
        }

        private void OnTablePropertiesPanelLoaded(object sender, RoutedEventArgs e)
        {
           
        }

        public virtual void SpreadSheet_DisableAddingSheet()
        {

        }

        private void OnValidateMeasureFormula(object item)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            string formula = page.getInputTableForm().TableCellParameterPanel.CellMeasurePanel.formulaTextBox.Text.Trim().ToUpper();
            if (!string.IsNullOrEmpty(formula))
            {
                if (!TagFormulaUtil.isFormula(formula) || !TagFormulaUtil.isSyntaxeFormulaCorrectly(formula))
                {
                    MessageDisplayer.DisplayWarning("Wrong measure formula", formula + " Is not a correct formula!");
                    return;
                }
                String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(formula);
                Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(formula));
                if (coord.X == coord.Y && coord.Y == -1)
                {
                    MessageDisplayer.DisplayWarning("Wrong measure formula", formula + " Is not a correct formula!");
                    return;
                }
            }            

            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
            propertyBar.ParameterLayoutAnchorable.IsActive = true;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }

            page.groupProperty.isMeasure = true;
            cellProperty = page.groupProperty.cellProperty;
            if (cellProperty.cellMeasure == null) cellProperty.cellMeasure = new CellMeasure();
            cellProperty.cellMeasure.measure = null;
            cellProperty.cellMeasure.formula = formula;
            cellProperty.cellMeasure.sheet = sheetName;
            if (TagFormulaUtil.isFormula(formula))
            {
                String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(formula);
                Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(formula));
                int sheetIndex = page.getInputTableForm().SpreadSheet.getActiveSheetIndex();
                object value = page.getInputTableForm().SpreadSheet.getValueAt((int)coord.Y, (int)coord.X, sheetName);
                String measureName = value != null ? value.ToString() : "";
                cellProperty.cellMeasure.name = measureName;
            }   

            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
             
            OnChange();
        }

       
      
        private void editorPage_Closed(object sender, EventArgs e)
        {
            if(sender is InputTableEditorItem)
            GetInputTableService().closeTable((sender as InputTableEditorItem).EditedObject.name);
        }

        private void SpreadSheet_CreateDesign(ExcelEventArg arg)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => CreateDesign()));
        }

        private void createDesign(object object_parameters)
        {
            DesignerForm designForm = new DesignerForm();
            DesignWindow designWindow = new DesignWindow();
            NavigationToken token = NavigationToken.GetCreateViewToken(Sourcing.Base.SourcingFunctionalitiesCode.DESIGN_EDIT);
            Controllable page = ApplicationManager.ControllerFactory.GetController(token.Functionality);
            page.NavigationToken = token;
            page.Initialize();
            designWindow.displayPage(page);
            page.Create();
            designWindow.InputTableEditorController = this;
            designWindow.DesignerEditorController = ((DesignerEditorController)page);
            ((DesignerEditorController)page).DesignWindow = designWindow;

            designWindow.ShowDialog();
        }

        /// <summary>
        /// open apply design view to create a new design
        /// </summary>
        public virtual void CreateDesign()
        {
            Kernel.Task.Worker worker = new Kernel.Task.Worker("Applying design...");
            worker.OnWorkWithParameter += this.createDesign;
            worker.StartWork(new object[] { });
        }

      
        private void SpreadSheet_OnBeforeRightClick()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();

            Action pasteAction = new Action((Action)(() =>
                {
                    bool activatePasteMenu = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyRange();
                    page.getInputTableForm().SpreadSheet.ActivatePasteBcephal(activatePasteMenu);
                }
            )
            );
            System.Windows.Application.Current.Dispatcher.Invoke(pasteAction);
        }

        private void SpreadSheet_AuditCell(ExcelEventArg arg)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Audit())); 
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<InputTable> page)
        {
            if (page == null) return;
            InputTableForm form = ((InputTableEditorItem)page).getInputTableForm();
            if (!isReport())
            {
                form.AllocationPropertiesPanel.AllocationForm.TransformationTreeService = GetInputTableService().TransformationTreeService;
                 ((InputTablePropertyBar)this.PropertyBar).AllocationLayoutAnchorable.Content = form.AllocationPropertiesPanel;
            }
            else
            {
                ((InputTablePropertyBar)this.PropertyBar).Pane.Children.Remove(((InputTablePropertyBar)this.PropertyBar).AllocationLayoutAnchorable);
            }

            ((InputTablePropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.TablePropertiesPanel;
            ((InputTablePropertyBar)this.PropertyBar).ParameterLayoutAnchorable.Content = form.TableCellParameterPanel;
            ((InputTablePropertyBar)this.PropertyBar).MappingLayoutAnchorable.Content = form.TableCellParameterPanel.TableCellMappingPanel;
            OnDisplayActiveCellData();
            setIsTemplateTableAction(page.EditedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosed(object sender, EventArgs args)
        {
            InputTableEditorItem removedPage = (InputTableEditorItem)sender;
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            string excelDir = getExcelFolder();
            string filePath = excelDir + removedPage.EditedObject.name + EdrawOffice.EXCEL_EXT;
            if (!removedPage.EditedObject.oid.HasValue && !System.IO.File.Exists(filePath))
            {
                ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.RemoveInputTable(removedPage.EditedObject);
            }
            removedPage.getInputTableForm().AllocationPropertiesPanel.AllocationForm.Dispose();
            removedPage.getInputTableForm().SpreadSheet.Close();
            Action action = () =>
            {
                GetInputTableService().closeTable(removedPage.EditedObject.name);
            };
            System.Windows.Application.Current.Dispatcher.Invoke(action);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosing(object sender, CancelEventArgs args)
        {
            base.OnPageClosing(sender, args);
            if (args.Cancel) return;
            InputTableEditorItem page = (InputTableEditorItem)sender;
            if (page.getInputTableForm().SpreadSheet != null && OperationState.STOP == page.getInputTableForm().SpreadSheet.Close())
            {
                try
                {
                    args.Cancel = true;
                    Kernel.Util.ClipbordUtil.ClearClipboard();
                }
                catch (Exception)
                { DisplayError("Unable to save Input Table", "Unable to save Excel file."); }
            }
        }

        #endregion


        #region Properties bar Handlers

        /// <summary>
        /// Cette méthode est exécutée lorsque le filtre de la table change.
        /// L'orsqu'on rajoute ou retire un target du le filtre
        /// </summary>
        protected void OnFilterChange(object item)
        {
            
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            if (TagFormulaUtil.isFormula(targetItem.formula) && !TagFormulaUtil.isSyntaxeFormulaCorrectly(targetItem.formula))
            {
                MessageDisplayer.DisplayWarning("Wrong Filter formula", targetItem.formula + " Is not a correct formula!");
                return;
            }

            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;

            string formula = targetItem.formula;
            if (!string.IsNullOrEmpty(formula))
            {
                if (!TagFormulaUtil.isFormula(formula) || !TagFormulaUtil.isSyntaxeFormulaCorrectly(formula))
                {
                    MessageDisplayer.DisplayWarning("Wrong filter formula", formula + " Is not a correct formula!");
                    return;
                }
            }


            if (!string.IsNullOrEmpty(formula) && TagFormulaUtil.isFormula(formula))
            {
                String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(formula);
                Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(formula));
                string sheetName = page.getInputTableForm().SpreadSheet.getActiveSheetName();
                object value = page.getInputTableForm().SpreadSheet.getValueAt((int)coord.Y, (int)coord.X, sheetName);
                String valueName = value != null ? value.ToString() : "";
                targetItem.refValueName = valueName;
                targetItem.value = null;
                targetItem.nameSheet = sheetName;
            }

            Kernel.Domain.Parameter parameter = new Parameter(page.EditedObject.name);
            parameter.isTarget = true;
            parameter.targetItem = targetItem;

            InputTable table = null;

            Action actionParams = () =>
            {
                table = GetInputTableService().parametrizeTable(parameter);
            };

            System.Windows.Application.Current.Dispatcher.Invoke(actionParams);
            if (table != null) 
            {
                if (table.filter != null) table.filter = table.correctFilter();
            }
            bool isNoAllocation = false;
            //if (!isReport())
            //{
            //    page.getInputTableForm().TableCellParameterPanel.allocationPanel.FillAllocationData();
            //    CellPropertyAllocationData data = page.getInputTableForm().TableCellParameterPanel.allocationPanel.AllocationData;
            //    isNoAllocation = data.type == CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
            //}
            page.getInputTableForm().TablePropertiesPanel.displayTable(table,isNoAllocation);
            page.EditedObject = table;
            page.EditedObject.isModified = true;
            OnChange();
        }

        protected void OnFilterDelete(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            Target scope = page.getInputTableForm().TablePropertiesPanel.filterScopePanel.Scope;
            scope.SynchronizeDeleteTargetItem(targetItem);

            Kernel.Domain.Parameter parameter = new Parameter(page.EditedObject.name);
            parameter.removeScope(targetItem);
            targetItem.isDeleted = true;

            InputTable table = null;

            Action actionParams = () =>
            {
                table = GetInputTableService().parametrizeTable(parameter);
            };

            System.Windows.Application.Current.Dispatcher.Invoke(actionParams);

            page.EditedObject.isModified = true;
            
            if(table == null) table = page.EditedObject;

            //if (table.filter != null) table.filter.SynchronizeDeleteTargetItem(targetItem); //table.filter.targetItemListChangeHandler.Items = table.filter.targetItemListChangeHandler.getItems();
            //eleminateDeletedObjects(table.filter.targetItemListChangeHandler);
            page.getInputTableForm().TablePropertiesPanel.filterScopePanel.DisplayScope(scope);

           // if (table.filter != null) table.filter = table.correctFilter();
           // page.EditedObject = table;
           // page.getInputTableForm().TablePropertiesPanel.filterScopePanel.DisplayScope(table.correctFilter());

            OnChange();
        }

        

        
        private void OnTableTemplateOptionChecked(object sender, RoutedEventArgs e)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (!page.getInputTableForm().TablePropertiesPanel.thowEvent) return;
            page.EditedObject.isModified = true;
            page.IsModify = true;
            Parameter parameter = new Parameter(page.EditedObject.name);
            InputTable table = null;
            if (sender is CheckBox)
            {
                parameter.setTemplate((sender as CheckBox).IsChecked.Value);
                table = GetInputTableService().parametrizeTable(parameter);
            }
            if (table == null) return;
            if (Save(page) == OperationState.STOP) return;
            setIsTemplateTableAction(table);
        }

        protected void OnResetAllCells(object sender, RoutedEventArgs arg)
        {
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Reset all cells", "You're about to reset all cells.\nDo You want to continue?");
            if (response != MessageBoxResult.Yes) return;

            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            page.EditedObject.isModified = true;
            Parameter parameter = new Parameter(page.EditedObject.name);
            InputTable table = null;
            parameter.isResetAllCells = true;
            table = GetInputTableService().parametrizeTable(parameter);
            if (table == null) table = page.EditedObject;
            OnDisplayActiveCellData();
        }

        private void OnTableActiveOptionChecked(object sender, RoutedEventArgs e)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            page.EditedObject.isModified = true;
            Parameter parameter = new Parameter(page.EditedObject.name);
            InputTable table = null;
            if (sender is CheckBox)
            {
                parameter.setActive((sender as CheckBox).IsChecked.Value);
                table = GetInputTableService().parametrizeTable(parameter);
            }
            if (table == null) table = page.EditedObject;
            page.getInputTableForm().TablePropertiesPanel.activeCheckBox.IsChecked = table.active;
            setActivationTableAction(table);
        }

        private void OnTableVisibleInShortcutOptionChecked(object sender, RoutedEventArgs e)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            page.EditedObject.isModified = true;
            Parameter parameter = new Parameter(page.EditedObject.name);
            InputTable table = null;
            if (sender is CheckBox)
            {
                parameter.setVisibleInShortcut((sender as CheckBox).IsChecked.Value);
                table = GetInputTableService().parametrizeTable(parameter);
            }
            if (table == null) table = page.EditedObject;
            page.getInputTableForm().TablePropertiesPanel.visibleInShortcutCheckBox.IsChecked = table.visibleInShortcut;
        }

        
        /// <summary>
        /// Cette méthode est exécutée lorsque le tag de la table change.
        /// L'orsqu'on rajoute ou retire un item du le filtre
        /// </summary>
        protected void OnTablePeriodChange(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            if (TagFormulaUtil.isFormula(periodItem.formula) && !TagFormulaUtil.isSyntaxeFormulaCorrectly(periodItem.formula))
            {
                MessageDisplayer.DisplayWarning("Wrong Period formula", periodItem.formula + " Is not a correct formula!");
                return;
            }
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            page.EditedObject.isModified = true;

            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            if (activeCell != null)
            {
                int row = activeCell.Row;
                int col = activeCell.Column;

                if (TagFormulaUtil.isFormula(periodItem.formula))
                {

                    String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(periodItem.formula);
                    String formula = TagFormulaUtil.getFormula(formulaRef, row, col, row, col);
                    periodItem.formula = formula;
                    periodItem.sheet = page.getInputTableForm().SpreadSheet.getActiveSheetName();
                    Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(periodItem.formula));
                    Object obj = GetValue((int)coord.X, (int)coord.Y);
                    if (obj == null) periodItem.value = null;
                    else if (obj is DateTime) periodItem.value = ((DateTime)obj).ToShortDateString();
                    else periodItem.value = null;
                }
                if (!String.IsNullOrEmpty(periodItem.operationNumber))
                {
                    if (TagFormulaUtil.isFormula(periodItem.operationNumber))
                    {
                        String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(periodItem.operationNumber);
                        String formula = TagFormulaUtil.getFormula(formulaRef, row, col, row, col);
                        periodItem.operationNumber = formula;
                        periodItem.sheet = page.getInputTableForm().SpreadSheet.getActiveSheetName(); ;
                        Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(periodItem.operationNumber));
                        Object obj = GetValue((int)coord.X, (int)coord.Y);

                        if (obj == null) periodItem.operationNumber = null;
                        else
                        {
                            int result;
                            bool ok = int.TryParse(obj.ToString(), out result);
                            if (!ok) periodItem.operationNumber = "";
                        }
                    }
                    else
                    {
                        int result;
                        bool ok = int.TryParse(periodItem.operationNumber, out result);
                        periodItem.operationNumber = ok ? result.ToString() : "";
                    }
                }
            }

            Kernel.Domain.Parameter parameter = new Parameter(page.EditedObject.name);
            parameter.setPeriod(periodItem, page.EditedObject.excelFileName);

            InputTable table = null;
            Action actionParams = () =>
            {
                table = GetInputTableService().parametrizeTable(parameter);
            };
            System.Windows.Application.Current.Dispatcher.Invoke(actionParams);

            page.EditedObject.isModified = true;

            if (table.period != null) table.period.itemListChangeHandler.Items = table.period.itemListChangeHandler.getItems();

            if (page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel != null) page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.DisplayPeriod(table.period, true);
            else page.getInputTableForm().TablePropertiesPanel.periodPanel.DisplayPeriod(table.period, true);

            OnChange();
        }

        protected void OnTablePeriodDelete(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();

            Kernel.Domain.Parameter parameter = new Parameter(page.EditedObject.name);
            parameter.removePeriod(periodItem);

            InputTable table = page.EditedObject;

            Action actionParams = () =>
            {
                table = GetInputTableService().parametrizeTable(parameter);
            };
            System.Windows.Application.Current.Dispatcher.Invoke(actionParams);

            //if (table.period != null) table.period.itemListChangeHandler.Items = table.period.itemListChangeHandler.getItems();
            page.EditedObject.isModified = true;
            if (page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel != null)
            {
                if (page.EditedObject.period != null)
                {
                    page.EditedObject.period.SynchronizeDeletePeriodItem(periodItem);
                }
                page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.DisplayPeriod(page.EditedObject.period, true);
            }
            else page.getInputTableForm().TablePropertiesPanel.periodPanel.DisplayPeriod(table.period, true);
            OnChange();
        }
                
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on reset un groupe de cellules.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnResetCells(object sender, RoutedEventArgs arg)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;            
            page.groupProperty = new GroupProperty(null, range);
            page.groupProperty.cellProperty.nameSheet = sheetName;
            page.groupProperty.cellProperty.column = col;
            page.groupProperty.cellProperty.row = row;
            page.groupProperty.cellProperty.name = activeCell.Name;
            page.groupProperty.isReset = true;
            page.getInputTableForm().AllocationPropertiesPanel.Display(page.groupProperty.cellProperty);
            OnChange();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnForallocationChange()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            bool? forAllocation = page.getInputTableForm().AllocationPropertiesPanel.ForAllocationCheckBox.IsChecked;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isForAllocation = true;
            cellProperty = page.groupProperty.cellProperty;
            cellProperty.IsForAllocation = forAllocation.Value;
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            OnChange();
        }



        protected void OnCellPeriodChange(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            if (TagFormulaUtil.isFormula(periodItem.formula) && !TagFormulaUtil.isSyntaxeFormulaCorrectly(periodItem.formula))
            {
                MessageDisplayer.DisplayWarning("Wrong Period formula", periodItem.formula + " Is not a correct formula!");
                return;
            }

            if (TagFormulaUtil.isFormula(periodItem.operationNumber) && !TagFormulaUtil.isSyntaxeFormulaCorrectly(periodItem.operationNumber))
            {
                MessageDisplayer.DisplayWarning("Wrong Period Number formula", periodItem.operationNumber + " Is not a correct formula!");
                return;
            }

            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isPeriod = true;
            cellProperty = page.groupProperty.cellProperty;
            if (cellProperty.period == null) cellProperty.period = new Period();
           
            PeriodItem itemUpdated = cellProperty.period.SynchronizePeriodItems(periodItem);
            if (TagFormulaUtil.isFormula(itemUpdated.formula))
            {
                String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(itemUpdated.formula);
                String formula = TagFormulaUtil.getFormula(formulaRef, row, col, row, col);
                itemUpdated.formula = formula;
                itemUpdated.sheet = sheetName;
                Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(itemUpdated.formula));
                Object obj = GetValue((int)coord.X, (int)coord.Y);
                if (obj == null) itemUpdated.value = null;
                else if (obj is DateTime) itemUpdated.value = ((DateTime)obj).ToShortDateString();
                else itemUpdated.value = null;
            }
            if (!String.IsNullOrEmpty(itemUpdated.operationNumber))
            {
                if (TagFormulaUtil.isFormula(itemUpdated.operationNumber))
                {
                    String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(itemUpdated.operationNumber);
                    String formula = TagFormulaUtil.getFormula(formulaRef, row, col, row, col);
                    itemUpdated.operationNumber = formula;
                    itemUpdated.sheet = sheetName;
                    Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(itemUpdated.operationNumber));
                    Object obj = GetValue((int)coord.X, (int)coord.Y);

                    if (obj == null) itemUpdated.operationNumber = null;
                    else
                    {
                        bool ok;
                        try
                        {
                            ok = Convert.ToInt32(obj) > 0;
                        }
                        catch (Exception) 
                        {
                            ok = false;
                        }
                        if(!ok)  itemUpdated.operationNumber = "";
                    }
                }
                else 
                {
                    int result;
                    bool ok = int.TryParse(itemUpdated.operationNumber, out result);
                    itemUpdated.operationNumber = ok ? result.ToString() : "";
                }
            }
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            OnChange();
        }

        protected void OnCellPeriodDelete(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
           // if (periodItem.name.Equals("Date")) return;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isPeriod = true;
            cellProperty = page.groupProperty.cellProperty;
            if (cellProperty.period == null) cellProperty.period = new Period();
            cellProperty.period.SynchronizeDeletePeriodItem(periodItem);
            //cellProperty.period = setDefaultPeriodItem(cellProperty.period);
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            OnChange();
        }
        

        /// <summary>
        /// Cette méthode est exécutée lorsque le AllocationData d'un groupe de cellule change.
        /// </summary>
        protected void OnAllocationDataChange()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            CellPropertyAllocationData data = page.getInputTableForm().AllocationPropertiesPanel.CellAllocationData;
            
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isCellPropertyAllocationData = true;
            cellProperty = page.groupProperty.cellProperty;
            cellProperty.cellAllocationData = data;
        
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            bool isNoAllocation = false;
            if (!isReport())
            {
                isNoAllocation = data.type == CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
                page.getInputTableForm().TablePropertiesPanel.filterScopePanel.DisplayScope(page.EditedObject.correctFilter(), isNoAllocation);
            }
            
            OnChange();
        }


        /// <summary>
        /// Cette méthode est exécutée lorsque le scope d'un groupe de cellule change.
        /// L'orsqu'on rajoute ou retire un target du scope
        /// </summary>
        protected void OnCellScopeChange(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            if (TagFormulaUtil.isFormula(targetItem.formula) && !TagFormulaUtil.isSyntaxeFormulaCorrectly(targetItem.formula))
            {
                MessageDisplayer.DisplayWarning("Wrong Scope formula", targetItem.formula + " Is not a correct formula!");
                return;
            }

            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;

            string formula = targetItem.formula;
            if (!string.IsNullOrEmpty(formula))
            {
                if (!TagFormulaUtil.isFormula(formula) || !TagFormulaUtil.isSyntaxeFormulaCorrectly(formula))
                {
                    MessageDisplayer.DisplayWarning("Wrong scope formula", formula + " Is not a correct formula!");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(formula) && TagFormulaUtil.isFormula(formula))
            {
                String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(formula);
                Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(formula));
                String valueName = null;
                if(coord.Y != -1 && coord.X != -1 ){
                    object value = page.getInputTableForm().SpreadSheet.getValueAt((int)coord.Y, (int)coord.X, sheetName);
                    valueName = value != null ? value.ToString() : "";
                }
                targetItem.refValueName = valueName;
                targetItem.nameSheet = sheetName;
                targetItem.value = null;
            }


            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;

            if (isReport()) cellProperty.cellAllocationData = null;
            else
            {
                //if (cellProperty.cellAllocationData == null)
                //{
                //    page.getInputTableForm().TableCellParameterPanel.allocationPanel.FillAllocationData();
                //    cellProperty.cellAllocationData = page.getInputTableForm().TableCellParameterPanel.allocationPanel.AllocationData;
                //}
            }
             
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isTarget = true;
            cellProperty = page.groupProperty.cellProperty;
            if(cellProperty.cellScope == null) cellProperty.cellScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            cellProperty.cellScope.SynchronizeTargetItems(targetItem);
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            OnChange();
        }

        protected void OnCellScopeDelete(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isTarget = true;
            cellProperty = page.groupProperty.cellProperty;
            if (cellProperty.cellScope == null) cellProperty.cellScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            cellProperty.cellScope.SynchronizeDeleteTargetItem(targetItem);
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            OnChange();
        }
                        
        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getInputTableForm().TablePropertiesPanel.nameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                String name = page.getInputTableForm().TablePropertiesPanel.nameTextBox.Text;
                Rename(name);
            }
        }
        
        protected Object GetValue(int colFormula, int rowFormula)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            int sheetIndex = page.getInputTableForm().SpreadSheet.getActiveSheetIndex();
            string sheetName = page.getInputTableForm().SpreadSheet.getSheetName(sheetIndex);
            if (sheetName == null) return null;
            object value = page.getInputTableForm().SpreadSheet.getValueAt(rowFormula, colFormula, sheetName);
            return value;
        }

        protected void onGroupFieldChange()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            string name = page.getInputTableForm().TablePropertiesPanel.groupField.textBox.Text;
            BGroup group = page.getInputTableForm().TablePropertiesPanel.groupField.Group;
            
            Parameter parameter = new Parameter(page.EditedObject.name);
            parameter.setGroup(group);
           InputTable table = GetInputTableService().parametrizeTable(parameter);
           if (table == null) return;
           ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.updateInputTable(name, page.Title, true);
           page.getInputTableForm().TablePropertiesPanel.displayTable(table);
           page.EditedObject.group = table.group;
           page.EditedObject.isModified = true;
        }
        
        private void OnNewPeriodName(object sender, RequestNavigateEventArgs e)
        {
            EditorItem<InputTable> page = getInputTableEditor().getActivePage();
            if (page == null) return;
            page.InitializeCustomDialog("Create Period Name");
            if (page.CustomDialog.ShowCenteredToMouse().Value)
            {
                string name = page.namePanel.NameTextBox.Text;
                if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Create Period Name", "The name can't be empty!");
                    return;
                }
                PeriodName tagName = new PeriodName(name);
                List<PeriodName> tagNames = GetInputTableService().PeriodNameService.getAll();
                if (tagNames == null) tagNames = new List<PeriodName>(0);
                if (rootPeriodName.hasPeriodName(name)) 
                {
                    Kernel.Util.MessageDisplayer.DisplayInfo("Duplicate Period name", "Item named: " + name + " already exist!");
                    return;
                }
                rootPeriodName.AddChild((PeriodName)tagName);
                rootPeriodName = GetInputTableService().PeriodNameService.Save(rootPeriodName);
                ((InputTableSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);

                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable){
                    if (((InputTableEditorItem)page).getInputTableForm().TableCellParameterPanel.reportPeriodPanel != null) ((InputTableEditorItem)page).getInputTableForm().TableCellParameterPanel.reportPeriodPanel.SetPeriodItemName(tagName.name);
                    else ((InputTableEditorItem)page).getInputTableForm().TableCellParameterPanel.periodPanel.SetPeriodItemName(tagName.name);
                }
                else{
                    if (((InputTableEditorItem)page).getInputTableForm().TablePropertiesPanel.reportPeriodPanel != null) ((InputTableEditorItem)page).getInputTableForm().TablePropertiesPanel.reportPeriodPanel.SetPeriodItemName(tagName.name);
                    else ((InputTableEditorItem)page).getInputTableForm().TablePropertiesPanel.periodPanel.SetPeriodItemName(tagName.name);
                }
            }
        }
        
        #endregion
        

        #region SideBar Initializations
        List<PeriodName> periodNames = new List<PeriodName>(0);
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {

            ((InputTableSideBar)SideBar).EntityGroup.ModelService = GetInputTableService().ModelService;
            ((InputTableSideBar)SideBar).EntityGroup.InitializeTreeViewDatas();

             List<InputTableBrowserData> datas = this.Service.getBrowserDatas();
            ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.fillTree(new ObservableCollection<InputTableBrowserData>(datas));
                        
            ((InputTableSideBar)SideBar).MeasureGroup.MeasureService = GetInputTableService().MeasureService;
            ((InputTableSideBar)SideBar).MeasureGroup.InitializeTreeViewDatas(isReport());
            
            ((InputTableSideBar)SideBar).PeriodNameGroup.PeriodNameService = GetInputTableService().PeriodNameService;
            ((InputTableSideBar)SideBar).PeriodNameGroup.InitializeTreeViewDatas();
            rootPeriodName = ((InputTableSideBar)SideBar).PeriodNameGroup.rootPeriodName;
            defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            

            List<BrowserData> designs = GetInputTableService().DesignService.getBrowserDatas();
            ((InputTableSideBar)SideBar).DesignerGroup.DesignerTreeview.fillTree(new ObservableCollection<BrowserData>(designs));

            Target targetAll = GetInputTableService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((InputTableSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            List<Target> CustomizedTargets = GetInputTableService().TargetService.getAll();
            ((InputTableSideBar)SideBar).CustomizedTargetGroup.TargetTreeview.fillTree(new ObservableCollection<Target>(CustomizedTargets));

            BGroup group = GetInputTableService().GroupService.getDefaultGroup();
        }
        
        public void refreshDesignInSideBar()
        {
            List<BrowserData> designs = GetInputTableService().DesignService.getBrowserDatas();
            ((InputTableSideBar)SideBar).DesignerGroup.DesignerTreeview.fillTree(new ObservableCollection<BrowserData>(designs));
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.SelectionChanged += onSelectInputTableFromSidebar;
            ((InputTableSideBar)SideBar).MeasureGroup.MeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            ((InputTableSideBar)SideBar).EntityGroup.OnSelectTarget += OnSelectTarget;
            //((InputTableSideBar)SideBar).EntityGroup.EntityTreeview.ExpandAttribute += OnExpandAttribute;
            //((InputTableSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
            //((InputTableSideBar)SideBar).EntityGroup.EntityTreeview.OnRightClick += onRightClickFromSidebar;
            ((InputTableSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
            ((InputTableSideBar)SideBar).PeriodNameGroup.OnSelectPeriodInterval += onSelectPeriodNameFromSidebar;
            ((InputTableSideBar)SideBar).PeriodNameGroup.OnSelectPeriodName += onSelectPeriodNameFromSidebar;

            ((InputTableSideBar)SideBar).DesignerGroup.DesignerTreeview.SelectionChanged += onSelectDesignFromSidebar;
            ((InputTableSideBar)SideBar).CustomizedTargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
            ((InputTableSideBar)SideBar).TreeLoopGroup.TransformationTreeLoopTreeview.SelectionChanged += onSelectLoopFromSidebar;
        }

        private void OnSelectTarget(Target target)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
            page.getInputTableForm().TablePropertiesPanel.filterScopePanel.ActiveItemPanel.inputTableService = this.GetInputTableService();
            if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
            {
                Range currentRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                if (currentRange == null) return;
                page.getInputTableForm().TableCellParameterPanel.filterScopePanel.SetTargetValue(target);
            }
            else
            {
                page.getInputTableForm().TablePropertiesPanel.filterScopePanel.SetTargetValue(target);
            }
        }


        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectInputTableFromSidebar(object sender)
        {
            if (sender != null && sender is InputTable)
            {
                InputTable table = (InputTable)sender;
                EditorItem<InputTable> page = getInputTableEditor().getPage(table.name);
                Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
                string excelDir = getExcelFolder();
                string filePath = excelDir + table.name + EdrawOffice.EXCEL_EXT;
                if (page != null)
                {
                    page.fillObject();
                    getInputTableEditor().selectePage(page);
                }
                else if (table.oid != null && table.oid.HasValue && table.oid.Value > 0)
                {
                    this.Open(table.oid.Value);
                }
                else
                {
                    page = getInputTableEditor().addOrSelectPage(table);
                    initializePageHandlers(page);
                    page.Title = table.name;
                    getInputTableEditor().ListChangeHandler.AddNew(table);
                }
                InputTableEditorItem pageOpen = (InputTableEditorItem)getInputTableEditor().getActivePage();
               // pageOpen.getInputTableForm().SpreadSheet.Open(filePath, EdrawOffice.EXCEL_ID);
                UpdateStatusBar(null);
            }
        }
        
        BusyAction action;
        protected static int CELLS_WITHOUT_LOADING = 250;

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une mesure sur la sidebar.
        /// Cette opération a pour but d'assigner la mesure sélectionnée 
        /// aux cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La mesure sélectionnée</param>
        /// 
        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is Measure)
            {
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                Measure measure = (Measure)sender;
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.AllocationLayoutAnchorable)
                {
                    page.getInputTableForm().AllocationPropertiesPanel.AllocationPanel.setReferenceMeasure(measure);
                    return;
                }                
                
                Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                if (range == null) return;
                string sheetName = range.Sheet.Name;
                propertyBar = (InputTablePropertyBar)this.PropertyBar;
                propertyBar.ParameterLayoutAnchorable.IsActive = true;
                Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
                int  row = activeCell.Row;
                int col = activeCell.Column;
                CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
                if (page.groupProperty == null)
                {
                    page.groupProperty = new GroupProperty(cellProperty, range);
                    page.groupProperty.cellProperty.nameSheet = sheetName; 
                    page.groupProperty.cellProperty.column = col;
                    page.groupProperty.cellProperty.row = row;
                    page.groupProperty.cellProperty.name = activeCell.Name;
                }
                
                page.groupProperty.isMeasure = true;
                cellProperty = page.groupProperty.cellProperty;
                if(cellProperty.cellMeasure == null) cellProperty.cellMeasure = new CellMeasure();
                cellProperty.cellMeasure.measure = measure;
                cellProperty.cellMeasure.formula = "";
                //if (page.groupProperty.cellProperty.cellAllocationData != null) page.groupProperty.cellProperty.cellAllocationData.outputMeasure = measure;
                page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
                OnChange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void onSelectPeriodFromSidebar(object sender)
        {
            if (sender != null && sender is PeriodInterval)
            {
                PeriodInterval period = (PeriodInterval)sender;
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
                {
                    //page.getInputTableForm().TableCellParameterPanel.periodPanel.SetPeriod(period);
                }
                else
                {
                    //page.getInputTableForm().TablePropertiesPanel.periodPanel.SetPeriod(period);
                }
            }
        }

        protected virtual void onSelectPeriodNameFromSidebar(object sender)
        {
            if (sender == null) return;
            if(sender is PeriodName)
            {
                PeriodName periodName = (PeriodName)sender;
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
                {
                    Range currentRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                    if (currentRange == null) return;
                    setCellPeriodName(page, periodName.name);
                }
                else setTablePeriodName(page, periodName.name);
            }
            else if (sender is PeriodInterval) 
            {
                PeriodInterval periodInterval = (PeriodInterval)sender;
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
                {
                    Range currentRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                    if (currentRange == null) return;
                    setCellPeriodInterval(page, periodInterval);
                }
                else setTablePeriodInterval(page, periodInterval);
            }
        }

        protected virtual void setCellPeriodName(InputTableEditorItem page, String name)
        {
            page.getInputTableForm().TableCellParameterPanel.periodPanel.SetPeriodItemName(name);
        }

        protected virtual void setCellPeriodInterval(InputTableEditorItem page, PeriodInterval interval)
        {
            page.getInputTableForm().TableCellParameterPanel.periodPanel.SetPeriodInterval(interval);
        }

        protected virtual void setTablePeriodName(InputTableEditorItem page, String name)
        {
            page.getInputTableForm().TablePropertiesPanel.periodPanel.SetPeriodItemName(name);
        }

        protected virtual void setTablePeriodInterval(InputTableEditorItem page, PeriodInterval interval)
        {
            page.getInputTableForm().TablePropertiesPanel.periodPanel.SetPeriodInterval(interval);
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
                Target target = (Target)sender;
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;                
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                page.getInputTableForm().TablePropertiesPanel.filterScopePanel.ActiveItemPanel.inputTableService = this.GetInputTableService();
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
                {
                    Range currentRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                    if (currentRange == null) return;
                    page.getInputTableForm().TableCellParameterPanel.filterScopePanel.SetTargetValue(target);
                }
                else
                {
                    page.getInputTableForm().TablePropertiesPanel.filterScopePanel.SetTargetValue(target);
                }
            }
        }

        protected void onSelectLoopFromSidebar(object sender)
        {
            if (sender != null && sender is TransformationTreeItem)
            {
                TransformationTreeItem loop = (TransformationTreeItem)sender;
                InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
                page.getInputTableForm().TablePropertiesPanel.filterScopePanel.ActiveItemPanel.inputTableService = this.GetInputTableService();
                
                if (propertyBar.Pane.SelectedContent == propertyBar.ParameterLayoutAnchorable)
                {
                    Range currentRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
                    if (currentRange == null) return;

                    if (loop.IsScope) page.getInputTableForm().TableCellParameterPanel.filterScopePanel.SetLoopValue(loop);
                    else if (loop.IsPeriod) setCellPeriodLoop(page, loop);
                }
                else
                {
                    if (loop.IsScope) page.getInputTableForm().TablePropertiesPanel.filterScopePanel.SetLoopValue(loop);
                    else if (loop.IsPeriod) setTablePeriodLoop(page, loop);
                }
            }
        }

        protected virtual void setCellPeriodLoop(InputTableEditorItem page, TransformationTreeItem loop)
        {
            page.getInputTableForm().TableCellParameterPanel.periodPanel.SetLoopValue(loop);
        }

        protected virtual void setTablePeriodLoop(InputTableEditorItem page, TransformationTreeItem loop)
        {
            page.getInputTableForm().TablePropertiesPanel.periodPanel.SetLoopValue(loop);
        }

        /// <summary>
        /// Cette methode est appelée lorsque le user selectionne un desgin sur la sidebar.
        /// On demande confirmaation et on applique le design à partier de la cellulle active.
        /// </summary>
        /// <param name="sender"></param>
        public void onSelectDesignFromSidebar(object sender)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null || sender == null) return;
            if (!(sender is Design)) return;
            MaskDesign(true);
            
            Design design = (Design)sender;
            design = GetInputTableService().DesignService.getByOid(design.oid.Value);

            canShowHeader canshowheader = new canShowHeader();
            Periodicity periodicity = GetInputTableService().PeriodicityService.getPeriodicity();

            ApplyDesignDialog applydesignDialog = new ApplyDesignDialog();
            applydesignDialog.design = design;
            applydesignDialog.periodicity = periodicity;

            applydesignDialog.ShowDialog();

            if (applydesignDialog.requestCancelDesign)
            {
                MaskDesign(false);
                MessageDisplayer.DisplayInfo("Apply Design", applydesignDialog.design + " cancelled !");
            }
            else if (applydesignDialog.requestApplyDesign)
            {

                Range currentRange = page.getInputTableForm().SpreadSheet.getActiveCellAsRange();
                if (currentRange == null) return;
                currentRange.Sheet = page.getInputTableForm().SpreadSheet.getActiveSheet();
                currentRange.Sheet.TableName = page.EditedObject.name;


                if (applydesignDialog.showHeader)
                {
                    int nbColumnsHeader = design.columns.GetLineCount();
                    int nbRowsHeader = design.rows.GetLineCount();
                    int nbRowsCentral = design.central.GetLineItemCount();

                    canshowheader.columns = (currentRange.Cells[0].Row - nbColumnsHeader) >= 1;
                    canshowheader.lines = (currentRange.Cells[0].Column - nbRowsHeader) >= 1;
                    canshowheader.central = (currentRange.Cells[0].Row - nbColumnsHeader) > 1 && (currentRange.Cells[0].Column - nbRowsHeader) > 1;

                    if ((!canshowheader.columns && nbColumnsHeader > 0) && (!canshowheader.lines && nbRowsHeader > 0))
                    {
                        MessageBoxResult resultCols;
                        resultCols = MessageDisplayer.DisplayYesNoCancelQuestion("Apply Design", "The selected cell cannot allow you to display headers.\n Do you still want to apply design without it ?");
                        if (resultCols != MessageBoxResult.Yes)
                        {
                            MaskDesign(false);
                            MessageDisplayer.DisplayInfo("Apply Design", applydesignDialog.design + " cancelled !");
                            return;
                        }
                    }
                    else if (!(!canshowheader.columns && nbColumnsHeader > 0) && (!canshowheader.lines && nbRowsHeader > 0))
                    {
                        MessageBoxResult resultRows;
                        resultRows = MessageDisplayer.DisplayYesNoCancelQuestion("Apply Design", "The selected cell cannot allow you to display headers of rows.\n Do you still want to apply design without it ?");
                        if (resultRows != MessageBoxResult.Yes)
                        {
                            MaskDesign(false);
                            MessageDisplayer.DisplayInfo("Apply Design", applydesignDialog.design + " cancelled !");
                            return;
                        }
                    }
                    else if ((!canshowheader.columns && nbColumnsHeader > 0) && !(!canshowheader.lines && nbRowsHeader > 0))
                    {
                        MessageBoxResult resultCols;
                        resultCols = MessageDisplayer.DisplayYesNoCancelQuestion("Apply Design", "The selected cell cannot allow you to display headers of colunms.\n Do you still want to apply design without it ?");
                        if (resultCols != MessageBoxResult.Yes)
                        {
                            MaskDesign(false);
                            MessageDisplayer.DisplayInfo("Apply Design", applydesignDialog.design + " cancelled !");
                            return;
                        }
                    }
                }
                else
                {
                    canshowheader.central = false;
                    canshowheader.columns = false;
                    canshowheader.lines = false;
                }
       
                
                if (GetInputTableService().applyDesign(design.oid.Value, isReport(), currentRange))
                {
                    setDesign(page, design, currentRange, periodicity, canshowheader);
                    MaskDesign(false);
                    MessageDisplayer.DisplayInfo("Apply Design", applydesignDialog.design + " successfully applied !");
                }
                else 
                {
                    MaskDesign(false);
                    MessageDisplayer.DisplayError("Apply Design", "Unable to apply " + applydesignDialog.design + " !");
                }
              
                OnDisplayActiveCellData();
                OnChange();
            }
        }

        public struct canShowHeader
        {
            public bool central;
            public bool columns;
            public bool lines;
        }

        private void applyDesign(object object_parameters)
        {
            object[] parameters = (object[])object_parameters;
            InputTableEditorItem page = (InputTableEditorItem)parameters[0];
            Design design = (Design)parameters[1];                        
            Periodicity periodicity = (Periodicity)parameters[3];
            Range currentRange = (Range)parameters[2];
            canShowHeader showHeader = (canShowHeader)parameters[4];
            ApplyDesignUtil util = new ApplyDesignUtil();
            util.apply(page, design, currentRange, periodicity, showHeader);
        }

        // Démarre un nouveau thread pour la méthode appliquerDesign()
        public void setDesign(InputTableEditorItem page, Design design, Range currentRange, Periodicity _periodicity, canShowHeader showHeader)
        {
            Kernel.Task.Worker worker = new Kernel.Task.Worker("Applying design...");
            worker.OnWorkWithParameter += this.applyDesign;
            worker.StartWork(new object[] { page, design, currentRange, _periodicity, showHeader });
        }

        #endregion


        #region Spreadsheet

        /// <summary>
        /// Cette méthode est éxécutée lorsque la selection change dans le SpreadSheet.
        /// On affiche le nom de la cellule active.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSpreadSheetSelectionChanged(Kernel.Ui.Office.ExcelEventArg args)
        {
            InputTablePropertyBar propertyBar = (InputTablePropertyBar)this.PropertyBar;
            if (propertyBar.Pane.SelectedContent != propertyBar.ParameterLayoutAnchorable)
                propertyBar.ParameterLayoutAnchorable.IsActive = true;
            OnDisplayActiveCellData();
        }

        protected void updateAdressSelection(int row, int col, string sheet){
            prevRowName = row;
            prevColName = col;
            prevSheetName = sheet;
        }

        protected void OnDisplayActiveCellData()
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;        
            Kernel.Ui.Office.Range activeCell = page.getInputTableForm().SpreadSheet.getActiveCellAsRange();
            Kernel.Ui.Office.Range activeRange = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (activeCell == null) return;
            if (activeRange == null) return;
            int row = activeCell.Items[0].Row1;
            int col = activeCell.Items[0].Column1;
            String sheetname = activeCell.Sheet.Name;
            updateAdressSelection(row, col, sheetname);
            CellProperty cellProperty = GetInputTableService().getActiveCell(page.EditedObject.name, page.groupProperty, row, col, sheetname);
            page.groupProperty = null;
            if (cellProperty == null)
            {
                cellProperty = new CellProperty(activeCell.Name, row, col, sheetname);
            }
            bool isNoAllocation = false;
            if (!isReport())
            {
            //    page.getInputTableForm().AllocationPropertiesPanel.FillAllocationData();
            //    cellProperty.cellAllocationData = page.getInputTableForm().AllocationPropertiesPanel.CellAllocationData;
            //    isNoAllocation = cellProperty.cellAllocationData.type == CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
            //
            }

            if (cellProperty.cellScope != null)
            {
                cellProperty.cellScope.targetItemListChangeHandler.Items = cellProperty.cellScope.targetItemListChangeHandler.getItems();
                foreach (TargetItem item in cellProperty.cellScope.targetItemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula))
                    {
                        Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        Object value = GetValue((int)coord.X, (int)coord.Y);
                        item.refValueName = value != null ? value.ToString() : null;
                        if(item.value != null)  item.value.name = item.refValueName;
                        if (page.isImported) item.value = null;
                    }
                }
            }

            if (cellProperty.period != null)
            {
                cellProperty.period.itemListChangeHandler.Items = cellProperty.period.itemListChangeHandler.getItems();
                foreach (PeriodItem item in cellProperty.period.itemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula))
                    {
                        Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        Object value = GetValue((int)coord.X, (int)coord.Y);
                        DateTime date;
                        item.value = value != null && DateTime.TryParse(value.ToString(), out date) ? value.ToString() : null;
                        
                        if (page.isImported) item.value = null;
                    }
                }
            }
                       
            if (cellProperty.cellMeasure != null && TagFormulaUtil.isFormula(cellProperty.cellMeasure.formula))
            {
                if (TagFormulaUtil.isFormula(cellProperty.cellMeasure.formula))
                {
                    String formulaRef = TagFormulaUtil.getFormulaWithoutEqualSign(cellProperty.cellMeasure.formula);
                    Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(cellProperty.cellMeasure.formula));
                    int sheetIndex = page.getInputTableForm().SpreadSheet.getActiveSheetIndex();
                    object value = page.getInputTableForm().SpreadSheet.getValueAt((int)coord.Y, (int)coord.X, cellProperty.nameSheet);
                    String measureName = value != null ? value.ToString() : "";
                    cellProperty.cellMeasure.name = measureName;
                    if (cellProperty.cellMeasure.measure != null) cellProperty.cellMeasure.measure.name = measureName;
                }
            }


            if (page.EditedObject.filter != null)
            {
                page.EditedObject.filter = page.EditedObject.correctFilter();
                //page.EditedObject.filter.targetItemListChangeHandler.Items = page.EditedObject.correctFilter().targetItemListChangeHandler.getItems();
                foreach (TargetItem item in page.EditedObject.filter.targetItemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula))
                    {
                        Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        Object value = GetValue((int)coord.X, (int)coord.Y);
                        item.refValueName = value != null ? value.ToString() : null;
                        if (item.value != null) item.value.name = item.refValueName;
                        if (page.isImported) item.value = null;
                    }
                }
            }

            if (page.EditedObject.period != null)
            {
                page.EditedObject.period.itemListChangeHandler.Items = page.EditedObject.period.itemListChangeHandler.getItems();
                foreach (PeriodItem item in page.EditedObject.period.itemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula))
                    {
                        Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        Object value = GetValue((int)coord.X, (int)coord.Y);
                        DateTime date;
                        item.value = value != null && DateTime.TryParse(value.ToString(), out date) ? value.ToString() : null;

                        if (page.isImported) item.value = null;
                    }
                }
            }
                        
            page.getInputTableForm().TableCellParameterPanel.Display(cellProperty);
            page.getInputTableForm().TablePropertiesPanel.displayTable(page.EditedObject,isNoAllocation);
            page.getInputTableForm().AllocationPropertiesPanel.Display(cellProperty);
            UpdateStatusBar(null);
            
        }
        PeriodName rootPeriodName { get; set; }
        PeriodName defaultPeriodName { get; set; }
      
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateStatusBar(Parameter parameter)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            String activeCellName = activeCell != null ? activeCell.Name : "";

            if (parameter == null)
            {
                parameter = new Parameter(page.EditedObject.name, range, activeCellName);
                //parameter = GetInputTableService().getTableProperties(parameter);
            }
            int bcephalTotalCellCount = parameter.cellCount;
            int cellCount = range.CellCount;
            int bcephalCellsInSelection = parameter.cellCountInRange;

            long cardinality = 0;
            CellProperty activeCellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (activeCellProperty != null && activeCellProperty.cellScope != null)
            {
                cardinality = activeCellProperty.cellScope.cardinality;
                if (cardinality < 0) cardinality = 0;
            }

            if (OnSelectionChange != null) OnSelectionChange(getStatusBarLabel((long)-1));
            else ApplicationManager.Instance.MainWindow.StatusLabel.Content = getStatusBarLabel(cardinality);
        }

        public string getStatusBarLabel(long cardinality = 0)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            if (range == null) return "";
            if (activeCell == null) return "";
            return "Selected Range: " + range.FullName
                        + "    Selection count: " + range.CellCount
                //+ "    Bcephal Cells count: " + bcephalTotalCellCount
                //+ "    Bcephal Cells in selection: " + bcephalCellsInSelection
                        + "           Active Cell: " + activeCell + (cardinality != -1 ? " Cardinality: " + cardinality : "");

        }
               

        /// <summary>
        /// Cette méthode est éxécutée lorsque édite une ou un groupe de celulles dans le SpreadSheet.
        /// On met à jour la valeur numérique dans les cellProperties correspondants.
        /// </summary>
        /// <param name="arg"></param>
        protected virtual void OnSpreadSheetEdited(Kernel.Ui.Office.ExcelEventArg arg)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            Kernel.Ui.Office.Range range = arg.Range;
            if (range == null) return;
            
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            String activeCellName = activeCell != null ? activeCell.Name : "";

            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(null, range);
            }
            page.groupProperty.isValueChanged = true;
            GetInputTableService().getActiveCell(page.EditedObject.name, page.groupProperty, row, col, range.Sheet.Name);
            page.groupProperty = null;
            Action action = () =>
            {
                OnChange();
            };
            System.Windows.Application.Current.Dispatcher.Invoke(action);                       
        }

        Point getDecalageOnPaste(Range sourceRange,Range destRange) 
        {
           int decalageCol = destRange.Cells[0].Column - sourceRange.Cells[0].Column;
           int decalageRow = destRange.Cells[0].Row - sourceRange.Cells[0].Row;
           return new Point((int)decalageCol,(int)decalageRow);
        }

        private void SpreadSheet_PasteBcephal(ExcelEventArg arg)
        {            
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            Kernel.Ui.Office.Range copiedRange = null;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => copiedRange = Kernel.Util.ClipbordUtil.getRange()));
            if (copiedRange == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.copiedRange = copiedRange;
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isPaste = true;            
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { OnDisplayActiveCellData(); OnChange(); }));
       }

        /// <summary>
        /// paste special 
        /// </summary>
        /// <param name="arg"></param>
        private void SpreadSheet_PartialPasteBcephal(ExcelEventArg arg, List<string> selections)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            Kernel.Ui.Office.Range range = page.getInputTableForm().SpreadSheet.GetSelectedRange();
            if (range == null) return;
            Kernel.Ui.Office.Range copiedRange = null;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => copiedRange = Kernel.Util.ClipbordUtil.getRange()));
            if (copiedRange == null) return;
            string sheetName = range.Sheet.Name;
            Kernel.Ui.Office.Cell activeCell = page.getInputTableForm().SpreadSheet.getActiveCell();
            int row = activeCell.Row;
            int col = activeCell.Column;
            CellProperty cellProperty = page.getInputTableForm().TableCellParameterPanel.CellProperty;
            if (page.groupProperty == null)
            {
                page.groupProperty = new GroupProperty(cellProperty, range);
                page.groupProperty.copiedRange = copiedRange;
                page.groupProperty.cellProperty.nameSheet = sheetName;
                page.groupProperty.cellProperty.column = col;
                page.groupProperty.cellProperty.row = row;
                page.groupProperty.cellProperty.name = activeCell.Name;
            }
            page.groupProperty.isPartialPaste = true;
            page.groupProperty.partialPasteSelections = selections;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { OnDisplayActiveCellData(); OnChange(); }));
        }

              
        private void SpreadSheet_CopyBcephal(ExcelEventArg arg)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return;
            arg.Range.Sheet.TableName = page.EditedObject.name;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Kernel.Util.ClipbordUtil.SetRange(arg.Range)));
        }

        #endregion


        #region Commands Handlers

        public void RemoveMenuCommands()
        {
            RemoveCommands();
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            base.RemoveCommands();
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ImportMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ExportMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RunMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ClearMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ApplyToAllMenuItem);

                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(DeleteMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveAsMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RenameMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(NewMenuItem);


                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ImportCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ExportCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ApplyToAllCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RunCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ClearCommandBinding);

                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(NewCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RenameCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveAsCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(DeleteCommandBinding);
            }
        }
        
        protected override void initializeCommands()
        {
            base.initializeCommands();
            this.ImportCommandBinding = new CommandBinding(ImportCommand, ImportCommandExecuted, ImportCommandEnabled);
            this.ExportCommandBinding = new CommandBinding(ExportCommand, ExportCommandExecuted, ExportCommandEnabled);
            this.RunCommandBinding = new CommandBinding(RunCommand, RunCommandExecuted, RunCommandEnabled);
            this.ClearCommandBinding = new CommandBinding(ClearCommand, ClearCommandExecuted, ClearCommandEnabled);
            this.ApplyToAllCommandBinding = new CommandBinding(ApplyToAllCommand, ApplyToAllCommandExecuted, ApplyToAllCommandEnabled);

            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, ClearMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, RunMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, ApplyToAllMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, ExportMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, ImportMenuItem);
           
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(ImportCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(ExportCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(ApplyToAllCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(RunCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(ClearCommandBinding);
            }
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new InputTableEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            ApplyToAllMenuItem.IsChecked = true;
            return new InputTableToolBar(); 
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new InputTableToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() 
        {
            InputTableSideBar sidebar = new InputTableSideBar();
            return sidebar; 
        }

        /// <summary>
        /// InputTablePropertyBar
        /// </summary>
        /// <returns></returns>
        protected override PropertyBar getNewPropertyBar() { return new InputTablePropertyBar(); }


        #endregion
        

        #region Utils

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
            //UpdateStatusBar(null);
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState ApplyToAll()
        {
            ((InputTableToolBar)this.ToolBar).ApplyToAllCheckBox.IsChecked = ApplyToAllMenuItem.IsChecked;
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();

            foreach (InputTableEditorItem page in getInputTableEditor().getPages())
            {
                if (page.getInputTableForm().SpreadSheet != null)
                {
                    page.getInputTableForm().SpreadSheet.Close();
                }
            }
            if (getInputTableEditor().NewPage != null && ((InputTableEditorItem)getInputTableEditor().NewPage).getInputTableForm().SpreadSheet != null)
                ((InputTableEditorItem)getInputTableEditor().NewPage).getInputTableForm().SpreadSheet.Close();
            ApplicationManager.MainWindow.StatusLabel.Content = "";
            Kernel.Util.ClipbordUtil.ClearClipboard();
            GetInputTableService().closeAllTables();
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance d'InputTable.
        /// Le nom de l'imput table est unique.
        /// L'input table créé appartient au defaultGroup et est actif.
        /// </summary>
        /// <returns>une nouvelle instance d'InputTable</returns>
        protected virtual InputTable GetNewInputTable()
        {
            InputTable table = new InputTable();
            table.name = getNewPageName("Table");
            table.active = true;
            table.visibleInShortcut = true;
            table.group = GetInputTableService().GroupService.getDefaultGroup();
            Parameter parameter = new Parameter(table.name);
            parameter.setGroup(table.group);
            GetInputTableService().parametrizeTable(parameter);
            return table;
        }

        /// <summary>
        /// Customize Spreedsheet
        /// </summary>
        /// <param name="page"></param>
        private void CustomizeSpreedSheet(InputTableEditorItem page)
        {
            page.getInputTableForm().SpreadSheet.DisableTitleBar(true);
            page.getInputTableForm().SpreadSheet.DisableFormualaBar(false);
            page.getInputTableForm().SpreadSheet.DisableToolBar(false);
            page.getInputTableForm().SpreadSheet.AddSeparatorMenu();
            //page.getInputTableForm().SpreadSheet.AddExcelMenu(EdrawOffice.PARTIAL_PASTE_BCEPHAL_LABEL);
            page.getInputTableForm().SpreadSheet.AddExcelMenu(EdrawOffice.PASTE_BCEPHAL_LABEL);
            page.getInputTableForm().SpreadSheet.AddExcelMenu(EdrawOffice.COPY_BCEPHAL_LABEL);
            page.getInputTableForm().SpreadSheet.AddSeparatorMenu();
            page.getInputTableForm().SpreadSheet.AddExcelMenu(EdrawOffice.CREATE_DESIGN_LABEL);
            page.getInputTableForm().SpreadSheet.AddExcelMenu(EdrawOffice.AUDIT_CELL_LABEL);
        }

        /// <summary>
        /// Cette méthode permet de construire le chemin vers le fichier excel
        ///</summary>
        /// <param name="table">l'input table parent du fichier excel</param>
        /// <returns>le chemin complet du fichier excel</returns>
        protected virtual string buildExcelFilePath(string name)
        {
            InputTableEditorItem page = (InputTableEditorItem)getInputTableEditor().getActivePage();
            string excelDir = "";
            string filePath = excelDir + name + EdrawOffice.EXCEL_EXT;
            string newName = name;
            int i = 0;
            foreach (InputTableEditorItem unInputTable in getInputTableEditor().getPages())
            {
                i++;
                if (unInputTable != page && filePath == unInputTable.EditedObject.excelFileName)
                {
                    filePath = excelDir + name + i + EdrawOffice.EXCEL_EXT;
                }
            }
            return filePath;
        }

        protected virtual string buildExcelFileName(String name)
        {
            return GetInputTableService().buildExcelFileName(name);
        }

        protected virtual string getExcelFolder()
        {
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            return fileDirs != null ? fileDirs.InputTableDir : "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override InputTable GetObjectByName(string name)
        {
            //return ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.getInputTableByName(name);
            return GetInputTableService().getByName(name);
        }

        protected override string getNewPageName(string prefix)
        {
            //int i = 1;
            //string name = prefix + i;
            //bool valid = false;
            //while (!valid)
            //{
            //    name = prefix + i;
            //    InputTable obj = GetObjectByName(name);
            //    if (obj == null)
            //    {
            //        string fileName = buildExcelFileName(name);
            //        if (!System.IO.File.Exists(fileName)) return name;
            //    }
            //    i++;
            //}
            return GetInputTableService().getNewTableName(prefix);
        }



        protected virtual void UpdateInputTableSidebarName(string newName, string tableName, bool updateGroup)
        {
            ((InputTableSideBar)SideBar).InputTableGroup.InputTableTreeview.updateInputTable(newName, tableName, updateGroup);
        }

        protected virtual void ChangeExcelFileName(string newName, InputTableEditorItem page, InputTable table)
        {
            UpdateInputTableSidebarName(newName, page.Title, false);
            page.getInputTableForm().SpreadSheet.DocumentName = newName;
            page.getInputTableForm().SpreadSheet.ChangeTitleBarCaption(newName);
            page.Title = newName;
            table.excelFileName = newName + EdrawOffice.EXCEL_EXT;
            page.getInputTableForm().TablePropertiesPanel.nameTextBox.Text = newName;
            table.name = newName;
            table.isModified = true;           
        }

        #endregion


        
    }
}
