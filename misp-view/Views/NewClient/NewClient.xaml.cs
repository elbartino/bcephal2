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
    /// Logique d'interaction pour NewClient.xaml
    /// </summary>
    public partial class NewClient : UserControl
    {

        public DetailClient detailC;
        //public ActivateEventHandler Activated;
        public Misp.Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Misp.Kernel.Ui.Base.ChangeItemEventHandler Added;
        public bool trow = false;
        static int number;

        public NewClient()
        {
            InitializeComponent();
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