using Misp.Kernel.Application;
using Misp.Kernel.Domain;
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

        PrivilegeObserver observer;

        #endregion


        #region Constructors

        public NavDashboardView()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        #endregion


        #region Operations



        #endregion


        #region Handlers
        
        private void InitializeHandlers()
        {
            this.DashboardBar.Selection += OnCategorySelected;
            this.DashboardLayout.Selection += OnBlockSelected;
            this.DashboardLayout.BlockHide += OnBlockHided;

            this.SubDashboardLayout.Selection += OnBlockSelected;
        }

        private void OnCategorySelected(object item)
        {
            if (item != null && item is NavDashboardCategory)
            {
                NavDashboardCategory category = (NavDashboardCategory)item;
                if (category.Block != null)
                {
                    category.Block.Category = category;
                    this.DashboardLayout.AddBlock(category.Block);
                    category.IsEnabled = false;
                }
            }
        }

        private void OnBlockSelected(object item)
        {
            if (item != null && item is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)item;
                if (block.IsLeaf)
                {
                    if(block.IsSearch) HistoryHandler.Instance.openPage(block.NavigationToken);
                    else HistoryHandler.Instance.openPage(block.NavigationToken);
                }
                else
                {
                    this.SubDashboardLayout.Clear();
                    foreach(NavDashboardBlock child in block.Children)
                    {
                        child.ParentBlock = block;
                        this.SubDashboardLayout.AddBlock(child);
                    }
                }
            }
        }

        private void OnBlockHided(object item)
        {
            if (item != null && item is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)item;
                this.DashboardLayout.RemoveBlock(block);
                if (block.Category != null) block.Category.IsEnabled = true;
            }
        }


        #endregion


        #region Init
        
        public void BuildCategories()
        {
            observer = new PrivilegeObserver();
            this.DashboardBar.NavButton.Items.Clear();

            foreach (Plugin.IPlugin plugin in ApplicationManager.Instance.Plugins)
            {
                foreach (NavDashboardCategory category in plugin.NavDashboardCategories)
                {
                    if (CheckUserRights(category)) this.DashboardBar.AddCategory(category);
                }
            }
        }

        private bool CheckUserRights(NavDashboardCategory category)
        {            
            List<Right> rights = new List<Right>(0);
            bool hasPrivilage = true;
            bool hasCreatePrivilage = true;
            bool hasViewOrEditPrivilage = true;
            if (observer != null && !observer.user.IsAdmin())
            {
                hasPrivilage = false;
                hasCreatePrivilage = false;
                hasViewOrEditPrivilage = false;
                rights = observer.GetRights(category.FunctionalityCode);
                if (rights.Count == 0)
                {
                    if (observer.hasAcendentPrivilege(category.FunctionalityCode))
                    {
                        hasPrivilage = true;
                        hasCreatePrivilage = true;
                        hasViewOrEditPrivilage = true;
                    }
                    else return false;
                }

                foreach (Right right in rights)
                {
                    if (string.IsNullOrWhiteSpace(right.rightType))
                    {
                        hasPrivilage = true;
                        hasCreatePrivilage = true;
                        hasViewOrEditPrivilage = true;
                    }
                    else if (right.rightType.Equals(RightType.CREATE.ToString())) hasCreatePrivilage = true;
                    else if (right.rightType.Equals(RightType.EDIT.ToString())) hasViewOrEditPrivilage = true;
                    else if (right.rightType.Equals(RightType.VIEW.ToString())) hasViewOrEditPrivilage = true;
                }
            }

            if (hasPrivilage || hasCreatePrivilage || hasViewOrEditPrivilage)
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}
