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
        }

        private void InitializeHandlers()
        {
            functionnalityGrid.ChangeHandler += OnGridSelectionchange;

        }

        public bool ValidateEdition()
        { 
            if(nameTextBox.Text == null || nameTextBox.Text == "") return false;                
            return true; 
        }


        public Domain.Profil Fill(Domain.Profil pf)
        {
            pf.active = activeBox.IsChecked.Value;
            pf.name = nameTextBox.Text;
            List<Rights> items = new List<Rights>(0);
            foreach (object item in functionnalityGrid.Items)
            {
                if (item is Rights)
                {
                    items.Add((Rights)item);
                }
            }
            pf.rightsListChangeHandler.updatedItems = new List<Rights>(items);
            return pf;
        }

        public void Display(Domain.Profil profil)
        {
            nameTextBox.Text = profil.name;
            activeBox.IsChecked = profil.active;
            List<Rights> items = Misp.Kernel.Domain.Rights.generateDefaultFunction();
            functionnalityGrid.ItemsSource = items;
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

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.activeBox);
            controls.Add(this.functionnalityGrid);
            return controls;
        }

    }
}
