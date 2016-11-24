using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
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
    /// Interaction logic for RelationshipItemPanel.xaml
    /// </summary>
    public partial class RelationshipItemPanel : Grid
    {
        
        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du TargetItem.
        /// </summary>
        public event ActionEventHandler Changed;

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
        
        #endregion


        #region Properties

        public GrilleRelationshipItem RelationshipItem { get; set; }

        public int Index { get; set; }

        public bool IsPrimary { get; set; }

        protected bool throwEvents;

        #endregion


        #region Constructors

        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        public RelationshipItemPanel()
        {
            InitializeComponent();
            InitializeHandlers();
            throwEvents = true;
        }

        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        public RelationshipItemPanel(bool isPrimary) : this()
        {
            IsPrimary = isPrimary;
            CustomizeComponents();
        }

        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="index">Panel index</param>
        public RelationshipItemPanel(Grille grid, bool isPrimary = false)
            : this(isPrimary)
        {
            throwEvents = false;
            if (isPrimary) this.comboBox.ItemsSource = grid.PrimaryColumnsDataSource;
            else this.comboBox.ItemsSource = grid.RelatedColumnsDataSource;
            throwEvents = true;
        }
        
        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="index">Panel index</param>
        public RelationshipItemPanel(Grille grid, int index, bool isPrimary = false)
            : this(grid, isPrimary)
        {
            this.Index = index;
        }
        
        /// <summary>
        /// Build a new instance of RelationshipItemPanel
        /// </summary>
        /// <param name="item">RelationshipItem to display in this panel</param>
        public RelationshipItemPanel(Grille grid, GrilleRelationshipItem item, bool isPrimary = false)
            : this(grid, isPrimary)
        {
            Display(item); 
        }
        
        #endregion
        

        #region Operations

        public void Display(GrilleRelationshipItem item)
        {
            throwEvents = false;
            this.RelationshipItem = item;
            if (item != null) this.Index = item.position + 1;
            this.comboBox.SelectedItem = item != null && item.column != null ? item.column.name : "";
            this.checkBox.IsChecked = item != null ? item.exclusive : false;
            this.checkBox.IsEnabled = this.comboBox.SelectedItem != null;
            throwEvents = true;
        }
        
        public GrilleRelationshipItem Fill()
        {
            GrilleColumn column = (GrilleColumn) this.comboBox.SelectedItem;
            if (this.RelationshipItem == null) this.RelationshipItem = new GrilleRelationshipItem();
            this.RelationshipItem.position = Index;
            this.RelationshipItem.primary = IsPrimary;
            this.RelationshipItem.exclusive = this.checkBox.IsChecked.Value;
            this.RelationshipItem.column = column;
            return this.RelationshipItem;
        }

        public GrilleColumn SelectedColumn()
        {
            return (GrilleColumn) this.comboBox.SelectedItem;
        }

        private void CustomizeComponents()
        {
            checkBox.Visibility = IsPrimary ? Visibility.Collapsed : Visibility.Visible;
            addButton.Visibility = !IsPrimary ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.deleteButton.Click += OnButtonClick;
            this.comboBox.SelectionChanged += OnComboBoxSelectionChanged;
            this.checkBox.Checked += OnCheckboxChecked;
            this.checkBox.Unchecked += OnCheckboxChecked;
        }
           

        private void OnComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (Changed != null && throwEvents)
            {
                if (!Changed(this))
                {
                    throwEvents = false;
                    this.comboBox.SelectedItem = this.RelationshipItem != null ? this.RelationshipItem.column : null;
                    throwEvents = true;
                }
            }
            throwEvents = false;
            this.checkBox.IsEnabled = this.comboBox.SelectedItem != null;
            throwEvents = true;
        }
        
        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            if (Changed != null && throwEvents)
            {
                if (!Changed(this))
                {
                    throwEvents = false;
                    this.checkBox.IsChecked = this.RelationshipItem != null ? this.RelationshipItem.exclusive : false;
                    throwEvents = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null) Deleted(this);
        }

        
        #endregion


    }
}
