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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Misp.Kernel.Util;
namespace Misp.Kernel.Ui.Attribute
{
    /// <summary>
    /// Interaction logic for MeasureTreeView.xaml
    /// 
    /// Cette classe implémente l'arbre d'édition des mesure.
    /// </summary>
    public partial class AttributeTreeView : ScrollViewer
    {

        #region events
        
        public ChangeEventHandler Changed;
        
        #endregion
   
        #region properties
        static string Label_DEFAULT_ATTRIBUTE = "Add attribute";
        public Kernel.Domain.Entity Entity { get; set; }
        public Kernel.Domain.Attribute Root { get; set; }
        public Kernel.Domain.Attribute defaultValue = new Kernel.Domain.Attribute();
        public AttributeValueTreeView attributeValueTree { get; set; }
        private bool isTreeInCutMode { get; set; }

        private bool CanMoveUp
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedAttributes.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;

                if (result && selectedAttributes.Keys.ElementAt(0).GetParent() != this.Root)
                    result = selectedAttributes.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;

                if (result && listeIndex.Count > 1) result = isContiguousList();

                return result;
            }
        }

        private bool CanMoveDown
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedAttributes.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[lastIndex] != defaultValue.GetPosition() - 1;

                if (result && selectedAttributes.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedAttributes.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;
                    if (result) result = selectedAttributes.Keys.ElementAt(lastIndex).GetPosition() != selectedAttributes.Keys.ElementAt(lastIndex).GetParent().GetItems().Count - 1;

                }
                if (result && listeIndex.Count > 1) result = isContiguousList();

                return result;
            }
        }

        #endregion
     
        #region Constructor

        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public AttributeTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
          //  this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.Attribute();
            this.defaultValue = new Domain.Attribute();
            this.defaultValue.IsDefault = true;
            defaultValue.name = Label_DEFAULT_ATTRIBUTE;
        }

        public void DisplayEntity(Kernel.Domain.Entity entity)
        {
            this.Entity = entity;
            if (entity != null)
            {
                Kernel.Domain.Attribute root = new Kernel.Domain.Attribute();
                root.childrenListChangeHandler = entity.attributeListChangeHandler;
                //root.entity = entity;
                this.DisplayRoot(root);
            }
            else
            {
                this.DisplayRoot(null);
            }
        }
        
        /// <summary>
        /// Affiche le contenu de la mesure
        /// </summary>
        /// <param name="root"></param>
        private void DisplayRoot(Kernel.Domain.Attribute root)
        {
            this.Root = root;

            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {
                ForgetDefault(defaultValue);
                RefreshParent(this.Root);
                this.Root.AddChild(defaultValue);
                SetSelectedAttribute(defaultValue);
                this.tree.ItemsSource = this.Root.GetItems();
            }
        }

        public void ForgetDefault(Kernel.Domain.Attribute defaultvalue)
        {
            this.Root.ForgetChild(defaultvalue);
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
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(Misp.Kernel.Domain.Attribute));
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
            this.MouseDown +=new MouseButtonEventHandler(OnMouseDown);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
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
            catch (Exception) { }
        }

        /// <summary>
        /// Vérifie si l'on va cliquer sur le noeud par défaut afin d'activer le mode
        /// édition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemClick(object sender, MouseButtonEventArgs e)
        {
            try{
                Kernel.Domain.Attribute attribute;
                EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (treeViewItem != null)
                {
                    SelectionManager(treeViewItem, false);
                }
                else
                {
                    GetSelectedMultiAttribute();
                    attribute = GetSelectedAttribute();
                    tree.Focus();
                }
            }catch(Exception){}
        }

        private void SelectionManager(EO.Wpf.TreeViewItem treeViewItem, bool isRightClick=true) 
        {
            Kernel.Domain.Attribute attribute;

            if (treeViewItem.Header.ToString() == defaultValue.name)
            {
                attribute = null;
                treeViewItem.Focus();
            }
            else
            {
                attribute = this.Root.GetChildByName(treeViewItem.Header.ToString()) as Domain.Attribute;
                if (!CtrlPressed)
                {
                    if (!isRightClick || (isRightClick && !selectedItems.ContainsKey(treeViewItem) && !selectedAttributes.Keys.Contains(attribute)))
                    {
                        removeCTRLSelection();
                        selectedItems.Add(treeViewItem, null);
                        SetSelectedItem(treeViewItem);
                        selectedAttributes.Add(attribute, attribute.GetPosition());
                    }
                }
                else if (CtrlPressed && !selectedItems.ContainsKey(treeViewItem) && !selectedAttributes.Keys.Contains(attribute))
                {
                    selectedItems.Add(treeViewItem, null);
                    selectedAttributes.Add(attribute, attribute.GetPosition());
                    SetSelectedItem(treeViewItem);
                }
            }
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

            this.moveUpMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Moveup.png", UriKind.Relative)) };
            this.moveUpMenuItem.Click += new RoutedEventHandler(OnMoveUpNode);

            this.moveDownMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Movedown.png", UriKind.Relative)) };
            this.moveDownMenuItem.Click += new RoutedEventHandler(OnMoveDownNode);

        }

        #region Multiselection Bloc

        // a set of all selected measures
        public Dictionary<Kernel.Domain.Attribute, int> selectedAttributes = new Dictionary<Domain.Attribute, int>();

        // a set of all selected items
        Dictionary<EO.Wpf.TreeViewItem, string> selectedItems = new Dictionary<EO.Wpf.TreeViewItem, string>();
        //get blue selection
        public static SolidColorBrush SELECTION_COLOR = new SolidColorBrush(Color.FromRgb(51, 153, 255));

        // true only while left ctrl key is pressed
        bool CtrlPressed
        {
            get
            {
                return (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl));
            }
        }
        // deselects the tree item
        void Deselect(EO.Wpf.TreeViewItem treeViewItem)
        {
            treeViewItem.Background = Brushes.Transparent;// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }

  

        void SetSelectedItem(EO.Wpf.TreeViewItem treeViewItem,bool select=true) 
        {
            if (select)
            {
                treeViewItem.Background = SELECTION_COLOR; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
            }
            else 
            {
                treeViewItem.Background = Brushes.Transparent;// change background and foreground colors
                treeViewItem.Foreground = Brushes.Black;
            }
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
                selectedAttributes.Clear();
            }
        }
        
        #endregion

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
                Kernel.Domain.Attribute measure = GetSelectedAttribute();
                if (measure != null) measure.IsSelected = false;
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
        /// <returns>La mesure sélectionnée</returns>
        public Kernel.Domain.Attribute GetSelectedAttribute()
        {
            return this.tree.SelectedItem != null ?
                this.tree.SelectedItem as Kernel.Domain.Attribute : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée dans le cas de multiselection mode</returns>
        public Kernel.Domain.Attribute GetSelectedMultiAttribute()
        {
            Kernel.Domain.Attribute attribute = null;
            attribute = this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.Attribute :
                     (this.tree.SelectedItem == null && this.tree.SelectedValue != null && this.tree.SelectedValue != defaultValue ?
                     (this.tree.SelectedValue as Kernel.Domain.Attribute) : null);
            return attribute;
        }

        /// <summary>
        /// Selectionne une mesure dans l'arbre
        /// </summary>
        /// <param name="attribute">La mesure à sélectionner</param>
        public void SetSelectedAttribute(Kernel.Domain.Attribute measure)
        {
            if (measure != null)
            {
                if (measure.parent != null) measure.parent.IsExpanded = true;
                measure.IsSelected = true;
                
            }
            else
            {
                Kernel.Domain.Attribute selection = GetSelectedAttribute()==null?GetSelectedMultiAttribute():GetSelectedAttribute();
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
        /// verifie l'ordre de selection utilisé.
        /// </summary>
        /// <returns>true if selection is in block UP to Down or Down to Up</returns>
        public bool isContiguousList()
        {
            bool result = true;
            if (selectedAttributes.Count <= 1) return true;

            List<int> listeIndex = selectedAttributes.Values.ToList();
            listeIndex.BubbleSort();
            int i = 0;
            foreach(int index in listeIndex)
            {
                if (i == listeIndex.Count - 1) break;
                int valCurrent = listeIndex[i];
                int valNext = listeIndex[i + 1];
                if (valCurrent != valNext - 1)
                {
                    result = false;
                    break;
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            if (this.Entity != null)
            {
                if (this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
                {
                    string message = "You're not allowed to add a new attribute." + "\n" + "You have to clear allocation before add attribute.";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Add attribute", message);
                    return;
                }
            }
            Kernel.Domain.Attribute parent = null;
            Kernel.Domain.Attribute attribute = addDefaultNode(parent);
            removeCTRLSelection();
        }
        
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Delete".
        /// Elle permet de supprimer un noeud de l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteNode(object sender, RoutedEventArgs e)
        {
            if (this.Entity != null && this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to delete this attribute." + "\n" + "You have to clear allocation before delete this attribute.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Delete attribute", message);
                return;
            }
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Measure", "Do you want to delete selected attribute ?");
            if (result == MessageBoxResult.Yes)
            {
               this.Root.ForgetChild(defaultValue);
               foreach (Domain.Attribute attribute in selectedAttributes.Keys)
                {
                    DeleteNode(attribute);
                }
               this.Root.AddChild(defaultValue);
               SetSelectedAttribute(defaultValue);
            }

        }
        
        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)e.Item;
                e.Text = attribute.name;
            }
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute editedMeasure = (Kernel.Domain.Attribute)e.Item;
                    string name = e.Text.Trim();
                   
                    Kernel.Domain.Attribute ValidAttribute = ValidateName(editedMeasure, name);
                    if (ValidAttribute==null)
                    {
                        e.Canceled = true;
                        return;
                    }
                    if (editedMeasure.IsDefault)
                    {
                        if (this.Entity != null && this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
                        {
                            string message = "You're not allowed to add new attribute." + "\n" + "You have to clear allocation before add new attribute.";
                            Kernel.Util.MessageDisplayer.DisplayWarning("Add Attribute", message);
                            e.Canceled = true;
                            return;
                        }

                        if (name.ToUpper() != Label_DEFAULT_ATTRIBUTE.ToUpper())
                        {
                            Kernel.Domain.Attribute addedNode = new Domain.Attribute();
                            addedNode.name = name;
                            editedMeasure.name = Label_DEFAULT_ATTRIBUTE;
                            editedMeasure.parent.UpdateChild(editedMeasure);

                            addedNode = ValidateName(addedNode, name);
                            AddNode(null, addedNode.name);

                            e.Canceled = true;
                        }
                     }
                    else
                    {
                        Kernel.Domain.Attribute valid = ValidateName(editedMeasure, name);
                        editedMeasure.name = valid.name;
                        editedMeasure.parent.UpdateChild(editedMeasure);
                    }
                  }
                    if (Changed != null) Changed();
                
                //The event must be canceled, otherwise the TreeView will
                //set the TreeViewItem's Header to the new text
                e.Canceled = true;
            }
            catch (Exception) 
            {
                return;
            }
        }
      
        private void OnItemDragOver(object sender, EO.Wpf.ItemDragOverEventArgs e)
        {
            EO.Wpf.TreeViewItem item = (EO.Wpf.TreeViewItem)e.TargetItemContainer1;

            //Only allow items to be dragged onto the Celebrity items,
            //not CelebrityCategory items
            //if (item.Level != 1)
            //    e.Canceled = true;
        }

        //Handles the TreeView's ItemDrop event
        private void OnItemDrop(object sender, EO.Wpf.ItemDropEventArgs e)
        {
            Kernel.Domain.Attribute source = (Kernel.Domain.Attribute)e.SourceItem;
            Kernel.Domain.Attribute target1 = (Kernel.Domain.Attribute)e.TargetItem1;
            Kernel.Domain.Attribute target2 = (Kernel.Domain.Attribute)e.TargetItem2;
            Kernel.Domain.Attribute parent = target2 != null ? target2 : target1;

            source.parent.ForgetChild(source);            
            parent.AddChild(source);
            if (target2 != null) parent.SwichtPosition(target1, source);
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
            SetInClipboard(selectedAttributes.Keys.ToList<Kernel.Domain.Attribute>(), isTreeInCutMode);
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
            var listCut = selectedAttributes.Keys.ToList<Kernel.Domain.Attribute>();
            SetInClipboard(listCut, isTreeInCutMode);
            foreach (Domain.Attribute attribute in listCut)
            {
                DeleteNode(attribute);
            }
            if (Changed != null) Changed();
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
            if (this.Entity != null && this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to add new attribute." + "\n" + "You have to clear allocation before add new attribute.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Paste", message);
                return;
            }
            Domain.Attribute attribute = Kernel.Util.ClipbordUtil.GetAttribute()[0];
            getAttributesFromClipboard(Kernel.Util.ClipbordUtil.GetAttribute());
            System.Collections.ObjectModel.ObservableCollection<Domain.Attribute> l = (System.Collections.ObjectModel.ObservableCollection<Domain.Attribute>)this.tree.ItemsSource;
            //Domain.Attribute attributeFromSource = l[l.IndexOf(attribute)];
            Domain.Attribute att = null;
            foreach(Domain.Attribute att1 in l)
            {
                if (att1.name.Equals(attribute.name))
                    att = att1;

            }
            SetSelectedAttribute(att);
            this.tree.Items.Refresh();
            
        }

        /// <summary>
        /// Cette méthode permet de vérifier si un AttributeValue de l'arbre possède un nom identique à celui d'une attribute qui
        /// vient du presse-papier. S'il y a identité de nom, un nouveau nom est donné à l' AttributeValue venant du presse-papier. 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="name"></param>
        /// <returns>La attribute à copier</returns>
        private Kernel.Domain.Attribute ValidateName(Kernel.Domain.Attribute attribute, string name)
        {
            bool result = true;
            attribute.name = name;
            Kernel.Domain.Attribute currentAttribute = attribute.CloneObject() as Kernel.Domain.Attribute;
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty value", "Value can't be empty! ");
                result = false;
            }
            Kernel.Domain.Attribute m = (Kernel.Domain.Attribute)Root.GetNotEditedChildByName(attribute , name);
            if (m == null) return attribute;
            if (m != null && m.Equals(attribute))
            {
                if (attribute.IsDefault) result = false;
                result = true;
            }
            if ((m != null && !m.Equals(attribute)))
            {
                currentAttribute = currentAttribute.GetCopy() as Kernel.Domain.Attribute;
                currentAttribute = ValidateName(currentAttribute, currentAttribute.name);
            }
            if (result)
                return currentAttribute;
            return null;
        }
       
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sure le menu "Move Down".
        /// Elle permet de déplacer un noeud vers le bas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveDownNode(object sender, RoutedEventArgs e)
        {
            List<Kernel.Domain.Attribute> moveDownElements = selectedAttributes.Keys.ToList();
            int j = 0;
            for (int i = moveDownElements.Count - 1; i >= 0; i--)
            {
                Kernel.Domain.Attribute attribute = moveDownElements[j];
                MoveNode(attribute, false);
                j++;
            }
            removeCTRLSelection();
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Move Up".
        /// Elle permet de déplacer un noeud vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveUpNode(object sender, RoutedEventArgs e)
        {
            List<Kernel.Domain.Attribute> moveUpElements = selectedAttributes.Keys.ToList();
            int j = 0;
            for (int i = moveUpElements.Count - 1; i >= 0; i--)
            {
                Kernel.Domain.Attribute attribute = moveUpElements[j];
                MoveNode(attribute, true);
                j++;
            }
            removeCTRLSelection();
        }


        /// <summary>
        /// Cette méthode permet de désactiver un menuItem dans le cas
        /// où l'opération associée à ce menuItem n'est pas possible pour
        /// le noeud courant.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            GetSelectedAttribute();
            Kernel.Domain.Attribute selectedItem = GetSelectedAttribute();
            if (selectedAttributes.Count > 0 && selectedItem ==null) 
            {
                selectedItem = selectedAttributes.Keys.Last();
            }

            if (Root != null)
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;

                this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectedAttributes.Count <= 1;
                this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyAttribute() && selectedItem != defaultValue && isContiguousList();
                this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.moveUpMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue && CanMoveUp;
                this.moveDownMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue && CanMoveDown;
            }
            else
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion

        #region Copy/Cut/paste methods

        /// <summary>
        /// Mets les éléments selectionnés dans le presse-papiers.
        /// </summary>
        /// <param name="copieListAttribute"></param>
        /// <param name="isCut">true=>cut/false=>copy</param>
        private void SetInClipboard(List<Kernel.Domain.Attribute> copieListAttribute, bool isCut)
        {
            if (copieListAttribute.Count > 0)
            {
                Kernel.Util.ClipbordUtil.ClearClipboard();
                List<IHierarchyObject> listeCopy = new List<IHierarchyObject>(0);
                List<IHierarchyObject> listeCut = new List<IHierarchyObject>(0);

                if (!isCut)
                {
                    foreach (Kernel.Domain.Attribute measureToCopy in copieListAttribute)
                    {
                        listeCopy.Add(measureToCopy.GetCopy());
                    }
                }
                else
                { 
                    listeCopy.AddRange(copieListAttribute);
                    int nbreCutElement = listeCopy.Count;
                    if (nbreCutElement > 0)
                    {
                        for (int i = 0; i < nbreCutElement; i++)
                        {
                            this.Root.ForgetChild(listeCopy[i]);
                        }
                    }
                }
                foreach (Kernel.Domain.Attribute measureToCopy in listeCopy)
                {
                    listeCut.Add(measureToCopy.removeDefaultValue());
                }
                Kernel.Util.ClipbordUtil.SetHierarchyObject(listeCut);
            }
        }

        /// <summary>
        /// Colle les éléments dans treeview après un copy/cut.
        /// </summary>
        /// <param name="attributesInClipboard">les éléments présents dans le presse-papier</param>
        /// <param name="isCutOperation">Le mode false=>copy/true=>cut</param>
        /// <param name="parent">Le parent sur lequel on effectue l'opération.</param>
        private void getAttributesFromClipboard(List<Kernel.Domain.Attribute> attributesInClipboard, Kernel.Domain.Attribute parent = null)
        {
            if (attributesInClipboard != null && attributesInClipboard.Count > 0)
            {
                if (isTreeInCutMode)
                {
                    int nbCutElement = attributesInClipboard.Count;
                    bool doValidName =nbCutElement > 0 && attributesInClipboard[0].entity != this.Entity;
                    
                    for (int i = 0; i < nbCutElement; i++)
                    {
                        if (doValidName)
                        {
                            attributesInClipboard[i].entity = this.Entity;
                            attributesInClipboard[i] = ValidateName(attributesInClipboard[i], attributesInClipboard[i].name);
                        }
                        AddToTreeCopiedElements(RestoreAttributeForeground(attributesInClipboard[i]), parent);
                    }
                    isTreeInCutMode = false;
                    SetInClipboard(attributesInClipboard, isTreeInCutMode);
                    if(nbCutElement > 0) SetSelectedAttribute(attributesInClipboard[0]);
                    if (Changed != null) Changed();
                }
                else
                {
                    int nbreCopies = attributesInClipboard.Count;
                    if (attributesInClipboard != null && nbreCopies > 0)
                    {
                        for (int j = 0; j <= nbreCopies - 1; j++)
                        {
                            RestoreAttributeForeground(attributesInClipboard[j]);
                            pasteValue(attributesInClipboard[j], attributesInClipboard[j].name, parent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cette fonction permet d'ajouter à l'arbre les éléments (Target) venus du presse-papier.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void pasteValue(Kernel.Domain.Attribute attribute, string name, Kernel.Domain.Attribute parent = null)
        {
            Kernel.Domain.Attribute clipboardAddedAttribute = ValidateName(attribute, name);
            if (clipboardAddedAttribute != null)
            {
                AddToTreeCopiedElements(clipboardAddedAttribute, parent);
                int nbreAttributes = attribute.childrenListChangeHandler.Items.Count;
                for (int i = 0; i < nbreAttributes - 1; i++)
                {
                    pasteValue(attribute.childrenListChangeHandler.Items[i], attribute.childrenListChangeHandler.Items[i].name, clipboardAddedAttribute);
                }
            }
        }

        /// <summary>
        /// Cette méthod ajoute les éléments du presse-papier à l'arbre.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parent"></param>
        private void AddToTreeCopiedElements(Kernel.Domain.Attribute measure, Kernel.Domain.Attribute parent = null)
        {
            this.Root.ForgetChild(defaultValue);
            if (parent != null)
            {
                parent.AddChild(measure);
            }
            else
            {
                this.Root.AddChild(measure);
            }
            if (Changed != null) Changed();

            this.Root.AddChild(defaultValue);
        }

        /// <summary>
        /// Cette méthode permet de rétablir la couleur d'un attribute. Ceci en spécifiant que l' 
        /// attribute copié n'est pas l'attribute par défaut.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private Kernel.Domain.Attribute RestoreAttributeForeground(Kernel.Domain.Attribute attribute)
        {
            attribute.IsDefault = false;
            foreach (Kernel.Domain.AttributeValue value in attribute.valueListChangeHandler.Items)
            {
                RestoreValueForeground(value);
            }
            foreach (Kernel.Domain.Attribute subMeasure in attribute.GetItems())
            {
                RestoreAttributeForeground(subMeasure);
            }
            return attribute;
        }

        /// <summary>
        /// Cette méthode permet de rétablir la couleur d'un value. Ceci en spécifiant que le 
        /// value copiée n'est pas le value par défaut.
        /// </summary>
        private Kernel.Domain.AttributeValue RestoreValueForeground(Kernel.Domain.AttributeValue value)
        {
            if (!value.IsDefault) value.IsDefault = false;
            foreach (Kernel.Domain.AttributeValue subValue in value.GetItems())
            {
                RestoreValueForeground(subValue);
            }
            return value;
        }
        #endregion


        #region Node Model Actions

        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="parent">Le noeud auquel il fau ajouter un fils</param>
        /// <param name="name">le nom du noeud</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual Kernel.Domain.Attribute AddNode(Kernel.Domain.Attribute parent,string name="")
        {
            Kernel.Domain.Attribute child = GetNewTreeViewModel();
            if (name != "") child.name = name;
            if (parent != null)
            {
                parent.AddChild(child);
                parent.UpdateParents();
            }
            else
            {
                if (this.Root != null)
                {
                    this.Root.AddChild(child);
                    this.Root.SwichtPosition(defaultValue, child);
                }
                else return null;
            }
            SetSelectedAttribute(defaultValue);
            if (Changed != null) Changed();
            return child;
        }

        /// <summary>
        /// Ajoute la valeur par défaut après avoir créer une mesure
        /// </summary>
        /// <param name="value">le parent de l'attribut à créer</param>
        /// <returns>l'attribut créé</returns>
        public Domain.Attribute addDefaultNode(Kernel.Domain.Attribute value = null)
        {
            Domain.Attribute attribute = AddNode(value);
            return attribute;
        }
        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(Kernel.Domain.Attribute item)
        {
            if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
                {
                    string message = "You're not allowed to delete Attribute " + item.name + "\n" + "You have to clear allocation before delete this Attribute";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Delete Attribute", message);
                }
                else if (item != null && item.parent != null)
                {
                    int index = item.GetPosition();
                    item.GetParent().RemoveChild(item);
                    if (Changed != null) Changed();
                }
            }
        
        /// <summary>
        /// Déplace un noeud vers le haut ou vers le bas
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sens">Le sens du déplacement. 0 => UP, 1=> DOWN</param>
        public void MoveNode(Kernel.Domain.Attribute item, bool up)
        {
            if (item.parent != null)
            {
                int position = item.position + (up ? -1 : 1);
                IHierarchyObject child = item.parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(item.position); 
                    item.parent.UpdateChild(child);
                    item.SetPosition(position);
                    item.parent.UpdateChild(item);
                    if (Changed != null) Changed();
                    SetSelectedAttribute(item);
                }
            }
        }

        /// <summary>
        /// Transforme un noeud en sous-noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void IndentNode(Kernel.Domain.Attribute item)
        {
            if (item.parent != null)
            {
                int position = item.GetPosition();
                IHierarchyObject child = item.parent.GetChildByPosition(position - 1);
                if (child != null)
                {
                    item.parent.ForgetChild(item);
                    child.AddChild(item);
                    if (Changed != null) Changed();
                    SetSelectedAttribute(item);
                }
            }
        }

        /// <summary>
        /// Transforme un sous-noeud en noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void OutdentNode(Kernel.Domain.Attribute item)
        {
            if (item.parent != null)
            {
                IHierarchyObject parent = item.parent.GetParent();
                if (parent != null)
                {
                    item.parent.ForgetChild(item);
                    parent.AddChild(item);
                    if (Changed != null) Changed();
                    SetSelectedAttribute(item);
                }
            }
        }



          
        #endregion;

        protected Kernel.Domain.Attribute GetNewTreeViewModel()
        {
            Kernel.Domain.Attribute measure = new Kernel.Domain.Attribute();
            measure.name = "Attribute";
            if (Root != null)
            {
                Kernel.Domain.Attribute m = null;
                int i = 1;
                do
                {
                    measure.name = "Attribute" + i++;
                    m = (Kernel.Domain.Attribute)Root.GetChildByName(measure.name);
                }
                while (m != null);
            }
            return measure;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
             //SetSelectedAttribute(this.GetSelectedAttribute());
        }
    }
}
