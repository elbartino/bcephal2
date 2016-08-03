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
        public ConnectedUserProfile()
        {
            InitializeComponent();
        }

        public void Display(Kernel.Domain.User user)
        {
            User = user;
            userNameTextbox.Text = user.name;
            userLoginTextbox.Text = user.login;
            userFirstNameTextbox.Text = user.firstName;
            isAdminUserCheckbox.IsChecked = user.administrator;
            userProfileTextbox.Text = user.profil != null ? user.profil.name : "";
            userMailTextbox.Text = user.email;
        }

        public Domain.User Fill()
        {
            User.name =  userNameTextbox.Text;
            User.firstName = userFirstNameTextbox.Text.Trim(); ;
            User.email = userMailTextbox.Text.Trim();
           // User.password = password2Textbox.tex
            return User;
        }

    }
}
