using Misp.Kernel.Application;
using Misp.Kernel.Domain;
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

namespace Misp.Kernel.Administration.FunctionnalityViews
{
    /// <summary>
    /// Interaction logic for FunctionnalityView.xaml
    /// </summary>
    public partial class FunctionnalityView : ScrollViewer
    {
        protected static int MAX_BLOCK = 30;

        public List<FunctionnalityGroupField> DisplayedGroupField { get; set; }

        public Dictionary<string, string> dicoFunctionalities { get; set; }

        public List<String> grantedFunctionalityCode
        {
            get
            {
                return dicoFunctionalities != null ? dicoFunctionalities.Keys.ToList() : new List<String>(0);
            }
        }

        public FunctionnalityGroupField ModelGroupField { get; set; }
        public FunctionnalityGroupField TableGroupField { get; set; }
        public FunctionnalityGroupField ReportGroupField { get; set; }
        public FunctionnalityGroupField TreeGroupField { get; set; }
        public FunctionnalityGroupField CombinedTreeGroupField { get; set; }
        public FunctionnalityGroupField TargetGroupField { get; set; }
        public FunctionnalityGroupField AutomaticTargetGroupField { get; set; }
        public FunctionnalityGroupField DesignGroupField { get; set; }
        public FunctionnalityGroupField CalculatedMeasureGroupField { get; set; }
        public FunctionnalityGroupField StructuredReportGroupField { get; set; }
        public FunctionnalityGroupField AutomaticUploadGroupField { get; set; }
        public FunctionnalityGroupField ReconciliationFilterGroupField { get; set; }

        public FunctionnalityGroupField AutomaticGridGroupField { get; set; }
        public FunctionnalityGroupField InputGridGroupField { get; set; }
        public FunctionnalityGroupField ReportGridGroupField { get; set; }

        public FunctionnalityGroupField AutomaticPostingGridGroupField { get; set; }
        public FunctionnalityGroupField PostingGridGroupField { get; set; }

        public FunctionnalityView()
        {
            this.DisplayedGroupField = new List<FunctionnalityGroupField>(0);
            InitializeComponent();
            InitializeGroupFields();
            Display(this.DisplayedGroupField);
        }

        public void AddGroupField(FunctionnalityGroupField groupField)
        {
            if (groupField == null) return;           
            this.DisplayedGroupField.Add(groupField);
            Display(this.DisplayedGroupField);
        }

        public void Display(List<FunctionnalityGroupField> groupFields)
        {
            int groupFieldCount = groupFields.Count;
            buildGrid(groupFieldCount);
            if (groupFieldCount <= 0) return;
            int n = 0;
            foreach (FunctionnalityGroupField groupField in groupFields)
            {
                n++;
                if (n > MAX_BLOCK) return;
                int row = (n <= 2 || (n == 3 && groupFieldCount > 4)) ? 0 : 1;
                int col = 0;
                if (n == 1 || (n == 3 && groupFieldCount <= 4) || (n == 4 && groupFieldCount > 4)) col = 0;
                else if (n == 2 || n == 5) col = 1;
                else if (n == 4 && groupFieldCount == 4) col = 1;
                else if (n == 3 && groupFieldCount > 4) col = 3;
                else if (n > 5) col = 3;

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
                AddGroupField(groupField, row, col, rowSpan, colSpan);
            }
        }

        private void AddGroupField(FunctionnalityGroupField block, int row, int col, int rowSpan, int colSpan)
        {
            Grid.SetRow(block, row);
            Grid.SetColumn(block, col);
            Grid.SetRowSpan(block, rowSpan);
            Grid.SetColumnSpan(block, colSpan);
            block.DisplayFunctionality();
            this.GroupFieldGrid.Children.Add(block);
        }

        private void buildGrid(int groupFieldCount)
        {
            this.GroupFieldGrid.Children.Clear();
            this.GroupFieldGrid.RowDefinitions.Clear();
            this.GroupFieldGrid.ColumnDefinitions.Clear();

            if (groupFieldCount <= 0) return;
            int row = 1;
            int col = 1;
            if (groupFieldCount == 1) { row = 1; col = 1; }
            else if (groupFieldCount == 2) { row = 1; col = 2; }
            else if (groupFieldCount == 3) { row = 2; col = 2; }
            else if (groupFieldCount == 4) { row = 2; col = 2; }
            else if (groupFieldCount == 5) { row = 2; col = 3; }
            else if (groupFieldCount == 6) { row = 2; col = 3; }

            else if (groupFieldCount == 7) { row = 3; col = 3; }
            else if (groupFieldCount == 8) { row = 3; col = 3; }
            else if (groupFieldCount == 9) { row = 3; col = 3; }
            else if (groupFieldCount == 10) { row = 4; col = 3; }
            else if (groupFieldCount == 11) { row = 4; col = 3; }
            else if (groupFieldCount == 12) { row = 4; col = 3; }

            else { row = 5; col = 3; }

            for (int i = 1; i <= row; i++)
            {
                RowDefinition def = new RowDefinition();
                def.Height = new GridLength(ModelGroupField.Height + 20);//new GridLength(1, GridUnitType.Star);
                this.GroupFieldGrid.RowDefinitions.Add(def);
            }

            for (int i = 1; i <= col; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);//new GridLength(ModelBlock.Width + 30);//new GridLength(1, GridUnitType.Star);
                this.GroupFieldGrid.ColumnDefinitions.Add(def);
            }
        }
                
