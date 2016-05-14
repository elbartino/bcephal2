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

namespace Misp.Sourcing.Designer
{
    /// <summary>
    /// Interaction logic for DesignerPropertiesPanel.xaml
    /// </summary>
    public partial class DesignerPropertiesPanel : ScrollViewer
    {

        #region Events

        public event ChangeEventHandler Changed;

        #endregion
        

        #region Properties

        private bool throwEvent = true;

        DimensionField activeDimensionField;

        public DimensionField ActiveDimensionField 
        { 
            get{return activeDimensionField;} 
            set{activeDimensionField = value; activateField(activeDimensionField);} 
        }

        public Design Design { get; set; }

        #endregion
        

        #region Contructors

        public DesignerPropertiesPanel()
        {
            InitializeComponent();
            this.CentralField.IsCentral = true;
            InitializeHandlers();
        }

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            MenuItem AddLineMenuItem = new MenuItem();
            AddLineMenuItem.Header = "Add new line...";

            AddLineMenuItem.Click += OnAddLineMenuItemClicked;

            visibleInShortcutCheckBox.Checked += OnChecked;
            visibleInShortcutCheckBox.Unchecked += OnChecked;

            AddTotalColumnRightCheckBox.Checked += OnChecked;
            AddTotalColumnRightCheckBox.Unchecked += OnChecked;
            AddTotalRowBelowCheckBox.Checked += OnChecked;
            AddTotalRowBelowCheckBox.Unchecked += OnChecked;
            ConcatenateColumnHearderCheckBox.Checked += OnChecked;
            ConcatenateColumnHearderCheckBox.Unchecked += OnChecked;
            ConcatenateRowHearderCheckBox.Checked += OnChecked;
            ConcatenateRowHearderCheckBox.Unchecked += OnChecked;

            this.CentralField.Updated += OnUpdated;
            this.CentralField.Activated += OnActivated;
            this.ColumnsField.Updated += OnUpdated;
            this.ColumnsField.Activated += OnActivated;
            this.RowsField.Updated += OnUpdated;
            this.RowsField.Activated += OnActivated;

            this.columnsGroupBox.MouseDown += OnColumnsBoxClick;
            this.rowsGroupBox.MouseDown += OnRowsBoxClick;
            this.centralGroupBox.MouseDown += OnCentralBoxClick;
        }
        
        #endregion


        #region Operations

        public void SetValue(object value)
        {
            if (this.ActiveDimensionField == null) this.ActiveDimensionField = ColumnsField;
            if (value is Measure)
            {
                if (CanAddMeasure()) this.ActiveDimensionField.SetValue(value);
            }
            else if (value is PeriodInterval)
            {
                if (CanAddPeriod()) this.ActiveDimensionField.SetValue(value);
            }
            else if (value is PeriodName) 
            {
                if (CanAddPeriod()) this.ActiveDimensionField.SetValue(value);
            }
            else this.ActiveDimensionField.SetValue(value);
        }

        public bool CanAddMeasure()
        {
            if (this.ActiveDimensionField == null) this.ActiveDimensionField = ColumnsField;
            if (CentralField.ContainsMeasure())
            {
                Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "The central block already contains a measure!");
                return false;
            }

            else if (this.ActiveDimensionField == ColumnsField)
            {
                if (RowsField.ContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "The rows block already contains a measure!");
                    return false;
                }

