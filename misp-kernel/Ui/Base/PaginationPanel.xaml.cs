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
    /// Interaction logic for PaginationPanel.xaml
    /// </summary>
    public partial class PaginationPanel : StackPanel
    {
        public PaginationPanel()
        {
            InitializeComponent();
        }

        public void UpdatePagination(long currentPage, long pageCount, string comment)
        {
            this.GotoFirstPageButton.IsEnabled = currentPage > 1 && currentPage <= pageCount;
            this.GotoPreviousPageButton.IsEnabled = currentPage > 1 && currentPage <= pageCount;
            this.GotoNextPageButton.IsEnabled = currentPage > 0 && currentPage < pageCount;
            this.GotoLastPageButton.IsEnabled = currentPage > 0 && currentPage < pageCount;

            this.ProgressBar.Minimum = 1;
            this.ProgressBar.Maximum = pageCount;
            this.ProgressBar.Value = currentPage;

            this.CurrentPageTextBox.Text = currentPage.ToString();
            this.PageCountTextBlock.Text = pageCount.ToString();
            this.CommentLabel.Content = string.IsNullOrWhiteSpace(comment) ? "" : comment;            
        }
    }
}
