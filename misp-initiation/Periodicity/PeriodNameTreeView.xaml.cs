using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
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

namespace Misp.Initiation.Periodicity
{
    /// <summary>
    /// Interaction logic for PeriodNameTreeView.xaml
    /// </summary>
    public partial class PeriodNameTreeView : ScrollViewer
    {        
        
        #region events
        
        public ChangeEventHandler Changed;
        
        #endregion
   
        #region properties

        public static string Label_DEFAULT_PERIOD = "Add Period";
        public PeriodName Root { get; set; }
        public PeriodName defaultValue;
        private bool isTreeInCutMode { get; set; }

        private bool CanMoveUp
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedPeriodNames.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[0] != 0;
                if (result && selectedPeriodNames.Keys.ElementAt(0).GetParent() != this.Root)
                    result = selectedPeriodNames.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;
                if (result && listeIndex.Count > 1) result = isContiguousList();
                return result;
            }
        }

        private bool CanMoveDown
        {
            get
            {
                bool result = false;
                List<int> listeIndex = selectedPeriodNames.Values.ToList();
                listeIndex.BubbleSort();
                int lastIndex = listeIndex.Count - 1;
                result = listeIndex.Count > 0 && listeIndex[lastIndex] != defaultValue.GetPosition() - 1;
                if (result && selectedPeriodNames.Keys.ElementAt(0).GetParent() != this.Root)
                {
                    result = selectedPeriodNames.Keys.ElementAt(lastIndex).GetParent().GetItems().Count != listeIndex.Count;
                    if (result) 
                        result = selectedPeriodNames.Keys.ElementAt(lastIndex).GetPosition() != selectedPeriodNames.Keys.ElementAt(lastIndex).GetParent().GetItems().Count - 1;
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
        public PeriodNameTreeView()
        {
            InitializeComponent();
            this.Focusable = true;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new PeriodName();
            this.defaultValue = new PeriodName();
            this.defaultValue.IsDefault = true;
            defaultValue.name = Label_DEFAULT_PERIOD;
        }

        /// <summary>
        /// Affiche le contenu de la PeriodName
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(PeriodName root)
        {
            this.Root = root;
            if (this.Root == null) this.tree.ItemsSource = null;
            else
            {
                RefreshParent(this.Root);
                if(this.Root.GetChildByName(defaultValue.name) == null) this.Root.AddChild(defaultValue);
                this.tree.ItemsSource = this.Root.GetItems();
                if (this.Root.childrenListChangeHandler.Items.Count > 0) this.Root.childrenListChangeHandler.Items[0].IsSelected = true;
                else SetSelectedPeriod(defaultValue);
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
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate(typeof(PeriodName));
            dataTemplate.ItemsSource = new Binding("childrenListChangeHandler.Items");
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding("name"));
            dataTemplate.VisualTree = factory;
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

                if (treeViewItem != null) SelectionManager(treeViewItem, false); 
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
                PeriodName period;
                EO.Wpf.TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (treeViewItem != null)
                {
                    SelectionManager(treeViewItem, false);
                }
                else
                {
                    GetSelectedMultiPeriod();
                    period = GetSelectedPeriod();
                    tree.Focus();
                }
            }catch(Exception){}
        }

        private void SelectionManager(EO.Wpf.TreeViewItem treeViewItem, bool isRightClick=true) 
        {
            PeriodName period;

            if (treeViewItem.Header.ToString() == defaultValue.name)
            {
                period = null;
                treeViewItem.Focus();
            }
            else
            {
                period = this.Root.GetChildByName(treeViewItem.Header.ToString()) as PeriodName;
                if (!CtrlPressed)
                {
                    if (!isRightClick || (isRightClick && !selectedItems.ContainsKey(treeViewItem) && !selectedPeriodNames.Keys.Contains(period)))
                    {
                        removeCTRLSelection();
                        selectedItems.Add(treeViewItem, null);
                        SetSelectedItem(treeViewItem);
                        selectedPeriodNames.Add(period, period.GetPosition());
                    }
                }
                else if (CtrlPressed && !selectedItems.ContainsKey(treeViewItem) && !selectedPeriodNames.Keys.Contains(period))
                {
                    selectedItems.Add(treeViewItem, null);
                    selectedPeriodNames.Add(period, period.GetPosition());
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
        public Dictionary<PeriodName, int> selectedPeriodNames = new Dictionary<PeriodName, int>();

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
                selectedPeriodNames.Clear();
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
                PeriodName period = GetSelectedPeriod();
                if (period != null) period.IsSelected = false;
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
        public PeriodName GetSelectedPeriod()
        {
            return this.tree.SelectedItem != null ?
                this.tree.SelectedItem as PeriodName : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée dans le cas de multiselection mode</returns>
        public PeriodName GetSelectedMultiPeriod()
        {
            PeriodName period = null;
            period = this.tree.SelectedItem != null ? this.tree.SelectedItem as PeriodName :
                     (this.tree.SelectedItem == null && this.tree.SelectedValue != null && this.tree.SelectedValue != defaultValue ?
                     (this.tree.SelectedValue as PeriodName) : null);
            return period;
        }

        /// <summary>
        /// Selectionne une mesure dans l'arbre
        /// </summary>
        /// <param name="attribute">La mesure à sélectionner</param>
        public void SetSelectedPeriod(PeriodName period)
        {
            if (period != null)
            {
                if (period.parent != null) period.parent.IsExpanded = true;
                period.IsSelected = true;
                
            }
            else
            {
                PeriodName selection = GetSelectedPeriod() == null ? GetSelectedMultiPeriod() : GetSelectedPeriod();
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
            if (selectedPeriodNames.Count <= 1) return true;

            List<int> listeIndex = selectedPeriodNames.Values.ToList();
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
            PeriodName parent = null;
            PeriodName period = addDefaultNode(parent);
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
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Period", "Do you want to delete selected period ?");
            if (result == MessageBoxResult.Yes)
            {
                this.Root.ForgetChild(defaultValue);
                foreach (PeriodName period in selectedPeriodNames.Keys)
                {
                    if (period.iDateDefault)
                    {
                        string message = "You're not allowed to delete default or usable period :  " + period.name;
                        Kernel.Util.MessageDisplayer.DisplayWarning("Delete Period", message);
                        continue;
                    }
                    DeleteNode(period);
                }
                this.Root.AddChild(defaultValue);
                SetSelectedPeriod(defaultValue);
            }
        }
        
        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is PeriodName)
            {
                PeriodName period = (PeriodName)e.Item;
                e.Text = period.name;
            }
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            try
            {
                if (e.Item is PeriodName)
                {
                    PeriodName editedPeriod = (PeriodName)e.Item;
                    string name = e.Text.Trim();
                    string oldName = editedPeriod.name;

                    PeriodName ValidPeriod = ValidateName(editedPeriod, name);
                    if (ValidPeriod == null)
                    {
                        editedPeriod.name = oldName;
                        e.Canceled = true;
                        return;
                    }
                    bool isSameName = editedPeriod.name.ToUpper().Equals(name.ToUpper());
                    if (isSameName)
                    {
                        e.Canceled = true;
                        return;
                    }
                    if (editedPeriod.IsDefault)
                    {
                        if (name.ToUpper() != Label_DEFAULT_PERIOD.ToUpper())
                        {
                            PeriodName addedNode = new PeriodName();
                            addedNode.name = name;
                            editedPeriod.name = Label_DEFAULT_PERIOD;
                            editedPeriod.parent.UpdateChild(editedPeriod);
                            addedNode = ValidateName(addedNode, name);
                            AddNode(null, addedNode.name);
                            e.Canceled = true;
                        }
                     }
                    else
                    {
                        PeriodName valid = ValidateName(editedPeriod, name);
                        editedPeriod.name = name;
                        editedPeriod.parent.UpdateChild(editedPeriod);
                        if (!editedPeriod.name.Equals(Label_DEFAULT_PERIOD)) 
                        {
                            defaultValue.name = Label_DEFAULT_PERIOD;
                            AddNode(null, name);
                        }
                        //this.tree.Items.Refresh();
                    }
                }
                if (Changed != null) Changed();
                
                //The event must be canceled, otherwise the TreeView will
                //set the TreeViewItem's Header to the new text
                e.Canceled = true;
            }
            catch (Exception) { return; }
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
            SetInClipboard(selectedPeriodNames.Keys.ToList<PeriodName>(), isTreeInCutMode);
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
            var listCut = selectedPeriodNames.Keys.ToList<PeriodName>();
            SetInClipboard(listCut, isTreeInCutMode);
            foreach (PeriodName period in listCut)
            {
                DeleteNode(period);
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
            PeriodName period = Kernel.Util.ClipbordUtil.GetPeriodName()[0];
            getPeriodsFromClipboard(Kernel.Util.ClipbordUtil.GetPeriodName());
            System.Collections.ObjectModel.ObservableCollection<PeriodName> l = (System.Collections.ObjectModel.ObservableCollection<PeriodName>)this.tree.ItemsSource;
            //Domain.Attribute attributeFromSource = l[l.IndexOf(attribute)];
            PeriodName att = null;
            foreach (PeriodName att1 in l)
            {
                if (att1.name.Equals(period.name))
                    att = att1;

            }
            SetSelectedPeriod(att);
            this.tree.Items.Refresh();            
        }

        /// <summary>
        /// Cette méthode permet de vérifier si un AttributeValue de l'arbre possède un nom identique à celui d'une attribute qui
        /// vient du presse-papier. S'il y a identité de nom, un nouveau nom est donné à l' AttributeValue venant du presse-papier. 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="name"></param>
        /// <returns>La attribute à copier</returns>
        private PeriodName ValidateName(PeriodName period, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty value", "Value can't be empty! ");
                return null;
            }
            PeriodName otherPeriod = (PeriodName)Root.GetChildByName(period, name);
            if (otherPeriod == null) return period;
            if (period.name.ToUpper().Equals(name.ToUpper())) return period;

            Kernel.Util.MessageDisplayer.DisplayError("Duplicate Period", "There is another period named : " + name);
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
            List<PeriodName> moveDownElements = selectedPeriodNames.Keys.ToList();
            int j = 0;
            for (int i = moveDownElements.Count - 1; i >= 0; i--)
            {
                PeriodName period = moveDownElements[j];
                MoveNode(period, false);
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
            List<PeriodName> moveUpElements = selectedPeriodNames.Keys.ToList();
            int j = 0;
            for (int i = moveUpElements.Count - 1; i >= 0; i--)
            {
                PeriodName period = moveUpElements[j];
                MoveNode(period, true);
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
            GetSelectedPeriod();
            PeriodName selectedItem = GetSelectedPeriod();
            if (selectedPeriodNames.Count > 0 && selectedItem ==null) 
            {
                selectedItem = selectedPeriodNames.Keys.Last();
            }

            if (Root != null)
            {
                this.tree.ContextMenu.Visibility = System.Windows.Visibility.Visible;

                this.newMenuItem.IsEnabled = this.Root != null && selectedItem != defaultValue && selectedPeriodNames.Count <= 1;
                this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.pasteMenuItem.IsEnabled = this.Root != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmptyPeriodName() && selectedItem != defaultValue && isContiguousList();
                this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem != defaultValue && isContiguousList();
                this.moveUpMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue && CanMoveUp;
                this.moveDownMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem != defaultValue && CanMoveDown;
                this.propertiesMenuItem.IsEnabled = selectedItem != null && selectedItem != defaultValue;
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
        private void SetInClipboard(List<PeriodName> copieListPeriod, bool isCut)
        {
            if (copieListPeriod.Count > 0)
            {
                Kernel.Util.ClipbordUtil.ClearClipboard();
                List<IHierarchyObject> listeCopy = new List<IHierarchyObject>(0);
                List<IHierarchyObject> listeCut = new List<IHierarchyObject>(0);

                if (!isCut)
                {
                    foreach (PeriodName periodToCopy in copieListPeriod)
                    {
                        listeCopy.Add(periodToCopy.GetCopy());
                    }
                    Kernel.Util.ClipbordUtil.SetHierarchyObject(listeCopy);
                }
                else
                {
                    listeCopy.AddRange(copieListPeriod);
                    int nbreCutElement = listeCopy.Count;
                    if (nbreCutElement > 0)
                    {
                        for (int i = 0; i < nbreCutElement; i++)
                        {
                            this.Root.ForgetChild(listeCopy[i]);
                        }
                    }
                    Kernel.Util.ClipbordUtil.SetHierarchyObject(listeCut);
                }                
            }
        }

        /// <summary>
        /// Colle les éléments dans treeview après un copy/cut.
        /// </summary>
        /// <param name="periodsInClipboard">les éléments présents dans le presse-papier</param>
        /// <param name="isCutOperation">Le mode false=>copy/true=>cut</param>
        /// <param name="parent">Le parent sur lequel on effectue l'opération.</param>
        private void getPeriodsFromClipboard(List<PeriodName> periodsInClipboard, PeriodName parent = null)
        {
            if (periodsInClipboard != null && periodsInClipboard.Count > 0)
            {
                if (isTreeInCutMode)
                {
                    int nbCutElement = periodsInClipboard.Count;
                    
                    for (int i = 0; i < nbCutElement; i++)
                    {
                        periodsInClipboard[i] = ValidateName(periodsInClipboard[i], periodsInClipboard[i].name);
                        AddToTreeCopiedElements(periodsInClipboard[i], parent);
                    }
                    isTreeInCutMode = false;
                    SetInClipboard(periodsInClipboard, isTreeInCutMode);
                    if(nbCutElement > 0) SetSelectedPeriod(periodsInClipboard[0]);
                    if (Changed != null) Changed();
                }
                else
                {
                    int nbreCopies = periodsInClipboard.Count;
                    if (periodsInClipboard != null && nbreCopies > 0)
                    {
                        for (int j = 0; j <= nbreCopies - 1; j++)
                        {
                            pasteValue(periodsInClipboard[j], periodsInClipboard[j].name, parent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cette fonction permet d'ajouter à l'arbre les éléments (Target) venus du presse-papier.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        private void pasteValue(PeriodName period, string name, PeriodName parent = null)
        {
            PeriodName clipboardAddedPeriod = ValidateName(period, name);
            if (clipboardAddedPeriod != null)
            {
                AddToTreeCopiedElements(clipboardAddedPeriod, parent);
                int nbreAttributes = period.childrenListChangeHandler.Items.Count;
                for (int i = 0; i < nbreAttributes - 1; i++)
                {
                    pasteValue(period.childrenListChangeHandler.Items[i], period.childrenListChangeHandler.Items[i].name, clipboardAddedPeriod);
                }
            }
        }

        /// <summary>
        /// Cette méthod ajoute les éléments du presse-papier à l'arbre.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parent"></param>
        private void AddToTreeCopiedElements(PeriodName period, PeriodName parent = null)
        {
            this.Root.ForgetChild(defaultValue);
            if (parent != null)
            {
                parent.AddChild(period);
            }
            else
            {
                this.Root.AddChild(period);
            }
            if (Changed != null) Changed();

            this.Root.AddChild(defaultValue);
        }
        
        #endregion


        #region Node Model Actions

        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="parent">Le noeud auquel il fau ajouter un fils</param>
        /// <param name="name">le nom du noeud</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual PeriodName AddNode(PeriodName parent, string name = "")
        {
            PeriodName child = GetNewTreeViewModel();
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
            SetSelectedPeriod(defaultValue);
            if (Changed != null) Changed();
            return child;
        }

        /// <summary>
        /// Ajoute la valeur par défaut après avoir créer une mesure
        /// </summary>
        /// <param name="value">le parent de l'attribut à créer</param>
        /// <returns>l'attribut créé</returns>
        public PeriodName addDefaultNode(PeriodName value = null)
        {
            PeriodName period = AddNode(value);
            return period;
        }
        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(PeriodName item)
        {
            if (item != null && item.parent != null)
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
        public void MoveNode(PeriodName item, bool up)
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
                    SetSelectedPeriod(item);
                }
            }
        }
                  
        #endregion;

        protected PeriodName GetNewTreeViewModel()
        {
            PeriodName period = new PeriodName();
            period.name = "Period";
            if (Root != null)
            {
                PeriodName m = null;
                int i = 1;
                do
                {
                    period.name = "Period" + i++;
                    m = (PeriodName)Root.GetChildByName(period.name);
                }
                while (m != null);
            }
            return period;
        }
                

    }
}
