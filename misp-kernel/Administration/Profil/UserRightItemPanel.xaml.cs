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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for UserRightItemPanel.xaml
    /// </summary>
    public partial class UserRightItemPanel : Grid
    {
        #region Events
        public ChangeEventHandler ChangeEventHandler;

        public event DeleteEventHandler Deleted;

        public event AddEventHandler Added;

        public event UpdateEventHandler Updated;

        public ActivateEventHandler Activated;

        public event RightEventHandler RightSelected;
        #endregion

        public Kernel.Domain.Profil profil { get; set; }

        public UserRightValuePanel ActiveUserRightValuePanel;

        public bool IsReadOnly { get; set; }

        //public bool trow = false;

        #region Contructor
        public UserRightItemPanel()
        {
            InitializeComponent();
            this.UserRightValuePanel.IsEnabled = false;
            initHandlers();
        }

        /// <summary>
        /// Construit une nouvelle instance de right
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">right à afficher</param>
        public UserRightItemPanel(Kernel.Domain.Profil item): this()
        {
            Display(item);
        }

        public UserRightItemPanel(int index)
            : this()
        {
            this.Index = index;
        }

        #endregion

        #region Operations
        bool update = true;
        bool added = false;
        bool delete = false;
        private int index;


        public int Index
        {
            get { return index; }
            set
            {
                index = value;
            }
        }

        public void Display(Kernel.Domain.Profil item, bool readOnly = false)
        {
            if (item != null)
            {
                update = false;
                this.profil = item;
                this.ProfilComboBox.SelectedItem = item;
                this.UserRightValuePanel.DisplayRightValue(item);
                update = true;
            }
            else
            {
                Reset();
                return;
            }
        }

        

        public void Reset()
        {
            if (this.IsReadOnly) this.SetReadOnly(this.IsReadOnly);
        }

        #endregion

        #region Handlers
        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;

            this.ProfilComboBox.SelectionChanged += onProfilChange;

            this.AddButton.GotFocus += OnGotFocus;
            this.DeleteButton.GotFocus += OnGotFocus;
            this.ProfilComboBox.GotFocus += OnGotFocus;

            this.AddButton.MouseDown += OnMouseDown;
            this.DeleteButton.MouseDown += OnMouseDown;
            this.ProfilComboBox.MouseDown += OnMouseDown;

            this.UserRightValuePanel.Activated += OnActivate;
            this.UserRightValuePanel.ChangeEventHandler += onUserRightValueChange;
            this.UserRightValuePanel.RightSelected += OnRightrSelected;
        }


        private void OnRightrSelected(Right right, bool selected)
        {
            if (RightSelected != null) RightSelected(right, selected);
        }

        

        private void onProfilChange(object sender, SelectionChangedEventArgs e)
        {
            Object selection = this.ProfilComboBox.SelectedItem;
            if (selection == null)
            {
                this.UserRightValuePanel.Visibility = Visibility.Collapsed;
                this.UserRightValuePanel.IsEnabled = false;
            }
            if (selection != null && selection.ToString().Trim() != null)
            {
                this.profil = (Domain.Profil)selection;
                this.UserRightValuePanel.Visibility = Visibility.Visible;
                this.UserRightValuePanel.IsEnabled = true;
            }

            if (Updated != null && update)
            {
                if (setProfilRight())
                    Updated(this);
            }
        }

        /// <summary>
        /// validate selection and set selected operator
        /// </summary>
        /// <returns></returns>
        private bool setProfilRight()
        {
            added = false;
            if (this.profil == null)
            {
                this.profil = new Kernel.Domain.Profil();
                added = true;
            }

            this.profil = (Kernel.Domain.Profil)this.ProfilComboBox.SelectedItem;

            bool add = this.added == true ? true : false;
            if (Added != null && added) Added(this);
            updateProfilRight();

            if (Updated != null && !added) Updated(this);
            return add;

        }

        private void updateProfilRight()
        {
            if (this.ProfilComboBox.SelectedItem != null )
            {
                Domain.Profil profil = (Domain.Profil)this.ProfilComboBox.SelectedItem;
                this.UserRightValuePanel.DisplayRightValue(profil);
            }
            else { }
        }

        private void onUserRightValueChange()
        {
            if (ChangeEventHandler != null) ChangeEventHandler();
        }


        private void ActivateComponent(object item)
        {
            if (Activated != null)
            {
                if (item is UserRightValuePanel)
                {
                    this.ActiveUserRightValuePanel = (UserRightValuePanel)item;
                }
                Activated(this);
            }
        }
        
        private void OnActivate(object item)
        {
            ActivateComponent(item);
        }

        private void ActiveHandler()
        {
            if (Activated != null) Activated(this);
        }
        

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ActiveHandler();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            ActiveHandler();
        }

        

        private bool isDeleted = false;
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null)
            {
                isDeleted = true;
                Deleted(this);
            }
            onUserRightValueChange();
            isDeleted = false;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (Added != null) Added(this);
        }
        #endregion

        public void SetItemPanelReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.ProfilComboBox.IsEnabled = !readOnly;
            this.AddButton.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.DeleteButton.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;            
        }

        public void SetReadOnly(bool readOnly)
        {
            SetItemPanelReadOnly(readOnly);
            if (this.UserRightValuePanel != null)
            {
                this.UserRightValuePanel.SetReadOnly(readOnly);
            }
        }

        public void FillProfil(List<Domain.Profil> list)
        {
            this.ProfilComboBox.ItemsSource = list;
        }

    }
}
