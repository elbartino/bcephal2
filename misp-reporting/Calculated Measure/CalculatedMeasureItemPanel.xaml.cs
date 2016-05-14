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

namespace Misp.Reporting.Calculated_Measure
{
    /// <summary>
    /// Interaction logic for CalculatedMeasureItemPanel.xaml
    /// </summary>
    public partial class CalculatedMeasureItemPanel : Grid
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
        public CalculatedMeasureItemPanel()
        {
            InitializeComponent();
            InitializeHandlers();
            this.OpenParComboBox.ItemsSource = new String[] {"","("};
            this.comboBox2.ItemsSource = new String[] { "+", "-", "^", "*", "/" };
            this.comboBox3.ItemsSource = new String[] {"",")"};
            
        }
        

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TargetItem à afficher</param>
        public CalculatedMeasureItemPanel(CalculatedMeasureItem calculatedMeasureItem):this()
        {
            Display(calculatedMeasureItem);
        }

        public CalculatedMeasureItemPanel(int index): this()
        {
            this.Index = index;
        }

        #endregion

        #region Properties

        public Label Label { get { return this.label; } }
        public Button Button { get { return this.deleteButton; } }
        public ComboBox OpenParComboBox { get { return this.comboBox; } }
        public ComboBox SignComboBox { get { return this.comboBox2; } }
        public ComboBox CloseParComboBox { get { return this.comboBox3; } }
        public TextBox TextBox { get { return this.measureItem; } }
        public CalculatedMeasureItem CalculatedMeasureItem { get; set; }

        private int index;


        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "Value " + index;
                if (index <= 1)
                {
                    this.SignComboBox.SelectedItem = null; 
                    this.SignComboBox.IsEnabled = false;
                    
                }

            }
        }

      

        #endregion

        #region Operations
        public void Display(CalculatedMeasureItem item)
        {
            update = false;
            this.CalculatedMeasureItem = item;
            if (item != null) this.Index = item.position + 1;
            this.TextBox.Text = item != null && !String.IsNullOrWhiteSpace(item.GetValue()) ? item.GetValue() : "";
            this.OpenParComboBox.SelectedItem = item != null && item.openPar == true ? "(" : null;
            this.SignComboBox.SelectedItem = item != null && item.sign != null ? item.sign : null;
            this.CloseParComboBox.SelectedItem = item != null && item.closePar == true ? ")" : null;
            if (item != null && item.sign != null && item.sign.Equals("=")) this.TextBox.IsEnabled = false;
            update = true;
        }

       
       
        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        bool added = false;
        public void SetValue(CalculatedMeasureItem value)
        {
            added = false;
            if (this.CalculatedMeasureItem == null)
            {
                this.CalculatedMeasureItem = new CalculatedMeasureItem();
                this.CalculatedMeasureItem.SetPosition(Index - 1);
                added = true;
            }
            this.CalculatedMeasureItem = value.GetCopy(this.CalculatedMeasureItem);
            updateMeasureItemOperator();
            //added = setCalculatedMeasureItemOperator();
            this.TextBox.Text = this.CalculatedMeasureItem != null ? this.CalculatedMeasureItem.GetValue() : "";
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
            this.SignComboBox.SelectionChanged += OnOperatorChanged;
            this.OpenParComboBox.SelectionChanged += OnOperatorChanged;
            this.CloseParComboBox.SelectionChanged += OnOperatorChanged;


            this.TextBox.LostFocus += OnTextLostFocus;
            this.TextBox.KeyDown += OnTextKeyDown;
            
        }
        
        bool update = true;
        private void OnOperatorChanged(object sender, RoutedEventArgs e)
        {
           if (Updated != null && update)
            {
                this.TextBox.IsEnabled = true;
                //if (this.CalculatedMeasureItem != null)
               // {
                    if (setCalculatedMeasureItemOperator())

                        if (this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals("="))
                            this.TextBox.IsEnabled = false;
                     
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
            if (this.CalculatedMeasureItem == null)
            {
                this.CalculatedMeasureItem = new CalculatedMeasureItem();
                this.CalculatedMeasureItem.SetPosition(Index - 1);
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
            
                if (this.OpenParComboBox.SelectedItem != null && this.OpenParComboBox.SelectedItem.ToString().Equals("("))
                {
                    this.CalculatedMeasureItem.openPar = true;
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
                else
                    this.CalculatedMeasureItem.openPar = false;

            //signCombobox
            if (this.SignComboBox.SelectedItem != null)
            {
                this.CalculatedMeasureItem.sign = this.SignComboBox.SelectedItem.ToString();
            }
            else
            {
                if (this.CalculatedMeasureItem.sign != null) this.CalculatedMeasureItem.sign = null;
            }

            //closeparCombobox
            if (this.CloseParComboBox.SelectedItem != null && this.CloseParComboBox.SelectedItem.ToString().Equals(")") )
            {
                this.CalculatedMeasureItem.closePar = true;
            }
            else
                this.CalculatedMeasureItem.closePar = false;
            
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

            if(sender is CalculatedMeasureItemPanel)
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
            String text = this.TextBox.Text;
            decimal amount;
            bool valid = decimal.TryParse(text, out amount);
            if (!valid) return;

            CalculatedMeasureItem value = new CalculatedMeasureItem();
            value.measureType = CalculatedMeasureItem.MeasureType.AMOUNT.ToString();
            value.amount = amount;
            SetValue(value);
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
    }
}
