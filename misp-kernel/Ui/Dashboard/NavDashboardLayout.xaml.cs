using DevExpress.Xpf.LayoutControl;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for NavDashboardLayout.xaml
    /// </summary>
    public partial class NavDashboardLayout : TileLayoutControl
    {
        
        #region Properties

        public ChangeItemEventHandler Selection { get; set; }

        #endregion


        #region Constructors

        public NavDashboardLayout()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void AddBlock(NavDashboardBlock block)
        {
            block.Selection -= OnBlockSelected;
            block.Hide -= OnBlockHided;
            block.Edit -= OnBlockEdited;
            block.Selection += OnBlockSelected;
            block.Hide += OnBlockHided;
            block.Edit += OnBlockEdited;
            this.Children.Add(block);
        }

        public void RemoveBlock(NavDashboardBlock block)
        {
            block.Selection -= OnBlockSelected;
            block.Hide -= OnBlockHided;
            block.Edit -= OnBlockEdited;
            this.Children.Remove(block);
        }

        #endregion


        #region Handlers
        
        private void OnBlockSelected(object item)
        {
            if (Selection != null) Selection(item);
        }

        private void OnBlockEdited(object item)
        {
            if (item != null && item is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)item;
                //Edit edit = new Edit();
                //edit.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //edit.ShowDialog();
                //Color tileColor = edit.colorTile.Color;
                //Color textColor = edit.textColorTile.Color;
                //current.Background = new SolidColorBrush(tileColor);
                //current.Foreground = new SolidColorBrush(textColor);
            }            
        }

        private void OnBlockHided(object item)
        {
            if (item != null && item is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)item;
                //current.RemoveFromVisualTree();
                //string s = current.Name;
                //s = s + "_item";
                //if (s == "reconciliation_item")
                //{
                //    reconciliation_item.IsEnabled = true;
                //}
                //if (s == "dailycontrols_item")
                //{
                //    dailycontrols_item.IsEnabled = true;
                //}
                //if (s == "pf_account_review_item")
                //{
                //    pf_account_review_item.IsEnabled = true;
                //}
                //if (s == "reconciliation_report_item")
                //{
                //    reconciliation_report_item.IsEnabled = true;
                //}
                //if (s == "settlement_evolution_item")
                //{
                //    settlement_evolution_item.IsEnabled = true;
                //}
                //if (s == "new_advisement_item")
                //{
                //    new_advisement_item.IsEnabled = true;
                //}
                //if (s == "ageing_balance_item")
                //{
                //    ageing_balance_item.IsEnabled = true;
                //}
                //if (s == "bank_account_item")
                //{
                //    bank_account_item.IsEnabled = true;
                //}
                //if (s == "list_advisements_item")
                //{
                //    list_advisements_item.IsEnabled = true;
                //}

                //cpt--;
                //if (cpt == 0)
                //{
                //    current.Visibility = Visibility.Hidden;
                //}
            }
        }

        #endregion

    }
}
