using Misp.Kernel.Util;
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
    /// Interaction logic for LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : Grid
    {
        public LoginPanel()
        {
            InitializeComponent();
        }

        public bool ValidateEdition()
        {
            bool result = true;
            if (String.IsNullOrWhiteSpace(loginTextBox.Text))
            {
                loginErrorLabel.Content = "Login " + AdministratorPanel.notEmpty;
                loginErrorLabel.Visibility = System.Windows.Visibility.Visible;
                result = false;
            }
            else
            {
                loginErrorLabel.Content = "";
                loginErrorLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            if(String.IsNullOrWhiteSpace(passwordTextBox.Password.Trim()))
            {
                passwordErrorLabel.Content ="Password "+AdministratorPanel.notEmpty;
                passwordErrorLabel.Visibility = System.Windows.Visibility.Visible;
                result = false;
            }
            else
            {
                passwordErrorLabel.Content ="";
                passwordErrorLabel.Visibility = System.Windows.Visibility.Collapsed;
            }

            return result;
        }

        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = loginTextBox.Text.Trim();
            user.password = passwordTextBox.Password.Trim();
            return user;
        }

        private void OnRequestNewPassword(object sender, RequestNavigateEventArgs e)
        {

        }
    }
}
