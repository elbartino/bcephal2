using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Navigation;
using Moriset_Main_final.View;
using Moriset_Main_final.View.PoPupDetail;
using Moriset_Main_final.View.PopupDetail;
using misp_view.Views.Review;
using misp_view.Views.BankAccount;


namespace Moriset_Main_final
{

    public partial class MainWindow : Window
    {
        static int counterTab = 0;
        static int cpt = 0;
        static Tile current;
        //private Dictionary<string, TileNavCategory> tilenavCat = new Dictionary<string, TileNavCategory>();
        // static String[] tabTileNav = new String[50];
        TileNavCategory[]tabNavCategory = new TileNavCategory[50];
        Window[]tabWindow = new Window[20];
        static int counterWindow = 0;
        Review a = new Review();
        static ListAdvisement_SubTile lasCurrent = new ListAdvisement_SubTile();
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Initialize Block

        private void reconciliation_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("reconciliation", "Reconciliation");
            customTileLayout.Children.Add(block_layout);
            block_layout.Click += reconciliationScreen;
            block_layout.MouseRightButtonDown += popmenu;
            reconciliation_item.IsEnabled = false;
            //bool HasSubMenu = true;

        }
        private void dailycontrols_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("dailycontrols", "Daily Controls");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += dailyScreen;
            dailycontrols_item.IsEnabled = false;
            //bool HasSubMenu = true;
        }
        private void pf_account_review_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("pf_account_review", "PF Account Review");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += pf_account_reviewScreen;
            pf_account_review_item.IsEnabled = false;
            //bool HasSubMenu = false;
        }
        private void reconciliation_report_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("reconciliation_report", "Reconciliation Report");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += reconciliation_reportScreen;
            reconciliation_report_item.IsEnabled = false;
            //bool HasSubMenu = true;
        }
        private void settlement_evolution_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("settlement_evolution", "Settlement Evolution");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += settlement_evolutionScreen;
            settlement_evolution_item.IsEnabled = false;
            //bool HasSubMenu = false;
        }
        private void new_advisement_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("new_advisement", "New Advisement");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += new_advisementScreen;
            new_advisement_item.IsEnabled = false;
            //bool HasSubMenu = true;
        }
        private void ageing_balance_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("ageing_balance", "Ageing Balance");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += ageing_balanceScreen;
            ageing_balance_item.IsEnabled = false;
            //bool HasSubMenu = false;
        }
        private void bank_account_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("bank_account", "Bank Account");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += bank_accountScreen;
            bank_account_item.IsEnabled = false;
            //bool HasSubMenu = false;
        }
        private void list_advisement_Click(object sender, EventArgs e)
        {
            Block block_layout = new Block("list_advisements", "List Advisements");
            customTileLayout.Children.Add(block_layout);
            block_layout.MouseRightButtonDown += popmenu;
            block_layout.Click += list_advisementScreen;
            list_advisements_item.IsEnabled = false;
            //bool HasSubMenu = true;

        }
        #endregion
        #region Properties SubMenu

        private void dailyScreen(object sender, EventArgs e)
        {
            DailyControls_SubTile ds = new DailyControls_SubTile();
            ds.Show();
        }

        private void reconciliationScreen(object sender, EventArgs e)
        {
            Reconciliation_SubTile rs = new Reconciliation_SubTile();
            rs.Show();
        }
        private void reconciliation_reportScreen(object sender, EventArgs e)
        {
            ReconciliationReport_SubTile rrs = new ReconciliationReport_SubTile();
            rrs.Show();
        }
        private void new_advisementScreen(object sender, EventArgs e)
        {
            NewAdvisement_SubTile nwa = new NewAdvisement_SubTile();
            nwa.Show();

        }
        private void list_advisementScreen(object sender, EventArgs e)
        {
            ListAdvisement_SubTile las = new ListAdvisement_SubTile();
            las.Show();
        }

        #endregion
        #region Properties Fenetres

        private void bank_accountScreen(object sender, EventArgs e)
        {
            Window w = new Window();
            BankAccount bas = new BankAccount();
            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(bas);
            w.Show();
            w.WindowState = WindowState.Maximized;
        }
        private void pf_account_reviewScreen(object sender, EventArgs e)
        {
            Window w = new Window();
            Review r = new Review();

            Grid g = new Grid();
            w.Content = g;
            g.Children.Add(r);
            w.Show();
            w.WindowState = WindowState.Maximized;

        }
        private void settlement_evolutionScreen(object sender, EventArgs e)
        {
            Window w = new Window();
            Review r = new Review();
            Grid g = new Grid();
            w.Content = g;

            r.tabControl.SelectedIndex = 1;
            g.Children.Add(r);
            w.Show();
            w.WindowState = WindowState.Maximized;

        }
        private void ageing_balanceScreen(object sender, EventArgs e)
        {
            Window w = new Window();
            Review r = new Review();
            Grid g = new Grid();
            w.Content = g;

            r.tabControl.SelectedIndex = 2;
            g.Children.Add(r);
            w.Show();
            w.WindowState = WindowState.Maximized;

        }
        #endregion
        #region Operations

        private void popmenu(object sender, EventArgs e)
        {

            if (sender is Tile)
            {
                ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.IsOpen = true;   
                current = sender as Tile;
            }
        }
        private void editMenu_Click(object sender, RoutedEventArgs e)
        {
            int counter  = 0;
            while (tabNavCategory[counter].Name != current.Name)
            {
                counter++;
            }
            Edit edit = new Edit();
            edit.tbNameTile.Text =(string)current.Content;
            edit.colorTile.Color = ((SolidColorBrush)current.Background).Color;
            edit.textColorTile.Color = ((SolidColorBrush)current.Foreground).Color;
            edit.WindowStartupLocation = WindowStartupLocation.CenterScreen;  
            edit.ShowDialog();
            Color tileColor = edit.colorTile.Color;
            Color textColor = edit.textColorTile.Color;
            current.Background = new SolidColorBrush(tileColor);
            current.Foreground = new SolidColorBrush(textColor);
            current.Content = edit.tbNameTile.Text; 
            current.Name = edit.tbNameTile.Text;
            tabNavCategory[counter].Content = current.Content;
            tabNavCategory[counter].Name = current.Name;
            tabNavCategory[counter].Background = new SolidColorBrush(tileColor);
            tabNavCategory[counter].Foreground = new SolidColorBrush(textColor);


        }
        private void hideMenu_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            while (tabNavCategory[counter].Name != current.Name)
            {
                counter++;
            }
            tabNavCategory[counter].IsEnabled = true;
            current.RemoveFromVisualTree();
            string s = current.Name;
            s = s + "_item";
            //if (s == tilenavCat["cat"].Name)
            //{
            //    tilenavCat["cat"].IsEnabled = true;
            //}
            if (s == "reconciliation_item")
            {
                reconciliation_item.IsEnabled = true;
            }
            if (s == "dailycontrols_item")
            {
                dailycontrols_item.IsEnabled = true;
            }
            if (s == "pf_account_review_item")
            {
                pf_account_review_item.IsEnabled = true;
            }
            if (s == "reconciliation_report_item")
            {
                reconciliation_report_item.IsEnabled = true;
            }
            if (s == "settlement_evolution_item")
            {
                settlement_evolution_item.IsEnabled = true;
            }
            if (s == "new_advisement_item")
            {
                new_advisement_item.IsEnabled = true;
            }
            if (s == "ageing_balance_item")
            {
                ageing_balance_item.IsEnabled = true;
            }
            if (s == "bank_account_item")
            {
                bank_account_item.IsEnabled = true;
            }
            if (s == "list_advisements_item")
            {
                list_advisements_item.IsEnabled = true;
            }

            cpt--;
            if (cpt == 0)
            {
                current.Visibility = Visibility.Hidden;
            }
        }
        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (cpt == 0)
            {
                tileNavigationPanel.Visibility = Visibility.Hidden;
                ++cpt;
            }
            else if (cpt == 1)
            {
                tileNavigationPanel.Visibility = Visibility.Visible;
                cpt = 0;
            }
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void createTile_Click(object sender, EventArgs e)
        {
            
            DialogBoxName dbn = new DialogBoxName();
            dbn.ShowDialog();
            Block custom = new Block("custom", "Custom");
            TileNavCategory cat = new TileNavCategory();
            custom.Content = dbn.NameTile;
            custom.Name = dbn.NameTile;
            custom.Click += dynamicScreen;
            customTileLayout.Children.Add(custom);
            custom.MouseRightButtonDown += popmenu;
            cat.Content = custom.Content;
            cat.Name = custom.Name;
            tabNavCategory[counterTab] = cat;
            //tilenavCat.Add("cat", cat);
            customisedTile.Items.Add(cat);
            cat.IsEnabled = false;
            counterTab++;

        }

        private void dynamicScreen(object sender, EventArgs e)
        {
            ScreenDynamic sd = new ScreenDynamic();
            if (tabWindow[0] == null)
            {
                MessageBox.Show("No screen created");
            }
            else
            {
                for (int i = 0; i < tabWindow.Length; i++)
                {
                    if (tabWindow[i] != null)
                    {
                        Button b = new Button();
                        Label l = new Label();
                        l.Width = 20;
                        b.Width = 142;
                        b.Content = tabWindow[i].Name;
                        b.Height = 60;
                        sd.spDynamic.Children.Add(l);
                        sd.spDynamic.Children.Add(b);
                    }
                }
            }
            sd.Show();
        }

        

        private void dyanmicEditMenu_Click(object sender, RoutedEventArgs e)
        {
            DynamicEdit de = new DynamicEdit();
            de.ShowDialog();
        }

        private void customisedTile_Click(object sender, EventArgs e)
        {

            //TileNavCategory tc = (TileNavCategory)sender;
            //Block custom = new Block(tc.Name, (string)tc.Content);
            //custom.Background = tc.Background;
            //custom.Foreground = tc.Foreground;
            //Block custom = new Block("custom", "custom");
            //customTileLayout.Children.Add(custom);


        }

        private void createScreen_Click(object sender, EventArgs e)
        {
            string s = "Screen_"+counterWindow;
            Window w = new Window();
            w.Name = s;
            tabWindow[counterWindow] = w;
            MessageBox.Show("Window "+counterWindow+" created");
            counterWindow++;
        }

        private void deleteScreen_Click(object sender, EventArgs e)
        {
            if (tabWindow[0] != null)
            {
                tabWindow[counterWindow-1] = null;
                int n = counterWindow-1;
                MessageBox.Show("Window "+n+" deleted");
                counterWindow--;
            }
            else
            {
                MessageBox.Show("No screen created");
            }
        }
    }
}

