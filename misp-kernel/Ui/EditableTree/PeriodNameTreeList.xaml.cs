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
    /// Interaction logic for PeriodNameTreeList.xaml
    /// </summary>
    public partial class PeriodNameTreeList : UserControl
    {

        
        #region events

        public ChangeEventHandler Changed;

        public ChangeItemEventHandler Expanded;

        public ChangeItemEventHandler ShowMore;

        #endregion

        
        #region Properties

        public Domain.PeriodName Root { get; set; }
        public ObservableCollection<Domain.PeriodName> Source { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PeriodNameTreeList()
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
        public void DisplayPeriod(Domain.PeriodName period)
        {
            this.DisplayRoot(period);
        }

        /// <summary>
        /// Display children od root node
        /// </summary>
        /// <param name="root"> Measure representing the root node </param>
        private void DisplayRoot(Domain.PeriodName root)
        {
            Source = new ObservableCollection<Domain.PeriodName>();
            this.Root = root;
            AddDefaultPeriods(this.Root);
            RefreshParent(this.Root);            
            treeList.ItemsSource = Source;  
        }

        /// <summary>
        /// Initialize chidren's parent
        /// </summary>
        /// <param name="item"></param>
        private void RefreshParent(Kernel.Domain.PeriodName item, bool addToSource = true)
        {
            if (item != null)
            {
                foreach (Domain.PeriodName child in item.childrenListChangeHandler.Items)
                {
                    if (addToSource) Source.Add(child);
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }

        protected void AddDefaultPeriods(Domain.PeriodName parent)
        {
            Domain.PeriodName addNewAttribute = new Kernel.Domain.PeriodName();
            addNewAttribute.IsAddNewItem = true;
            addNewAttribute.name = "Add new period...";
            addNewAttribute.parent = this.Root;
            this.Root.childrenListChangeHandler.Items.Add(addNewAttribute);
        }

        /// <summary>
        /// Remove default nodes from root attribute
        /// </summary>
        protected void ForgetDefaultPeriods(Domain.PeriodName parent)
        {
            foreach (Domain.PeriodName value in parent.childrenListChangeHandler.Items.ToArray())
            {
                if (value.IsDefault) parent.childrenListChangeHandler.Items.Remove(value);
            }
            if (parent != this.Root)
            {
                foreach (Domain.PeriodName value in this.Root.childrenListChangeHandler.Items.ToArray())
                {
                    if (value.IsDefault)
                    {
                        this.Root.childrenListChangeHandler.Items.Remove(value);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected Measure</returns>
        public Domain.PeriodName GetSelectedValue()
        {
            return this.treeList.SelectedItem != null ?
                this.treeList.SelectedItem as Domain.PeriodName : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The selectected Measures</returns>
        public List<Domain.PeriodName> GetSelectedValues()
        {
            List<Domain.PeriodName> periods = new List<Domain.PeriodName>(0);
            if (this.treeList.SelectedItems != null)
            {
                foreach (object item in this.treeList.SelectedItems)
                {
                    if (item is Domain.PeriodName) periods.Add((Domain.PeriodName)item);
                }
            }
            return periods;
        }

        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="attribute">The Measure to select</param>
        public void SetSelectedValue(Domain.PeriodName value)
        {
            if (value != null)
            {
                if (value.parent != null) value.parent.IsExpanded = true;
                value.IsSelected = true;
                treeList.SelectedItem = value;
            }
            else
            {
                Domain.PeriodName selection = GetSelectedValue();
                if (selection != null) selection.IsSelected = false;
                treeList.SelectedItem = null;
            }
        }
        
        /// <summary>
        /// Can given items be move up
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveUp(List<Domain.PeriodName> selectedItems)
        {
            foreach (Domain.PeriodName period in selectedItems)
            {
                Domain.PeriodName parent = period != null ? period.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(period) : -1;
                if(index <= 0) return false;
            }
            return selectedItems.Count > 0;
        }

        /// <summary>
        /// Can given items be move down
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool canMoveDown(List<Domain.PeriodName> selectedItems)
        {
            foreach (Domain.PeriodName period in selectedItems)
            {
                Domain.PeriodName parent = period != null ? period.parent : null;
                int index = parent != null ? parent.childrenListChangeHandler.Items.IndexOf(period) : -1;
                int count = parent != null ? parent.childrenListChangeHandler.Items.Count : -1;
                bool moveDown = count - 1 > index && !parent.childrenListChangeHandler.Items[index + 1].IsDefault;
                if (!moveDown) return false;
            }
            return selectedItems.Count > 0;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        protected bool isContiguous(List<Domain.PeriodName> selectedItems)
        {
            if (selectedItems.Count == 1) return true;
            selectedItems.BubbleSort();
            Domain.PeriodName parent = null;
            int i = 0;
            int index = -1;
            foreach (Domain.PeriodName period in selectedItems)
            {
                Domain.PeriodName newparent = period.parent;
                int newindex = newparent != null ? newparent.childrenListChangeHandler.Items.IndexOf(period) : -1;
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
        protected bool isContainsDefault(List<Domain.PeriodName> selectedItems)
        {
            if (selectedItems.Count == 1) return selectedItems[0].IsDefault;
            foreach (Domain.PeriodName period in selectedItems)
            {
                if (period.IsDefault) return true;
            }
            return false;
        }
        
        #endregion


        #region Handlers

        private void OnCustomCellAppearance(object sender, CustomCellAppearanceEventArgs e)
        {
            object row = treeList.GetRow(e.RowHandle);
            
            if (row != null && ((Domain.PeriodName)row).IsDefault)
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
                Domain.PeriodName period = (Domain.PeriodName)e.Row;
                if (!ValidateName(period, name))
                {
                    period.name = oldName;
                    e.Handled = true;
                    return;
                }
                if (!name.Equals(oldName.Trim()))
                {
                    if (period.IsDefault)
                    {
                        period.name = oldName;
                        Domain.PeriodName newPeriod = new Domain.PeriodName();
                        newPeriod.name = name;
                        newPeriod.parent = this.Root;
                        ForgetDefaultPeriods(this.Root);
                        this.Root.AddChild(newPeriod);
                        AddDefaultPeriods(this.Root);

                        int row = Source.Count;
                        if (row > 0) Source.Insert(row - 1, newPeriod);
                        else Source.Add(newPeriod);
                        SetSelectedValue(newPeriod);
                    }
                    else
                    {
                        period.name = name;
                        ForgetDefaultPeriods(period.parent);
                        period.parent.UpdateChild(period);
                        AddDefaultPeriods(period.parent);
                        SetSelectedValue(period);
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
            List<Domain.PeriodName> selectedItems = GetSelectedValues();
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
                this.deleteMenuItem.IsEnabled = slectionCount > 0 && !containsDefault;

                this.moveUpMenuItem.IsEnabled = canMoveUp(selectedItems) && isContiguousSelection && !containsDefault;
                this.moveDownMenuItem.IsEnabled = canMoveDown(selectedItems) && isContiguousSelection && !containsDefault;
                this.propertiesMenuItem.IsEnabled = true;
            }
            else this.contextMenu.Visibility = Visibility.Collapsed;
        }


        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            Domain.PeriodName parent = this.Root;            
            if (parent != null)
            {
                Kernel.Domain.PeriodName period = GetNewPeriod();
                ForgetDefaultPeriods(parent);
                parent.AddChild(period);
                AddDefaultPeriods(parent);
                
                int row = Source.Count;
                Source.Remove(period);
                if (row - 2 >= 0) Source.Insert(row - 2, period);
                else Source.Add(period);
                SetSelectedValue(period);

                if (Changed != null) Changed();
            }

        }


        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            List<Domain.PeriodName> periods = GetSelectedValues();
            if (periods.Count == 0) return;
            String message = "Do you want to delete Period: '" + periods[0] + "' ?";
            if (periods.Count > 1) message = "Do you want to delete the " + periods.Count + " selected Periods ?";
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Period", message);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Domain.PeriodName period in periods)
                {
                    if (IsUsedToGenerateUniverse(period)) return;
                    Domain.PeriodName parent = period.parent;

                    ForgetDefaultPeriods(parent);
                    if (period.oid.HasValue) parent.RemoveChild(period);
                    else parent.ForgetChild(period);
                    AddDefaultPeriods(parent);
                    removeFromSource(period);
                }
                if (Changed != null) Changed();
            }

        }

        private void removeFromSource(Domain.PeriodName period)
        {
            Source.Remove(period);
            foreach (Domain.PeriodName child in period.childrenListChangeHandler.Items)
            {
                removeFromSource(child);
            }
        }

        private void addToSource(Domain.PeriodName period)
        {            
            int row = Source.Count;
            if (row - 2 >= 0) Source.Insert(row - 2, period);
            else if (row - 1 >= 0) Source.Insert(row - 1, period);
            else Source.Add(period);
            period.childrenListChangeHandler.Items = new ObservableCollection<Domain.PeriodName>(period.childrenListChangeHandler.newItems);
            foreach (Domain.PeriodName child in period.childrenListChangeHandler.Items)
            {
                child.SetParent(period);
                child.name = GetNewPeriodName(child.name);
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
        
        private void OnMove(bool up)
        {
            List<Domain.PeriodName> periods = GetSelectedValues();
            if (periods.Count == 0) return;
            if (up) periods.BubbleSort();
            else periods.BubbleSortDesc();
            foreach (Domain.PeriodName period in periods)
            {
                Domain.PeriodName parent = period.parent;
                if (parent == null) parent = this.Root;

                ForgetDefaultPeriods(parent);
                int position = period.position + (up ? -1 : 1);
                Domain.PeriodName child = (Domain.PeriodName)parent.GetChildByPosition(position);
                if (child != null)
                {
                    child.SetPosition(period.position);
                    parent.UpdateChild(child);
                    period.SetPosition(position);
                    parent.UpdateChild(period);

                    int row = Source.IndexOf(child);
                    Source.Remove(period);
                    Source.Insert(row, period);
                }
                AddDefaultPeriods(parent);
            }
            treeList.RefreshData();
            treeList.SelectedItems = periods;
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
        private bool ValidateName(Kernel.Domain.PeriodName value, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Period name", "Name can't be empty! ");
                return false;
            }
            Domain.PeriodName found = getPeriodByName(this.Root, name);
            if (found == null || found.Equals(value)) return true;

            Kernel.Util.MessageDisplayer.DisplayError("Duplicate Period", "There is another Period named : '" + name + "'!");
            return false;
        }

        protected Domain.PeriodName getPeriodByName(Domain.PeriodName parent, string name)
        {
            foreach (Domain.PeriodName value in parent.childrenListChangeHandler.Items)
            {
                if (value.IsDefault) continue;
                if (value.name.ToUpper().Equals(name.ToUpper())) return value;
                Domain.PeriodName child = getPeriodByName(value, name);
                if (child != null) return child;
            }
            return null;
        }

        protected Domain.PeriodName GetNewPeriod()
        {
            Domain.PeriodName attribute = new Domain.PeriodName();
            attribute.name = "Period";
            if (Root != null)
            {
                Kernel.Domain.PeriodName m = null;
                int i = 1;
                do
                {
                    attribute.name = "Period" + i++;
                    m = (Domain.PeriodName)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute;
        }

        protected String GetNewPeriodName(string name)
        {
            Domain.PeriodName measure = new Domain.PeriodName();
            measure.name = name;
            if (Root != null)
            {
                Kernel.Domain.PeriodName m = (Domain.PeriodName)Root.GetChildByName(measure.name);
                int i = 1;
                while (m != null) ;
                {
                    measure.name = name + i++;
                    m = (Domain.PeriodName)Root.GetChildByName(measure.name);
                }                
            }
            return measure.name;
        }

        private bool IsUsedToGenerateUniverse(Domain.PeriodName value)
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
