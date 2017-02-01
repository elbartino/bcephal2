using System;
using System.Collections.Generic;
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

using Misp.Reporting.Base;
using Misp.Reporting.Report;
using Misp.Sourcing.Table;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;
using Xceed.Wpf.AvalonDock.Layout;
using System.Collections.ObjectModel;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for ReportPanel.xaml
    /// </summary>
    public partial class ReportPanel : Grid
    {

        #region Events

        public ChangeEventHandler ChangeEventHandler;

        public ChangeItemEventHandler SaveEndedEventHandler { get; set; }

        #endregion

        #region Properties

        public ReportEditorController ReportEditorController { get; set; }

        public List<TransformationTreeItem> loops { get; set; }

        public PropertyBar PropertyBar { get; set; }

        public TransformationTreeItem TreeItem { get; set; }

        #endregion


        #region Constructor

        public ReportPanel()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void DisplayItem()
        {            
            if (TreeItem == null) { return; }

            if (this.ReportEditorController == null) this.initializeReport();
            Report report = null;
            if (TreeItem.reportOid.HasValue)
            {
                report = (Report)this.ReportEditorController.GetReportService().getByOid(TreeItem.reportOid.Value);
                this.ReportEditorController.Open(report);
                OnChange();
            }
            else
            {
                if (this.ReportEditorController.getEditor().getPages().Count == 0)
                {
                    this.ReportEditorController.listeTotalReport = new ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData>(this.ReportEditorController.GetReportService().getAllBrowserDatas());
                    this.ReportEditorController.Create();
                    this.ReportEditorController.listeTotalReport = null;
                    report = (Report)this.ReportEditorController.getInputTableEditor().getActivePage().EditedObject;
                    OnChange();
                }

                ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.DeleteExcelSheet();
                Range range = ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.GetSelectedRange();

                Cell activeCell = ((ReportEditorItem)this.ReportEditorController.getEditor().getActivePage()).getReportForm().SpreadSheet.getActiveCell();
                ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
            }
        }


        public void RedisplayItem()
        {
            if (TreeItem == null) return;
            if (this.ReportEditorController == null) return;
        }

        #endregion

        #region Report

        public void initializeReport()
        {
            this.ReportEditorController = (ReportEditorController)ApplicationManager.Instance.ControllerFactory.GetController(ReportingFunctionalitiesCode.REPORT_EDIT, ViewType.EDITION, EditionMode.CREATE);
            this.ReportEditorController.Initialize();
            this.ReportEditorController.CustomizeMenuForTree(this.dockingManager);
            displayPage(this.ReportEditorController);
            this.ReportEditorController.OnSelectionChange += OnSheetSelectionChange;
            this.ReportEditorController.getInputTableEditor().OnRemoveNewPage += OnRemoveNewPage;
            this.ReportEditorController.DisableSheetAdding += ReportEditorController_DisableSheetAdding;
            this.StatusBarLabel1.Visibility = System.Windows.Visibility.Collapsed;
            this.StatusBarLabel2.Visibility = System.Windows.Visibility.Collapsed;
            this.StatusBarPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.ReportEditorController.ChangeEventListener += OnChange;

            initializeSideBarData(new ObservableCollection<TransformationTreeItem>(loops));
        }

        private void OnRemoveNewPage(bool remove = false)
        {

        }

        private void OnChange()
        {
            if (ChangeEventHandler != null) ChangeEventHandler();
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
            }
            Kernel.Util.ClipbordUtil.ClearClipboard();            
            this.ReportEditorController.RemoveMenuForTree(this.dockingManager);
            this.ReportEditorController = null;
            return true;
        }

        public bool SaveReport()
        {
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
            if (isModify) this.ReportEditorController.Save();
            else if (SaveEndedEventHandler != null) SaveEndedEventHandler(true);
            return true;
        }


        protected void UpdateSaveInfo(SaveInfo info, Object table)
        {
            if (info == null || info.isEnd == true)
            {
                if (TreeItem != null && table != null && table is InputTable)
                {
                    TreeItem.reportOid = ((InputTable)table).oid;
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

        public void initializeSideBarData(ObservableCollection<TransformationTreeItem> loops)
        {
            if (this.ReportEditorController != null)
            {
                TreeLoopGroup LoopGroup = ((ReportSideBar)this.ReportEditorController.SideBar).TreeLoopGroup;
                if (LoopGroup != null) LoopGroup.TransformationTreeLoopTreeview.fillTree(loops);
            }
        }

        #endregion


    }
}
