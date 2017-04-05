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
    /// Logique d'interaction pour NewAdvisement_SubTile.xaml
    /// </summary>
    public partial class NewAdvisement_SubTile : Window
    {
        public NewAdvisement_SubTile()
        {
            InitializeComponent();
        }
        public void createBtn()
        {
            Button b = new Button();
            Label l = new Label()
            {
                Width = 20
            };
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pf_advisement_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            newPrefunding ds = new newPrefunding();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;      
        }

        private void member_bank_advisement_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            newMember ds = new newMember();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;  
        }

        private void replenishment_instruction_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            newReplenishment ds = new newReplenishment();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(ds);
            w.Show();
            w.WindowState = WindowState.Maximized;  
        }

        private void settlement_advisement_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            newSettlement ds = new newSettlement();
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
