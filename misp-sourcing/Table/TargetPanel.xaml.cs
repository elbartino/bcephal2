using Misp.Kernel.Domain;
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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for TargetPanel.xaml
    /// </summary>
    public partial class TargetPanel : UserControl
    {
        public TargetPanel()
        {
            InitializeComponent();
            IsExpanded = true;
            Expand(false);
        }

        public bool IsExpanded { get; set; }
        

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des ScopeItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        public event ValidateFormulaEventHandler ValidateFormula;

        public Target Scope { get; set; }

        //public ScopeItemPanel ActiveItemPanel { get; set; }
        public TargetItemPanel ActiveItemPanel { get; set; }

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayScope(Target scope,bool isNoAllocation=false,bool readOnly = false)
        {
            this.Scope = scope;
            this.panel.Children.Clear();
            int index = 1;
            if (scope == null)
            {
                if (!this.IsReadOnly) 
                {
                this.ActiveItemPanel = new TargetItemPanel(index);
                this.ActiveItemPanel.SetReadOnly(readOnly);
                //this.ActiveItemPanel = new ScopeItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                }
                return;
            }
            foreach (TargetItem item in scope.targetItemListChangeHandler.Items)
            {
                TargetItemPanel itemPanel = new TargetItemPanel(item,isNoAllocation);
                if(this.IsReadOnly) itemPanel.SetReadOnly(this.IsReadOnly);
                AddItemPanel(itemPanel);
                index++;
            }

            if (!this.IsReadOnly)
            {
                this.ActiveItemPanel = new TargetItemPanel(index, isNoAllocation);
                this.ActiveItemPanel.SetReadOnly(readOnly);
                AddItemPanel(this.ActiveItemPanel);
            }
        }

        public void SetTargetValue(Target value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (TargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetTarget(value);
        }

        public void SetLoopValue(TransformationTreeItem loop)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (TargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetLoop(loop);
        }

        protected void AddItemPanel(TargetItemPanel itemPanel)
        {
            //itemPanel.Changed += OnChanged;
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.ValidateFormula += OnValidateFormula;
            itemPanel.Expand(IsExpanded);
            this.panel.Children.Add(itemPanel);
        }

        private void OnValidateFormula(object item)
        {
            TargetItemPanel panel = (TargetItemPanel)item;
            OnChanged(panel.TargetItem);
        }


        

        private void OnActivated(object item)
        {
            TargetItemPanel panel = (TargetItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            TargetItemPanel panel = (TargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.AddTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnUpdated(object item)
        {
            TargetItemPanel panel = (TargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.UpdateTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnDeleted(object item)
        {
            TargetItemPanel panel = (TargetItemPanel)item;
            if (panel.TargetItem != null)
            {
                if (this.Scope == null) this.Scope = GetNewScope();
                panel.TargetItem.parent = this.Scope;
                //this.Scope.RemoveTargetItem(panel.TargetItem);
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (TargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach (object pan in this.panel.Children)
                {
                    ((TargetItemPanel)pan).Index = index++;
                }
                if (Changed != null) Changed();
                if (ItemDeleted != null && panel.TargetItem != null) ItemDeleted(panel.TargetItem);
            }
        }

        private void OnChanged(object item)
        {
            if (this.Scope == null) this.Scope = GetNewScope();
            if (this.panel.Children.Count <= this.Scope.targetItemListChangeHandler.Items.Count)
            {
                this.ActiveItemPanel = new TargetItemPanel(this.Scope.targetItemListChangeHandler.Items.Count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        private Target GetNewScope()
        {
            Target scope = new Target();
            scope.targetType = Target.TargetType.COMBINED.ToString();
            scope.type = Target.Type.OBJECT_VC.ToString();
            return scope;
        }


        public void Expand(bool expand)
        {
            if (IsExpanded == expand) return;
            IsExpanded = expand;
            foreach(UIElement child in this.panel.Children){
                if (child is TargetItemPanel) ((TargetItemPanel)child).Expand(expand);
            }
            if (expand)
            {
                hearderRow.Height = new GridLength(20);
            }
            else
            {
                hearderRow.Height = new GridLength(0);
            }
        }

        public void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.hearderGrid.Visibility = !readOnly ? Visibility.Visible : Visibility.Collapsed;
            if (this.panel.Children.Count > 0) 
            {
                foreach (UIElement item in this.panel.Children)
                {
                    if (item is TargetItemPanel) 
                    {
                        ((TargetItemPanel)item).SetReadOnly(readOnly);
                    }
                }
            }
        }
        

    }
}
