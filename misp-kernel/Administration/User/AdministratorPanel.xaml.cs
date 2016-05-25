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
    /// Interaction logic for AdministratorPanel.xaml
    /// </summary>
    public partial class AdministratorPanel : Grid
    {
        public static string notEmpty = "cannot be empty!";

        public AdministratorPanel()
        {
            InitializeComponent();
        }

        public bool ValidateEdition()
        {
            bool result = true;
            if (String.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                FirstNameErrorLabel.Content = "First name " + notEmpty;
                FirstNameErrorLabel.Visibility = Visibility.Visible;
                result = false;
            }
            //else
            //{
            //    FirstNameErrorLabel.Content = "";
            //    FirstNameErrorLabel.Visibility = Visibility.Hidden;
            //}

            if (String.IsNullOrWhiteSpace((FirstNameTextBox.Text)))
            {
                lastNameErrorLabel.Content = "Last name " + notEmpty;
                lastNameErrorLabel.Visibility = Visibility.Visible;
                result = false;
            }
            //else
            //{
            //    lastNameErrorLabel.Content = "";
            //    lastNameErrorLabel.Visibility = Visibility.Hidden;
            //}

            if (String.IsNullOrWhiteSpace((LoginTextBox.Text)))
            {
                LoginErrorLabel.Content = "Login " + notEmpty;
                LoginErrorLabel.Visibility = Visibility.Visible;
                result = false;
            }
            //else
            //{
            //    lastNameErrorLabel.Content = "";
            //    lastNameErrorLabel.Visibility = Visibility.Hidden;
            //}

            if (String.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                EmailNameErrorLabel.Content = "Email " + notEmpty;
                EmailNameErrorLabel.Visibility = System.Windows.Visibility.Visible;
                result = false;
            }
            else
            {
                if (!validateEmail(EmailTextBox.Text))
                {
                    EmailNameErrorLabel.Content = "Wrong email format!";
                    EmailNameErrorLabel.Visibility = System.Windows.Visibility.Visible;
                    result = false;
                }
                //else
                //{
                //    EmailNameErrorLabel.Content = "";
                //    EmailNameErrorLabel.Visibility = System.Windows.Visibility.Hidden;
                //}
            }

            if (String.IsNullOrWhiteSpace(PasswordTextBox.Password.Trim()))
            {
                PasswordErrorLabel.Content = "Password " + notEmpty;
                PasswordErrorLabel.Visibility = System.Windows.Visibility.Visible;
                result = false;
            }
            else
            {
                if (!validatePassword(PasswordTextBox.Password.Trim(), ConfirmPasswordTextBox.Password.Trim()))
                {
                    ConfirmPasswordErrorLabel.Content = "Password does not match!";
                    ConfirmPasswordErrorLabel.Visibility = System.Windows.Visibility.Visible;
                    result = false;
                }
                else
                {
                    PasswordErrorLabel.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            return result;
        }

        private bool validatePassword(String password1, String passwordConfirm) 
        {
            return String.Compare(password1,passwordConfirm,false) == 0;
        }

        public bool validateEmail(String email)
        {
            return true;
        }


        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = LoginTextBox.Text.Trim();
            user.email = EmailTextBox.Text.Trim();
            user.name = FirstNameTextBox.Text;
            user.password = ConfirmPasswordTextBox.Password.Trim();
            user.admin = true;
            return user;
        }

    }
}
