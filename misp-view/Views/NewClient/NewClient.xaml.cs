using misp_view.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour NewClient.xaml
    /// </summary>
    public partial class NewClient : UserControl
    {
        public ObservableCollection<Client> client { get; set; }
        public DetailClient detailC;
        //public ActivateEventHandler Activated;
        public bool trow = false;
        static int number;

        public NewClient()
        {
            
            InitializeComponent();
            Client c = new Client();
            client = new ObservableCollection<Client>();
            c.ClientID = "Bobar";
            c.Number = "1248";
            client.Add(c);
            init();

        }

        protected void init()
        {
            //cbMainContact.Items.Refresh();
            cbMainContact.ItemsSource = (ListContacts.listC);
        }




        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {

        }

        

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddContact ac = new AddContact();
            ac.Name = "Contact" + number;
            //ac.Width = 922;
            //ac.DeleteButton.Click += OnDeleteButtonClick;
            spAddContact.Children.Add(ac);
            number++;
        }

        private void cbMainContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}