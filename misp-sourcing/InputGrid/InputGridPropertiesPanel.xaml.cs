using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
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

namespace Misp.Sourcing.InputGrid
{
    /// <summary>
    /// Interaction logic for InputGridPropertiesPanel.xaml
    /// </summary>
    public partial class InputGridPropertiesPanel : UserControl
    {
        
        #region Events

        public event SelectedItemChangedEventHandler selectionColumnChanged;
        public event UpdateEventHandler Changed;
        public event ActionEventHandler CanRemoveColumn;

        public event ChangeItemEventHandler OnSetTableVisible;

        #endregion
        

        #region Properties

        public bool throwEvent = true;

        public Grille Grid { get; set; }

        #endregion
        

        #region Contructors

        public InputGridPropertiesPanel()
        {
            InitializeComponent();
            InitializeColumnListBoxContextMenu();
            InitializeHandlers();
            this.gridEachLoop.Visibility = System.Windows.Visibility.Visible;
            this.visibleInShortcutCheckbox.Visibility = System.Windows.Visibility.Visible;
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
            visibleInShortcutCheckbox.Checked += OnSetVisible;
            visibleInShortcutCheckbox.Unchecked += OnSetVisible;
            RemoveColumnMenuItem.Click += OnRemoveColumn;
        }

        private void OnSetVisible(object sender, RoutedEventArgs e)
        {
            if (OnSetTableVisible != null) OnSetTableVisible(visibleInShortcutCheckbox.IsChecked.Value);
        }
        
                
        #endregion


        #region Operations
        
        public void SetValue(object value)
        {
            if (value is Persistent && isUniqueColumn((Persistent)value))
            {
                if (value is Measure)
                {
                    Measure measure = (Measure)value;
                    if (measure.IsLeaf()) ColumnForms.SetValue(measure);
                    //else SetListValue(measure.Leafs);
                }
                else
                {
                    ColumnForms.SetValue(value);
                }
            }            
        }

        public void SetListValue(List<Measure> values)
        {
            int col = this.Grid.columnListChangeHandler.Items.Count + 1;
            GrilleColumn column = ColumnForms.Column;
            foreach (Measure value in values)
            {
                column = new GrilleColumn();
                column.isModified = false;
                column.isAdded = false;
                column.position = col++;
                column.SetValue(value);
                this.Grid.AddColumn(column);  
            }
            ColumnForms.Display(column);
            OnColumnChanged(true);
        }

        protected bool isUniqueColumn(Persistent obj)
        {
            String type = null;
            if (obj is Measure) type = ParameterType.MEASURE.ToString();
            else if (obj is Target) type = ParameterType.SCOPE.ToString();
            else if (obj is PeriodName) type = ParameterType.PERIOD.ToString();
            GrilleColumn column = this.Grid.GetColumn(type, obj.oid.Value);
            if (column != null)
            {
                MessageDisplayer.DisplayWarning("Duplicate column", "The column '" + obj.ToString() + "' is already selected!");
                return false;
            }
            return true;
        }

        public void SelecteColumn(Kernel.Ui.Office.Cell activeCell)
        {
            if (activeCell == null) return;
            int col = activeCell.Column;
            if (this.Grid == null) return;
            GrilleColumn column = this.Grid.GetColumn(col);            
            ColumnForms.Display(column);
        }

        public void Display(Grille grid)
        {
            if (grid == null) return;
            throwEvent = false;
            this.Grid = grid;
            NameTextBox.Text = this.Grid.name;
            groupField.Group = this.Grid.group;
            visibleInShortcutCheckbox.IsChecked = grid.visibleInShortcut;
            CommentTextBlock.Text = this.Grid.comment;
            this.ColumnsListBox.ItemsSource = new ObservableCollection<GrilleColumn>(this.Grid.columnListChangeHandler.Items);
            ColumnForms.Grid = this.Grid;
            throwEvent = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void FillGrid(Grille grid)
        {
            if (grid == null) return;
            grid.name = NameTextBox.Text;
            grid.visibleInShortcut = visibleInShortcutCheckbox.IsChecked.Value;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.STRUCTURED_REPORT.label;
            grid.group = groupField.Group;
            grid.comment = CommentTextBlock.Text.Trim();
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
            GrilleColumn column = (GrilleColumn)this.ColumnsListBox.SelectedItem;
            FillActiveColumn();
            ColumnForms.Display(column);
            if(selectionColumnChanged!=null)
            selectionColumnChanged(column);
          
        }
                
        private void OnColumnChanged(object rebuild)
        {
            if (!ColumnForms.Column.isAdded) this.Grid.AddColumn(ColumnForms.Column);
            else if (!ColumnForms.Column.isModified) this.Grid.UpdateColumn(ColumnForms.Column);
            this.ColumnsListBox.ItemsSource = new ObservableCollection<GrilleColumn>(this.Grid.columnListChangeHandler.Items);
            throwEvent = false;
            this.ColumnsListBox.SelectedItem = ColumnForms.Column;
            throwEvent = true;
            OnChanged((bool)rebuild);
        }

        private void OnRemoveColumn(object sender, RoutedEventArgs e)
        {
            if(ColumnsListBox.SelectedIndex == -1) return;

            e.Handled = true;
            if (!canRemoveColumn(ColumnsListBox.SelectedItems)) return;

            foreach (Object column in ColumnsListBox.SelectedItems)
            {
                this.Grid.RemoveColumn((GrilleColumn)column);
            }
            this.ColumnsListBox.ItemsSource = new ObservableCollection<GrilleColumn>(this.Grid.columnListChangeHandler.Items);
            OnChanged(true);
        }

        private bool canRemoveColumn(Object column)
        {
            if (CanRemoveColumn != null) return CanRemoveColumn(column);
            else return true;
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
