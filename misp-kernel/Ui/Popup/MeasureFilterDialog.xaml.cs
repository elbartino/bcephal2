using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
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
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Popup
{
    /// <summary>
    /// Interaction logic for MeasureFilterDialog.xaml
    /// </summary>
    public partial class MeasureFilterDialog : Window
    {
        
        #region Properties

        Domain.Measure Measure { get; set; }

        public Domain.Measure Selection { get; set; }

        BrowserDataFilter Filter { get; set; }

        public MeasureService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService(); } }

        #endregion


        #region Constructors

        public MeasureFilterDialog() 
        {
            InitializeComponent();           
        }

        #endregion


        #region Operations

        public void Display(Domain.Measure measure)
        {
            this.Measure = measure;            
            Filter = new BrowserDataFilter();
            Filter.page = 0;
            Filter.pageSize = BrowserDataFilter.DEFAULT_PAGE_SIZE;
            if (measure != null)
            {
                this.Title = measure.name;
                Filter.groupOid = measure.oid.Value;
            }            
            Search();
        }

        public void Search()
        {
            this.Selection = null; 
            Filter.criteria = SearchTextBox.Text;            
            Filter.orderAsc = descButton.IsChecked.HasValue && descButton.IsChecked.Value ? false : true;
            Filter.pageSize = ShowAllChechBox.IsChecked.Value ? int.MaxValue : BrowserDataFilter.DEFAULT_PAGE_SIZE;
            BrowserDataPage<Domain.Measure> page = Service.getAllDescendents(Filter);
            this.listBox.ItemsSource = page != null ? page.rows : null;
            this.okButton.IsEnabled = Selection != null;
            SearchTextBox.Focus();
        }

        #endregion
        

        #region Handlers

        private void OnSelectValue(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                Object tag = ((RadioButton)sender).Tag;
                if (tag is int?)
                {
                    this.Selection = new Domain.Measure();
                    this.Selection.oid = (int?)tag;
                    this.Selection.name = (String)((RadioButton)sender).Content;
                    this.okButton.IsEnabled = Selection != null;
                }
            }
        }

        private void OnSelectAll(object sender, RoutedEventArgs e)
        {
            this.selectDeselectAllChechBox.Content = "Deselect All";
        }

        private void OnDeselectAll(object sender, RoutedEventArgs e)
        {
            this.selectDeselectAllChechBox.Content = "Select All";
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Selection = null;
            this.Close();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnOrdering(object sender, RoutedEventArgs e)
        {
            descButton.BorderBrush = Brushes.White;
            ascButton.BorderBrush = Brushes.White;
            Search();
        }

        private void OnShowAll(object sender, RoutedEventArgs e)
        {
            Search();
        }
        
        #endregion





    }
}
