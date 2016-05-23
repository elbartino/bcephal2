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
    /// Interaction logic for ProfileMainPanel.xaml
    /// </summary>
    public partial class ProfileMainPanel : Grid
    {
        public ProfileMainPanel()
        {
            InitializeComponent();
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);

            return controls;
        }

        public void Display(Domain.Profil profil)
        {
            //this.nameTextBox.Text = user.name;
            //this.passTextBox.Text = user.password;
            //this.activeCheckBox.IsChecked = user.active;
            //this.emailTextBox.Text = user.email;
        }
    }
}
