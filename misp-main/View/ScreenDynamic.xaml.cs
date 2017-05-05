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

namespace Moriset_Main_final.View
{
    /// <summary>
    /// Logique d'interaction pour ScreenDynamic.xaml
    /// </summary>
    public partial class ScreenDynamic : Window
    {
        public ScreenDynamic()
        {
            InitializeComponent();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
