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

        public int objectOid;

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
                this.UserRightValuePanel.objectOid = objectOid;
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

        private void OnMouseLUDown(object sender, MouseButtonEventArgs e)
        {
            ActivateComponent(sender);
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

            if (Updated != null && !added) {
                add = true;
                Updated(this); 
            }
            return add;

        }

        private void updateProfilRight()
        {
            if (this.ProfilComboBox.SelectedItem != null )
            {
                Domain.Profil profil = (Domain.Profil)this.ProfilComboBox.SelectedItem;
                this.UserRightValuePanel.objectOid = objectOid;
                this.UserRightValuePanel.DisplayRightValue(profil);
            }
            else { }
        }

        public List<Domain.Right> deleteRightValue(Domain.Profil profil)
        {
            List<Domain.Right> deletedRight = new List<Domain.Right>();
            foreach (Domain.Right r in profil.rightsListChangeHandler.Items)
            {
                if (r.objectOid == objectOid)
                {
                    if (r.objectOid == objectOid && r.rightType == RightType.VIEW.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.EDIT.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.EDIT_CELL.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.EDIT_ALLOCATION.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.LOAD.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.DELETE.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.CLEAR.ToString())
                    {
                        deletedRight.Add(r);
                    }
                    if (r.objectOid == objectOid && r.rightType == RightType.SAVE_AS.ToString())
                    {
                        deletedRight.Add(r);
                    }
                }
            }
            return deletedRight;
        }

        public Domain.Profil fillObject(Domain.Profil pf)
        {
            UserRightValuePanel userValuePan = this.UserRightValuePanel;
            if (userValuePan.V.IsChecked.Value)
            {
                Right right = new Right("V");
                right.rightType = RightType.VIEW.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.VIEW) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.VIEW);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.ET.IsChecked.Value)
            {
                Right right = new Right("ET");
                right.rightType = RightType.EDIT.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.EDIT) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.EDIT);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.EC.IsChecked.Value)
            {
                Right right = new Right("EC");
                right.rightType = RightType.EDIT_CELL.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.EDIT_CELL) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.EDIT_CELL);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.EA.IsChecked.Value)
            {
                Right right = new Right("EA");
                right.rightType = RightType.EDIT_ALLOCATION.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.EDIT_ALLOCATION) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.EDIT_ALLOCATION);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.L.IsChecked.Value)
            {
                Right right = new Right("L");
                right.rightType = RightType.LOAD.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.LOAD) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.LOAD);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.D.IsChecked.Value)
            {
                Right right = new Right("D");
                right.rightType = RightType.DELETE.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.DELETE) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.DELETE);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.C.IsChecked.Value)
            {
                Right right = new Right("C");
                right.rightType = RightType.CLEAR.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.CLEAR) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.CLEAR);
                if (rg != null) pf.RemoveRight(rg);
            }

            if (userValuePan.S.IsChecked.Value)
            {
                Right right = new Right("S");
                right.rightType = RightType.SAVE_AS.ToString();
                right.objectOid = objectOid;
                if (getRightByOidObject(pf, RightType.SAVE_AS) == null) pf.AddRight(right);
            }
            else
            {
                Right rg = getRightByOidObject(pf, RightType.SAVE_AS);
                if (rg != null) pf.RemoveRight(rg);
            }

            return pf;
        }

        private Right getRightByOidObject(Domain.Profil pf, RightType t)
        {
            foreach (Right r in pf.rightsListChangeHandler.Items)
            {
                if (r.rightType != null && r.rightType.Equals(t.ToString()) 
                    && r.objectOid == objectOid) return r;
            }
            return null;
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

        public void FillProfilComboBox(List<Domain.Profil> list)
        {
            this.ProfilComboBox.ItemsSource = list;
        }

    }
}
