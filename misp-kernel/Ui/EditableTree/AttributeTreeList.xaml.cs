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

namespace Misp.Kernel.Ui.EditableTree
{
    public partial class AttributeTreeList : UserControl
    {
    
        #region events

        public ChangeEventHandler Changed;

        public ChangeItemEventHandler Expanded;

        public ChangeItemEventHandler ShowMore;

        #endregion


        #region Properties

        /// <summary>
        /// Display entity
        /// </summary>
        public Kernel.Domain.Entity Entity { get; set; }
        public Domain.Attribute Root { get; set; }
        public ObservableCollection<Domain.Attribute> Source { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AttributeTreeList()
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
        /// Display entity Attributes.
        /// Builds the root node and calls DisplayRoot()
        /// </summary>
        /// <param name="entity"> Entity to display </param>
        public void DisplayAttribute(Domain.Attribute attribute)
        {
            this.DisplayRoot(attribute);
        }

        /// <summary>
        /// Display children od root node
        /// </summary>
        /// <param name="root"> Attribute representing the root node </param>
        private void DisplayRoot(Domain.Attribute root)
        {
            Source = new ObservableCollection<Domain.Attribute>();
            this.Root = root;
            if (this.Root != null)
            {
                ForgetDefaultAttributes(this.Root);
                AddDefaultAttributes(this.Root);
                RefreshParent(this.Root);
            }
            treeList.ItemsSource = Source;  
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.Attribute item, bool addToSource = true)
        {
            if (item != null)
            {
                foreach (Domain.Attribute child in item.childrenListChangeHandler.Items)
                {
                    if (addToSource) Source.Add(child);
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        public void AddDefaultAttributes(Domain.Attribute parent)
        {
            Domain.Attribute addNewAttribute = new Kernel.Domain.Attribute();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new attribute...";
            addNewAttribute.parent = this.Root;
            this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);

            if (parent.isCompleted && parent.HasMoreElements())
            {
                Domain.Attribute showModeAttributes = new Domain.Attribute();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more attribute...";
                showModeAttributes.parent = parent;
                parent.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
            if (parent != this.Root && this.Root.isCompleted && this.Root.HasMoreElements())
            {
                Domain.Attribute showModeAttributes = new Domain.Attribute();
                showModeAttributes.IsShowMoreItem = true;
                showModeAttributes.name = "Show more attribute...";
                showModeAttributes.parent = this.Root;
                this.Root.childrenListChangeHandler.Items.Add(showModeAttributes);
            }
        }

        /// <summary>
        /// Remove default nodes from root attribute
        /// </summary>
        public void ForgetDefaultAttributes(Domain.Attribute parent)
        {
            foreach (Domain.Attribute value in parent.childrenListChangeHandler.Items.ToArray())
            {
                if (value.IsDefault) parent.childrenListChangeHandler.Items.Remove(value);
                //this.Source.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.Attribute value in this.Root.childrenListChangeHandler.Items.ToArray())
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
        /// <returns>The selectected Attribute</returns>
        public Domain.Attribute GetSelectedValue()
        {
            return this.treeList.SelectedItem != null ?
                this.treeList.SelectedItem as Domain.Attribute : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected Attributes</returns>
        public List<Domain.Attribute> GetSelectedValues()
        {
            List<Domain.Attribute> attributes = new List<Domain.Attribute>(0);
            if (this.treeList.SelectedItems != null)
            {
                foreach (object item in this.treeList.SelectedItems)
                {
                    if (item is Domain.Attribute) attributes.Add((Domain.Attribute)item);
                }
            }
            return attributes;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="attribute">The Attribute to select</param>
        public void SetSelectedValue(Domain.Attribute value)
        {
            if (value != null)
            {
                if (value.parent != null) value.parent.IsExpanded = true;
                value.IsSelected = true;
                treeList.SelectedItem = value;
            }
            else
            {
                Domain.Attribute selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
                treeList.SelectedItem = null;
            }
        }
        
        /// <summary>
        /// Can given items be move up
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveUp(List<Domain.Attribute> selectedItems)
        {
            foreach (Domain.Attribute attribute in selectedItems)
            {
                Domain.Attribute parent = attribute != null ? attribute.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attribute) : -1;
                if(index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be move down
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveDown(List<Domain.Attribute> selectedItems)
        {
            foreach (Domain.Attribute attribute in selectedItems)
            {
                Domain.Attribute parent = attribute != null ? attribute.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attribute) : -1;
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
        protected bool canOutdent(List<Domain.Attribute> selectedItems)
        {
            foreach (Domain.Attribute attribute in selectedItems)
            {
                if (attribute == null || attribute.parent == null || attribute.parent == this.Root) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be outdent
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canIndent(List<Domain.Attribute> selectedItems)
        {
            foreach (Domain.Attribute attribute in selectedItems)
            {
                Domain.Attribute parent = attribute != null ? attribute.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(attribute) : -1;
                if (index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool isContiguous(List<Domain.Attribute> selectedItems)
        {
            if (selectedItems.Count == 1) return true;
            selectedItems.BubbleSort();
            Domain.Attribute parent = null;
            int i = 0;
            int index = -1;
            foreach (Domain.Attribute attribute in selectedItems)
            {
                Domain.Attribute newparent = attribute.parent;
                int newindex = newparent != null ? newparent.childrenListChangeHandler.Items.IndexOf(attribute) : -1;
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
        protected bool isContainsDefault(List<Domain.Attribute> selectedItems)
        {
            if (selectedItems.Count == 1) return selectedItems[0].IsDefault;            
            foreach (Domain.Attribute attribute in selectedItems)
            {
                if (attribute.IsDefault) return true;
            }
            return false;
        }

        protected Domain.Attribute GetCopy(Domain.Attribute attribute)
        {
            Domain.Attribute copy = new Domain.Attribute();

            copy.name = "Copy Of " + attribute.name;
            copy.IsDefault = false;
            copy.position = attribute.position;
            copy.parent = null;
            foreach (Domain.Attribute child in attribute.childrenListChangeHandler.Items)
            {
                Domain.Attribute childcopy = GetCopy(child);
                copy.AddChild(childcopy);
                childcopy.parent = null;
            }
            return copy;
        }

        #endregion


        #region Handlers

        private void OnCustomCellAppearance(object sender, CustomCellAppearanceEventArgs e)
        {
            object row = treeList.GetRow(e.RowHandle);
            
            if (row != null && ((Domain.Attribute)row).IsDefault)
            {
                bool isForeground = e.Property != null && e.Property.Name == "Foreground";
                bool isBackground = e.Property != null && e.Property.Name == "Background";
                if (isForeground) e.Result = Brushes.Red;
                e.Handled = true;
            }

        }

        private void OnShownEditor(object sender, TreeListEditorEventArgs e)
        {
            TextEdit editor = (TextEdit) treeListView.ActiveEditor;
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
                    Domain.Attribute attribute = GetSelectedValue();
                    if (attribute != null && ValidateName(attribute, name))
                    {
                        if (attribute.IsDefault)
                        {
                            attribute.name = oldName;
                            Domain.Attribute newAttribute = new Domain.Attribute();
                            newAttribute.name = name;
                            newAttribute.parent = this.Root;
                            ForgetDefaultAttributes(this.Root);
                            this.Root.AddChild(newAttribute);
                            AddDefaultAttributes(this.Root);

                            int row = Source.Count;
                            if (row - 2 >= 0) Source.Insert(row - 2, newAttribute);
                            else if (row - 1 >= 0) Source.Insert(row - 1, newAttribute);
                            else Source.Add(newAttribute);
                            SetSelectedValue(newAttribute);
                        }
                        else
                        {
                            attribute.name = name;
                            ForgetDefaultAttributes(attribute.parent);
                            attribute.parent.UpdateChild(attribute);
                            AddDefaultAttributes(attribute.parent);
                            SetSelectedValue(attribute);
                        }
                        if (Changed != null) Changed();
                    }
                    else attribute.name = oldName;
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
            List<Domain.Attribute> selectedItems = GetSelectedValues();
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
                this.pasteMenuItem.IsEnabled = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyAttribute() && !containsDefault;
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
            Domain.Attribute parent = GetSelectedValue();
            if (IsUsedToGenerateUniverse(parent)) return;
            Kernel.Domain.Attribute attribute = GetNewAttribute();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultAttributes(parent);
                parent.AddChild(attribute);
                AddDefaultAttributes(parent);
                
                int row = Source.Count;
                Source.Remove(attribute);
                if (row - 2 >= 0) Source.Insert(row - 2, attribute);
                else Source.Add(attribute);
                SetSelectedValue(attribute);

                if (Changed != null) Changed();
            }

        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Attribute> attributes = GetSelectedValues();            
            if (attributes.Count == 0) return;
            Kernel.Util.ClipbordUtil.ClearClipboard();
            List<Object> listeCopy = new List<Object>(0);
            foreach (Domain.Attribute attribute in attributes)
            {
                ForgetDefaultAttributes(attribute);
                Domain.Attribute copy = GetCopy(attribute);
                copy.parent = null;
                listeCopy.Add(copy);
            }
            if (listeCopy.Count == 0) return;
            Kernel.Util.ClipbordUtil.SetAttributes(listeCopy);
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            Domain.Attribute parent = GetSelectedValue();
            if (parent == null || parent.IsDefault) parent = this.Root;
            List<Domain.Attribute> attributes = Kernel.Util.ClipbordUtil.GetAttributes();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (Domain.Attribute attribute in attributes)
                {
                    ForgetDefaultAttributes(parent);
                    attribute.SetParent(parent);
                    parent.AddChild(attribute);
                    AddDefaultAttributes(parent);
                    addToSource(attribute);
                }
                if (Changed != null) Changed();
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Attribute> attributes = GetSelectedValues();
            if (attributes.Count == 0) return;
            String message = "Do you want to delete Attribute: '" + attributes[0] + "' ?";
            if (attributes.Count > 1) message = "Do you want to delete the " + attributes.Count + " selected attributes ?";
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Attribute", message);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.Attribute attribute in attributes)
                {
                    if (IsUsedToGenerateUniverse(attribute)) return;
                    Domain.Attribute parent = attribute.parent;

                    ForgetDefaultAttributes(parent);
                    if (attribute.oid.HasValue) parent.RemoveChild(attribute);
                    else parent.ForgetChild(attribute);
                    AddDefaultAttributes(parent);
                    removeFromSource(attribute);
                }
                if (Changed != null) Changed();
            }

        }

        private void removeFromSource(Domain.Attribute attribute)
        {
            Source.Remove(attribute);
            foreach (Domain.Attribute child in attribute.childrenListChangeHandler.Items)
            {
                removeFromSource(child);
            }
        }

        private void addToSource(Domain.Attribute attribute)
        {            
            int row = Source.Count;
            if (row - 2 >= 0) Source.Insert(row - 2, attribute);
            else if (row - 1 >= 0) Source.Insert(row - 1, attribute);
            else Source.Add(attribute);
            attribute.childrenListChangeHandler.Items = new ObservableCollection<Domain.Attribute>(attribute.childrenListChangeHandler.newItems);
            foreach (Domain.Attribute child in attribute.childrenListChangeHandler.Items)
            {
                child.SetParent(attribute);
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
            List<Domain.Attribute> attributes = GetSelectedValues();            
            if (attributes.Count == 0) return;
            attributes.BubbleSort();
            foreach (Domain.Attribute attribute in attributes)
            {
                Domain.Attribute parent = attribute.parent;
                if (parent == null) parent = this.Root;
                int position = attribute.GetPosition();
                Domain.Attribute brother = (Domain.Attribute)parent.GetChildByPosition(position - 1);
                if (brother == null) return;
                ForgetDefaultAttributes(parent);
                parent.ForgetChild(attribute);
                brother.AddChild(attribute);                
                brother.IsExpanded = true;

                int row = Source.IndexOf(brother);
                Source.Remove(brother);
                Source.Insert(row, brother);
                AddDefaultAttributes(parent);
            }
            treeList.SelectedItems = attributes;
            if (Changed != null) Changed();
        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Attribute> attributes = GetSelectedValues();
            if (attributes.Count == 0) return;
            attributes.BubbleSort();
            foreach (Domain.Attribute attribute in attributes)
            {
                Domain.Attribute parent = attribute.parent;
                if (parent == null) parent = this.Root;
                Domain.Attribute grandParent = parent.parent;
                if (grandParent == null) return;

                ForgetDefaultAttributes(grandParent);
                parent.ForgetChild(attribute);
                grandParent.AddChild(attribute);                
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
                    Source.Remove(attribute);
                    if (row -2 >= 0) Source.Insert(row-2, attribute);
                    else Source.Add(attribute);
                }
                AddDefaultAttributes(grandParent);
            }
            treeList.SelectedItems = attributes;
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            List<Domain.Attribute> attributes = GetSelectedValues();            
            if (attributes.Count == 0) return;
            if (up) attributes.BubbleSort();
            else attributes.BubbleSortDesc();
            foreach (Domain.Attribute attribute in attributes)
            {
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

                    int row = Source.IndexOf(child);
                    Source.Remove(attribute);
                    Source.Insert(row, attribute);
                }
                AddDefaultAttributes(parent);
            }
            treeList.SelectedItems = attributes;
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
        private bool ValidateName(Kernel.Domain.Attribute value, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Attribute name", "Name can't be empty! ");
                return false;
            }
            Domain.Attribute found = getAttributeByName(this.Root, name);
            if (found == null || found.Equals(value)) return true;

            Kernel.Util.MessageDisplayer.DisplayError("Duplicate Attribute", "There is another attribute named : '" + name + "'!");
            return false;
        }

        protected Domain.Attribute getAttributeByName(Domain.Attribute parent, string name)
        {
            foreach (Domain.Attribute value in parent.childrenListChangeHandler.Items)
            {
                if (value.IsDefault) continue;
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
                Domain.Attribute child = getAttributeByName(value, name);
                if (child != null) return child;
            }
            return null;
        }

        protected Domain.Attribute GetNewAttribute()
        {
            Domain.Attribute attribute = new Domain.Attribute();
            attribute.name = "Attribute";
            if (Root != null)
            {
                Kernel.Domain.Attribute m = null;
                int i = 1;
                do
                {
                    attribute.name = "Attribute" + i++;
                    m = (Domain.Attribute)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        private bool IsUsedToGenerateUniverse(Domain.Attribute attribute)
        {
            if (this.Entity != null && this.Entity.model != null && this.Entity.usedToGenerateUniverse && this.Entity.model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                string message = "You're not allowed to add a new attribute." + "\n" + "You have to clear allocation before add attribute.";
                Kernel.Util.MessageDisplayer.DisplayWarning("Add attribute", message);
                return true;
            }
            return false;
        }
        
        #endregion

    }
}
