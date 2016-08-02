using System;
using System.Threading;
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
using System.Windows.Threading;
using System.ComponentModel;
using Misp.Kernel.Application;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Controls;
using Misp.Kernel.Task;
using Misp.Kernel.Domain;


namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public event OnCancelProgressionEventHandler OnCancelProgression;
        public delegate void OnCancelProgressionEventHandler();

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            dockingManager.MouseRightButtonDown += OnRightClick;
            this.closeProgressButton.Click += OnClick;
            this.closeProgressButton1.Click += OnClick;
            SetPogressBar1Visible(false);
            SetPogressBar2Visible(false);
            setCloseButtonVisible(false);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (OnCancelProgression != null) OnCancelProgression(); 
        }

        private void OnPageTabDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HistoryHandler.Instance.ActivePage != null) HistoryHandler.Instance.ActivePage.Rename();
        }


        public void SetStatusBarVisible(bool visible)
        {
            StatusBarPanel.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void SetPogressBar1Visible(bool visible)
        {
            StatusBarLabel1.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            ProgressGrid1.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void SetPogressBar2Visible(bool visible)
        {
            StatusBarLabel2.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            ProgressGrid2.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void UpdatePogressBar1(int value, int max, string text)
        {
            ProgressBar1.Value = value;
            ProgressBar1.Maximum = max;
            ProgressBarTextBlock1.Text = text;
            SetPogressBar1Visible(true);
        }

        public void UpdatePogressBar2(int value,int max, string text)
        {
            ProgressBar2.Value = value;
            ProgressBar2.Maximum = max;
            ProgressBarTextBlock2.Text = text;
            SetPogressBar2Visible(true);
        }

        public void setCloseButtonVisible(bool isVisible) 
        {
            closeProgressButton.Visibility = isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed ;
        }

        public void setCloseButton1Visible(bool isVisible)
        {
            closeProgressButton1.Visibility = isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void setCloseButtonToolTip(String text)
        {
            closeProgressButton.ToolTip = text;
        }

        public void setCloseButton1ToolTip(String text)
        {
            closeProgressButton1.ToolTip = text;
        }

        /// <summary>
        /// Cette méthode est excécuté lorsqu'on fait un click droit sur l'entête
        /// d'une page de l'éditeur.
        /// La page en question est activée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource != null && e.OriginalSource is TextBlock)
            {
                string title = ((TextBlock)e.OriginalSource).Text;
                if (string.IsNullOrEmpty(title)) return;
                if (e.Source is Xceed.Wpf.AvalonDock.Controls.LayoutDocumentPaneControl)
                {
                    Xceed.Wpf.AvalonDock.Controls.LayoutDocumentPaneControl source = (Xceed.Wpf.AvalonDock.Controls.LayoutDocumentPaneControl)e.Source;
                    if (source.Model == null) return;
                    if (source.Model is IEditor) ((IEditor)source.Model).selectePage(title);                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            displayPage(null);
            if (MenuBar != null) MenuBar.customizeForFileClosed();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {            
            OperationState state = HistoryHandler.Instance.openPage(new NavigationToken(FunctionalitiesCode.FILE_FUNCTIONALITY, ViewType.LOGOUT));
            e.Cancel = OperationState.STOP == state;
            if(state != OperationState.STOP)
            Kernel.Util.ClipbordUtil.ClearClipboard();
            
        }


        
        /// <summary>
        /// The MenuBar
        /// </summary>
        public MenuBar MenuBar { get; set; }

        /// <summary>
        /// The PropertyBar
        /// </summary>
        public PropertyBar PropertyBar { get; set; }

        /// <summary>
        /// The ToolBar
        /// </summary>
        public Misp.Kernel.Ui.Base.ToolBar ToolBar { get; set; }


        //public ContextMenu ContextMenu
        //{
        //    get { return contextMenu; }
        //    set { this.contextMenu = value; }
        //}

        /// <summary>
        /// The rigthPanel
        /// </summary>
        public LayoutAnchorablePane RigthPanel
        {
            get { return rigthPanel; }
            set { this.rigthPanel = value; }
        }

        /// <summary>
        /// The PropertiesContainer
        /// </summary>
        public LayoutAnchorable PropertiesContainer
        {
            get { return propertiesContainer; }
            set { this.propertiesContainer = value; }
        }

        /// <summary>
        /// The PropertiesContainer
        /// </summary>
        public LayoutAnchorablePane LeftPanel
        {
            get { return leftPanel; }
            set { this.leftPanel = value; }
        }
        
        /// <summary>
        /// The SideBarContainer
        /// </summary>
        public LayoutAnchorable SideBarContainer
        {
            get { return sideBarContainer; }
            set { this.sideBarContainer = value; }
        }

        public void displayToolBar(Misp.Kernel.Ui.Base.ToolBar toolBar)
        {
            if (ToolBar != null) { toolBarGrid.Children.Remove(ToolBar); }
            ToolBar = toolBar;
            if (toolBar != null)
            {
                toolBar.Background = Brushes.Transparent;
                toolBar.HorizontalAlignment = HorizontalAlignment.Right;
                toolBar.VerticalAlignment = VerticalAlignment.Center;
                toolBar.Margin = new Thickness(0, 4, 10, -45);
                Grid.SetRow(toolBar, 0);
                Grid.SetColumn(toolBar, 1);
                toolBarGrid.Children.Add(toolBar);
            }
        }

        public void displaySideBar(SideBar sideBar)
        {
            SideBarContainer.Content = null;
            SideBarContainer.Content = sideBar;
            sideBarContainer.CanAutoHide = false;
        }
        
        public void displayPropertyBar(PropertyBar propertyBar)
        {
            GridLength w = rigthPanel.DockWidth;
            if (rigthPanelGroup.Children.Count > 0)
            {
                try
                {
                    rigthPanelGroup.Children.Clear();
                }
                catch (Exception) { rigthPanelGroup.Children.Clear(); }
            }
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

        public void displayMenuBar(MenuBar menuBar)
        {
            if (MenuBar != null) { mainGrid.Children.Remove(MenuBar); }
            MenuBar = menuBar;
            if (MenuBar != null) 
            {
                Grid.SetRow(menuBar, 0);
                Grid.SetColumn(menuBar, 0);
                mainGrid.Children.Add(menuBar);
            }
        }

        public void displayLogo(Image image)
        {
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            toolBarGrid.Children.Add(image);
        }

        public void displayPage(Controller.Controllable page)
        {
            if (page == null)
            {
                displayToolBar(null);
                displaySideBar(null);
                displayView(null);
                displayPropertyBar(null);
                dockingManager.Visibility = Visibility.Collapsed;
                FileClosedView.Visibility = Visibility.Visible;
                DashboardView.Visibility = Visibility.Collapsed;
                LoginPanel.Visibility = Visibility.Collapsed;
                //FileOpenedView.Visibility = Visibility.Collapsed;
                return;
            }
            displayToolBar(page.ToolBar);
            displaySideBar(page.SideBar);
            displayView(page.View);
            displayPropertyBar(page.PropertyBar);
            if (page.SideBar != null)
            {
                page.SideBar.SelectStatus(page.ModuleName);
            }
            if (page.Functionality == FunctionalitiesCode.FILE_FUNCTIONALITY)
            {
                ((File.FileController)page).RefreshDashboard();
                dockingManager.Visibility = Visibility.Collapsed;
            }
            else if (page.Functionality == FunctionalitiesCode.HOME_PAGE_FUNCTIONALITY)
            {
                dockingManager.Visibility = Visibility.Collapsed;
                FileClosedView.Visibility = ApplicationManager.Instance.File == null ? Visibility.Visible : Visibility.Collapsed;
                DashboardView.Visibility = ApplicationManager.Instance.File != null ? Visibility.Visible : Visibility.Collapsed;
                //FileOpenedView.Visibility = ApplicationManager.Instance.File != null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                dockingManager.Visibility = Visibility.Visible;
                FileClosedView.Visibility = Visibility.Collapsed;
                DashboardView.Visibility = Visibility.Collapsed;
                LoginPanel.Visibility = Visibility.Collapsed;
                //FileOpenedView.Visibility = Visibility.Collapsed;
            }
        }

         public void OnBusyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            BusyAction action = (BusyAction) sender;
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


      
    }
 
}
