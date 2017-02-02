using DevExpress.Xpf.Core;
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
    /// Interaction logic for GridBrowserToolbar.xaml
    /// </summary>
    public partial class GridBrowserToolbar : Grid
    {
        public Kernel.Ui.Base.ChangeItemEventHandler ChangeHandler;

        public int current;
        public int total;

        protected bool throwHandler;

        public GridBrowserToolbar()
        {
            ThemeManager.SetThemeName(this, "None");
            InitializeComponent();
            List<int> sizes = new List<int>(0);            
            sizes.Add(10);
            sizes.Add(20);
            sizes.Add(25);
            sizes.Add(30);
            sizes.Add(40);
            sizes.Add(50);
            sizes.Add(60);
            sizes.Add(100);
            this.pageSizeComboBox.ItemsSource = sizes;
            this.pageSizeComboBox.SelectedItem = 25;
            InitializeHandlers();
            current = 0;
            this.Visibility = Visibility.Hidden;
            throwHandler = true;
        }

        
        public void displayPage(GrillePage page)
        {
            throwHandler = false;
            current = page.currentPage;
            total = page.pageCount;
            this.currentPage.Content = page.currentPage;
            this.totalPageLabel.Content = page.pageCount;
            this.label.Content = "" + page.pageFirstItem + " to " + page.pageLastItem + " / " + page.totalItemCount;

            this.firstPageButton.IsEnabled = page.currentPage > 1;
            this.previousPageButton.IsEnabled = page.currentPage > 1;

            this.nextPageButton.IsEnabled = page.currentPage < page.pageCount;
            this.LastPageButton.IsEnabled = page.currentPage < page.pageCount;

            this.firstPageButton.Foreground = this.firstPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.previousPageButton.Foreground = this.previousPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.nextPageButton.Foreground = this.nextPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;
            this.LastPageButton.Foreground = this.LastPageButton.IsEnabled ? Brushes.Black : Brushes.Gray;

            if (!this.showAllCheckBox.IsChecked.Value)
            {
                this.pageSizeComboBox.SelectedItem = page.pageSize;
                this.pageSizeComboBox.IsEnabled = page.pageCount > 0;
            }

            this.Visibility = page.pageCount > 0 ? Visibility.Visible : Visibility.Hidden;
            throwHandler = true;
        }

        private void InitializeHandlers()
        {
            this.firstPageButton.Click += OnFirstPage;
            this.previousPageButton.Click += OnPreviousPage;
            this.nextPageButton.Click += OnNextPage;
            this.LastPageButton.Click += OnLastPage;
            this.pageSizeComboBox.SelectionChanged += OnSelectPageSize;
            this.showAllCheckBox.Checked += OnShowAllChecked;
            this.showAllCheckBox.Unchecked += OnShowAllChecked;
        }

        private void OnShowAllChecked(object sender, RoutedEventArgs e)
        {
            this.pageSizeComboBox.IsEnabled = !this.showAllCheckBox.IsChecked.Value;
            if (throwHandler) GotoPage(1);
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