        private void InitializeGroupFields()
        {
            this.ModelGroupField = buildGroupField(FunctionalitiesCode.INITIATION);

            this.TableGroupField = buildGroupField(FunctionalitiesCode.SOURCING);
            this.ReportGroupField = buildGroupField(FunctionalitiesLabel.REPORT_TABLE_DASHBOARD_LABEL);
            this.StructuredReportGroupField = buildGroupField(FunctionalitiesLabel.STRUCTURED_REPORT_DASHBOARD_LABEL);
            this.TreeGroupField = buildGroupField(FunctionalitiesLabel.TRANSFORMATION_TREE_DASHBOARD_LABEL);
            this.CombinedTreeGroupField = buildGroupField(FunctionalitiesLabel.COMBINED_TRANSFORMATION_TREE_DASHBOARD_LABEL);
            this.TargetGroupField = buildGroupField(FunctionalitiesLabel.TARGET_DASHBOARD_LABEL);
            this.DesignGroupField = buildGroupField(FunctionalitiesLabel.DESIGN_DASHBOARD_LABEL);
            this.CalculatedMeasureGroupField = buildGroupField(FunctionalitiesLabel.CALCULATED_MEASURE_DASHBOARD_LABEL);
            this.AutomaticUploadGroupField = buildGroupField(FunctionalitiesLabel.AUTOMATIC_SOURCING_DASHBOARD_LABEL);
            this.InputGridGroupField = buildGroupField(FunctionalitiesLabel.INPUT_GRID_LABEL);
            this.ReportGridGroupField = buildGroupField(FunctionalitiesLabel.REPORT_GRID_LABEL);
            this.AutomaticGridGroupField = buildGroupField(FunctionalitiesLabel.AUTOMATIC_GRID_DASHBOARD_LABEL);
            this.AutomaticTargetGroupField = buildGroupField(FunctionalitiesLabel.AUTOMATIC_TARGET_DASHBOARD_LABEL);
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain())
            {
                this.PostingGridGroupField = buildGroupField(FunctionalitiesLabel.POSTING_GRID_DASHBOARD_LABEL);
                this.AutomaticPostingGridGroupField = buildGroupField(FunctionalitiesLabel.AUTOMATIC_POSTING_GRID_DASHBOARD_LABEL);
                this.ReconciliationFilterGroupField = buildGroupField(FunctionalitiesLabel.RECONCILIATION_FILTER_DASHBOARD_LABEL);
            }
            this.DisplayedGroupField.Add(this.ModelGroupField);
            this.DisplayedGroupField.Add(this.TableGroupField);
            this.DisplayedGroupField.Add(this.ReportGroupField);
            this.DisplayedGroupField.Add(this.TreeGroupField);

            this.DisplayedGroupField.Add(this.AutomaticUploadGroupField);
            this.DisplayedGroupField.Add(this.AutomaticTargetGroupField);
            this.DisplayedGroupField.Add(this.ReportGridGroupField);
            this.DisplayedGroupField.Add(this.InputGridGroupField);
            this.DisplayedGroupField.Add(this.TargetGroupField);
            this.DisplayedGroupField.Add(this.StructuredReportGroupField);
             this.DisplayedGroupField.Add(this.DesignGroupField);
            this.DisplayedGroupField.Add(this.CombinedTreeGroupField);
            this.DisplayedGroupField.Add(this.CalculatedMeasureGroupField);
        }

        private FunctionnalityGroupField buildGroupField(string newFunctionCode)
        {
            FunctionnalityGroupField groupField = new FunctionnalityGroupField(newFunctionCode);
            groupField.AddFunctionality += OnAddFunctionality;
            groupField.RemoveFunctionality += OnRemoveFunctionality;
            groupField.FunctionnalityView = this;
            groupField.functionality = ApplicationManager.Instance.FunctionalityFactory.Get(newFunctionCode);
            return groupField;
        }

        private void OnAddFunctionality(Functionality data)
        {
            if (dicoFunctionalities == null) dicoFunctionalities = new Dictionary<string, string>(0);
            string name = "";
            if (!dicoFunctionalities.TryGetValue(data.Code, out name))
            {
                dicoFunctionalities.Add(data.Code, data.Name);
            }
        }

        private void OnRemoveFunctionality(Functionality data)
        {
            if (dicoFunctionalities == null) dicoFunctionalities = new Dictionary<string, string>(0);
            string name = "";
            if (dicoFunctionalities.TryGetValue(data.Code, out name))
            {
                dicoFunctionalities.Remove(data.Code);
            }
        }

    }
}
