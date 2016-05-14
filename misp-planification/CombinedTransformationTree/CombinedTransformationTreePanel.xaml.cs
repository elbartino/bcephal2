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
using Misp.Planification.CombinedTransformationTree;
using Misp.Kernel.Util;

namespace Misp.Planification.CombinedTransformationTree
{
    /// <summary>
    /// Interaction logic for ScopePanel.xaml
    /// </summary>
    public partial class CombinedTransformationTreePanel : ScrollViewer, IChangeable
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

        public Kernel.Domain.CombinedTransformationTree CombinedTransformationTree { get; set; }

        public TransformationTreeItemPanel ActiveItemPanel { get; set; }

        #endregion


        #region Constructors

        public CombinedTransformationTreePanel()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayTransformationTrees(Kernel.Domain.CombinedTransformationTree combinedTransformationTree)
        {
            this.CombinedTransformationTree = combinedTransformationTree;
            this.panel.Children.Clear();
            int index = 1;
            if (combinedTransformationTree == null) 
            {
                this.ActiveItemPanel = new TransformationTreeItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return; 
            }
            combinedTransformationTree.treeItemListChangeHandler.Items.BubbleSort();
            foreach (Kernel.Domain.CombinedTransformationTreeItem item in combinedTransformationTree.treeItemListChangeHandler.Items)
            {
                TransformationTreeItemPanel itemPanel = new TransformationTreeItemPanel(item);
                AddItemPanel(itemPanel);
                index++;
            }
            this.ActiveItemPanel = new TransformationTreeItemPanel(index);
            AddItemPanel(this.ActiveItemPanel);
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void setTransformationTree(Kernel.Domain.TransformationTree tree)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (TransformationTreeItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetValue(tree);
        }

        protected void AddItemPanel(TransformationTreeItemPanel itemPanel)
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
            TransformationTreeItemPanel panel = (TransformationTreeItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            TransformationTreeItemPanel panel = (TransformationTreeItemPanel)item;
            if (this.CombinedTransformationTree == null) this.CombinedTransformationTree = new Kernel.Domain.CombinedTransformationTree();
            this.CombinedTransformationTree.AddTreeItem(panel.CombinedTransformationTreeItem);
            OnChanged(panel.CombinedTransformationTreeItem);
        }

        private void OnUpdated(object item)
        {
            TransformationTreeItemPanel panel = (TransformationTreeItemPanel)item;
            if (this.CombinedTransformationTree == null) this.CombinedTransformationTree = new Kernel.Domain.CombinedTransformationTree();
            panel.CombinedTransformationTreeItem.position = panel.Index - 1;
            this.CombinedTransformationTree.UpdateTreeItem(panel.CombinedTransformationTreeItem);
            OnChanged(panel.CombinedTransformationTreeItem);
        }

        private void OnDeleted(object item)
        {
            TransformationTreeItemPanel panel = (TransformationTreeItemPanel)item;
            this.panel.Children.Remove(panel);
            if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel && this.panel.Children.Count > 0 )
                this.ActiveItemPanel = (TransformationTreeItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            
            int index = 1;
            foreach(object pan in this.panel.Children)
            {
                ((TransformationTreeItemPanel)pan).Index = index++;
            }
            if (ItemDeleted != null && panel.CombinedTransformationTreeItem != null) ItemDeleted(panel.CombinedTransformationTreeItem);
            if (Changed != null) Changed();
        }
        
        private void OnChanged(object item)
        {
            if (this.CombinedTransformationTree == null) this.CombinedTransformationTree = new Kernel.Domain.CombinedTransformationTree();
            if (this.panel.Children.Count <= this.CombinedTransformationTree.treeItemListChangeHandler.Items.Count)
            {
                this.ActiveItemPanel = new TransformationTreeItemPanel(this.CombinedTransformationTree.treeItemListChangeHandler.Items.Count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }
        
        #endregion

    }
}
