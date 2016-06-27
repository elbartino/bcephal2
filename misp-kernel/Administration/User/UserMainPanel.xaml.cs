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
            controls.Add(this.typeBox);
            controls.Add(this.activeBox);
            controls.Add(this.loginTextBox);
            controls.Add(this.passwordTextBox);
            return controls;
        }

        public void Display(Domain.User user)
        {
            nameTextBox.Text = user.name;
            firstNameTextBox.Text = user.firstName;
            userIDTextBox.Text = user.name;
            departementTextBox.Text = user.firstName;
            emailTextBox.Text = user.email;
            typeBox.IsChecked = user.active;
            activeBox.IsChecked = user.active;
            loginTextBox.Text = user.login;
            passwordTextBox.Password = user.password; 
        }

        public void Fill(Domain.User user)
        {
            user.name = nameTextBox.Text;
            user.firstName = firstNameTextBox.Text;
            user.userID = userIDTextBox.Text;
            user.departement = departementTextBox.Text;
            user.email = emailTextBox.Text.Trim();
            user.active = activeBox.IsChecked.Value;
            user.type = typeBox.IsChecked.Value;
            user.login = loginTextBox.Text.Trim();
            user.password = passwordTextBox.Password;
        }

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {


        }

    }
}
