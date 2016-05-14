using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for FileOpenedView.xaml
    /// </summary>
    public partial class FileOpenedView : Grid
    {
        
       // List<InputTableBrowserData> listeTables { get; set; }
        public static bool IstableListOrderedByName;
       // List<InputTableBrowserData> listeReports { get; set; }
        public static bool IsReportListOrderedByName;
        //List<BrowserData> listeModels { get; set; }
        public static bool IsModelListOrderedByName;
        //List<BrowserData> listeTrees { get; set; }
        public static bool IsTreeListOrderedByName;

        public static FileService FileService { get; set; }

        public FileOpenedView()
        {
            InitializeComponent();
            BuildNewControls();
            BuildGuidedTourControl();
        }

        public void BuildRecentOpenedContols() 
        {
            //DashboardData data = FileService.DashBoardService.getDashboardData();
            //if (data == null) return;

            //if (IsModelListOrderedByName)
            //{
            //    IsModelListOrderedByName = false;
            //    updateBrowserdataOrder(OrderByCriteria.NAME, true);
            //}
            //else 
            //{
            //    SortRecentBrowserDataByName(data.models, true);
            //}

            //if (IstableListOrderedByName)
            //{
            //    IstableListOrderedByName = false;
            //    updateInputTableBrowserdataOrder(OrderByCriteria.NAME, false);
            //}
            //else
            //{
            //    SortRecentInputTableBrowserDataByName(data.tables, false);
            //}

            //if (IsReportListOrderedByName)
            //{
            //    IsReportListOrderedByName = false;
            //    updateInputTableBrowserdataOrder(OrderByCriteria.NAME, true);
            //}
            //else
            //{
            //    SortRecentInputTableBrowserDataByName(data.reports, true);
            //}

            //if (IsTreeListOrderedByName)
            //{
            //    IsTreeListOrderedByName = false;
            //    updateBrowserdataOrder(OrderByCriteria.NAME, false);
            //}
            //else
            //{
            //    SortRecentBrowserDataByName(data.transformationTrees, false);
            //}
        }

        /// <summary>
        /// Build Recent Opened Files menu
        /// </summary>
        public void BuildRecentOpenedContols(DashboardData data)
        {
            this.ModelItemsPanel.Children.Clear();
            this.TablesItemsPanel.Children.Clear();
            this.TranformationTreeItemsPanel.Children.Clear();
            this.ReportsItemsPanel.Children.Clear();
            if (data != null)
            {
               
                    foreach (BrowserData model in data.models)
                    {
                        string header = model.name;
                        NavigationToken token = NavigationToken.GetModifyViewToken("INITIATION_FUNCTIONALITY", model.oid);
                        this.ModelItemsPanel.Children.Add(BuildRecentFileControl(model.name, model.name, token));
                    }
               
                NewModelTextBlock.Visibility = this.ModelItemsPanel.Children.Count > 0 ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible;
                ModelContextMenu.Visibility = this.ModelItemsPanel.Children.Count == 0 ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible;

              
                    foreach (InputTableBrowserData table in data.tables)
                    {
                        string header = table.name;
                        NavigationToken token = NavigationToken.GetModifyViewToken("NEW_INPUT_TABLE_FUNCTIONALITY", table.oid);
                        this.TablesItemsPanel.Children.Add(BuildCheckBoxFileControl(table.name, table.name, token));
                    }
               
                /*TableContextMenu.Visibility = this.TablesItemsPanel.Children.Count == 0 ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible;*/
                
                    foreach (InputTableBrowserData report in data.reports)
                    {
                        string header = report.name;
                        NavigationToken token = NavigationToken.GetModifyViewToken("NEW_REPORT_FUNCTIONALITY", report.oid);
                        this.ReportsItemsPanel.Children.Add(BuildCheckBoxFileControl(report.name, report.name, token));
                    }
                ReportContextMenu.Visibility = this.ReportsItemsPanel.Children.Count == 0 ? System.Windows.Visibility.Collapsed
                   : System.Windows.Visibility.Visible;

               
                    foreach (BrowserData tree in data.transformationTrees)
                    {
                        string header = tree.name;
                        NavigationToken token = NavigationToken.GetModifyViewToken("NEW_TRANSFORMATION_TREE_FUNCTIONALITY", tree.oid);
                        this.TranformationTreeItemsPanel.Children.Add(BuildCheckBoxFileControl(tree.name, tree.name, token));
                    }
               
                TranformationTreeItemsPanel.Visibility = this.TranformationTreeItemsPanel.Children.Count == 0 ? System.Windows.Visibility.Collapsed
                   : System.Windows.Visibility.Visible;
            }
            
        }


        public void SortRecentInputTableBrowserDataByName(List<InputTableBrowserData> InputTableBrowserData, bool isReport)
        {
            if (isReport)
            {
                this.ReportsItemsPanel.Children.Clear();
                foreach (InputTableBrowserData report in InputTableBrowserData)
                {
                    string header = report.name;
                    NavigationToken token = NavigationToken.GetModifyViewToken("NEW_REPORT_FUNCTIONALITY", report.oid);
                    this.ReportsItemsPanel.Children.Add(BuildCheckBoxFileControl(report.name, report.name, token));
                }
            }
            else
            {
                this.TablesItemsPanel.Children.Clear();
                foreach (InputTableBrowserData table in InputTableBrowserData)
                {
                    string header = table.name;
                    NavigationToken token = NavigationToken.GetModifyViewToken("NEW_INPUT_TABLE_FUNCTIONALITY", table.oid);
                    this.TablesItemsPanel.Children.Add(BuildCheckBoxFileControl(table.name, table.name, token));
                }
            }
        }

        public void SortRecentBrowserDataByName(List<BrowserData> BrowserData,bool isModel)
        {
            if (isModel)
            {
                this.ModelItemsPanel.Children.Clear();
                foreach (BrowserData model in BrowserData)
                {
                    string header = model.name;
                    NavigationToken token = NavigationToken.GetModifyViewToken("INITIATION_FUNCTIONALITY", model.oid);
                    this.ModelItemsPanel.Children.Add(BuildRecentFileControl(model.name, model.name, token));
                }
            }
            else
            {
                this.TranformationTreeItemsPanel.Children.Clear();
                foreach (BrowserData tree in BrowserData)
                {
                    string header = tree.name;
                    NavigationToken token = NavigationToken.GetModifyViewToken("NEW_TRANSFORMATION_TREE_FUNCTIONALITY", tree.oid);
                    this.TranformationTreeItemsPanel.Children.Add(BuildCheckBoxFileControl(tree.name, tree.name, token));
                }
            }
        }

        protected void BuildNewControls()
        {
            NavigationToken newTableToken = NavigationToken.GetCreateViewToken("NEW_INPUT_TABLE_FUNCTIONALITY");
            NavigationToken newReportToken = NavigationToken.GetCreateViewToken("NEW_REPORT_FUNCTIONALITY");
            NavigationToken newModelToken = NavigationToken.GetCreateViewToken("INITIATION_FUNCTIONALITY");
            NavigationToken newTreeToken = NavigationToken.GetCreateViewToken("NEW_TRANSFORMATION_TREE_FUNCTIONALITY");
            
            Run run1 = new Run("New Table");
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Table"),
                DataContext = newTableToken
            };
            NewTableTextBlock.Inlines.Add(hyperLink);
            NewTableTextBlock.ToolTip = "Create a new input table";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);
            clearTableMenuItem.Click += new RoutedEventHandler(OnClearRecentTables);
            orderTableByNameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_incr.png", UriKind.Relative)) };
            orderTableByNameMenuItem.Click += new RoutedEventHandler(OnOrderingTablesByName);

            run1 = new Run("New Report");
            hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Report"),
                DataContext = newReportToken
            };
            NewReportTextBlock.Inlines.Add(hyperLink);
            NewReportTextBlock.ToolTip = "Create a new report";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);
            clearReportMenuItem.Click += new RoutedEventHandler(OnClearRecentReports);
            orderReportByNameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_incr.png", UriKind.Relative)) };
            orderReportByNameMenuItem.Click += new RoutedEventHandler(OnOrderingReportsByName);
            
            run1 = new Run("New Tranformation Tree");
            hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Tranformation Tree"),
                DataContext = newTreeToken
            };
            NewTranformationTreeTextBlock.Inlines.Add(hyperLink);
            NewTranformationTreeTextBlock.ToolTip = "Create a new Tranformation Tree";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);
            clearTreesMenuItem.Click += new RoutedEventHandler(OnClearRecentTrees);
            orderTreeByNameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_incr.png", UriKind.Relative)) };
            orderTreeByNameMenuItem.Click += new RoutedEventHandler(OnOrderingTreesByName);

            run1 = new Run("New Model");
            hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Model"),
                DataContext = newModelToken
            };
            NewModelTextBlock.Inlines.Add(hyperLink);
            NewModelTextBlock.ToolTip = "Create a new model";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);
            clearModelsMenuItem.Click += new RoutedEventHandler(OnClearRecentModels);
            orderModelsByNameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_incr.png", UriKind.Relative)) };
            orderModelsByNameMenuItem.Click += new RoutedEventHandler(OnOrderingModelsByName);
        }

        protected void BuildGuidedTourControl()
        {
            Run run1 = new Run("Guided Tour...");
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://www.b-cephal.com"),
                DataContext = "Guided Tour"
            };
            GuidedTourTextBlock.Inlines.Add(hyperLink);
            GuidedTourTextBlock.ToolTip = "Guided Tour";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnGuidedTourRequestNavigate);
        }

        private void OnGuidedTourRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        protected TextBlock BuildRecentFileControl(string filePath, string header, NavigationToken token)
        {
            Run run1 = new Run(header);
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + header),
                DataContext = token
            };
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(hyperLink);
            textBlock.ToolTip = filePath;

            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);

            return textBlock;
        }

        protected StackPanel BuildCheckBoxFileControl(string filePath, string header, NavigationToken token)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            
            TextBlock textBlock = BuildRecentFileControl(filePath, header, token);
            CheckBox checkBox = new CheckBox();
            checkBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            checkBox.Margin = Margin = new System.Windows.Thickness(0, 0, 5, 0);
            checkBox.ToolTip = "Set '" + filePath + "' invisible";
            checkBox.IsChecked = true;
            checkBox.Tag = token;
            checkBox.Unchecked += OnSetInvisible;

            panel.Children.Add(checkBox);
            panel.Children.Add(textBlock);
            return panel;
        }

        private void OnSetInvisible(object sender, RoutedEventArgs e)
        {
            if (sender == null || !(sender is CheckBox)) return;
            Object tag = ((CheckBox)sender).Tag;
            if (tag == null || !(tag is NavigationToken)) return;
            NavigationToken token = (NavigationToken) tag;

            //Boolean data = FileService.DashBoardService.setInvisible(token.Functionality, (int)token.ItemId);

            //if ("NEW_INPUT_TABLE_FUNCTIONALITY".Equals(token.Functionality)) updateInputTableBrowserdataOrder(IstableListOrderedByName ? OrderByCriteria.NAME : OrderByCriteria.LAST_MODIFICATION_DATE, false);
            //else if ("NEW_REPORT_FUNCTIONALITY".Equals(token.Functionality)) updateInputTableBrowserdataOrder(IsReportListOrderedByName ? OrderByCriteria.NAME : OrderByCriteria.LAST_MODIFICATION_DATE, true);
            //else if ("NEW_TRANSFORMATION_TREE_FUNCTIONALITY".Equals(token.Functionality)) updateBrowserdataOrder(IsTreeListOrderedByName ? OrderByCriteria.NAME : OrderByCriteria.LAST_MODIFICATION_DATE, false);
        }


        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (sender is Hyperlink)
            {
                Hyperlink link = (Hyperlink)sender;
                object context = link.DataContext;
                if (context is NavigationToken)
                {
                    NavigationToken token = (NavigationToken)context;
                    HistoryHandler.Instance.openPage(token);
                }
            }
        }
 
        #region Clearing recents Item methods

        private void OnClearRecentTrees(object sender, RoutedEventArgs e)
        {
            clearRecentsItems(new TransformationTree());
        }

        private void OnClearRecentReports(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.Browser.InputTableBrowserData objectToClear = new InputTableBrowserData();
            objectToClear.isReport = true;
            clearRecentsItems(objectToClear);
        }

        private void OnClearRecentTables(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.Browser.InputTableBrowserData objectToClear = new InputTableBrowserData();
            clearRecentsItems(objectToClear);
        }

        private void OnClearRecentModels(object sender, RoutedEventArgs e)
        {
            clearRecentsItems(new Kernel.Domain.Model());
        }

        public void clearRecentsItems(Object objectType)
        {
            if (objectType is Kernel.Domain.Model)
            {
                this.ModelItemsPanel.Children.Clear();
                //this.ModelContextMenu.Visibility = System.Windows.Visibility.Collapsed;
                this.NewModelTextBlock.Visibility = System.Windows.Visibility.Visible;
                //FileService.DashBoardService.clear(DashboardConfiguration.MODEL.ToString());
            }
            else if (objectType is Kernel.Domain.Browser.InputTableBrowserData)
            {
                if (((Kernel.Domain.Browser.InputTableBrowserData)objectType).isReport)
                {
                    this.ReportsItemsPanel.Children.Clear();
                    //this.ReportContextMenu.Visibility = System.Windows.Visibility.Collapsed;
                    //FileService.DashBoardService.clear(DashboardConfiguration.REPORT.ToString());
                }
                else
                {
                    this.TablesItemsPanel.Children.Clear();
                    //this.TableContextMenu.Visibility = System.Windows.Visibility.Collapsed;
                    //FileService.DashBoardService.clear(DashboardConfiguration.INPUTTABLE.ToString());
                }
            }
            else if (objectType is Kernel.Domain.TransformationTree)
            {
                this.TranformationTreeItemsPanel.Children.Clear();
                //this.TreeContextMenu.Visibility = System.Windows.Visibility.Collapsed;
                //FileService.DashBoardService.clear(DashboardConfiguration.TREE.ToString());
            }
        }
      
        #endregion

        #region Ordering recents Items methods
        #region Order By Name
        private void OnOrderingReportsByName(object sender, RoutedEventArgs e)
        {
            updateInputTableBrowserdataOrder(OrderByCriteria.NAME,  true);
        }

        private void updateInputTableBrowserdataOrder(OrderByCriteria orderBy, bool isreport) 
        {
            //if (FileService == null) return;
            ////if (isreport && IsReportListOrderedByName) return;
            ////if (!isreport && IstableListOrderedByName) return;
            //String name = isreport ? DashboardConfiguration.REPORT.ToString() : DashboardConfiguration.INPUTTABLE.ToString();
            //List<InputTableBrowserData> tableBrowserDatas = FileService.DashBoardService.getDashboardInputTableBrowserData(name, orderBy);
                            
            //if (tableBrowserDatas == null) return;
            //tableBrowserDatas.BubbleSort();
            //tableBrowserDatas.Limit();
            //if (isreport) IsReportListOrderedByName = true;
            //else IstableListOrderedByName = true;

            //SortRecentInputTableBrowserDataByName(tableBrowserDatas, isreport);
        }

        private void updateBrowserdataOrder(OrderByCriteria orderBy, bool ismodel)
        {
            //if (FileService == null) return;
            ////if (ismodel && IsModelListOrderedByName) return;
            ////if (!ismodel && IsTreeListOrderedByName) return;
            //string name = ismodel ? DashboardConfiguration.MODEL.ToString() : DashboardConfiguration.TREE.ToString();
            //List<BrowserData> browserDatas = FileService.DashBoardService.getDashboardBrowserData(name, orderBy);
            //browserDatas.BubbleSort();
            //browserDatas.Limit();
            //if (browserDatas == null) return;
        
            //if (ismodel) IsModelListOrderedByName = true;
            //else IsTreeListOrderedByName = true;

            //SortRecentBrowserDataByName(browserDatas, ismodel);
        }

        private void OnOrderingTreesByName(object sender, RoutedEventArgs e)
        {
            updateBrowserdataOrder(OrderByCriteria.NAME,  false);
        }

        private void OnOrderingTablesByName(object sender, RoutedEventArgs e)
        {
            updateInputTableBrowserdataOrder(OrderByCriteria.NAME, false);
        }

        private void OnOrderingModelsByName(object sender, RoutedEventArgs e)
        {
            updateBrowserdataOrder(OrderByCriteria.NAME, true);
        }
        #endregion
        #endregion
        
    }
}
