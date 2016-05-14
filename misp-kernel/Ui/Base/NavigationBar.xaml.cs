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

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : Grid
    {
        public event ChangeEventHandler Changed;

        public long CurrentPage { get; set; }
        public long TotalPage { get; set; }

        public NavigationBar()
        {
            CurrentPage = 1;
            InitializeComponent();
            InitializeHandlers();
        }


        public void UpdateBar(long currentPage, long totalPage)
        {
            CurrentPage = currentPage;
            TotalPage = totalPage;

            FirstPageButton.IsEnabled = CurrentPage > 1 && CurrentPage <= TotalPage;
            PreviousPageButton.IsEnabled = CurrentPage > 1 && CurrentPage <= TotalPage;
            NextPageButton.IsEnabled = TotalPage > 1 && CurrentPage < TotalPage;
            LastPageButton.IsEnabled = TotalPage > 1 && CurrentPage < TotalPage;

            CurrentPageTextBox.Text = CurrentPage.ToString();
            TotalPageTextBox.Text = TotalPage.ToString();

            if (CurrentPage <= 0 || TotalPage <= 0) ClearCommentLabel();
        }

        public void SetComment(long first, long last, long totalItemCount)
        {
            if (first <= 0 || last <= 0) { ClearCommentLabel(); return; }
            this.CommentLabel.Content = "Line: " + first + " to " + last + " / " + totalItemCount;
        }

        public void ClearCommentLabel()
        {
            this.CommentLabel.Content = "";
        }

        protected void InitializeHandlers()
        {
            FirstPageButton.Click += OnFirstPageButtonClick;
            PreviousPageButton.Click += OnPreviousPageButtonClick;
            NextPageButton.Click += OnNextPageButtonClick;
            LastPageButton.Click += OnLastPageButtonClick;
        }

        private void OnLastPageButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateBar(TotalPage, TotalPage);
            if (Changed != null) Changed();
        }

        private void OnNextPageButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateBar(CurrentPage + 1, TotalPage);
            if (Changed != null) Changed();
        }

        private void OnPreviousPageButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateBar(CurrentPage - 1, TotalPage);
            if (Changed != null) Changed();
        }

        private void OnFirstPageButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateBar(1, TotalPage);
            if (Changed != null) Changed();
        }


        
    }
}
