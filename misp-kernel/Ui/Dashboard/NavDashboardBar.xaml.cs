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

        public void AddCategory(NavDashboardCategory category)
        {
            category.Selection -= OnCategorySelected;
            category.Selection += OnCategorySelected;
            this.NavButton.Items.Add(category);
        }

        public void RemoveCategory(NavDashboardCategory category)
        {
            category.Selection -= OnCategorySelected;
            this.NavButton.Items.Remove(category);
        }

        #endregion


        #region Handlers
        
        private void OnCategorySelected(object item)
        {
            if (Selection != null) Selection(item);
        }


        #endregion


        
    }
}
