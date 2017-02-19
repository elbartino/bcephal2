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

        public int objectOid;

        public event RightEventHandler RightSelected;

        public UserRightValuePanel()
        {
            InitializeComponent();
            initHandlers();
        }

        public void DisplayRightValue(Domain.Profil pf)
        {
           // reset();
            List<Right> rs = new List<Right>(pf.rightsListChangeHandler.Items);
            foreach (Right right in rs)
            {
                if (right.rightType != null && right.objectOid == objectOid)
                {
                    if (right.rightType.Equals(RightType.VIEW.ToString())) this.V.IsChecked = right.rightType.Equals(RightType.VIEW.ToString());
                    else if (right.rightType.Equals(RightType.EDIT.ToString())) this.ET.IsChecked = right.rightType.Equals(RightType.EDIT.ToString());
                    else if (right.rightType.Equals(RightType.EDIT_CELL.ToString())) this.EC.IsChecked = right.rightType.Equals(RightType.EDIT_CELL.ToString());
                    else if (right.rightType.Equals(RightType.EDIT_ALLOCATION.ToString())) this.EA.IsChecked = right.rightType.Equals(RightType.EDIT_ALLOCATION.ToString());
                    else if (right.rightType.Equals(RightType.LOAD.ToString())) this.L.IsChecked = right.rightType.Equals(RightType.LOAD.ToString());
                    else if (right.rightType.Equals(RightType.CLEAR.ToString())) this.C.IsChecked = right.rightType.Equals(RightType.CLEAR.ToString());
                    else if (right.rightType.Equals(RightType.DELETE.ToString())) this.D.IsChecked = right.rightType.Equals(RightType.DELETE.ToString());
                    else if (right.rightType.Equals(RightType.SAVE_AS.ToString())) this.S.IsChecked = right.rightType.Equals(RightType.SAVE_AS.ToString());
                }
            }
        }

        protected void initHandlers()
        {
            this.V.GotFocus += OnGotFocus;
            this.ET.GotFocus += OnGotFocus;
            this.EC.GotFocus += OnGotFocus;
            this.EA.GotFocus += OnGotFocus;
            this.L.GotFocus += OnGotFocus;
            this.S.GotFocus += OnGotFocus;
            this.C.GotFocus += OnGotFocus;
            this.D.GotFocus += OnGotFocus;

            this.V.MouseDown += OnMouseDown;
            this.ET.MouseDown += OnMouseDown;            
            this.EC.MouseDown += OnMouseDown;            
            this.EA.MouseDown += OnMouseDown;
            this.L.MouseDown += OnMouseDown;
            this.C.MouseDown += OnMouseDown;
            this.S.MouseDown += OnMouseDown;
            this.D.MouseDown += OnMouseDown;

            //this.RightSelected += OnRightSelected;

            this.V.Checked += OnChecked;
            this.ET.Checked += OnChecked;
            this.EC.Checked += OnChecked;
            this.EA.Checked += OnChecked;
            this.L.Checked += OnChecked;
            this.C.Checked += OnChecked;
            this.S.Checked += OnChecked;
            this.D.Checked += OnChecked;

            this.V.Unchecked += OnChecked;
            this.ET.Unchecked += OnChecked;
            this.EC.Unchecked += OnChecked;
            this.EA.Unchecked += OnChecked;
            this.L.Unchecked += OnChecked;
            this.C.Unchecked += OnChecked;
            this.S.Unchecked += OnChecked;
            this.D.Unchecked += OnChecked;
        }

        //private void OnRightSelected(Right right, bool selected)
        //{
        //    if (RightSelected != null) RightSelected(right, selected);
        //}


        private void OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            bool selected = box.IsChecked.Value;
            Right right = null;

            if (box.Name == this.V.Name)
            {
                right = new Right("V");
                right.rightType = RightType.VIEW.ToString();
                
            }
            else if (box.Name == this.ET.Name)
            {
                right = new Right("ET");
                right.rightType = RightType.EDIT.ToString();
            }
            else if (box.Name == this.EC.Name)
            {
                right = new Right("EC");
                right.rightType = RightType.EDIT_CELL.ToString();
            }
            else if (box.Name == this.EA.Name)
            {
                right = new Right("EA");
                right.rightType = RightType.EDIT_ALLOCATION.ToString();
            }
            else if (box.Name == this.L.Name)
            {
                right = new Right("L");
                right.rightType = RightType.LOAD.ToString();
            }
            else if (box.Name == this.C.Name)
            {
                right = new Right("C");
                right.rightType = RightType.CLEAR.ToString();
            }
            else if (box.Name == this.S.Name)
            {
                right = new Right("S");
                right.rightType = RightType.SAVE_AS.ToString();
            }
            if(right != null) RightSelected(right, selected);
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

        public void reset()
        {
            this.V.IsChecked = false;
            this.ET.IsChecked = false;
            this.EC.IsChecked = false;
            this.EA.IsChecked = false;
            this.D.IsChecked = false;
            this.L.IsChecked = false;
            this.C.IsChecked = false;
            this.S.IsChecked = false;
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
