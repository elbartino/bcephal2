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
    /// Interaction logic for UserRightValuePanel.xaml
    /// </summary>
    public partial class UserRightValuePanel : Grid
    {
        public ActivateEventHandler Activated;
                
        public ChangeEventHandler ChangeEventHandler;

        public bool IsReadOnly { get; set; }        

        private bool trow;
        public UserRightValuePanel()
        {
            InitializeComponent();
            initHandlers();
        }

        public void DisplayRightValue(Domain.Profil profil, bool readOnly)
        {
            foreach (Right right in profil.rightsListChangeHandler.Items)
            {
                if (1 == 1) this.V.IsChecked = readOnly;
                if (1 == 1) this.ET.IsChecked = readOnly;
                if (1 == 1) this.EC.IsChecked = readOnly;
                if (1 == 1) this.EA.IsChecked = readOnly;
                if (1 == 1) this.D.IsChecked = readOnly;
                if (1 == 1) this.L.IsChecked = readOnly;
                if (1 == 1) this.C.IsChecked = readOnly;
                if (1 == 1) this.S.IsChecked = readOnly;
            }            
            
        }

        public Domain.Profil FillRights(Domain.Profil profil)
        {
            profil.rightsListChangeHandler.resetOriginalList();
            if (V.IsChecked == true)
            {
                Right v = new Right("V");
                profil.AddRight(v);
            }
            if (ET.IsChecked == true)
            {
                Right et = new Right("Edit Table");
                profil.AddRight(et);
            }
            if (EC.IsChecked == true)
            {
                Right ec = new Right("Edit Cell");
                profil.AddRight(ec);
            }
            if (EA.IsChecked == true)
            {
                Right ea = new Right("Edit All");
                profil.AddRight(ea);
            }
            if (D.IsChecked == true)
            {
                Right d = new Right("Delete");
                profil.AddRight(d);
            }
            if (L.IsChecked == true)
            {
                Right l = new Right("Load");
                profil.AddRight(l);
            }
            if (C.IsChecked == true)
            {
                Right c = new Right("Clear");
                profil.AddRight(c);
            }
            if (S.IsChecked == true)
            {
                Right s = new Right("Save As");
                profil.AddRight(s);
            }
            return profil;
        }

        protected void initHandlers()
        {
            this.V.GotFocus += OnGotFocus;
            this.V.MouseDown += OnMouseDown;

            this.ET.GotFocus += OnGotFocus;
            this.ET.MouseDown += OnMouseDown;

            this.EC.GotFocus += OnGotFocus;
            this.EC.MouseDown += OnMouseDown;

            this.EA.GotFocus += OnGotFocus;
            this.EA.MouseDown += OnMouseDown;

            this.L.GotFocus += OnGotFocus;
            this.L.MouseDown += OnMouseDown;

            this.C.GotFocus += OnGotFocus;
            this.C.MouseDown += OnMouseDown;

            this.S.GotFocus += OnGotFocus;
            this.S.MouseDown += OnMouseDown;

            this.D.GotFocus += OnGotFocus;
            this.D.MouseDown += OnMouseDown;
        }

        public void SetReadOnly(bool readOnly)
        {
            this.V.IsEnabled = readOnly;
            this.ET.IsEnabled = readOnly;
            this.EC.IsEnabled = readOnly;
            this.EA.IsEnabled = readOnly;
            this.D.IsEnabled = readOnly;
            this.L.IsEnabled = readOnly;
            this.C.IsEnabled = readOnly;
            this.S.IsEnabled = readOnly;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
            OnChange();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
            OnChange();
        }

        private void OnChange()
        {
            if (ChangeEventHandler != null) ChangeEventHandler();
        }
        
    }
}
