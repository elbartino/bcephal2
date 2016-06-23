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
            IntializeHandlers();
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(profilPanel.getEditableControls()); 
            return controls;
        }

        public void Display(Domain.Profil profil)
        {
            profilPanel.Display(profil);
        }

        public void Fill(Domain.Profil profil)
        {
            profil = profilPanel.Fill(profil);
        }

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            
            
        }

        
    }
}
