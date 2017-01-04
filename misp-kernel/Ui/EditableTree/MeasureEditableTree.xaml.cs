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
    /// Interaction logic for MeasureEditableTree.xaml
    /// </summary>
    public partial class MeasureEditableTree : UserControl, INotifyPropertyChanged
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
        /// Root node
        /// </summary>
        public Domain.Measure Root { get; set; }
        
        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeasureEditableTree()
        {
            InitializeComponent();
        }
        
        #endregion


        #region Operations

        /// <summary>
        /// Display entity Measures.
        /// Builds the root node and calls DisplayRoot()
        /// </summary>
        /// <param name="entity"> Entity to display </param>
        public void DisplayMeasure(Domain.Measure measure)
        {
            this.DisplayRoot(measure);
        }

        /// <summary>
        /// Display children od root node
        /// </summary>
        /// <param name="root"> Measure representing the root node </param>
        private void DisplayRoot(Domain.Measure root)
        {
            this.Root = root;
            if (this.Root == null) this.treeView.ItemsSource = null;
            else
            {
                ForgetDefaultMeasures(this.Root);
                RefreshParent(this.Root);
                AddDefaultMeasures(this.Root);                
                this.treeView.ItemsSource = this.Root.childrenListChangeHandler.Items;
            }
        }

        protected void AddDefaultMeasures(Domain.Measure parent)
        {
            Domain.Measure addNewAttribute = new Kernel.Domain.Measure();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new measure...";
            addNewAttribute.parent = this.Root;
            this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);

            if (parent.isCompleted && parent.HasMoreElements())
            {
                Domain.Measure showModeAttributes = new Domain.Measure();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more measure...";
                showModeAttributes.parent = parent;
                parent.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
            if (parent != this.Root && this.Root.isCompleted && this.Root.HasMoreElements())
            {
                Domain.Measure showModeAttributes = new Domain.Measure();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more measure...";
                showModeAttributes.parent = this.Root;
                this.Root.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
        }

        /// <summary>
        /// Remove default nodes from root attribute
        /// </summary>
        protected void ForgetDefaultMeasures(Domain.Measure parent)
        {
            foreach (Domain.Measure value in parent.childrenListChangeHandler.Items.ToArray())
            {
                if (value.IsDefault) parent.childrenListChangeHandler.Items.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.Measure value in this.Root.childrenListChangeHandler.Items.ToArray())
                {
                    if (value.IsDefault) this.Root.childrenListChangeHandler.Items.Remove(value);
                }
            }
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.Measure item)
        {
            if (item != null)
            {
                foreach (Domain.Measure child in item.childrenListChangeHandler.Items)
                {
                    child.parent = item;
                    RefreshParent(child);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected Measure</returns>
        public Kernel.Domain.Measure GetSelectedValue()
        {
            return this.treeView.SelectedItem != null ?
                this.treeView.SelectedItem as Domain.Measure : null;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="attribute">The Measure to select</param>
        public void SetSelectedValue(Domain.Measure value)
        {
            if (value != null)
            {
                if (value.parent != null) value.parent.IsExpanded = true;
                value.IsSelected = true;
            }
            else
            {
                Domain.Measure selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
            }
        }

        #endregion


        #region Handlers

        // if a text box has just become visible, we give it the keyboard input focus and select contents
        private void editableTextBoxHeaderIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
            Domain.Measure value = GetSelectedValue();
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
                    Domain.Measure value = GetSelectedValue();
                    if (value != null && ValidateName(value, name))
                    {
                        if (value.IsDefault)
                        {
                            textBox.Text = oldText;
                            Domain.Measure newValue = new Domain.Measure();
                            newValue.name = name;
                            newValue.parent = this.Root;
                            ForgetDefaultMeasures(this.Root);
                            this.Root.AddChild(newValue);
                            AddDefaultMeasures(this.Root);
                            SetSelectedValue(newValue);
                        }
                        else
                        {
                            value.name = name;
                            ForgetDefaultMeasures(value.parent);
                            value.parent.UpdateChild(value);
                            AddDefaultMeasures(value.parent);
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
            Domain.Measure value = GetSelectedValue();
            if (value != null && value.IsShowMoreItem)
            {
                ForgetDefaultMeasures(value.parent);
                if (value.parent.HasMoreElements() && ShowMore != null) ShowMore(value);
                AddDefaultMeasures(value.parent);
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
            Domain.Measure value = (Domain.Measure)((TreeViewItem)e.OriginalSource).Header;
            if (value != this.Root && Expanded != null)
            {
                ForgetDefaultMeasures(value);
                Expanded(value);
                AddDefaultMeasures(value);
            }
        }

        private void contextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Domain.Measure selectedItem = GetSelectedValue();
            if (selectedItem == null || selectedItem.IsDefault) this.contextMenu.Visibility = Visibility.Collapsed;
            else if (Root != null)
            {
                this.contextMenu.Visibility = Visibility.Visible;
                bool isContiguousSelection = true;// isContiguousList();
                int slectionCount = 1;

                Domain.Measure parent = selectedItem != null ? selectedItem.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(selectedItem) : -1;
                int count = parent != null ? parent.childrenListChangeHandler.Items.Count : -1;
                bool canMoveUp = index > 0;
                bool canMoveDown = count - 1 > index && !parent.childrenListChangeHandler.Items[index + 1].IsDefault;
                bool canIndent = index > 0;
                bool canOutdent = parent != null && !parent.Equals(Root);

                this.newMenuItem.IsEnabled = (selectedItem == null || !selectedItem.IsDefault) && slectionCount <= 1;
                this.cutMenuItem.IsEnabled = selectedItem != null && !selectedItem.IsDefault && selectedItem.parent != null && isContiguousSelection;
                this.copyMenuItem.IsEnabled = selectedItem != null && !selectedItem.IsDefault && selectedItem.parent != null && isContiguousSelection;
                this.pasteMenuItem.IsEnabled = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyAttributeValue() && (selectedItem == null || !selectedItem.IsDefault) && slectionCount <= 1;
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
            Domain.Measure parent = GetSelectedValue();
            if (IsUsedToGenerateUniverse(parent)) return;            
            Kernel.Domain.Measure value = GetNewMeasure();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultMeasures(parent);
                parent.AddChild(value);
                AddDefaultMeasures(parent);              
                SetSelectedValue(value);
                if (Changed != null) Changed();
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
            Domain.Measure attribute = GetSelectedValue();
            if (attribute == null) return;
            if (IsUsedToGenerateUniverse(attribute)) return;            
            Domain.Measure parent = attribute.parent;
            if (parent == null) parent = this.Root;
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Measure", "Do you want to delete Measure: '" + attribute + "' ?");
            if (result == MessageBoxResult.Yes)
            {
                ForgetDefaultMeasures(parent);
                if(attribute.oid.HasValue) parent.RemoveChild(attribute);
                else parent.ForgetChild(attribute);
                AddDefaultMeasures(parent);
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
            Domain.Measure attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Measure parent = attribute.parent;
            if (parent == null) parent = this.Root;
            int position = attribute.GetPosition();
            Domain.Measure brother = (Domain.Measure)parent.GetChildByPosition(position - 1);
            if (brother == null) return;

            ForgetDefaultMeasures(parent);
            parent.ForgetChild(attribute);
            brother.AddChild(attribute);
            AddDefaultMeasures(parent);
            brother.IsExpanded = true;
            SetSelectedValue(attribute);
            if (Changed != null) Changed();

        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            Domain.Measure attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Measure parent = attribute.parent;
            if (parent == null) parent = this.Root;
            Domain.Measure grandParent = parent.parent;
            if (grandParent == null) return;

            ForgetDefaultMeasures(grandParent);
            parent.ForgetChild(attribute);
            grandParent.AddChild(attribute);
            AddDefaultMeasures(grandParent);
            parent.IsExpanded = true;
            SetSelectedValue(attribute);
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            Domain.Measure attribute = GetSelectedValue();
            if (attribute == null) return;
            Domain.Measure parent = attribute.parent;
            if (parent == null) parent = this.Root;

            ForgetDefaultMeasures(parent);
            int position = attribute.position + (up ? -1 : 1);
            Domain.Measure child = (Domain.Measure)parent.GetChildByPosition(position);
            if (child != null)
            {
                child.SetPosition(attribute.position);
                parent.UpdateChild(child);
                attribute.SetPosition(position);
                parent.UpdateChild(attribute);
                if (Changed != null) Changed();
            }
            AddDefaultMeasures(parent);
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
        private bool ValidateName(Kernel.Domain.Measure value, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Measure name", "Name can't be empty! ");
                return false;
            }
            Domain.Measure found = getMeasureByName(this.Root, name);
            if (found == null || found.Equals(value)) return true;

            Kernel.Util.MessageDisplayer.DisplayError("Duplicate Measure", "There is another measure named : '" + name + "'!");
            return false;
        }

        protected Domain.Measure getMeasureByName(Domain.Measure parent, string name)
        {
            foreach (Domain.Measure value in parent.childrenListChangeHandler.Items)
            {
                if (value.IsDefault) continue;
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
                Domain.Measure child = getMeasureByName(value, name);
                if (child != null) return child;
            }
            return null;
        }

        protected Domain.Measure GetNewMeasure()
        {
            Domain.Measure attribute = new Domain.Measure();
            attribute.name = "Measure";
            if (Root != null)
            {
                Kernel.Domain.Measure m = null;
                int i = 1;
                do
                {
                    attribute.name = "Measure" + i++;
                    m = (Domain.Measure)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        private bool IsUsedToGenerateUniverse(Domain.Measure value)
        {
            //if (value != null && value.usedToGenerateUniverse && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            //{
            //    string message = "You're not allowed to modify value." + "\n" + "You have to clear allocation before modify value.";
            //    Kernel.Util.MessageDisplayer.DisplayWarning("Modify value", message);
            //    return true;
            //}
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