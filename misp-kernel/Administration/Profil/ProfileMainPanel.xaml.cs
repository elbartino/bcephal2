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
        public event OnSelectAllClickedEventHandler OnSelectAllClicked;
        public delegate void OnSelectAllClickedEventHandler();

        public event OnDeSelectAllClickedEventHandler OnDeSelectAllClicked;
        public delegate void OnDeSelectAllClickedEventHandler();               

        public Hyperlink SelectAllHyperlink { get; private set; }

        public Hyperlink DeSelectAllHyperlink { get; private set; }

        public ProfileMainPanel()
        {
            InitializeComponent();
            BuildSelectAllHyperlink();
            BuildDeSelectAllHyperlink();
            IntializeHandlers();
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
            else
            {
                profil.buildRight();
                functionnalityGrid.ItemsSource = profil.defaultListRights;
            }
        }

        public void Fill(Domain.Profil profil)
        {
            profil.active = activeBox.IsChecked.Value;
            profil.name = nameTextBox.Text;
            List<Right> items = new List<Right>(0);
            foreach (object item in functionnalityGrid.Items)
            {
                if (item is Right)
                {                    
                    items.Add((Right)item);
                }
            }
            profil.rightsListChangeHandler.updatedItems = new List<Right>(items);
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

        protected void BuildSelectAllHyperlink()
        {
            Run run1 = new Run("Select All");
            SelectAllHyperlink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "Select All")
            };
            SelectAllHyperlink.RequestNavigate += OnManageSelectAll;
            selectAllTextBlock.Inlines.Add(SelectAllHyperlink);
            selectAllTextBlock.ToolTip = "Select All Right : View and Edit";
        }

        

        protected void BuildDeSelectAllHyperlink()
        {
            Run run1 = new Run("De-Select All");
            DeSelectAllHyperlink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "De-Select All")
            };
            DeSelectAllHyperlink.RequestNavigate += OnManageDeSelectAll;
            deselectAllTextBlock.Inlines.Add(DeSelectAllHyperlink);
            deselectAllTextBlock.ToolTip = "De-Select All Right : View and Edit";
        }


        private void OnManageSelectAll(object sender, RequestNavigateEventArgs e)
        {
            List<Right> items = new List<Right>(0);
            foreach (object item in functionnalityGrid.Items)
            {
                items.Add(((Right)item));
            }
            functionnalityGrid.ItemsSource = items;
        }

        private void OnManageDeSelectAll(object sender, RequestNavigateEventArgs e)
        {
            List<Right> items = new List<Right>(0);
            foreach (object item in functionnalityGrid.Items)
            {
                items.Add(((Right)item));
            }
            functionnalityGrid.ItemsSource = items;
        }

        private void OnGridSelectionchange()
        {
            List<Right> items = new List<Right>(0);
            List<long> ids = new List<long>(0);
            foreach (object item in functionnalityGrid.SelectedItems)
            {
                if (item is Right)
                {
                    items.Add((Right)item);
                    //ids.Add(((Rights)item).id);
                }
            }
        }
    }
}
