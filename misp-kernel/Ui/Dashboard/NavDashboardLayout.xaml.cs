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
            block.Selection += OnBlockSelected;
            this.Children.Add(block);
        }

        public void RemoveBlock(NavDashboardBlock block)
        {
            block.Selection -= OnBlockSelected;
            this.Children.Remove(block);
        }

        #endregion


        #region Handlers
        
        private void OnBlockSelected(object item)
        {
            if (Selection != null) Selection(item);
        }


        #endregion

    }
}
