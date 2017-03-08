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

namespace misp_view.Views.Settlement
{
    /// <summary>
    /// Logique d'interaction pour newSettlement.xaml
    /// </summary>
    public partial class newSettlement : UserControl
    {
        public newSettlement()
        {
            InitializeComponent();
        }
        private void tbPrefRequest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            w.Show();
        }
    }
}
