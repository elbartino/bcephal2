using DevExpress.Xpf.LayoutControl;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Misp.Kernel.Ui.Dashboard
{
    public class NavBlock : Tile
    {
        
        #region Properties

        public NavCategory Category { get; set; }
        public NavBlock ParentBlock { get; set; }
        public List<NavBlock> Children { get; set; }

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

        public NavBlock(Object content = null, NavigationToken navigationToken = null)
        {
            this.Children = new List<NavBlock>(0);
            this.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x9b, 0xbb));
            this.Foreground = Brushes.White;
            this.Height = 55;
            this.Width = 140;
            this.Content = content;
            this.NavigationToken = navigationToken;
            InitHandlers();
        }
        
        #endregion


        #region Operations

        public virtual void BeforeSelection()
        {

        }
        
        public void Dispose()
        {
            RemoveHandlers();
        }

        #endregion


        #region Handlers

        protected virtual void InitHandlers()
        {
            this.Click += OnClick;
            this.MouseRightButtonDown += OnMouseRightButtonDown;
        }

        protected virtual void RemoveHandlers()
        {
            this.Click -= OnClick;
            this.MouseRightButtonDown -= OnMouseRightButtonDown;
        }

        private void OnClick(object sender, EventArgs e)
        {
            BeforeSelection();
            if (Selection != null) Selection(this);
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavBlock)
            {
                NavBlockContextMenu contextMenu = new NavBlockContextMenu();
                contextMenu.PlacementTarget = (NavBlock)sender;
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
