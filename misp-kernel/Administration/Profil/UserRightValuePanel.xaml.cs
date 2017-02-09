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

        public void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.V.IsEnabled = readOnly;
            this.ET.IsEnabled = readOnly;
            this.EC.IsEnabled = readOnly;
            this.EA.IsEnabled = readOnly;
            this.D.IsEnabled = readOnly;
            this.L.IsEnabled = readOnly;
            this.C.IsEnabled = readOnly;
            this.S.IsEnabled = readOnly;
        }

    }
}
