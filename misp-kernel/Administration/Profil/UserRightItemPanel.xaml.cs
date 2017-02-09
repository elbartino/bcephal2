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
        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;
        public Kernel.Ui.Base.ChangeItemEventHandler Updated;
        public ActivateEventHandler Activated;

        public Kernel.Domain.Profil profil { get; set; }

        public bool IsReadOnly { get; set; }

        public bool trow = false;

        public UserRightItemPanel()
        {
            InitializeComponent();
            this.ProfilComboBox.ItemsSource = null;
            initHandlers();
        }


        public void Display(object item, bool readOnly = false)
        {
            if (item != null && item is Kernel.Domain.Profil)
            {
                this.profil = (Kernel.Domain.Profil)item;
                this.UserRightValuePanel.DisplayRightValue(this.profil, readOnly);
            }
            else
            {
                Reset();
                return;
            }
            trow = true;
        }

        

        public void Reset()
        {
            trow = false;            
            if (this.IsReadOnly) this.SetReadOnly(this.IsReadOnly);
            trow = true;
        }

        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;

            this.ProfilComboBox.SelectionChanged += onChange;

            this.AddButton.GotFocus += OnGotFocus;
            this.DeleteButton.GotFocus += OnGotFocus;
            this.ProfilComboBox.GotFocus += OnGotFocus;

            this.AddButton.MouseDown += OnMouseDown;
            this.DeleteButton.MouseDown += OnMouseDown;
            this.ProfilComboBox.MouseDown += OnMouseDown;

            this.UserRightValuePanel.Activated += OnActivate;
            this.UserRightValuePanel.ChangeEventHandler += onChange;

            //this.UserRightValuePanel.V.Checked += onViewChecked;
            //this.UserRightValuePanel.ET.Checked += onEditTableChecked;
            //this.UserRightValuePanel.EC.Checked += onEditCellChecked;
            //this.UserRightValuePanel.EA.Checked += onEditAllChecked;
            //this.UserRightValuePanel.D.Checked += onDeleteChecked;
            //this.UserRightValuePanel.L.Checked += onLoadChecked;
            //this.UserRightValuePanel.C.Checked += onClearChecked;
            //this.UserRightValuePanel.S.Checked += onSaveAsChecked;


        }

        private void onChange()
        {
            if (trow && ChangeEventHandler != null)
            {
                if (!isDeleted)
                {
                    //FillLoopProperties(null);
                }
                ChangeEventHandler();
            }
        }

        #region Activation Method
        private void ActivateComponent(object item)
        {
            if (Activated != null)
            {
                //if (item is LoopCalculatedValuePanel)
                //{
                //    this.ActiveLoopCalucatedValue = (LoopCalculatedValuePanel)item;
                //}
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

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            if (trow)
            {
                //FillLoopProperties(getBrackets());
                onChange();
            }
        }

        private bool isDeleted = false;
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null)
            {
                isDeleted = true;
                Deleted(this);
            }
            onChange();
            isDeleted = false;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Added != null) Added(this);
        }

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

    }
}
