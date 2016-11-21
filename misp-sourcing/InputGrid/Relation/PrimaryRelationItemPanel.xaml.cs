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
    /// Interaction logic for PrimaryRelationItemPanel.xaml
    /// </summary>
    public partial class PrimaryRelationItemPanel : Grid
    {
        
        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du TargetItem.
        /// </summary>
        public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le TargetItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        #endregion


        #region Properties

        public GrilleRelationship Relationship { get; set; }

        public int Index { get; set; }

        public Grille Grid { get; set; }

        protected bool throwEvents;

        #endregion


        #region Constructors

        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        public PrimaryRelationItemPanel()
        {
            InitializeComponent();            
            InitializeHandlers();
            throwEvents = true;
        }

        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="index">Panel index</param>
        public PrimaryRelationItemPanel(Grille grid)
            : this()
        {
            this.Grid = grid;
            throwEvents = false;
            this.comboBox.ItemsSource = this.Grid.columnListChangeHandler.Items;
            throwEvents = true;
        }
        
        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="index">Panel index</param>
        public PrimaryRelationItemPanel(Grille grid, int index)
            : this(grid)
        {
            this.Index = index;
        }
        
        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="item">RelationshipItem to display in this panel</param>
        public PrimaryRelationItemPanel(Grille grid, GrilleRelationship item)
            : this(grid)
        {
            Display(item); 
        }
        
        #endregion
        

        #region Operations

        public void Display(GrilleRelationship item)
        {
            throwEvents = false;
            this.Relationship = item;
            if (item != null) this.Index = item.position + 1;
            this.comboBox.SelectedItem = item != null && item.primaryColumn != null ? item.primaryColumn.name : "";
            throwEvents = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetValue(Target value)
        {
            //bool added = false;
            //if (this.TargetItem == null) 
            //{ 
            //    this.TargetItem = new TargetItem(Index - 1);
            //    added = true; 
            //}

            //this.TargetItem.value = value;
            //this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();
            //this.TextBox.Text = value != null ? value.name : "";
            //if (Added != null && added) Added(this);

            //if (Updated != null && !added) Updated(this);

            //if (Changed != null) Changed();
        }


        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.addButton.Click += OnAddButtonClick;
            this.deleteButton.Click += OnDeleteButtonClick;
            this.GotFocus += OnGotFocus;
            this.addButton.GotFocus += OnGotFocus;
            this.deleteButton.GotFocus += OnGotFocus;
            this.comboBox.GotFocus += OnGotFocus;
            this.comboBox.SelectionChanged += OnComboBoxSelectionChanged;
        }
        
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }


        private void OnComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (Updated != null && throwEvents)
            {
                if (this.Relationship != null)
                {
                    //this.RelationshipItem.column = this.comboBox.SelectedItem.ToString();
                    Updated(this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null) Deleted(this);
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (Added != null) Added(this);
        }
        
        #endregion

    }
}
