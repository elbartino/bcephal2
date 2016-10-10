using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Designer;
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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        public DesignWindow()
        {
            InitializeComponent();
            this.Title = "Design";
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        public DesignerEditorController DesignerEditorController { get; set; }

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

        public void displaySideBar(SideBar sideBar)
        {
            SideBarContainer.Content = null;
            SideBarContainer.Content = sideBar;
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

        public void displayPage(Misp.Kernel.Controller.Controllable page)
        {
            if (page == null)
            {
                displayToolBar(null);
                displaySideBar(null);
                displayView(null);
                displayPropertyBar(null);
                dockingManager.Visibility = Visibility.Collapsed;
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
            if (page.FunctionalityCode == FunctionalitiesCode.FILE_FUNCTIONALITY)
            {
            }
            else
            {
                dockingManager.Visibility = Visibility.Visible;

            }
        }

        public void displayToolBar(Misp.Kernel.Ui.Base.ToolBar toolBar)
        {
  
            if (ToolBar != null) { toolBarGrid.Children.Remove(ToolBar); }
            ToolBar = toolBar;
            if (toolBar != null)
            {
                toolBar.Children.RemoveAt(1);
                toolBar.Background = Brushes.Transparent;
                toolBar.HorizontalAlignment = HorizontalAlignment.Right;
                toolBar.VerticalAlignment = VerticalAlignment.Center;
                toolBar.Margin = new Thickness(0, 4, 10, -45);
                Grid.SetRow(toolBar, 0);
                Grid.SetColumn(toolBar, 1);
                toolBarGrid.Children.Add(toolBar);
               
                
            }
        }

       
        public InputTableEditorController InputTableEditorController { get; set; }

       
        public Design curentDesign { get; set; }

        private void Design_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DesignerEditorController != null) 
            {
                if (this.DesignerEditorController.IsModify) 
                {
                   OperationState result =    this.DesignerEditorController.TryToSaveBeforeClose();
                   if (result == OperationState.STOP) e.Cancel = true;
                }
            }
            if (this.InputTableEditorController != null)
            {
                this.InputTableEditorController.refreshDesignInSideBar();
               
            }
        }

       
        private void Design_Window_Closed(object sender, EventArgs e)
        {
            if (this.curentDesign != null)
            {
                this.InputTableEditorController.onSelectDesignFromSidebar(curentDesign);
            }
        }
    }
}
