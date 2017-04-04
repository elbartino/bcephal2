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

namespace Misp.Sourcing.FilterScope
{
    /// <summary>
    /// Interaction logic for RTargetPanel.xaml
    /// </summary>
    public partial class RTargetPanel : UserControl
    {
        #region Events
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        public event ValidateFormulaEventHandler ValidateFormula;
        #endregion

        #region Properties
        public Target Scope { get; set; }

        public RTargetItemPanel ActiveItemPanel { get; set; }

        public bool IsReadOnly { get; set; }

        #endregion

        public RTargetPanel()
        {
            InitializeComponent();
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayScope(Target scope, bool isNoAllocation = false, bool readOnly = false)
        {
            this.panel.Children.Clear();
            if (scope != null)
            DisplayScope(scope, isNoAllocation);
            
            isNoAllocation = scope != null ? isNoAllocation : false;
            AddDefaultItem(isNoAllocation);
        }



        private void DisplayScope(Target scope, bool isNoAllocation = false)
        {
            this.Scope = scope;
            foreach (TargetItem item in scope.targetItemListChangeHandler.Items)
            {
                RTargetItemPanel itemPanel = new RTargetItemPanel(item, isNoAllocation);
                //if (this.IsReadOnly) itemPanel.SetReadOnly(this.IsReadOnly);
                AddItemPanel(itemPanel);
            }
        }

        private void AddDefaultItem(bool isNoAllocation = false)
        {
            if (this.IsReadOnly) return;
            int index = this.panel.Children.Count + 1;
            this.ActiveItemPanel = new RTargetItemPanel(index, isNoAllocation);
            //this.ActiveItemPanel.SetReadOnly(readOnly);
            AddItemPanel(this.ActiveItemPanel);
        }

        public void SetTargetValue(Target value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (RTargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetTarget(value);
        }

        public void SetLoopValue(TransformationTreeItem loop)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (RTargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetLoop(loop);
        }

        protected void AddItemPanel(RTargetItemPanel itemPanel)
        {
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.ValidateFormula += OnValidateFormula;
            //itemPanel.Expand(IsExpanded);
            this.panel.Children.Add(itemPanel);
        }

        private void OnActivated(object item)
        {
            RTargetItemPanel panel = (RTargetItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            RTargetItemPanel panel = (RTargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.AddTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnUpdated(object item)
        {
            RTargetItemPanel panel = (RTargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.UpdateTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnDeleted(object item)
        {
            RTargetItemPanel panel = (RTargetItemPanel)item;
            if (panel.TargetItem != null)
            {
                if (this.Scope == null) this.Scope = GetNewScope();
                panel.TargetItem.parent = this.Scope;
                //this.Scope.RemoveTargetItem(panel.TargetItem);
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (RTargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach (object pan in this.panel.Children)
                {
                    ((RTargetItemPanel)pan).Index = index++;
                }
                if (Changed != null) Changed();
                if (ItemDeleted != null && panel.TargetItem != null) ItemDeleted(panel.TargetItem);
            }
        }

        private void OnValidateFormula(object item)
        {
            RTargetItemPanel panel = (RTargetItemPanel)item;
            OnChanged(panel.TargetItem);
        }

        private void OnChanged(object item)
        {
            if (this.Scope == null) this.Scope = GetNewScope();
            if (this.panel.Children.Count <= this.Scope.targetItemListChangeHandler.Items.Count)
            {
                this.ActiveItemPanel = new RTargetItemPanel(this.Scope.targetItemListChangeHandler.Items.Count + 1);
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

    }
}
