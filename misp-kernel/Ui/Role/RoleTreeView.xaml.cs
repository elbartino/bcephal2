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
using RestSharp.Contrib;
using EO.Wpf;
using Misp.Kernel.Util;


namespace Misp.Kernel.Ui.Role
{
    /// <summary>
    /// Interaction logic for RoleTreeView.xaml
    /// 
    /// Cette classe implémente l'arbre d'édition des role.
    /// </summary>
    public partial class RoleTreeView : ScrollViewer
    {

        public ChangeEventHandler Changed;

        static string  Label_DEFAULT_ROLE = "Add Role";
        public Kernel.Domain.Role Root { get; set; }
        public Kernel.Domain.Role CurrentCutObject { get; set; }
        public Kernel.Domain.Role CurrentCopiedObject { get; set; }
        public Kernel.Domain.Role defaultValue;
        private Service.RoleService RoleService;
        private bool isTreeInCutMode { get; set; }
        public List<int> indexSelecte { get; set; }

 


        #region Constructor
        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public RoleTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.Role();
            defaultValue = new Domain.Role();
            defaultValue.name = Label_DEFAULT_ROLE;
            defaultValue.IsDefault = true;
            this.RoleService = Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetRoleService();

        }
        /// <summary>
        /// Affiche le contenu de la mesure
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(Kernel.Domain.Role root)
        {
            this.Root = root;
            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.Root.AddChild(defaultValue);
                
                this.tree.ItemsSource = this.Root.GetItems();
                SetSelectedRole(defaultValue);


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

        #region Initialization

        /// <summary>
        /// Initialise le DataTemplate
        /// </summary>
        protected virtual void InitializeDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(Misp.Kernel.Domain.Role));
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
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnItemClick);
            this.PreviewMouseLeftButtonUp += OnMouseDown;
            this.MouseDown += OnMouseDown;
        }


       #region Multiselection Bloc
       public  bool isSelectionDownToUp = false;
        //get blue selection
        public static SolidColorBrush SELECTION_COLOR = new SolidColorBrush(Color.FromRgb(51, 153, 255));
        // a set of all selected measures
        Dictionary<Kernel.Domain.Role, int> selectedRoles = new Dictionary<Domain.Role, int>();

        // a set of all selected items
        Dictionary<EO.Wpf.TreeViewItem, string> selectedItems = new Dictionary<EO.Wpf.TreeViewItem, string>();

        // true only while ctrl key is pressed
        bool CtrlPressed
        {
            get
            {
                return (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl)||System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl));
            }
        }
        // deselects the tree item
        void Deselect(EO.Wpf.TreeViewItem treeViewItem)
        {
            treeViewItem.Background = Brushes.Transparent;// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        void ChangeSelectedState(EO.Wpf.TreeViewItem treeViewItem)
        {
            if (!selectedItems.ContainsKey(treeViewItem))
            { // select

                treeViewItem.Background = SELECTION_COLOR; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
                selectedItems.Add(treeViewItem, null); // add the item to selected items
            }
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }


        //met a jour la selection liste en cours
        private void UpdateSelectionList(Domain.Role selectedMeasure)
        {
            if (!selectedRoles.ContainsKey(selectedMeasure))
            { // add
                selectedRoles.Remove(selectedMeasure);
                selectedRoles.Add(selectedMeasure, selectedRoles.Count + 1);
            }
            else
            { // remove
                selectedRoles.Remove(selectedMeasure);
            }
        }

       /// <summary>
       ///
       /// </summary>
       /// <param name="treeViewItem"></param>
       private void DoSelectedItemChanged(EO.Wpf.TreeViewItem treeViewItem)
       {
           Domain.Role selectedRole;
           if (treeViewItem == null)
           {
               removeCTRLSelection();
               return;
           }
           else
           {
               selectedRole = GetSelectedRole();
           }

           removeCTRLSelection();
           ChangeSelectedState(treeViewItem);
           if (selectedRole != null)
               UpdateSelectionList(selectedRole);
       }

       private void removeCTRLSelection()
       {
           if (!CtrlPressed)
           {
               List<EO.Wpf.TreeViewItem> selectedTreeViewItemList = new List<EO.Wpf.TreeViewItem>();
               foreach (EO.Wpf.TreeViewItem treeViewItem1 in selectedItems.Keys)
               {
                   selectedTreeViewItemList.Add(treeViewItem1);
               }

               foreach (EO.Wpf.TreeViewItem treeViewItem1 in selectedTreeViewItemList)
               {
                   Deselect(treeViewItem1);
               }
               selectedItems.Clear();
               selectedRoles.Clear();
           }
       }

       private void SelectionManager(EO.Wpf.TreeViewItem treeViewItem, bool isRightClick = true)
       {
           Kernel.Domain.Role role;

           if (treeViewItem.Header.ToString() == defaultValue.name)
           {
               role = null;
               treeViewItem.Focus();
           }
           else
           {
               role = this.Root.GetChildByName(treeViewItem.Header.ToString()) as Domain.Role;
               if (!CtrlPressed)
               {
                   if (!isRightClick || (isRightClick && !selectedItems.ContainsKey(treeViewItem) && !selectedRoles.Keys.Contains(role)))
                   {
                       removeCTRLSelection();
                       selectedItems.Add(treeViewItem, null);
                       SetSelectedItem(treeViewItem);
                       selectedRoles.Add(role, selectedRoles.Count + 1);
                   }
               }
               else if (CtrlPressed && !selectedItems.ContainsKey(treeViewItem) && !selectedRoles.Keys.Contains(role))
               {
                   selectedItems.Add(treeViewItem, null);
                   selectedRoles.Add(role, selectedRoles.Count + 1);
                   SetSelectedItem(treeViewItem);
               }
           }
       }

       private void SetSelectedItem(EO.Wpf.TreeViewItem treeViewItem)
       {
           treeViewItem.Background = SELECTION_COLOR; // change background and foreground colors
           treeViewItem.Foreground = Brushes.White;
       }

        #endregion
       /// <summary>
        /// Vérifie si l'on va cliquer sur le noeud par défaut afin d'activer le mode
        /// édition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemClick(object sender, MouseButtonEventArgs e)
        {
            Kernel.Domain.Role r;
            EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                SelectionManager(treeViewItem, false);
            }
            else
            {
                r = GetSelectedRole();
                tree.Focus();
            }
        }

        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            EO.Wpf.TreeViewItem treeViewItem = new EO.Wpf.TreeViewItem();
            if (e != null)
                treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            else
                treeViewItem = ((sender as EO.Wpf.TreeView).SelectedItem as EO.Wpf.TreeViewItem);

            if (treeViewItem != null)
            {
                SelectionManager(treeViewItem, false);
            }
            else removeCTRLSelection();
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

            this.cutMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Cut.png", UriKind.Relative)) };
            this.cutMenuItem.Click += new RoutedEventHandler(OnCutNode);

            this.pasteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Paste.png", UriKind.Relative)) };
            this.pasteMenuItem.Click += new RoutedEventHandler(OnPasteNode);

            this.deleteMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.deleteMenuItem.Click += new RoutedEventHandler(OnDeleteNode);

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
                SelectionManager(treeViewItem);
                e.Handled = true;
            }
            else
            {
                Kernel.Domain.Role r = GetSelectedRole();
                if (r != null) r.IsSelected = false;
                tree.Focus();
                removeCTRLSelection();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La role sélectionnée dans le cas de multiselection mode</returns>
        public Kernel.Domain.Role GetSelectedMultiRole()
        {
            Kernel.Domain.Role r = null;
            r = this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.Role :
                     (this.tree.SelectedItem == null && this.tree.SelectedValue != null && this.tree.SelectedValue != defaultValue ?
                     (this.tree.SelectedValue as Kernel.Domain.Role) : null);
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée</returns>
        public Kernel.Domain.Role GetSelectedRole()
        {
            return this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.Role : null;
        }
   
        /// <summary>
        /// Selectionne une mesure dans l'arbre
        /// </summary>
        /// <param name="measure">La mesure à sélectionner</param>
        public void SetSelectedRole(Kernel.Domain.Role role)
        {
            if (role != null)
            {
                role.IsSelected = true;
             
            }
            else
            {
                Kernel.Domain.Role selection = GetSelectedMultiRole();
                if (selection != null) selection.IsSelected = false;
            }
            try
            {
                tree.Focus();
            }
            catch (Exception) 
            {
            
            }
        }


     /// <summary>
     /// Ajoute la valeur par défaut après avoir créer une role
     /// </summary>
     /// <param name="value">le parent de la role à créer</param>
     /// <returns>la mesure créé</returns>
        public Domain.Role addDefaultNode(Kernel.Domain.Role value=null) 
        {
            Domain.Role r =  AddNode(value);
            return r;
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.Role r = GetSelectedRole();            
            AddNode(r);
            if (r != null)
            {
                removeCTRLSelection();
            }
        }
        
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Delete".
        /// Elle permet de supprimer un noeud de l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteNode(object sender, RoutedEventArgs e)
        {
           
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Role", "Do you want to delete selected Role ?");
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.Role r in selectedRoles.Keys)
                { 
                    DeleteNode(r);
                    
                }
            }
        }

        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is Kernel.Domain.Role)
                {
                    Kernel.Domain.Role editedMeasure = (Kernel.Domain.Role)e.Item;
                    string name = e.Text.Trim();

                    Kernel.Domain.Role validName = ValidateName(editedMeasure, name);
                    if (validName == null)
                    {
                        e.Canceled = true;
                        return;
                    }
                    if (editedMeasure.IsDefault)
                    {
                        if (name.ToUpper() != Label_DEFAULT_ROLE.ToUpper())
                        {
                            Kernel.Domain.Role addedNode = new Domain.Role();
                            addedNode.name = name;

                            editedMeasure.name = Label_DEFAULT_ROLE;
                            editedMeasure.parent.UpdateChild(editedMeasure);

                            addedNode = ValidateName(addedNode, name);
                            AddNode(null, addedNode.name);
                            e.Canceled = true;
                        }
                    }
                    else
                    {
                        Kernel.Domain.Role valid = ValidateName(editedMeasure, name);
                        editedMeasure.name = valid.name;
                        editedMeasure.parent.UpdateChild(editedMeasure);
                    }
                    if (Changed != null) Changed();
                }
                //The event must be canceled, otherwise the TreeView will
                //set the TreeViewItem's Header to the new text
                e.Canceled = true;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void OnItemDragOver(object sender, EO.Wpf.ItemDragOverEventArgs e)
        {
            EO.Wpf.TreeViewItem item = (EO.Wpf.TreeViewItem)e.TargetItemContainer1;

        }

        //Handles the TreeView's ItemDrop event
        private void OnItemDrop(object sender, EO.Wpf.ItemDropEventArgs e)
        {
            Kernel.Domain.Role source = (Kernel.Domain.Role)e.SourceItem;
            Kernel.Domain.Role target1 = (Kernel.Domain.Role)e.TargetItem1;
            Kernel.Domain.Role target2 = (Kernel.Domain.Role)e.TargetItem2;
            Kernel.Domain.Role parent = target2 != null ? target2 : target1;

            parent.AddChild(source);
            e.Canceled = true;
            if (Changed != null) Changed();            
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
            Kernel.Domain.Role r = GetSelectedMultiRole();
            List<Kernel.Domain.Role> listeSelectedR = selectedRoles.Keys.ToList<Kernel.Domain.Role>();
            if ((listeSelectedR.Count == 0) && r != null) listeSelectedR.Add(r);
            isTreeInCutMode = false;
            SetInClipboard(listeSelectedR);
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
            isTreeInCutMode = true;
            Kernel.Domain.Role r = GetSelectedRole();
            List<Kernel.Domain.Role> listeSelectedRole = selectedRoles.Keys.ToList<Kernel.Domain.Role>();
            if ((listeSelectedRole.Count == 0) && r != null) listeSelectedRole.Add(r);
            Kernel.Domain.Role parent = r.parent;
            SetInClipboard(listeSelectedRole, parent);
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
            Kernel.Domain.Role parent = GetSelectedRole();
           
            if(selectedRoles.Count > 0)
            if (selectedRoles.ContainsKey(GetSelectedMultiRole())) parent = GetSelectedMultiRole();
            getRolesFromClipboard(Kernel.Util.ClipbordUtil.GetRole(), parent);

       }
        

   
        /// <summary>
        /// Cette méthode permet de vérifier si une measure de l'arbre possède un nom identique à celui d'une measure qui
        /// vient du presse-papier. S'il y a identité de nom, un nouveau nom est donné à la measure venant du presse-papier. 
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <returns>La measure à copier</returns>
        private Kernel.Domain.Role ValidateName(Kernel.Domain.Role role, string name) 
     {
         bool result = true;
         role.name = name;
         Kernel.Domain.Role currentMeasure = role.CloneObject() as Kernel.Domain.Role;
         if(string.IsNullOrEmpty(name))
         {
             Kernel.Util.MessageDisplayer.DisplayError("Empty Measure", "Measure can't be empty! ");
             result = false;
         }
         Kernel.Domain.Role m = (Kernel.Domain.Role)Root.GetNotEditedChildByName(role, name);

         if (m == null) return role;

         if (m != null && m.Equals(role))
         {
             if (role.IsDefault) result = false;
             result = true;
         }
         if (m != null && !m.Equals(role))
         {
             currentMeasure = currentMeasure.GetCopy() as Kernel.Domain.Role;
             currentMeasure = ValidateName(currentMeasure, currentMeasure.name);
         }
         if(result)
             return currentMeasure;
         return null;
     }

       
        ///// <summary>
        ///// verifie l'ordre de selection utilisé.
        ///// </summary>
        ///// <returns>true if selection is in block UP to Down or Down to Up</returns>
        //public bool SelectionOrder()
        //{
        //    int i = 0;
        //    bool IsSelectionBlock = true;
        //    indexSelecte = new List<int>(0);
        //    if(selectedRoles.Keys.Count>0)
        //    {
        //        indexSelecte.Add(selectedRoles.Keys.ElementAt(i).GetPosition());
        //        while (i < (selectedRoles.Keys.Count() - 1) && IsSelectionBlock)
        //        {
        //            indexSelecte.Add(selectedRoles.Keys.ElementAt(i + 1).GetPosition());
        //            IsSelectionBlock = ((selectedRoles.Keys.ElementAt(i).GetPosition() < selectedRoles.Keys.ElementAt(i + 1).GetPosition()) && (selectedRoles.Keys.ElementAt(i).GetPosition() + 1 == selectedRoles.Keys.ElementAt(i + 1).GetPosition())) || ((selectedRoles.Keys.ElementAt(i).GetPosition() > selectedRoles.Keys.ElementAt(i + 1).GetPosition()) && (selectedRoles.Keys.ElementAt(i).GetPosition() - 1 == selectedRoles.Keys.ElementAt(i + 1).GetPosition())) ? true : false;
        //            if(IsSelectionBlock)
        //            if (selectedRoles.Keys.ElementAt(i).parent != selectedRoles.Keys.ElementAt(i + 1).parent)
        //                return false;
        //            //verify if selection order is up to down, return false otherwise
        //            this.isSelectionDownToUp = (selectedRoles.Keys.ElementAt(i).GetPosition() > selectedRoles.Keys.ElementAt(i + 1).GetPosition()) && (selectedRoles.Keys.ElementAt(i).GetPosition() - 1 == selectedRoles.Keys.ElementAt(i + 1).GetPosition());
               
        //            i++;
        //        }
        //        indexSelecte.BubbleSort();
        //        return IsSelectionBlock;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Cette méthode permet de désactiver un menuItem dans le cas
        /// où l'opération associée à ce menuItem n'est pas possible pour
        /// le noeud courant.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {

            Kernel.Domain.Role selectedItem = GetSelectedRole();
            if (selectedRoles.Count > 0 && selectedItem == null)
            {
                selectedItem = selectedRoles.Keys.Last();
            }

            if (Root != null)
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;
                this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectedRoles.Count <= 1;
                this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue ;
                this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyMeasure() && selectedItem != defaultValue && selectedRoles.Count <= 1;
                this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue ;                
            }
            else
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        #endregion   

        #region Node Model Actions


        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="parent">Le noeud auquel il fau ajouter un fils</param>
        /// <param name="name">le nom du noeud</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual Kernel.Domain.Role AddNode(Kernel.Domain.Role parent, string name="")
        {
            Kernel.Domain.Role child = GetNewTreeViewModel();
            if (name != "") child.name = name;
            this.Root.AddChild(child);

            if (Changed != null) Changed();
            SetSelectedRole(defaultValue);
            return child;
        }
        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(Kernel.Domain.Role item)
        {
            if(item != null && item.parent != null)
            {
                    item.parent.RemoveChild(item);
                    if (Changed != null) Changed();
            }
        }
        

        #endregion;

        #region Copy/Cut/Paste Methods

        /// <summary>
        /// Mets les éléments selectionnés dans le presse-papiers.
        /// </summary>
        /// <param name="copieListMeasure"></param>
        /// <param name="isCut">true=>cut/false=>copy</param>
        private void SetInClipboard(List<Kernel.Domain.Role> copieListMeasure, Kernel.Domain.Role parent = null)
        {
            if (copieListMeasure.Count > 0)
            {
                Kernel.Util.ClipbordUtil.ClearClipboard();
                List<IHierarchyObject> listeCopy = new List<IHierarchyObject>(0);
                if (!isTreeInCutMode)
                {
                    foreach (Kernel.Domain.Role roleToCopy in copieListMeasure)
                    {
                        listeCopy.Add(roleToCopy.GetCopy());
                    }
                }
                else
                {
                    listeCopy.AddRange(copieListMeasure);
                    if (parent == null) parent = this.Root;
                    int nbreCutElement = listeCopy.Count;
                    
                }
                Kernel.Util.ClipbordUtil.SetHierarchyObject(listeCopy);
            }
        }



        /// <summary>
        /// Colle les éléments dans treeview après un copy/cut.
        /// </summary>
        /// <param name="measuresInClipboard">les éléments présents dans le presse-papier</param>
        /// <param name="isCutOperation">Le mode false=>copy/true=>cut</param>
        /// <param name="parent">Le parent sur lequel on effectue l'opération.</param>
        private void getRolesFromClipboard(List<Kernel.Domain.Role> rolesInClipboard, Kernel.Domain.Role parent = null)
        {
            if (rolesInClipboard != null && rolesInClipboard.Count > 0)
            {
                if (isTreeInCutMode)
                {
                    int nbCutElement = rolesInClipboard.Count;
                    for (int i = 0; i < nbCutElement; i++)
                    {
                        AddToTreeCopiedElements(RestoreMeasureForeground(rolesInClipboard[i]), parent);
                    }
                    isTreeInCutMode = false;
                    SetInClipboard(rolesInClipboard);
                    if (Changed != null) Changed();
                }
                else
                {
                    int nbreCopies = rolesInClipboard.Count;
                    if (rolesInClipboard != null && nbreCopies > 0)
                    {
                        for (int j = 0; j <= nbreCopies - 1; j++)
                        {
                            RestoreMeasureForeground(rolesInClipboard[j]);
                            pasteValue(rolesInClipboard[j], rolesInClipboard[j].name, parent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cette méthode permet de rétablir la couleur d'une measure. Ceci en spécifiant que la 
        /// measure copiée n'est pas la mesure par défaut.
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        private Kernel.Domain.Role RestoreMeasureForeground(Kernel.Domain.Role role)
        {
            role.IsDefault = false;
            int j = 0;
            for (int i = role.GetItems().Count - 1; i >= 0; i--)
            {
                Kernel.Domain.Role subMeasure = role.childrenListChangeHandler.Items[j];
                subMeasure = ValidateName(subMeasure, subMeasure.name);
                if (subMeasure.name != role.childrenListChangeHandler.Items[j].name)
                    role.childrenListChangeHandler.Items[j].name = subMeasure.name;
                RestoreMeasureForeground(subMeasure);
                j++;
            }
            return role;
        }

        /// <summary>
        /// Cette méthod ajoute les éléments du presse-papier à l'arbre.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="parent"></param>
        private void AddToTreeCopiedElements(Kernel.Domain.Role r, Kernel.Domain.Role parent = null)
        {
            if (parent != null)
            {
                parent.AddChild(r);
            }
            else
            {
                this.Root.AddChild(r);
            }
            if (Changed != null) Changed();

            this.Root.AddChild(defaultValue);
        }

        /// <summary>
        /// Cette fonction permet d'ajouter à l'arbre les éléments (Target) venus du presse-papier.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void pasteValue(Kernel.Domain.Role r, string name, Kernel.Domain.Role parent = null)
        {
            Kernel.Domain.Role clipboardAddedRole = ValidateName(r, name);
            if (clipboardAddedRole != null)
            {
                AddToTreeCopiedElements(clipboardAddedRole, parent);
            }
        }

      
        #endregion
       
        protected Kernel.Domain.Role GetNewTreeViewModel(Domain.Role value = null)
        {
                Kernel.Domain.Role role = new Kernel.Domain.Role();
                role.name = "Role1";
                if (Root != null)
                {
                    Kernel.Domain.Role m = null;
                    int i = 1;
                    do
                    {
                        role.name = "Role" + i++;
                        m = (Kernel.Domain.Role)Root.GetChildByName(role.name);
                    }
                    while (m != null);
                }
                return role;
        }

        

        
    }
}