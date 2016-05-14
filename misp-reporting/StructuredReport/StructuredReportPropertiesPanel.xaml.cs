using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for StructuredReportPropertiesPanel.xaml
    /// </summary>
    public partial class StructuredReportPropertiesPanel : UserControl
    {
        
        #region Events

        public event SelectedItemChangedEventHandler selectionColumnChanged;
        public event UpdateEventHandler Changed;

        public event OnAllocateEachEventHandler OnAllocateEach;
        public delegate void OnAllocateEachEventHandler(bool value);

        public event ChangeItemEventHandler OnSetTableVisible;

        #endregion
        

        #region Properties

        public bool throwEvent = true;

        public Misp.Kernel.Domain.StructuredReport StructuredReport { get; set; }

        #endregion
        

        #region Contructors

        public StructuredReportPropertiesPanel()
        {
            InitializeComponent();
            InitializeColumnListBoxContextMenu();
            InitializeHandlers();
            this.gridEachLoop.Visibility = System.Windows.Visibility.Visible;
            this.visibleInShortcutCheckbox.Visibility = System.Windows.Visibility.Visible;
            this.checkboxAllocateEach.Visibility = System.Windows.Visibility.Collapsed;
        }

        
        private void InitializeColumnListBoxContextMenu()
        {
        }

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        private void InitializeHandlers()
        {
            ColumnForms.Changed += OnColumnChanged;
            ColumnsListBox.SelectionChanged += OnSelectedColumnChanged;
            checkboxAllocateEach.Checked += OnCheck;
            checkboxAllocateEach.Unchecked += OnCheck;
            checkboxAllocateEach.Click += OnCheck;
            visibleInShortcutCheckbox.Checked += OnSetVisible;
            visibleInShortcutCheckbox.Unchecked += OnSetVisible;
            RemoveColumnMenuItem.Click += OnRemoveColumn;
        }

        private void OnSetVisible(object sender, RoutedEventArgs e)
        {
            if (OnSetTableVisible != null) OnSetTableVisible(visibleInShortcutCheckbox.IsChecked.Value);
        }
        
        private void OnCheck(object sender, RoutedEventArgs e)
        {
            if (OnAllocateEach != null) OnAllocateEach(checkboxAllocateEach.IsChecked.Value);
        }

                
        #endregion


        #region Operations
        
        public void SetValue(object value)
        {
            if (value is Measure)
            {
                Measure measure = (Measure)value;
                if (measure.IsLeaf) ColumnForms.SetValue(measure);
                else SetListValue(measure.Leafs);
            }
            else
            {
                ColumnForms.SetValue(value);
            }
        }

        public void SetListValue(List<Measure> values)
        {
            int col = this.StructuredReport.columnListChangeHandler.Items.Count + 1;
            StructuredReportColumn column = ColumnForms.Column;
            foreach (Measure value in values)
            {
                column = new StructuredReportColumn();
                column.isModified = false;
                column.isAdded = false;
                column.position = col++;
                column.SetValue(value);
                this.StructuredReport.AddColumn(column);  
            }
            ColumnForms.Display(column);
            OnColumnChanged(true);
        }

        public void SelecteColumn(Kernel.Ui.Office.Cell activeCell)
        {
            if (activeCell == null) return;
            int col = activeCell.Column;            
            StructuredReportColumn column = this.StructuredReport.GetColumn(col);            
            ColumnForms.Display(column);
        }

        public void Display(Misp.Kernel.Domain.StructuredReport report)
        {
            if (report == null) return;
            throwEvent = false;
            this.StructuredReport = report;
            NameTextBox.Text = this.StructuredReport.name;
            groupField.Group = this.StructuredReport.group;
            visibleInShortcutCheckbox.IsChecked = report.visibleInShortcut;
            this.ColumnsListBox.ItemsSource = new ObservableCollection<StructuredReportColumn>(this.StructuredReport.columnListChangeHandler.Items);
            ColumnForms.StructuredReport = this.StructuredReport;
            throwEvent = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void FillStructuredReport(Misp.Kernel.Domain.StructuredReport report)
        {
            if (report == null) return;
            report.name = NameTextBox.Text;
            report.visibleInShortcut = visibleInShortcutCheckbox.IsChecked.Value;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.STRUCTURED_REPORT.label;
            report.group = groupField.Group;
            FillActiveColumn();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void FillActiveColumn()
        {
            ColumnForms.Fill();
        }
        
        #endregion

        private void OnSelectedColumnChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!throwEvent) return;
            StructuredReportColumn column = (StructuredReportColumn)this.ColumnsListBox.SelectedItem;
            FillActiveColumn();
            ColumnForms.Display(column);
            if(selectionColumnChanged!=null)
            selectionColumnChanged(column);
          
        }
                
        private void OnColumnChanged(object rebuild)
        {
            if (!ColumnForms.Column.isAdded) this.StructuredReport.AddColumn(ColumnForms.Column);
            else if (!ColumnForms.Column.isModified) this.StructuredReport.UpdateColumn(ColumnForms.Column);
            this.ColumnsListBox.ItemsSource = new ObservableCollection<StructuredReportColumn>(this.StructuredReport.columnListChangeHandler.Items);
            throwEvent = false;
            this.ColumnsListBox.SelectedItem = ColumnForms.Column;
            throwEvent = true;
            OnChanged((bool)rebuild);
        }

        private void OnRemoveColumn(object sender, RoutedEventArgs e)
        {
            if(ColumnsListBox.SelectedIndex == -1) return;
            foreach (Object column in ColumnsListBox.SelectedItems)
            {
                this.StructuredReport.RemoveColumn((StructuredReportColumn)column);
            }
            this.ColumnsListBox.ItemsSource = new ObservableCollection<StructuredReportColumn>(this.StructuredReport.columnListChangeHandler.Items);
            OnChanged(true);
        }

        private void OnChanged(bool rebuild)
        {
            if (throwEvent && Changed != null) Changed(rebuild);
        }


        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

                
    }
}