                if (this.ColumnsField.ContainsMeasure() && !ColumnsField.ActiveLineFieldContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "Another line of columns block already contains a measure!");
                    return false;
                }

            }

            else if (this.ActiveDimensionField == RowsField)
            {
                if (ColumnsField.ContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "The columns block already contains a measure!");
                    return false;
                }

                if (this.RowsField.ContainsMeasure() && !RowsField.ActiveLineFieldContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "Another line of rows block already contains a measure!");
                    return false;
                }
            }

            else if (this.ActiveDimensionField == CentralField)
            {
                if (ColumnsField.ContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "The columns block already contains a measure!");
                    return false;
                }

                if (RowsField.ContainsMeasure())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Measure", "The rows block already contains a measure!");
                    return false;
                }
            }
            return true;
        }

        public bool CanAddPeriod()
        {
            if (this.ActiveDimensionField == null) this.ActiveDimensionField = ColumnsField;
            if (CentralField.ContainsPeriod())
            {
                Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "The central block already contains a period!");
                return false;
            }

            else if (this.ActiveDimensionField == ColumnsField)
            {
                if (RowsField.ContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "The rows block already contains a period!");
                    return false;
                }

                if (this.ColumnsField.ContainsPeriod() && !ColumnsField.ActiveLineFieldContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "Another line of columns block already contains a period!");
                    return false;
                }
            }

            else if (this.ActiveDimensionField == RowsField)
            {
                if (ColumnsField.ContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "The columns block already contains a period!");
                    return false;
                }
               
                if (this.RowsField.ContainsPeriod() && !RowsField.ActiveLineFieldContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "Another line of rows block already contains a period!");
                    return false;
                }
               
            }

            else if (this.ActiveDimensionField == CentralField)
            {
                if (ColumnsField.ContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "The columns block already contains a period!");
                    return false;
                }

                if (RowsField.ContainsPeriod())
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Design Period", "The rows block already contains a period!");
                    return false;
                }
            }
            return true;
        }

        
        public void Display(Design design)
        {
            if (design == null) return;
            throwEvent = false;
            this.Design = design;
            NameTextBox.Text = this.Design.name;
            groupField.Group = this.Design.group;
            visibleInShortcutCheckBox.IsChecked = this.Design.visibleInShortcut;
            
            this.CentralField.Display(this.Design.central);
            this.ColumnsField.Display(this.Design.columns);
            this.RowsField.Display(this.Design.rows);
            
            AddTotalColumnRightCheckBox.IsChecked = this.Design.addTotalColumnRight;
            AddTotalRowBelowCheckBox.IsChecked = this.Design.addTotalRowBelow;
            ConcatenateColumnHearderCheckBox.IsChecked = this.Design.concatenateColumnHearder;
            ConcatenateRowHearderCheckBox.IsChecked = this.Design.concatenateRowHearder;            
            throwEvent = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void FillDesign(Design design)
        {
            if (design == null) return;
            design.name = NameTextBox.Text;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.DESIGN.label;
            design.group = groupField.Group;
            design.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
            design.addTotalColumnRight = AddTotalColumnRightCheckBox.IsChecked.Value;
            design.addTotalRowBelow = AddTotalRowBelowCheckBox.IsChecked.Value;
            design.concatenateColumnHearder = ConcatenateColumnHearderCheckBox.IsChecked.Value;
            design.concatenateRowHearder = ConcatenateRowHearderCheckBox.IsChecked.Value;
        }
        
        #endregion

        private void OnAddLineMenuItemClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (throwEvent && Changed != null) Changed();
        }

        private void OnUpdated(object item)
        {
            DimensionField field = (DimensionField)item;
            if (throwEvent && Changed != null) Changed();
        }

        private void OnActivated(object item)
        {
            DimensionField field = (DimensionField)item;
            this.ActiveDimensionField = field;
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        private void activateField(DimensionField activeDimensionField)
        {
            columnsGroupBox.BorderBrush = Brushes.LightGray;
            rowsGroupBox.BorderBrush = Brushes.LightGray;
            centralGroupBox.BorderBrush = Brushes.LightGray;

            columnsGroupBox.BorderThickness = new Thickness(1);
            rowsGroupBox.BorderThickness = new Thickness(1);
            centralGroupBox.BorderThickness = new Thickness(1);

            if (activeDimensionField != null)
            {
                if (activeDimensionField == ColumnsField)
                {
                    columnsGroupBox.BorderBrush = Brushes.Black;
                    columnsGroupBox.BorderThickness = new Thickness(2);
                }
                else if (activeDimensionField == RowsField)
                {
                    rowsGroupBox.BorderBrush = Brushes.Black;
                    rowsGroupBox.BorderThickness = new Thickness(2);
                }
                else
                {
                    centralGroupBox.BorderBrush = Brushes.Black;
                    centralGroupBox.BorderThickness = new Thickness(2);
                }
            }
        }


        private void OnCentralBoxClick(object sender, MouseButtonEventArgs e)
        {
            this.ActiveDimensionField = CentralField;
        }

        private void OnRowsBoxClick(object sender, MouseButtonEventArgs e)
        {
            this.ActiveDimensionField = RowsField;
        }

        private void OnColumnsBoxClick(object sender, MouseButtonEventArgs e)
        {
            this.ActiveDimensionField = ColumnsField;
        }
        

    }
}
