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

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for PostingFilterForm.xaml
    /// </summary>
    public partial class PostingFilterForm : Grid
    {
        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;
        public bool thrawChange = true;

        public PostingFilter postingFilter { get; set; }

        public PostingFilterForm()
        {
            InitializeComponent();
            IntializeHandlers();           
        }

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            this.creditCheckBox.Checked += OnCreditSelectionChange;
            this.creditCheckBox.Unchecked += OnCreditSelectionChange;
            this.debitCheckBox.Checked += OnDebitSelectionChange;
            this.debitCheckBox.Unchecked += OnDebitSelectionChange;
            this.includeRecoCheckBox.Checked += OnIncluRecoSelectionChange;
            this.includeRecoCheckBox.Unchecked += OnIncluRecoSelectionChange;
            this.resetButton.Click += OnReset;
            this.filterPTForm.ChangeHandler += OnChange;
        }

        

        
                
        /// <summary>
        /// display filter scope and period 
        /// 
        /// </summary>
        /// <param name="bankReco"></param>
        public void Display(PostingFilter filter)
        {
            postingFilter = filter;
            thrawChange = false;
            reset();
            if (filter != null)
            {
                filterPTForm.Display(filter);
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
        public PostingFilter Fill()        {
            if (postingFilter == null) postingFilter = new PostingFilter();
            this.filterPTForm.Fill(postingFilter);
            postingFilter.creditChecked = this.creditCheckBox.IsChecked.Value;
            postingFilter.debitChecked = this.debitCheckBox.IsChecked.Value;
            postingFilter.includeRecoChecked = this.includeRecoCheckBox.IsChecked.Value;
            return postingFilter;
        }

        /// <summary>
        /// clear object
        /// </summary>
        /// <param name="bankReco"></param>
        public void reset()
        {
            this.filterPTForm.reset();
            this.creditCheckBox.IsChecked = false;
            this.debitCheckBox.IsChecked = false;
            this.includeRecoCheckBox.IsChecked = false;
        }
        
        /// <summary>
        /// implement action on creditCheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreditSelectionChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// implement action on debitCheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDebitSelectionChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// implement action on incluRecoCheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIncluRecoSelectionChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void OnChange()
        {
            if (ChangeHandler != null && thrawChange) ChangeHandler();
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            reset();
        }
        
    }
}
