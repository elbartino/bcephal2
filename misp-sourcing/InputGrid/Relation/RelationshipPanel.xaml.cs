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

namespace Misp.Sourcing.InputGrid.Relation
{
    /// <summary>
    /// Interaction logic for RelationshipPanel.xaml
    /// </summary>
    public partial class RelationshipPanel : ScrollViewer
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

        public GrilleRelationship Relationship { get; set; }

        public RelationshipItemPanel ActiveItemPanel { get; set; }

        public Grille Grid { get; set; }

        #endregion


        #region Constructors

        public RelationshipPanel()
        {
            InitializeComponent();
        }
        
        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationship"></param>
        public void Display(Grille grid, GrilleRelationship relationship)
        {
            this.Grid = grid;
            this.Relationship = relationship;
            this.panel.Children.Clear();
            int index = 1;
            if (relationship == null) 
            {
                this.ActiveItemPanel = new RelationshipItemPanel(grid, index);
                AddItemPanel(this.ActiveItemPanel);
                return; 
            }
            foreach (GrilleRelationshipItem item in relationship.itemListChangeHandler.Items)
            {
                RelationshipItemPanel itemPanel = new RelationshipItemPanel(grid, item);
                AddItemPanel(itemPanel);
                index++;
            }
            this.ActiveItemPanel = new RelationshipItemPanel(grid, index);
            AddItemPanel(this.ActiveItemPanel);
        }

        protected void AddItemPanel(RelationshipItemPanel itemPanel)
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
            RelationshipItemPanel panel = (RelationshipItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            RelationshipItemPanel panel = (RelationshipItemPanel)item;
            if (this.Relationship == null) this.Relationship = GetNewRelationship();
            //this.Relationship.AddTargetItem(panel.RelationshipItem);
            OnChanged(panel.RelationshipItem);
        }

        private void OnUpdated(object item)
        {
            RelationshipItemPanel panel = (RelationshipItemPanel)item;
            if (this.Relationship == null) this.Relationship = GetNewRelationship();
            //this.Relationship.UpdateTargetItem(panel.RelationshipItem);
            OnChanged(panel.RelationshipItem);
        }

        private void OnDeleted(object item)
        {
            RelationshipItemPanel panel = (RelationshipItemPanel)item;
            if (panel.RelationshipItem != null)
            {
                if (this.Relationship == null) this.Relationship = GetNewRelationship();
                //panel.RelationshipItem.parent = this.Relationship;
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (RelationshipItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach(object pan in this.panel.Children)
                {
                    ((RelationshipItemPanel)pan).Index = index++;
                }
                if (Changed != null) Changed();
                if (ItemDeleted != null && panel.RelationshipItem != null) ItemDeleted(panel.RelationshipItem);
            }
        }
        
        private void OnChanged(object item)
        {
            if (this.Relationship == null) this.Relationship = GetNewRelationship();
            if (this.panel.Children.Count <= this.Relationship.itemListChangeHandler.Items.Count)
            {
                int countItems = this.Relationship.itemListChangeHandler.Items.Count + 1;
                this.ActiveItemPanel = new RelationshipItemPanel(this.Grid, countItems);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        private GrilleRelationship GetNewRelationship()
        {
            GrilleRelationship Relationship = new GrilleRelationship();
            return Relationship;
        }

        #endregion

    }
}
