using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Reporting.StructuredReport
{
    /// <summary>
    /// Interaction logic for StructuredReportColumnForm.xaml
    /// </summary>
    public partial class StructuredReportColumnForm : UserControl
    {

        #region Events

        public event UpdateEventHandler Changed;

        #endregion

        public Misp.Kernel.Domain.StructuredReport StructuredReport { get; set; }

        public StructuredReportColumn Column { get; set; }

        private bool ModifyThisColumn { get; set; }

        public bool DisplayCellRef { get; set; }

        protected bool throwChange = true;

        public StructuredReportColumnForm()
        {
            InitializeComponent();
            InitializePeriodFormula();
            DisplaySpecial(false);
            InitializeHandlers();
            DisplayCellRef = false;
        }

        private void InitializePeriodFormula()
        {
            this.PeriodFormulaOperationComboBox.ItemsSource = new String[] { Operation.PLUS.sign, Operation.SUB.sign };
            this.PeriodFormulaOperationComboBox.SelectedItem = Operation.PLUS.sign;

            this.PeriodFormulaGranulartityComBox.ItemsSource = new String[]
            {
                Granularity.WEEK.name, Granularity.MONTH.name,
                Granularity.YEAR.name, Granularity.DAY.name,
            };
            this.PeriodFormulaGranulartityComBox.SelectedItem = Granularity.WEEK.name;
            DisplayPeriodFormula(false);
        }

        public void Display(StructuredReportColumn column){
            throwChange = false;
            this.Column = column;
            if (this.Column == null) this.Column = GetNewColumn();
            String colName = Kernel.Util.RangeUtil.GetColumnName(this.Column.position);
            ColumnTextBox.Text = colName;
            TypeTextBox.Text = this.Column.type != null ? this.Column.type : "";
            NameTextBox.Text = this.Column.name != null ? this.Column.name : "";
            CellRefTextBox.Text = this.Column.cellRef != null ? this.Column.cellRef : "";
            FreeTextBox.Text = this.Column.freeText != null ? this.Column.freeText : "";
            LoopTextBox.Text = this.Column.loop != null ? this.Column.loop.name : "";
            StartAtTextBox.Text = this.Column.incrementalStart.HasValue ? this.Column.incrementalStart.Value.ToString() : "1";
            ShowCheckBox.IsChecked = this.Column.show;
            DisplaySpecial(this.Column.type);
            DisplayPeriodFormula(this.Column.type, this.Column.loop);

            PeriodFormulaNumberValueTextBox.Text = this.Column.periodFormulaNumber.HasValue ? this.Column.periodFormulaNumber.Value.ToString() :
                Kernel.Util.TagFormulaUtil.isSyntaxeFormulaCorrectly(this.Column.periodFormula) ? this.Column.periodFormula : "";
            this.PeriodFormulaOperationComboBox.SelectedItem = string.IsNullOrEmpty(this.Column.periodFormulaOperation) ? Operation.PLUS.sign : this.Column.periodFormulaOperation;
            this.PeriodFormulaGranulartityComBox.SelectedItem = string.IsNullOrEmpty(this.Column.periodFormulaGranularity) ? Granularity.WEEK.name : this.Column.periodFormulaGranularity;            

            ItemForm.Display(this.Column);
            this.ModifyThisColumn = false;
            throwChange = true;
        }

        public void Fill()
        {
            if (this.Column == null) return;
            this.Column.freeText = FreeTextBox.Text;            
            this.Column.name = NameTextBox.Text;
            this.Column.cellRef = CellRefTextBox.Text;
            int result;
            bool ok = int.TryParse(StartAtTextBox.Text, out result);
            this.Column.incrementalStart = ok ? result : 1;

            this.Column.periodFormulaGranularity = PeriodFormulaGranulartityComBox.SelectedItem != null ? PeriodFormulaGranulartityComBox.SelectedItem.ToString() : "";
            this.Column.periodFormulaOperation = PeriodFormulaOperationComboBox.SelectedItem != null ? PeriodFormulaOperationComboBox.SelectedItem.ToString() : "";
           
            //this.Column.periodFormulaNumber = null;
            //this.Column.periodFormulaGranularity = null;
            //this.Column.periodFormulaOperation = null;
            //String numberText = PeriodFormulaNumberValueTextBox.Text.Trim();
            ////if (!string.IsNullOrWhiteSpace(numberText))
            //{
            //    int number;
            //    ok = int.TryParse(numberText, out number);
            //    if (ok)
            //    {
            //        this.Column.periodFormulaNumber = number;
            //        this.Column.periodFormula = null;
            //    }
            //    else if (Kernel.Util.TagFormulaUtil.isSyntaxeFormulaCorrectly(numberText))
            //    {
            //        this.Column.periodFormulaNumber = null;
            //        this.Column.periodFormula = numberText;
            //    }
            //    else
            //    {
            //        this.Column.periodFormulaNumber = null;
            //        this.Column.periodFormula = null;
            //    }

            //    this.Column.periodFormulaGranularity = PeriodFormulaGranulartityComBox.SelectedItem.ToString();
            //    this.Column.periodFormulaOperation = PeriodFormulaOperationComboBox.SelectedItem.ToString();
            //}
        }
        
        public void SetValue(object value)
        {
            if (this.Column == null || !this.ModifyThisColumn) this.Column = GetNewColumn();
            this.Column.SetValue(value);
            Display(this.Column);
            OnChanged(true);
        }

        public StructuredReportColumn GetNewColumn(){
            StructuredReportColumn column = new StructuredReportColumn();
            column.isModified = false;
            column.isAdded = false;
            column.position = this.StructuredReport.columnListChangeHandler.Items.Count + 1;
            return column;
        }

        private void InitializeHandlers()
        {
            ItemForm.Changed += OnChanged;
            ShowCheckBox.Checked += OnShowCheckBoxChecked;
            ShowCheckBox.Unchecked += OnShowCheckBoxChecked;
            NameTextBox.KeyUp += OnNameTextChanged;
            CellRefTextBox.KeyUp += OnCellRefTextChanged;
            FreeTextBox.KeyUp += OnValidateFreeTextFormula;

            PeriodFormulaNumberValueTextBox.KeyUp += OnPeriodFormulaNumberChanged;
            PeriodFormulaOperationComboBox.SelectionChanged += OnPeriodFormulaOperationChanged;
            PeriodFormulaGranulartityComBox.SelectionChanged += OnPeriodFormulaGranulartityChanged;
        }
                        
        private void OnValidateFreeTextFormula(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Column.freeText = FreeTextBox.Text;
                OnChanged(true);
            }
        }

        private void OnCellRefTextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Column.cellRef = CellRefTextBox.Text;
                OnChanged(true);
            }
        }

        private void OnNameTextChanged(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter){
                this.Column.name = NameTextBox.Text;
                OnChanged(true);
            }
        }

        private void OnPeriodFormulaNumberChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              setPeriodItems();
              if (throwChange) OnChanged(true);
            }
        }

        private void OnPeriodFormulaGranulartityChanged(object sender, SelectionChangedEventArgs e)
        {
            setPeriodItems();
            if (throwChange)
            {
                this.Column.periodFormulaGranularity = PeriodFormulaGranulartityComBox.SelectedItem != null ? PeriodFormulaGranulartityComBox.SelectedItem.ToString() : "";
                this.Column.periodFormulaOperation = PeriodFormulaOperationComboBox.SelectedItem != null ? PeriodFormulaOperationComboBox.SelectedItem.ToString() : "";
                OnChanged(true);
            }
        }


        private void setPeriodItems() 
        {
            int number;
            string value = PeriodFormulaNumberValueTextBox.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                PeriodFormulaNumberValueTextBox.Text = "";
            }
            else
            {
                
                bool ok = int.TryParse(value, out number);
                if (ok)
                {
                    this.Column.periodFormulaNumber = number;
                    this.Column.periodFormula = null;
                    PeriodFormulaNumberValueTextBox.Text = value;
                }
                else if (Kernel.Util.TagFormulaUtil.isSyntaxeFormulaCorrectly(value))
                {
                    this.Column.periodFormulaNumber = null;
                    this.Column.periodFormula = value;
                    PeriodFormulaNumberValueTextBox.Text = value;
                }
                else
                {
                    this.Column.periodFormula = null;
                    this.Column.periodFormulaNumber = null;
                    PeriodFormulaNumberValueTextBox.Text = "";
                }
            }
            
           }

        private void OnPeriodFormulaOperationChanged(object sender, SelectionChangedEventArgs e)
        {
            setPeriodItems();
            if (throwChange)
            {
                this.Column.periodFormulaGranularity = PeriodFormulaGranulartityComBox.SelectedItem != null ? PeriodFormulaGranulartityComBox.SelectedItem.ToString() : "";
                this.Column.periodFormulaOperation = PeriodFormulaOperationComboBox.SelectedItem != null ? PeriodFormulaOperationComboBox.SelectedItem.ToString() : "";
                OnChanged(true);
            }
        }
        
        private void OnShowCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            this.Column.show = ShowCheckBox.IsChecked.Value;
            if(throwChange) OnChanged(true);
        }



        private void OnChanged(object rebuild)
        {
            if (Changed != null) Changed((bool)rebuild);
        }

        protected void DisplaySpecial(bool display = true) 
        {
            this.SpecialPanelRow.Height = display ? new GridLength(30) : new GridLength(0);
            this.ItempPanelRow.Height = display ? new GridLength(0, GridUnitType.Star) : new GridLength(1, GridUnitType.Star);
        }

        protected void DisplayPeriodFormula(bool display = true)
        {
            this.PeriodFormulaRow.Height = display ? new GridLength(30) : new GridLength(0);
        }

        protected void DisplaySpecial(String type)
        {
            if (type == null)
            {
                this.SpecialPanelRow.Height = new GridLength(0);
                this.ItempPanelRow.Height = new GridLength(0, GridUnitType.Star);
            }
            else if (type == StructuredReportColumn.Type.MEASURE.ToString())
            {
                this.SpecialPanelRow.Height = DisplayCellRef ? new GridLength(30) : new GridLength(0);
                this.ItempPanelRow.Height = new GridLength(1, GridUnitType.Star);
                CellRefGrid.Visibility = DisplayCellRef ? Visibility.Visible : Visibility.Collapsed;
                LoopGrid.Visibility = Visibility.Collapsed;
                IncrementalGrid.Visibility = Visibility.Collapsed;
                FreeTextGrid.Visibility = Visibility.Collapsed;
            }
            else if (type == StructuredReportColumn.Type.TARGET.ToString()
                || type == StructuredReportColumn.Type.TAG.ToString()
                || type == StructuredReportColumn.Type.PERIOD.ToString())
            {
                DisplaySpecial(false);                
            }
            else
            {
                DisplaySpecial(true);
                LoopGrid.Visibility = Visibility.Collapsed;
                IncrementalGrid.Visibility = Visibility.Collapsed;
                FreeTextGrid.Visibility = Visibility.Collapsed;
                CellRefGrid.Visibility = Visibility.Collapsed;
                if (type == StructuredReportColumn.Type.LOOP.ToString()) LoopGrid.Visibility = System.Windows.Visibility.Visible;
                else if (type == StructuredReportColumn.Type.INCREMENTAL.ToString()) IncrementalGrid.Visibility = System.Windows.Visibility.Visible;
                else if (type == StructuredReportColumn.Type.FREE.ToString()) FreeTextGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        protected void DisplayPeriodFormula(String type, TransformationTreeItem loop)
        {
            if (type == null) DisplayPeriodFormula(false);
            else if (type == StructuredReportColumn.Type.PERIOD.ToString()) DisplayPeriodFormula(true);
            else if (type == StructuredReportColumn.Type.LOOP.ToString() && loop != null && loop.IsPeriod) DisplayPeriodFormula(true);
            else DisplayPeriodFormula(false);
        }

    }
}
