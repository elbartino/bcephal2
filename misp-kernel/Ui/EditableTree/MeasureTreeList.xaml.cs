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
    /// <summary>
    /// Interaction logic for MeasureTreeList.xaml
    /// </summary>
    public partial class MeasureTreeList : UserControl
    {

        #region events

        public ChangeEventHandler Changed;

        public ChangeItemEventHandler Expanded;

        public ChangeItemEventHandler ShowMore;

        #endregion


        #region Properties

        public Domain.Measure Root { get; set; }
        public ObservableCollection<Domain.Measure> Source { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeasureTreeList()
        {
            ThemeManager.SetThemeName(this, "Office2016White");
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
            Source = new ObservableCollection<Domain.Measure>();
            this.Root = root;
            AddDefaultMeasures(this.Root);
            RefreshParent(this.Root);            
            treeList.ItemsSource = Source;  
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.Measure item, bool addToSource = true)
        {
            if (item != null)
            {
                foreach (Domain.Measure child in item.childrenListChangeHandler.Items)
                {
                    if (addToSource) Source.Add(child);
                    child.SetParent(item);
                    RefreshParent(child);
                }
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
                //this.Source.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.Measure value in this.Root.childrenListChangeHandler.Items.ToArray())
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
        /// <returns>The selectected Measure</returns>
        public Domain.Measure GetSelectedValue()
        {
            return this.treeList.SelectedItem != null ?
                this.treeList.SelectedItem as Domain.Measure : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected Measures</returns>
        public List<Domain.Measure> GetSelectedValues()
        {
            List<Domain.Measure> measures = new List<Domain.Measure>(0);
            if (this.treeList.SelectedItems != null)
            {
                foreach (object item in this.treeList.SelectedItems)
                {
                    if (item is Domain.Measure) measures.Add((Domain.Measure)item);
                }
            }
            return measures;
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
                treeList.SelectedItem = value;
            }
            else
            {
                Domain.Measure selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
                treeList.SelectedItem = null;
            }
        }
        
        /// <summary>
        /// Can given items be move up
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveUp(List<Domain.Measure> selectedItems)
        {
            foreach (Domain.Measure measure in selectedItems)
            {
                Domain.Measure parent = measure != null ? measure.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(measure) : -1;
                if(index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be move down
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveDown(List<Domain.Measure> selectedItems)
        {
            foreach (Domain.Measure measure in selectedItems)
            {
                Domain.Measure parent = measure != null ? measure.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(measure) : -1;
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
        protected bool canOutdent(List<Domain.Measure> selectedItems)
        {
            foreach (Domain.Measure measure in selectedItems)
            {
                if (measure == null || measure.parent == null || measure.parent == this.Root) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be outdent
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canIndent(List<Domain.Measure> selectedItems)
        {
            foreach (Domain.Measure measure in selectedItems)
            {
                Domain.Measure parent = measure != null ? measure.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(measure) : -1;
                if (index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool isContiguous(List<Domain.Measure> selectedItems)
        {
            if (selectedItems.Count == 1) return true;
            selectedItems.BubbleSort();
            Domain.Measure parent = null;
            int i = 0;
            int index = -1;
            foreach (Domain.Measure measure in selectedItems)
            {
                Domain.Measure newparent = measure.parent;
                int newindex = newparent != null ? newparent.childrenListChangeHandler.Items.IndexOf(measure) : -1;
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
        protected bool isContainsDefault(List<Domain.Measure> selectedItems)
        {
            if (selectedItems.Count == 1) return selectedItems[0].IsDefault;            
            foreach (Domain.Measure measure in selectedItems)
            {
                if (measure.IsDefault) return true;
            }
            return false;
        }

        protected Domain.Measure GetCopy(Domain.Measure measure)
        {
            Domain.Measure copy = new Domain.Measure();

            copy.name = "Copy Of " + measure.name;
            copy.IsDefault = false;
            copy.position = measure.position;
            copy.parent = null;
            foreach (Domain.Measure child in measure.childrenListChangeHandler.Items)
            {
                Domain.Measure childcopy = GetCopy(child);
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
            
            if (row != null && ((Domain.Measure)row).IsDefault)
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
                String name = e.Value != null ? e.Value.ToString().Trim() : "";
                String oldName = e.OldValue != null ? e.OldValue.ToString().Trim() : "";
                Domain.Measure measure = (Domain.Measure)e.Row;
                if (!ValidateName(measure, name))
                {
                    measure.name = oldName;
                    e.Handled = true;
                    return;
                }
                if (!name.Equals(oldName.Trim()))
                {
                    if (measure.IsDefault)
                    {
                        measure.name = oldName;
                        Domain.Measure newMeasure = new Domain.Measure();
                        newMeasure.name = name;
                        newMeasure.parent = this.Root;
                        ForgetDefaultMeasures(this.Root);
                        this.Root.AddChild(newMeasure);
                        AddDefaultMeasures(this.Root);

                        int row = Source.Count;
                        if (row > 0) Source.Insert(row - 1, newMeasure);
                        else Source.Add(newMeasure);
                        SetSelectedValue(newMeasure);
                    }
                    else
                    {
                        measure.name = name;
                        ForgetDefaultMeasures(measure.parent);
                        measure.parent.UpdateChild(measure);
                        AddDefaultMeasures(measure.parent);
                        SetSelectedValue(measure);
                    }
                    if (Changed != null) Changed();
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
            List<Domain.Measure> selectedItems = GetSelectedValues();
            if (selectedItems.Count == 0) this.contextMenu.Visibility = Visibility.Collapsed;
            else if (Root != null)
            {                
                bool containsDefault = isContainsDefault(selectedItems);
                if (containsDefault)
                {
                    this.contextMenu.Visibility = Visibility.Collapsed;
                    return;
                }
                this.contextMenu.Visibility = Visibility.Visible;
                bool isContiguousSelection = isContiguous(selectedItems);
                int slectionCount = selectedItems.Count;

                this.newMenuItem.IsEnabled = slectionCount <= 1 && (slectionCount == 0 || !selectedItems[0].IsDefault);
                this.cutMenuItem.IsEnabled = slectionCount > 0 && isContiguousSelection && !containsDefault;
                this.copyMenuItem.IsEnabled = slectionCount > 0 && isContiguousSelection && !containsDefault;
                this.pasteMenuItem.IsEnabled = !Kernel.Util.ClipbordUtil.IsClipBoardEmptyMeasure() && !containsDefault;
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
            Domain.Measure parent = GetSelectedValue();
            if (IsUsedToGenerateUniverse(parent)) return;
            Kernel.Domain.Measure measure = GetNewMeasure();
            if (parent == null) parent = this.Root;
            if (parent != null)
            {
                ForgetDefaultMeasures(parent);
                parent.AddChild(measure);
                AddDefaultMeasures(parent);
                
                int row = Source.Count;
                Source.Remove(measure);
                if (row - 2 >= 0) Source.Insert(row - 2, measure);
                else Source.Add(measure);
                SetSelectedValue(measure);

                if (Changed != null) Changed();
            }

        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Measure> measures = GetSelectedValues();            
            if (measures.Count == 0) return;
            Kernel.Util.ClipbordUtil.ClearClipboard();
            List<Object> listeCopy = new List<Object>(0);
            //foreach (Domain.Measure measure in measures)
            //{
            //    ForgetDefaultMeasures(measure);
            //    Domain.Measure copy = GetCopy(measure);
            //    copy.parent = null;
            //    listeCopy.Add(copy);
            //}
            //if (listeCopy.Count == 0) return;
            Kernel.Util.ClipbordUtil.SetMeasures(measures.ToList<Object>());
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            Domain.Measure parent = GetSelectedValue();
            if (parent == null || parent.IsDefault) parent = this.Root;
            List<Domain.Measure> measures = Kernel.Util.ClipbordUtil.GetMeasures();
            if (measures != null && measures.Count > 0)
            {
                foreach (Domain.Measure measure in measures)
                {
                    measure.name = GetNewMeasureName(measure.name);
                    ForgetDefaultMeasures(parent);
                    measure.SetParent(parent);
                    parent.AddChild(measure);
                    AddDefaultMeasures(parent);
                    addToSource(measure);
                }
                measures.Clear();
                if (Changed != null) Changed();
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Measure> measures = GetSelectedValues();
            if (measures.Count == 0) return;
            String message = "Do you want to delete Measure: '" + measures[0] + "' ?";
            if (measures.Count > 1) message = "Do you want to delete the " + measures.Count + " selected measures ?";
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Measure", message);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.Measure measure in measures)
                {
                    if (IsUsedToGenerateUniverse(measure)) return;
                    Domain.Measure parent = measure.parent;

                    ForgetDefaultMeasures(parent);
                    if (measure.oid.HasValue) parent.RemoveChild(measure);
                    else parent.ForgetChild(measure);
                    AddDefaultMeasures(parent);
                    removeFromSource(measure);
                }
                if (Changed != null) Changed();
            }

        }

        private void removeFromSource(Domain.Measure measure)
        {
            Source.Remove(measure);
            foreach (Domain.Measure child in measure.childrenListChangeHandler.Items)
            {
                removeFromSource(child);
            }
        }

        private void addToSource(Domain.Measure measure)
        {            
            int row = Source.Count;
            if (row - 2 >= 0) Source.Insert(row - 2, measure);
            else if (row - 1 >= 0) Source.Insert(row - 1, measure); 
            else Source.Add(measure);
            measure.childrenListChangeHandler.Items = new ObservableCollection<Domain.Measure>(measure.childrenListChangeHandler.newItems);
            foreach (Domain.Measure child in measure.childrenListChangeHandler.Items)
            {
                child.SetParent(measure);
                child.name = GetNewMeasureName(child.name);
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
            List<Domain.Measure> measures = GetSelectedValues();            
            if (measures.Count == 0) return;
            measures.BubbleSort();
            foreach (Domain.Measure measure in measures)
            {
                Domain.Measure parent = measure.parent;
                if (parent == null) parent = this.Root;
                int position = measure.GetPosition();
                Domain.Measure brother = (Domain.Measure)parent.GetChildByPosition(position - 1);
                if (brother == null) return;
                ForgetDefaultMeasures(parent);
                parent.ForgetChild(measure);
                brother.AddChild(measure);                
                brother.IsExpanded = true;

                int row = Source.IndexOf(brother);
                Source.Remove(brother);
                Source.Insert(row, brother);
                AddDefaultMeasures(parent);
            }
            treeList.SelectedItems = measures;
            if (Changed != null) Changed();
        }

        private void OnOutdentClick(object sender, RoutedEventArgs e)
        {
            List<Domain.Measure> measures = GetSelectedValues();
            if (measures.Count == 0) return;
            measures.BubbleSort();
            foreach (Domain.Measure measure in measures)
            {
                Domain.Measure parent = measure.parent;
                if (parent == null) parent = this.Root;
                Domain.Measure grandParent = parent.parent;
                if (grandParent == null) return;

                ForgetDefaultMeasures(grandParent);
                parent.ForgetChild(measure);
                grandParent.AddChild(measure);                
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
                    Source.Remove(measure);
                    if (row -2 >= 0) Source.Insert(row-2, measure);
                    else Source.Add(measure);
                }
                AddDefaultMeasures(grandParent);
            }
            treeList.SelectedItems = measures;
            if (Changed != null) Changed();
        }

        private void OnMove(bool up)
        {
            List<Domain.Measure> measures = GetSelectedValues();            
            if (measures.Count == 0) return;
            if (up) measures.BubbleSort();
            else measures.BubbleSortDesc();
            foreach (Domain.Measure measure in measures)
            {
                Domain.Measure parent = measure.parent;
                if (parent == null) parent = this.Root;

                ForgetDefaultMeasures(parent);
                int position = measure.position + (up ? -1 : 1);
                Domain.Measure child = (Domain.Measure)parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(measure.position);
                    parent.UpdateChild(child);
                    measure.SetPosition(position);
                    parent.UpdateChild(measure);

                    int row = Source.IndexOf(child);
                    Source.Remove(measure);
                    Source.Insert(row, measure);
                }
                AddDefaultMeasures(parent);
            }
            treeList.SelectedItems = measures;
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

        protected String GetNewMeasureName(string name)
        {
            Domain.Measure measure = new Domain.Measure();
            measure.name = name;
            if (Root != null)
            {
                Kernel.Domain.Measure m = (Domain.Measure)Root.GetChildByName(measure.name);
                int i = 1;
                while (m != null) ;
                {
                    measure.name = name + i++;
                    m = (Domain.Measure)Root.GetChildByName(measure.name);
                }                
            }
            return measure.name;
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
        
        #endregion


    }
}
