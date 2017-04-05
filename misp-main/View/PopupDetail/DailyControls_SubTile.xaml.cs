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

namespace Moriset_Main_final.View.PopupDetail
{
    /// <summary>
    /// Logique d'interaction pour DailyControls_SubTile.xaml
    /// </summary>
    public partial class DailyControls_SubTile : Window
    {
        public DailyControls_SubTile()
        {
            InitializeComponent();
        }

        private void deactiv(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
