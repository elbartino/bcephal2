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
    /// Logique d'interaction pour ReconciliationReport.xaml
    /// </summary>
    public partial class ReconciliationReport_SubTile : Window
    {
        public ReconciliationReport_SubTile()
        {
            InitializeComponent();
        }

        private void desactiv(object sender, EventArgs e)
        {
            Close();
        }
    }
}
