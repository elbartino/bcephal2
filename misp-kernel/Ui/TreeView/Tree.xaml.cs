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

namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Interaction logic for Tree.xaml
    /// </summary>
    public partial class Tree : ScrollViewer
    {
        public ChangeEventHandler Changed;

        static string Label_DEFAULT_MEASURE = "Add measure";
        public Kernel.Domain.Measure Root { get; set; }
        public Kernel.Domain.Measure CurrentCutObject { get; set; }
        public Kernel.Domain.Measure defaultValue;

        public Tree()
        {
            InitializeComponent();
            this.Focusable = true;
            this.BorderBrush = Brushes.White;
            InitializeDataTemplate();
            InitializeHandlers();
            InitializeContextMenu();
            this.Root = new Domain.Measure();
            defaultValue = new Domain.Measure()
            {
                name = Label_DEFAULT_MEASURE,
                IsDefault = true,
            };
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                SetCurrentItemInEditMode(true);
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetCurrentItemInEditMode(true);
            //e.Handled = true;
        }

        private void SetCurrentItemInEditMode(bool EditMode)
        {
            // Make sure that the SelectedItem is actually a TreeViewItem
            // and not null or something else
            if (this.tree.SelectedItem is TreeViewItem)
            {
                TreeViewItem tvi = this.tree.SelectedItem as TreeViewItem;

                // Also make sure that the TreeViewItem
                // uses an EditableTextBlock as its header
                if (tvi.Header is EditableTextBox)
                {
                    EditableTextBox etb = tvi.Header as EditableTextBox;

                    // Finally make sure that we are
                    // allowed to edit the TextBlock
                    if (etb.IsEditable)
                        etb.IsInEditMode = EditMode;
                }
            }
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
                SetSelectedMeasure(defaultValue);
                this.tree.ItemsSource = this.Root.GetItems();
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
                e.Handled = true;
            }
            else 
            {
                Kernel.Domain.Measure measure = GetSelectedMeasure();
                if (measure != null) measure.IsSelected = false;
                
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La mesure sélectionnée</returns>
        public Kernel.Domain.Measure GetSelectedMeasure()
        {
            return this.tree.SelectedItem != null ?
                this.tree.SelectedItem as Kernel.Domain.Measure : null;
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
                Kernel.Domain.Measure selection = GetSelectedMeasure();
                if (selection != null) selection.IsSelected = false;
            }
            tree.Focus();
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
            AddNode(GetSelectedMeasure());
        }
        
        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Delete".
        /// Elle permet de supprimer un noeud de l'abre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteNode(object sender, RoutedEventArgs e)
        {
            DeleteNode(GetSelectedMeasure());
        }

        private void OnEnterEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            
        }

        private void OnExitEditMode(object sender, EO.Wpf.EditItemEventArgs e)
        {
            if (e.Item is Kernel.Domain.Measure)
            {
                Kernel.Domain.Measure editedMeasure = (Kernel.Domain.Measure) e.Item;
                string name = e.Text.Trim();
                if (!validateName(editedMeasure, name))
                {
                    e.Canceled = true;
                    return;
                }
                if (editedMeasure.IsDefault)
                {
                    Kernel.Domain.Measure measure = new Domain.Measure();
                    measure.name = name;
                    this.Root.ForgetChild(editedMeasure);
                    this.Root.AddChild(measure);
                    this.Root.AddChild(editedMeasure);
                    e.Canceled = true;
                }
                else
                {
                    editedMeasure.name = name;
                    editedMeasure.parent.UpdateChild(editedMeasure);
                }
                if (Changed != null) Changed();
            }
            else e.Canceled = true;
        }

        protected bool validateName(Kernel.Domain.Measure measure, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Measure", "Measure can't be empty! ");
                return false;
            }

            Kernel.Domain.Measure m = (Kernel.Domain.Measure)Root.GetChildByName(name);
            if (m != null && m.Equals(measure))
            {
                return true;
            }

            if (m != null && !m.Equals(measure))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Duplicate Measure", name + " already exists!");
                return false;
            }
            return true;
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
            Kernel.Domain.Measure measure = GetSelectedMeasure();
            if (measure != null)
            {
                IHierarchyObject copy = measure.GetCopy();
                Kernel.Util.ClipbordUtil.SetHierarchyObject(copy);                
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
            Kernel.Domain.Measure measure = GetSelectedMeasure();
            CurrentCutObject = measure;
            if (measure != null)
            {
                Kernel.Util.ClipbordUtil.SetHierarchyObject(CurrentCutObject);
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
            Kernel.Domain.Measure parent = GetSelectedMeasure();
            Kernel.Domain.Measure newItem = null;
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
                    IHierarchyObject copy = CurrentCutObject.GetCopy();
                    Kernel.Util.ClipbordUtil.SetHierarchyObject(copy);
                    CurrentCutObject = null;
                    if (Changed != null) Changed();
                }                
            }
            else
            {
                newItem = Kernel.Util.ClipbordUtil.GetMeasure();
                if (newItem != null)
                {
                    parent.AddChild(newItem);
                    Kernel.Util.ClipbordUtil.SetHierarchyObject(newItem.GetCopy());
                    if (Changed != null) Changed();
                }                    
            }
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sure le menu "Move Down".
        /// Elle permet de déplacer un noeud vers le bas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveDownNode(object sender, RoutedEventArgs e)
        {
            MoveNode(GetSelectedMeasure(), false);
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Move Up".
        /// Elle permet de déplacer un noeud vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveUpNode(object sender, RoutedEventArgs e)
        {
            MoveNode(GetSelectedMeasure(), true);
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Outdent".
        /// Elle permet de transformer un sous-noeud en noeud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutdentNode(object sender, RoutedEventArgs e)
        {
            OutdentNode(GetSelectedMeasure());
        }

        /// <summary>
        /// Cette méthode est appélée lorsque l'utilisateur clique sur le menu "Indent".
        /// Elle permet de transformer un noeud en sous-noeud .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIndentNode(object sender, RoutedEventArgs e)
        {
            IndentNode(GetSelectedMeasure());
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
            if (selectedItem != null && selectedItem != defaultValue)
            {

                    int lastPosition = -1;
                    if (selectedItem.GetParent() != this.Root) lastPosition = selectedItem.parent.GetItems().Count;
                    else lastPosition = defaultValue.GetPosition();

                    this.newMenuItem.IsEnabled = this.Root != null;
                    this.cutMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null;
                    this.copyMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null;
                    this.pasteMenuItem.IsEnabled = this.Root != null && selectedItem != null && !Kernel.Util.ClipbordUtil.IsClipBoardEmpty();
                    this.deleteMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null;
                    this.moveUpMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() > 0 && selectedItem.parent != null;
                    this.moveDownMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && !((selectedItem.GetPosition() + 1) == lastPosition);
                    this.indentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.GetPosition() > 0 && selectedItem.parent != null;
                    this.outdentMenuItem.IsEnabled = this.Root != null && selectedItem != null && selectedItem.parent != null && selectedItem.GetParent() != Root;

            }
        }

        #endregion
        


        #region Node Model Actions
        
        /// <summary>
        /// Ajoute un nouveau noeud fils au noeud passé en paramètre.
        /// </summary>
        /// <param name="model">Le noeud auquel il fau ajouter un fils</param>
        /// <returns>Le nouveau noed créé</returns>
        public virtual Kernel.Domain.Measure AddNode(Kernel.Domain.Measure parent)
        {
            Kernel.Domain.Measure child = GetNewTreeViewModel();
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
            SetSelectedMeasure(child);
            
            if (Changed != null) Changed();
            return child;
        }
        /// <summary>
        /// Supprime un noeud et ses fils.
        /// </summary>
        /// <param name="model">Le noeud à supprimer</param>
        public void DeleteNode(Kernel.Domain.Measure item)
        {
            if (item != null && item.parent != null)
            {
                MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Measure", "Do you want to delete measure : " + item.name + "?");
                if (result == MessageBoxResult.Yes)
                {
                    int index = item.GetPosition();
                    item.GetParent().RemoveChild(item);
                    if (Changed != null) Changed();
                }
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
        public void IndentNode(Kernel.Domain.Measure item)
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
                    SetSelectedMeasure(item);
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
