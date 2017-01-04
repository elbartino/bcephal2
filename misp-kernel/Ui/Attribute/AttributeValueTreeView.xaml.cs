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
using Misp.Kernel.Util;
namespace Misp.Kernel.Ui.Attribute
{
    /// <summary>
    /// Interaction logic for AttributeValueTreeView.xaml
    /// 
    /// Cette classe implémente l'arbre d'édition des AttributeValue.
    /// </summary>
    public partial class AttributeValueTreeView : ScrollViewer
    {

        #region events
        
        public ChangeEventHandler Changed;

        #endregion

        #region Properties
        static string Label_DEFAULT_ATTRIBUTEVALUE = "Add value";
        public AttributeValue Root { get; set; } 
        public AttributeValue CurrentCutObject { get; set; }
        public AttributeValue CurrentCopiedObject { get; set; }

        public Kernel.Domain.Attribute Attribute { get; set; }
        public AttributeValue defaultValue { get; set; }

        private Service.ModelService modelService;

        private bool isTreeInCutMode = false;
        
        private bool CanMoveUp
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedValues.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;

                if (result && selectedValues.Keys.ElementAt(0).GetParent() != this.Root)
                    result = selectedValues.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;

                if (result && listeIndex.Count > 1) result = isContiguousList();

                return result;
            }
        }

        private bool CanMoveDown
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedValues.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[lastIndex] != defaultValue.GetPosition() - 1;

                if (result && selectedValues.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedValues.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;
                    if (result) result = selectedValues.Keys.ElementAt(lastIndex).GetPosition() != selectedValues.Keys.ElementAt(lastIndex).GetParent().GetItems().Count - 1;

                }
                if (result && listeIndex.Count > 1) result = isContiguousList();

                return result;
            }
        }

        private bool CanIndent
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedValues.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;
                if (result && selectedValues.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedValues.Keys.ElementAt(lastIndex).GetParent().GetItems().Count > listeIndex.Count;
                    if (result)
                    {
                        AttributeValue firstPosition = selectedValues.Keys.ElementAt(lastIndex).GetParent().GetChildByPosition(0) as Kernel.Domain.AttributeValue;
                        result = selectedValues.Keys.ElementAt(lastIndex) != firstPosition;
                    }
                }
                if (result && listeIndex.Count > 1) result = isContiguousList();
                return result;
            }
        }

        private bool CanOutdent
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedValues.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;
                if (listeIndex.Count > 0 && selectedValues.Keys.ElementAt(0).GetParent() == this.Root)
                    result = false;

                if (listeIndex.Count > 0 && selectedValues.Keys.ElementAt(0).GetParent() != this.Root)
                    result = true;

                if (result && listeIndex.Count > 1) result = isContiguousList();
                return result;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public AttributeValueTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.AttributeValue();
            defaultValue = new AttributeValue();
            this.defaultValue.IsDefault = true;
            defaultValue.name = Label_DEFAULT_ATTRIBUTEVALUE;
            this.modelService = Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetModelService();
        }

        /// <summary>
        /// Affiche le contenu
        /// </summary>
        /// <param name="root"></param>
        public void DisplayAttribute(Kernel.Domain.Attribute attribute)
        {
            this.Attribute = attribute;
           // if(attribute !=null &&  attribute.valueListChangeHandler.Items.Count > 0)
            DisplayRoot(attribute != null && !attribute.IsDefault ? attribute.GetRootValue() : null);
        }
        
        /// <summary>
        /// Affiche le contenu
        /// </summary>
        /// <param name="root"></param>
        private void DisplayRoot(AttributeValue root)
        {
            if (this.Root != null && this.defaultValue != null)
            {
                this.Root.ForgetChild(this.defaultValue);
            }

            this.Root = root;            

            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {                
                RefreshParent(this.Root);
                this.Root.AddChild(defaultValue);
                this.tree.ItemsSource = this.Root.GetItems();
                SetSelectedValue(defaultValue);
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
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(AttributeValue));
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

        /// <summary>
        /// Vérifie si l'on va cliquer sur le noeud par défaut afin d'activer le mode
        /// édition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemClick(object sender, MouseButtonEventArgs e)
        {
            Kernel.Domain.AttributeValue attributeValue;
            EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
           
            if (treeViewItem != null)
            {
                SelectionManager(treeViewItem, false);
            }
            else
            {
                attributeValue = GetSelectedValue();
                tree.Focus();
            }
        }

        //verifie le click sur un noeud
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
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


        #region Multiselection Bloc

        // a set of all selected measures
        Dictionary<Kernel.Domain.AttributeValue, int> selectedValues = new Dictionary<Domain.AttributeValue, int>();

        // a set of all selected items
        Dictionary<EO.Wpf.TreeViewItem, int> selectedItems = new Dictionary<EO.Wpf.TreeViewItem, int>();

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

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        void ChangeSelectedState(EO.Wpf.TreeViewItem treeViewItem)
        {
            AttributeValue  attributeValue = this.Root.GetChildByName(treeViewItem.Header.ToString()) as Domain.AttributeValue;
            if (!selectedItems.ContainsKey(treeViewItem))
            { // select

                treeViewItem.Background = SELECTION_COLOR; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
                selectedItems.Add(treeViewItem, attributeValue.GetPosition()); // add the item to selected items
            }
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }


        //met a jour la selection liste en cours
        private void UpdateSelectionList(Domain.AttributeValue selectedValue)
        {
            if (!selectedValues.ContainsKey(selectedValue))
            { // add
                selectedValues.Add(selectedValue, selectedValue.GetPosition());
            }
            else
            { // remove
                selectedValues.Remove(selectedValue);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="treeViewItem"></param>
        private void DoSelectedItemChanged(EO.Wpf.TreeViewItem treeViewItem)
        {
            Domain.AttributeValue selectedMeasure;
            if (treeViewItem == null)
            {
                removeCTRLSelection();
                return;
            }
            else
            {
                selectedMeasure = GetSelectedValue();
            }

            removeCTRLSelection();
            ChangeSelectedState(treeViewItem);
            if (selectedMeasure != null)
                UpdateSelectionList(selectedMeasure);
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
                selectedValues.Clear();
            }
        }

        private void SelectionManager(EO.Wpf.TreeViewItem treeViewItem, bool isRightClick)
        {
            Kernel.Domain.AttributeValue attributeValue;

            if (treeViewItem.Header.ToString() == defaultValue.name)
            {
                attributeValue = null;
                treeViewItem.Focus();
            }
            else
            {
                attributeValue = this.Root.GetChildByName(treeViewItem.Header.ToString()) as Domain.AttributeValue;
                if (!CtrlPressed)
                {
                    if (isRightClick)
                    {
                        if (!selectedItems.ContainsKey(treeViewItem) && !selectedValues.Keys.Contains(attributeValue))
                        {
                            removeCTRLSelection();
                            selectedItems.Add(treeViewItem, attributeValue.GetPosition());
                            SetSelectedItem(treeViewItem);
                            selectedValues.Add(attributeValue, attributeValue.GetPosition());                        
                        }
                    }
                    else
                    {
                        removeCTRLSelection();
                        selectedItems.Add(treeViewItem, attributeValue.GetPosition());
                        SetSelectedItem(treeViewItem);
                        selectedValues.Add(attributeValue, attributeValue.GetPosition());
                    }
                }
                else
                {
                    if (!selectedItems.ContainsKey(treeViewItem) && !selectedValues.Keys.Contains(attributeValue))
                    {
                        selectedItems.Add(treeViewItem, attributeValue.GetPosition());
                        selectedValues.Add(attributeValue, attributeValue.GetPosition());
                        SetSelectedItem(treeViewItem);
                    }
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

            this.indentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Indent.png", UriKind.Relative)) };
            this.indentMenuItem.Click += new RoutedEventHandler(OnIndentNode);

            this.outdentMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Outdent.png", UriKind.Relative)) };
            this.outdentMenuItem.Click += new RoutedEventHandler(OnOutdentNode);

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
                SelectionManager(treeViewItem,true);
                e.Handled = true;
            }
            else
            {
                Kernel.Domain.AttributeValue attributeValue = GetSelectedValue();
                if (attributeValue != null) attributeValue.IsSelected = false;
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
        public AttributeValue GetSelectedValue()
        {
            return this.tree.SelectedItem != null ?
                this.tree.SelectedItem as AttributeValue : null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée dans le cas de multiselection mode</returns>
        public Kernel.Domain.AttributeValue GetSelectedMultiValue()
        {
            Kernel.Domain.AttributeValue measure = null;
            measure = this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.AttributeValue :
                     (this.tree.SelectedItem == null && this.tree.SelectedValue != null && this.tree.SelectedValue != defaultValue ?
                     (this.tree.SelectedValue as Kernel.Domain.AttributeValue) : null);
            return measure;
        }

        /// <summary>
        /// Selectionne une mesure dans l'arbre
        /// </summary>
        /// <param name="measure">La mesure à sélectionner</param>
        public void SetSelectedValue(AttributeValue value)
        {
            if (value != null)
            {
                if (value.parent != null) value.parent.IsExpanded = true;
                value.IsSelected = true;
            }
            else
            {
                AttributeValue selection = GetSelectedValue();
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
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            AttributeValue parent = GetSelectedValue();
           
                if (parent!= null && parent.usedToGenerateUniverse && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.model != null
                  && this.Attribute.entity.usedToGenerateUniverse && this.Attribute.entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
                {
                    string message = "You're not allowed to add new value." + "\n" + "You have to clear allocation before add new value.";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Add Value", message);
                    return;
                }
            AttributeValue value = addDefaultNode(parent);
            if (parent != null)
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
           MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Value", "Do you really want to delete current selection ?");
            if (result == MessageBoxResult.Yes)
            {
                List<AttributeValue> deleteElements = selectedValues.Keys.ToList();
                int j = 0;
                for (int i = deleteElements.Count - 1; i >= 0; i--)
                {
                    Kernel.Domain.AttributeValue value = deleteElements[j];
                    if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0 && value.usedToGenerateUniverse
               && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
               && this.Attribute.entity.model.IsUniverseGenerated())
                    {
                        string message = "You're not allowed to delete value " +value.name+  "\n" + "You have to clear allocation before delete.";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Delete Value", message);
                        break;
                    }
                    DeleteNode(value);
                    j++;
                }
            }
        }
        
        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is AttributeValue)
            {
                AttributeValue value = (AttributeValue)e.Item;
                e.Text = value.name;
            }
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is Kernel.Domain.AttributeValue)
                {
                    Kernel.Domain.AttributeValue editedMeasure = (Kernel.Domain.AttributeValue)e.Item;
                    string name = e.Text.Trim();
                    Kernel.Domain.AttributeValue validName = ValidateName(editedMeasure, name);
                        if (validName == null)
                        {
                            e.Canceled = true;
                            return;
                        }
                        if (editedMeasure.IsDefault)
                        {

                            if (name.ToUpper() != Label_DEFAULT_ATTRIBUTEVALUE.ToUpper())
                            {
                                Kernel.Domain.AttributeValue addedNode = new Domain.AttributeValue();
                                addedNode.name = name;

                                editedMeasure.name = Label_DEFAULT_ATTRIBUTEVALUE;
                                editedMeasure.parent.UpdateChild(editedMeasure);

                                addedNode = ValidateName(addedNode, name);
                                AddNode(null, addedNode.name);

                                e.Canceled = true;
                            }
                        }
                        else
                        {
                            Kernel.Domain.AttributeValue valid = ValidateName(editedMeasure, name);
                            editedMeasure.name = valid.name;
                            editedMeasure.parent.UpdateChild(editedMeasure);
                        }
                    if (Changed != null) Changed();
                }
                
                //The event must be canceled, otherwise the TreeView will
                //set the TreeViewItem's Header to the new text
                e.Canceled = true;
            }
            catch (Exception)
            {
                return;
            }
        }
      
        /// <summary>
        /// Cette méthode permet de vérifier si un AttributeValue de l'arbre possède un nom identique à celui d'une measure qui
        /// vient du presse-papier. S'il y a identité de nom, un nouveau nom est donné à l' AttributeValue venant du presse-papier. 
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <returns>La measure à copier</returns>
        private Kernel.Domain.AttributeValue ValidateName(Kernel.Domain.AttributeValue attributeValue, string name)
        {
            bool result = true;
            attributeValue.name = name;
            Kernel.Domain.AttributeValue currentAttributeValue = attributeValue.CloneObject() as Kernel.Domain.AttributeValue;
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty value", "Value can't be empty! ");
                result = false;
            }
            Kernel.Domain.AttributeValue m = (Kernel.Domain.AttributeValue)Root.GetNotEditedChildByName(attributeValue, name);
            if (m == null) return attributeValue;
            if (m != null && m.Equals(attributeValue))
            {
                if (attributeValue.IsDefault) result = false;
                result = true;
            }
            if (m != null && !m.Equals(attributeValue))
            {
                currentAttributeValue = currentAttributeValue.GetCopy() as Kernel.Domain.AttributeValue;
                currentAttributeValue = ValidateName(currentAttributeValue, currentAttributeValue.name);
            }

            

            if (result)
                return currentAttributeValue;
            return null;
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
            AttributeValue source = (AttributeValue)e.SourceItem;
            AttributeValue target1 = (AttributeValue)e.TargetItem1;
            AttributeValue target2 = (AttributeValue)e.TargetItem2;
            AttributeValue parent = target2 != null ? target2 : target1;

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
            Kernel.Domain.AttributeValue value = GetSelectedMultiValue();
            List<Kernel.Domain.AttributeValue> listeSelectedMeasure = selectedValues.Keys.ToList<Kernel.Domain.AttributeValue>();
            if ((listeSelectedMeasure.Count == 0) && value != null) listeSelectedMeasure.Add(value);
            SetInClipboard(listeSelectedMeasure,isTreeInCutMode);
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
            AttributeValue value = GetSelectedValue();
            List<Kernel.Domain.AttributeValue> listeSelectedMeasure = selectedValues.Keys.ToList<Kernel.Domain.AttributeValue>();
            if ((listeSelectedMeasure.Count == 0) && value != null) listeSelectedMeasure.Add(value);
            AttributeValue parent = value.parent;
            SetInClipboard(listeSelectedMeasure, isTreeInCutMode,parent);
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
            AttributeValue parent = GetSelectedValue();
            if (parent != null && parent.usedToGenerateUniverse && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.model != null
                && this.Attribute.entity.usedToGenerateUniverse && this.Attribute.entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to add new value." + "\n" + "You have to clear allocation before add new value.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Paste", message);
                return;
            }

            
            if (selectedValues.Count > 0)
                if (selectedValues.ContainsKey(GetSelectedMultiValue())) parent = GetSelectedMultiValue();

            getValuesFromClipboard(Kernel.Util.ClipbordUtil.GetAttributeValues(), parent);
 
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sure le menu "Move Down".
        /// Elle permet de déplacer un noeud vers le bas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveDownNode(object sender, RoutedEventArgs e)
        {
            List<AttributeValue> moveDownElements = selectedValues.Keys.ToList();
            int j = 0;
            for(int i = moveDownElements.Count - 1; i >= 0; i--)
            {
                Domain.AttributeValue value = moveDownElements[j];
                    if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0
              && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
              && this.Attribute.entity.model.IsUniverseGenerated())
                    {
                        string message = "You're not allowed to move this value." + "\n" + "You have to clear allocation before delete.";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Delete Value", message);
                        break;
                    }
                    MoveNode(value, false);
                    j++;
            }
            removeCTRLSelection();   
        }

        /// <summary>
        /// verifie l'ordre de selection utilisé.
        /// </summary>
        /// <returns>true if selection is in block UP to Down or Down to Up</returns>
        public bool isContiguousList()
        {
            int i = 0;
            bool result = true;
            if (selectedValues.Count <= 1) return true;

            List<int> listeIndex = selectedValues.Values.ToList();
            listeIndex.BubbleSort();
            foreach (KeyValuePair<AttributeValue,int> objet in selectedValues) 
            {
                if (i == listeIndex.Count - 1) break;
                KeyValuePair<AttributeValue, int> currentObjet = objet;
                KeyValuePair<AttributeValue, int> nextObjet = selectedValues.ElementAt(i+1);
                if (currentObjet.Key.GetParent() != nextObjet.Key.GetParent())
                {
                    result = false;
                    break;
                }

                int valCurrent = listeIndex[i];
                int valNext = listeIndex[i+1];
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
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Move Up".
        /// Elle permet de déplacer un noeud vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveUpNode(object sender, RoutedEventArgs e)
        {
            List<AttributeValue> moveUpElements = selectedValues.Keys.ToList();
            int j = 0;
            for (int i = moveUpElements.Count - 1; i >= 0; i--)
            {
               if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0
               && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
               && this.Attribute.entity.model.IsUniverseGenerated())
                    {
                        string message = "You're not allowed to move this value." + "\n" + "You have to clear allocation before delete.";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Delete Value", message);
                        break;
                    }
               Kernel.Domain.AttributeValue value = moveUpElements[j];
               MoveNode(value, true);
               j++;
           }
           removeCTRLSelection();
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Outdent".
        /// Elle permet de transformer un sous-noeud en noeud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutdentNode(object sender, RoutedEventArgs e)
        {
            List<AttributeValue> outDentElements = selectedValues.Keys.ToList();
            int j = 0;
            for(int i = outDentElements.Count -1; i>= 0;i--)
            {
                Kernel.Domain.AttributeValue value = outDentElements[j];
                Domain.AttributeValue parent = value.parent;
                if (parent != null && !parent.IsDefault && parent.usedToGenerateUniverse && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0
               && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
               && this.Attribute.entity.model.IsUniverseGenerated())
                {
                    string message = "You're not allowed to outdent this value." + "\n" + "You have to clear allocation before perform the outdent.";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Outdent value", message);
                    break;
                }
                OutdentNode(value);
                j++;
            }
            removeCTRLSelection();
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Indent".
        /// Elle permet de transformer un noeud en sous-noeud .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIndentNode(object sender, RoutedEventArgs e)
        {
            List<AttributeValue> inDentElements = selectedValues.Keys.ToList();
            int j = 0;
            for (int i = inDentElements.Count - 1; i >= 0; i--)
            {
                Kernel.Domain.AttributeValue value = inDentElements[j];
               if (value != null && !value.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0
               && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
               && this.Attribute.entity.model.IsUniverseGenerated() && value.usedToGenerateUniverse)
                {
                    string message = "You're not allowed to indent selected value." + "\n" + "You have to clear allocation before perform the indent.";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Indent value", message);
                    return;
                }
                IndentNode(value);
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
            Kernel.Domain.AttributeValue selectedItem = GetSelectedValue();
            if (selectedValues.Count > 0 && selectedItem == null)
            {
                selectedItem = selectedValues.Keys.Last();
            }

            if (Root != null)
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;
                
                this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectedValues.Count <= 1;
                this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyAttributeValue() && selectedItem != defaultValue && selectedValues.Count <= 1;
                this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.moveUpMenuItem.IsEnabled = this.Root != null && CanMoveUp && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue ;
                this.moveDownMenuItem.IsEnabled = this.Root != null && CanMoveDown && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.indentMenuItem.IsEnabled = this.Root != null && CanIndent && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.outdentMenuItem.IsEnabled = this.Root != null && CanOutdent && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
            }
            else
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.GetSelectedValue() != null)
                SetSelectedValue(this.GetSelectedValue());
        }
        #endregion
       
        #region Node Model Actions

        /// <summary>
        /// Ajoute la valeur par défaut après avoir créer une mesure
        /// </summary>
        /// <param name="value">le parent de l'attribut à créer</param>
        /// <returns>l'attribut créé</returns>
        public Domain.AttributeValue addDefaultNode(Kernel.Domain.AttributeValue value = null)
        {
            Domain.AttributeValue measure = AddNode(value);
           
            return measure;
        }

        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="parent">Le noeud auquel il fau ajouter un fils</param>
        /// <param name="name">le nom du noeud</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual AttributeValue AddNode(AttributeValue parent,string name="")
        {
            Kernel.Domain.AttributeValue child = GetNewTreeViewModel();
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
                else
                    return null;
            }

            SetSelectedValue(defaultValue);
            if (Changed != null) Changed();
            return child;
        }

        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(AttributeValue item)
        {
            if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
                {
                    string message = "You're not allowed to delete value " + item.name + "\n" + "You have to clear allocation before delete this value";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Delete Value", message);
                }
                else if (item != null && item.parent != null)
                {
                    int index = item.GetPosition();
                    selectedValues.Remove(item);
                    item.GetParent().RemoveChild(item);
                    if (Changed != null) Changed();
                }
            }
        
        /// <summary>
        /// Déplace un noeud vers le haut ou vers le bas
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sens">Le sens du déplacement. 0 => UP, 1=> DOWN</param>
        public void MoveNode(AttributeValue item, bool up)
        {
            if (item.parent != null)
            {
                selectedValues.Remove(item);
                int position = item.position + (up ? -1 : 1);
                IHierarchyObject child = item.parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(item.position); 
                    item.parent.UpdateChild(child);
                    item.SetPosition(position);
                    item.parent.UpdateChild(item);
                    selectedValues.Add(item, item.GetPosition());
                    if (Changed != null) Changed();
                    SetSelectedValue(item);
                }
            }
        }

        public void IndentNode(Domain.AttributeValue item) 
        {
            List<int> listeIndex = selectedValues.Values.ToList();
            listeIndex.BubbleSort();
            Kernel.Domain.AttributeValue parent = item.GetParent().GetChildByPosition(listeIndex[0]-1) as Kernel.Domain.AttributeValue;
            if (parent != null) 
            {
                item.parent.ForgetChild(item);
                parent.AddChild(item);
                item.UpdateParents();
                if (Changed != null) Changed();
                SetSelectedValue(item);
            }
        }

        /// <summary>
        /// Transforme un noeud en sous-noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void IndentNode(Dictionary<Domain.AttributeValue, int> items)
        {
            if (items.Count() > 0)
            {                
                Domain.AttributeValue item1 = items.Keys.ElementAt(0);
                if (item1 != null)
                {
                    int position = item1.GetPosition();
                    IHierarchyObject child = item1.parent.GetChildByPosition(position - 1);
                    Domain.AttributeValue value = (Domain.AttributeValue)child;
                    if (value != null && !value.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0
                        && this.Attribute != null && this.Attribute.entity != null && this.Attribute.entity.usedToGenerateUniverse
                        && this.Attribute.entity.model.IsUniverseGenerated() && value.usedToGenerateUniverse)
                    {
                        string message = "You're not allowed to indent selected value." + "\n" + "You have to clear allocation before perform the indent.";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Indent value", message);
                        return;
                    }
                    if(child != null)
                    {
                        foreach (Kernel.Domain.AttributeValue item in items.Keys)
                        {
                            item.parent.ForgetChild(item);
                            child.AddChild(item);
                            item.UpdateParents();
                            if (Changed != null) Changed();
                            SetSelectedValue(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transforme un sous-noeud en noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void OutdentNode(AttributeValue item)
        {
           if (item.parent != null)
            {
                IHierarchyObject parent = item.parent.GetParent();
                if (parent != null)
                {
                    selectedValues.Remove(item);
                    item.parent.ForgetChild(item);
                    parent.AddChild(item);
                    this.Root.SwichtPosition(defaultValue, item);
                    item.UpdateParents();
                    selectedValues.Add(item,item.GetPosition());
                    if (Changed != null) Changed();
                    SetSelectedValue(item);
                }
            }
        }

        protected AttributeValue GetNewTreeViewModel()
        {
            AttributeValue value = new AttributeValue();
            value.name = "Value";
            if (Root != null)
            {
                AttributeValue m = null;
                int i = 1;
                do
                {
                    value.name = "Value" + i++;
                    m = (AttributeValue)Root.GetChildByName(value.name);
                }
                while (m != null);
            }
            return value;
        }

        #endregion;

        #region Copy/Cut/Paste Methods

        /// <summary>
        /// Mets les éléments selectionnés dans le presse-papiers.
        /// </summary>
        /// <param name="copieListAttribute"></param>
        /// <param name="isCut">true=>cut/false=>copy</param>
        private void SetInClipboard(List<Kernel.Domain.AttributeValue> listeSelectedAttributeValue, bool isCut,Kernel.Domain.AttributeValue parent=null)
        {
            if (listeSelectedAttributeValue.Count > 0)
            {
                Kernel.Util.ClipbordUtil.ClearClipboard();
                List<IHierarchyObject> listeCopy = new List<IHierarchyObject>(0);
                if (!isCut)
                {
                    foreach (Kernel.Domain.AttributeValue AttributeValueToCopy in listeSelectedAttributeValue)
                    {
                        listeCopy.Add(AttributeValueToCopy.GetCopy());
                    }
                }
                else
                {
                    listeCopy.AddRange(listeSelectedAttributeValue);
                    if (parent == null) parent = this.Root;
                    int nbreCutElement = listeCopy.Count;
                    if (nbreCutElement > 0)
                    {
                        for (int i = 0; i < nbreCutElement; i++)
                        {
                            parent.ForgetChild(listeCopy[i]);
                        }
                    }
                }
                Kernel.Util.ClipbordUtil.SetHierarchyObject(listeCopy);
            }
        }

        /// <summary>
        /// Colle les éléments dans treeview après un copy/cut.
        /// </summary>
        /// <param name="attributesInClipboard">les éléments présents dans le presse-papier</param>
        /// <param name="isCutOperation">Le mode false=>copy/true=>cut</param>
        /// <param name="parent">Le parent sur lequel on effectue l'opération.</param>
        private void getValuesFromClipboard(List<Kernel.Domain.AttributeValue> valuesInClipboard, Kernel.Domain.AttributeValue parent = null)
        {
            if (valuesInClipboard != null && valuesInClipboard.Count > 0)
            {
                if (isTreeInCutMode)
                {
                    int nbCutElement = valuesInClipboard.Count;
                    for (int i = 0; i < nbCutElement; i++)
                    {
                        AddToTreeCopiedElements(restoreValueForeground(valuesInClipboard[i]), parent);
                    }
                    isTreeInCutMode = false;
                    SetInClipboard(valuesInClipboard, isTreeInCutMode);
                    if (Changed != null) Changed();
                }
                else
                {
                    int nbreCopies = valuesInClipboard.Count;
                    if (valuesInClipboard != null && nbreCopies > 0)
                    {
                        for (int j = 0; j <= nbreCopies - 1; j++)
                        {
                            Kernel.Domain.AttributeValue value = valuesInClipboard[j];
                            if (parent == null) value.SetPosition(this.Root.GetItems().Count);
                            else value.SetPosition(parent.GetItems().Count);
                            restoreValueForeground(value);
                            pasteValue(value, value.name, parent);
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
        private Kernel.Domain.AttributeValue restoreValueForeground(Kernel.Domain.AttributeValue value)
        {
            value.IsDefault = false;
            int j=0;
            for(int i = value.GetItems().Count-1;i>=0;i--)
            {
                Kernel.Domain.AttributeValue subvalue = value.childrenListChangeHandler.Items[j];
                subvalue = ValidateName(subvalue, subvalue.name);
                if(subvalue.name != value.childrenListChangeHandler.Items[j].name)
                value.childrenListChangeHandler.Items[j].name = subvalue.name;
                restoreValueForeground(subvalue);
                j++;
            }
            return value;
        }

        /// <summary>
        /// Cette fonction permet d'ajouter à l'arbre les éléments (Target) venus du presse-papier.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void pasteValue(Kernel.Domain.AttributeValue value, string name, Kernel.Domain.AttributeValue parent = null)
        {
            Kernel.Domain.AttributeValue clipboardAddedValue = ValidateName(value, name);
            if (clipboardAddedValue != null)
            {
                AddToTreeCopiedElements(clipboardAddedValue, parent);
            }
        }

        /// <summary>
        /// Cette méthod ajoute les éléments du presse-papier à l'arbre.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="parent"></param>
        private void AddToTreeCopiedElements(Kernel.Domain.AttributeValue measure, Kernel.Domain.AttributeValue parent = null)
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
       
        #endregion
     
       


    }
}
