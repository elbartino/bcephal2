using DevExpress.Xpf.Navigation;
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
    /// Interaction logic for NavDashboardBar.xaml
    /// </summary>
    public partial class NavDashboardBar : TileNavPane
    {

        #region Properties

        public ChangeItemEventHandler Selection { get; set; }

        #endregion


        #region Constructors

        public NavDashboardBar()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void AddStandardCategory(NavCategory category)
        {
            category.Selection -= OnCategorySelected;
            category.Selection += OnCategorySelected;
            this.StandardNavButton.Items.Add(category);
        }

        public void RemoveStandardCategory(NavCategory category)
        {
            category.Selection -= OnCategorySelected;
            this.StandardNavButton.Items.Remove(category);
        }

        public void ClearStandard()
        {
            int count = this.StandardNavButton.Items.Count;
            while (count > 0)
            {
                INavElement elt = this.StandardNavButton.Items[0];
                if (elt is NavCategory) this.RemoveStandardCategory((NavCategory)elt);
                else this.StandardNavButton.Items.Remove(elt);
                count--;
            }
        }

        #endregion


        #region Handlers
        
        private void OnCategorySelected(object item)
        {
            if (Selection != null) Selection(item);
        }

        private void OnStandardNavButtonLostFocus(object sender, RoutedEventArgs e)
        {
            this.StandardNavButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        


        
    }
}
