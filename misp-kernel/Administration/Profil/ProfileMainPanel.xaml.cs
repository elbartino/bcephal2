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

            List<Rights> items = Misp.Kernel.Domain.Rights.generateDefaultFunction();
            functionnalityGrid.ItemsSource = items;
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.activeBox);
            controls.Add(this.functionnalityGrid);
            return controls;
        }

        public void Display(Domain.Profil profil)
        {
            nameTextBox.Text = profil.name;
            activeBox.IsChecked = profil.active;
            if (profil.rightsListChangeHandler.getItems().Count > 0)
            {
                functionnalityGrid.ItemsSource = profil.rightsListChangeHandler.getItems();
            }
        }

        public void Fill(Domain.Profil profil)
        {
            profil.active = activeBox.IsChecked.Value;
            profil.name = nameTextBox.Text;
            List<Rights> items = new List<Rights>(0);
            foreach (object item in functionnalityGrid.Items)
            {
                if (item is Rights)
                {                    
                    items.Add((Rights)item);
                }
            }
            profil.rightsListChangeHandler.updatedItems = new List<Rights>(items);
        }

        public bool ValidateEdition()
        {
            if (nameTextBox.Text == null || nameTextBox.Text == "") return false;
            return true;
        }

        /// <summary>
        /// initialise handlers
        /// </summary>
        private void IntializeHandlers()
        {
            functionnalityGrid.ChangeHandler += OnGridSelectionchange;
            
        }

        private void OnGridSelectionchange()
        {
            List<Rights> items = new List<Rights>(0);
            List<long> ids = new List<long>(0);
            foreach (object item in functionnalityGrid.SelectedItems)
            {
                if (item is Rights)
                {
                    items.Add((Rights)item);
                    //ids.Add(((Rights)item).id);
                }
            }
        }
    }
}
