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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for CreateDesignDialogBox.xaml
    /// </summary>
    public partial class CreateDesignDialogBox : Window
    {
        public CreateDesignDialogBox()
        {
            InitializeComponent();
        }

       
        private void saveDesign_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancelDesign_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
