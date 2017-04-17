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

namespace Misp.Sourcing.GridViews
{
    /// <summary>
    /// Interaction logic for GrilleFilterTargetPanel.xaml
    /// </summary>
    public partial class GrilleFilterTargetPanel : ScrollViewer
    {

        #region Events

        public event ChangeEventHandler Changed;
        
        #endregion


        #region Properties

        public Target Scope { get; set; }

        public GrilleFilterTargetItemPanel ActiveItemPanel { get; set; }

        public bool IsReadOnly { get; set; }
        
        bool throwHandlers;
        
        #endregion


        #region Constructors

        public GrilleFilterTargetPanel()
        {
            InitializeComponent();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void DisplayScope(Target scope)
        {
            this.Scope = scope;
            this.panel.Children.Clear();
            if (scope != null)
            {
                foreach (TargetItem item in scope.targetItemListChangeHandler.Items)
                {
                    GrilleFilterTargetItemPanel itemPanel = new GrilleFilterTargetItemPanel(item);
                    AddItemPanel(itemPanel);
                }
            }
            AddDefaultItem();
        }

        private void AddDefaultItem()
        {
            if (this.IsReadOnly) return;
            int index = this.panel.Children.Count + 1;
            this.ActiveItemPanel = new GrilleFilterTargetItemPanel(index);
            AddItemPanel(this.ActiveItemPanel);
        }

        public void SetTargetValue(Target value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (GrilleFilterTargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetTarget(value);
        }
        
        protected void AddItemPanel(GrilleFilterTargetItemPanel itemPanel)
        {
            InitializeHandlers(itemPanel);
            this.panel.Children.Add(itemPanel);
        }

        


        private Target GetNewScope()
        {
            Target scope = new Target();
            scope.targetType = Target.TargetType.COMBINED.ToString();
            scope.type = Target.Type.OBJECT_VC.ToString();
            return scope;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers(GrilleFilterTargetItemPanel itemPanel)
        {
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
        }

        private void RemoveHandlers(GrilleFilterTargetItemPanel itemPanel)
        {
            itemPanel.Added -= OnAdded;
            itemPanel.Updated -= OnUpdated;
            itemPanel.Deleted -= OnDeleted;
            itemPanel.Activated -= OnActivated;
        }

        private void OnActivated(object item)
        {
            this.ActiveItemPanel = (GrilleFilterTargetItemPanel)item;
        }

        private void OnAdded(object item)
        {
            GrilleFilterTargetItemPanel panel = (GrilleFilterTargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.AddTargetItem(panel.TargetItem);
            OnChanged();
        }

        private void OnUpdated(object item)
        {
            GrilleFilterTargetItemPanel panel = (GrilleFilterTargetItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.UpdateTargetItem(panel.TargetItem);
            OnChanged();
        }

        private void OnDeleted(object item)
        {
            GrilleFilterTargetItemPanel panel = (GrilleFilterTargetItemPanel)item;
            if (panel.TargetItem != null)
            {
                if (this.Scope == null) this.Scope = GetNewScope();
                panel.TargetItem.parent = this.Scope;
                this.Scope.RemoveTargetItem(panel.TargetItem);
                RemoveHandlers(panel);
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (GrilleFilterTargetItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach (object pan in this.panel.Children)
                {
                    ((GrilleFilterTargetItemPanel)pan).Index = index++;
                } 
                OnChanged();
            }
            
        }

        private void OnChanged()
        {
            if (this.panel.Children.Count <= this.Scope.targetItemListChangeHandler.Items.Count)
            {
                this.ActiveItemPanel = new GrilleFilterTargetItemPanel(this.Scope.targetItemListChangeHandler.Items.Count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
        }

        #endregion


    }
}
