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
    /// Interaction logic for AttributeValueEditableTree.xaml
    /// </summary>
    public partial class AttributeValueEditableTree : UserControl, INotifyPropertyChanged
    {

        #region events

        public ChangeEventHandler Changed;

        public ChangeItemEventHandler Expanded;

        public ChangeItemEventHandler ShowMore;

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
        /// Display Attribute
        /// </summary>
        public Domain.Attribute Attribute { get; set; }

        /// <summary>
        /// Root node
        /// </summary>
        public Domain.AttributeValue Root { get; set; }
        
        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AttributeValueEditableTree()
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
        public void DisplayAttribute(Domain.Attribute attribute)
        {
            this.Attribute = attribute;
            if (attribute != null)
            {
                if (attribute.IsDefault)
                {
                    this.Attribute = null;
                    this.DisplayRoot(null);
                    return;
                }
                Domain.AttributeValue root = new Kernel.Domain.AttributeValue();
                root.childrenListChangeHandler = attribute.valueListChangeHandler;
                root.DataFilter = this.Attribute.DataFilter;
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
        /// <param name="root"> AttributeValue representing the root node </param>
        private void DisplayRoot(Domain.AttributeValue root)
        {
            this.Root = root;
            if (this.Root == null) this.treeView.ItemsSource = null;
            else
            {
                ForgetDefaultAttributeValues(this.Root);
                RefreshParent(this.Root);
                AddDefaultAttributeValues(this.Root);                
                this.treeView.ItemsSource = this.Root.childrenListChangeHandler.Items;
            }
        }
        
        protected void AddDefaultAttributeValues(Domain.AttributeValue parent)
        {
            Domain.AttributeValue addNewAttribute = new Kernel.Domain.AttributeValue();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new Value...";
            addNewAttribute.parent = this.Root;
            this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);

            if (parent.isCompleted && parent.HasMoreElements())
            {
                Domain.AttributeValue showModeAttributes = new Domain.AttributeValue();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more values...";
                showModeAttributes.parent = parent;
                parent.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
            if (parent != this.Root && this.Root.isCompleted && this.Root.HasMoreElements())
            {
                Domain.AttributeValue showModeAttributes = new Domain.AttributeValue();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more values...";
                showModeAttributes.parent = this.Root;
                this.Root.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
        }

        /// <summary>
        /// Remove default nodes from root attribute
        /// </summary>
        protected void ForgetDefaultAttributeValues(Domain.AttributeValue parent)
        {
            foreach (Domain.AttributeValue value in parent.childrenListChangeHandler.Items.ToArray())
            {
                if (value.IsDefault) parent.childrenListChangeHandler.Items.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.AttributeValue value in this.Root.childrenListChangeHandler.Items.ToArray())
                {
                    if (value.IsDefault) this.Root.childrenListChangeHandler.Items.Remove(value);
                }
            }
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.AttributeValue item)
        {
            if (item != null)
            {
                foreach (Domain.AttributeValue child in item.childrenListChangeHandler.Items)
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected AttributeValue</returns>
        public Kernel.Domain.AttributeValue GetSelectedValue()
        {
            return this.treeView.SelectedItem != null ?
                this.treeView.SelectedItem as Domain.AttributeValue : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="attribute">The AttributeValue to select</param>
        public void SetSelectedValue(Domain.AttributeValue value)
        {
            if (value != null)
            {
                if (value.parent != null) value.parent.IsExpanded = true;
                value.IsSelected = true;
            }
            else
            {
                Domain.AttributeValue selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
            }
        }

        #endregion


        #region Handlers

        // if a text box has just become visible, we give it the keyboard input focus and select contents
        private void editableTextBoxHeaderIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
            Domain.AttributeValue value = GetSelectedValue();
            if (value != null && value.IsShowMoreItem)
            {
                IsInEditMode = false;
                return;
            }
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
                    Domain.AttributeValue value = GetSelectedValue();
                    if (value != null && ValidateName(value, name))
                    {
                        if (value.IsDefault)
                        {
                            textBox.Text = oldText;
                            Domain.AttributeValue newValue = new Domain.AttributeValue();
                            newValue.name = name;
                            newValue.parent = this.Root;
                            ForgetDefaultAttributeValues(this.Root);
                            this.Root.AddChild(newValue);
                            AddDefaultAttributeValues(this.Root);
                            SetSelectedValue(newValue);
                        }
                        else
                        {
                            ForgetDefaultAttributeValues(value.parent);
                            value.parent.UpdateChild(value);
                            AddDefaultAttributeValues(value.parent);
                            SetSelectedValue(value);
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
            Domain.AttributeValue value = GetSelectedValue();
            if (value != null && value.IsShowMoreItem)
            {
                ForgetDefaultAttributeValues(value.parent);
                if (value.parent.HasMoreElements() && ShowMore != null) ShowMore(value);
                AddDefaultAttributeValues(value.parent);
                return;
            }
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
                SetSelectedValue(null);
            }
        }

        private void textBlockHeaderMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindTreeItem(e.OriginalSource as DependencyObject).IsSelected = true;
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            Domain.AttributeValue value = (Domain.AttributeValue)((TreeViewItem)e.OriginalSource).Header;
            if (value != this.Root && Expanded != null)
            {
                ForgetDefaultAttributeValues(value);
                Expanded(value);
                AddDefaultAttributeValues(value);
            }
        }

        private void contextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Domain.AttributeValue selectedItem = GetSelectedValue();
            if (selectedItem == null || selectedItem.IsDefault) this.contextMenu.Visibility = Visibility.Collapsed;
            else if (Root != null)
            {
                this.contextMenu.Visibility = Visibility.Visible;
                bool isContiguousSelection = true;// isContiguousList();
                int slectionCount = 1;

                Domain.AttributeValue parent = selectedItem != null ? selectedItem.parent : null;
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
                this.moveUpMenuItem.IsEnabled = canMoveUp;
                this.moveDownMenuItem.IsEnabled = canMoveDown;
                this.indentMenuItem.IsEnabled = canIndent;
                this.outdentMenuItem.IsEnabled = canOutdent;
            }
            else this.contextMenu.Visibility = Visibility.Collapsed;
        }

        
        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            Domain.AttributeValue parent = GetSelectedValue();
            if (IsUsedToGenerateUniverse(parent)) return;            
            Kernel.Domain.AttributeValue value = GetNewAttributeValue();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultAttributeValues(parent);
                parent.AddChild(value);                
                SetSelectedValue(value);
                if (Changed != null) Changed();
                AddDefaultAttributeValues(parent);
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
            Domain.AttributeValue value = GetSelectedValue();
            if (value == null) return;
            if (IsUsedToGenerateUniverse(value)) return;
            Domain.AttributeValue parent = value.parent;
            if (parent == null) parent = this.Root;
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Value", "Do you want to delete Value: '" + value + "' ?");
            if (result == MessageBoxResult.Yes)
            {
                ForgetDefaultAttributeValues(parent);
                if (value.oid.HasValue) parent.RemoveChild(value);
                else parent.ForgetChild(value);
                AddDefaultAttributeValues(parent);
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
            Domain.AttributeValue attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.AttributeValue parent = attribute.parent;
            if (parent == null) parent = this.Root;
            int position = attribute.GetPosition();
            Domain.AttributeValue brother = (Domain.AttributeValue)parent.GetChildByPosition(position - 1);
            if (brother == null) return;

            ForgetDefaultAttributeValues(parent);
            ForgetDefaultAttributeValues(brother);
            parent.ForgetChild(attribute);
            brother.AddChild(attribute);
            AddDefaultAttributeValues(parent);
            AddDefaultAttributeValues(brother);
            brother.IsExpanded = true;
            SetSelectedValue(attribute);
            if (Changed != null) Changed();

        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            Domain.AttributeValue attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.AttributeValue parent = attribute.parent;
            if (parent == null) parent = this.Root;
            Domain.AttributeValue grandParent = parent.parent;
            if (grandParent == null) return;

            ForgetDefaultAttributeValues(parent);
            ForgetDefaultAttributeValues(grandParent);
            parent.ForgetChild(attribute);
            grandParent.AddChild(attribute);
            AddDefaultAttributeValues(parent);
            AddDefaultAttributeValues(grandParent);
            parent.IsExpanded = true;
            SetSelectedValue(attribute);
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            Domain.AttributeValue attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.AttributeValue parent = attribute.parent;
            if (parent == null) parent = this.Root;

            ForgetDefaultAttributeValues(parent);
            int position = attribute.position + (up ? -1 : 1);
            Domain.AttributeValue child = (Domain.AttributeValue)parent.GetChildByPosition(position);
            if (child != null)
            {
                child.SetPosition(attribute.position);
                parent.UpdateChild(child);
                attribute.SetPosition(position);
                parent.UpdateChild(attribute);
                if (Changed != null) Changed();
            }
            AddDefaultAttributeValues(parent);
            SetSelectedValue(attribute);
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
        private bool ValidateName(Kernel.Domain.AttributeValue value, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Attribute Value name", "Name can't be empty! ");
                return false;
            }
            Domain.AttributeValue found = getAttributeValueByName(this.Root, name);
            if (found == null || found.Equals(value)) return true;
            
            Kernel.Util.MessageDisplayer.DisplayError("Duplicate value", "There is another value named : '" + name + "'!");
            return false;
        }

        protected Domain.AttributeValue getAttributeValueByName(Domain.AttributeValue parent, string name)
        {
            foreach (Domain.AttributeValue value in parent.childrenListChangeHandler.Items)
            {
                if (value.IsDefault) continue;
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
                Domain.AttributeValue child = getAttributeValueByName(value, name);
                if (child != null) return child;
            }
            return null;
        }

        protected Domain.AttributeValue GetNewAttributeValue()
        {
            Domain.AttributeValue attribute = new Domain.AttributeValue();
            attribute.name = "Value";
            if (Root != null)
            {
                Kernel.Domain.AttributeValue m = null;
                int i = 1;
                do
                {
                    attribute.name = "Value" + i++;
                    m = (Domain.AttributeValue)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        private bool IsUsedToGenerateUniverse(Domain.AttributeValue value)
        {
            if (value != null && value.usedToGenerateUniverse && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to modify value." + "\n" + "You have to clear allocation before modify value.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Modify value", message);
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