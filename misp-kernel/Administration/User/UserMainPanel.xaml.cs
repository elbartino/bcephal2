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

namespace Misp.Kernel.Administration.User
{
    /// <summary>
    /// Interaction logic for UserMainPanel.xaml
    /// </summary>
    public partial class UserMainPanel : Grid
    {
        public UserMainPanel()
        {
            InitializeComponent();
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
           
            return controls;
        }

        public void Display(Domain.User user)
        {
            this.nameTextBox.Text = user.name;
            this.passTextBox.Text = user.password;
            this.activeCheckBox.IsChecked = user.active;
            this.emailTextBox.Text = user.email;
        }

        public void Fill()
        {
            //this.nameTextBox.Text = user.name;
            //this.passTextBox.Text = user.password;
            //this.activeCheckBox.IsChecked = user.active;
            //this.emailTextBox.Text = user.email;
        }
    }
}
