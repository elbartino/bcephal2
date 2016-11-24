using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public GrilleRelationship Relationship 
        { 
            get { return this.Grid.relationship; }
            set { this.Grid.relationship = value; }
        }

        public RelationshipItemPanel ActiveItemPanel { get; set; }

        public Grille Grid { get; set; }

        public bool IsPrimary { get; set; }

        #endregion


        #region Constructors

        public RelationshipPanel()
        {
            InitializeComponent();
        }

        public RelationshipPanel(bool isPrimary) : this()
        {
            IsPrimary = isPrimary;
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
            this.panel.Children.Clear();
            int index = 1;
            if (this.Relationship == null) 
            {
                this.ActiveItemPanel = new RelationshipItemPanel(grid, index, IsPrimary);
                AddItemPanel(this.ActiveItemPanel);
                return; 
            }
            foreach (GrilleRelationshipItem item in this.Relationship.itemListChangeHandler.Items)
            {
                if (IsPrimary == item.primary)
                {
                    RelationshipItemPanel itemPanel = new RelationshipItemPanel(grid, item, IsPrimary);
                    AddItemPanel(itemPanel);
                    index++;
                }
            }
            this.ActiveItemPanel = new RelationshipItemPanel(grid, index, IsPrimary);
            AddItemPanel(this.ActiveItemPanel);
        }

        protected void AddItemPanel(RelationshipItemPanel itemPanel)
        {
            itemPanel.Changed += OnItemChanged;
            itemPanel.Deleted += OnDeleted;
            this.panel.Children.Add(itemPanel);
        }
        
        #endregion


        #region Handlers


        private bool OnItemChanged(object item)
        {
            RelationshipItemPanel panel = (RelationshipItemPanel)item;            
            if (this.Relationship == null) this.Relationship = GetNewRelationship();
            if (ValidateSelection(panel))
            {
                bool isNew = panel.RelationshipItem == null;
                panel.Fill();
                if (isNew) this.Relationship.AddItem(panel.RelationshipItem);
                else this.Relationship.UpdateItem(panel.RelationshipItem);
                OnChanged(panel.RelationshipItem, isNew);

                if (panel.RelationshipItem.primary) this.Grid.RelatedColumnsDataSource.Remove(panel.RelationshipItem.column);
                else this.Grid.PrimaryColumnsDataSource.Remove(panel.RelationshipItem.column);

                return true;
            }
            return false;
        }

        private bool ValidateSelection(RelationshipItemPanel panel)
        {
            GrilleColumn column = panel.SelectedColumn();
            GrilleRelationshipItem item = this.Relationship.GetItemByColumn(column);
            if (item == null || item == panel.RelationshipItem) return true;
            if (item != null)
            {
                String title = "";
                String message = "";
                if (panel.RelationshipItem == null)
                {
                    if (item.primary) title = "Wrong selection";
                }
                MessageDisplayer.DisplayWarning(title, message);
                return false;
            }
            return true;
        }
        
        private void OnDeleted(object item)
        {
            RelationshipItemPanel panel = (RelationshipItemPanel)item;
            if (panel.RelationshipItem != null)
            {
                if (this.Relationship == null) this.Relationship = GetNewRelationship();
                this.Relationship.RemoveItem(panel.RelationshipItem);
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

                if (panel.RelationshipItem.primary)
                {
                    this.Grid.RelatedColumnsDataSource.Add(panel.RelationshipItem.column);
                }
                else
                {
                    this.Grid.PrimaryColumnsDataSource.Add(panel.RelationshipItem.column);
                }
            }
        }
        
        private void OnChanged(object item, bool isNew)
        {
            if (this.Relationship == null) this.Relationship = GetNewRelationship();
            if (isNew && this.panel.Children.Count <= this.Relationship.itemListChangeHandler.Items.Count)
            {
                int countItems = this.Relationship.itemListChangeHandler.Items.Count + 1;
                this.ActiveItemPanel = new RelationshipItemPanel(this.Grid, countItems, IsPrimary);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }


        private GrilleRelationship GetNewRelationship()
        {
            GrilleRelationship Relationship = new GrilleRelationship();
            Relationship.Grid = Grid;
            return Relationship;
        }

        #endregion

    }
}
