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

namespace Misp.Kernel.Util
{
    /// <summary>
    /// Interaction logic for PasteBcephalDialog.xaml
    /// </summary>
    public partial class PasteBcephalDialog : Window
    {
       public  bool ok = false;
        public PasteBcephalDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.ok = true;
            //DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.ok = false;
           // DialogResult = false;
            this.Close();
        }

        
    }
}
