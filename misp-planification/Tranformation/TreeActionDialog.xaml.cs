using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Planification.Tranformation.InstructionControls;
using Misp.Reporting.Base;
using Misp.Reporting.Report;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for TreeActionDialog.xaml
    /// </summary>
    public partial class TreeActionDialog : Window
    {

        #region Events
        public event OnCloseSlideDialogEventHandler OnCloseSlideDialog;
        public event OnCloseTransformationTableDialogEventHandler OnCloseTransformationTableDialog;
        

        public delegate void OnCloseSlideDialogEventHandler();
        public delegate void OnCloseTransformationTableDialogEventHandler();

        public ChangeItemEventHandler SaveEndedEventHandler { get; set; }

        #endregion


        #region Properties

        public TransformationTreeItem Action { get; set; }

        public static bool IsActionReportView { get; set; }

        public TransformationTreeService TransformationTreeService { get; set; }
        
        public ReportEditorController ReportEditorController { get; set; }

        public List<TransformationTreeItem> loops { get; set; }


        /// <summary>
        /// The PropertyBar
        /// </summary>
        public PropertyBar PropertyBar { get; set; }

        
        #endregion


        #region Constructor

        public TreeActionDialog()
        {
            InitializeComponent();
            IsActionReportView = true;
            this.InstructionsPanel.OnCloseSlideDialog += OnCloseSlide;
            this.InstructionsPanel.OnCloseTransformationTableDialog += OnCloseTransformationTable;
            this.InstructionsPanel.ChangeEventHandler += OnChange;
            this.Owner = ApplicationManager.Instance.MainWindow;
            this.InstructionsPanel.ButtonGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void OnCloseTransformationTable()
        {
            
        }

        private void OnCloseSlide()
        {
            
        }

       
        
        #endregion


        #region Report

        public void Mask(bool mask, string content = "Action Loading...")
        {
            this.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                this.LoadingProgressBar.IsIndeterminate = true;
                this.LoadingLabel.Content = content;
                this.LoadingProgressBar.Visibility = Visibility.Visible;
                this.LoadingLabel.Visibility = Visibility.Visible;
                this.LoadingImage.Visibility = Visibility.Hidden;
            }
        }

        public void OnBusyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            BusyAction action = (BusyAction)sender;
            switch (e.PropertyName)
            {
                case "IsBusy":
                    if (action.IsBusy)
                    {
                        BusyBorder.Visibility = System.Windows.Visibility.Visible;
                        if (action.IsDeterministic)
                        {
                            LoadingProgressBar.Visibility = System.Windows.Visibility.Visible;
                            LoadingLabel.Visibility = System.Windows.Visibility.Visible;
                            LoadingImage.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            LoadingProgressBar.Visibility = System.Windows.Visibility.Hidden;
                            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
                            LoadingImage.Visibility = System.Windows.Visibility.Visible;
                            //LoadingImage.StartAnimate();
                        }
                    }
                    else
                    {
                        //LoadingImage.StopAnimate();
                        BusyBorder.Visibility = System.Windows.Visibility.Hidden;
                        action.EndWork();
                    }

                    break;

                case "LoadingPercentage":
                    LoadingProgressBar.Maximum = action.MaxLoadingPercentage;
                    LoadingProgressBar.Value = action.LoadingPercentage;
                    LoadingLabel.Content = action.LoadingStep;
                    break;
            }
        }

        public void initializeReport()
        {
            this.ReportEditorController = (ReportEditorController)ApplicationManager.Instance.ControllerFactory.GetController(ReportingFunctionalitiesCode.NEW_REPORT_FUNCTIONALITY);
            this.ReportEditorController.Initialize();
            this.ReportEditorController.CustomizeMenuForTree(this.dockingManager);
            displayPage(this.ReportEditorController);
            this.ReportEditorController.OnSelectionChange += OnSheetSelectionChange;
            this.ReportEditorController.getInputTableEditor().OnRemoveNewPage += OnRemoveNewPage;
            this.ReportEditorController.DisableSheetAdding +=ReportEditorController_DisableSheetAdding;
            this.StatusBarLabel1.Visibility = System.Windows.Visibility.Collapsed;
            this.StatusBarLabel2.Visibility = System.Windows.Visibility.Collapsed;
            this.StatusBarPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.ReportEditorController.ChangeEventListener += OnChange;
        }

        private void OnChange()
        {
            SaveButton.IsEnabled = true;
        }

        private void OnSheetSelectionChange(string statusBarText)
        {
            this.StatusLabel.Content = statusBarText;    
        }

        private void ReportEditorController_DisableSheetAdding()
        {
            
        }

        public bool CloseReportWithoutSave()
        {
            if (!IsActionReportView) return true;
            if (this.ReportEditorController == null) return true;
            foreach (InputTableEditorItem page in this.ReportEditorController.getInputTableEditor().getAllPages())
            {
                this.ReportEditorController.IsModify = false;
                page.getInputTableForm().SpreadSheet.Close();
                page.IsModify = false;
                if (page.EditedObject != null)
                {
                    Action action = () =>
                    {
                        this.ReportEditorController.GetReportService().closeTable(page.EditedObject.name);
                    };
                    System.Windows.Application.Current.Dispatcher.Invoke(action);
                }
                //page.Close();
            }            
            Kernel.Util.ClipbordUtil.ClearClipboard();
            BlockPanel.TransformationTables = null;
            BlockPanel.TransformationSlides = null;
            this.ReportEditorController.RemoveMenuForTree(this.dockingManager);
            this.ReportEditorController = null;
            return true;
        }

        public bool SaveAction() 
        {
            if (!IsActionReportView) return true;
            if (this.ReportEditorController == null) return true;
            foreach (ReportEditorItem page in this.ReportEditorController.getEditor().getAllPages())
            {
                if (page.EditedObject != null)
                {
                    Parameter parameter = new Parameter(page.EditedObject.name);
                    parameter.setTransformationTree(1);
                    InputTable table = this.ReportEditorController.GetInputTableService().parametrizeTable(parameter);
                }
            }
            this.ReportEditorController.GetInputTableService().SaveTableHandler += UpdateSaveInfo;
            Boolean isModify = this.ReportEditorController.IsModify;
            if(isModify) this.ReportEditorController.Save();
            else if (SaveEndedEventHandler != null) SaveEndedEventHandler(true);
            return true;
        }


        protected void UpdateSaveInfo(SaveInfo info, Object table)
        {
            if (!IsActionReportView) return;
            if (info == null || info.isEnd == true)
            {
                if (Action != null && table != null && table is InputTable)
                {
                    Action.reportOid = ((InputTable)table).oid;
                    if (SaveEndedEventHandler != null) SaveEndedEventHandler(info != null ? info.isEnd : true);
                }
             }
        }

        public void Dispose()
        {
            CloseReportWithoutSave();
            if (this.ReportEditorController == null) return;
            this.ReportEditorController.RemoveMenuCommands();
            if (this.ReportEditorController.getInputTableEditor().NewPage != null && ((InputTableEditorItem)this.ReportEditorController.getInputTableEditor().NewPage).getInputTableForm().SpreadSheet != null)
                ((InputTableEditorItem)this.ReportEditorController.getInputTableEditor().NewPage).getInputTableForm().SpreadSheet.Close();            
            
            foreach (InputTableEditorItem page in this.ReportEditorController.getInputTableEditor().getPages())
            {
                this.ReportEditorController.IsModify = false;
                page.getInputTableForm().SpreadSheet.Close();
                page.IsModify = false;
                page.Close();
            }
            Close();
        }

        protected void displayPage(ReportEditorController page)
        {
            if (page == null)
            {
                displaySideBar(null);
                displayView(null);
                displayPropertyBar(null);
                dockingManager.Visibility = Visibility.Collapsed;
                return;
            }

            displaySideBar(page.SideBar);
            displayView(page.View);
            displayPropertyBar(page.PropertyBar);
            
            if (page.SideBar != null)
            {
                TreeLoopGroup LoopGroup = ((ReportSideBar)page.SideBar).TreeLoopGroup;
                page.SideBar.RemoveGroup(page.SideBar.StatusGroup);
                page.SideBar.RemoveGroup(0);
                if (LoopGroup != null)
                {
                    page.SideBar.AddGroup(LoopGroup, 0);
                }
            }
            dockingManager.Visibility = Visibility.Visible;
        }

        private void displaySideBar(SideBar sideBar)
        {
            sideBarContainer.Content = null;
            sideBarContainer.CanAutoHide = false;
            sideBarContainer.CanClose = false;
            sideBarContainer.CanHide = false;
            sideBarContainer.CanFloat = false;
            sideBarContainer.Content = sideBar;
        }

        private void displayPropertyBar(PropertyBar propertyBar)
        {
            GridLength w = rigthPanel.DockWidth;
            rigthPanelGroup.Children.Clear();
            if (PropertyBar != null)
            {
                w = PropertyBar.DockWidth;
                foreach (ILayoutAnchorablePane pane in PropertyBar.Panes)
                {
                    rigthPanelGroup.Children.Remove(pane);
                }
            }
            PropertyBar = propertyBar;
            if (propertyBar != null)
            {
                foreach (ILayoutAnchorablePane pane in propertyBar.Panes)
                {
                    rigthPanelGroup.Children.Add(pane);
                }

            }
        }

        private void displayView(Object view)
        {
            dockPanel.Children.Clear();
            dockPanel.Children.Add(leftPanel);
            if (view != null && view is LayoutDocumentPane)
            {
                ((LayoutDocumentPane)view).DockWidth = new GridLength(200, GridUnitType.Star);
                rigthPanel.DockWidth = new GridLength(300);
                leftPanel.DockWidth = new GridLength(200);
                rigthPanelGroup.DockWidth = new GridLength(300);
                dockPanel.Children.Add((LayoutDocumentPane)view);
            }
            dockPanel.Children.Add(rigthPanelGroup);

        }

        #endregion

        
        #region Operations
        
        public void DisplayItem()
        {
            if (Action == null) { Reset(); return; }

            if (this.ReportEditorController == null) this.initializeReport();
            Report report = null;
            if (Action.reportOid.HasValue)
            {
                 report = (Report)this.ReportEditorController.GetReportService().getByOid(Action.reportOid.Value);
                 this.ReportEditorController.Open(report);
                 SaveButton.IsEnabled = false;
            }
            else
            {
                
                if (this.ReportEditorController.getEditor().getPages().Count == 0)
                {
                    this.ReportEditorController.listeTotalReport = new ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData>(this.ReportEditorController.GetReportService().getAllBrowserDatas());
                    this.ReportEditorController.Create();
                    this.ReportEditorController.listeTotalReport = null;
                    report = (Report)this.ReportEditorController.getInputTableEditor().getActivePage().EditedObject;
                    SaveButton.IsEnabled = true;
                }
                
                ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.DeleteExcelSheet();
                Range range = ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.GetSelectedRange();

                Cell activeCell = ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.getActiveCell();
                ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
            }
            
            BlockPanel.listeTotalReport = new ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData>();

            BlockPanel.listeTotalReport.Add(new Kernel.Domain.Browser.InputTableBrowserData() 
            {
                name = report.name,
                oid = report.oid != null ? report.oid.Value : 0,
                isReport = true
            });
            this.NameTextBox.Text = Action.name;
            if (Action.Instruction == null && !String.IsNullOrWhiteSpace(Action.conditions))
            {
                Action.Instruction = TransformationTreeService.getInstructionObject(Action.conditions);
            }
            BlockPanel.Loops = this.loops;
            
            this.InstructionsPanel.Display(Action.Instruction);           
        }


        public void RedisplayItem()
        {
            if (Action == null) return;
            if (this.ReportEditorController == null) return;
            this.NameTextBox.Text = Action.name;
            SaveButton.IsEnabled = false;
        }


        private void OnRemoveNewPage(bool remove = false)
        {
            
        }
    
        public void Reset()
        {
            this.NameTextBox.Text = "";      
        }

        public void FillItem()
        {
            if (Action == null) Action = new TransformationTreeItem(false);
            Action.name = this.NameTextBox.Text.Trim();

            Action.Instruction = this.InstructionsPanel.Fill();
            Action.conditions = TransformationTreeService.getInstructionString(Action.Instruction);
            //BlockPanel.Loops = null;
            //BlockPanel.TransformationTables = null;
            //BlockPanel.TransformationSlides = null;
        }

        #endregion
        

        #region Handlers

        public void initializeSideBarData(ObservableCollection<TransformationTreeItem> loops)
        {
            if (this.ReportEditorController != null)
            {
                TreeLoopGroup LoopGroup = ((ReportSideBar)this.ReportEditorController.SideBar).TreeLoopGroup;                
                if (LoopGroup != null) LoopGroup.TransformationTreeLoopTreeview.fillTree(loops);
            }
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        public void initializeSideBarHandlers()
        {
            
        }
        
        #endregion


    }
}
