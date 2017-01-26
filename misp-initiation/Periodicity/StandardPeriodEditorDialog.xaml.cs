using Misp.Kernel.Domain;
using Misp.Kernel.Util;
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

namespace Misp.Initiation.Periodicity
{
    /// <summary>
    /// Interaction logic for StandardPeriodEditorDialog.xaml
    /// </summary>
    public partial class StandardPeriodEditorDialog : Window
    {
        public event ValidateChangeEventHandler OnValidateChange;
        public delegate void ValidateChangeEventHandler(object item, bool cancel);        
        public PeriodName periodName;
        public bool tryToValidate = false;

        public StandardPeriodEditorDialog()
        {
            InitializeComponent();
            this.Owner = Misp.Kernel.Application.ApplicationManager.Instance.MainWindow;
            this.periodNameTextBox.IsEnabled = false;
            initializeViewHandler();
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void DisplayObject()
        {
            tryToValidate = false;
            Reset();
            if (periodName == null) { tryToValidate = true; return; }
            this.periodNameTextBox.Text = periodName.name;
            this.standardPeriodNameTextBox.Text = periodName.getNewGroupName();
            periodName.curentIntervalGroupName = this.standardPeriodNameTextBox.Text; 
            this.periodNbrTextBox.Text = "" + (periodName.incrementationCount > 0 ? periodName.incrementationCount : 1);
            if (!String.IsNullOrEmpty(periodName.periodFrom)) this.fromDateBox.SelectedDate = periodName.periodFromTime;
            if (!String.IsNullOrEmpty(periodName.periodTo)) this.toDateBox.SelectedDate = periodName.periodToTime;

            if (periodName.isYear()) this.yearButton.IsChecked = true;
            this.MonthButton.IsChecked = periodName.isMonth();
            this.dayButton.IsChecked = periodName.isDay();
            this.weekButton.IsChecked = periodName.isWeek();

            if (periodName.showDay) this.showDay.IsChecked = periodName.showDay;
            this.showMonth.IsChecked = periodName.showMonth;
            this.showWeek.IsChecked = periodName.showWeek;
            this.showYear.IsChecked = periodName.showYear;
            EnableDisableShowBoxes();
            tryToValidate = true;
            RefreshPeriodsGrid();            
        }

        public void Reset()
        {
            this.periodNameTextBox.Text = "";
            this.standardPeriodNameTextBox.Text = "";
            this.fromDateBox.SelectedDate = null;
            this.toDateBox.SelectedDate = null;            
            this.periodNbrTextBox.Text = "1";
            this.yearButton.IsChecked = true;
            this.MonthButton.IsChecked = false;
            this.dayButton.IsChecked = false;
            this.weekButton.IsChecked = false;
            this.showYear.IsChecked = true;
            this.showDay.IsChecked = false;
            this.showMonth.IsChecked = false;
            this.showWeek.IsChecked = false;
            periodGrid.ItemsSource = null;
        }

        public void FillObject(PeriodName periodName)
        {
            periodName.name = this.periodNameTextBox.Text.Trim();
            periodName.curentIntervalGroupName = this.standardPeriodNameTextBox.Text.Trim();
            periodName.granularity = GetGranularity();
            periodName.incrementationCount = GetPeriodIncrement();
            periodName.showYear = this.showYear.IsChecked.Value;
            periodName.showMonth = this.showMonth.IsChecked.Value;
            periodName.showWeek = this.showWeek.IsChecked.Value;
            periodName.showDay = this.showDay.IsChecked.Value;
            if (this.fromDateBox.SelectedDate.HasValue) periodName.periodFromTime = this.fromDateBox.SelectedDate.Value;
            else periodName.periodFrom = null;
            if (this.toDateBox.SelectedDate.HasValue) periodName.periodToTime = this.toDateBox.SelectedDate.Value;
            else periodName.periodTo = null;
            periodName.incrementationCount = GetPeriodIncrement();
        }

        public bool ValidateEdition()
        {
            if (string.IsNullOrEmpty(this.periodNameTextBox.Text.Trim()))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong Name", "Period name can't be empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.standardPeriodNameTextBox.Text.Trim()))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong Name", "standard Period name can't be empty!");
                return false;
            }
            if (!isDateCorrect()) return false;

            int increment;
            bool ok = int.TryParse(this.periodNbrTextBox.Text.Trim(), out increment);
            if (!ok || increment <= 0)
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong value", "The value of number of period between two periods is wrong!");
                return false;
            }
            if (increment <= 0)
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong value", "The value of number of period between two periods can't be negative!");
                return false;
            }
            return true;
        }

        protected String GetGranularity()
        {
            if (this.yearButton.IsChecked.Value) return Granularity.YEAR.name;
            if (this.MonthButton.IsChecked.Value) return Granularity.MONTH.name;
            if (this.weekButton.IsChecked.Value) return Granularity.WEEK.name;
            if (this.dayButton.IsChecked.Value) return Granularity.DAY.name;
            return Granularity.YEAR.name;
        }

        protected int GetPeriodIncrement()
        {
            if (String.IsNullOrEmpty(this.periodNbrTextBox.Text.Trim())) return 1;
            int increment;
            bool ok = int.TryParse(this.periodNbrTextBox.Text.Trim(), out increment);
            if (!ok || increment <= 0) return 1;
            return increment;
        }

        private bool canBuildPeriodsGrid()
        {
            if (this.fromDateBox.SelectedDate == null) return false;
            if (this.toDateBox.SelectedDate == null) return false;
            return true;
        }

        private void RefreshPeriodsGrid()
        {
            periodGrid.ItemsSource = null;
            if (this.periodName == null) return;
            PeriodName item = new PeriodName();
            FillObject(item);
            if (!canBuildPeriodsGrid())
            {
                noResultLabel.Visibility = System.Windows.Visibility.Visible;
                periodGrid.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (!periodGridExpander.IsExpanded) return;
            noResultLabel.Visibility = System.Windows.Visibility.Collapsed;
            periodGrid.Visibility = System.Windows.Visibility.Visible;
            BuildPeriods(item);
            periodGrid.ItemsSource = item.GetRootPeriodInterval().Leafs;
        }

        private void BuildPeriods(PeriodName item)
        {
            if (item == null) return;
            item.GetRootPeriodInterval().childrenListChangeHandler.originalList = PeriodNameUtil.buildIntervals(item);            
        }

        private void initializeViewHandler()
        {
            this.OkButton.Click += OnOkButtonClick;
            this.CancelButton.Click += OnCancelButtonClick;
            
            this.fromDateBox.SelectedDateChanged += OnFromDateChanged;
            this.toDateBox.SelectedDateChanged += OnToDateChanged;
            
            this.MonthButton.Checked += OnChooseGranularity;
            this.yearButton.Checked += OnChooseGranularity;
            this.dayButton.Checked += OnChooseGranularity;
            this.weekButton.Checked += OnChooseGranularity;

            this.MonthButton.Unchecked += OnChooseGranularity;
            this.yearButton.Unchecked += OnChooseGranularity;
            this.dayButton.Unchecked += OnChooseGranularity;
            this.weekButton.Unchecked += OnChooseGranularity;
            
            this.periodNbrTextBox.KeyUp += OnValidatePeriodIncrement;
            this.periodGridExpander.Expanded += OnPeriodGridExpanded;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.periodName == null) this.periodName = new PeriodName();
             
             DateTime? from = this.fromDateBox.SelectedDate;
            DateTime? to = this.toDateBox.SelectedDate;
            if (from==null || to == null)
                return;
            if (!ValidateEdition()) return;
            PeriodName item = new PeriodName();
            FillObject(item);
            BuildPeriods(item);
            this.periodName.Update(item);
            if (OnValidateChange != null) OnValidateChange(this.periodName, true);
            this.Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (OnValidateChange != null) OnValidateChange(null, false);
            this.Close();
        }
        
        private void OnPeriodGridExpanded(object sender, RoutedEventArgs e)
        {
            RefreshPeriodsGrid();
        }

        private void OnValidatePeriodIncrement(object sender, KeyEventArgs args)
        {
            if (!tryToValidate) return;
            if (args.Key == Key.Enter) RefreshPeriodsGrid();
        }
        
        private void OnChooseGranularity(object sender, RoutedEventArgs e)
        {
            if (!tryToValidate) return;            
            EnableDisableShowBoxes();
            RefreshPeriodsGrid();
        }

        private void EnableDisableShowBoxes()
        {
            if (this.dayButton.IsChecked.Value)
            {
                this.showMonth.IsEnabled = true;
                this.showYear.IsEnabled = true;
                this.showWeek.IsEnabled = true;
                this.showDay.IsEnabled = true;

                this.showMonth.IsChecked = true;
                this.showYear.IsChecked = true;
                this.showWeek.IsChecked = true;
                this.showDay.IsChecked = true;
            }
            else if (this.weekButton.IsChecked.Value)
            {
                this.showYear.IsEnabled = true;
                this.showMonth.IsEnabled = true;
                this.showWeek.IsEnabled = true;
                this.showDay.IsEnabled = false;

                this.showYear.IsChecked = true;
                this.showMonth.IsChecked = true;                
                this.showWeek.IsChecked = true;
                this.showDay.IsChecked = false;
            }
            else if (this.MonthButton.IsChecked.Value)
            {
                this.showYear.IsEnabled = true;
                this.showMonth.IsEnabled = true;
                this.showWeek.IsEnabled = false;
                this.showDay.IsEnabled = false;

                this.showYear.IsChecked = true;
                this.showMonth.IsChecked = true;
                this.showWeek.IsChecked = false;
                this.showDay.IsChecked = false;
            }
            else if (this.yearButton.IsChecked.Value)
            {
                this.showYear.IsEnabled = true;
                this.showMonth.IsEnabled = false;
                this.showWeek.IsEnabled = false;
                this.showDay.IsEnabled = false;

                this.showYear.IsChecked = true;
                this.showMonth.IsChecked = false;
                this.showWeek.IsChecked = false;
                this.showDay.IsChecked = false;
            }   
        }

        public bool validateDate(DateTime? FromDate, DateTime? ToDate)
        {
            DateTime? from = FromDate;
            DateTime? to = ToDate;
        
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                Kernel.Util.MessageDisplayer.DisplayError("Wrong Periodicity", "From date can't be greater than to date!");
                return false;
            }
          
            if ((from == null || from.Value == null) && (to == null || to.Value == null))
                return false;
            return true;
        }

        public bool isDateCorrect()
        {
            return validateDate(this.fromDateBox.SelectedDate, this.toDateBox.SelectedDate);
        }

        private void OnToDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!tryToValidate) return;
            if (!isDateCorrect())
            {
                this.toDateBox.SelectedDate = null;
                return;
            }
            RefreshPeriodsGrid();
        }

        private void OnFromDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!tryToValidate) return;
            if (!isDateCorrect())
            {
                this.fromDateBox.SelectedDate = null;
                return;
            }
            RefreshPeriodsGrid();            
        }
                
    }
}
