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
    /// Interaction logic for NavDashboardBlock.xaml
    /// </summary>
    public partial class NavDashboardBlock : Tile
    {
        
        #region Properties

        public ChangeItemEventHandler Selection { get; set; }
        public ChangeItemEventHandler Hide { get; set; }
        public ChangeItemEventHandler Edit { get; set; }

        #endregion


        #region Constructors

        public NavDashboardBlock()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
        }

        public NavDashboardBlock(String name, String title) : this()
        {            
            this.Content = title;
            this.Name = name;            
        }

        #endregion


        #region Operations



        #endregion


        #region Handlers

        private void OnClick(object sender, EventArgs e)
        {
            if (Selection != null) Selection(this);
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavDashboardBlock)
            {
                NavDashboardBlock block = (NavDashboardBlock)sender;
                ContextMenu contextMenu = this.FindResource("block_context_menu") as ContextMenu;
                contextMenu.PlacementTarget = block;
                contextMenu.IsOpen = true;
            }
        }

        private void OnEdit(object sender, RoutedEventArgs e)
        {
            if (Edit != null) Edit(this);
        }

        private void OnHide(object sender, RoutedEventArgs e)
        {
            if (Hide != null) Hide(this);
        }

        #endregion
                
    }
}
