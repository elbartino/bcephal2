using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Table;
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

namespace Misp.Sourcing.FilterScope
{
    /// <summary>
    /// Interaction logic for RTargetItemPanel.xaml
    /// </summary>
    public partial class RTargetItemPanel : UserControl
    {

        #region Services
        
        public InputTableService inputTableService { get; set; }
        
        #endregion
      
        #region Components

        public Label Label { get { return this.label; } }

        public ComboBox OperatorComboBox { get { return this.comboBox; } }
        public ComboBox FilterComboBox { get { return this.filterComboBox; } }

        public Button DeleteButton { get { return this.button; } }

        public TextBox AttributeTextBox { get { return this.AttributeNameTextBox; } }
        public TextBox ValueTextBox { get { return this.valueTextBox; } }
        public TextBox FormulaTextBox { get { return this.formulaTextBox; } }

        #endregion 
        
        #region Properties
        
        public bool thraw { get; set; }
        bool update = true;
        bool added = false;
        public bool IsReadOnly { get; set; }

        private bool IsNoAllocation;

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "Value " + index;
                this.OperatorComboBox.IsEnabled = index > 1;
                showHeader(index <= 1);
            }
        }
                
        public TargetItem TargetItem { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du TargetItem.
        /// </summary>
        public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le TargetItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        public event ValidateFormulaEventHandler ValidateFormula;

        #endregion

        #region Constructors
        
        public RTargetItemPanel()
        {
            InitializeComponent();
            InitializeFilter();
        }

        public RTargetItemPanel(int index) : this()
        {
            this.Index = index;
        }

        /// <summary>
        /// Construit une nouvelle instance de TargetItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public RTargetItemPanel(int index,bool isNoAllocation = false) : this(index)
        {
            this.IsNoAllocation = isNoAllocation;
        }
                      
        /// <summary>
        /// Construit une nouvelle instance de TargetItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TagItem à afficher</param>
        public RTargetItemPanel(TargetItem item,bool isNoAllocation = false) : this()
        {
            Display(item,isNoAllocation);
        }
        #endregion

        #region Initializers
        public void InitializeFilter()
        {
            thraw = false;
            InitializeComboboxItemSource();
            InitializeHandlers();
            thraw = true;
        }
      
        private void InitializeComboboxItemSource()
        {
            this.filterComboBox.ItemsSource = new FilterScopeValues[] {FilterScopeValues.IS_BLANCK
                ,FilterScopeValues.NOT_BLANCK,FilterScopeValues.BEGIN_WITH,
                FilterScopeValues.NOT_BEGIN_WITH,FilterScopeValues.CONTAINS
                ,FilterScopeValues.NOT_CONTAINS,FilterScopeValues.NOT_CONTAINS};

            this.OperatorComboBox.ItemsSource = new String[] { TargetItem.Operator.AND.ToString(), 
                TargetItem.Operator.NOT.ToString(), TargetItem.Operator.OR.ToString() };
            this.OperatorComboBox.SelectedItem = TargetItem.Operator.AND.ToString();
        }

        private void InitializeHandlers()
        {
            InitializeComboboxHandlers();
            InitializeButtonHandlers();
            InitializeTextBoxHandlers();
            InitializeActivationHandlers();
        }

        private void InitializeActivationHandlers() 
        {
            this.FormulaTextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;         
            this.PreviewMouseDown += OnMouseDown;
            this.AttributeTextBox.PreviewMouseDown += OnMouseDown;
            this.ValueTextBox.PreviewMouseDown += OnMouseDown;
            this.FormulaTextBox.PreviewMouseDown += OnMouseDown;
        }

        private void InitializeComboboxHandlers()
        {
            this.filterComboBox.SelectionChanged += OnChooseFilter;
            this.OperatorComboBox.SelectionChanged += OnOperatorChanged;
        }

        private void InitializeButtonHandlers()
        {
            this.DeleteButton.Click += OnDeleteButtonClick;
        }

        private void InitializeTextBoxHandlers()
        {
            this.FormulaTextBox.KeyDown += OnValidateFormula;
            this.AttributeTextBox.TextChanged += AttributeTextBox_TextChanged;
        }

        #endregion

        #region View's methods

        private void OnChooseFilter(object sender, SelectionChangedEventArgs e)
        {
            ManageFilterView(sender as ComboBox);
        }

        private void AttributeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ManageValuesTextBoxes(sender as TextBox);
        }

        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
            UpdateTargetItemOperator();
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveItemCurrentItem();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            ActivateCurrentItem();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ActivateCurrentItem();
        }

        private void OnValidateFormula(object sender, KeyEventArgs e)
        {
            CheckFormula(e.Key);
        }

        private void ManageValuesTextBoxes(TextBox attributeTextbox)
        {
            string attributeName = this.AttributeTextBox.Text;
            this.ValueTextBox.IsEnabled = false;
            bool enableFormulaText = attributeName == null || string.IsNullOrEmpty(attributeName) || string.IsNullOrWhiteSpace(attributeName);
            this.FormulaTextBox.IsEnabled = !enableFormulaText;
        }

        private void ManageFilterView(ComboBox sender)
        {
            if (!thraw) return;
            if (!(((ComboBox)sender).SelectedItem is FilterScopeValues)) return;
            FilterScopeValues selectedFilter = (FilterScopeValues)((ComboBox)sender).SelectedItem;
            bool showFilterText = !(selectedFilter == FilterScopeValues.IS_BLANCK
                    || selectedFilter == FilterScopeValues.NOT_BLANCK);
            this.filterTextBox.Visibility = showFilterText ? Visibility.Visible : Visibility.Collapsed;
            showFormulaView(false);
        }

        private void showFilterHeader(bool show)
        {
            this.targetFormula.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool IsFilterViewVisible(Kernel.Domain.Attribute attribute)
        {
            bool show = attribute != null;
            this.FilterComboBox.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            this.filterTextBox.Visibility = System.Windows.Visibility.Collapsed;
            return show;
        }

        private void showFormulaView(bool show)
        {
            this.formulaTextBox.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void showHeader(bool show) 
        {
             targetName.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed; 
             targetValue.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
             targetFormula.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed; 
        }

        #endregion

        #region Display Methods
       
        public void Display(TargetItem item,bool isNoAllocation=false)
        {
            IsNoAllocation = isNoAllocation;
            update = false;
            this.TargetItem = item;
            if (this.TargetItem != null) this.Index = this.TargetItem.position + 1;
            DisplayAttribute();
            DisplayValue();
            DisplayFormula();
            DisplayOperator();
            DisplayFilterScope();
            update = true;
        }

        private void DisplayAttribute()
        {
            if (!IsFilterViewVisible(this.TargetItem != null ? this.TargetItem.attribute : null))
            {
                this.AttributeTextBox.Text = "";
                return;
            }
            this.AttributeTextBox.Text = this.TargetItem.attribute.name;
        }

        private void DisplayValue()
        {
            this.ValueTextBox.Text = TargetItem != null ? TargetItem.value != null ? TargetItem.value.name : (TargetItem.loop != null ? TargetItem.loop.name : (TargetItem.refValueName != null ? TargetItem.refValueName : "")) : "";
            if (this.TargetItem != null && this.TargetItem.loop != null) this.valueTextBox.Foreground = Brushes.Red;
        }

        private void DisplayFormula()
        {
            this.FormulaTextBox.Text = this.TargetItem != null && this.TargetItem.formula != null ? this.TargetItem.formula : "";
        }

        private void DisplayOperator()
        {
            this.OperatorComboBox.SelectedItem = this.TargetItem != null && this.TargetItem.operatorType != null ? this.TargetItem.operatorType : TargetItem.Operator.AND.ToString();
        }

        private void DisplayFilterScope()
        {
            //this.FilterComboBox.SelectedItem = this.TargetItem != null && this.TargetItem.DataFilter
        }

        #endregion

        #region Set Methods

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

        public void SetLoop(TransformationTreeItem loop)
        {
            if (loop == null) return;
            added = false;
            setTargetItem(Index);
            setLoop(loop);
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
            setAttributeValue(value);
            setAttribute(value);            
        }

        private void setAttribute(Target value) 
        {
            if (value != null && value is Kernel.Domain.Attribute)
            this.TargetItem.attribute = (Kernel.Domain.Attribute)value;
        }

        private void setAttributeValue(Target value) 
        {
            if (value != null && !(value is Kernel.Domain.AttributeValue)) return;
            this.TargetItem.attribute = null;
            this.TargetItem.value = value;
            this.TargetItem.formula = "";
            this.TargetItem.loop = null;
        }

        private void setLoop(TransformationTreeItem loop)
        {
            this.TargetItem.attribute = null;
            this.TargetItem.value = null;
            this.TargetItem.formula = "";
            this.TargetItem.loop = loop;
        }

        #endregion

        #region Business Methods

        private void CheckFormula(Key keyPressed)
        {
            if (this.TargetItem == null)
            {
                this.TargetItem = new TargetItem(Index - 1);
            }
            string formula = formulaTextBox.Text;
            this.TargetItem.formula = formula;

            if (string.IsNullOrEmpty(formula))
            {
                this.TargetItem.value = null;
            }

            if (keyPressed == Key.Enter && ValidateFormula != null) ValidateFormula(this);
        }

        private void ActivateCurrentItem()
        {
            if (Activated != null) Activated(this);
        }

        private void RemoveItemCurrentItem()
        {
            if (Deleted != null) Deleted(this);
        }

        private void AddOrUpdateItem()
        {
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }

        private void UpdateTargetItemOperator()
        {
            if (Updated == null || !update) return;
            if (this.TargetItem == null) return;
            this.TargetItem.operatorType = this.OperatorComboBox.SelectedItem.ToString();
            Updated(this);
        }

        #endregion
    }
}
