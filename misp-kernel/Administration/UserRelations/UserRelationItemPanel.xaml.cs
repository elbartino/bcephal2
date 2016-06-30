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
            this.roleComboBox.ItemsSource = new String[] {"","("};
           // this.comboBox2.ItemsSource = new String[] { "+", "-", "^", "*", "/" };
            this.userComboBox.ItemsSource = new String[] {"",")"};
            
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
        public ComboBox RoleComboBox { get { return this.roleComboBox; } }
        //public ComboBox SignComboBox { get { return this.comboBox2; } }
        public ComboBox UserComboBox { get { return this.userComboBox; } }
        //public TextBox TextBox { get { return this.measureItem; } }
        public Relation RelationItem { get; set; }

        private int index;


        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "R " + index;
                if (index <= 1)
                {
                    //this.SignComboBox.SelectedItem = null; 
                    //this.SignComboBox.IsEnabled = false;
                    
                }

            }
        }

      

        #endregion

        #region Operations
        public void Display(Relation item)
        {
            update = false;
            this.RelationItem = item;
            //if (item != null) this.Index = item.position + 1;
            //this.TextBox.Text = item != null && !String.IsNullOrWhiteSpace(item.GetValue()) ? item.GetValue() : "";
            //this.OpenParComboBox.SelectedItem = item != null && item.openPar == true ? "(" : null;
            //this.SignComboBox.SelectedItem = item != null && item.sign != null ? item.sign : null;
            //this.CloseParComboBox.SelectedItem = item != null && item.closePar == true ? ")" : null;
            //if (item != null && item.sign != null && item.sign.Equals("=")) this.TextBox.IsEnabled = false;
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
                //this.RelationItem.SetPosition(Index - 1);
                added = true;
            }
            //this.RelationItem = value.GetCopy(this.RelationItem);
            updateMeasureItemOperator();
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
           // this.TextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;
           // this.SignComboBox.SelectionChanged += OnOperatorChanged;
            this.RoleComboBox.SelectionChanged += OnRelationChanged;
            this.UserComboBox.SelectionChanged += OnRelationChanged;


            //this.TextBox.LostFocus += OnTextLostFocus;
            //this.TextBox.KeyDown += OnTextKeyDown;
            
        }
        
        bool update = true;
        private void OnRelationChanged(object sender, RoutedEventArgs e)
        {
           if (Updated != null && update)
            {
              //  this.TextBox.IsEnabled = true;
                //if (this.CalculatedMeasureItem != null)
               // {
                    if (setCalculatedMeasureItemOperator())

                        //if (this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals("="))
                        //    this.TextBox.IsEnabled = false;
                     
                    Updated(this);
                //}
                //else if (this.ComboBox.SelectedItem != null && this.ComboBox.SelectedItem.ToString().Equals("="))
                   // this.TextBox.IsEnabled = false;
                

            }
        }

        /// <summary>
        /// validate selection and set selected operator
        /// </summary>
        /// <returns></returns>
        private bool setCalculatedMeasureItemOperator()
        {
            added = false;
            if (this.RelationItem == null)
            {
                this.RelationItem = new Relation();
                //this.RelationItem.SetPosition(Index - 1);
                added = true;
            }

            bool add = this.added == true ? true : false;
            if (Added != null && added) Added(this);
            updateMeasureItemOperator();

            /*if (this.SignComboBox.SelectedItem == null && index==1)
            {
               
                this.CalculatedMeasureItem.closePar = false;
               if (this.CalculatedMeasureItem.openPar == true) this.OpenParComboBox.SelectedItem = this.OpenParComboBox.Items.GetItemAt(0);
                if (this.CalculatedMeasureItem.sign != null) this.CalculatedMeasureItem.sign = null;
                return false ;
            }
            else
            {

                if (this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals(" "))
                {
                    this.CalculatedMeasureItem.closePar = false;
                    this.CalculatedMeasureItem.openPar = false;
                    if (this.CalculatedMeasureItem.sign != null) this.CalculatedMeasureItem.sign = null;
                }
                if (this.OpenParComboBox.SelectedItem.ToString().Equals("("))
                {
                    this.CalculatedMeasureItem.openPar = true;
                    this.CalculatedMeasureItem.closePar = false;
                    if (this.CalculatedMeasureItem.sign != null) this.CalculatedMeasureItem.sign = null;
                    
                    int pos = this.CalculatedMeasureItem.position;
                    CalculatedMeasureItem last = null;
                    if (pos > 0)
                    {
                        last = this.CalculatedMeasureItem.calculatedMeasure.GetItemByPosition(pos - 1);
                    }
                    
                    if (last != null && last.measure != null)
                    {
                        last.measure = null;
                        this.CalculatedMeasureItem.calculatedMeasure.UpdateItem(last);
                    }
                }
                if (this.CloseParComboBox.SelectedItem.ToString().Equals(")"))
                {
                   
                        this.CalculatedMeasureItem.closePar = true;
                        
                }

                
                if (this.OpenParComboBox.SelectedItem.ToString().Equals("+") || this.OpenParComboBox.SelectedItem.ToString().Equals("-") || this.OpenParComboBox.SelectedItem.ToString().Equals("/") || this.OpenParComboBox.SelectedItem.ToString().Equals("*") || this.OpenParComboBox.SelectedItem.ToString().Equals("^"))
                {
                    this.CalculatedMeasureItem.sign = this.OpenParComboBox.SelectedItem.ToString();
                    this.CalculatedMeasureItem.closePar = false;
                    this.CalculatedMeasureItem.openPar = false;
                }

            }*/
            
            if (Updated != null && !added) Updated(this);
            return add;

        }

        private void updateMeasureItemOperator()
        {
            //openParCombobox

            if (this.RoleComboBox.SelectedItem != null && this.RoleComboBox.SelectedItem.ToString().Equals("("))
            {
                //this.RelationItem.openPar = true;
                /*if (this.CalculatedMeasureItem.sign != null) this.CalculatedMeasureItem.sign = null;

                int pos = this.CalculatedMeasureItem.position;
                CalculatedMeasureItem last = null;
                if (pos > 0)
                {
                     last = this.CalculatedMeasureItem.calculatedMeasure.GetItemByPosition(pos - 1);
                }
                if (last != null && last.measure != null)
                {
                    last.measure = null;
                    this.CalculatedMeasureItem.calculatedMeasure.UpdateItem(last);
                }*/
            }
            else { }
                    //this.RelationItem.openPar = false;

            ////signCombobox
            //if (this.SignComboBox.SelectedItem != null)
            //{
            //    this.RelationItem.sign = this.SignComboBox.SelectedItem.ToString();
            //}
            //else
            //{
            //    if (this.RelationItem.sign != null) this.RelationItem.sign = null;
            //}

            ////closeparCombobox
            //if (this.CloseParComboBox.SelectedItem != null && this.CloseParComboBox.SelectedItem.ToString().Equals(")") )
            //{
            //    this.RelationItem.closePar = true;
            //}
            //else
            //    this.RelationItem.closePar = false;
            
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
           // border.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 153, 255));
            if (Activated != null)
            {
                Activated(this);
            }

            if (sender is UserRelationItemPanel)
            {
                /*if (CloseParOrEqualSelected != null && this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals("="))
                {
                    this.TextBox.IsEnabled = false;
                    CloseParOrEqualSelected(this.OpenParComboBox.SelectedItem);
                }
                if (CloseParOrEqualSelected != null && this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals(")"))
                {
                    CloseParOrEqualSelected(this.OpenParComboBox.SelectedItem);
                    //this.TextBox.IsEnabled = false;
                }*/
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
            if (Deleted != null) Deleted(this);

        }


        #endregion



        public void changeBorder()
        {
           border.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 222, 222));
        }

        public void FillUsers(List<Domain.User> list)
        {
            this.userComboBox.ItemsSource = list;
        }

        public void FillRoles(List<Domain.Role> list)
        {
            this.roleComboBox.ItemsSource = list;
        }
    }
}
