using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using Misp.Reporting.Calculated_Measure;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using Xceed.Wpf.AvalonDock;
using System.Windows.Input;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Reporting.Report
{
    public class ReportEditorController : InputTableEditorController
    {

        public event DisableSheetAddingEventHandler DisableSheetAdding;
        public delegate void DisableSheetAddingEventHandler();

        public bool throwSheetWritting = true;

        public ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData> listeTotalReport { get; set; }

        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            /*bool canExecute;
            if (sender is bool) canExecute = (bool)sender;
            else
            {
                ReportEditorItem page = (ReportEditorItem)getInputTableEditor().getActivePage();
                if (page == null) return;
                if (getInputTableEditor().newPageEventHandler == null) canExecute = false;
                else canExecute = getInputTableEditor().getAllPages().Count > 2;
            }
            e.CanExecute = canExecute;*/
            e.CanExecute = false;
        }

        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de ReportEditorController.
        /// </summary>
        public ReportEditorController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.REPORT;
        }

        #endregion

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.REPORT;
        }

        #region Others

        protected override InputTable GetNewInputTable()
        {
            Kernel.Domain.Report report = new Kernel.Domain.Report();
            report.name = getNewPageName("Report");
            report.active = true;
            report.visibleInShortcut = true;
            report.group = GetInputTableService().GroupService.getDefaultGroup();
           
            return report;
        }

        protected override void UpdateInputTableSidebarName(string newName, string tableName, bool updateGroup)
        {
            ((ReportSideBar)SideBar).ReportGroup.InputTableTreeview.updateInputTable(newName, tableName, updateGroup);
        }

        protected override InputTable GetObjectByName(string name)
        {
            return ((ReportSideBar)SideBar).ReportGroup.InputTableTreeview.getInputTableByName(name,this.listeTotalReport);
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            InputTableToolBar toolBar = new InputTableToolBar();
            toolBar.RunButton.Content = "Run";
            toolBar.RunButton.ToolTip = "Run Report";
            toolBar.SaveButton.ToolTip = "Save Report";
            toolBar.CloseButton.ToolTip = "Exit Report Editor";

            toolBar.Children.Remove(toolBar.ClearButton);
            toolBar.Children.Remove(toolBar.SaveClearRunButton);
            toolBar.Children.Remove(toolBar.ApplyToAllCheckBox);
            return toolBar; 
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new ReportEditor(this.SubjectType, this.FunctionalityCode); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new ReportSideBar(); }

        protected override PropertyBar getNewPropertyBar() 
        {
            InputTablePropertyBar bar = new InputTablePropertyBar();
            bar.TableLayoutAnchorable.Title = "Report Properties";
            bar.CellLayoutAnchorable.Title = "Cell Properties";
            return bar; 
        }

        #endregion

        protected override bool isReport()
        {
            return true;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputTables.
        /// </summary>
        /// <returns>ReportService</returns>
        public ReportService GetReportService()
        {
            return (ReportService)base.Service;
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
        public override OperationState Run()
        {
            OperationState state = OperationState.CONTINUE;
            ReportEditorItem page = (ReportEditorItem)getInputTableEditor().getActivePage();
            if (page == null) return state;
            if (page.EditedObject.template)
            {
                SaveAs(page.EditedObject.name + "temp01");
            }
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

            GetReportService().RunAllocationTableHandler += updateRunProgress;
            Mask(true, "Running...");
            GetReportService().RunAllocationTable(data);
            return state;
        }

        protected override void RunNextRunActionData()
        {
            if (nextRunActionData != null)
            {
                GetReportService().RunAllocationTableHandler += updateRunProgress;
                Mask(true, "Running...");
                GetReportService().RunAllocationTable(nextRunActionData);
                nextRunActionData = null;
            }
        }

        private void updateRunProgress(AllocationRunInfo info)
        {
            ReportEditorItem page = (ReportEditorItem)getInputTableEditor().getActivePage();

            if (info != null)
            {
                throwSheetWritting = false;
                page.getInputTableForm().SpreadSheet.ThrowEvent = false;
                foreach (CellAllocationRunInfoBrowserData data in info.infos)
                {
                    page.getReportForm().SpreadSheet.SetValueAt(data.row, data.column,
                        data.sheet, data.loadedAmount);
                }
                page.getInputTableForm().SpreadSheet.ThrowEvent = true;
                throwSheetWritting = true;
            }

            if (info == null || info.runEnded == true)
            {                
                GetReportService().RunAllocationTableHandler -= updateRunProgress;
                page.IsModify = true;
                nextRunActionData = null;
                OnChange();
                Mask(false);
            }
            else
            {
                int rate = info.totalCellCount != 0 ? (Int32)(info.runedCellCount * 100 / info.totalCellCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "Running Report : " + rate + " %" + " (" + info.runedCellCount + "/" + info.totalCellCount + ")";
                
            }
        }
        
        
        protected override void initializeCommands()
        {
            base.initializeCommands();            
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ClearMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(ApplyToAllMenuItem);                
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(ClearCommandBinding);
            }
        }
        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            base.initializeSideBarData();            
            List<Kernel.Domain.CalculatedMeasure> CalculatedMeasures = GetReportService().calculatedMeasureService.getAllCalculatedMeasure();
            if (CalculatedMeasures != null)
                ((ReportSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.fillTree(new ObservableCollection<CalculatedMeasure>(CalculatedMeasures));
        }

          /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            base.initializeSideBarHandlers();
            ((ReportSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.SelectionChanged += base.onSelectMeasureFromSidebar;
        }
        

        /// <summary>
        /// Cette méthode est éxécutée lorsque édite une ou un groupe de celulles dans le SpreadSheet.
        /// On met à jour la valeur numérique dans les cellProperties correspondants.
        /// </summary>
        /// <param name="arg"></param>
        protected override void OnSpreadSheetEdited(Kernel.Ui.Office.ExcelEventArg arg)
        {
            if (throwSheetWritting) base.OnSpreadSheetEdited(arg);
            else OnChange();
        }

        public override void SpreadSheet_DisableAddingSheet()
        {
            base.SpreadSheet_DisableAddingSheet();
        }
        
        protected override void setCellPeriodName(InputTableEditorItem page, String name)
        {
            page.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.SetPeriodItemName(name);
        }

        protected override void setCellPeriodInterval(InputTableEditorItem page, PeriodInterval interval)
        {
            page.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.SetPeriodInterval(interval);			
        }

        protected override void setTablePeriodName(InputTableEditorItem page, String name)
        {
            page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.SetPeriodItemName(name);
        }

        protected override void setTablePeriodInterval(InputTableEditorItem page, PeriodInterval interval)
        {
            page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.SetPeriodInterval(interval);
        }

        protected override void setCellPeriodLoop(InputTableEditorItem page, TransformationTreeItem loop)
        {
            page.getInputTableForm().TableCellParameterPanel.reportPeriodPanel.SetLoopValue(loop);
        }

        protected override void setTablePeriodLoop(InputTableEditorItem page, TransformationTreeItem loop)
        {
            page.getInputTableForm().TablePropertiesPanel.reportPeriodPanel.SetLoopValue(loop);
        }

        public void CustomizeMenuForTree(DockingManager dockingManager)
        {           
            dockingManager.DocumentContextMenu.Items.Clear();
            dockingManager.DocumentContextMenu.CommandBindings.Clear();

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

            InputTablePropertyBar bar = (InputTablePropertyBar)this.PropertyBar;
            bar.Pane.Children.Remove(bar.AdministratorLayoutAnchorable);
        }

        public void RemoveMenuForTree(DockingManager dockingManager)
        {
            dockingManager.DocumentContextMenu.Items.Clear();
            dockingManager.DocumentContextMenu.CommandBindings.Clear();
        }


    }
}
