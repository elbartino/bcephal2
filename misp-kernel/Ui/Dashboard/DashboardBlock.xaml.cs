using DevExpress.Xpf.Core;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardBlock.xaml
    /// </summary>
    public partial class DashboardBlock : Border
    {

        public string FunctionalityCode { get; set; }
        public DashBoardConfiguration Configuration { get; set; }
        public DashBoardService DashBoardService { get; set; }
        public int? userOid 
        {
            get 
            {
                return ApplicationManager.Instance.User != null ? ApplicationManager.Instance.User.oid : null;
            }
        }
        
        public DashboardView DashboardView { get; set; }

        protected DashboardConfigurationForm configurationForm;

        public DashboardBlock()
        {
            ThemeManager.SetThemeName(this, "None");
            InitializeComponent();
            initContextHandlers();
        }

        public DashboardBlock(DashboardView DashboardView) : this()
        {
            this.DashboardView = DashboardView;
        }

        public DashboardBlock(string functionalityCode) : this()
        {
            this.FunctionalityCode = functionalityCode;
        }

        public void RefreshData()
        {
            if (DashBoardService == null) return;
            List<BrowserData> datas = DashBoardService.getDashboardDatas(GetParam());
            this.SetData(datas);
        }

        public void Reset()
        {
            this.Configuration = null;
            this.SetData(new List<BrowserData>(0));
        }

        public void SetData(List<BrowserData> datas)
        {
            this.ItemsPanel.Children.Clear();
            foreach(BrowserData data in datas)
            {
                DashboradBlockItem item = new DashboradBlockItem(data, this.FunctionalityCode);
                item.Block = this;
                this.ItemsPanel.Children.Add(item);
            }
        }

        public void SelectAll()
        {
            CheckAll(true);
        }

        public void DeselectAll()
        {
            CheckAll(false);
        }

        private void CheckAll(bool check)
        {
            foreach (UIElement elt in this.ItemsPanel.Children)
            {
                if (elt is DashboradBlockItem) ((DashboradBlockItem)elt).CheckBox.IsChecked = check;
            }
        }

        public List<int> GetSelectedIds()
        {
            List<int> ids = new List<int>(0);
            foreach (UIElement item in this.ItemsPanel.Children)
            {
                if (!(item is DashboradBlockItem)) continue;
                DashboradBlockItem blockItem = (DashboradBlockItem)item;
                if (blockItem.CheckBox.IsChecked.Value) ids.Add(blockItem.oid);
            }
            return ids;
        }

        private void initContextHandlers()
        {
            this.ContextMenuOpening += OnContextMenuOpening;
            this.NewMenuItem.Click += OnNew;
            this.OpenMenuItem.Click += OnOpen;
            this.RunMenuItem.Click += OnRun;
            this.ClearAndRunMenuItem.Click += OnClearAndRun;
            this.ClearMenuItem.Click += OnClear;
            this.HideMenuItem.Click += OnHide;
            this.DeleteMenuItem.Click += OnDelete;

            this.SelectAllMenuItem.Click += OnSelectAll;
            this.DeselectAllMenuItem.Click += OnDeselectAll;

            this.OrderByNameAscMenuItem.Click += OnOrderByNameAsc;
            this.OrderByNameDescMenuItem.Click += OnOrderByNameDesc;
            this.OrderByDateAscMenuItem.Click += OnOrderByDateAsc;
            this.OrderByDateDescMenuItem.Click += OnOrderByDateDesc;

            this.ConfigurationMenuItem.Click += OnConfiguration;
        }

        private void OnDeselectAll(object sender, RoutedEventArgs e)
        {
            this.DeselectAll();
        }

        private void OnSelectAll(object sender, RoutedEventArgs e)
        {
            this.SelectAll();
        }

        protected void saveConfigAndRefreshData()
        {
            Configuration = this.DashBoardService.saveDashboardConfiguration(Configuration,userOid);
            RefreshData();
        }

        private void OnConfiguration(object sender, RoutedEventArgs e)
        {
            if (Configuration == null) Configuration = new DashBoardConfiguration(this.TitleLabel.Content.ToString());
            configurationForm = new DashboardConfigurationForm();
            configurationForm.Display(Configuration);

            Dialog dialog = new Dialog("Configuration", configurationForm);
            dialog.Width = 380;
            dialog.Height = 200;
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Configuration = configurationForm.Fill();                
                dialog.Close();
                dialog = null;
                configurationForm = null;
                saveConfigAndRefreshData();
            }
            else
            {
                dialog.Close();
                dialog = null;
                configurationForm = null;
            }
        }

        private void OnOrderByDateDesc(object sender, RoutedEventArgs e)
        {
            if (Configuration == null) return;
            Configuration.orderBy = DashBoardConfiguration.MODIFICATION_DATE;
            Configuration.orderByDirection = DashBoardConfiguration.DESC;
            saveConfigAndRefreshData();
        }

        private void OnOrderByDateAsc(object sender, RoutedEventArgs e)
        {
            if (Configuration == null) return;
            Configuration.orderBy = DashBoardConfiguration.MODIFICATION_DATE;
            Configuration.orderByDirection = DashBoardConfiguration.ASC;
            saveConfigAndRefreshData();
        }

        private void OnOrderByNameDesc(object sender, RoutedEventArgs e)
        {
            if (Configuration == null) return;
            Configuration.orderBy = DashBoardConfiguration.NAME;
            Configuration.orderByDirection = DashBoardConfiguration.DESC;
            saveConfigAndRefreshData();
        }

        private void OnOrderByNameAsc(object sender, RoutedEventArgs e)
        {
            if (Configuration == null) return;
            Configuration.orderBy = DashBoardConfiguration.NAME;
            Configuration.orderByDirection = DashBoardConfiguration.ASC;
            saveConfigAndRefreshData();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            if (isTable()) new DashboardActions().DeleteTables(oids, this);
            else if (isReport()) new DashboardActions().DeleteReports(oids, this);
            else if (isTransformationTree())
            {
                DashboardBlock tableBlock = null;
                if (this.DashboardView.DisplayedBlocks.Contains(this.DashboardView.TableBlock)) tableBlock = this.DashboardView.TableBlock;
                new DashboardActions().DeleteTrees(oids, this, tableBlock);
            }
            else if (isCombinedTransformationTree()) new DashboardActions().DeleteCombinedTrees(oids, this);
            else if (isStructuredReport()) new DashboardActions().DeleteStructuredReports(oids, this);
            else if (isModel()) new DashboardActions().DeleteModels(oids, this);
            else if (isTarget()) new DashboardActions().DeleteTargets(oids, this);
            else if (isCalculatedMeasure()) new DashboardActions().DeleteCalculatedMeasures(oids, this);
            else if (isAutomaticUpload()) new DashboardActions().DeleteAutomaticUploads(oids, this);
            else if (isDesign()) new DashboardActions().DeleteDesigns(oids, this);
            else if (isReconciliationFilterUpload()) new DashboardActions().DeleteReconciliationFilters(oids, this);
            else if (isReconciliationPostingUpload()) new DashboardActions().DeleteReconciliationPostings(oids, this);
            else if (isTransactionFileTypeUpload()) new DashboardActions().DeleteTransactionFileTypes(oids, this);
            else if (isInputGrid()) new DashboardActions().DeleteInputGrid(oids, this);
            else if (isReportGrid()) new DashboardActions().DeleteReportGrid(oids, this);
            else if (isAutomaticGrid()) new DashboardActions().DeleteAutomaticGrid(oids, this);
            else if (isAutomaticPostingGrid()) new DashboardActions().DeleteAutomaticPostingGrid(oids, this);
            else if (isPostingGrid()) new DashboardActions().DeletePostingGrid(oids, this);
            else if (isAutomaticEnrichmentTable()) new DashboardActions().DeleteAutomaticEnrichmentTable(oids, this);
            else if (isEnrichmentTable()) new DashboardActions().DeleteEnrichmentTable(oids, this);
        }

        private void OnHide(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            if (isTable()) new DashboardActions().HideTables(oids, this);
            else if (isReport()) new DashboardActions().HideReports(oids, this);
            else if (isTransformationTree()) new DashboardActions().HideTrees(oids, this);
            else if (isCombinedTransformationTree()) new DashboardActions().HideCombinedTrees(oids, this);
            else if (isStructuredReport()) new DashboardActions().HideStructuredReports(oids, this);
            else if (isModel()) new DashboardActions().HideModels(oids, this);
            else if (isTarget()) new DashboardActions().HideTargets(oids, this);
            else if (isCalculatedMeasure()) new DashboardActions().HideCalculatedMeasures(oids, this);
            else if (isAutomaticUpload()) new DashboardActions().HideAutomaticUploads(oids, this);
            else if (isDesign()) new DashboardActions().HideDesigns(oids, this);
            else if (isReconciliationFilterUpload()) new DashboardActions().HideReconciliationFilters(oids, this);
            else if (isInputGrid()) new DashboardActions().HideInputGrids(oids, this);
            else if (isPostingGrid()) new DashboardActions().HidePostingGrids(oids, this);
            else if (isEnrichmentTable()) new DashboardActions().HideEnrichmentTables(oids, this);
            else if (isReportGrid()) new DashboardActions().HideReportGrids(oids, this);
            else if (isAutomaticGrid()) new DashboardActions().HideAutomaticGrids(oids, this);
            else if (isAutomaticPostingGrid()) new DashboardActions().HideAutomaticPostingGrids(oids, this);
            else if (isReconciliationPostingUpload()) new DashboardActions().HideReconciliationPostings(oids, this);
            else if (isTransactionFileTypeUpload()) new DashboardActions().HideTransactionFileTypes(oids, this);
            else if (isAutomaticEnrichmentTable()) new DashboardActions().HideAutomaticEnrichmentTables(oids, this);
            
            //if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            //List<int> oids = GetSelectedIds();
            //if (oids.Count == 0) return;
            //bool ok = this.DashBoardService.setInvisible(this.FunctionalityCode, oids);
            //if (ok) RefreshData();
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            if (isTable())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.InputGridBlock.FunctionalityCode);
                new DashboardActions().ClearTables(oids, block);
            }
            else if (isTransformationTree())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.TableBlock.FunctionalityCode);
                new DashboardActions().ClearTrees(oids, block);
            }
            else if (isEnrichmentTable())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.TableBlock.FunctionalityCode);
                new DashboardActions().ClearEnrichmentTables(oids, block);
            }
            else if (isCombinedTransformationTree()) new DashboardActions().ClearCombinedTrees(oids);
        }

        private void OnClearAndRun(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            if (isTable()) new DashboardActions().ClearAndRunTables(oids);
            else if (isTransformationTree())
            {
                DashboardBlock tableBlock = null;
                if (this.DashboardView.DisplayedBlocks.Contains(this.DashboardView.TableBlock)) tableBlock = this.DashboardView.TableBlock;
                new DashboardActions().ClearAndRunTrees(oids);
            }
            else if (isCombinedTransformationTree())
            {
                DashboardBlock tableBlock = null;
                if (this.DashboardView.DisplayedBlocks.Contains(this.DashboardView.TableBlock)) tableBlock = this.DashboardView.TableBlock;
                new DashboardActions().ClearAndRunCombinedTrees(oids);
            }
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            if (isTable())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.InputGridBlock.FunctionalityCode);
                new DashboardActions().RunTables(oids, block);
            }
            else if (isReport()) new DashboardActions().RunReports(oids);
            else if (isTransformationTree())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.TableBlock.FunctionalityCode);
                new DashboardActions().RunTrees(oids, block);
            }
            else if (isCombinedTransformationTree())
            {
                if (oids.Count > 1)
                {
                    MessageDisplayer.DisplayWarning("Combined Tranformation Tree", "You're not allowed to run more than one combined tree at time.\nSelect one combined tree.");
                    return;
                }
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.TableBlock.FunctionalityCode);                
                Dictionary<int, List<int>> cTreeWithTreeOid = new DashboardActions().getTreeOid(oids);
                if (cTreeWithTreeOid == null || cTreeWithTreeOid.Count == 0) return;
                foreach (int s in cTreeWithTreeOid.Keys)
                {
                    try
                    {
                        cTreeWithTreeOid.TryGetValue(s, out oids);
                        if (oids == null || oids.Count <= 0) continue;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    new DashboardActions().RunCombinedTrees(oids, block);
                }
            }
            else if (isStructuredReport())
            {
                if (oids.Count > 1)
                {
                    MessageDisplayer.DisplayWarning("Structured Report", "You're not allowed to run more than one structured report at time.\nSelect one report.");
                    return;
                }
                new DashboardActions().RunStructuredReports(oids);
            }
            else if (isAutomaticUpload())
            {
                if (oids.Count > 1)
                {
                    MessageDisplayer.DisplayWarning("Automatic Upload", "You're not allowed to run more than one upload at time.\nSelect one upload.");
                    return;
                }
                new DashboardActions().RunAutomaticUploads(oids);
            }
            else if (isEnrichmentTable())
            {
                DashboardBlock block = this.DashboardView.getDisplayedBlock(this.DashboardView.TableBlock.FunctionalityCode);
                new DashboardActions().RunEnrichmentTables(oids, block);
            }
        }

        private void OnOpen(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            List<int> oids = GetSelectedIds();
            if (oids.Count == 0) return;
            NavigationToken token = NavigationToken.GetModifyViewToken(this.FunctionalityCode, oids[oids.Count-1]);
            HistoryHandler.Instance.openPage(token);
        }

        private void OnNew(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FunctionalityCode)) return;
            NavigationToken token = NavigationToken.GetCreateViewToken(this.FunctionalityCode);
            HistoryHandler.Instance.openPage(token);
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            bool enable = GetSelectedIds().Count > 0;
            this.OpenMenuItem.IsEnabled = enable;
            this.RunMenuItem.IsEnabled = enable;
            this.ClearAndRunMenuItem.IsEnabled = enable;
            this.ClearMenuItem.IsEnabled = enable;
            this.HideMenuItem.IsEnabled = enable;
            this.DeleteMenuItem.IsEnabled = enable;

            int itemCount = this.ItemsPanel.Children.Count;
            enable = itemCount > 0;
            this.OrderByNameAscMenuItem.IsEnabled = enable;
            this.OrderByNameDescMenuItem.IsEnabled = enable;
            this.OrderByDateAscMenuItem.IsEnabled = enable;
            this.OrderByDateDescMenuItem.IsEnabled = enable;

            int selectedCount = GetSelectedIds().Count;
            this.SelectAllMenuItem.IsEnabled = itemCount > selectedCount;
            this.DeselectAllMenuItem.IsEnabled = selectedCount > 0;
        }

        public Boolean isTable()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.INPUT_TABLE_EDIT);
        }

        public Boolean isModel()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.INITIATION_MODEL);
        }

        public Boolean isReport()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.REPORT_EDIT);
        }
        
        public Boolean isDesign()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.DESIGN_EDIT);
        }

        public Boolean isStructuredReport()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.STRUCTURED_REPORT_EDIT);
        }

        public Boolean isTarget()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.TARGET_EDIT);
        }

        public Boolean isTransformationTree()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.TRANSFORMATION_TREE_EDIT);
        }

        public Boolean isCombinedTransformationTree()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT);
        }

        public Boolean isCalculatedMeasure()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.CALCULATED_MEASURE_EDIT);
        }

        public Boolean isAutomaticUpload()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.AUTOMATIC_SOURCING_EDIT);
        }

        public Boolean isInputGrid()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.INPUT_TABLE_GRID_EDIT);
        }

        public Boolean isReportGrid()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.REPORT_GRID_EDIT);
        }

        public Boolean isAutomaticGrid()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.AUTOMATIC_INPUT_TABLE_GRID_EDIT);
        }

        public Boolean isPostingGrid()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.POSTING_GRID_EDIT);
        }

        public Boolean isAutomaticPostingGrid()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.AUTOMATIC_POSTING_GRID_EDIT);
        }

        public Boolean isAutomaticTarget() 
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.AUTOMATIC_TARGET_EDIT);
        }

        public Boolean isReconciliationFilterUpload()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.RECONCILIATION_FILTER_EDIT);
        }

        public Boolean isReconciliationPostingUpload()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.RECONCILIATION_POSTINGS);
        }

        public Boolean isTransactionFileTypeUpload()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.TRANSACTION_FILE_TYPES_FUNCTIONALITY);
        }

        public Boolean isAutomaticEnrichmentTable()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.AUTOMATIC_ENRICHMENT_TABLE_EDIT);
        }

        public Boolean isEnrichmentTable()
        {
            return !string.IsNullOrWhiteSpace(FunctionalityCode) && FunctionalityCode.Equals(FunctionalitiesCode.ENRICHMENT_TABLE_EDIT);
        }

        public string GetParam()
        {
            if (isTable()) return DashBoardService.TABLES;
            if (isReport()) return DashBoardService.REPORTS;
            if (isTarget()) return DashBoardService.TARGETS;
            if (isTransformationTree()) return DashBoardService.TREES;
            if (isCombinedTransformationTree()) return DashBoardService.COMBINED_TREES;
            if (isDesign()) return DashBoardService.DESIGNS;
            if (isAutomaticUpload()) return DashBoardService.AUTOMATIC_UPLOADS;
            if (isCalculatedMeasure()) return DashBoardService.CALCULATED_MEASURES;
            if (isModel()) return DashBoardService.MODELS;
            if (isStructuredReport()) return DashBoardService.STRUCTURED_REPORTS;
            if (isReconciliationFilterUpload()) return DashBoardService.RECONCILIATION_FILTERS;
            if (isReconciliationPostingUpload()) return DashBoardService.RECONCILIATION_POSTINGS;
            if (isTransactionFileTypeUpload()) return DashBoardService.TRANSACTION_FILE_TYPES;
            if (isInputGrid()) return DashBoardService.INPUT_GRID;
            if (isReportGrid()) return DashBoardService.REPORT_GRID;
            if (isAutomaticGrid()) return DashBoardService.AUTOMATIC_GRID;
            if (isAutomaticTarget()) return DashBoardService.AUTOMATIC_TARGET;
            if (isAutomaticPostingGrid()) return DashBoardService.AUTOMATIC_POSTING_GRID;
            if (isPostingGrid()) return DashBoardService.POSTING_GRID;
            if (isAutomaticEnrichmentTable()) return DashBoardService.AUTOMATIC_ENRICHMENT_TABLE;
            if (isEnrichmentTable()) return DashBoardService.ENRICHMENT_TABLE;

            return null;
        }
        
    }
}
