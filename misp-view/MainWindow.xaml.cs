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
using misp_view.Views.Review;
using misp_view.Views.Prefunding;
using misp_view.Views.Replenishment;
using misp_view.Views.Member;
using misp_view.Views.Settlement;
using misp_view.Views.NewClient;
using misp_view.Views.BankAccount;
using misp_view.Views.Details;
using DevExpress.Xpf.PdfViewer;
using misp_view.Views.PDF;

namespace misp_view
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Review r = new Review();
        newPrefunding p = new newPrefunding();
        listPrefunding lp = new listPrefunding();
        newReplenishment rp = new newReplenishment();
        listReplenishment lrp = new listReplenishment();
        newMember m = new newMember();
        listMember lm= new listMember();
        newSettlement s = new newSettlement();
        listSettlement ls = new listSettlement();
        NewClient nc = new NewClient();
        ListClient lc = new ListClient();
        BankAccount ba = new BankAccount();
        public MainWindow()
        {
            InitializeComponent();
            //PdfViewerControl viewer = new PdfViewerControl();
            //this.Content = viewer;
        }
        private void btnPrefunding_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(p);

        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            // Doesn't work if click more than once -> Has to be fixed   
            gridInfo.Children.Add(r);
        }

        private void btnListPrefunding_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(lp);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(p);

        }

        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(lp);
        }

        private void MenuItemReplenishment_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(rp);
        }

        private void MenuItemReplenishment_Click2(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(lrp);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            // Doesn't work if click more than once -> Has to be fixed   
            gridInfo.Children.Add(r);
        }

        private void MenuItem_Click_newMember(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(m);
        }

        private void MenuItem_Click_listMember(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(lm);
        }

        private void MenuItem_ClicknewSettlement(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(s);

        }

        private void MenuItem_ClicklistSettlement(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(ls);

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(nc);
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(ba);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Detail d = new Detail();
            d.ShowDialog();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            PDFView pdf = new PDFView();
            //pdf.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.None;
            pdf.OpenDocument("C://TEMP/Test.pdf");
            pdf.Show();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            gridInfo.Children.Clear();
            gridInfo.Children.Add(lc);
        }
    }
}
