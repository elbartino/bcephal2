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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for Targe.xaml
    /// </summary>
    public partial class TargetItemPanel : UserControl
    {
        #region Constructor

        public InputTableService inputTableService { get; set; }
        public bool IsExpanded { get; set; }
        public TargetItemPanel()
        {
            InitializeComponent();
            this.ComboBox.ItemsSource = new String[] { TargetItem.Operator.AND.ToString(), 
                TargetItem.Operator.NOT.ToString(), TargetItem.Operator.OR.ToString() };
            this.ComboBox.SelectedItem = TargetItem.Operator.AND.ToString();
            InitializeHandlers();
            IsExpanded = true;
        }

        
        /// <summary>
        /// Construit une nouvelle instance de TargetItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public TargetItemPanel(int index,bool isNoAllocation = false) : this()
        {
            this.IsNoAllocation = isNoAllocation;
            this.Index = index;
        }

    
        /// <summary>
        /// Construit une nouvelle instance de TargetItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TagItem à afficher</param>
        public TargetItemPanel(TargetItem item,bool isNoAllocation = false)
            : this()
        {
            Display(item,isNoAllocation);
        }

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

        #region Properties

        public bool IsReadOnly { get; set; }

        private int index;
        private bool IsNoAllocation;

        public TargetItem TargetItem { get; set; }

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "Value " + index;
                this.ComboBox.IsEnabled = index > 1 && !IsNoAllocation;
            }
        }

        public Label Label { get { return this.label; } }
        public Button Button { get { return this.button; } }
        public ComboBox ComboBox { get { return this.comboBox; } }
        public TextBox AttributeTextBox { get { return this.AttributeNameTextBox; } }
        public TextBox ValueTextBox { get { return this.valueTextBox; } }
        public TextBox FormulaTextBox { get { return this.formulaTextBox; } }

        #endregion

        #region Operations

        public void Display(TargetItem item,bool isNoAllocation=false)
        {
            IsNoAllocation = isNoAllocation;
            update = false;
            this.TargetItem = item;
            if (item != null) this.Index = item.position + 1;

            this.AttributeTextBox.Text = item != null && item.attribute != null ? item.attribute.name : "";
            this.ValueTextBox.Text = item != null ? item.value != null ? item.value.name : (item.loop != null ? item.loop.name : (item.refValueName != null ? item.refValueName : "")) : "";
            if(item != null && item.loop != null) this.valueTextBox.Foreground = Brushes.Red;
            this.FormulaTextBox.Text = item != null && item.formula != null ? item.formula : "";
            this.ComboBox.SelectedItem = item != null && item.operatorType != null ? item.operatorType : TargetItem.Operator.AND.ToString();
            update = true;
        }

       

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetTarget(Target value)
        {
            bool added = false;
            if (value == null) return;
            if (this.TargetItem == null)
            {
                this.TargetItem = new TargetItem(Index - 1);
                added = true;
            }

            this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();
            
            if (value is Kernel.Domain.Attribute)
            {
                this.TargetItem.attribute = (Kernel.Domain.Attribute)value;
            }
            else{
                this.TargetItem.attribute = null;
                this.TargetItem.value = value;
                this.TargetItem.formula = "";
                this.TargetItem.loop = null;
            }

            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }

        public void SetLoop(TransformationTreeItem loop)
        {
            bool added = false;
            if (loop == null) return;
            if (this.TargetItem == null)
            {
                this.TargetItem = new TargetItem(Index - 1);
                added = true;
            }

            this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();

            this.TargetItem.attribute = null;
            this.TargetItem.value = null;
            this.TargetItem.formula = "";
            this.TargetItem.loop = loop;

            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }
        
        public void Expand(bool expand)
        {
            if (IsExpanded == expand) return;
            IsExpanded = expand;
            if (expand)
            {
                AttributeCol.Width = new GridLength(1, GridUnitType.Star);
                FormulaCol.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                AttributeCol.Width = new GridLength(0, GridUnitType.Star);
                FormulaCol.Width = new GridLength(0, GridUnitType.Star);
            }
        }


        #endregion
       
        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.Button.Click += OnButtonClick;
            //this.Button.GotFocus += OnGotFocus;
            //this.TagTextBox.GotFocus += OnGotFocus;
            //this.ValueTextBox.GotFocus += OnGotFocus;
            this.FormulaTextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;
            this.ComboBox.SelectionChanged += OnOperatorChanged;

            this.PreviewMouseDown += OnMouseDown;
            this.AttributeTextBox.PreviewMouseDown += OnMouseDown;
            this.ValueTextBox.PreviewMouseDown += OnMouseDown;
            this.FormulaTextBox.PreviewMouseDown += OnMouseDown;
            this.FormulaTextBox.KeyDown += OnValidateFormula;
            this.AttributeTextBox.TextChanged +=AttributeTextBox_TextChanged;
            
        }

        private void AttributeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = this.AttributeTextBox.Text;
            if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                this.ValueTextBox.IsEnabled = false;
                this.FormulaTextBox.IsEnabled = false;
            }
            else 
            {
                this.ValueTextBox.IsEnabled = false;
                this.FormulaTextBox.IsEnabled = true;
            }
            
        }



        private void OnValidateFormula(object sender, KeyEventArgs e)
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

            if (e.Key == Key.Enter && ValidateFormula != null) ValidateFormula(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        bool update = true;
        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.TargetItem != null)
                {
                    this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();
                    Updated(this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null) Deleted(this);
        }


        #endregion

        public void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.button.Visibility = readOnly ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.formulaTextBox.IsEnabled = !readOnly;
            this.valueTextBox.IsEnabled = !readOnly;
        }
    }
}
