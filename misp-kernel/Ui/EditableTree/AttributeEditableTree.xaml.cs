using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Misp.Kernel.Ui.EditableTree
{
    /// <summary>
    /// Interaction logic for AttributeEditableTree.xaml
    /// </summary>
    public partial class AttributeEditableTree : UserControl, INotifyPropertyChanged
    {

        #region events

        public ChangeEventHandler Changed;

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This flag indicates whether the tree view items shall (if possible) open in edit mode
        /// </summary>

        bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set
            {
                isInEditMode = value;
                PropertyChangedEventHandler handler = PropertyChanged;
                if(handler != null) handler(this, new PropertyChangedEventArgs("IsInEditMode"));
            }
        }

        /// <summary>
        /// Text in a text box before editing - to enable cancelling changes
        /// </summary>
        string oldText;

        /// <summary>
        /// Display entity
        /// </summary>
        public Kernel.Domain.Entity Entity { get; set; }

        /// <summary>
        /// Root node
        /// </summary>
        public Kernel.Domain.Attribute Root { get; set; }
        
        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AttributeEditableTree()
        {
            InitializeComponent();
        }
        
        #endregion

        #region Operations

        /// <summary>
        /// Display entity attributes.
        /// Builds the root node and calls DisplayRoot()
        /// </summary>
        /// <param name="entity"> Entity to display </param>
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
        /// Display children od root node
        /// </summary>
        /// <param name="root"> Attribute representing the root node </param>
        private void DisplayRoot(Kernel.Domain.Attribute root)
        {
            this.Root = root;
            if (this.Root == null) this.treeView.ItemsSource = null;
            else
            {
                ForgetDefaultAttributes(this.Root);
                RefreshParent(this.Root);
                AddDefaultAttributes(this.Root);                
                this.treeView.ItemsSource = this.Root.childrenListChangeHandler.Items;
            }
        }

        public void AddDefaultAttributes(Domain.Attribute parent)
        {
            Kernel.Domain.Attribute addNewAttribute = new Kernel.Domain.Attribute();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new Attribute...";
            addNewAttribute.parent = parent;

            Kernel.Domain.Attribute showModeAttributes = new Kernel.Domain.Attribute();
            showModeAttributes.IsShowMoreItem = true;
            showModeAttributes.name = "Show more Attributes...";
            showModeAttributes.parent = parent;
            if (parent == this.Root)
            {
                //parent.childrenListChangeHandler.Items.Add(showModeAttributes);
                parent.childrenListChangeHandler.Items.Add(addNewAttribute);
            }
            else
            {
                //this.Root.childrenListChangeHandler.Items.Add(showModeAttributes);
                this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);
            }
        }

        /// <summary>
        /// Remove default nodes from root attribute
        /// </summary>
        public void ForgetDefaultAttributes(Domain.Attribute parent)
        {
            foreach (Kernel.Domain.Attribute attribute in parent.childrenListChangeHandler.Items.ToArray())
            {
                if (attribute.IsDefault) parent.childrenListChangeHandler.Items.Remove(attribute);
            }
            if (parent != this.Root)
            {
                foreach (Kernel.Domain.Attribute attribute in this.Root.childrenListChangeHandler.Items.ToArray())
                {
                    if (attribute.IsDefault) this.Root.childrenListChangeHandler.Items.Remove(attribute);
                }
            }
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.Attribute item)
        {
            if (item != null)
            {
                foreach (Kernel.Domain.Attribute child in item.childrenListChangeHandler.Items)
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected attribute</returns>
        public Kernel.Domain.Attribute GetSelectedValue()
        {
            return this.treeView.SelectedItem != null ?
                this.treeView.SelectedItem as Kernel.Domain.Attribute : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="attribute">The attribute to select</param>
        public void SetSelectedAttribute(Kernel.Domain.Attribute attribute)
        {
            if (attribute != null)
            {
                if (attribute.parent != null) attribute.parent.IsExpanded = true;
                attribute.IsSelected = true;
            }
            else
            {
                Kernel.Domain.Attribute selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
                this.treeView.Focus();
            }
        }

        #endregion


        #region Handlers

        // if a text box has just become visible, we give it the keyboard input focus and select contents
        private void editableTextBoxHeaderIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.IsVisible)
            {
                textBox.Focus();
                textBox.SelectAll();
                oldText = textBox.Text.Trim();      // back up - for possible cancelling
            }
        }

        // stop editing on Enter or Escape (then with cancel)
        private void editableTextBoxHeaderKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                String name = textBox.Text.Trim();

                if (!name.Equals(oldText.Trim()))
                {
                    Domain.Attribute attribute = GetSelectedValue();
                    if (attribute != null && ValidateName(attribute, name))
                    {
                        if (attribute.IsDefault)
                        {
                            textBox.Text = oldText;
                            Domain.Attribute newattribute = new Domain.Attribute();
                            newattribute.name = name;
                            newattribute.parent = this.Root;
                            ForgetDefaultAttributes(this.Root);
                            this.Root.AddChild(newattribute);
                            AddDefaultAttributes(this.Root);
                            SetSelectedAttribute(newattribute);
                        }
                        else
                        {
                            ForgetDefaultAttributes(attribute.parent);
                            attribute.parent.UpdateChild(attribute);
                            AddDefaultAttributes(attribute.parent);
                            SetSelectedAttribute(attribute);
                        }
                        if (Changed != null) Changed();
                    }
                    else textBox.Text = oldText;
                }
                IsInEditMode = false;
            }
            if (e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                textBox.Text = oldText;
                IsInEditMode = false;
            }
        }

        // stop editing on lost focus
        private void editableTextBoxHeaderLostFocus(object sender, RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        // it might happen, that the user pressed F2 while a non-editable item was selected
        private void treeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IsInEditMode = false;
        }

        // we (possibly) switch to edit mode when the user presses F2
        private void treeViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                IsInEditMode = true;
        }

        // the user has clicked a header - proceed with editing if it was selected
        private void textBlockHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FindTreeItem(e.OriginalSource as DependencyObject).IsSelected)
            {
                IsInEditMode = true;
                e.Handled = true;       // otherwise the newly activated control will immediately loose focus
            }
        }

        private void mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != this.treeView)
            {
                SetSelectedAttribute(null);
            }
        }

        private void textBlockHeaderMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindTreeItem(e.OriginalSource as DependencyObject).IsSelected = true;
        }

        private void contextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Domain.Attribute selectedItem = GetSelectedValue();
            if (selectedItem == null || selectedItem.IsDefault) this.contextMenu.Visibility = Visibility.Collapsed;
            else if (Root != null)
            {
                this.contextMenu.Visibility = Visibility.Visible;
                bool isContiguousSelection = true;// isContiguousList();
                int slectionCount = 1;

                Domain.Attribute parent = selectedItem != null ? selectedItem.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(selectedItem) : -1;
                int count = parent != null ? parent.childrenListChangeHandler.Items.Count : -1;
                bool canMoveUp = index > 0;
                bool canMoveDown = count - 1 > index && !parent.childrenListChangeHandler.Items[index + 1].IsDefault;
                bool canIndent = index > 0;
                bool canOutdent = parent != null && !parent.Equals(Root);
                
                this.newMenuItem.IsEnabled = (selectedItem == null || !selectedItem.IsDefault) && slectionCount <= 1;
                this.cutMenuItem.IsEnabled = selectedItem != null && !selectedItem.IsDefault && selectedItem.parent != null && isContiguousSelection;
                this.copyMenuItem.IsEnabled = selectedItem != null && !selectedItem.IsDefault && selectedItem.parent != null && isContiguousSelection;
                this.pasteMenuItem.IsEnabled = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyValues() && (selectedItem == null || !selectedItem.IsDefault) && slectionCount <= 1;
                this.deleteMenuItem.IsEnabled =     selectedItem != null && !selectedItem.IsDefault && selectedItem.parent != null && isContiguousSelection;
                this.moveUpMenuItem.IsEnabled =   canMoveUp;
                this.moveDownMenuItem.IsEnabled = canMoveDown;
                this.indentMenuItem.IsEnabled = canIndent;
                this.outdentMenuItem.IsEnabled = canOutdent;
            }
            else this.contextMenu.Visibility = Visibility.Collapsed;
        }

        
        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            if (IsUsedToGenerateUniverse()) return;
            Domain.Attribute parent = GetSelectedValue();
            Kernel.Domain.Attribute attribute = GetNewAttribute();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultAttributes(parent);
                parent.AddChild(attribute);                
                SetSelectedAttribute(attribute);
                if (Changed != null) Changed();
                AddDefaultAttributes(parent);
            }
            
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (IsUsedToGenerateUniverse()) return;
            Domain.Attribute attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Attribute parent = attribute.parent;
            if (parent == null) parent = this.Root;
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Attribute", "Do you want to delete Attribute: '" + attribute + "' ?");
            if (result == MessageBoxResult.Yes)
            {
                ForgetDefaultAttributes(parent);
                if(attribute.oid.HasValue) parent.RemoveChild(attribute);
                else parent.ForgetChild(attribute);
                AddDefaultAttributes(parent);
                if (Changed != null) Changed();
            }
        }

        private void OnMoveupClick(object sender, RoutedEventArgs e)
        {
            OnMove(true);
        }

        private void OnMovedownClick(object sender, RoutedEventArgs e)
        {
            OnMove(false);
        }

        private void OnIndentClick(object sender, RoutedEventArgs e)
        {
            Domain.Attribute attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Attribute parent = attribute.parent;
            if (parent == null) parent = this.Root;
            int position = attribute.GetPosition();
            Domain.Attribute brother = (Domain.Attribute)parent.GetChildByPosition(position - 1);
            if (brother == null) return;

            ForgetDefaultAttributes(parent);
            parent.ForgetChild(attribute);
            brother.AddChild(attribute);
            AddDefaultAttributes(parent);
            brother.IsExpanded = true;
            SetSelectedAttribute(attribute);
            if (Changed != null) Changed();

        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            Domain.Attribute attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Attribute parent = attribute.parent;
            if (parent == null) parent = this.Root;
            Domain.Attribute grandParent = parent.parent;
            if (grandParent == null) return;

            ForgetDefaultAttributes(grandParent);
            parent.ForgetChild(attribute);
            grandParent.AddChild(attribute); 
            AddDefaultAttributes(grandParent);
            parent.IsExpanded = true;
            SetSelectedAttribute(attribute);
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            Domain.Attribute attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Attribute parent = attribute.parent;
            if (parent == null) parent = this.Root;

            ForgetDefaultAttributes(parent);
            int position = attribute.position + (up ? -1 : 1);
            Domain.Attribute child = (Domain.Attribute)parent.GetChildByPosition(position);
            if (child != null)
            {
                child.SetPosition(attribute.position);
                parent.UpdateChild(child);
                attribute.SetPosition(position);
                parent.UpdateChild(attribute);
                if (Changed != null) Changed();
            }
            AddDefaultAttributes(parent);
            SetSelectedAttribute(attribute);
            if (Changed != null) Changed();
        }
        
        #endregion


        #region Utils

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="name"></param>
        /// <returns>La attribute à copier</returns>
        private bool ValidateName(Kernel.Domain.Attribute attribute, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Attribute name", "Name can't be empty! ");
                return false;
            }
            Domain.Attribute found = getAttributeByName(this.Root, name);
            if (found == null || found.Equals(attribute)) return true;
            
            Kernel.Util.MessageDisplayer.DisplayError("Duplicate Attribute", "There is another Attribute named : '" + name + "'!");
            return false;
        }

        protected Domain.Attribute getAttributeByName(Domain.Attribute parent, string name)
        {
            foreach (Domain.Attribute attribute in parent.childrenListChangeHandler.Items)
            {
                if (attribute.IsDefault) continue;
                if (attribute.name.ToUpper().Equals(name.ToUpper())) return attribute;
                Domain.Attribute child = getAttributeByName(attribute, name);
                if (child != null) return child;
            }
            return null;
        }

        protected Kernel.Domain.Attribute GetNewAttribute()
        {
            Kernel.Domain.Attribute attribute = new Kernel.Domain.Attribute();
            attribute.name = "Attribute";
            if (Root != null)
            {
                Kernel.Domain.Attribute m = null;
                int i = 1;
                do
                {
                    attribute.name = "Attribute" + i++;
                    m = (Kernel.Domain.Attribute)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        private bool IsUsedToGenerateUniverse()
        {
            if (this.Entity != null && this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to add a new attribute." + "\n" + "You have to clear allocation before add attribute.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Add attribute", message);
                return true;
            }
            return false;
        }


        // searches for the corresponding TreeViewItem,
        // based on http://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu
        static TreeViewItem FindTreeItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
        }

        #endregion

    }
}