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

namespace Misp.Sourcing.GridViews
{
    /// <summary>
    /// Interaction logic for GrilleFilterTargetItemPanel.xaml
    /// </summary>
    public partial class GrilleFilterTargetItemPanel : Grid
    {
        
        #region Properties

        bool throwHandlers;
        bool update = true;
        bool added = false;
        public bool IsReadOnly { get; set; }
        
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.ItemLabel.Content = "Value " + index;
                this.OperatorComboBox.IsEnabled = index > 1;
            }
        }
                
        public TargetItem TargetItem { get; set; }

        #endregion
        
        
        #region Events
        
        public event AddEventHandler Added;
        public event UpdateEventHandler Updated;
        public event DeleteEventHandler Deleted;
        public event ActivateEventHandler Activated;

        #endregion


        #region Constructors
        
        public GrilleFilterTargetItemPanel()
        {
            InitializeComponent();
            InitializeFilter();
            ShowFilter(false);
        }

        public GrilleFilterTargetItemPanel(int index) : this()
        {
            this.Index = index;
        }
           
        public GrilleFilterTargetItemPanel(TargetItem item) : this()
        {
            Display(item);
        }

        #endregion


        #region Operations

        public void Display(TargetItem item)
        {
            update = false;
            this.TargetItem = item;
            this.OperatorComboBox.SelectedItem = this.TargetItem != null && this.TargetItem.operatorType != null ? this.TargetItem.operatorType : TargetItem.Operator.AND.ToString();
                
            if (this.TargetItem != null)
            {
                this.Index = this.TargetItem.position + 1;
                if (this.TargetItem.value != null)
                {
                    this.NameTextBox.Text = this.TargetItem.value.name;
                }
                else if (this.TargetItem.attribute != null)
                {                    
                    this.NameTextBox.Text = this.TargetItem.attribute.name;
                    this.FilterComboBox.SelectedItem = this.TargetItem.filterOperator;
                    this.FilterTextBox.Text = this.TargetItem.filterValue1 != null ? this.TargetItem.filterValue1 : "";
                    ShowFilter(true);
                }
            }            
            update = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetTarget(Target value)
        {
            if (value == null) return;
            added = false;
            setTargetItem(Index);
            setTargetValue(value);
            AddOrUpdateItem();
        }

        private void setTargetItem(int position)
        {
            if (this.TargetItem == null)
            {
                this.TargetItem = new TargetItem(Index - 1);
                added = true;
            }
            this.TargetItem.operatorType = this.OperatorComboBox.SelectedItem.ToString();
        }

        private void setTargetValue(Target value)
        {
            throwHandlers = false;
            if (value != null)
            {
                bool showFilter = false;
                if (value != null && value is Kernel.Domain.Attribute)
                {
                    this.TargetItem.attribute = (Kernel.Domain.Attribute)value;                    
                    this.TargetItem.value = null;

                    if (this.FilterComboBox.SelectedItem == null) this.FilterComboBox.SelectedItem = FilterScopeValues.NOT_BLANCK.ToString();
                    this.TargetItem.filterOperator = this.FilterComboBox.SelectedItem.ToString();
                    this.TargetItem.filterValue1 = this.FilterTextBox.Text;

                    showFilter = true;
                }
                else if (value != null && value is Kernel.Domain.AttributeValue)
                {
                    this.TargetItem.attribute = null;
                    this.TargetItem.value = value;
                    this.TargetItem.filterOperator = null;
                    this.TargetItem.filterValue1 = null;
                }
                this.NameTextBox.Text = value.name;
                this.ShowFilter(showFilter);
            }
            throwHandlers = true;
        }
        
        private void AddOrUpdateItem()
        {
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }

        #endregion


        #region Initializers

        public void InitializeFilter()
        {
            throwHandlers = false;
            InitializeComboboxItemSource();
            InitializeHandlers();
            throwHandlers = true;
        }
      
        private void InitializeComboboxItemSource()
        {
            this.FilterComboBox.ItemsSource = new String[] {FilterScopeValues.IS_BLANCK.ToString()
                ,FilterScopeValues.NOT_BLANCK.ToString(),FilterScopeValues.BEGIN_WITH.ToString(),
                FilterScopeValues.NOT_BEGIN_WITH.ToString(),FilterScopeValues.CONTAINS.ToString()
                ,FilterScopeValues.NOT_CONTAINS.ToString(),FilterScopeValues.NOT_CONTAINS.ToString()};

            this.OperatorComboBox.ItemsSource = new String[] { TargetItem.Operator.AND.ToString(), 
                TargetItem.Operator.NOT.ToString(), TargetItem.Operator.OR.ToString() };
            this.OperatorComboBox.SelectedItem = TargetItem.Operator.AND.ToString();
        }

        private void InitializeHandlers()
        {
            this.FilterComboBox.SelectionChanged += OnFilterSelectionChanged;
            this.OperatorComboBox.SelectionChanged += OnOperatorSelectionChanged;
            this.DeleteButton.Click += OnDeleteButtonClick;
            this.FilterTextBox.KeyDown += OnValidateFilterText;
            this.GotFocus += OnGotFocus;
            this.PreviewMouseDown += OnMouseDown;
            this.NameTextBox.PreviewMouseDown += OnMouseDown;
            this.FilterComboBox.PreviewMouseDown += OnMouseDown;
            this.FilterTextBox.PreviewMouseDown += OnMouseDown;
            this.OperatorComboBox.PreviewMouseDown += OnMouseDown;
        }

        private void RemoveHandlers()
        {
            this.FilterComboBox.SelectionChanged -= OnFilterSelectionChanged;
            this.OperatorComboBox.SelectionChanged -= OnOperatorSelectionChanged;
            this.DeleteButton.Click -= OnDeleteButtonClick;
            this.FilterTextBox.KeyDown -= OnValidateFilterText;
            this.GotFocus += OnGotFocus;
            this.PreviewMouseDown -= OnMouseDown;
            this.NameTextBox.PreviewMouseDown -= OnMouseDown;
            this.FilterComboBox.PreviewMouseDown -= OnMouseDown;
            this.FilterTextBox.PreviewMouseDown -= OnMouseDown;
            this.OperatorComboBox.PreviewMouseDown -= OnMouseDown;
        }
        
        #endregion


        #region Handlers

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object item = this.FilterComboBox.SelectedItem;
            if (item != null)
            {
                this.TargetItem.filterOperator = item.ToString();
                UpdateFilterTextBoxVisibility();
                if (throwHandlers && Updated != null) Updated(this);
            }
        }

        private void OnOperatorSelectionChanged(object sender, RoutedEventArgs e)
        {
            Object item = this.OperatorComboBox.SelectedItem;
            if (throwHandlers && item != null)
            {
                this.TargetItem.operatorType = this.OperatorComboBox.SelectedItem.ToString();
                if (Updated != null) Updated(this);
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null) Deleted(this);
        }

        private void OnValidateFilterText(object sender, KeyEventArgs e)
        {
            if (this.TargetItem == null) this.TargetItem = new TargetItem(Index - 1);
            string filterValue = this.FilterTextBox.Text;
            this.TargetItem.filterValue1 = filterValue;

            if (e.Key == Key.Enter && Updated != null) Updated(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
        }        
        
        
        private void ShowFilter(bool show)
        {
            double n = show ? 1 : 0;
            this.FilterComboBoxCol.Width = new GridLength(n, GridUnitType.Star);
            this.FilterTextBoxCol.Width = new GridLength(n, GridUnitType.Star);
            UpdateFilterTextBoxVisibility();
        }

        private void UpdateFilterTextBoxVisibility(bool show = true)
        {
            if (!show) this.FilterTextBoxCol.Width = new GridLength(0, GridUnitType.Star);
            else
            {
                Object item = this.FilterComboBox.SelectedItem;
                if (item != null)
                {
                    String selectedFilter = (String)item;
                    bool showFilterText = selectedFilter != FilterScopeValues.IS_BLANCK.ToString()
                        && selectedFilter != FilterScopeValues.NOT_BLANCK.ToString();
                    double n = showFilterText ? 1 : 0;
                    this.FilterTextBoxCol.Width = new GridLength(n, GridUnitType.Star);
                }
            }
        }

        #endregion

        
    }
}
