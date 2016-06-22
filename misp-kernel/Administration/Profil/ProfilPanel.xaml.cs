using Misp.Kernel.Domain;
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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for ProfilPanel.xaml
    /// </summary>
    public partial class ProfilPanel : Grid
    {
        public static string notEmpty = "cannot be empty!";

        public ProfilPanel()
        {
            InitializeComponent();
            Display();
        }

        public bool ValidateEdition()
        { return true; }

        public Domain.Profil Fill()
        {
            Domain.Profil user = new Domain.Profil();
            user.active = true;
            return user;
        }

        public void Display()
        {
            List<FunctionRight> items = FunctionRight.generateDefaultFunction();
            functionnalityGrid.ItemsSource = items;
        }

        
    }
}
