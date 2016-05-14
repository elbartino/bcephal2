using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Misp.Kernel.Domain;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Cette classe implémente un arbre éditable pemettant d'effectuer des actions telles que:
    ///     - Ajouter un noeud, 
    ///     - Supprimer un noeud, 
    ///     - Déplacer un noeud vers le haut, le bas, la gauche ou la droite.
    ///     - ...
    /// </summary>
    /// 
    public class EditableTree : System.Windows.Controls.TreeView 
    {


        public static EditableTextBlock valeurSelection;


        public ChangeHandler Change;
        public delegate void ChangeHandler();

        public bool CanCreateSubNode { get; set; }
        

        #region Properties

        public MenuItem NewMenuItem { get; set; }
        public MenuItem EditMenuItem { get; set; }
        public MenuItem CopyMenuItem { get; set; }
        public MenuItem CutMenuItem { get; set; }
        public MenuItem PasteMenuItem { get; set; }
        public MenuItem DeleteMenuItem { get; set; }
        public MenuItem MoveUpMenuItem { get; set; }
        public MenuItem MoveDownMenuItem { get; set; }
        public MenuItem IndentMenuItem { get; set; }
        public MenuItem OutdentMenuItem { get; set; }

        public IHierarchyObject Root { get; set; }
        public IHierarchyObject CurrentParent{get;set;}
        protected IHierarchyObject CurrentCutObject { get; set; }


        #endregion
        
        
        #region Constructor

        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public EditableTree()
        {
            this.CanCreateSubNode = true;
            this.Root = GetNewTreeViewModel();
            InitializeComponent();
            this.Focusable = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(IHierarchyObject root)
        {
            this.Root = root;
            //this.ItemsSource = new ObservableCollection<IHierarchyObject>();
            if (this.Root == null) this.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.ItemsSource = this.Root.GetItems();
            }
        }

        private void RefreshParent(IHierarchyObject item)
        {
            if (item != null)
            {
                foreach(IHierarchyObject child in item.GetItems())
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }
        
        #endregion


        #region Initializations

        /// <summary>
        /// Initialise le DataTemplate, les handlers et le menu contextuel.
        /// </summary>
        private void InitializeComponent()
        {
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
        }
        
        /// <summary>
        /// Initialise le DataTemplate
        /// </summary>
        protected virtual void InitializeDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(GetDataType());
            dataTemplate.ItemsSource = new Binding(GetChildrenBindingName());
            
            FrameworkElementFactory factory = new FrameworkElementFactory(GetRendererDataType());
            factory.SetBinding(EditableTextBlock.TextProperty, new Binding(GetRendererBindingName()));

            dataTemplate.VisualTree = factory;
       
            HierarchicalDataTemplate template = System.Windows.Application.Current.Resources["EditableTreeViewTemplate"] as HierarchicalDataTemplate;
            this.ItemTemplate = template;
        }

        protected virtual string GetBindingName() { return "Items"; }
        protected virtual object GetDataType() { return typeof(IHierarchyObject); }
        protected virtual string GetChildrenBindingName() { return "Children"; }
        protected virtual Type GetRendererDataType() { return typeof(EditableTextBlock); }
        protected virtual string GetRendererBindingName() { return "name"; }

        protected virtual string GetClipbordDataFormat() { return "Misp.IHierarchyObject"; }

        protected virtual void InitializeHandlers()
        {
            this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnPreviewMouseRightButtonDown);
        }  
        /// <summary>
        /// Initialise le menu contextuel.
        /// </summary>
        protected virtual void InitializeContextMenu()
        {
            this.NewMenuItem = new MenuItem();
            this.NewMenuItem.Header = "New";
            this.NewMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.NewMenuItem.Click += new RoutedEventHandler(OnNewNode);

            this.EditMenuItem = new MenuItem();
            this.EditMenuItem.Header = "Edit";
            this.EditMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Edit.png", UriKind.Relative)) };
            this.EditMenuItem.Click += new RoutedEventHandler(OnEditNode);

            this.CopyMenuItem = new MenuItem();
            this.CopyMenuItem.Header = "Copy";
            this.CopyMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative)) };
            this.CopyMenuItem.Click += new RoutedEventHandler(OnCopyNode);

            this.CutMenuItem = new MenuItem();
            this.CutMenuItem.Header = "Cut";
            this.CutMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Cut.png", UriKind.Relative)) };
            this.CutMenuItem.Click += new RoutedEventHandler(OnCutNode);

            this.PasteMenuItem = new MenuItem();
            this.PasteMenuItem.Header = "Paste";
            this.PasteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Paste.png", UriKind.Relative)) };
            this.PasteMenuItem.Click += new RoutedEventHandler(OnPasteNode);

            this.DeleteMenuItem = new MenuItem();
            this.DeleteMenuItem.Header = "Delete";
            this.DeleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.DeleteMenuItem.Click += new RoutedEventHandler(OnDeleteNode);

            this.MoveUpMenuItem = new MenuItem();
            this.MoveUpMenuItem.Header = "Move up";
            this.MoveUpMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.MoveUpMenuItem.Click += new RoutedEventHandler(OnMoveUpNode);

            this.MoveDownMenuItem = new MenuItem();
            this.MoveDownMenuItem.Header = "Move down";
            this.MoveDownMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.MoveDownMenuItem.Click += new RoutedEventHandler(OnMoveDownNode);

            this.IndentMenuItem = new MenuItem();
            this.IndentMenuItem.Header = "Indent";
            this.IndentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.IndentMenuItem.Click += new RoutedEventHandler(OnIndentNode);

            this.OutdentMenuItem = new MenuItem();
            this.OutdentMenuItem.Header = "Outdent";
            this.OutdentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.OutdentMenuItem.Click += new RoutedEventHandler(OnOutdentNode);

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Items.Add(this.NewMenuItem);
            //this.ContextMenu.Items.Add(this.EditMenuItem);
            this.ContextMenu.Items.Add(this.CutMenuItem);
            this.ContextMenu.Items.Add(this.CopyMenuItem);
            this.ContextMenu.Items.Add(this.PasteMenuItem);
            this.ContextMenu.Items.Add(this.DeleteMenuItem);
            this.ContextMenu.Items.Add(new Separator());
            this.ContextMenu.Items.Add(this.MoveUpMenuItem);
            this.ContextMenu.Items.Add(this.MoveDownMenuItem);
            this.ContextMenu.Items.Add(new Separator());
            this.ContextMenu.Items.Add(this.IndentMenuItem);
            this.ContextMenu.Items.Add(this.OutdentMenuItem);
        }
      
        #endregion

        #region Copy, Cut , Paste Actions
        /// <summary>
        /// Cette methode permet la copie d'un élément sélectionné dans le treeview
        /// Après la séléction de l'objet, un copie de cet objet est placée dans le presse-papiers
        /// en attente d'être collé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyNode(object sender, RoutedEventArgs e)
        {
            IHierarchyObject selectedItem = (IHierarchyObject)this.SelectedItem;
            IHierarchyObject copy = selectedItem.GetCopy();
            Util.ClipbordUtil.SetHierarchyObject(copy);
        }

        /// <summary>
        /// Cette méthode permet de couper un élément sélectionné dans le treeview
        /// Lorsqu'on clique sur le menu contextuelle cut,
        /// la variable CurrentCutObjet prend la valeur de l'élément selectionné
        /// et une copie de cette dernière est placé dans le presse-papier.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCutNode(object sender, RoutedEventArgs e)
        {
            IHierarchyObject selectedItem = (IHierarchyObject)this.SelectedItem;
            CurrentCutObject = selectedItem;
            Util.ClipbordUtil.SetHierarchyObject(CurrentCutObject.GetCopy());
        }

        /// <summary>
        /// Cette méthode permet de coller un objet précédement copier ou couper.
        /// A cet effet on vérifie si le Paste vient après un Cut(si CurrentCutObject!=null) ou un Copy,
        /// puis on met dans le presse-papiers une copie de l'élément coupé ou copié.
        /// On affecte un nouveau parent à l'élément coupé/copié.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPasteNode(object sender, RoutedEventArgs e)
        {
            IHierarchyObject parent;
            IHierarchyObject newItem = null;
            if (CurrentParent == null)
                parent = this.Root;
            else
                parent = (IHierarchyObject)this.SelectedItem;

            if (CurrentCutObject != null)
            {
                CurrentCutObject.GetParent().ForgetChild(CurrentCutObject);
                newItem = CurrentCutObject;
                IHierarchyObject copy = CurrentCutObject.GetCopy();
                Util.ClipbordUtil.SetHierarchyObject(copy);
                if (CurrentCutObject == parent)
                {
                    CurrentCutObject = null;
                    return;
                }
                CurrentCutObject = null;
            }
            else
            {
                Console.WriteLine(this.GetClipbordDataFormat());
                /*
                newItem = Util.ClipbordUtil.GetHierarchyObject(this.GetClipbordDataFormat());
                if(newItem != null)
                Util.ClipbordUtil.SetHierarchyObject(newItem.GetCopy());*/
            }

            if (newItem == null) return;
            if (parent != null && CanCreateSubNode)
            {
                parent.AddChild(newItem);
                parent.UpdateParents();
            }
            else
            {
                this.Root.AddChild(newItem);
            }
            SelectItem(this, newItem);
            if (Change != null) Change();
        }
        #endregion
     

        #region Context Menu Handlers


        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            CurrentParent = AddNode(CurrentParent);
            this.SelectItem(this, CurrentParent);
            CurrentParent = null;
        }

      
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Delete".
        /// Elle permet de supprimer un noeud de l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteNode(object sender, RoutedEventArgs e)
        {
            DeleteNode((IHierarchyObject)this.SelectedItem);
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Edit".
        /// Elle permet de mettre un noeud en mode "édition".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditNode(object sender, RoutedEventArgs e)
        {
            //valeurSelection.IsInEditMode = true;
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sure le menu "Move Down".
        /// Elle permet de déplacer un noeud vers le bas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveDownNode(object sender, RoutedEventArgs e)
        {
            MoveNode((IHierarchyObject)this.SelectedItem, 1);
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Move Up".
        /// Elle permet de déplacer un noeud vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveUpNode(object sender, RoutedEventArgs e)
        {
            MoveNode((IHierarchyObject)this.SelectedItem, 0);
        }
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Outdent".
        /// Elle permet de transformer un sous-noeud en noeud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutdentNode(object sender, RoutedEventArgs e)
        {
            OutdentNode((IHierarchyObject)this.SelectedItem);
        }
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Indent".
        /// Elle permet de transformer un noeud en sous-noeud .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIndentNode(object sender, RoutedEventArgs e)
        {
            IndentNode((IHierarchyObject)this.SelectedItem);
        }
        /// <summary>
        /// Cette méthode permet de désactiver un menuItem dans le cas
        /// où l'opération associée à ce menuItem n'est pas possible pour
        /// le noeud courant.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            IHierarchyObject selectedItem = (IHierarchyObject)this.SelectedItem;
            this.NewMenuItem.IsEnabled = this.Root != null;
            this.EditMenuItem.IsEnabled = this.Root != null && selectedItem != null && CurrentParent != null;
            this.CutMenuItem.IsEnabled = this.Root != null && selectedItem != null && CurrentParent != null;
            this.CopyMenuItem.IsEnabled = this.Root != null && selectedItem != null && CurrentParent != null;
            this.PasteMenuItem.IsEnabled = this.Root != null && selectedItem != null && !Util.ClipbordUtil.IsClipBoardEmpty();
            this.DeleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && CurrentParent != null;
            this.MoveUpMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() > 0 && CurrentParent != null;
            this.MoveDownMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() < selectedItem.GetParent().GetItems().Count - 1 && CurrentParent != null;
            this.IndentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() > 0 && CurrentParent != null;
            this.OutdentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetParent() != null && selectedItem.GetParent() != Root && CurrentParent != null;
        }
        /// <summary>
        /// Cette methode permet la selection du noeud présent derrière la souris
        /// lorsque fait un click-droit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem == null)
                CurrentParent = null;
            else
            {

                treeViewItem.Focus();
                e.Handled = true;
                CurrentParent = (IHierarchyObject)this.SelectedItem;
            }
        }
        #endregion
        
        #region Tree Handlers

        #endregion

        #region Node Model Actions

        protected virtual IHierarchyObject GetNewTreeViewModel() { return null; }

        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="model">Le noeud auquel il fau ajouter un fils</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual IHierarchyObject AddNode(IHierarchyObject parent) 
        {            
            IHierarchyObject child = GetNewTreeViewModel();
            if (parent != null && CanCreateSubNode){
                parent.AddChild(child);
                parent.UpdateParents();
            }   
            else 
            { 
                this.Root.AddChild(child);             
            }
            if (Change != null) Change();

            
            return child;
        }

        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(IHierarchyObject item)
        {
            if (item != null && item.GetParent() != null)
            {
                MessageBoxResult result = Util.MessageDisplayer.DisplayYesNoQuestion("Delete Item", "Do you want to delete node : " + item.ToString() + "?");
                if (result == MessageBoxResult.Yes)
                {
                    int index = item.GetPosition();
                    item.GetParent().RemoveChild(item);
                    item.GetParent().UpdateParents();

                    if (Change != null) Change();
                    if(this.Root.GetItems().Count>1) selectNodeAfterDelete(item,index);
                }
            }
        }

        public void selectNodeAfterDelete(IHierarchyObject itemSupprime,int i) 
        {
            IHierarchyObject itemParent = itemSupprime.GetParent();
            IHierarchyObject itemASelectionne;
        
                if (itemParent.GetItems().Count > 0)
                {
                    if (i > 0)
                        itemASelectionne = itemParent.GetItems()[i - 1] as IHierarchyObject;
                    else
                        itemASelectionne = itemParent.GetItems()[0] as IHierarchyObject;
                }
                else
                itemASelectionne = itemParent;

                SelectItem(this, itemASelectionne);                        
        }

        /// <summary>
        /// Déplace un noeud vers le haut ou vers le bas
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sens">Le sens du déplacement. 0 => UP, 1=> DOWN</param>
        public void MoveNode(IHierarchyObject item, int sens = 0)
        {            
            if (item.GetParent() != null)
            {
                int position = item.GetPosition() + (sens == 0 ? -1 : 1);
                IHierarchyObject child = item.GetParent().GetChildByPosition(position);
                if (child != null) 
                { 
                    child.SetPosition(item.GetPosition()); item.GetParent().UpdateChild(child);
                    item.SetPosition(position);
                    item.GetParent().UpdateChild(item);
                    if (Change != null) Change();
                    SelectItem(this,item);
                }                
            }
        }

        /// <summary>
        /// Transforme un noeud en sous-noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void IndentNode(IHierarchyObject item)
        {
            if (item.GetParent() != null)
            {
                int position = item.GetPosition();
                IHierarchyObject child = item.GetParent().GetChildByPosition(position - 1);
                if (child != null)
                {
                    item.GetParent().ForgetChild(item);
                    child.AddChild(item);
                    if (Change != null) Change();
                    SelectItem(this, item);
                }
            }
        }

        /// <summary>
        /// Transforme un sous-noeud en noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void OutdentNode(IHierarchyObject item)
        {
            if (item.GetParent() != null)
            {
                IHierarchyObject parent = item.GetParent().GetParent();
                if (parent != null)
                {
                    item.GetParent().ForgetChild(item);
                    parent.AddChild(item);
                    if (Change != null) Change();
                    SelectItem(this, item);
                }
            }
        }

        public bool IsNameInUse(IHierarchyObject newItem, IHierarchyObject root = null)
        {
            if (root == null)
                root = this.Root;



            Type typeConteneur = this.GetRendererDataType();



            foreach (IHierarchyObject child in root.GetItems())
            {
                if (child == newItem)
                    return true;
                else
                    IsNameInUse(newItem, child);
            }
            return false;
        }

        public void SelectItem(EditableTree treeView, object item)
        {
            ExpandAndSelectItem(treeView, item);
        }
        /// <summary>
        /// Cette méthode permet de selectionner et de déployer un noeud
        /// </summary>
        /// <param name="parentContainer">l'arbre qui contient l'ensemble des noeuds et sous noeuds </param>
        /// <param name="itemToSelect">L'élément à sélectionner</param>
        /// <returns></returns>
        private bool ExpandAndSelectItem(ItemsControl parentContainer, object itemToSelect)
        {
            foreach (Object item in parentContainer.Items)
            {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (item == itemToSelect && currentContainer != null)
                {
                    currentContainer.IsSelected = true;
                    currentContainer.BringIntoView();
                    currentContainer.Focus();
                    return true;
                }
            }
            foreach (Object item in parentContainer.Items)
            {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (currentContainer != null && currentContainer.Items.Count > 0)
                {

                    bool wasExpanded = currentContainer.IsExpanded;
                    currentContainer.IsExpanded = true;

                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        EventHandler eh = null;
                        eh = new EventHandler(delegate
                        {
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                if (ExpandAndSelectItem(currentContainer, itemToSelect) == false)
                                {
                                    currentContainer.IsExpanded = false;
                                }
                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else
                    {
                        if (ExpandAndSelectItem(currentContainer, itemToSelect) == false)
                        {
                            currentContainer.IsExpanded = wasExpanded;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
        #endregion;
    }
}
