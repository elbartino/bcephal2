using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

namespace Misp.Kernel.Administration.UserRelations
{
    /// <summary>
    /// Interaction logic for CalculatedMeasureItemPanel.xaml
    /// </summary>
    public partial class UserRelationItemPanel : Grid
    {

        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du CalculatedMeasureItem.
        /// </summary>
       // public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le CalculatedMeasureItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        /// <summary>
        /// 
        /// </summary>
        public event SelectedItemChangedEventHandler CloseParOrEqualSelected;

        #endregion

        #region Constructor

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureItemPanel
        /// </summary>
        public UserRelationItemPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }
        

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TargetItem à afficher</param>
        public UserRelationItemPanel(Relation item)
            : this()
        {
            Display(item);
        }

        public UserRelationItemPanel(int index)
            : this()
        {
            this.Index = index;
        }

        #endregion

        #region Properties

        public Label Label { get { return this.label; } }
        public Button Button { get { return this.deleteButton; } }
        public Button AddButton { get { return this.newButton; } }
        public ComboBox RoleComboBox { get { return this.roleComboBox; } }
        public ComboBox UserComboBox { get { return this.userComboBox; } }
        public Relation RelationItem { get; set; }

        private int index;


        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "R " + index;
            }
        }

      

        #endregion

        #region Operations
        public void Display(Relation item)
        {
            update = false;
            this.RelationItem = item;
            this.RoleComboBox.SelectedItem = item.role;
            this.userComboBox.SelectedItem = item.owner;
            update = true;
        }

       
       
        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        bool added = false;
        public void SetValue(Relation value)
        {
            added = false;
            if (this.RelationItem == null)
            {
                this.RelationItem = new Relation();
                added = true;
            }
            updateRelationItem();
            //added = setCalculatedMeasureItemOperator();
            //this.TextBox.Text = this.RelationItem != null ? this.RelationItem.na : "";
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
            this.AddButton.Click += OnButtonClick;
            this.AddButton.Click += OnButtonClick;
           
            this.GotFocus += OnGotFocus;
            this.RoleComboBox.GotFocus += OnGotFocus;
            this.UserComboBox.GotFocus += OnGotFocus;

            this.RoleComboBox.SelectionChanged += OnRelationChanged;
            this.UserComboBox.SelectionChanged += OnRelationChanged;
        }
        
        bool update = true;
        private void OnRelationChanged(object sender, RoutedEventArgs e)
        {
           if (Updated != null && update)
            {
                if (setRelationItem())
                        Updated(this);
            }
        }

        /// <summary>
        /// validate selection and set selected operator
        /// </summary>
        /// <returns></returns>
        private bool setRelationItem()
        {
            added = false;
            if (this.RelationItem == null)
            {
                this.RelationItem = new Relation();
                added = true;
            }

            this.RelationItem.role = this.roleComboBox.SelectedItem as Domain.Role;
            this.RelationItem.owner = this.userComboBox.SelectedItem as Domain.User;

            bool add = this.added == true ? true : false;
            if (Added != null && added) Added(this);
            updateRelationItem();
            
            if (Updated != null && !added) Updated(this);
            return add;

        }

        private void updateRelationItem()
        {
            if (this.RoleComboBox.SelectedItem != null && this.RoleComboBox.SelectedItem.ToString().Equals("("))
            {
            }
            else { }
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null)
            {
                Activated(this);
            }
        }


        private void OnTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OnValidateEdition();
        }

        private void OnTextLostFocus(object sender, RoutedEventArgs e)
        {
            OnValidateEdition();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValidateEdition()
        {
            String text = "";//this.TextBox.Text;
            decimal amount;
            bool valid = decimal.TryParse(text, out amount);
            if (!valid) return;

            Relation value = new Relation();
            //value.measureType = CalculatedMeasureItem.MeasureType.AMOUNT.ToString();
            //value.amount = amount;
            //SetValue(value);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            if ((Button)sender == this.Button) { if (Deleted != null) Deleted(this); }
            if ((Button)sender == this.newButton) { if (Added != null) Added(this); }
            e.Handled = true;
        }


        #endregion



        public void changeBorder()
        {
           //border.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 222, 222));
        }

        public void FillUsers(List<Domain.User> list)
        {
            var strings = (from o in list
                           select o.ToString()).ToList();
            this.userComboBox.ItemsSource = strings;
        }
           

        public void FillRoles(List<Domain.Role> list)
        {
            var strings = (from o in list
                           select o.ToString()).ToList();
            this.roleComboBox.ItemsSource = strings;
        }

      
    }
}
