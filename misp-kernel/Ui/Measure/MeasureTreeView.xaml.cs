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


namespace Misp.Kernel.Ui.Measure
{
    /// <summary>
    /// Interaction logic for MeasureTreeView.xaml
    /// 
    /// Cette classe implémente l'arbre d'édition des mesure.
    /// </summary>
    public partial class MeasureTreeView : ScrollViewer
    {

        public ChangeEventHandler Changed;

        static string  Label_DEFAULT_MEASURE = "Add measure";
        public Kernel.Domain.Measure Root { get; set; }
        public Kernel.Domain.Measure CurrentCutObject { get; set; }
        public Kernel.Domain.Measure CurrentCopiedObject { get; set; }
        public Kernel.Domain.Measure defaultValue;
        private Service.MeasureService MeasureService;
        private bool isTreeInCutMode { get; set; }
        public List<int> indexSelecte { get; set; }

        private bool CanMoveUp
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedMeasures.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;

                if (result && selectedMeasures.Keys.ElementAt(0).GetParent() != this.Root)
                    result = selectedMeasures.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;

                if (result && listeIndex.Count > 1) result = isContiguousList();

                return result;
            }
        }

        private bool CanMoveDown
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedMeasures.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[lastIndex] != defaultValue.GetPosition() - 1;

                if (result && selectedMeasures.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedMeasures.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;
                    if (result) result = selectedMeasures.Keys.ElementAt(lastIndex).GetPosition() != selectedMeasures.Keys.ElementAt(lastIndex).GetParent().GetItems().Count - 1;

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
                List<int> listeIndex = selectedMeasures.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;
                if (result && selectedMeasures.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedMeasures.Keys.ElementAt(lastIndex).GetParent().GetItems().Count > listeIndex.Count;
                    if (result)
                    {
                        Kernel.Domain.Measure firstPosition = selectedMeasures.Keys.ElementAt(lastIndex).GetParent().GetChildByPosition(0) as Kernel.Domain.Measure;
                        result = selectedMeasures.Keys.ElementAt(lastIndex) != firstPosition;
                    }
                }
                if (result && listeIndex.Count > 1) result = isContiguousList();
                return result;
            }
        }

        private void updatePositionInSelection(Domain.Measure measure, int newPosition)
        {
            int oldPosition = measure.position;
            if (selectedMeasures.TryGetValue(measure, out oldPosition))
            {
                selectedMeasures[measure] = newPosition;
            }
        }

        private bool CanOutdent
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedMeasures.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;
                if (listeIndex.Count > 0 && selectedMeasures.Keys.ElementAt(0).GetParent() == this.Root)
                    result = false;

                if (listeIndex.Count > 0 && selectedMeasures.Keys.ElementAt(0).GetParent() != this.Root)
                    result = true;

                if (result && listeIndex.Count > 1) result = isContiguousList();
                return result;
            }
        }


        #region Constructor
        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public MeasureTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.Measure();
            defaultValue = new Domain.Measure();
            defaultValue.name = Label_DEFAULT_MEASURE;
            defaultValue.IsDefault = true;
            this.MeasureService = Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService();

        }
        /// <summary>
        /// Affiche le contenu de la mesure
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(Kernel.Domain.Measure root)
        {
            this.Root = root;
            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                this.Root.AddChild(defaultValue);
                
                this.tree.ItemsSource = this.Root.GetItems();
                SetSelectedMeasure(defaultValue);


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
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(Misp.Kernel.Domain.Measure));
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
        Dictionary<Kernel.Domain.Measure, int> selectedMeasures = new Dictionary<Domain.Measure, int>();

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
        private void UpdateSelectionList(Domain.Measure selectedMeasure)
        {
            if (!selectedMeasures.ContainsKey(selectedMeasure))
            { // add
                selectedMeasures.Add(selectedMeasure, selectedMeasure.GetPosition());
            }
            else
            { // remove
                selectedMeasures.Remove(selectedMeasure);
            }
        }

       /// <summary>
       ///
       /// </summary>
       /// <param name="treeViewItem"></param>
       private void DoSelectedItemChanged(EO.Wpf.TreeViewItem treeViewItem)
       {
           Domain.Measure selectedMeasure;
           if (treeViewItem == null)
           {
               removeCTRLSelection();
               return;
           }
           else
           {
               selectedMeasure = GetSelectedMeasure();
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
               selectedMeasures.Clear();
           }
       }

       private void SelectionManager(EO.Wpf.TreeViewItem treeViewItem, bool isRightClick = true)
       {
           Kernel.Domain.Measure measure;

           if (treeViewItem.Header.ToString() == defaultValue.name)
           {
               measure = null;
               treeViewItem.Focus();
           }
           else
           {
               measure = this.Root.GetChildByName(treeViewItem.Header.ToString()) as Domain.Measure;
               if (!CtrlPressed)
               {
                   if (!isRightClick || (isRightClick && !selectedItems.ContainsKey(treeViewItem) && !selectedMeasures.Keys.Contains(measure)))
                   {
                       removeCTRLSelection();
                       selectedItems.Add(treeViewItem, null);
                       SetSelectedItem(treeViewItem);
                       selectedMeasures.Add(measure, measure.GetPosition());
                   }
               }
               else if (CtrlPressed && !selectedItems.ContainsKey(treeViewItem) && !selectedMeasures.Keys.Contains(measure))
               {
                   selectedItems.Add(treeViewItem, null);
                   selectedMeasures.Add(measure, measure.GetPosition());
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
            Kernel.Domain.Measure measure;
            EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                SelectionManager(treeViewItem, false);
            }
            else
            {
                measure = GetSelectedMeasure();
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
                SelectionManager(treeViewItem);
                e.Handled = true;
            }
            else
            {
                Kernel.Domain.Measure measure = GetSelectedMeasure();
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
        /// <returns>La mesure sélectionnée dans le cas de multiselection mode</returns>
        public Kernel.Domain.Measure GetSelectedMultiMeasure()
        {
            Kernel.Domain.Measure measure = null;
            measure = this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.Measure :
                     (this.tree.SelectedItem == null && this.tree.SelectedValue != null && this.tree.SelectedValue != defaultValue ?
                     (this.tree.SelectedValue as Kernel.Domain.Measure) : null);
            return measure;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée</returns>
        public Kernel.Domain.Measure GetSelectedMeasure()
        {
            return this.tree.SelectedItem != null ? this.tree.SelectedItem as Kernel.Domain.Measure :null ;
        }
   
        /// <summary>
        /// Selectionne une mesure dans l'arbre
        /// </summary>
        /// <param name="measure">La mesure à sélectionner</param>
        public void SetSelectedMeasure(Kernel.Domain.Measure measure)
        {
            if (measure != null)
            {
                if (measure.parent != null) measure.parent.IsExpanded = true;
                measure.IsSelected = true;
             
            }
            else
            {
                Kernel.Domain.Measure selection = GetSelectedMultiMeasure();
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
     /// Ajoute la valeur par défaut après avoir créer une mesure
     /// </summary>
     /// <param name="value">le parent de la mesure à créer</param>
     /// <returns>la mesure créé</returns>
        public Domain.Measure addDefaultNode(Kernel.Domain.Measure value=null) 
        {
            Domain.Measure measure =  AddNode(value);
            return measure;
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "New".
        /// Elle permet de créer un nouveau noeud dans l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewNode(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.Measure measure = GetSelectedMeasure();
            if (measure != null && !measure.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0 && this.MeasureService.isMeasureUseAllocation(measure))
            {
                string message = "You're not allowed to add measure." + "\n" + "You have to clear allocation before add measure";
                Kernel.Util.MessageDisplayer.DisplayWarning("Add measure", message);
                 return;
            }
            AddNode(measure);
            if (measure != null)
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
           
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Measure", "Do you want to delete selected measure ?");
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.Measure measure in selectedMeasures.Keys)
                {
                    if (Kernel.Application.ApplicationManager.Instance.AllocationCount > 0 && this.MeasureService.isMeasureUseAllocation(measure))
                    {
                        string message = "You're not allowed to delete measure " + measure.name+ "\n" + "You have to clear allocation before delete this measure";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Delete measure", message);
                        break;
                    }
                    DeleteNode(measure);
                    
                }
            }
        }

        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            //if (e != null)
            //{
            //    if (e.Item is Kernel.Domain.Measure)
            //    {
            //        Kernel.Domain.Measure measure = (Kernel.Domain.Measure)e.Item;
            //        e.Text = measure.name;
            //        defaultValue.IsDefault = false;
            //    }
            //}
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is Kernel.Domain.Measure)
                {
                    Kernel.Domain.Measure editedMeasure = (Kernel.Domain.Measure)e.Item;
                    string name = e.Text.Trim();
                    
                    Kernel.Domain.Measure validName = ValidateName(editedMeasure, name);
                    if (validName == null)
                    {
                        e.Canceled = true;
                        return;
                    }
                    if (editedMeasure.IsDefault)
                    {
                        if (name.ToUpper() != Label_DEFAULT_MEASURE.ToUpper())
                        {
                            Kernel.Domain.Measure addedNode = new Domain.Measure();
                            addedNode.name = name;

                            editedMeasure.name = Label_DEFAULT_MEASURE;
                            editedMeasure.parent.UpdateChild(editedMeasure);

                            addedNode = ValidateName(addedNode, name);
                            AddNode(null, addedNode.name);
                            e.Canceled = true;
                        }
                    }
                    else
                    {
                        Kernel.Domain.Measure valid = ValidateName(editedMeasure, name);
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

            //Only allow items to be dragged onto the Celebrity items,
            //not CelebrityCategory items
            //if (item.Level != 1)
            //    e.Canceled = true;
        }

        //Handles the TreeView's ItemDrop event
        private void OnItemDrop(object sender, EO.Wpf.ItemDropEventArgs e)
        {
            Kernel.Domain.Measure source = (Kernel.Domain.Measure) e.SourceItem;
            Kernel.Domain.Measure target1 = (Kernel.Domain.Measure) e.TargetItem1;
            Kernel.Domain.Measure target2 = (Kernel.Domain.Measure)e.TargetItem2;
            Kernel.Domain.Measure parent = target2 != null ? target2 : target1;

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
            Kernel.Domain.Measure measure = GetSelectedMultiMeasure();
            List<Kernel.Domain.Measure> listeSelectedMeasure = selectedMeasures.Keys.ToList<Kernel.Domain.Measure>();
            if ((listeSelectedMeasure.Count == 0) && measure != null) listeSelectedMeasure.Add(measure);
            isTreeInCutMode = false;
            SetInClipboard(listeSelectedMeasure);
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
            Kernel.Domain.Measure measure = GetSelectedMeasure();
            List<Kernel.Domain.Measure> listeSelectedMeasure = selectedMeasures.Keys.ToList<Kernel.Domain.Measure>();
            if ((listeSelectedMeasure.Count == 0) && measure != null) listeSelectedMeasure.Add(measure);
            Kernel.Domain.Measure parent = measure.parent;
            SetInClipboard(listeSelectedMeasure,parent);
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
            Kernel.Domain.Measure parent = GetSelectedMeasure();

            if (parent != null && !parent.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to add measure." + "\n" + "You have to clear allocation before add measure";
                Kernel.Util.MessageDisplayer.DisplayWarning("Paste", message);
                return;
            }
            
            if(selectedMeasures.Count > 0)
            if (selectedMeasures.ContainsKey(GetSelectedMultiMeasure())) parent = GetSelectedMultiMeasure();
            getMeasuresFromClipboard(Kernel.Util.ClipbordUtil.GetMeasure(), parent);

       }
        

   
        /// <summary>
        /// Cette méthode permet de vérifier si une measure de l'arbre possède un nom identique à celui d'une measure qui
        /// vient du presse-papier. S'il y a identité de nom, un nouveau nom est donné à la measure venant du presse-papier. 
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <returns>La measure à copier</returns>
     private Kernel.Domain.Measure ValidateName(Kernel.Domain.Measure measure,string name) 
     {
         bool result = true;
         measure.name = name;
         Kernel.Domain.Measure currentMeasure = measure.CloneObject() as Kernel.Domain.Measure;
         if(string.IsNullOrEmpty(name))
         {
             Kernel.Util.MessageDisplayer.DisplayError("Empty Measure", "Measure can't be empty! ");
             result = false;
         }
         Kernel.Domain.Measure m = (Kernel.Domain.Measure)Root.GetNotEditedChildByName(measure , name);
         
         if (m == null) return measure;
         
         if(m != null && m.Equals(measure))
         {
             if (measure.IsDefault) result = false;
             result = true;
         }
         if (m != null && !m.Equals(measure))
         {
             currentMeasure = currentMeasure.GetCopy() as Kernel.Domain.Measure;
             currentMeasure = ValidateName(currentMeasure, currentMeasure.name);
         }
         if(result)
             return currentMeasure;
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
            if (this.isSelectionDownToUp) permuteSelectioniList();
            int nbre = selectedMeasures.Keys.Count();

            if (SelectionOrder() == true)
            {
                while (nbre > 0)
                {
                    Domain.Measure measure = selectedMeasures.Keys.ElementAt(nbre - 1);
                    MoveNode(measure, false);
                    nbre--;
                }
            }
               
            
        }
        /// <summary>
        /// verifie l'ordre de selection utilisé.
        /// </summary>
        /// <returns>true if selection is in block UP to Down or Down to Up</returns>
        public bool SelectionOrder()
        {
            int i = 0;
            bool IsSelectionBlock = true;
            indexSelecte = new List<int>(0);
            if(selectedMeasures.Keys.Count>0)
            {
                indexSelecte.Add(selectedMeasures.Keys.ElementAt(i).GetPosition());
                while (i < (selectedMeasures.Keys.Count() - 1) && IsSelectionBlock)
                {
                    indexSelecte.Add(selectedMeasures.Keys.ElementAt(i + 1).GetPosition());
                    IsSelectionBlock = ((selectedMeasures.Keys.ElementAt(i).GetPosition() < selectedMeasures.Keys.ElementAt(i + 1).GetPosition()) && (selectedMeasures.Keys.ElementAt(i).GetPosition() + 1 == selectedMeasures.Keys.ElementAt(i + 1).GetPosition())) || ((selectedMeasures.Keys.ElementAt(i).GetPosition() > selectedMeasures.Keys.ElementAt(i + 1).GetPosition()) && (selectedMeasures.Keys.ElementAt(i).GetPosition() - 1 == selectedMeasures.Keys.ElementAt(i + 1).GetPosition())) ? true : false;
                    if(IsSelectionBlock)
                    if (selectedMeasures.Keys.ElementAt(i).GetParent() != selectedMeasures.Keys.ElementAt(i + 1).GetParent())
                        return false;
                    //verify if selection order is up to down, return false otherwise
                    this.isSelectionDownToUp = (selectedMeasures.Keys.ElementAt(i).GetPosition() > selectedMeasures.Keys.ElementAt(i + 1).GetPosition()) && (selectedMeasures.Keys.ElementAt(i).GetPosition() - 1 == selectedMeasures.Keys.ElementAt(i + 1).GetPosition());
               
                    i++;
                }
                indexSelecte.BubbleSort();
                return IsSelectionBlock;
            }
            return false;
        }
        /// <summary>
        /// permute la liste de selection si down to UP
        /// </summary>
        public void permuteSelectioniList()
        {
            int nbre = selectedMeasures.Keys.Count();
            Domain.Measure[] copyOfSelectectedMeasures = new Domain.Measure[nbre]  ;

            if (nbre > 1 && isSelectionDownToUp)
            {
                selectedMeasures.Keys.CopyTo(copyOfSelectectedMeasures, 0);
                selectedMeasures.Clear();
                int i;
                for ( i=nbre-1 ; i>=0 ; i--)
                {
                    selectedMeasures.Add(copyOfSelectectedMeasures[i], copyOfSelectectedMeasures[i].GetPosition());
                }
                

            }
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Move Up".
        /// Elle permet de déplacer un noeud vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveUpNode(object sender, RoutedEventArgs e)
        {
            if (this.isSelectionDownToUp) permuteSelectioniList();
            int nbre = selectedMeasures.Keys.Count();

            if (SelectionOrder() == true)
            {
                int index = 0;
                while (index < nbre)
                {
                    Domain.Measure measure = selectedMeasures.Keys.ElementAt(index);
                    MoveNode(measure, true);
                    index++;

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
            if (this.isSelectionDownToUp) permuteSelectioniList();
            foreach (Kernel.Domain.Measure measure in selectedMeasures.Keys)
            {
                Domain.Measure parent = measure.parent;
                if (parent != null && !parent.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0 && this.MeasureService.isMeasureUseAllocation(parent))
                {
                    string message = "You're not allowed to outdent measure." + "\n" + "You have to clear allocation before outdent measure";
                    Kernel.Util.MessageDisplayer.DisplayWarning("Outdent", message);
                    break;
                }
                OutdentNode(measure);
            }
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Indent".
        /// Elle permet de transformer un noeud en sous-noeud .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIndentNode(object sender, RoutedEventArgs e)
        {
            if (this.isSelectionDownToUp) permuteSelectioniList();
            IndentNode(selectedMeasures);
        }

        /// <summary>
        /// Cette méthode permet de désactiver un menuItem dans le cas
        /// où l'opération associée à ce menuItem n'est pas possible pour
        /// le noeud courant.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {

            Kernel.Domain.Measure selectedItem = GetSelectedMeasure();
            if (selectedMeasures.Count > 0 && selectedItem == null)
            {
                selectedItem = selectedMeasures.Keys.Last();
            }

            if (Root != null)
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;

                this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectedMeasures.Count <= 1;
                this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyMeasure() && selectedItem != defaultValue && selectedMeasures.Count <= 1;
                this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.moveUpMenuItem.IsEnabled = this.Root != null && CanMoveUp && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.moveDownMenuItem.IsEnabled = this.Root != null && CanMoveDown && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.indentMenuItem.IsEnabled = this.Root != null && CanIndent && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
                this.outdentMenuItem.IsEnabled = this.Root != null && CanOutdent && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue;
            }
            else
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
            }





            //Kernel.Domain.Measure selectedItem = GetSelectedMeasure();
            //if (selectedMeasures.Count > 0 && selectedItem == null)
            //{
            //    selectedItem = selectedMeasures.Keys.Last();
            //}

            //if (Root != null)
            //{
            //    this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;

            //    int lastPosition = -1;
            //    int firstPosition = -1;
            //    if (selectedItem != null)
            //    {
            //        if (selectedItem.GetParent() != this.Root)
            //        {
            //            lastPosition = selectedItem.parent.GetItems().Count - 1;
            //        }
            //        else
            //        {
            //            lastPosition = defaultValue.GetPosition();
            //        }
            //        firstPosition = 0;
            //    }

            //    Kernel.Domain.Measure firstSelectedValueItem = null;
            //    Kernel.Domain.Measure lastSelectedValueItem = null;

            //    SelectionOrder();
            //    int selectionCount = selectedItems.Count;
            //    if (selectionCount == 1)
            //    {
            //        selectedItem = this.Root.GetChildByName(selectedItem.name) as Domain.Measure;
            //    }

            //    if (selectionCount > 1)
            //    {
            //        if (this.isSelectionDownToUp)
            //        {
            //            firstSelectedValueItem = selectedMeasures.Keys.Last();
            //            lastSelectedValueItem = selectedMeasures.Keys.First();
            //        }
            //        else
            //        {
            //            firstSelectedValueItem = selectedMeasures.Keys.First();
            //            lastSelectedValueItem = selectedMeasures.Keys.Last();
            //        }
            //        if (indexSelecte.Count > 0)
            //        {
            //            firstPosition = indexSelecte[0];
            //            int itemFirstPost = (firstSelectedValueItem.GetParent() as Kernel.Domain.Measure).childrenListChangeHandler.Items[0].GetPosition();
            //            if (firstPosition != itemFirstPost)
            //                firstPosition = itemFirstPost;
            //        }
            //    }
            //    else
            //    {
            //        firstSelectedValueItem = selectedItem;
            //        lastSelectedValueItem = selectedItem;
            //    }

            //    this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectionCount <= 1;
            //    this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && SelectionOrder();
            //    this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && SelectionOrder();
            //    this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyMeasure()  && selectedItem != defaultValue && (selectionCount == 1 || selectionCount == 0);
            //    this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                
            //    this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && SelectionOrder();
          
            //    this.moveUpMenuItem.IsEnabled = this.Root != null && selectedItem != null && SelectionOrder()
            //    && lastSelectedValueItem.parent != null && firstSelectedValueItem.parent != null && selectedItem.parent != null
            //    && selectedItem != defaultValue
            //    && selectedItem.parent != this.Root ? selectionCount > 1 ? firstSelectedValueItem != null && firstSelectedValueItem.position > firstPosition
            //    && lastSelectedValueItem.position <= lastPosition : selectedItem.position > firstPosition : selectionCount > 1 ?
            //    firstSelectedValueItem.position > firstPosition && lastSelectedValueItem != null && lastSelectedValueItem.position <= lastPosition :
            //     selectedItem != null && selectedItem.position > firstPosition;


            //    this.moveDownMenuItem.IsEnabled = this.Root != null && SelectionOrder() && selectedItem != null && selectedItem != defaultValue  && lastSelectedValueItem != null
            //    && selectedItem.parent != null && selectedItem.GetParent() != this.Root ? selectionCount > 1 ?
            //    lastSelectedValueItem.GetPosition() < lastPosition : selectedItem != null && selectedItem.GetPosition() < lastPosition : selectionCount > 1 ?
            //    lastSelectedValueItem.GetPosition() < lastPosition - 1 : selectedItem != null && selectedItem.GetPosition() < lastPosition - 1;

            //    this.indentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue  && firstSelectedValueItem != null
            //    && selectionCount > 1 ? firstSelectedValueItem.GetPosition() > firstPosition : selectedItem != null && selectedItem.GetPosition() > firstPosition && SelectionOrder();

            //    this.outdentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue && SelectionOrder()
            //     && selectedItem.parent != null && selectedItem.GetParent() != this.Root ? true : false;

            //}
            //else
            //{
            //    this.tree.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        #endregion
        


        #region Node Model Actions

        /// <summary>
        /// verifie l'ordre de selection utilisé.
        /// </summary>
        /// <returns>true if selection is in block UP to Down or Down to Up</returns>
        public bool isContiguousList()
        {
            int i = 0;
            bool result = true;
            if (selectedMeasures.Count <= 1) return true;

            List<int> listeIndex = selectedMeasures.Values.ToList();
            listeIndex.BubbleSort();
            foreach (KeyValuePair<Kernel.Domain.Measure, int> objet in selectedMeasures)
            {
                if (i == listeIndex.Count - 1) break;
                KeyValuePair<Kernel.Domain.Measure, int> currentObjet = objet;
                KeyValuePair<Kernel.Domain.Measure, int> nextObjet = selectedMeasures.ElementAt(i + 1);
                if (currentObjet.Key.GetParent() != nextObjet.Key.GetParent())
                {
                    result = false;
                    break;
                }

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
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="parent">Le noeud auquel il fau ajouter un fils</param>
        /// <param name="name">le nom du noeud</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual Kernel.Domain.Measure AddNode(Kernel.Domain.Measure parent,string name="")
        {
            Kernel.Domain.Measure child = GetNewTreeViewModel();
            if (name != "") child.name = name;
            if (parent != null)
            {
                parent.AddChild(child);
                parent.UpdateParents();
            }
            else
            {
                this.Root.AddChild(child);
                this.Root.SwichtPosition(defaultValue, child);            
            }

            if (Changed != null) Changed();
            SetSelectedMeasure(defaultValue);
            return child;
        }
        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(Kernel.Domain.Measure item)
        {
            if(item != null && item.parent != null)
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
        public void MoveNode(Kernel.Domain.Measure item, bool up)
        {
            if (item.parent != null)
            {
               int position = item.position + (up ? -1 : 1);
                IHierarchyObject child = item.parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(item.position); 
                    item.parent.UpdateChild(child);
                    updatePositionInSelection(item, position);
                    item.SetPosition(position);
                    item.parent.UpdateChild(item);
                    if (Changed != null) Changed();
                    SetSelectedMeasure(item);
                }
            }
        }

        /// <summary>
        /// Transforme un noeud en sous-noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void IndentNode(Dictionary<Domain.Measure, int> items)
        {
            if (items.Count() > 0)
            {
				List<Domain.Measure> measures = items.Keys.ToList();
				measures.BubbleSort();
                Domain.Measure item1 = measures[0];
                if (item1 != null)
                {
                    int position = item1.GetPosition();
                    IHierarchyObject child = item1.parent.GetChildByPosition(position - 1);
                    Domain.Measure measure = (Domain.Measure)child;
                    if (measure != null && !measure.IsDefault && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0 && this.MeasureService.isMeasureUseAllocation(measure))
                    {
                        string message = "You're not allowed to indent measure." + "\n" + "You have to clear allocation before indent measure";
                        Kernel.Util.MessageDisplayer.DisplayWarning("Indent", message);
                        return;
                    }
                    if (child != null)
                    {
                        foreach (Kernel.Domain.Measure item in measures)
                        {
                            item.parent.ForgetChild(item);
                            child.AddChild(item);
                            if (Changed != null) Changed();
                            SetSelectedMeasure(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transforme un sous-noeud en noeud
        /// </summary>
        /// <param name="model">le noeud à édenté</param>
        public void OutdentNode(Kernel.Domain.Measure item)
        {
            if (item.parent != null)
            {
                IHierarchyObject parent = item.parent.GetParent();
                if (parent != null)
                {
                    item.parent.ForgetChild(item);
                    parent.AddChild(item);
                    this.Root.SwichtPosition(defaultValue, item);            
                    if (Changed != null) Changed();
                    SetSelectedMeasure(item);
                }
            }
        }
        #endregion;

        #region Copy/Cut/Paste Methods

        /// <summary>
        /// Mets les éléments selectionnés dans le presse-papiers.
        /// </summary>
        /// <param name="copieListMeasure"></param>
        /// <param name="isCut">true=>cut/false=>copy</param>
        private void SetInClipboard(List<Kernel.Domain.Measure> copieListMeasure,Kernel.Domain.Measure parent = null)
        {
            if (copieListMeasure.Count > 0)
            {
                Kernel.Util.ClipbordUtil.ClearClipboard();
                List<IHierarchyObject> listeCopy = new List<IHierarchyObject>(0);
                if (!isTreeInCutMode)
                {
                    foreach (Kernel.Domain.Measure measureToCopy in copieListMeasure)
                    {
                        listeCopy.Add(measureToCopy.GetCopy());
                    }
                }
                else
                {
                    listeCopy.AddRange(copieListMeasure);
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
        /// <param name="measuresInClipboard">les éléments présents dans le presse-papier</param>
        /// <param name="isCutOperation">Le mode false=>copy/true=>cut</param>
        /// <param name="parent">Le parent sur lequel on effectue l'opération.</param>
        private void getMeasuresFromClipboard(List<Kernel.Domain.Measure> measuresInClipboard, Kernel.Domain.Measure parent = null)
        {
            if (measuresInClipboard != null && measuresInClipboard.Count > 0)
            {
                if (isTreeInCutMode)
                {
                    int nbCutElement = measuresInClipboard.Count;
                    for (int i = 0; i < nbCutElement; i++)
                    {
                        AddToTreeCopiedElements(RestoreMeasureForeground(measuresInClipboard[i]), parent);
                    }
                    isTreeInCutMode = false;
                    SetInClipboard(measuresInClipboard);
                    if (Changed != null) Changed();
                }
                else
                {
                    int nbreCopies = measuresInClipboard.Count;
                    if (measuresInClipboard != null && nbreCopies > 0)
                    {
                        for (int j = 0; j <= nbreCopies - 1; j++)
                        {
                            RestoreMeasureForeground(measuresInClipboard[j]);
                            pasteValue(measuresInClipboard[j], measuresInClipboard[j].name, parent);
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
        private Kernel.Domain.Measure RestoreMeasureForeground(Kernel.Domain.Measure measure)
        {
            measure.IsDefault = false;
            int j = 0;
            for(int i = measure.GetItems().Count-1;i>=0;i--)
            {
                Kernel.Domain.Measure subMeasure = measure.childrenListChangeHandler.Items[j];
                subMeasure = ValidateName(subMeasure, subMeasure.name);
                if (subMeasure.name != measure.childrenListChangeHandler.Items[j].name)
                    measure.childrenListChangeHandler.Items[j].name = subMeasure.name;
                RestoreMeasureForeground(subMeasure);
                j++;
            }
            return measure;
        }

        /// <summary>
        /// Cette méthod ajoute les éléments du presse-papier à l'arbre.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="parent"></param>
        private void AddToTreeCopiedElements(Kernel.Domain.Measure measure, Kernel.Domain.Measure parent = null)
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
        /// Cette fonction permet d'ajouter à l'arbre les éléments (Target) venus du presse-papier.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void pasteValue(Kernel.Domain.Measure measure, string name, Kernel.Domain.Measure parent = null)
        {
            Kernel.Domain.Measure clipboardAddedMeasure = ValidateName(measure, name);
            if (clipboardAddedMeasure != null)
            {
                AddToTreeCopiedElements(clipboardAddedMeasure, parent);
               // for (int i = measure.childrenListChangeHandler.Items.Count - 1; i >= 0; i--)
                //{
                 //   pasteValue(measure.childrenListChangeHandler.Items[i], measure.childrenListChangeHandler.Items[i].name, clipboardAddedMeasure);
               // }
            }
        }

      
        #endregion
       
        protected Kernel.Domain.Measure GetNewTreeViewModel(Domain.Measure value = null)
        {
                Kernel.Domain.Measure measure = new Kernel.Domain.Measure();
                measure.name = "Measure1";
                if (Root != null)
                {
                    Kernel.Domain.Measure m = null;
                    int i = 1;
                    do
                    {
                        measure.name = "Measure" + i++;
                        m = (Kernel.Domain.Measure)Root.GetChildByName(measure.name);
                    }
                    while (m != null);
                }
                return measure;
        }

        

        
    }
}
