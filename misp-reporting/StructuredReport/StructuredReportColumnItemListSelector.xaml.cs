using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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
    /// Interaction logic for StructuredReportColumnItemListSelector.xaml
    /// </summary>
    public partial class StructuredReportColumnItemListSelector : UserControl
    {

        #region Events

        public event UpdateEventHandler Changed;
        
        #endregion
        
        public PeriodicityService PeriodicityService { get; set; }

        private ModelService ModelService { get; set; }

        public Periodicity Periodicity { get; set; }

        private StructuredReportColumn Column;


        public StructuredReportColumnItemListSelector()
        {
            InitializeComponent();
            InitializeHandlers();
        }
                
        public void Display(StructuredReportColumn column)
        {
            this.Column = column;
            FromListBox.ItemsSource = new ObservableCollection<Object>();
            ToListBox.ItemsSource = new ObservableCollection<Object>();
            if (column.type == StructuredReportColumn.Type.MEASURE.ToString())
            {
                Measure measure = column.measure;
                if (measure != null && !measure.IsLeaf())
                {
                    FromListBox.ItemsSource = measure.GetLeafs();                    
                }
            }
            else if (column.type == StructuredReportColumn.Type.PERIOD.ToString())
            {
                if (column.periodName != null) FromListBox.ItemsSource = column.periodName.Leafs;
                else if (column.periodInterval != null && !column.periodInterval.IsLeaf) FromListBox.ItemsSource = column.periodInterval.Leafs;
            }
            else if (column.type == StructuredReportColumn.Type.TARGET.ToString())
            {
                Target target = column.scope;
                if (target != null)
                {
                    if (target is AttributeValue && !((AttributeValue)target).IsLeaf)
                    {
                        FromListBox.ItemsSource = ((AttributeValue)target).Leafs;
                    }
                    else if (target is Misp.Kernel.Domain.Attribute  )
                    {
                        FromListBox.ItemsSource = ((Misp.Kernel.Domain.Attribute)target).LeafAttributeValues;                        
                    }
                    else if (target is Entity)
                    {
                        FromListBox.ItemsSource = ((Entity)target).LeafAttributeValues;
                    }
                }
            }

            ToListBox.ItemsSource = column.itemListChangeHandler.Items;
            updateButtons();
        }

        private void InitializeHandlers()
        {
            AddAllButton.Content = "->>";
            AddButton.Content = "->";
            RemoveAllButton.Content = "<<-";
            RemoveButton.Content = "<-";

            updateButtons();

            AddAllButton.Click += OnAddAllClicked;
            AddButton.Click += OnAddClicked;
            RemoveAllButton.Click += OnRemoveAllClicked;
            RemoveButton.Click += OnRemoveClicked;

            FromListBox.SelectionChanged += OnSelectionChanged;
            ToListBox.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateButtons();
        }

        public void updateButtons()
        {
            int from = FromListBox.Items.Count;
            int to = ToListBox.Items.Count;

            AddAllButton.IsEnabled = from > 0 && from > to;
            AddButton.IsEnabled = FromListBox.SelectedItem != null && from > to;

            RemoveAllButton.IsEnabled = ToListBox.Items.Count > 0;
            RemoveButton.IsEnabled = ToListBox.SelectedItem != null;
        }

        private void OnRemoveClicked(object sender, RoutedEventArgs e)
        {
            object from = ToListBox.SelectedItem;
            this.Column.Remove(from);
            ToListBox.ItemsSource = this.Column.itemListChangeHandler.Items;
            OnChanged(false);
            updateButtons();
        }

        private void OnRemoveAllClicked(object sender, RoutedEventArgs e)
        {
            this.Column.RemoveAll();
            ToListBox.ItemsSource = this.Column.itemListChangeHandler.Items;
            OnChanged(false);
            updateButtons();
        }

        private void OnAddClicked(object sender, RoutedEventArgs e)
        {
           // object from = FromListBox.SelectedItem;
            var obsColl = FromListBox.SelectedItems;
            foreach (object from in obsColl)
            {
                String name = from is PeriodInterval ? ((PeriodInterval)from).fromAsString : from.ToString();
                if (this.Column.ContainsIntemWithValueString(name))
                {
                    Kernel.Util.MessageDisplayer.DisplayWarning("Duplicate item", name + " is already selected!");
                    return;
                }
                this.Column.Add(from);
            }
                ToListBox.ItemsSource = this.Column.itemListChangeHandler.Items;
                OnChanged(false);
                updateButtons();
           
        }

        private void OnAddAllClicked(object sender, RoutedEventArgs e)
        {
            foreach (Object obj in FromListBox.Items)
            {
                String name = obj is PeriodInterval ? ((PeriodInterval)obj).fromAsString : obj.ToString();
                if (this.Column.ContainsIntemWithValueString(name)) continue;
                this.Column.Add(obj);
            }
            ToListBox.ItemsSource = this.Column.itemListChangeHandler.Items;
            OnChanged(false); 
            updateButtons();
        }

        private void OnChanged(bool rebuild)
        {
            if (Changed != null) Changed(rebuild);
        }


    }
}
