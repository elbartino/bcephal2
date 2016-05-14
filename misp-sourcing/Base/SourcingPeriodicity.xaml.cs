using Misp.Kernel.Domain;
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

namespace Misp.Sourcing.Base
{
    /// <summary>
    /// Interaction logic for periodicity.xaml
    /// </summary>
    public partial class SourcingPeriodicity : StackPanel
    {

        public event Kernel.Ui.Base.ChangeEventHandler Change;

        public event Kernel.Ui.Base.ValidateFormulaEventHandler ValidateFromFormula;
        public event Kernel.Ui.Base.ValidateFormulaEventHandler ValidateToFormula;

        public event OnDateChangeEventHandler OnDateChange;
        public delegate void OnDateChangeEventHandler(string dateFrom, string dateTo);
        public bool tryToValidate = true;

        public bool IsExpanded { get; set; }
        
        public SourcingPeriodicity()
        {
            InitializeComponent();
            InitializeHandlers();
            fromFormatComboBox.ItemsSource = DateFormat.Formats;
            toFormatComboBox.ItemsSource = DateFormat.Formats;
            IsExpanded = true;
            Expand(false);
         }

        public string GetDateFromAsString()
        {
            string format = "{0:dd-MM-yyyy}";
            DateTime? from = dateFrom.SelectedDate;
            //return from != null && from.HasValue ? String.Format(format, from.Value) : null;
            return from != null && from.HasValue ? from.Value.ToShortDateString() : null;
        }

        public string GetDateToAsString()
        {
            string format = "{0:dd-MM-yyyy}";
            DateTime? to = dateTo.SelectedDate;
            //return to != null && to.HasValue ? String.Format(format, to.Value) : null;
            return to != null && to.HasValue ? to.Value.ToShortDateString() : null;
        }

        public void DisplayPeriodForm(string value, string formuala, string format)
        {
            tryToValidate = false;
            dateFrom.SelectedDate = null;
            fromValueTextBox.Text = value != null ? value : "";
            fromFormulaTextBox.Text = formuala != null ? formuala : "";
            fromFormatComboBox.SelectedItem = format != null ? format : "";
            tryToValidate = true;
        }

        public void DisplayPeriodForm(DateTime date, string value, string formuala, string format)
        {
            DisplayPeriodForm(value, formuala, format);
            tryToValidate = false;
            dateFrom.SelectedDate = date;            
            tryToValidate = true;
        }

        public void DisplayPeriodTo(string value, string formuala, string format)
        {
            tryToValidate = false;
            dateTo.SelectedDate = null;
            toValueTextBox.Text = value != null ? value : "";
            toFormulaTextBox.Text = formuala != null ? formuala : "";
            toFormatComboBox.SelectedItem = format != null ? format : "";
            tryToValidate = true;
        }

        public void DisplayPeriodTo(DateTime date, string value, string formuala, string format)
        {
            DisplayPeriodTo(value, formuala, format);
            tryToValidate = false;
            dateTo.SelectedDate = date;
            tryToValidate = true;
        }

        public void SetPeriod(Kernel.Domain.PeriodInterval period)
        {
            tryToValidate = false;
            dateFrom.SelectedDate = period.periodFromDateTime;
            fromValueTextBox.Text = period.periodFromDateTime.ToShortDateString();
            fromFormulaTextBox.Text = "";

            dateTo.SelectedDate = period.periodToDateTime;
            toValueTextBox.Text = period.periodToDateTime.ToShortDateString();
            toFormulaTextBox.Text = "";
            tryToValidate = true;
            if (Change != null) Change();
        }

        public void Expand(bool expand)
        {
            if (expand) return;

            if (IsExpanded == expand) return;
            IsExpanded = expand;
            if (expand)
            {
                FromFormulaCol.Width = new GridLength(1, GridUnitType.Star);
                FromValueCol.Width = new GridLength(2, GridUnitType.Star);
                FromFormatCol.Width = new GridLength(1, GridUnitType.Star);
                ToFormulaCol.Width = new GridLength(1, GridUnitType.Star);
                ToValueCol.Width = new GridLength(2, GridUnitType.Star);
                ToFormatCol.Width = new GridLength(1, GridUnitType.Star);
                hearderGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                FromFormulaCol.Width = new GridLength(0, GridUnitType.Star);
                FromValueCol.Width = new GridLength(0, GridUnitType.Star);
                FromFormatCol.Width = new GridLength(0, GridUnitType.Star);
                ToFormulaCol.Width = new GridLength(0, GridUnitType.Star);
                ToValueCol.Width = new GridLength(0, GridUnitType.Star);
                ToFormatCol.Width = new GridLength(0, GridUnitType.Star);
                hearderGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        
        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected void InitializeHandlers()
        {
            dateFrom.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(onFromDateChanged);
            this.fromFormulaTextBox.KeyDown += OnValidateFromFormula;
            this.toFormulaTextBox.KeyDown += OnValidateToFormula;
        }

        private void OnValidateFromFormula(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ValidateFromFormula != null) ValidateFromFormula(this);
        }

        private void OnValidateToFormula(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ValidateToFormula != null) ValidateToFormula(this);
        }

        /// <summary>
        /// Cette methode est exécutée lorsque l'utilisateur change la date de début.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void onFromDateChanged(Object sender, SelectionChangedEventArgs args)
        {
            DateTime? from = dateFrom.SelectedDate;
            DateTime? to = dateTo.SelectedDate;
            if (from.HasValue && !to.HasValue)
            {
                dateTo.SelectedDate = from;
            }
        }


        public bool validateEdition(DateTime? FromDate, DateTime? ToDate)
        {
            DateTime? from = FromDate;
            DateTime? to = ToDate;
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong Periodicity", "From date can't be greater than to date!");
                return false;
            }
            return true;
        }
  
        public bool isDateCorrect() 
        {
            return validateEdition(this.dateFrom.SelectedDate,this.dateTo.SelectedDate);
        }

        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tryToValidate)
            {
                if (!this.isDateCorrect()) return;
                toValueTextBox.Text = this.GetDateToAsString();
                toFormulaTextBox.Text = "";
                if (Change != null)
                {
                    Change();
                }
                if (OnDateChange != null)
                    OnDateChange(this.GetDateFromAsString(), this.GetDateToAsString());
            }
        }

        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tryToValidate)
            {
                if (!this.isDateCorrect()) return;
                fromValueTextBox.Text = this.GetDateFromAsString();
                fromFormulaTextBox.Text = "";
                if (Change != null)
                {
                    Change();
                }
                if (OnDateChange != null)
                    OnDateChange(this.GetDateFromAsString(), this.GetDateToAsString());
            }
        }
    }
}
