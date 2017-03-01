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

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
