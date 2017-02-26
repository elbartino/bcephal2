using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeEditor: Editor<Misp.Kernel.Domain.TransformationTree>
    {
        LayoutDocument layoutContent;

        public DockingManager manager = new DockingManager();
        public MenuItem RenameMenuItem { get; set; }
      //  public MenuItem DeleteMenuItem { get; set; }
        public MenuItem SaveMenuItem { get; set; }
        public MenuItem SaveAsMenuItem { get; set; }


        public TransformationTreeEditor(Kernel.Domain.SubjectType subjectType, String functionality)
            : base(subjectType, functionality)
        {
            initializeLayoutContent();
        }

        protected void initializeLayoutContent()
        {
            layoutContent = new LayoutDocument();
            layoutContent.Title = "Transformation Tree";
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
            
           // DeleteMenuItem = new MenuItem();
           // DeleteMenuItem.Header = "Delete";
           
            SaveMenuItem = new MenuItem();
            SaveMenuItem.Header = "Save";
           
            SaveAsMenuItem = new MenuItem();
            SaveAsMenuItem.Header = "SaveAs";
                       
            manager.DocumentContextMenu = new ContextMenu();
            manager.DocumentContextMenu.Items.Add(SaveMenuItem);
            manager.DocumentContextMenu.Items.Add(RenameMenuItem);
           // manager.DocumentContextMenu.Items.Add(DeleteMenuItem);            
            

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
        protected override EditorItem<Misp.Kernel.Domain.TransformationTree> getNewPage() 
        {
            TransformationTreeEditorItem page = new TransformationTreeEditorItem(this.SubjectType);
            return page; 
        }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 2)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }

    }
}
