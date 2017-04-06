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
    /// Interaction logic for NavDashboardView.xaml
    /// </summary>
    public partial class NavDashboardView : Grid
    {

        #region Properties



        #endregion


        #region Constructors

        public NavDashboardView()
        {
            InitializeComponent();
            InitializeHandlers();
            UserInit();
        }

        private void UserInit()
        {
            this.DashboardBar.AddCategory(new NavDashboardCategory("reconciliation_item", "Reconciliation"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("reconciliation_item", "Reconciliation"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("dailycontrols_item", "Daily Controls"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("pf_account_review_item", "PF Account Review"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("reconciliation_report_item", "Reconciliation Report"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("settlement_evolution_item", "Settlement Evolution"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("new_advisement_item", "New Advisement"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("ageing_balance_item", "Ageing Balance"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("bank_account_item", "Bank Account"));
            this.DashboardBar.AddCategory(new NavDashboardCategory("list_advisements_item", "List Advisements"));
        }

        #endregion


        #region Operations



        #endregion


        #region Handlers
        
        private void InitializeHandlers()
        {
            this.DashboardBar.Selection += OnCategorySelected;
            this.DashboardLayout.Selection += OnBlockSelected;
        }

        private void OnCategorySelected(object item)
        {
            if (item != null && item is NavDashboardCategory)
            {
                NavDashboardCategory category = (NavDashboardCategory)item;
                NavDashboardBlock block = new NavDashboardBlock(category.Name, category.Content.ToString());
                this.DashboardLayout.AddBlock(block);
            }
        }

        private void OnBlockSelected(object item)
        {
            if (item != null && item is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)item;
            }
        }


        #endregion


        
    }
}
