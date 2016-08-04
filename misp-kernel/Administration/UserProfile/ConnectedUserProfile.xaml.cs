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

namespace Misp.Kernel.Administration.UserProfile
{
    /// <summary>
    /// Interaction logic for ConnectedUserProfile.xaml
    /// </summary>
    public partial class ConnectedUserProfile : Grid
    {

        Domain.User User;
        bool passwordChanged;

        public event Misp.Kernel.Ui.Base.ChangeEventHandler Changed;        

        public ConnectedUserProfile()
        {
            InitializeComponent();
            passwordPanel.Visibility = System.Windows.Visibility.Collapsed;
            InitializeHandlers();
        }

        public void Display(Kernel.Domain.User user)
        {
            User = user;
            userNameTextbox.Text = user.name;
            userLoginTextbox.Text = user.login;
            userFirstNameTextbox.Text = user.firstName;
            isAdminUserCheckbox.IsChecked = user.administrator;
            isActiveUserCheckbox.IsChecked = user.active;
            userProfileTextbox.Text = user.profil != null ? user.profil.name : "";
            userMailTextbox.Text = user.email;
        }

        public Domain.User Fill(Kernel.Domain.User user)
        {
            user.email = userMailTextbox.Text.Trim();
            user.password = confirmPasswordTextbox.Password;
            return user;
        }

        public void InitializeHandlers()
        {
            changePasswordCheckbox.Checked += OnActivatePasswordOptions;
            changePasswordCheckbox.Unchecked += OnActivatePasswordOptions;
        }

        private void OnEndEditNewPassword(object sender, MouseEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(newPasswordTextbox.Password) && String.IsNullOrWhiteSpace(confirmPasswordTextbox.Password))
            {
                return;
            }
            if (!UserUtil.validatePassword(passwordOldTextbox.Password, confirmPasswordTextbox.Password))
            {
                if (Changed != null) Changed();
            }
        }

     

        private void OnActivatePasswordOptions(object sender, RoutedEventArgs e)
        {
            if (changePasswordCheckbox.IsChecked == true)
            {
                passwordPanel.Visibility = System.Windows.Visibility.Visible;
                if (Changed != null) Changed();
            }
            else if (changePasswordCheckbox.IsChecked == false)
            {
                passwordPanel.Visibility = System.Windows.Visibility.Collapsed;
                Console.Visibility = System.Windows.Visibility.Collapsed;
                Console.Text = "";
                passwordOldTextbox.Password = "";
                newPasswordTextbox.Password = "";
                confirmPasswordTextbox.Password = "";
            }
            
        }

        public bool ValidateEdition()
        {
            String errors = "";
            String line = "";
            bool focusSetted = false;

            if (String.IsNullOrWhiteSpace(userMailTextbox.Text))
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
            else if (!UserUtil.validateEmail(userMailTextbox.Text))
            {
                errors += line + "Wrong email format.";
                line = "\n";
                if (!focusSetted)
                {
                    userMailTextbox.Focus();
                    userMailTextbox.SelectAll();
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(passwordOldTextbox.Password) && String.IsNullOrWhiteSpace(newPasswordTextbox.Password) && String.IsNullOrWhiteSpace(confirmPasswordTextbox.Password))
            {

            }

            else if (String.IsNullOrWhiteSpace(passwordOldTextbox.Password) && !String.IsNullOrWhiteSpace(newPasswordTextbox.Password))
            {
                errors += line + "Old Password can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    passwordOldTextbox.Focus();
                    passwordOldTextbox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (!UserUtil.validatePassword(this.User.password, passwordOldTextbox.Password))
            {
                errors += line + "Wrong old Password.";
                line = "\n";
                if (!focusSetted)
                {
                    passwordOldTextbox.Focus();
                    passwordOldTextbox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (String.IsNullOrWhiteSpace(newPasswordTextbox.Password) && !String.IsNullOrWhiteSpace(passwordOldTextbox.Password) && !String.IsNullOrWhiteSpace(confirmPasswordTextbox.Password))
            {
                errors += line + "New Password can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    newPasswordTextbox.Focus();
                    newPasswordTextbox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (!Util.UserUtil.validatePassword(newPasswordTextbox.Password, confirmPasswordTextbox.Password))
            {
                errors += line + "Password does not match.";
                line = "\n";
                if (!focusSetted)
                {
                    confirmPasswordTextbox.Focus();
                    confirmPasswordTextbox.SelectAll();
                    focusSetted = true;
                }
            }
           
            bool isValid = String.IsNullOrWhiteSpace(errors);
            this.Console.Text = errors;
            this.Console.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }


        public IEnumerable<object> getEditableControls()
        {
            List<object> list = new List<object>(0);
            list.Add(confirmPasswordTextbox);
            list.Add(newPasswordTextbox);
            list.Add(passwordOldTextbox);
            list.Add(Console);
            list.Add(userFirstNameTextbox);
            list.Add(userLoginTextbox);
            list.Add(userMailTextbox);
            list.Add(userNameTextbox);
            return list;
        }
    }
}
