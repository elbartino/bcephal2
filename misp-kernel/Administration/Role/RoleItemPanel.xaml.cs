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

namespace Misp.Kernel.Administration.Role
{
    /// <summary>
    /// Interaction logic for CalculatedMeasureItemPanel.xaml
    /// </summary>
    public partial class RoleItemPanel : Grid
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

        public RolePanel rolePanel;

        #endregion

        #region Constructor

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureItemPanel
        /// </summary>
        public RoleItemPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }
        

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">TargetItem à afficher</param>
        public RoleItemPanel(Domain.Role role)
            : this()
        {
            Display(role);
        }

        public RoleItemPanel(int index): this()
        {
            this.Index = index;
        }

        #endregion

        #region Properties

        public Label Label { get { return this.label; } }
        public Button Button { get { return this.deleteButton; } }
        public Button newButton { get { return this.addButton; } }
        
        public TextBox TextBox { get { return this.measureItem; } }
        public Domain.Role Role { get; set; }

        private int index;


        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Label.Content = "Role " + index;
                if (index <= 1)
                {
                    this.addButton.IsEnabled = true;
                }

            }
        }

      

        #endregion

        #region Operations
        public void Display(Domain.Role item)
        {
            update = false;
            this.Role = item;
            if (item != null) this.Index = item.position + 1;
            this.TextBox.Text = item != null && !String.IsNullOrWhiteSpace(item.name) ? item.name : "";
            update = true;
        }

       
       
        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        bool added = false;
        public void SetValue(Domain.Role value)
        {
            added = false;
            if (this.Role == null)
            {
                this.Role = new Domain.Role();
                this.Role.SetPosition(Index - 1);
                added = true;
            }
            //this.Role = value.GetCopy(this.Role);
            //updateMeasureItemOperator();
            added = setRoleItemAdded();
            this.TextBox.Text = this.Role != null ? this.Role.name : "";
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
            this.addButton.Click += OnButtonClick;
            this.Button.Click += OnButtonClick;
            this.Button.GotFocus += OnGotFocus;
            this.addButton.GotFocus += OnGotFocus;
            this.TextBox.GotFocus += OnGotFocus;
            this.GotFocus += OnGotFocus;
            
            this.TextBox.LostFocus += OnTextLostFocus;
            this.TextBox.KeyDown += OnTextKeyDown;
            this.TextBox.KeyUp += OnChangeName;
        }

        private void OnChangeName(object sender, KeyEventArgs args)
        {
            if (Updated != null && !added) Updated(this);
            if (args.Key == Key.Enter)
            {
                Added(this);
            }
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
        }
        
        bool update = true;
       
        /// <summary>
        /// validate selection and set selected operator
        /// </summary>
        /// <returns></returns>
        private bool setRoleItemAdded()
        {
            added = false;
            if (this.Role == null)
            {
                this.Role = new Domain.Role();
                this.Role.SetPosition(Index - 1);
                added = true;
            }

            bool add = this.added == true ? true : false;
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
            return add;

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
            String text = this.TextBox.Text;
            if (rolePanel != null)
            {
                foreach (UIElement el in rolePanel.panel.Children)
                {
                    RoleItemPanel item = (RoleItemPanel)el;
                    String roleName = item.TextBox.Text;

                    if (roleName.Equals(text) && item != this)
                    {
                        Util.MessageDisplayer.DisplayError("Duplicate Name", "There is another Target named: " + text);
                        this.TextBox.Text = null;
                    }
                }
            }

            decimal amount;
            bool valid = decimal.TryParse(text, out amount);
            if (!valid) return;

            Domain.Role value = new Domain.Role();
           
            SetValue(value);
        }
       
        
        
        #endregion
        
        public void changeBorder()
        {
          // border.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 222, 222));
        }
    }
}
