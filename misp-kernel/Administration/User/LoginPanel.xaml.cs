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
            String errors = "";
            String line = "";
            bool focusSetted = false;
            if (String.IsNullOrWhiteSpace(loginTextBox.Text))
            {
                errors += line + "Login can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    loginTextBox.Focus();
                    loginTextBox.SelectAll();
                    focusSetted = true;
                }
            }

            else if (String.IsNullOrWhiteSpace(passwordTextBox.Password))
            {
                errors += line + "Password can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    passwordTextBox.Focus();
                    passwordTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            bool isValid = String.IsNullOrWhiteSpace(errors);
            this.Console.Text = errors;
            this.Console.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }

        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = loginTextBox.Text.Trim();
            user.password = passwordTextBox.Password.Trim();
            return user;
        }

        public void reset()
        {
            loginTextBox.Text = "";
            passwordTextBox.Password = "";
        }

    }
}
