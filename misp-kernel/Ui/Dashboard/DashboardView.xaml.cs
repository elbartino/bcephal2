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

        public DashboardBlock AutomaticEnrichmentTableBlock { get; set; }
        public DashboardBlock EnrichmentTableBlock { get; set; }

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
            //InitializeBlocks();
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
            DisplayedBlocks = new List<DashboardBlock>(0);
            if (DisplayedBlocks.Count == 0)
            {
                Dictionary<string, object> dico = new Dictionary<string, object>(0);
                List<DashBoardConfiguration> configurations = DashBoardService.getAllDashboardConfiguration(userOid);
                if (configurations == null || configurations.Count == 0)
                {
                    if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
                    {
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.INITIATION_MODEL_LABEL, 1, userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.AUTOMATIC_SOURCING_DASHBOARD_LABEL, 2, userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.RECONCILIATION_FILTER_DASHBOARD_LABEL, 3,userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.REPORT_LABEL, 4,userOid));
                    }
                    else
                    {
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.INITIATION_MODEL_LABEL, 1,userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.TRANSFORMATION_LABEL, 2,userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.INPUT_TABLE_LABEL, 3, userOid));
                        configurations.Add(new DashBoardConfiguration(FunctionalitiesLabel.REPORT_LABEL, 4,userOid));
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
            this.BlockGrid.Children.Remove(block);
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
                def.Height = new GridLength(new DashboardBlock().Height + 20);//new GridLength(1, GridUnitType.Star);
                this.BlockGrid.RowDefinitions.Add(def);
            }

            for (int i = 1; i <= col; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);//new GridLength(ModelBlock.Width + 30);//new GridLength(1, GridUnitType.Star);
                this.BlockGrid.ColumnDefinitions.Add(def);
            }
        }

        PrivilegeObserver observer;
        public void InitializeBlocks()
        {
            observer = new PrivilegeObserver();

            this.ModelBlock = buildBlock(FunctionalitiesLabel.INITIATION_MODEL_LABEL, FunctionalitiesLabel.INITIATION_NEW_MODEL_LABEL, FunctionalitiesLabel.INITIATION_RECENT_MODEL_LABEL, FunctionalitiesCode.INITIATION_MODEL);

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

            this.AutomaticEnrichmentTableBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_ENRICHMENT_TABLE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_ENRICHMENT_TABLE_LABEL, FunctionalitiesLabel.RECENT_AUTOMATIC_ENRICHMENT_TABLE_LABEL, FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT);
            this.EnrichmentTableBlock = buildBlock(FunctionalitiesLabel.ENRICHMENT_TABLE_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_ENRICHMENT_TABLE_LABEL, FunctionalitiesLabel.RECENT_ENRICHMENT_TABLE_LABEL, FunctionalitiesCode.ENRICHMENT_TABLE_EDIT);
            
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
            {
                this.PostingGridBlock = buildBlock(FunctionalitiesLabel.POSTING_GRID_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_POSTING_GRID_LABEL, FunctionalitiesLabel.RECENT_POSTING_GRID_LABEL, FunctionalitiesCode.POSTING_GRID_EDIT);
                this.AutomaticPostingGridBlock = buildBlock(FunctionalitiesLabel.AUTOMATIC_POSTING_GRID_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_AUTOMATIC_POSTING_GRID_LABEL, FunctionalitiesLabel.RECENT_AUTOMATIC_POSTING_GRID_LABEL, FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT);

                this.ReconciliationFilterBlock = buildBlock(FunctionalitiesLabel.RECONCILIATION_FILTER_DASHBOARD_LABEL, FunctionalitiesLabel.NEW_RECONCILIATION_FILTER_LABEL, FunctionalitiesLabel.RECENT_RECONCILIATION_LABEL, FunctionalitiesCode.RECONCILIATION_FILTER_EDIT);
            }

            Dictionary<string, object> dico = new Dictionary<string, object>(0);

            if (this.AutomaticUploadBlock != null) dico.Add(this.AutomaticUploadBlock.TitleLabel.Content.ToString(), this.AutomaticUploadBlock);
            if (this.CalculatedMeasureBlock != null) dico.Add(this.CalculatedMeasureBlock.TitleLabel.Content.ToString(), this.CalculatedMeasureBlock);
            if (this.CombinedTreeBlock != null) dico.Add(this.CombinedTreeBlock.TitleLabel.Content.ToString(), this.CombinedTreeBlock);
            if (this.DesignBlock != null) dico.Add(this.DesignBlock.TitleLabel.Content.ToString(), this.DesignBlock);
            if (this.TableBlock != null) dico.Add(this.TableBlock.TitleLabel.Content.ToString(), this.TableBlock);
            if (this.ModelBlock != null) dico.Add(this.ModelBlock.TitleLabel.Content.ToString(), this.ModelBlock);
            if (this.ReportBlock != null) dico.Add(this.ReportBlock.TitleLabel.Content.ToString(), this.ReportBlock);
            if (this.StructuredReportBlock != null) dico.Add(this.StructuredReportBlock.TitleLabel.Content.ToString(), this.StructuredReportBlock);
            if (this.TargetBlock != null) dico.Add(this.TargetBlock.TitleLabel.Content.ToString(), this.TargetBlock);
            if (this.TreeBlock != null) dico.Add(this.TreeBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            if (this.AutomaticGridBlock != null) dico.Add(this.AutomaticGridBlock.TitleLabel.Content.ToString(), this.AutomaticGridBlock);
            if (this.InputGridBlock != null) dico.Add(this.InputGridBlock.TitleLabel.Content.ToString(), this.InputGridBlock);
            if (this.ReportGridBlock != null) dico.Add(this.ReportGridBlock.TitleLabel.Content.ToString(), this.ReportGridBlock);
            if (this.AutomaticTargetBlock != null) dico.Add(this.AutomaticTargetBlock.TitleLabel.Content.ToString(), this.AutomaticTargetBlock);
            if (this.PostingGridBlock != null) dico.Add(this.PostingGridBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            if (this.AutomaticPostingGridBlock != null) dico.Add(this.AutomaticPostingGridBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            if (this.ReconciliationFilterBlock != null) dico.Add(this.ReconciliationFilterBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            if (this.AutomaticEnrichmentTableBlock != null) dico.Add(this.AutomaticEnrichmentTableBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            if (this.EnrichmentTableBlock != null) dico.Add(this.EnrichmentTableBlock.TitleLabel.Content.ToString(), this.TreeBlock);
            
            this.MultiSelectorCombobox.ItemsSource = dico;
            this.MultiSelectorCombobox.checkBoxHandler -= OnSelectionChanged;
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
            List<Right> rights = new List<Right>(0);
            bool hasPrivilage = true;
            bool hasCreatePrivilage = true;
            bool hasViewOrEditPrivilage = true;
            if (observer != null && !observer.user.IsAdmin())
            {
                hasPrivilage = false;
                hasCreatePrivilage = false;
                hasViewOrEditPrivilage = false;
                rights = observer.GetRights(newFunctionCode);
                if (rights.Count == 0)
                {
                    if (observer.hasAcendentPrivilege(newFunctionCode))
                    {
                        hasPrivilage = true;
                        hasCreatePrivilage = true;
                        hasViewOrEditPrivilage = true;
                    }
                    else return null;
                }
                                
                foreach (Right right in rights)
                {
                    if (string.IsNullOrWhiteSpace(right.rightType))
                    {
                        hasPrivilage = true;
                        hasCreatePrivilage = true;
                        hasViewOrEditPrivilage = true;
                    }
                    else if (right.rightType.Equals(RightType.CREATE.ToString())) hasCreatePrivilage = true;
                    else if (right.rightType.Equals(RightType.EDIT.ToString())) hasViewOrEditPrivilage = true;
                    else if (right.rightType.Equals(RightType.VIEW.ToString())) hasViewOrEditPrivilage = true;
                }                
            }

            if (hasPrivilage || hasCreatePrivilage || hasViewOrEditPrivilage)
            {
                DashboardBlock block = new DashboardBlock(newFunctionCode);
                block.DashboardView = this;
                block.TitleLabel.Content = title;
                block.RecentItemsTextBlock.Text = recentItemsLabel;
                if (hasPrivilage || hasCreatePrivilage) buildNewControl(block, newLabel, newFunctionCode);
                customizeMenu(block, newFunctionCode);
                return block;
            }
            return null;
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
                || newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_TARGET_EDIT)
                || newFunctionCode.Equals(FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT))
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
            else if (newFunctionCode.Equals(FunctionalitiesCode.ENRICHMENT_TABLE_EDIT))
            {
                //block.contextMenu.Items.Add(block.NewMenuItem);
                //block.contextMenu.Items.Add(block.OpenMenuItem);
                block.contextMenu.Items.Add(block.RunMenuItem);
                block.contextMenu.Items.Add(block.ClearMenuItem);
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
            if (this.ModelBlock != null && this.ModelBlock.TitleLabel.Content.Equals(configuration.name)) return this.ModelBlock;
            if (this.TableBlock != null && this.TableBlock.TitleLabel.Content.Equals(configuration.name)) return this.TableBlock;
            if (this.ReportBlock != null && this.ReportBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReportBlock;
            if (this.StructuredReportBlock != null && this.StructuredReportBlock.TitleLabel.Content.Equals(configuration.name)) return this.StructuredReportBlock;
            if (this.TreeBlock != null && this.TreeBlock.TitleLabel.Content.Equals(configuration.name)) return this.TreeBlock;
            if (this.CombinedTreeBlock != null && this.CombinedTreeBlock.TitleLabel.Content.Equals(configuration.name)) return this.CombinedTreeBlock;
            if (this.TargetBlock != null && this.TargetBlock.TitleLabel.Content.Equals(configuration.name)) return this.TargetBlock;
            if (this.DesignBlock != null && this.DesignBlock.TitleLabel.Content.Equals(configuration.name)) return this.DesignBlock;
            if (this.AutomaticUploadBlock != null && this.AutomaticUploadBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticUploadBlock;
            if (this.CalculatedMeasureBlock != null && this.CalculatedMeasureBlock.TitleLabel.Content.Equals(configuration.name)) return this.CalculatedMeasureBlock;
            if (this.ReconciliationFilterBlock != null && this.ReconciliationFilterBlock != null && this.ReconciliationFilterBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReconciliationFilterBlock;
            if (this.InputGridBlock != null && this.InputGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.InputGridBlock;
            if (this.ReportGridBlock != null && this.ReportGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.ReportGridBlock;
            if (this.AutomaticGridBlock != null && this.AutomaticGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticGridBlock;
            if (this.AutomaticTargetBlock != null && this.AutomaticTargetBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticTargetBlock;
            if (this.AutomaticPostingGridBlock != null && this.AutomaticPostingGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticPostingGridBlock;
            if (this.PostingGridBlock != null && this.PostingGridBlock.TitleLabel.Content.Equals(configuration.name)) return this.PostingGridBlock;
            if (this.AutomaticEnrichmentTableBlock != null && this.AutomaticEnrichmentTableBlock.TitleLabel.Content.Equals(configuration.name)) return this.AutomaticEnrichmentTableBlock;
            if (this.EnrichmentTableBlock != null && this.EnrichmentTableBlock.TitleLabel.Content.Equals(configuration.name)) return this.EnrichmentTableBlock;
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
            if (this.ModelBlock != null && name.Equals(this.ModelBlock.TitleLabel.Content)) return this.ModelBlock;
            if (this.TableBlock != null && name.Equals(this.TableBlock.TitleLabel.Content)) return this.TableBlock;
            if (this.ReportBlock != null && name.Equals(this.ReportBlock.TitleLabel.Content)) return this.ReportBlock;
            if (this.StructuredReportBlock != null && name.Equals(this.StructuredReportBlock.TitleLabel.Content)) return this.StructuredReportBlock;
            if (this.TreeBlock != null && name.Equals(this.TreeBlock.TitleLabel.Content)) return this.TreeBlock;
            if (this.CombinedTreeBlock != null && name.Equals(this.CombinedTreeBlock.TitleLabel.Content)) return this.CombinedTreeBlock;
            if (this.TargetBlock != null && name.Equals(this.TargetBlock.TitleLabel.Content)) return this.TargetBlock;
            if (this.DesignBlock != null && name.Equals(this.DesignBlock.TitleLabel.Content)) return this.DesignBlock;
            if (this.CalculatedMeasureBlock != null && name.Equals(this.CalculatedMeasureBlock.TitleLabel.Content)) return this.CalculatedMeasureBlock;
            if (this.AutomaticUploadBlock != null && name.Equals(this.AutomaticUploadBlock.TitleLabel.Content)) return this.AutomaticUploadBlock;
            if (this.InputGridBlock != null && name.Equals(this.InputGridBlock.TitleLabel.Content)) return this.InputGridBlock;
            if (this.ReportGridBlock != null && name.Equals(this.ReportGridBlock.TitleLabel.Content)) return this.ReportGridBlock;
            if (this.AutomaticGridBlock != null && name.Equals(this.AutomaticGridBlock.TitleLabel.Content)) return this.AutomaticGridBlock;
            if (this.ReconciliationFilterBlock != null && name.Equals(this.ReconciliationFilterBlock.TitleLabel.Content)) return this.ReconciliationFilterBlock;
            if (this.AutomaticTargetBlock != null && name.Equals(this.AutomaticTargetBlock.TitleLabel.Content)) return this.AutomaticTargetBlock;
            if (this.AutomaticPostingGridBlock != null && name.Equals(this.AutomaticPostingGridBlock.TitleLabel.Content)) return this.AutomaticPostingGridBlock;
            if (this.PostingGridBlock != null && name.Equals(this.PostingGridBlock.TitleLabel.Content)) return this.PostingGridBlock;
            if (this.AutomaticEnrichmentTableBlock != null && name.Equals(this.AutomaticEnrichmentTableBlock.TitleLabel.Content)) return this.AutomaticEnrichmentTableBlock;
            if (this.EnrichmentTableBlock != null && name.Equals(this.EnrichmentTableBlock.TitleLabel.Content)) return this.EnrichmentTableBlock;
            return null;
        }

        public DashboardBlock getDisplayedBlock(String code)
        {
            if (code == null) return null;
            foreach (DashboardBlock item in DisplayedBlocks)
            {
                if (item.FunctionalityCode.Equals(code)) return item;
            }
            return null;
        }


    }
}
