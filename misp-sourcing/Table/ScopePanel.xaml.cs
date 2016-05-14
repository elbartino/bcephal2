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
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for ScopePanel.xaml
    /// </summary>
    public partial class ScopePanel : ScrollViewer, IChangeable
    {

        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des ScopeItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;
        
        #endregion


        #region Properties

        public Target Scope { get; set; }

        public ScopeItemPanel ActiveItemPanel { get; set; }

        public bool isCustomize { get; set; }
        #endregion


        #region Constructors

        public ScopePanel()
        {
            InitializeComponent();
        }

        public ScopePanel(bool isCustomize) :this()
        {
            this.isCustomize = isCustomize;
        }

        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayScope(Target scope)
        {
            this.Scope = scope;
            this.panel.Children.Clear();
            int index = 1;
            if (scope == null) 
            {
                this.ActiveItemPanel = isCustomize  ? new ScopeItemPanel(index,isCustomize) : new ScopeItemPanel(index) ;
                AddItemPanel(this.ActiveItemPanel);
                return; 
            }
            foreach(TargetItem item in scope.targetItemListChangeHandler.Items)
            {
                ScopeItemPanel itemPanel = isCustomize ?  new ScopeItemPanel(item,isCustomize) : new ScopeItemPanel(item) ;
                AddItemPanel(itemPanel);
                index++;
            }
            this.ActiveItemPanel = isCustomize ? new ScopeItemPanel(index, isCustomize) : new ScopeItemPanel(index);
            AddItemPanel(this.ActiveItemPanel);
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetTargetItemValue(Target value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (ScopeItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetValue(value);
        }

        protected void AddItemPanel(ScopeItemPanel itemPanel)
        {
            //itemPanel.Changed += OnChanged;
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            this.panel.Children.Add(itemPanel);
        }

        #endregion


        #region Handlers

        private void OnActivated(object item)
        {
            ScopeItemPanel panel = (ScopeItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            ScopeItemPanel panel = (ScopeItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.AddTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnUpdated(object item)
        {
            ScopeItemPanel panel = (ScopeItemPanel)item;
            if (this.Scope == null) this.Scope = GetNewScope();
            this.Scope.UpdateTargetItem(panel.TargetItem);
            OnChanged(panel.TargetItem);
        }

        private void OnDeleted(object item)
        {
            ScopeItemPanel panel = (ScopeItemPanel)item;
            if (panel.TargetItem != null)
            {
                if (this.Scope == null) this.Scope = GetNewScope();
                panel.TargetItem.parent = this.Scope;
                //this.Scope.RemoveTargetItem(panel.TargetItem);
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (ScopeItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach(object pan in this.panel.Children)
                {
                    ((ScopeItemPanel)pan).Index = index++;
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
                int countItems = this.Scope.targetItemListChangeHandler.Items.Count + 1;
                this.ActiveItemPanel = isCustomize ? new ScopeItemPanel(countItems, isCustomize) : new ScopeItemPanel(countItems);
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

        #endregion

    }
}
