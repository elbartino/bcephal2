using Misp.Kernel.Service;
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
    /// Interaction logic for UserMainPanel.xaml
    /// </summary>
    public partial class UserMainPanel : Grid
    {

        public UserMainPanel()
        {
            InitializeComponent();
            IntializeHandlers();
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.firstNameTextBox);
            controls.Add(this.userIDTextBox);
            controls.Add(this.departementTextBox);
            controls.Add(this.emailTextBox);
            controls.Add(this.adminCheckBox);
            controls.Add(this.activeBox);
            controls.Add(this.loginTextBox);
            controls.Add(this.passwordTextBox);
            return controls;
        }

        public void Display(Domain.User user)
        {
            nameTextBox.Text = user.name;
            firstNameTextBox.Text = user.firstName;
            //userIDTextBox.Text = user.userID;
            //departementTextBox.Text = user.departement;
            emailTextBox.Text = user.email;
            adminCheckBox.IsChecked = user.IsAdmin();
            activeBox.IsChecked = user.active;
            loginTextBox.Text = user.login;
            passwordTextBox.Password = user.password;
            ManageAdministratorView(user.IsAdmin());
            if (!user.IsAdmin())
            {
                profilcomboBox.SelectedItem = user.profil;
            }
            RelationPanel.DisplayUserRelations(user);
        }

        public void Fill(Domain.User user)
        {
            user.name = nameTextBox.Text;
            user.firstName = firstNameTextBox.Text;
           // user.userID = userIDTextBox.Text;
           // user.departement = departementTextBox.Text;
            user.email = emailTextBox.Text.Trim();
            user.active = activeBox.IsChecked.Value;
            user.administrator = adminCheckBox.IsChecked.Value;
            user.login = loginTextBox.Text.Trim();
            user.password = passwordTextBox.Password;
            if (!user.IsAdmin())
            {
                Domain.Profil profil = (Domain.Profil)profilcomboBox.SelectedItem;
                user.profil = profil;
            }
        }

        public void InitProfilComboBox(ProfilService profilService)
        {
            List<Domain.Profil> profils = profilService.getAll();
            this.profilcomboBox.ItemsSource = profils;
            this.profilcomboBox.SelectedIndex = 0;
        }

        public void InitRelationPanel(UserService userservice) 
        {
            List<Domain.User> users = userservice.getAll();
            Domain.Role RootRole = userservice.RoleService.getRootRole();
            this.RelationPanel.FillUsers(users);
            this.RelationPanel.FillRoles(RootRole.childrenListChangeHandler.Items.ToList());
        }

        public bool ValidateEdition()
        {
            String errors = "";
            String line = "";
            bool focusSetted = false;
            if (String.IsNullOrWhiteSpace(nameTextBox.Text) && String.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                errors += line + "Name and first name can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    if (String.IsNullOrWhiteSpace(nameTextBox.Text))
                    {
                        nameTextBox.Focus();
                        nameTextBox.SelectAll();
                    }
                    else
                    {
                        firstNameTextBox.Focus();
                        firstNameTextBox.SelectAll();
                    }
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                errors += line + "Email can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    emailTextBox.Focus();
                    emailTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (!validateEmail(emailTextBox.Text))
            {
                //errors += line + "Wrong email format.";
                //line = "\n";
                //if (!focusSetted)
                //{
                //    emailTextBox.Focus();
                //    emailTextBox.SelectAll();
                //    focusSetted = true;
                //}
            }

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

            if (String.IsNullOrWhiteSpace(passwordTextBox.Password))
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

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            this.adminCheckBox.Checked += OnManageAdministrator;
            this.adminCheckBox.Unchecked += OnManageAdministrator;

        }

        private void OnManageAdministrator(object sender, RoutedEventArgs e)
        {
            ManageAdministratorView(this.adminCheckBox.IsChecked.Value);
        }

        public void ManageAdministratorView(bool isAdmin)
        {
            this.profilcomboBox.IsEnabled = !isAdmin;
        }

    }
}
