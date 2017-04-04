using misp_view.Views;
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

namespace misp_view.Views.NewClient
{
    /// <summary>
    /// Logique d'interaction pour AddContact.xaml
    /// </summary>
    public partial class AddContact : Grid
    {

        public Misp.Kernel.Ui.Base.ChangeItemEventHandler Added;
        public AddContact()
        {
            InitializeComponent();
            HideDetailsView();
            init();
        }

        protected void init()
        {
            // this.AddButton.Click += OnAddButtonClick;
            //this.DeleteButton.Click += OnDeleteButtonClick;
            this.ShowDetailsButton.Click += OnShowDetails;
            this.HideDetailsButton.Click += OnHideDetails;
            //this.DClient.Activated += OnActivate;
            DClient.btnAdd.Click += updateText;

        }

        private void updateText(object sender, RoutedEventArgs e)
        {

            
            if (DClient.tbLastName.Text != "" || DClient.tbLastName.Text != "")
            {
                tbNameClient.Text = DClient.tbFirstName.Text;
                tbLastNameClient.Text = DClient.tbLastName.Text;
                string s = DClient.tbFirstName.Text + ", " + DClient.tbLastName.Text;
                ListContacts.Record(s);
                //ListContacts.Display();
                DClient.btnAdd.Visibility = Visibility.Hidden;
            }
        }


        private void OnHideDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView();
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(false);
        }

        private void HideDetailsView(bool hideDetails = true)
        {
            DClient.Visibility = hideDetails ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.HideDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            this.ShowDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        
        public void DeleteButton_Click(object sender, RoutedEventArgs e)
        {      
            gridMain.Children.RemoveRange(0,10);
            string s = DClient.tbFirstName.Text + ", " + DClient.tbLastName.Text;
            ListContacts.Remove(s);
        }

        public void gridMain_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
