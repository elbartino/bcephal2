using System;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using System.Threading.Tasks;
using System.Windows.Markup;
using DevExpress.Utils.Menu;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Navigation;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Xpf.Bars;
using Moriset_Main_final.View;
using Moriset_Main_final.View.PoPupDetail;
using Moriset_Main_final.View.PopupDetail;
using misp_view.Views.Review;
using misp_view.Views.BankAccount;


namespace Moriset_Main_final
{

    public partial class MainWindow : Window
    {

        static int cpt = 0;
        static Tile current;

        static String[] tabTileNav = new String[50];
        
        Review a = new Review();
        static ListAdvisement_SubTile lasCurrent = new ListAdvisement_SubTile();
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Initialize Block

                private void reconciliation_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("reconciliation", "Reconciliation");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.Click += reconciliationScreen;
                    reconciliation_item.IsEnabled = false;
                    bool HasSubMenu = true;
            
                }
                private void dailycontrols_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("dailycontrols", "Daily Controls");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += dailyScreen;
                    dailycontrols_item.IsEnabled = false;
                    bool HasSubMenu = true;
                }
                private void pf_account_review_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("pf_account_review", "PF Account Review");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += pf_account_reviewScreen;
                    pf_account_review_item.IsEnabled = false;
                    bool HasSubMenu = false;
                }
                private void reconciliation_report_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("reconciliation_report", "Reconciliation Report");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += reconciliation_reportScreen;
                    reconciliation_report_item.IsEnabled = false;
                    bool HasSubMenu = true;
                }
                private void settlement_evolution_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("settlement_evolution", "Settlement Evolution");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += settlement_evolutionScreen;
                    settlement_evolution_item.IsEnabled = false;
                    bool HasSubMenu = false;
                }
                private void new_advisement_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("new_advisement", "New Advisement");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += new_advisementScreen;
                    new_advisement_item.IsEnabled = false;
                    bool HasSubMenu = true;
                }
                private void ageing_balance_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("ageing_balance", "Ageing Balance");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += ageing_balanceScreen;
                    ageing_balance_item.IsEnabled = false;
                    bool HasSubMenu = false;
                }
                private void bank_account_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("bank_account", "Bank Account");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += bank_accountScreen;
                    bank_account_item.IsEnabled = false;
                    bool HasSubMenu = false;
                }
                private void list_advisement_Click(object sender, EventArgs e)
                {
                    Moriset_Main_final.View.Block block_layout = new Moriset_Main_final.View.Block("list_advisements", "List Advisements");
                    customTileLayout.Children.Add(block_layout);
                    block_layout.MouseRightButtonDown += popmenu;
                    block_layout.Click += list_advisementScreen;
                    list_advisements_item.IsEnabled = false;
                    bool HasSubMenu = true;
                  
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
                    Reconciliation_SubTile ds = new Reconciliation_SubTile();
                    ds.Show();
                }
                private void reconciliation_reportScreen(object sender, EventArgs e)
                {
                    ReconciliationReport_SubTile ds = new ReconciliationReport_SubTile();
                    ds.Show();
                }
                private void new_advisementScreen(object sender, EventArgs e)
                {
                    NewAdvisement_SubTile ds = new NewAdvisement_SubTile();
                    ds.Show();
      
                }
                private void list_advisementScreen(object sender, EventArgs e)
                {
                    ListAdvisement_SubTile ds = new ListAdvisement_SubTile();
                    ds.Show();
                }
       
                #endregion
        #region Properties Fenetres

                private void bank_accountScreen(object sender, EventArgs e)
                {
                    Window w = new Window();
                    BankAccount ds = new BankAccount();
                    Grid g = new Grid();
                    w.Content = g;
                    g.Children.Add(ds);
                    w.Show();
                    w.WindowState = WindowState.Maximized; 
                }
                private void pf_account_reviewScreen(object sender, EventArgs e)
                {
                    Window w = new Window();
                    Review ds = new Review();
            
                    Grid g = new Grid();
                    w.Content = g;
                    g.Children.Add(ds);
                    w.Show();
                    w.WindowState = WindowState.Maximized;
            
                }
                private void settlement_evolutionScreen(object sender, EventArgs e)
                {
                    Window w = new Window();
                    Review d = new Review();
                    Grid g = new Grid();
                    w.Content = g;

                    d.tabControl.SelectedIndex = 1;
                    g.Children.Add(d);
                    w.Show();
                    w.WindowState = WindowState.Maximized;
            
                }
                private void ageing_balanceScreen(object sender, EventArgs e)
                {
                    Window w = new Window();
                    Review d = new Review();
                    Grid g = new Grid();
                    w.Content = g;

                    d.tabControl.SelectedIndex = 2;
                    g.Children.Add(d);
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
                    Edit edit = new Edit();
                    edit.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    edit.ShowDialog();
                    Color tileColor = edit.colorTile.Color;
                    Color textColor = edit.textColorTile.Color;
                    current.Background = new SolidColorBrush(tileColor);
                    current.Foreground = new SolidColorBrush(textColor);        
                }
                private void hideMenu_Click(object sender, RoutedEventArgs e)
                {
                    current.RemoveFromVisualTree();
                    string s = current.Name;
                    s = s + "_item";
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
                        if(cpt==0)
                        {
                            tileNavigationPanel.Visibility = Visibility.Hidden;
                            ++cpt;                   
                        }
                        else if (cpt ==1 ){
                            tileNavigationPanel.Visibility = Visibility.Visible;
                            cpt = 0;
                        }
                }
        #endregion
    }     
  }

