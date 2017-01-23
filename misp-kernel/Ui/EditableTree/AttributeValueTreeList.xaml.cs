using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Ui.Base;
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
using Misp.Kernel.Util;
using System.Collections.ObjectModel;
using Misp.Kernel.Domain;
using System.Web.Script.Serialization;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Editors;
using Misp.Kernel.Domain.Browser;


namespace Misp.Kernel.Ui.EditableTree
{

    public partial class AttributeValueTreeList : UserControl
    {

        #region events

        public ChangeEventHandler Changed;

        public ChangeItemEventHandler Expanded;

        public ChangeItemEventHandler ShowMore;

        #endregion


        #region Properties

        public Domain.Attribute Attribute { get; set; }
        public Domain.AttributeValue Root { get; set; }
        public ObservableCollection<Domain.AttributeValue> Source { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AttributeValueTreeList()
        {
            ThemeManager.SetThemeName(this, "Office2016White");
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
                root.name = "Root";
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
        /// Display entity AttributeValues.
        /// Builds the root node and calls DisplayRoot()
        /// </summary>
        /// <param name="entity"> Entity to display </param>
        public void DisplayAttributeValue(Domain.AttributeValue attributeValue)
        {
            this.DisplayRoot(attributeValue);
        }

        /// <summary>
        /// Display children od root node
        /// </summary>
        /// <param name="root"> AttributeValue representing the root node </param>
        private void DisplayRoot(Domain.AttributeValue root)
        {
            Source = new ObservableCollection<Domain.AttributeValue>();
            this.Root = root;
            if (this.Root != null)
            {
                ForgetDefaultAttributeValues(this.Root);
                AddDefaultAttributeValues(this.Root);
                RefreshParent(this.Root);
            }
            treeList.ItemsSource = Source;
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.AttributeValue item, bool addToSource = true)
        {
            if (item != null)
            {
                foreach (Domain.AttributeValue child in item.childrenListChangeHandler.Items)
                {
                    if (addToSource) Source.Add(child);
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        protected void AddDefaultAttributeValues(Domain.AttributeValue parent)
        {
            Domain.AttributeValue addNewAttribute = new Kernel.Domain.AttributeValue();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new attributeValue...";
            addNewAttribute.parent = this.Root;
            this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);

            if (parent.isCompleted && parent.HasMoreElements())
            {
                Domain.AttributeValue showModeAttributes = new Domain.AttributeValue();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more attributeValue...";
                showModeAttributes.parent = parent;
                parent.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
            if (parent != this.Root && this.Root.isCompleted && this.Root.HasMoreElements())
            {
                Domain.AttributeValue showModeAttributes = new Domain.AttributeValue();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more attributeValue...";
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
                //this.Source.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.AttributeValue value in this.Root.childrenListChangeHandler.Items.ToArray())
                {
                    if (value.IsDefault)
                    {
                        this.Root.childrenListChangeHandler.Items.Remove(value);
                        //this.Source.Remove(value);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected AttributeValue</returns>
        public Domain.AttributeValue GetSelectedValue()
        {
            return this.treeList.SelectedItem != null ?
                this.treeList.SelectedItem as Domain.AttributeValue : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected AttributeValues</returns>
        public List<Domain.AttributeValue> GetSelectedValues()
        {
            List<Domain.AttributeValue> attributeValues = new List<Domain.AttributeValue>(0);
            if (this.treeList.SelectedItems != null)
            {
                foreach (object item in this.treeList.SelectedItems)
                {
                    if (item is Domain.AttributeValue) attributeValues.Add((Domain.AttributeValue)item);
                }
            }
            return attributeValues;
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
                treeList.SelectedItem = value;
            }
            else
            {
                Domain.AttributeValue selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
                treeList.SelectedItem = null;
            }
        }

        /// <summary>
        /// Can given items be move up
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveUp(List<Domain.AttributeValue> selectedItems)
        {
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                Domain.AttributeValue parent = attributeValue != null ? attributeValue.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attributeValue) : -1;
                if (index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be move down
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveDown(List<Domain.AttributeValue> selectedItems)
        {
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                Domain.AttributeValue parent = attributeValue != null ? attributeValue.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attributeValue) : -1;
                int count = parent != null ? parent.childrenListChangeHandler.Items.Count : -1;
                bool moveDown = count - 1 > index && !parent.childrenListChangeHandler.Items[index + 1].IsDefault;
                if (!moveDown) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be outdent
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canOutdent(List<Domain.AttributeValue> selectedItems)
        {
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                if (attributeValue == null || attributeValue.parent == null || attributeValue.parent == this.Root) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be outdent
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canIndent(List<Domain.AttributeValue> selectedItems)
        {
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                Domain.AttributeValue parent = attributeValue != null ? attributeValue.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attributeValue) : -1;
                if (index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool isContiguous(List<Domain.AttributeValue> selectedItems)
        {
            if (selectedItems.Count == 1) return true;
            selectedItems.BubbleSort();
            Domain.AttributeValue parent = null;
            int i = 0;
            int index = -1;
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                Domain.AttributeValue newparent = attributeValue.parent;
                int newindex = newparent != null ? newparent.childrenListChangeHandler.Items.IndexOf(attributeValue) : -1;
                if (++i > 1)
                {
                    if (parent != newparent) return false;
                    if (index + 1 != newindex) return false;
                }
                parent = newparent;
                index = newindex;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool isContainsDefault(List<Domain.AttributeValue> selectedItems)
        {
            if (selectedItems.Count == 1) return selectedItems[0].IsDefault;
            foreach (Domain.AttributeValue attributeValue in selectedItems)
            {
                if (attributeValue.IsDefault) return true;
            }
            return false;
        }

        protected Domain.AttributeValue GetCopy(Domain.AttributeValue attributeValue)
        {
            Domain.AttributeValue copy = new Domain.AttributeValue();

            copy.name = "Copy Of " + attributeValue.name;
            copy.IsDefault = false;
            copy.position = attributeValue.position;
            copy.parent = null;
            foreach (Domain.AttributeValue child in attributeValue.childrenListChangeHandler.Items)
            {
                Domain.AttributeValue childcopy = GetCopy(child);
                copy.AddChild(childcopy);
                childcopy.parent = null;
            }
            return copy;
        }

        public void addPage(Domain.AttributeValue selection, BrowserDataPage<Domain.AttributeValue> page)
        {
            if (!selection.isCompleted)
            {
                foreach (Domain.AttributeValue value in selection.childrenListChangeHandler.originalList.ToArray())
                {
                    selection.childrenListChangeHandler.forget(value);
                    removeFromSource(value);
                }
            }
            Domain.AttributeValue sel = null;
            foreach (Domain.AttributeValue value in page.rows)
            {
                value.parent = selection;
                addToSource(value);
                sel = value;
            }
            selection.childrenListChangeHandler.Items.BubbleSort();
            SetSelectedValue(sel);
        }

        #endregion


        #region Handlers

        private void OnExpanded(object sender, TreeListNodeAllowEventArgs e)
        {
            Domain.AttributeValue value = (Domain.AttributeValue)e.Row;
            if (value != null && value != this.Root && Expanded != null)
            {
                ForgetDefaultAttributeValues(value);
                Expanded(value);
                AddDefaultAttributeValues(value);
            }
        }

        private void OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            Kernel.Domain.AttributeValue value = GetSelectedValue();
            if (value != null && value.IsShowMoreItem)
            {
                ForgetDefaultAttributeValues(value.parent);
                if (value.parent.HasMoreElements() && ShowMore != null) ShowMore(value);
                AddDefaultAttributeValues(value.parent);
                return;
            }
        }

        private void OnCustomCellAppearance(object sender, CustomCellAppearanceEventArgs e)
        {
            object row = treeList.GetRow(e.RowHandle);

            if (row != null && ((Domain.AttributeValue)row).IsDefault)
            {
                bool isForeground = e.Property != null && e.Property.Name == "Foreground";
                bool isBackground = e.Property != null && e.Property.Name == "Background";
                if (isForeground) e.Result = ((Domain.AttributeValue)row).IsAddNewItem ? Brushes.Red : Brushes.Green;
                e.Handled = true;
            }

        }

        private void OnShownEditor(object sender, TreeListEditorEventArgs e)
        {
            TextEdit editor = (TextEdit)treeListView.ActiveEditor;
            editor.ContextMenu = null;
            editor.SelectAll();
        }

        private void OnCellValueChanged(object sender, TreeListCellValueChangedEventArgs e)
        {
            if (e.Row != null)
            {
                String name = e.Value.ToString().Trim();
                String oldName = e.OldValue.ToString().Trim();
                if (!name.Equals(oldName.Trim()))
                {
                    Domain.AttributeValue attributeValue = GetSelectedValue();
                    if (attributeValue != null && ValidateName(attributeValue, name))
                    {
                        if (attributeValue.IsDefault)
                        {
                            attributeValue.name = oldName;
                            Domain.AttributeValue newAttributeValue = new Domain.AttributeValue();
                            newAttributeValue.name = name;
                            newAttributeValue.parent = this.Root;
                            ForgetDefaultAttributeValues(this.Root);
                            this.Root.AddChild(newAttributeValue);
                            AddDefaultAttributeValues(this.Root);

                            int row = Source.Count;
                            if (row - 2 >= 0) Source.Insert(row - 2, newAttributeValue);
                            else if (row - 1 >= 0) Source.Insert(row - 1, newAttributeValue);
                            else Source.Add(newAttributeValue);
                            SetSelectedValue(newAttributeValue);
                        }
                        else
                        {
                            attributeValue.name = name;
                            ForgetDefaultAttributeValues(attributeValue.parent);
                            attributeValue.parent.UpdateChild(attributeValue);
                            AddDefaultAttributeValues(attributeValue.parent);
                            SetSelectedValue(attributeValue);
                        }
                        if (Changed != null) Changed();
                    }
                    else attributeValue.name = oldName;
                }
            }
        }


        /// <summary>
        /// Customize context menu regarding selected items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            List<Domain.AttributeValue> selectedItems = GetSelectedValues();
            if (selectedItems.Count == 0) this.contextMenu.Visibility = Visibility.Collapsed;
            else if (Root != null)
            {
                this.contextMenu.Visibility = Visibility.Visible;
                bool containsDefault = isContainsDefault(selectedItems);
                bool isContiguousSelection = isContiguous(selectedItems);
                int slectionCount = selectedItems.Count;

                this.newMenuItem.IsEnabled = slectionCount <= 1 && (slectionCount == 0 || !selectedItems[0].IsDefault);
                this.cutMenuItem.IsEnabled = slectionCount > 0 && isContiguousSelection && !containsDefault;
                this.copyMenuItem.IsEnabled = slectionCount > 0 && isContiguousSelection && !containsDefault;
                this.pasteMenuItem.IsEnabled = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyAttributeValue() && !containsDefault;
                this.deleteMenuItem.IsEnabled = slectionCount > 0 && !containsDefault;

                this.moveUpMenuItem.IsEnabled = canMoveUp(selectedItems) && isContiguousSelection && !containsDefault;
                this.moveDownMenuItem.IsEnabled = canMoveDown(selectedItems) && isContiguousSelection && !containsDefault;
                this.indentMenuItem.IsEnabled = canIndent(selectedItems) && isContiguousSelection && !containsDefault;
                this.outdentMenuItem.IsEnabled = canOutdent(selectedItems) && isContiguousSelection && !containsDefault;
            }
            else this.contextMenu.Visibility = Visibility.Collapsed;
        }


        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            Domain.AttributeValue parent = GetSelectedValue();
            if (IsUsedToGenerateUniverse(parent)) return;
            Kernel.Domain.AttributeValue attributeValue = GetNewAttributeValue();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultAttributeValues(parent);
                parent.AddChild(attributeValue);
                AddDefaultAttributeValues(parent);

                int row = Source.Count;
                Source.Remove(attributeValue);
                if (row - 2 >= 0) Source.Insert(row - 2, attributeValue);
                else Source.Add(attributeValue);
                SetSelectedValue(attributeValue);

                if (Changed != null) Changed();
            }

        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            List<Domain.AttributeValue> attributeValues = GetSelectedValues();
            if (attributeValues.Count == 0) return;
            Kernel.Util.ClipbordUtil.ClearClipboard();
            List<Object> listeCopy = new List<Object>(0);
            //foreach (Domain.AttributeValue attributeValue in attributeValues)
            //{
            //    ForgetDefaultAttributeValues(attributeValue);
            //    Domain.AttributeValue copy = GetCopy(attributeValue);
            //    copy.parent = null;
            //    listeCopy.Add(copy);
            //}
            //if (listeCopy.Count == 0) return;
            Kernel.Util.ClipbordUtil.SetAttributeValues(attributeValues.ToList<Object>());
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            Domain.AttributeValue parent = GetSelectedValue();
            if (parent == null || parent.IsDefault) parent = this.Root;
            List<Domain.AttributeValue> attributeValues = Kernel.Util.ClipbordUtil.GetAttributeValues();
            if (attributeValues != null && attributeValues.Count > 0)
            {
                foreach (Domain.AttributeValue attributeValue in attributeValues)
                {
                    Domain.AttributeValue copy = GetCopy(attributeValue);
                    copy.name = GetNewAttributeValueName(copy.name);    

                    ForgetDefaultAttributeValues(parent);
                    attributeValue.SetParent(parent);
                    parent.AddChild(copy);
                    AddDefaultAttributeValues(parent);
                    addToSource(copy);
                }
                if (Changed != null) Changed();
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            List<Domain.AttributeValue> attributeValues = GetSelectedValues();
            if (attributeValues.Count == 0) return;
            String message = "Do you want to delete AttributeValue: '" + attributeValues[0] + "' ?";
            if (attributeValues.Count > 1) message = "Do you want to delete the " + attributeValues.Count + " selected attributeValues ?";
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete AttributeValue", message);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.AttributeValue attributeValue in attributeValues)
                {
                    if (IsUsedToGenerateUniverse(attributeValue)) return;
                    Domain.AttributeValue parent = attributeValue.parent;

                    ForgetDefaultAttributeValues(parent);
                    if (attributeValue.oid.HasValue) parent.RemoveChild(attributeValue);
                    else parent.ForgetChild(attributeValue);
                    AddDefaultAttributeValues(parent);
                    removeFromSource(attributeValue);
                }
                if (Changed != null) Changed();
            }

        }

        private void removeFromSource(Domain.AttributeValue attributeValue)
        {
            Source.Remove(attributeValue);
            foreach (Domain.AttributeValue child in attributeValue.childrenListChangeHandler.Items)
            {
                removeFromSource(child);
            }
        }

        private void addToSource(Domain.AttributeValue attributeValue)
        {
            int row = Source.Count;
            if (row - 2 >= 0) Source.Insert(row - 2, attributeValue);
            else if (row - 1 >= 0) Source.Insert(row - 1, attributeValue);
            else Source.Add(attributeValue);
            attributeValue.childrenListChangeHandler.Items = new ObservableCollection<Domain.AttributeValue>(attributeValue.childrenListChangeHandler.newItems);
            foreach (Domain.AttributeValue child in attributeValue.childrenListChangeHandler.Items)
            {
                child.SetParent(attributeValue);
                addToSource(child);
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
            List<Domain.AttributeValue> attributeValues = GetSelectedValues();
            if (attributeValues.Count == 0) return;
            attributeValues.BubbleSort();
            foreach (Domain.AttributeValue attributeValue in attributeValues)
            {
                Domain.AttributeValue parent = attributeValue.parent;
                if (parent == null) parent = this.Root;
                int position = attributeValue.GetPosition();
                Domain.AttributeValue brother = (Domain.AttributeValue)parent.GetChildByPosition(position - 1);
                if (brother == null) return;
                ForgetDefaultAttributeValues(parent);
                parent.ForgetChild(attributeValue);
                brother.AddChild(attributeValue);
                brother.IsExpanded = true;

                int row = Source.IndexOf(brother);
                Source.Remove(brother);
                Source.Insert(row, brother);
                AddDefaultAttributeValues(parent);
            }
            treeList.SelectedItems = attributeValues;
            if (Changed != null) Changed();
        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            List<Domain.AttributeValue> attributeValues = GetSelectedValues();
            if (attributeValues.Count == 0) return;
            attributeValues.BubbleSort();
            foreach (Domain.AttributeValue attributeValue in attributeValues)
            {
                Domain.AttributeValue parent = attributeValue.parent;
                if (parent == null) parent = this.Root;
                Domain.AttributeValue grandParent = parent.parent;
                if (grandParent == null) return;

                ForgetDefaultAttributeValues(grandParent);
                parent.ForgetChild(attributeValue);
                grandParent.AddChild(attributeValue);
                parent.IsExpanded = true;

                int row = Source.IndexOf(grandParent);
                if (row >= 0)
                {
                    Source.Remove(grandParent);
                    Source.Insert(row, grandParent);
                }
                else
                {
                    row = Source.Count;
                    Source.Remove(attributeValue);
                    if (row - 2 >= 0) Source.Insert(row - 2, attributeValue);
                    else Source.Add(attributeValue);
                }
                AddDefaultAttributeValues(grandParent);
            }
            treeList.SelectedItems = attributeValues;
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            List<Domain.AttributeValue> attributeValues = GetSelectedValues();
            if (attributeValues.Count == 0) return;
            if (up) attributeValues.BubbleSort();
            else attributeValues.BubbleSortDesc();
            foreach (Domain.AttributeValue attributeValue in attributeValues)
            {
                Domain.AttributeValue parent = attributeValue.parent;
                if (parent == null) parent = this.Root;

                ForgetDefaultAttributeValues(parent);
                int position = attributeValue.position + (up ? -1 : 1);
                Domain.AttributeValue child = (Domain.AttributeValue)parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(attributeValue.position);
                    parent.UpdateChild(child);
                    attributeValue.SetPosition(position);
                    parent.UpdateChild(attributeValue);

                    int row = Source.IndexOf(child);
                    Source.Remove(attributeValue);
                    Source.Insert(row, attributeValue);
                }
                AddDefaultAttributeValues(parent);
            }
            treeList.SelectedItems = attributeValues;
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
                Kernel.Util.MessageDisplayer.DisplayError("Empty AttributeValue name", "Name can't be empty! ");
                return false;
            }
            Domain.AttributeValue found = getAttributeValueByName(this.Root, name);
            if (found == null || found.Equals(value)) return true;

            Kernel.Util.MessageDisplayer.DisplayError("Duplicate AttributeValue", "There is another attributeValue named : '" + name + "'!");
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
            attribute.name = "AttributeValue";
            if (Root != null)
            {
                Kernel.Domain.AttributeValue m = null;
                int i = 1;
                do
                {
                    attribute.name = "AttributeValue" + i++;
                    m = (Domain.AttributeValue)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        protected String GetNewAttributeValueName(string name)
        {
            Domain.AttributeValue attribute = new Domain.AttributeValue();
            attribute.name = name;
            if (Root != null)
            {
                Kernel.Domain.AttributeValue m = null;
                int i = 1;
                do
                {
                    attribute.name = name + i++;
                    m = (Domain.AttributeValue)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute.name;
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

        #endregion

    }
}
