using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    public class BrowserGridContextMenu : ContextMenu
    {

        #region Constructor

        public BrowserGridContextMenu()
        {
            InitComponents();
        }

        protected virtual void InitComponents()
        {
            this.NewMenuItem = new MenuItem();
            this.NewMenuItem.Header = "New";
            this.NewMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            
            this.OpenMenuItem = new MenuItem();
            this.OpenMenuItem.Header = "Open";
            this.OpenMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Open.png", UriKind.Relative)) };
            
            this.RenameMenuItem = new MenuItem();
            this.RenameMenuItem.Header = "Rename";
            this.RenameMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Edit.png", UriKind.Relative)) };

            this.SaveAsMenuItem = new MenuItem();
            this.SaveAsMenuItem.Header = "Save a copy...";
            this.SaveAsMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative)) };
            
            this.CopyMenuItem = new MenuItem();
            this.CopyMenuItem.Header = "Copy";
            this.CopyMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative)) };
            
            this.PasteMenuItem = new MenuItem();
            this.PasteMenuItem.Header = "Paste";
            this.PasteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Paste.png", UriKind.Relative)) };
            
            this.DeleteMenuItem = new MenuItem();
            this.DeleteMenuItem.Header = "Delete";
            this.DeleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };

            this.Items.Add(this.NewMenuItem);
            this.Items.Add(this.OpenMenuItem);
            //this.Items.Add(this.RenameMenuItem);
            this.Items.Add(this.SaveAsMenuItem);
            //this.Items.Add(new Separator());
            //this.Items.Add(this.CopyMenuItem);
            //this.Items.Add(this.PasteMenuItem);
            this.MenuSeparator = new Separator();
            this.Items.Add(this.MenuSeparator);
            this.Items.Add(this.DeleteMenuItem);
            
        }

        #endregion

        #region Properties

        public MenuItem NewMenuItem { get; protected set; }
        public MenuItem OpenMenuItem { get; protected set; }
        public MenuItem RenameMenuItem { get; protected set; }
        public MenuItem SaveAsMenuItem { get; protected set; }    
        public MenuItem CopyMenuItem { get; protected set; }        
        public MenuItem PasteMenuItem { get; protected set; }
        public MenuItem DeleteMenuItem { get; protected set; }
        public Separator MenuSeparator { get; protected set; }
        #endregion

        #region Handlers
        #endregion

    }
}
