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
    /// Interaction logic for PrimaryRelationPanel.xaml
    /// </summary>
    public partial class PrimaryRelationPanel : ScrollViewer
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

        public GrilleRelationships Relationships { get; set; }

        public PrimaryRelationItemPanel ActiveItemPanel { get; set; }

        public RelationshipPanel RelationshipPanel { get; set; }

        public Grille Grid { get; set; }

        #endregion


        #region Constructors

        public PrimaryRelationPanel()
        {
            InitializeComponent();
        }
        
        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationship"></param>
        public void Display(Grille grid)
        {
            this.Grid = grid;
            this.Relationships = this.Grid != null ? this.Grid.relationships : null;
            this.panel.Children.Clear();
            int index = 1;
            if (this.Relationships == null) 
            {
                this.ActiveItemPanel = new PrimaryRelationItemPanel(grid, index);
                AddItemPanel(this.ActiveItemPanel);
                return; 
            }
            foreach (GrilleRelationship item in this.Relationships.relationshipListChangeHandler.Items)
            {
                PrimaryRelationItemPanel itemPanel = new PrimaryRelationItemPanel(grid, item);
                AddItemPanel(itemPanel);
                index++;
            }
            this.ActiveItemPanel = new PrimaryRelationItemPanel(grid, index);
            AddItemPanel(this.ActiveItemPanel);
        }

        protected void AddItemPanel(PrimaryRelationItemPanel itemPanel)
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
            PrimaryRelationItemPanel panel = (PrimaryRelationItemPanel)item;
            this.ActiveItemPanel = panel;
            this.RelationshipPanel.Display(panel.Relationship);
        }

        private void OnAdded(object item)
        {
            PrimaryRelationItemPanel panel = (PrimaryRelationItemPanel)item;
            if (this.Relationships == null) this.Relationships = GetNewRelationship();
            //this.Relationship.AddTargetItem(panel.RelationshipItem);
            OnChanged(panel.Relationship);
        }

        private void OnUpdated(object item)
        {
            PrimaryRelationItemPanel panel = (PrimaryRelationItemPanel)item;
            if (this.Relationships == null) this.Relationships = GetNewRelationship();
            //this.Relationship.UpdateTargetItem(panel.RelationshipItem);
            OnChanged(panel.Relationship);
        }

        private void OnDeleted(object item)
        {
            PrimaryRelationItemPanel panel = (PrimaryRelationItemPanel)item;
            if (panel.Relationship != null)
            {
                if (this.Relationships == null) this.Relationships = GetNewRelationship();
                //panel.RelationshipItem.parent = this.Relationship;
                this.panel.Children.Remove(panel);
                if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                    this.ActiveItemPanel = (PrimaryRelationItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                int index = 1;
                foreach(object pan in this.panel.Children)
                {
                    ((PrimaryRelationItemPanel)pan).Index = index++;
                }
                if (Changed != null) Changed();
                if (ItemDeleted != null && panel.Relationship != null) ItemDeleted(panel.Relationship);
            }
        }
        
        private void OnChanged(object item)
        {
            if (this.Relationships == null) this.Relationships = GetNewRelationship();
            if (this.panel.Children.Count <= this.Relationships.relationshipListChangeHandler.Items.Count)
            {
                int countItems = this.Relationships.relationshipListChangeHandler.Items.Count + 1;
                this.ActiveItemPanel = new PrimaryRelationItemPanel(Grid, countItems);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        private GrilleRelationships GetNewRelationship()
        {
            GrilleRelationships Relationships = new GrilleRelationships();
            return Relationships;
        }

        #endregion

    }
}
