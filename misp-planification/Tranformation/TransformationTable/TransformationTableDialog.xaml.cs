using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Planification.Base;
using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Planification.Tranformation.TransformationTable
{
    
    /// <summary>
    /// Interaction logic for TransformationTableDialog.xaml
    /// </summary>
    public partial class TransformationTableDialog : Window
    {
        #region Properties

        public Kernel.Domain.StructuredReport TransformationTable { get; set; }

        public TransformationTableController TransformationTableController { get; set; }

        #endregion

        #region Events
        public event OnTransformationTableNameChangeEventHandler OnTransformationTableNameChange;
        public delegate void OnTransformationTableNameChangeEventHandler(String newName);
        #endregion

        #region Constructor

        public TransformationTableDialog()
        {
            this.Owner = ApplicationManager.Instance.MainWindow;
            InitializeComponent();
            InitializeTransformationTable();
        }

        #endregion
        
        #region Operations

        public void DisplayItem()
        {
            if (TransformationTable == null) { Reset(); return; }
            if (TransformationTable.oid.HasValue)
            {
                this.TransformationTableController.Open(TransformationTable);
                StructuredReportEditorItem page = (StructuredReportEditorItem)this.TransformationTableController.getStructuredReportEditor().getActivePage();
                page.getStructuredReportForm().StructuredReportPropertiesPanel.checkboxAllocateEach.IsChecked = ((Kernel.Domain.TransformationTable)page.EditedObject).allocateEachLoop;
                SaveButton.IsEnabled = false;
            }
            else
            {
                if (this.TransformationTableController.getStructuredReportEditor().getPages().Count == 0)
                {
                    this.TransformationTableController.Create();
                    this.TransformationTable = this.TransformationTableController.getStructuredReportEditor().getActivePage().EditedObject;
                }
                ((StructuredReportEditorItem)this.TransformationTableController.getStructuredReportEditor().getActivePage()).getStructuredReportForm().SpreadSheet.DeleteExcelSheet();
                ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
                SaveButton.IsEnabled = true;
            }
        }

        public void Reset()
        {
            
        }

        public void FillItem()
        {
            //if (this.TransformationTable == null) this.TransformationTable = new Kernel.Domain.TransformationTable();
            //this.TransformationTable.t = this.NameTextBox.Text.Trim();
            //this.TransformationTable.TreeActionCondition = this.ConditionPanel.TreeActionCondition;
            //this.TransformationTable.conditions = this.ConditionPanel.buildCondition(this.ConditionPanel.TreeActionCondition);
        }

        #endregion
        
        #region TransformationTable


        public void InitializeTransformationTable() 
        {
            this.TransformationTableController = (TransformationTableController)ApplicationManager.Instance.ControllerFactory.GetController(PlanificationFunctionalitiesCode.NEW_TRANSFORMATION_TABLE_FUNCTIONALITY);
            this.TransformationTableController.Initialize();
            displayPage(this.TransformationTableController);
            this.TransformationTableController.getStructuredReportEditor().NewPageSelected -= this.TransformationTableController.NewPageSelectedHandler;
            
            this.TransformationTableController.ChangeEventListener += OnChange;
        }

        private void OnChange()
        {
            SaveButton.IsEnabled = true;
        }


        public void initializeSideBarData(ObservableCollection<TransformationTreeItem> loops)
        {
            if (this.TransformationTableController != null)
            {
                TreeLoopGroup LoopGroup = ((TransformationTableSideBar)this.TransformationTableController.SideBar).TreeLoopGroup;
                if (LoopGroup != null) LoopGroup.TransformationTreeLoopTreeview.fillTree(loops);
            }
        }


        private void onTableNameChange(string newName)
        {
            if (OnTransformationTableNameChange != null) OnTransformationTableNameChange(newName);
        }

        public bool CloseTableWithoutSave()
        {
            if (this.TransformationTableController == null) return true;
            foreach (StructuredReportEditorItem page in this.TransformationTableController.getStructuredReportEditor().getAllPages())
            {
                page.getStructuredReportForm().SpreadSheet.Close();
                this.TransformationTableController.IsModify = false;
                page.IsModify = false;
                page.Close();
            }
            HistoryHandler.Instance.closePage(this.TransformationTableController);
            ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
            Kernel.Util.ClipbordUtil.ClearClipboard();
            return true;
        }

        public Kernel.Domain.StructuredReport CloseTableWithSave()
        {
            if (this.TransformationTableController == null) return null;
            //this.TransformationTableController.GetTransformationTableService().SaveTableHandler += UpdateSaveInfo;
            Kernel.Domain.StructuredReport report = this.TransformationTableController.getStructuredReportEditor().getActivePage().EditedObject;
            this.TransformationTableController.Save();
            CloseTableWithoutSave();
            return report;
        }

        public Kernel.Domain.StructuredReport SaveTable()
        {
            if (this.TransformationTableController == null) return null;
            //this.TransformationTableController.GetTransformationTableService().SaveTableHandler += UpdateSaveInfo;
            Kernel.Domain.StructuredReport report = this.TransformationTableController.getStructuredReportEditor().getActivePage().EditedObject;
            this.TransformationTableController.Save();
            this.SaveButton.IsEnabled = false;
            return report;
        }

        public void Dispose()
        {
            CloseTableWithoutSave();
            //this.TransformationTableController.RemoveMenuCommands();
            if (this.TransformationTableController.getStructuredReportEditor().NewPage != null && ((StructuredReportEditorItem)this.TransformationTableController.getStructuredReportEditor().NewPage).getStructuredReportForm().SpreadSheet != null)
                ((StructuredReportEditorItem)this.TransformationTableController.getStructuredReportEditor().NewPage).getStructuredReportForm().SpreadSheet.Close();
            this.TransformationTableController = null;
            Close();
        }
        
        /// <summary>
        /// The PropertyBar
        /// </summary>
        public PropertyBar PropertyBar { get; set; }
  
        public void displaySideBar(SideBar sideBar)
        {
            sideBarContainer.Content = null;
            sideBarContainer.Content = sideBar;
            sideBarContainer.CanAutoHide = false;
            sideBarContainer.CanFloat = false;
            sideBarContainer.CanClose = false;
            sideBarContainer.CanHide = false;
        }

        public void displayPropertyBar(PropertyBar propertyBar)
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

        public void displayView(Object view)
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

        public void displayPage(Misp.Kernel.Controller.Controllable page)
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
            dockingManager.Visibility = Visibility.Visible;
        }

        #endregion

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

    }
}
