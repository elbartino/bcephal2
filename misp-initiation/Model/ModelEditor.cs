using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;

namespace Misp.Initiation.Model
{
    public class ModelEditor : Editor<Misp.Kernel.Domain.Model>
    {
        LayoutDocument layoutContent;

        public DockingManager manager = new DockingManager();
        public MenuItem RenameMenuItem { get; set; }
        public MenuItem DeleteMenuItem { get; set; }
        public MenuItem SaveMenuItem { get; set; }
        public MenuItem SaveAsMenuItem { get; set; }

        public ModelEditor()
        {
            initializeLayoutContent();
        }

        protected void initializeLayoutContent()
        {
            layoutContent = new LayoutDocument();
            layoutContent.Title = "Models";
            layoutContent.CanClose = false;
            layoutContent.CanFloat = false;
            
            

            LayoutRoot rootPanel = new LayoutRoot();
            LayoutPanel panel = new LayoutPanel();
                        
            panel.Children.Add(this);
            rootPanel.RootPanel = panel;
            manager.Layout = rootPanel;
            layoutContent.Content = manager;
            
            RenameMenuItem = new MenuItem();
            RenameMenuItem.Header = "Rename";
            
            DeleteMenuItem = new MenuItem();
            DeleteMenuItem.Header = "Delete";
           
            SaveMenuItem = new MenuItem();
            SaveMenuItem.Header = "Save";
           
            SaveAsMenuItem = new MenuItem();
            SaveAsMenuItem.Header = "SaveAs";
                       
            manager.DocumentContextMenu = new ContextMenu();
            manager.DocumentContextMenu.Items.Add(SaveMenuItem);
            manager.DocumentContextMenu.Items.Add(RenameMenuItem);
            manager.DocumentContextMenu.Items.Add(DeleteMenuItem);            
            

            manager.MouseRightButtonDown += OnRightClick;
        }

        /// <summary>
        /// Cette méthode est excécuté lorsqu'on fait un click droit sur l'entête
        /// d'une page de l'éditeur.
        /// La page en question est activée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource != null && e.OriginalSource is TextBlock)
            {
                string title = ((TextBlock)e.OriginalSource).Text;
                if (!string.IsNullOrEmpty(title))
                {
                    this.selectePage(title);
                }
            }
        }
        
        public LayoutDocument getLayoutContent() { return layoutContent; }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Misp.Kernel.Domain.Model> getNewPage() { return new ModelEditorItem(); }

        
    }
}
