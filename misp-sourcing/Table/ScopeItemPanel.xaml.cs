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
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for ScopeItemPanel.xaml
    /// </summary>
    public partial class ScopeItemPanel : Grid
    {

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

        #endregion

        bool isCustomize { get; set; }
        #region Constructor

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        public ScopeItemPanel()
        {
            InitializeComponent();
            this.ComboBox.ItemsSource = new String[] { TargetItem.Operator.AND.ToString(), 
                TargetItem.Operator.NOT.ToString(), TargetItem.Operator.OR.ToString() };
            this.ComboBox.SelectedItem = TargetItem.Operator.AND.ToString();

            this.comboBoxBracketOpen.ItemsSource = new String[] { "", "(", "((" };
            this.comboBoxBracketClose.ItemsSource = new String[] { "", ")", "))" };
            
            InitializeHandlers();
        }

        public void CustomizeTargetView() 
        {
            this.comboBoxBracketOpen.Visibility = System.Windows.Visibility.Visible;
            this.comboBoxBracketClose.Visibility = System.Windows.Visibility.Visible;
        }

        public ScopeItemPanel(bool isCustomized) :this()
        {
            this.isCustomize = isCustomize;
            this.valueCol.Width = new GridLength(80);
        }

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public ScopeItemPanel(int index) : this()
        {
            this.Index = index;
        }

        public ScopeItemPanel(int index, bool isCustomize):this()
        {
            this.isCustomize = isCustomize;
            this.Index = index;
        }

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TargetItem à afficher</param>
        public ScopeItemPanel(TargetItem item) : this()
        {
            Display(item); 
        }

        public ScopeItemPanel(TargetItem item, bool isCustomized) :this()
        {
            Display(item, isCustomized);
        }

        #endregion


        #region Properties

        private int index;

        public TargetItem TargetItem { get; set; }

        public int Index 
        { 
            get { return index; } 
            set 
            { 
                index = value; 
                this.Label.Content = "Value " + index;
                this.ComboBox.IsEnabled = index > 1;
                this.comboBoxBracketClose.Visibility = isCustomize ? Visibility.Visible : Visibility.Collapsed;
                this.comboBoxBracketOpen.Visibility = isCustomize ? Visibility.Visible : Visibility.Collapsed;
            } 
        }

        public Label Label { get {return this.label;} }
        public Button Button { get { return this.button; } }
        public ComboBox ComboBox { get { return this.comboBox; } }
        public ComboBox ComboBoxOpenPar { get { return this.comboBoxBracketOpen; } }
        public ComboBox ComboBoxClosePar { get { return this.comboBoxBracketClose; } }
        public TextBox TextBox { get { return this.textBox; } }

        #endregion


        #region Operations

        public void Display(TargetItem item, bool isCustomize =false)
        {
            update = false;
            this.TargetItem = item;
            this.isCustomize = isCustomize;
            if (item != null) this.Index = item.position + 1;
            this.TextBox.Text = item != null && item.value != null ? item.value.name : "";
            this.ComboBox.SelectedItem = item != null && item.operatorType != null ? item.operatorType : TargetItem.Operator.AND.ToString();
            if (isCustomize)
            {
                this.valueCol.Width = new GridLength(80);
                this.ComboBoxClosePar.SelectedItem = item != null && item.closingBracket != null ? item.closingBracket : "";
                this.ComboBoxOpenPar.SelectedItem = item != null && item.openingBracket != null ? item.openingBracket : "";
            }
            update = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetValue(Target value)
        {
            bool added = false;
            if (this.TargetItem == null) 
            { 
                this.TargetItem = new TargetItem(Index - 1);
                added = true; 
            }

            this.TargetItem.value = value;
            this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();
            this.TextBox.Text = value != null ? value.name : "";
            if (Added != null && added) Added(this);

            if (Updated != null && !added) Updated(this);

            //if (Changed != null) Changed();
        }

        public void SetReadOnly(bool readOnly)
        {
            if (readOnly)
            {
                this.comboBoxBracketClose.IsEnabled = !readOnly;
                this.comboBoxBracketOpen.IsEnabled = !readOnly;
                this.button.Visibility = readOnly ? Visibility.Collapsed : Visibility.Visible;
                this.ComboBoxOpenPar.IsEnabled = !readOnly;
                this.textBox.IsEnabled = !readOnly;
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
            this.Button.GotFocus += OnGotFocus;
            this.TextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;
            this.ComboBox.SelectionChanged += OnOperatorChanged;
            this.ComboBoxClosePar.SelectionChanged += OnCloseParChanged;
            this.ComboBoxOpenPar.SelectionChanged += OnOpenParChanged;
        }

        private void OnOpenParChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.TargetItem != null)
                {
                    this.TargetItem.openingBracket = this.ComboBoxOpenPar.SelectedItem.ToString();
                    Updated(this);
                }
            }
        }

        private void OnCloseParChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.TargetItem != null)
                {
                    this.TargetItem.closingBracket = this.ComboBoxClosePar.SelectedItem.ToString();
                    Updated(this);
                }
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
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



       
    }
}
