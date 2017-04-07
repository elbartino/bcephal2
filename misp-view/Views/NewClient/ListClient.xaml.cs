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
    /// Logique d'interaction pour ListClient.xaml
    /// </summary>
    public partial class ListClient : UserControl
    {
        NewClient nc = new NewClient();
        ObservableCollection<Client> cl;
        

        public ListClient()
        {
            InitializeComponent();
            cl = new ObservableCollection<Client>();
            cl=nc.client;
            this.DataContext = cl;
        }

    }
}
