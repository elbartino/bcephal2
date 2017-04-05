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
using misp_view.Views.Prefunding;
using misp_view.Views.Member;
using misp_view.Views.Replenishment;
using misp_view.Views.Settlement;

namespace Moriset_Main_final.View.PopupDetail
{
    /// <summary>
    /// Logique d'interaction pour ListAdvisement.xaml
    /// </summary>
    public partial class ListAdvisement_SubTile : Window
    {
        public ListAdvisement_SubTile()
        {
            InitializeComponent();
        }

        private void pf_advisements_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            listPrefunding ds = new listPrefunding();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;
        }

        private void member_bank_advisements_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            listMember ds = new listMember();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;
        }

        private void replenishment_instructions_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            listReplenishment ds = new listReplenishment();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;
        }

        private void settlement_advisements_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            listSettlement ds = new listSettlement();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;
        }

        private void desactiv(object sender, EventArgs e)
        {
            Close();
        }
    }
}
