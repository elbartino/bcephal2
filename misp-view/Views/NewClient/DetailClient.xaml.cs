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
using System.Windows.Shapes;

namespace misp_view.Views.NewClient
{
    /// <summary>
    /// Logique d'interaction pour DetailClient.xaml
    /// </summary>
    public partial class DetailClient : Grid
    {
        //public ActivateEventHandler Activated;
        public DetailClient()
        {
            InitializeComponent();
        }

        private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void tbLastName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        //private void OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (Activated != null) Activated(this);
        //}

    }
}
