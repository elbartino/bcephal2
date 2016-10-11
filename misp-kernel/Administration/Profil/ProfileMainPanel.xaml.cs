using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for ProfileMainPanel.xaml
    /// </summary>
    public partial class ProfileMainPanel : Grid
    {  
        
        public ProfileMainPanel()
        {
            InitializeComponent();
            this.RightsPanel.SetFunctionalities(ApplicationManager.Instance.FunctionalityFactory.Functionalities);
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.activeBox);
            return controls;
        }

        public void Display(Domain.Profil profil)
        {
            nameTextBox.Text = profil.name;
            activeBox.IsChecked = profil.active;
            this.RightsPanel.Display(profil);
        }

        public void Fill(Domain.Profil profil)
        {
            profil.active = activeBox.IsChecked.Value;
            profil.name = nameTextBox.Text;
        }

        public bool ValidateEdition()
        {
            if (nameTextBox.Text == null || nameTextBox.Text == "") return false;
            return true;
        }
                                
    }
}
