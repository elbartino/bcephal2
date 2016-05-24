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
        public AdministratorPanel()
        {
            InitializeComponent();
        }

        public bool ValidateEdition()
        {
            if (String.IsNullOrWhiteSpace(LastNameTextBox.Text) && String.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageDisplayer.DisplayWarning("Empty name", "Last and first names can't be empty!");
                return false;
            }

            return true;
        }

        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = LoginTextBox.Text.Trim();
            user.email = EmailTextBox.Text.Trim();
            
            return user;
        }

    }
}
