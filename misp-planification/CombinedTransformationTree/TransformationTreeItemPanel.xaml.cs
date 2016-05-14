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

namespace Misp.Planification.CombinedTransformationTree
{
    /// <summary>
    /// Interaction logic for ScopeItemPanel.xaml
    /// </summary>
    public partial class TransformationTreeItemPanel : Grid
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


        #region Constructor

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        public TransformationTreeItemPanel()
        {
            InitializeComponent();
            this.ComboBox.ItemsSource = new String[] { TargetItem.Operator.AND.ToString(), 
                TargetItem.Operator.NOT.ToString(), TargetItem.Operator.OR.ToString() };
            this.ComboBox.SelectedItem = TargetItem.Operator.AND.ToString();
            this.ComboBox.Visibility = System.Windows.Visibility.Collapsed;
            InitializeHandlers();
        }

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public TransformationTreeItemPanel(int index) : this()
        {
            this.Index = index;            
        }

        /// <summary>
        /// Construit une nouvelle instance de ScopeItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TargetItem à afficher</param>
        public TransformationTreeItemPanel(CombinedTransformationTreeItem tree) : this()
        {
            Display(tree); 
        }

        #endregion


        #region Properties

        private int index;

        public CombinedTransformationTreeItem CombinedTransformationTreeItem { get; set; }

        public int Index 
        { 
            get { return index; } 
            set 
            { 
                index = value; 
                this.Label.Content = "Position " + index;
                this.ComboBox.IsEnabled = index > 1;
            } 
        }

        public Label Label { get {return this.label;} }
        public Button Button { get { return this.button; } }
        public ComboBox ComboBox { get { return this.comboBox; } }
        public TextBox TextBox { get { return this.textBox; } }

        #endregion


        #region Operations

        public void Display(CombinedTransformationTreeItem item)
        {
            update = false;
            this.CombinedTransformationTreeItem = item;
            if (item != null) this.Index = item.position + 1;
            this.TextBox.Text = item == null ? "" : item.tree != null && item.tree.oid.HasValue ? item.tree.name : "TREE NOT FOUND";
            update = true;
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetValue(TransformationTree tree)
        {
            bool added = false;
            if (this.CombinedTransformationTreeItem == null) 
            {
                this.CombinedTransformationTreeItem = new CombinedTransformationTreeItem(Index - 1);
                added = true; 
            }
            
            int position = this.CombinedTransformationTreeItem.position;
            this.CombinedTransformationTreeItem.tree = tree;
            this.CombinedTransformationTreeItem.position = position;
            String name = tree.name;
            this.TextBox.Text = tree != null ? tree.name : "";
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }


        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.Button.Click += OnButtonClick;
            this.Button.GotFocus += OnGotFocus;
            this.TextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;
            this.ComboBox.SelectionChanged += OnOperatorChanged;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        bool update = true;
        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
            if (Updated != null && update)
            {
                if (this.CombinedTransformationTreeItem != null)
                {
                   // this.TargetItem.operatorType = this.ComboBox.SelectedItem.ToString();
                    Updated(this);
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
