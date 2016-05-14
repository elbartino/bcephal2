using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Planification.Base;
using Misp.Planification.PresentationView;
using Misp.Reporting.Report;
using Misp.Sourcing.Table;
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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for TransformationSlideDialog.xaml
    /// </summary>
    public partial class TransformationSlideDialog : Window
    {
        #region Properties

        public Kernel.Domain.Presentation TransformationSlide { get; set; }

        public PresentationEditorController PresentationEditorController { get; set; }

        public bool IsReportView { get; set; }

        #endregion

        #region Events
        public event OnTransformationSlideNameChangeEventHandler OnTransformationSlideNameChange;
        public delegate void OnTransformationSlideNameChangeEventHandler(String newName);
        #endregion

        #region Constructor

        public TransformationSlideDialog()
        {
            InitializeComponent();
            InitializeTransformationSlide();
        }
        
        #endregion


        #region Initializations

        public void InitializeTransformationSlide()
        {
            this.PresentationEditorController = (PresentationEditorController)ApplicationManager.Instance.ControllerFactory.GetController(PlanificationFunctionalitiesCode.NEW_SLIDE_FUNCTIONALITY);
            this.PresentationEditorController.slideDialog = this;
            this.PresentationEditorController.Initialize();
            this.PresentationEditorController.RemoveMenuCommands();
            displayPage(this.PresentationEditorController);
            this.PresentationEditorController.getPresentationEditor().NewPageSelected -= this.PresentationEditorController.NewPageSelectedHandler;
            this.PresentationEditorController.getPresentationEditor().OnRemoveNewPage += OnRemoveNewPage;
            this.PresentationEditorController.ChangeEventListener += OnChange;
        }

        public void OnChange()
        {
            SaveButton.IsEnabled = true;
        }

        private void OnRemoveNewPage(bool remove = false)
        {

        }

        #endregion


        #region Operations

        public void DisplayTransformationSlide()
        {
            if (TransformationSlide == null) return;
            if (TransformationSlide.oid.HasValue)
            {
                this.PresentationEditorController.Open(TransformationSlide);
                PresentationEditorItem page = (PresentationEditorItem)this.PresentationEditorController.getPresentationEditor().getActivePage();
                SaveButton.IsEnabled = false;
            }
            else
            {
                if (this.PresentationEditorController.getPresentationEditor().getPages().Count == 0)
                {
                    this.PresentationEditorController.Create();
                    this.TransformationSlide = this.PresentationEditorController.getPresentationEditor().getActivePage().EditedObject;
                }
                this.PresentationEditorController.customiseSlideMenu(this.dockingManager);
                ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
                SaveButton.IsEnabled = true;
                this.PresentationEditorController.OnChange();
            }
            
        }
        
        #endregion
        


        #region TransformationSlide

        public void initializeSideBarData(System.Collections.ObjectModel.ObservableCollection<TransformationTreeItem> loops)
        {
            if (this.PresentationEditorController != null)
            {
                this.PresentationEditorController.Loops = loops;
                TreeLoopGroup LoopGroup = ((PresentationSideBar)this.PresentationEditorController.SideBar).TreeLoopGroup;
                if (LoopGroup != null) LoopGroup.TransformationTreeLoopTreeview.fillTree(loops);
            }
        }
      

        private void onSlideNameChange(string newName)
        {
            if (OnTransformationSlideNameChange != null) OnTransformationSlideNameChange(newName);
        }

        public bool CloseSlideWithoutSave()
        {
            if (this.PresentationEditorController == null) return true;
            foreach (PresentationEditorItem page in this.PresentationEditorController.getPresentationEditor().getAllPages())
            {
                page.getPresentationForm().SlideView.Close();
                this.PresentationEditorController.IsModify = false;
                page.IsModify = false;
                page.Close();
            }
            HistoryHandler.Instance.closePage(this.PresentationEditorController);
            ApplicationManager.Instance.MainWindow.StatusLabel.Content = "";
            Kernel.Util.ClipbordUtil.ClearClipboard();
            TreeActionDialog.IsActionReportView = true;
            return true;
        }

        public bool CloseReportWithSave()
        {
            this.PresentationEditorController.SaveReport();
            return true;
        }
        
        public bool CloseReportWithoutSave()
        {
            this.PresentationEditorController.CloseReportWithoutSave();
            return true;
        }


        public Kernel.Domain.Presentation CloseSlideWithSave()
        {
            if (this.PresentationEditorController == null) return null;
            Kernel.Domain.Presentation report = this.PresentationEditorController.getPresentationEditor().getActivePage().EditedObject;
            this.PresentationEditorController.Save();
            CloseSlideWithoutSave();
            return report;
        }


        public Kernel.Domain.Presentation SaveSlide()
        {
            if (this.PresentationEditorController == null) return null;
            Kernel.Domain.Presentation report = this.PresentationEditorController.getPresentationEditor().getActivePage().EditedObject;
            this.PresentationEditorController.Save();
            SaveButton.IsEnabled = false;
            return report;
        }


        #endregion


        #region Utils

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

        #endregion
        
    }
}
