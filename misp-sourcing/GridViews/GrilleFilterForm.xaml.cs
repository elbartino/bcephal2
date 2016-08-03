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

namespace Misp.Sourcing.GridViews
{
    /// <summary>
    /// Interaction logic for GrilleFilterForm.xaml
    /// </summary>
    public partial class GrilleFilterForm : Grid
    {

        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;
        public bool thrawChange = true;
        public GrilleFilter GrilleFilter { get; set; }

        public GrilleFilterForm()
        {
            InitializeComponent();
            this.periodFilter.CustomizeForReport();
            this.periodFilter.NewPeriodTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            reset();
            IntializeHandlers();     
        }
        
        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            this.resetButton.Click += OnReset;
            this.targetFilter.Changed += OnChange;
            this.periodFilter.Changed += OnChange;

            this.creditCheckBox.Checked += OnChange;
            this.creditCheckBox.Unchecked += OnChange;
            this.debitCheckBox.Checked += OnChange;
            this.debitCheckBox.Unchecked += OnChange;
            this.includeRecoCheckBox.Checked += OnChange;
            this.includeRecoCheckBox.Unchecked += OnChange;
        }

                        
        /// <summary>
        /// display filter scope and period 
        /// 
        /// </summary>
        /// <param name="bankReco"></param>
        public void Display(GrilleFilter filter)
        {
            this.GrilleFilter = filter;
            thrawChange = false;
            reset();
            if (filter != null)
            {
                targetFilter.DisplayScope(filter.filterScope);
                periodFilter.DisplayPeriod(filter.filterPeriod);
                this.creditCheckBox.IsChecked = filter.creditChecked;
                this.debitCheckBox.IsChecked = filter.debitChecked;
                this.includeRecoCheckBox.IsChecked = filter.includeRecoChecked;
            }
            thrawChange = true;
        }

        /// <summary>
        /// fill object
        /// </summary>
        /// <param name="bankReco"></param>
        public GrilleFilter Fill()
        {
            if (this.GrilleFilter == null) this.GrilleFilter = new GrilleFilter();
            this.GrilleFilter.filterScope = targetFilter.Scope;
            this.GrilleFilter.filterPeriod = periodFilter.Period;
            this.GrilleFilter.creditChecked = this.creditCheckBox.IsChecked.Value;
            this.GrilleFilter.debitChecked = this.debitCheckBox.IsChecked.Value;
            this.GrilleFilter.includeRecoChecked = this.includeRecoCheckBox.IsChecked.Value;
            return this.GrilleFilter;
        }

        /// <summary>
        /// clear object
        /// </summary>
        /// <param name="bankReco"></param>
        public void reset()
        {
            targetFilter.DisplayScope(null);
            periodFilter.DisplayPeriod(null);
        }
        

        public void OnChange()
        {
            if (ChangeHandler != null && thrawChange) ChangeHandler();
        }

        /// <summary>
        /// implement action on creditCheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            reset();
        }

    }
}
