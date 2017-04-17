using Misp.Kernel.Domain.Browser;
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

namespace Misp.Kernel.Ui.Base.BrowserUI
{
    /// <summary>
    /// Interaction logic for PaginationBar.xaml
    /// </summary>
    public partial class PaginationBar : Grid
    {
        public Kernel.Ui.Base.ChangeItemEventHandler ChangeHandler;

        public int current;
        public int total;

        protected bool throwHandler;

        public PaginationBar()
        {
            InitializeComponent();
            List<int> sizes = new List<int>(0);
            sizes.Add(5);
            sizes.Add(10);
            sizes.Add(20);
            sizes.Add(25);
            sizes.Add(30);
            sizes.Add(40);
            sizes.Add(50);
            sizes.Add(60);
            sizes.Add(100);
            this.pageSizeComboBox.ItemsSource = sizes;
            this.pageSizeComboBox.SelectedItem = BrowserDataFilter.DEFAULT_PAGE_SIZE;
            InitializeHandlers();
            current = 0;
            //this.Visibility = Visibility.Hidden;
            throwHandler = true;
        }


        public void displayPage(int pageSize, int pageFirstItem, int pageLastItem,int totalItemCount,int pageCount,int currentPage)
        {
            throwHandler = false;
            current = currentPage;
            total = pageCount;
            this.currentPage.Content = currentPage;
            this.totalPageLabel.Content = pageCount;
            this.label.Content = "" + pageFirstItem + " to " + pageLastItem + " / " + totalItemCount;

            this.firstPageButton.IsEnabled = currentPage > 1;
            this.previousPageButton.IsEnabled = currentPage > 1;

            this.nextPageButton.IsEnabled = currentPage < pageCount;
            this.LastPageButton.IsEnabled = currentPage < pageCount;

            this.firstPageButton.Foreground = this.firstPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.previousPageButton.Foreground = this.previousPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.nextPageButton.Foreground = this.nextPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.LastPageButton.Foreground = this.LastPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;

            this.pageSizeComboBox.SelectedItem = pageSize;
            this.pageSizeComboBox.IsEnabled = pageCount > 0;

            this.Visibility = pageCount > 0 ? Visibility.Visible : Visibility.Hidden;
            throwHandler = true;
        }

        private void InitializeHandlers()
        {
            this.firstPageButton.Click += OnFirstPage;
            this.previousPageButton.Click += OnPreviousPage;
            this.nextPageButton.Click += OnNextPage;
            this.LastPageButton.Click += OnLastPage;
            this.pageSizeComboBox.SelectionChanged += OnSelectPageSize;
        }

        private void OnSelectPageSize(object sender, SelectionChangedEventArgs e)
        {
            if (throwHandler) GotoPage(1);
        }

        private void GotoPage(int page)
        {
            if (ChangeHandler != null) ChangeHandler(page);
        }

        private void OnFirstPage(object sender, RoutedEventArgs e)
        {
            GotoPage(1);
        }

        private void OnPreviousPage(object sender, RoutedEventArgs e)
        {
            GotoPage(current - 1);
        }

        private void OnNextPage(object sender, RoutedEventArgs e)
        {
            GotoPage(current + 1);
        }

        private void OnLastPage(object sender, RoutedEventArgs e)
        {
            GotoPage(total);
        }

    }
}
