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
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Service;
using System.Collections.ObjectModel;
using Misp.Kernel.Util;

namespace Misp.Kernel.Ui.Group
{
    /// <summary>
    /// Interaction logic for GroupTreeView.xaml
    /// </summary>
    public partial class GroupTreeView : ScrollViewer
    {
        
        public ChangeEventHandler Changed;

        public BGroup Root { get; set; }
        public BGroup CurrentCutObject { get; set; }
        public BGroup CurrentCopiedObject { get; set; }
        public BGroup defaultValue;
        public SubjectType subjectType;

        /// <summary>
        /// Le GroupService.
        /// </summary>
        public GroupService GroupService { get; set; }

        #region Constructor

        /// <summary>
        /// Crée une nouvelle instance de GroupTreeView
        /// </summary>
        public GroupTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.BGroup();
            defaultValue = new Domain.BGroup();
            defaultValue.name = SubjectType.DEFAULT.label;
            subjectType = SubjectType.DEFAULT;
        }
        
        /// <summary>
        /// Affiche le contenu du group
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(BGroup root)
        {
            this.tree.Items.Clear();
            this.Root = root;
            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.tree.ItemsSource = this.Root.GetItems() ;
                SetSelectedGroup(defaultValue);
            }
        }

        private void RefreshParent(IHierarchyObject item)
        {
            if (item != null)
            {
                foreach (IHierarchyObject child in item.GetItems())
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }
        
        #endregion


        #region Initialization

        /// <summary>
        /// Initialise le DataTemplate
        /// </summary>
        protected virtual void InitializeDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(BGroup));
            dataTemplate.ItemsSource = new Binding("childrenListChangeHandler.Items");

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding("name"));

            dataTemplate.VisualTree = factory;
            //this.tree.ItemTemplate = dataTemplate;
        }

        /// <summary>
        /// Initialise les handlers sur l'arbre
        /// </summary>
        protected virtual void InitializeHandlers()
        {
            this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnPreviewMouseRightButtonDown);
        }

       
        /// <summary>
        /// Initialise le menu contextuel.
        /// </summary>
        protected virtual void InitializeContextMenu()
        {            
            this.newMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/New.png", UriKind.Relative)) };
            this.newMenuItem.Click += new RoutedEventHandler(OnNewNode);

            this.copyMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Copy.png", UriKind.Relative)) };
            this.copyMenuItem.Click += new RoutedEventHandler(OnCopyNode);

            this.pasteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Paste.png", UriKind.Relative)) };
            this.pasteMenuItem.Click += new RoutedEventHandler(OnPasteNode);

            this.indentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Indent.png", UriKind.Relative)) };
            this.indentMenuItem.Click += new RoutedEventHandler(OnIndentNode);

            this.outdentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Outdent.png", UriKind.Relative)) };
            this.outdentMenuItem.Click += new RoutedEventHandler(OnOutdentNode);

            this.deleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.deleteMenuItem.Click += new RoutedEventHandler(OndeleteNode);

        }
        /// <summary>
        /// methode appelé lorsqu'on supprime un group node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OndeleteNode(object sender, RoutedEventArgs e)
        {
           BGroup group = GetSelectedGroup();
           var response = MessageDisplayer.DisplayYesNoQuestion("Suppression du Groupe", " Do you really want to delete? ");
           if (response == MessageBoxResult.Yes)
           {
               BGroup parent = group.parent;
               DeleteGroup(group);
               parent.RemoveChild(group);
               parent.UpdateParents();
               ((ObservableCollection<BGroup>)tree.ItemsSource).Remove(group);
           }
           
            
        }

        /// <summary>
        /// Cette methode permet la selection du noeud présent derrière la souris
        /// lorsque fait un click-droit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            else 
            {
                BGroup group = GetSelectedGroup();
                if (group != null) group.IsSelected = false;
                tree.Focus(); 
            }
        }

        static EO.Wpf.TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is EO.Wpf.TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as EO.Wpf.TreeViewItem;
        }

        #endregion


        #region Context Menu Handlers

        protected void SaveGroup(BGroup group)
        {
            BGroup g = null;
            if (group.parent != Root)
            {
                SaveGroup(group.parent);
                g = GroupService.getGroupByName(group.name);
            }
            else
            {
                g = GroupService.Save(group);
            }
            if (g != null) group.oid = g.oid;
        }

        /// <summary>
        /// call delete group service
        /// </summary>
        /// <param name="group"></param>
        protected void DeleteGroup(BGroup group)
        {
            if (group != null)
            {
               
                foreach (BGroup child in group.childrenListChangeHandler.Items)
                {
                    DeleteGroup(child);
                }
                    GroupService.delete(group);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Le group sélectionné</returns>
        public BGroup GetSelectedGroup()
        {
            return this.tree.SelectedItem != null ?
                this.tree.SelectedItem as BGroup : null;
        }

        /// <summary>
        /// Selectionne un groupe dans l'arbre
        /// </summary>
        /// <param name="group">Le groupe à sélectionner</param>
        public void SetSelectedGroup(string name)
        {
            BGroup group = (BGroup)this.Root.GetChildByName(name);
            SetSelectedGroup(group);
        }

        /// <summary>
        /// Selectionne un groupe dans l'arbre
        /// </summary>
        /// <param name="group">Le groupe à sélectionner</param>
        public void SetSelectedGroup(BGroup group)
        {
            if (group != null)
            {
                if (group.parent != null) group.parent.IsExpanded = true;
                group.IsSelected = true;
            }
            else
            {
                BGroup selection = GetSelectedGroup();
                if (selection != null) selection.IsSelected = false;
            }
            tree.Focus();
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            BGroup parent = GetSelectedGroup();
            BGroup group = AddNode(parent);
        }
                
        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is BGroup)
            {
                BGroup group = (BGroup)e.Item;
                if (group.IsDefaultGroup) { e.Canceled = true; return; }
                e.Text = group.name;
            }
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is BGroup)
            {
                BGroup group = (BGroup)e.Item;
                string name = e.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Empty group", "Group can't be empty! ");
                    e.Canceled = true;
                    return;
                }
                BGroup m = (BGroup)Root.GetChildByName(name);
                if (m != null && m.Equals(group)) { e.Canceled = true; return; }
                if (m != null && !m.Equals(group))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Duplicate group", name + " already exists!");
                    e.Canceled = true;
                    return;
                }
                group.name = name;
                group.parent.UpdateChild(group);
                SaveGroup(group);
                if (Changed != null) Changed();
            }
            //The event must be canceled, otherwise the TreeView will
            //set the TreeViewItem's Header to the new text
            e.Canceled = true;
        }
        
        /// <summary>
        /// Cette methode permet la copie d'un élément sélectionné dans le treeview
        /// Après la séléction de l'objet, un copie de cet objet est placée dans le presse-papiers
        /// en attente d'être collé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyNode(object sender, RoutedEventArgs e)
        {
            BGroup group = GetSelectedGroup();
            if (group != null)
            {
                List<Domain.BGroup> grps = new List<BGroup>(0);
                grps.Add(group);
                //IHierarchyObject copy = group.GetCopy();
                Kernel.Util.ClipbordUtil.SetGroups(grps.ToList<Object>());                
            }
            CurrentCutObject = null;
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
            BGroup group = GetSelectedGroup();
            CurrentCutObject = group;
            if (group != null)
            {
                List<Domain.BGroup> grps = new List<BGroup>(0);
                grps.Add(group);
                Kernel.Util.ClipbordUtil.SetGroups(grps.ToList<Object>());
            }            
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
            BGroup parent = GetSelectedGroup();
            if (parent == null) parent = this.Root;
           
            if (CurrentCutObject != null)
            {                  
                if (CurrentCutObject == parent)
                {
                    CurrentCutObject = null;
                }
                else
                {
                    CurrentCutObject.parent.ForgetChild(CurrentCutObject);
                    parent.AddChild(CurrentCutObject);
                    SaveGroup(CurrentCutObject);
                    List<Domain.BGroup> groupes = Kernel.Util.ClipbordUtil.GetGroupes();
                                         
                    CurrentCutObject = null;
                    if (Changed != null) Changed();
                }                
            }
            else
            {
                List<BGroup> listeGroupe = Kernel.Util.ClipbordUtil.GetGroupes();
                List<BGroup> cListGroupe = new List<BGroup>();
                foreach (BGroup newItem in listeGroupe)
                {
                    if (newItem != null)
                    {
                        BGroup newitem = newItem.GetCopy() as BGroup;
                        parent.AddChild(newitem);
                        SaveGroup(newitem);
                        cListGroupe.Add(newitem);
                        Kernel.Util.ClipbordUtil.SetGroups(cListGroupe.ToList<Object>());
                        if (Changed != null) Changed();
                    }
                }
            }
        }
        
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Outdent".
        /// Elle permet de transformer un sous-noeud en noeud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutdentNode(object sender, RoutedEventArgs e)
        {
            OutdentNode(GetSelectedGroup());
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Indent".
        /// Elle permet de transformer un noeud en sous-noeud .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIndentNode(object sender, RoutedEventArgs e)
        {
            IndentNode(GetSelectedGroup());
        }

        /// <summary>
        /// Cette méthode permet de désactiver un menuItem dans le cas
        /// où l'opération associée à ce menuItem n'est pas possible pour
        /// le noeud courant.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            BGroup selectedItem = GetSelectedGroup();
            bool isDefaultGroup = selectedItem != null && selectedItem.IsDefaultGroup;
            this.newMenuItem.IsEnabled = this.Root != null && !isDefaultGroup;
            this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && !selectedItem.IsDefaultGroup &&  selectedItem.parent != null;
            this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null;
            this.pasteMenuItem.IsEnabled = this.Root != null && selectedItem != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyGroup() && !isDefaultGroup;
            this.indentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() > 0 && selectedItem.parent != null && !isDefaultGroup;
            this.outdentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem.GetParent() != Root;
        }

        #endregion
        


        #region Node Model Actions
        
        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="model">Le noeud auquel il fau ajouter un fils</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual BGroup AddNode(BGroup parent)
        {
            BGroup child = GetNewTreeViewModel();
            if (parent != null)
            {
                parent.AddChild(child);
                parent.UpdateParents();
              
            }
            else
            {
                this.Root.AddChild(child);
            }
           // GroupService.Save(this.Root);
            SaveGroup(child);
            SetSelectedGroup(child);
            if (Changed != null) Changed();
            return child;
        }

        private void refreshDataSource()
        {
            this.tree.ItemsSource = this.Root.GetItems();
            this.tree.Items.Refresh();
        }

        

        /// <summary>
        /// Transforme un noeud en sous-noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void IndentNode(BGroup item)
        {
            if (item.parent != null)
            {
                int position = item.GetPosition();
                IHierarchyObject child = item.parent.GetChildByPosition(position - 1);
                if (child != null)
                {
                    item.parent.ForgetChild(item);
                    child.AddChild(item);
                    SaveGroup(item); 
                    if (Changed != null) Changed();
                    SetSelectedGroup(item);
                }
            }
        }

        /// <summary>
        /// Transforme un sous-noeud en noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void OutdentNode(BGroup item)
        {
            if (item.parent != null)
            {
                IHierarchyObject parent = item.parent.GetParent();
                if (parent != null)
                {
                    item.parent.ForgetChild(item);
                    parent.AddChild(item);
                    SaveGroup(item); 
                    if (Changed != null) Changed();
                    SetSelectedGroup(item);
                }
            }
        }

        
        #endregion;

        protected BGroup GetNewTreeViewModel()
        {
            BGroup group = new BGroup();
            group.name = "Group";
            group.subjectType = subjectType.label; 
            if (Root != null)
            {
                BGroup m = null;
                int i = 1;
                do
                {
                    group.name = "Group" + i++;
                    m = (BGroup)Root.GetChildByName(group.name);
                }
                while (m != null);
            }
            return group;
        }

    }
}
