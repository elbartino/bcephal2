using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class UserPanel : Grid
    {
        public static string notEmpty = "cannot be empty!";

        public UserPanel()
        {
            InitializeComponent();
        }

        public bool ValidateEdition()
        {
            String errors = "";
            String line = "";
            bool focusSetted = false;
            if (String.IsNullOrWhiteSpace(NameTextBox.Text) && String.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                errors += line + "Name and first name can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    if (String.IsNullOrWhiteSpace(NameTextBox.Text))
                    {
                        NameTextBox.Focus();
                        NameTextBox.SelectAll();
                    }
                    else
                    {
                        FirstNameTextBox.Focus();
                        FirstNameTextBox.SelectAll();
                    }
                    focusSetted = true;
                }
            }
                        
            if (String.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                //errors += line + "Email can't be empty.";
                //line = "\n";
                //if (!focusSetted)
                //{
                //    EmailTextBox.Focus();
                //    EmailTextBox.SelectAll();
                //    focusSetted = true;
                //}
            }
            else if (!validateEmail(EmailTextBox.Text))
            {
                errors += line + "Wrong email format.";
                line = "\n";
                if (!focusSetted)
                {
                    EmailTextBox.Focus();
                    EmailTextBox.SelectAll();
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                errors += line + "Login can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    LoginTextBox.Focus();
                    LoginTextBox.SelectAll();
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                errors += line + "Password can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    PasswordTextBox.Focus();
                    PasswordTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (!validatePassword(PasswordTextBox.Password, PasswordTextBox.Password))
            {
                errors += line + "Password does not match.";
                line = "\n";
                if (!focusSetted)
                {
                    //ConfirmPasswordTextBox.Focus();
                    //ConfirmPasswordTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            bool isValid = String.IsNullOrWhiteSpace(errors);
            this.Console.Text = errors;
            this.Console.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }

        private bool validatePassword(String password1, String passwordConfirm) 
        {
            return String.Compare(password1, passwordConfirm, false) == 0;
        }

        public bool validateEmail(String email)
        {
            string strRegex = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(email))
                return (true);
            else
                return (false);
        }


        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = LoginTextBox.Text.Trim();
            user.email = EmailTextBox.Text.Trim();
            user.name = FirstNameTextBox.Text;
            //user.password = ConfirmPasswordTextBox.Password;
            user.admin = true;
            return user;
        }

    }
}
