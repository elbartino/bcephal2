using Misp.Kernel.Domain;
using Misp.Kernel.Application;
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
using Misp.Kernel.Util;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for RPeriodItemPanel.xaml
    /// </summary>
    public partial class RPeriodItemPanel : UserControl
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

        public event ValidateFormulaEventHandler ValidateFormula;

        #endregion


        #region Constructor
        
        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        public RPeriodItemPanel()
        {
            throwevent = false;
            InitializeComponent();
            
            this.SignComboBox.ItemsSource = new String[] { DateOperator.EQUALS.sign, DateOperator.BEFORE.sign, 
                DateOperator.BEFORE_OR_EQUALS.sign, DateOperator.AFTER.sign, DateOperator.AFTER_OR_EQUALS.sign};
            this.SignComboBox.SelectedItem = DateOperator.EQUALS.sign;

            this.operationComboBox.ItemsSource = new String[] { Operation.PLUS.sign,Operation.SUB.sign };
            this.operationComboBox.SelectedItem = Operation.PLUS.sign;

            this.granulartityComBox.ItemsSource = new String[]        
            {
                Granularity.WEEK.name,
                Granularity.MONTH.name,
                Granularity.YEAR.name,
                Granularity.DAY.name,
            };
            this.granulartityComBox.SelectedItem = Granularity.WEEK.name;

            InitializeHandlers();
            throwevent = true;
        }

        /// <summary>
        /// Construit une nouvelle instance de PeriodItemPnel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public RPeriodItemPanel(int index) : this()
        {
            this.Index = index;
        }

        /// <summary>
        /// Construit une nouvelle instance de PeriodItemPnel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">PeriodItem à afficher</param>
        public RPeriodItemPanel(PeriodItem item) : this()
        {
            Display(item);
        }
        

        #endregion


        #region Properties
        
        public PeriodItem PeriodItem { get; set; }

        public int Index { get; set; }

        #endregion


        #region Operations

        public bool update = true;
        public bool throwevent = true;

        public void Display(PeriodItem item)
        {
            update = false;
            throwevent = false;
            bool enableForLoop = item.loop != null;
            if (enableForLoop)
            {
                this.SignComboBox.Width = 100;
                this.SignComboBox.ItemsSource = new String[] { 
                DateOperator.DURING_PERIOD.sign, DateOperator.BEFORE_START_PERIOD.sign, DateOperator.BEFORE_END_PERIOD.sign,
                DateOperator.AFTER_START_PERIOD.sign, DateOperator.AFTER_END_PERIOD.sign};
                this.SignComboBox.SelectedItem = DateOperator.DURING_PERIOD.sign;

                this.Label.Foreground = Brushes.Red;
                this.FormulaTextBox.IsEnabled = !enableForLoop;
                this.ValueDatePicker.IsEnabled = !enableForLoop;
                this.SignComboBox.IsEnabled = true;
            }

            this.PeriodItem = item;
            if (item == null) return;
            this.Index = item.position + 1;
            this.Label.Content = item.name != null ? item.name : "";
            if (item.value != null) this.ValueDatePicker.SelectedDate = item.valueDateTime;
            this.FormulaTextBox.Text = item.formula != null ? item.formula : "";
            if (!String.IsNullOrEmpty(item.operatorSign)) this.SignComboBox.SelectedItem = item.operatorSign;
            if (!String.IsNullOrEmpty(item.operationGranularity)) this.granulartityComBox.SelectedItem = item.operationGranularity;
            if (!String.IsNullOrEmpty(item.operation)) this.operationComboBox.SelectedItem = item.operation;
            if (!String.IsNullOrEmpty(item.operationNumber))
            {
                int number;
                if (!TagFormulaUtil.isFormula(item.operationNumber))
                {
                    bool ok = int.TryParse(item.operationNumber, out number);
                    item.operationNumber = (ok && number > 0) ? "" + number : "";
                }
                this.numberValueTextBox.Text = item.operationNumber;
            }
            
            update = true;
            throwevent = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cours d'édition</param>
        public void SetValue(PeriodItem item)
        {
            bool added = false;
            update = false;
            if (this.PeriodItem == null)
            {
                this.PeriodItem = new PeriodItem(Index - 1);                
                added = true;
            }
            this.PeriodItem.name = item.name;
            this.PeriodItem.value = item.value;
            this.PeriodItem.formula = item.formula;
            this.PeriodItem.period = item.period;
            this.PeriodItem.operationDate = item.operationDate;
            this.PeriodItem.operationGranularity = item.operationGranularity;

            this.Label.Content = this.PeriodItem != null ? this.PeriodItem.name : "";
            DateTime result;
            if (this.PeriodItem != null && DateTime.TryParse(this.PeriodItem.value,out result)) this.ValueDatePicker.SelectedDate = this.PeriodItem.valueDateTime;
            this.FormulaTextBox.Text = this.PeriodItem != null ? this.PeriodItem.formula : "";
            this.SignComboBox.SelectedItem = this.PeriodItem != null ? this.PeriodItem.operatorSign : DateOperator.EQUALS.sign;
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }


        public void SetLoop(TransformationTreeItem loop)
        {
            bool added = false;
            if (loop == null) return;
            if (this.PeriodItem == null)
            {
                this.PeriodItem = new PeriodItem(Index - 1);
                added = true;
            }

            this.PeriodItem.operatorSign = this.SignComboBox.SelectedItem.ToString();
            this.PeriodItem.period =null;
            this.PeriodItem.operationDate = "";
            this.PeriodItem.operationGranularity = "";
            this.PeriodItem.value = null;
            this.PeriodItem.formula = loop.name;
            this.PeriodItem.loop = loop;
            this.PeriodItem.name = "";

            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetPeriodName(string name)
        {
            bool added = false;
            if (this.PeriodItem == null)
            {
                this.PeriodItem = new PeriodItem(Index - 1);
                added = true;
            }
            this.PeriodItem.name = name;
            this.Label.Content = name;
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }

        public void fillItem()
        {
            if (this.PeriodItem == null) this.PeriodItem = new PeriodItem(Index - 1); 
            this.PeriodItem.operationNumber = numberValueTextBox.Text.Trim().ToUpper();
            this.PeriodItem.operationGranularity = granulartityComBox.SelectedItem.ToString();
            this.PeriodItem.operation = operationComboBox.SelectedItem.ToString();
            this.PeriodItem.operatorSign = this.SignComboBox.SelectedItem.ToString();
            this.PeriodItem.value = this.ValueDatePicker.SelectedDate.HasValue ? this.ValueDatePicker.SelectedDate.Value.ToShortDateString() : null;
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.Button.Click += OnButtonClick;
            this.FormulaTextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;

            this.SignComboBox.SelectionChanged += OnOperatorChanged;

            this.PreviewMouseDown += OnMouseDown;
            this.ValueDatePicker.PreviewMouseDown += OnMouseDown;
            this.FormulaTextBox.PreviewMouseDown += OnMouseDown;

            this.granulartityComBox.PreviewMouseDown += OnMouseDown;
            this.numberValueTextBox.PreviewMouseDown += OnMouseDown;
            this.operationComboBox.PreviewMouseDown += OnMouseDown;

            this.granulartityComBox.GotFocus += OnGotFocus;
            this.numberValueTextBox.GotFocus += OnGotFocus;
            this.operationComboBox.GotFocus += OnGotFocus;

            this.FormulaTextBox.KeyDown += OnValidateFormula;
            this.ValueDatePicker.SelectedDateChanged += OnValueSelectedDateChanged;
            this.granulartityComBox.SelectionChanged += OnGranularityChanged;
            this.operationComboBox.SelectionChanged += OnOperationChanged;
            this.numberValueTextBox.KeyDown += OnValidateNumber;
         //   this.numberValueTextBox.LostFocus += OnLostFocus;
          //  this.numberValueTextBox.MouseLeave += OnMouseLeave;
        }
             
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ValidateNumber(Key.None);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ValidateNumber(Key.None);
        }

        private void ValidateNumber(Key key)
        {
            if (Updated != null && update)
            {
                if (this.PeriodItem != null)
                {
                    this.PeriodItem.operationNumber = numberValueTextBox.Text.Trim().ToUpper();
                    this.PeriodItem.operationGranularity = granulartityComBox.SelectedItem.ToString();
                    this.PeriodItem.operation = operationComboBox.SelectedItem.ToString();
                    if (key == Key.None && ValidateFormula != null) ValidateFormula(this);
                    if (key == Key.Enter && ValidateFormula != null) ValidateFormula(this);
                }
            }
        }

        private void OnValidateNumber(object sender, KeyEventArgs e)
        {
            ValidateNumber(e.Key);
        }

      


        private void OnOperationChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!throwevent) return;
            if (this.PeriodItem == null) this.PeriodItem = new PeriodItem(Index - 1);
            this.PeriodItem.operationNumber = numberValueTextBox.Text.Trim().ToUpper();
            this.PeriodItem.operationGranularity = granulartityComBox.SelectedItem.ToString();
            this.PeriodItem.operation = operationComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(this.numberValueTextBox.Text))
            {
                bool isValid = TagFormulaUtil.isSyntaxeFormulaCorrectly(this.numberValueTextBox.Text);
                if (!isValid)
                {
                    int result;
                    isValid = int.TryParse(this.numberValueTextBox.Text, out result);
                    isValid = isValid ? result > 0 : false;
                }
                this.PeriodItem.operationNumber = isValid ? this.numberValueTextBox.Text : "";
            }
            if (Updated != null && update) Updated(this);
        }

        private void OnGranularityChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!throwevent) return;
            if (this.PeriodItem == null) this.PeriodItem = new PeriodItem(Index - 1);
            this.PeriodItem.operationNumber = numberValueTextBox.Text.Trim().ToUpper();
            this.PeriodItem.operationGranularity = granulartityComBox.SelectedItem.ToString();
            this.PeriodItem.operation = operationComboBox.SelectedItem.ToString();
            if (Updated != null && update) Updated(this);          
        }

        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
            if (!throwevent) return;
            if (this.PeriodItem == null) this.PeriodItem = new PeriodItem(Index - 1);
            this.PeriodItem.operatorSign = this.SignComboBox.SelectedItem.ToString();
            if (Updated != null && update) Updated(this);
        }

        private void OnValueSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!throwevent) return;
            bool added = false;
            if (this.ValueDatePicker.SelectedDate.HasValue)
            {
                if (this.PeriodItem == null)
                {
                    this.PeriodItem = new PeriodItem(Index - 1);
                    added = true;
                }
                this.PeriodItem.value = this.ValueDatePicker.SelectedDate.Value.ToShortDateString();
                this.PeriodItem.operationGranularity = this.granulartityComBox.SelectedItem.ToString();
                if (string.IsNullOrEmpty(this.PeriodItem.operationDate))
                {
                    String sign = this.SignComboBox.SelectedItem.ToString();
                    if (DateOperator.getBySign(sign) != null) this.PeriodItem.operationDate = DateOperator.getBySign(sign).name;
                }
                if (!string.IsNullOrEmpty(this.numberValueTextBox.Text))
                {
                    bool isValid = TagFormulaUtil.isSyntaxeFormulaCorrectly(this.numberValueTextBox.Text);
                    if (!isValid)
                    {
                        int result;
                        isValid = int.TryParse(this.numberValueTextBox.Text, out result);
                    }
                    this.PeriodItem.operationNumber = isValid ? this.numberValueTextBox.Text : "";
                }
                this.PeriodItem.operation = this.operationComboBox.SelectedItem.ToString();
            }
            else this.PeriodItem.value = null;
            if (Added != null && added && update) Added(this);
            else if (Updated != null && update) Updated(this);  
        }
        
        private void OnValidateFormula(object sender, KeyEventArgs e)
        {
            if (!throwevent) return;
            if (this.PeriodItem == null) this.PeriodItem = new PeriodItem(Index - 1);
            if (ValueDatePicker.SelectedDate.HasValue)
            {
                this.PeriodItem.valueDateTime = ValueDatePicker.SelectedDate.Value;
            }

            this.PeriodItem.operationGranularity = this.granulartityComBox.SelectedItem.ToString();
            if (string.IsNullOrEmpty(this.PeriodItem.operationDate))
            {
                String sign = this.SignComboBox.SelectedItem.ToString();
                if (DateOperator.getBySign(sign) != null) this.PeriodItem.operationDate = DateOperator.getBySign(sign).name;
            }
            if (!string.IsNullOrEmpty(this.numberValueTextBox.Text))
            {
                bool isValid = TagFormulaUtil.isSyntaxeFormulaCorrectly(this.numberValueTextBox.Text);
                if (!isValid)
                {
                    int result;
                    isValid = int.TryParse(this.numberValueTextBox.Text, out result);
                }
                this.PeriodItem.operationNumber = isValid ? this.numberValueTextBox.Text : "";
            }
            this.PeriodItem.operation = this.operationComboBox.SelectedItem.ToString();

            this.PeriodItem.formula = FormulaTextBox.Text.Trim().ToUpper();
            if (e.Key == Key.Enter && ValidateFormula != null && update) ValidateFormula(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
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
            this.SignComboBox.IsEnabled = !readOnly;
            this.FormulaTextBox.IsReadOnly = readOnly;
            this.numberValueTextBox.IsReadOnly = readOnly;
            this.operationComboBox.IsEnabled = !readOnly;
            this.ValueDatePicker.IsEnabled = !readOnly;
            this.Button.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}