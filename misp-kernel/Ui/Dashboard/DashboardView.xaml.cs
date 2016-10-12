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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Grid
    {

        protected static int MAX_BLOCK = 30;

        public List<DashboardBlock> DisplayedBlocks { get; set; }
        public DashBoardService DashBoardService { get; set; }

        public DashboardBlock ModelBlock { get; set; }
        public DashboardBlock TableBlock { get; set; }
        public DashboardBlock ReportBlock { get; set; }
        public DashboardBlock TreeBlock { get; set; }
        public DashboardBlock CombinedTreeBlock { get; set; }
        public DashboardBlock TargetBlock { get; set; }
        public DashboardBlock AutomaticTargetBlock { get; set; }
        public DashboardBlock DesignBlock { get; set; }
        public DashboardBlock CalculatedMeasureBlock { get; set; }
        public DashboardBlock StructuredReportBlock { get; set; }
        public DashboardBlock AutomaticUploadBlock { get; set; }
        public DashboardBlock ReconciliationFilterBlock { get; set; }
        //public DashboardBlock ReconciliationPostingBlock { get; set; }
        //public DashboardBlock TransactionFileTypeBlock { get; set; }
		public DashboardBlock AutomaticGridBlock { get; set; }
		public DashboardBlock InputGridBlock { get; set; }
        public DashboardBlock ReportGridBlock { get; set; }

        public DashboardBlock AutomaticPostingGridBlock { get; set; }
        public DashboardBlock PostingGridBlock { get; set; }

        public int? userOid
        { 
            get  
            {
              return ApplicationManager.Instance.User != null ? ApplicationManager.Instance.User.oid : null;
            }
        }
          
        public DashboardView()
        {
            this.DisplayedBlocks = new List<DashboardBlock>(0);
            InitializeComponent();
            BuildGuidedTourControl();
            InitializeBlocks();
        }

        public void AddBlock(DashboardBlock block)
        {
            if (block == null) return;
            if (DisplayedBlocks.Count >= MAX_BLOCK) return;
            DashBoardConfiguration configuration = DashBoardService.getDashboardConfigurationByName(block.TitleLabel.Content.ToString(),this.userOid);
            if (configuration == null)
            {
                configuration = new DashBoardConfiguration(block.TitleLabel.Content.ToString(), DisplayedBlocks.Count);
            }
            configuration.position = DisplayedBlocks.Count + 1;
            block.Configuration = DashBoardService.saveDashboardConfiguration(configuration,userOid);           
            
            RefreshData(block);
            this.DisplayedBlocks.Add(block);
            Display(this.DisplayedBlocks);
        }

        public void RemoveBlock(DashboardBlock block)
        {
            if (block == null) return;
            if (block.Configuration != null)
            {
                block.Configuration.position = DashBoardConfiguration.DEFAULT_POSITION;
                block.Configuration = DashBoardService.saveDashboardConfiguration(block.Configuration,userOid);
            }
            block.Reset();
            this.DisplayedBlocks.Remove(block);
            Display(this.DisplayedBlocks);
        }

        public void Reset()
        {
            if (this.DisplayedBlocks != null) this.DisplayedBlocks.Clear();
            this.MultiSelectorCombobox.SelectedItems = new Dictionary<string, object>(0);
        }

        public void Refresh()
        {
            if (DisplayedBlocks == null) DisplayedBlocks = new List<DashboardBlock>(0);
            if (DisplayedBlocks.Count == 0)
            {
                Dictionary<string, object> dico = new Dictionary<string, object>(0);
                List<DashBoardConfiguration> configurations = DashBoardService.getAllDashboardConfiguration(userOid);
                if (configurations == null || configurations.Count == 0)
                {
                    if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
                    {
                        configurations.Add(new DashBoardConfiguration(this.ModelBlock.TitleLabel.Content.ToString(), 1,userOid));
                        configurations.Add(new DashBoardConfiguration(this.AutomaticUploadBlock.TitleLabel.Content.ToString(), 2,userOid));
                        configurations.Add(new DashBoardConfiguration(this.ReconciliationFilterBlock.TitleLabel.Content.ToString(), 3,userOid));
                        configurations.Add(new DashBoardConfiguration(this.ReportBlock.TitleLabel.Content.ToString(), 4,userOid));
                    }
                    else
                    {
                        configurations.Add(new DashBoardConfiguration(this.ModelBlock.TitleLabel.Content.ToString(), 1,userOid));
                        configurations.Add(new DashBoardConfiguration(this.TreeBlock.TitleLabel.Content.ToString(), 2,userOid));
                        configurations.Add(new DashBoardConfiguration(this.TableBlock.TitleLabel.Content.ToString(), 3,userOid));
                        configurations.Add(new DashBoardConfiguration(this.ReportBlock.TitleLabel.Content.ToString(), 4,userOid));
                    }
                    configurations = DashBoardService.saveListDashboardConfiguration(configurations,userOid);
                    if (configurations == null) configurations = new List<DashBoardConfiguration>(0);
                }
                foreach (DashBoardConfiguration configuration in configurations)
                {
                    DashboardBlock block = GetBlockByConfiguration(configuration);
                    if (block == null) continue;
                    block.Configuration = configuration;
                    if (DisplayedBlocks.Count >= MAX_BLOCK) break;
                    if (configuration.position == DashBoardConfiguration.DEFAULT_POSITION) continue;
                    if (dico.ContainsKey(block.TitleLabel.Content.ToString())) continue;
                    DisplayedBlocks.Add(block);
                    dico.Add(block.TitleLabel.Content.ToString(), block);
                }
                this.MultiSelectorCombobox.SelectedItems = dico;
            }
            foreach (DashboardBlock block in DisplayedBlocks) RefreshData(block);
            Display(this.DisplayedBlocks);
        }

        

        public void RefreshData(DashboardBlock block)
        {
            block.DashBoardService = DashBoardService;
            block.RefreshData();
        }

        public void Display(List<DashboardBlock> blocks)
        {
            int blockCount = blocks.Count;
            buildGrid(blockCount);
            if (blockCount <= 0) return;
            int n = 0;
            foreach (DashboardBlock block in blocks)
            {
                n++;
                if (n > MAX_BLOCK) return;
                int row = (n <= 2 || (n == 3 && blockCount > 4)) ? 0 : 1;
                int col = 0;
                if(n == 1 || (n == 3 && blockCount <= 4) || (n == 4 && blockCount > 4)) col = 0;
                else if(n == 2 || n == 5)col = 1;
                else if (n == 4 && blockCount == 4) col = 1;
                else if (n == 3 && blockCount > 4) col = 3;
                else if(n > 5) col = 3;

                if (n > 6)
                {
                    int rest = n % 3;
                    int dev = (n - rest) / 3;
                    row = rest > 0 ? dev : dev - 1;
                    col = rest > 0 ? rest - 1 : 2;
                }

                int rowSpan = 1;
                int colSpan = 1;
                //if (n == 3 && blockCount == 3) colSpan = 2;
                //if (n == 3 && blockCount > 4) rowSpan = 2;
                AddBlock(block, row, col, rowSpan, colSpan);
            }
        }

        private void AddBlock(DashboardBlock block, int row, int col, int rowSpan, int colSpan)
        {
            Grid.SetRow(block, row);
            Grid.SetColumn(block, col);
            Grid.SetRowSpan(block, rowSpan);
            Grid.SetColumnSpan(block, colSpan);
            this.BlockGrid.Children.Add(block);
        }

        private void buildGrid(int blockCount)
        {
            this.BlockGrid.Children.Clear();
            this.BlockGrid.RowDefinitions.Clear();
            this.BlockGrid.ColumnDefinitions.Clear();

            if (blockCount <= 0) return;
            int row = 1;
            int col = 1;
            if (blockCount == 1) { row = 1; col = 1; }
            else if (blockCount == 2) { row = 1; col = 2; }
            else if (blockCount == 3) { row = 2; col = 2; }
            else if (blockCount == 4) { row = 2; col = 2; }
            else if (blockCount == 5) { row = 2; col = 3; }
            else if (blockCount == 6) { row = 2; col = 3; }

            else if (blockCount == 7) { row = 3; col = 3; }
            else if (blockCount == 8) { row = 3; col = 3; }
            else if (blockCount == 9) { row = 3; col = 3; }
            else if (blockCount == 10) { row = 4; col = 3; }
            else if (blockCount == 11) { row = 4; col = 3; }
            else if (blockCount == 12) { row = 4; col = 3; }

            else { row = 5; col = 3; }

            for (int i = 1; i <= row; i++)
            {
                RowDefinition def = new RowDefinition();
                def.Height = new GridLength(ModelBlock.Height + 20);//new GridLength(1, GridUnitType.Star);
                this.BlockGrid.RowDefinitions.Add(def);
            }

            for (int i = 1; i <= col; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);//new GridLength(ModelBlock.Width + 30);//new GridLength(1, GridUnitType.Star);
                this.BlockGrid.ColumnDefinitions.Add(def);
            }
        }

        private void InitializeBlocks()
        {
            this.ModelBlock = buildBlock(FunctionalitiesLabel.INITIATION_MODEL_LABEL, FunctionalitiesLabel.INITIATION_NEW_MODEL_LABEL, FunctionalitiesLabel.INITIATION_RECENT_MODEL_LABEL, FunctionalitiesCode.INITIATION);

            this.TableBlock = buildBlock(FunctionalitiesLabel.INPUT_TABLE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_INPUT_TABLE_LABEL, FunctionalitiesLabel.RECENT_INPUT_TABLE_LABEL, FunctionalitiesCode.INPUT_TABLE_EDIT);
            this.ReportBlock = buildBlock(FunctionalitiesLabel.REPORT_TABLE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_REPORT_LABEL, FunctionalitiesLabel.RECENT_REPORT_LABEL, FunctionalitiesCode.REPORT_EDIT);
            this.StructuredReportBlock = buildBlock(FunctionalitiesLabel.STRUCTURED_REPORT_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_STRUCTURED_REPORT_LABEL, FunctionalitiesLabel.RECENT_STRUCTURED_REPORT_LABEL, FunctionalitiesCode.STRUCTURED_REPORT_EDIT);
            this.TreeBlock = buildBlock(FunctionalitiesLabel.TRANSFORMATION_TREE_DASHBOARD_LABEL,  FunctionalitiesLabel.NEW_TRANSFORMATION_TREE_LABEL, FunctionalitiesLabel.RECENT_TRANSFORMATION_TREE_LABEL, FunctionalitiesCode.TRANSFORMATION_TREE_EDIT);
            this.CombinedTreeBlock = buildBlock(FunctionalitiesLabel.COMBINED_TRANSFORMATION_TREE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_COMBINED_TRANSFORMATION_TREES_LABEL,FunctionalitiesLabel.RECENT_TRANSFORMATION_TREE_LABEL , FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT);
            this.TargetBlock = buildBlock(FunctionalitiesLabel.TARGET_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_TARGET_LABEL, FunctionalitiesLabel.RECENT_TARGET_LABEL, FunctionalitiesCode.TARGET_EDIT);
            this.DesignBlock = buildBlock(FunctionalitiesLabel.DESIGN_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_DESIGN_LABEL, FunctionalitiesLabel.RECENT_TARGET_LABEL, FunctionalitiesCode.DESIGN_EDIT);
            this.CalculatedMeasureBlock = buildBlock(FunctionalitiesLabel.CALCULATED_MEASURE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_CALCULATED_MEASURE_LABEL, FunctionalitiesLabel.RECENT_CALCULATED_MEASURE_LABEL, FunctionalitiesCode.CALCULATED_MEASURE_EDIT);
            this.AutomaticUploadBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_SOURCING_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_SOURCING_LABEL,FunctionalitiesLabel.RECENT_AUTOMATIC_SOURCING_LABEL , FunctionalitiesCode.AUTOMATIC_SOURCING_EDIT);
            this.InputGridBlock = buildBlock(FunctionalitiesLabel.INPUT_GRID_LABEL, FunctionalitiesLabel.NEW_INPUT_GRID_LABEL,FunctionalitiesLabel.RECENT_INPUT_GRID_LABEL, FunctionalitiesCode.INPUT_TABLE_GRID_EDIT);
            this.ReportGridBlock = buildBlock(FunctionalitiesLabel.REPORT_GRID_LABEL, FunctionalitiesLabel.NEW_REPORT_GRID_LABEL, FunctionalitiesLabel.RECENT_REPORT_GRID_LABEL, FunctionalitiesCode.REPORT_GRID_EDIT);
            this.AutomaticGridBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_GRID_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_GRID_LABEL, FunctionalitiesLabel.RECENT_AUTOMATIC_GRID_LABEL, FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT);
            this.AutomaticTargetBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_TARGET_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_TARGET_LABEL, FunctionalitiesLabel.RECENT_AUTOMATIC_TARGET_LABEL, FunctionalitiesCode.AUTOMATIC_TARGET_EDIT);
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
            {
                this.PostingGridBlock = buildBlock(FunctionalitiesLabel.POSTING_GRID_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_POSTING_GRID_LABEL, FunctionalitiesLabel.RECENT_POSTING_GRID_LABEL, FunctionalitiesCode.POSTING_GRID_EDIT);
                this.AutomaticPostingGridBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_POSTING_GRID_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_POSTING_GRID_LABEL, FunctionalitiesLabel.RECENT_AUTOMATIC_POSTING_GRID_LABEL, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT);

                this.ReconciliationFilterBlock = buildBlock(FunctionalitiesLabel.RECONCILIATION_FILTER_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_RECONCILIATION_FILTER_LABEL, FunctionalitiesLabel.RECENT_RECONCILIATION_LABEL, FunctionalitiesCode.RECONCILIATION_FILTER_EDIT);
            }

            Dictionary<string, object> dico = new Dictionary<string, object>(0);
            dico.Add(this.AutomaticUploadBlock.TitleLabel.Content.ToString(), this.AutomaticUploadBlock);
            dico.Add(this.CalculatedMeasureBlock.TitleLabel.Content.ToString(), this.CalculatedMeasureBlock);
            dico.Add(this.CombinedTreeBlock.TitleLabel.Content.ToString(), this.CombinedTreeBlock);
            dico.Add(this.DesignBlock.TitleLabel.Content.ToString(), this.DesignBlock);
            dico.Add(this.TableBlock.TitleLabel.Content.ToString(), this.TableBlock);
            dico.Add(this.ModelBlock.TitleLabel.Content.ToString(), this.ModelBlock);
            dico.Add(this.ReportBlock.TitleLabel.Content.ToString(), this.ReportBlock);
            dico.Add(this.StructuredReportBlock.TitleLabel.Content.ToString(), this.StructuredReportBlock);
            dico.Add(this.TargetBlock.TitleLabel.Content.ToString(), this.TargetBlock);
            dico.Add(this.TreeBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            dico.Add(this.AutomaticGridBlock.TitleLabel.Content.ToString(), this.AutomaticGridBlock);
            dico.Add(this.InputGridBlock.TitleLabel.Content.ToString(), this.InputGridBlock);
            dico.Add(this.ReportGridBlock.TitleLabel.Content.ToString(), this.ReportGridBlock);
            dico.Add(this.AutomaticTargetBlock.TitleLabel.Content.ToString(), this.AutomaticTargetBlock);
            if (this.ReconciliationFilterBlock != null)
            {
                dico.Add(this.PostingGridBlock.TitleLabel.Content.ToString(), this.TreeBlock);
                dico.Add(this.AutomaticPostingGridBlock.TitleLabel.Content.ToString(), this.TreeBlock);
                dico.Add(this.ReconciliationFilterBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            }
            
            this.MultiSelectorCombobox.ItemsSource = dico;
            this.MultiSelectorCombobox.checkBoxHandler += OnSelectionChanged;

        }

        private void OnSelectionChanged(object item)
        {
            if (!(item is CheckBox)) return;
            CheckBox checkbox = (CheckBox)item;

            if (DisplayedBlocks.Count >= MAX_BLOCK && checkbox.IsChecked.HasValue && checkbox.IsChecked.Value)
            {
                MessageDisplayer.DisplayWarning("Dashboard", "You can't display more than " + MAX_BLOCK + " blocks.\nRemove one block and try again.");                
                checkbox.IsChecked = false;
                return;
            }
            string name = checkbox.Content.ToString();
            DashboardBlock block = findBlock(name);
            if (block == null) return;
            if (checkbox.IsChecked.HasValue && checkbox.IsChecked.Value) AddBlock(block);
            else RemoveBlock(block);
        }

        private DashboardBlock buildBlock(string title, string newLabel, string recentItemsLabel, string newFunctionCode)
        {
            DashboardBlock block = new DashboardBlock(newFunctionCode);
            block.DashboardView = this;
            block.TitleLabel.Content = title;
            block.RecentItemsTextBlock.Text = recentItemsLabel;
            buildNewControl(block, newLabel, newFunctionCode);
            customizeMenu(block, newFunctionCode);
            return block;
        }

        private DashboardBlock buildBlock(string title, Dictionary<string, object> newLabels, string recentItemsLabel, string newFunctionCode)
        {
            DashboardBlock block = new DashboardBlock(newFunctionCode);
            block.DashboardView = this;
            block.TitleLabel.Content = title;
            block.RecentItemsTextBlock.Text = recentItemsLabel;
            foreach (string newLabel in newLabels.Keys)
            {
                buildNewControl(block, newLabel, newLabels[newLabel].ToString());
            }
            customizeMenu(block, newFunctionCode);
            return block;
        }

        private void customizeMenu(DashboardBlock block, string newFunctionCode)
        {
            if (string.IsNullOrWhiteSpace(newFunctionCode)) return;
            block.contextMenu.Items.Clear();
            if(newFunctionCode.Equals(FunctionalitiesCode.INITIATION))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem); 
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
            else if(newFunctionCode.Equals(FunctionalitiesCode.INPUT_TABLE_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.TRANSFORMATION_TREE_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.RunMenuItem);
                block.contextMenu.Items.Add(block.ClearMenuItem);
                //block.contextMenu.Items.Add(block.ClearAndRunMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem);
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
            else if (newFunctionCode.Equals(FunctionalitiesCode.REPORT_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.STRUCTURED_REPORT_EDIT))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.RunMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem);
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
            else if (newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_SOURCING_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_TARGET_EDIT))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                //block.contextMenu.Items.Add(block.RunMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem);
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
            else if (newFunctionCode.Equals(FunctionalitiesCode.CALCULATED_MEASURE_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.DESIGN_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.TARGET_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.INPUT_TABLE_GRID_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.POSTING_GRID_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.REPORT_GRID_EDIT))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem);
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
            else if (newFunctionCode.Equals(FunctionalitiesCode.RECONCILIATION_FILTER_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.RECONCILIATION_POSTINGS)
                || newFunctionCode.Equals(FunctionalitiesCode.TRANSACTION_FILE_TYPES_FUNCTIONALITY))
            {
                block.contextMenu.Items.Add(block.NewMenuItem);
                block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.HideMenuItem);
                block.contextMenu.Items.Add(block.DeleteMenuItem);
                block.contextMenu.Items.Add(new Separator());
                block.contextMenu.Items.Add(block.SelectAllMenuItem);
                block.contextMenu.Items.Add(block.DeselectAllMenuItem);
                block.contextMenu.Items.Add(block.OrderByMenuItem);
                block.contextMenu.Items.Add(block.ConfigurationMenuItem);
            }
        }
        

        private DashboardBlock GetBlockByConfiguration(DashBoardConfiguration configuration)
        {
            if (configuration == null || string.IsNullOrWhiteSpace(configuration.name)) return null;
            if (this.ModelBlock.TitleLabel.Content.Equals(configuration.name)) return this.ModelBlock;
            if (this.TableBlock.TitleLabel.Content.Equals(configuration.name)) return this.TableBlock;
            if (this.ReportBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReportBlock;
            if (this.StructuredReportBlock.TitleLabel.Content.Equals(configuration.name)) return this.StructuredReportBlock;
            if (this.TreeBlock.TitleLabel.Content.Equals(configuration.name)) return this.TreeBlock;
            if (this.CombinedTreeBlock.TitleLabel.Content.Equals(configuration.name)) return this.CombinedTreeBlock;
            if (this.TargetBlock.TitleLabel.Content.Equals(configuration.name)) return this.TargetBlock;
            if (this.DesignBlock.TitleLabel.Content.Equals(configuration.name)) return this.DesignBlock;
            if (this.AutomaticUploadBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticUploadBlock;
            if (this.CalculatedMeasureBlock.TitleLabel.Content.Equals(configuration.name)) return this.CalculatedMeasureBlock;
            if (this.ReconciliationFilterBlock != null && this.ReconciliationFilterBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReconciliationFilterBlock;
            if (this.InputGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.InputGridBlock;
            if (this.ReportGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReportGridBlock;
            if (this.AutomaticGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticGridBlock;
            if (this.AutomaticTargetBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticTargetBlock;
            if (this.AutomaticPostingGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticPostingGridBlock;
            if (this.PostingGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.PostingGridBlock;
            return null;
        }


        protected void buildNewControl(DashboardBlock block, string newLabel, string newFunctionCode)
        {
            NavigationToken token = NavigationToken.GetCreateViewToken(newFunctionCode);
            Run run1 = new Run(newLabel);
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + newLabel),
                DataContext = token
            };
            block.NewItemTextBlock.Inlines.Add(hyperLink);
            block.NewItemTextBlock.ToolTip = "Create a " + newLabel;
            hyperLink.RequestNavigate += OnRequestNavigate;            
        }

        protected void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
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
            hyperLink.RequestNavigate += OnGuidedTourRequestNavigate;
        }

        private void OnGuidedTourRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private DashboardBlock findBlock(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (name.Equals(this.ModelBlock.TitleLabel.Content)) return this.ModelBlock;
            if (name.Equals(this.TableBlock.TitleLabel.Content)) return this.TableBlock;
            if (name.Equals(this.ReportBlock.TitleLabel.Content)) return this.ReportBlock;
            if (name.Equals(this.StructuredReportBlock.TitleLabel.Content)) return this.StructuredReportBlock;
            if (name.Equals(this.TreeBlock.TitleLabel.Content)) return this.TreeBlock;
            if (name.Equals(this.CombinedTreeBlock.TitleLabel.Content)) return this.CombinedTreeBlock;
            if (name.Equals(this.TargetBlock.TitleLabel.Content)) return this.TargetBlock;
            if (name.Equals(this.DesignBlock.TitleLabel.Content)) return this.DesignBlock;
            if (name.Equals(this.CalculatedMeasureBlock.TitleLabel.Content)) return this.CalculatedMeasureBlock;
            if (name.Equals(this.AutomaticUploadBlock.TitleLabel.Content)) return this.AutomaticUploadBlock;
            if (name.Equals(this.InputGridBlock.TitleLabel.Content)) return this.InputGridBlock;
            if (name.Equals(this.ReportGridBlock.TitleLabel.Content)) return this.ReportGridBlock;
            if (name.Equals(this.AutomaticGridBlock.TitleLabel.Content)) return this.AutomaticGridBlock;
            if (this.ReconciliationFilterBlock != null && name.Equals(this.ReconciliationFilterBlock.TitleLabel.Content)) return this.ReconciliationFilterBlock;
            if (name.Equals(this.AutomaticTargetBlock.TitleLabel.Content)) return this.AutomaticTargetBlock;
            if (name.Equals(this.AutomaticPostingGridBlock.TitleLabel.Content)) return this.AutomaticPostingGridBlock;
            if (name.Equals(this.PostingGridBlock.TitleLabel.Content)) return this.PostingGridBlock;
            return null;
        }


    }
}
