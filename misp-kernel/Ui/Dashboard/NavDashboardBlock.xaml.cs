using DevExpress.Xpf.LayoutControl;
using Misp.Kernel.Application;
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

        public NavDashboardCategory Category { get; set; }
        public NavDashboardBlock ParentBlock { get; set; }
        public List<NavDashboardBlock> Children { get; set; }

        public NavigationToken NavigationToken { get; set; }

        public ChangeItemEventHandler Selection { get; set; }
        public ChangeItemEventHandler Hide { get; set; }
        public ChangeItemEventHandler Edit { get; set; }

        public bool IsLeaf { get { return this.Children.Count == 0; } }
        public bool HasChildren { get { return this.Children.Count > 0; } }
        public bool IsSearch { get { return this.NavigationToken != null && this.NavigationToken.ViewType == ViewType.SEARCH; } }
        public bool IsCreation { get { return this.NavigationToken != null && this.NavigationToken.EditionMode == EditionMode.CREATE; } }
        public bool IsEdition { get { return this.NavigationToken != null && this.NavigationToken.EditionMode == EditionMode.MODIFY; } }

        public Color BackgroundColor 
        { 
            get { return ((SolidColorBrush)this.Background).Color; }
            set { this.Background = new SolidColorBrush(value); }
        }
        public Color ForegroundColor 
        { 
            get { return ((SolidColorBrush)this.Foreground).Color; }
            set { this.Foreground = new SolidColorBrush(value); }
        }

        #endregion


        #region Constructors

        public NavDashboardBlock()
        {
            this.Children = new List<NavDashboardBlock>(0);
            InitializeComponent();
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
            this.Foreground = Brushes.White;
        }

        public NavDashboardBlock(String title, NavigationToken navigationToken = null)
            : this()
        {            
            this.Content = title;
            this.NavigationToken = navigationToken;            
        }

        #endregion


        #region Operations
        
        public void Dispose()
        {
            this.Click -= OnClick;
            this.MouseRightButtonDown -= OnMouseRightButtonDown;
        }

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
