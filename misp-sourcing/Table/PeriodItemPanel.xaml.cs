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
    /// Interaction logic for PeriodItemPanel.xaml
    /// </summary>
    public partial class PeriodItemPanel : UserControl
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

        protected bool forReport = false;

        public bool isTableView = false;
        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        public PeriodItemPanel()
        {
            InitializeComponent();
            this.SignComboBox.ItemsSource = new String[] { DateOperator.EQUALS.sign, DateOperator.BEFORE.sign, 
                DateOperator.BEFORE_OR_EQUALS.sign, DateOperator.AFTER.sign, DateOperator.AFTER_OR_EQUALS.sign};
            this.SignComboBox.SelectedItem = DateOperator.EQUALS.sign;

            this.operationComboBox.ItemsSource = new String[] 
            {
               Operation.PLUS.sign,Operation.SUB.sign     
            };

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
            OperatorCol.Width = new GridLength(0, GridUnitType.Star);
        }

        /// <summary>
        /// Construit une nouvelle instance de PeriodItemPnel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public PeriodItemPanel(int index, bool forReport,bool forAutomaticSourcing = false,bool isTableView =false) : this()
        {
            this.Index = index;
            setForReport(forReport);
            setForAutomaticSourcing(forAutomaticSourcing);
            if (!isTableView) customizeCellView();
        }

        /// <summary>
        /// Construit une nouvelle instance de PeriodItemPnel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">PeriodItem à afficher</param>
        public PeriodItemPanel(PeriodItem item, bool forReport,bool forAutomaticSourcing=false,bool isTableView = false)
            : this()
        {
            Display(item);
            setForReport(forReport,isTableView);
            setForAutomaticSourcing(forAutomaticSourcing);
            if (!isTableView) customizeCellView();
        }

        public void setForReport(bool report,bool isTableView=false)
        {
            this.forReport = report;
            if (forReport)
            {
                OperatorCol.Width = new GridLength(40);
                if (isTableView)
                {
                    ValueCol.Width = new GridLength(1.3, GridUnitType.Star);
                    ValueCol.MinWidth = 100;
                    FormulaCol.MinWidth = 60;
                    FormulaCol.Width = new GridLength(1.3, GridUnitType.Star);
                }
                else 
                {
                    ValueCol.Width = new GridLength(1.3, GridUnitType.Star);
                    ValueCol.MinWidth = 100;
                    FormulaCol.MinWidth = 50;
                    FormulaCol.Width = new GridLength(1.3, GridUnitType.Star);
                }
            }            
        }

        public void setForAutomaticSourcing(bool forAutomaticSourcing = false)
        {
            if (forAutomaticSourcing) 
            {
                this.FormulaTextBox.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void customizeCellView() 
        {
            if (!forReport)
            {
                ValueCol.Width = new GridLength(1.3, GridUnitType.Star);
                ValueCol.MinWidth = 120;
                FormulaCol.MinWidth = 60;
                FormulaCol.Width = new GridLength(1.3, GridUnitType.Star);
            }
        }

        #endregion


        #region Properties

        private int index;

        public PeriodItem PeriodItem { get; set; }

        public int Index 
        { 
            get { return index; } 
            set 
            { 
                index = value; 
                this.Label.Content = "Value " + index;
            } 
        }

        #endregion


        #region Operations

        bool update = true;
        public void Display(PeriodItem item)
        {
            update = false;
            this.PeriodItem = item;
            if (item == null) return;
            this.Index = item.position + 1;
            this.NameTextBox.Text = item.name != null ? item.name : "";
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
            bool enableForLoop = item.loop != null;
            if (enableForLoop)
            {
                this.NameTextBox.Text = item.loop.name;
                this.NameTextBox.Foreground = Brushes.Red;
                this.numberValueTextBox.IsEnabled = !enableForLoop;
                this.FormulaTextBox.IsEnabled = !enableForLoop;
                this.granulartityComBox.IsEnabled = !enableForLoop;
                this.operationComboBox.IsEnabled = !enableForLoop;
                this.ValueDatePicker.IsEnabled = !enableForLoop;
                this.SignComboBox.IsEnabled = !enableForLoop;
            }
            update = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cours d'édition</param>
        public void SetValue(PeriodItem item,bool forreport=false)
        {
            bool added = forreport;
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

            this.NameTextBox.Text = this.PeriodItem != null ? this.PeriodItem.name : "";
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
            this.NameTextBox.Text = name;
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
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
            this.NameTextBox.PreviewMouseDown += OnMouseDown;
            this.ValueDatePicker.PreviewMouseDown += OnMouseDown;
            this.FormulaTextBox.PreviewMouseDown += OnMouseDown;

            this.granulartityComBox.PreviewMouseDown += OnMouseDown;
            this.numberValueTextBox.PreviewMouseDown += OnMouseDown;
            this.operationComboBox.PreviewMouseDown += OnMouseDown;

            this.granulartityComBox.GotFocus += OnGotFocus;
            this.numberValueTextBox.GotFocus += OnGotFocus;
            this.operationComboBox.GotFocus += OnGotFocus;
            
            this.FormulaTextBox.KeyDown += OnValidateFormula;
            this.NameTextBox.TextChanged += OnPeriodNameChanged;
            this.ValueDatePicker.SelectedDateChanged += OnValueSelectedDateChanged;
            this.granulartityComBox.SelectionChanged += OnGranularityChanged;
            this.operationComboBox.SelectionChanged += OnOperationChanged;
            
            this.numberValueTextBox.KeyDown += OnValidateNumber;
         //   this.numberValueTextBox.LostFocus += OnLostFocus;
         //   this.numberValueTextBox.MouseLeave += OnMouseLeave;
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
            if (Updated != null && update)
            {
                if (this.PeriodItem != null)
                {
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
                    Updated(this);
                }
            }
        }

        private void OnGranularityChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.PeriodItem != null)
                {
                    this.PeriodItem.operationNumber = numberValueTextBox.Text.Trim().ToUpper();
                    this.PeriodItem.operationGranularity = granulartityComBox.SelectedItem.ToString();
                    this.PeriodItem.operation = operationComboBox.SelectedItem.ToString();
                    Updated(this);
                }
            }            
        }

        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.PeriodItem != null)
                {                    
                    this.PeriodItem.operatorSign = this.SignComboBox.SelectedItem.ToString();
                    Updated(this);
                }
            }
        }

        private void OnValueSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (update)
            {
                bool added = false;
                if (this.ValueDatePicker.SelectedDate.HasValue)
                {
                    if (this.PeriodItem == null)
                    {
                        this.PeriodItem = new PeriodItem(Index - 1);
                        this.PeriodItem.name = this.NameTextBox.Text;
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
                if (Updated != null && !added) Updated(this);
                if (Added != null && added) Added(this);
            }
        }


        private void OnPeriodNameChanged(object sender, TextChangedEventArgs e)
        {
            string name = this.NameTextBox.Text;
            if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                this.ValueDatePicker.IsEnabled = false;
                this.FormulaTextBox.IsEnabled = false;
                this.SignComboBox.IsEnabled = false;
            }
            else
            {
                this.ValueDatePicker.IsEnabled = true;
                this.FormulaTextBox.IsEnabled = true;
                this.SignComboBox.IsEnabled = true;
            }
        }

        private void OnValidateFormula(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ValidateFormula != null)
            {
                if (this.PeriodItem == null)
                {
                    this.PeriodItem = new PeriodItem(Index - 1);
                }
                this.PeriodItem.name = NameTextBox.Text;
                if (ValueDatePicker.SelectedDate.HasValue)
                {
                    this.PeriodItem.valueDateTime = ValueDatePicker.SelectedDate.Value;
                }

                this.PeriodItem.formula = FormulaTextBox.Text.Trim().ToUpper();
                ValidateFormula(this);
            }
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

    }
}
