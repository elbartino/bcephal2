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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for RecoWriteOffDialog.xaml
    /// </summary>
    public partial class RecoWriteOffDialog : Window
    {
        public RecoWriteOffDialog()
        {
            InitializeComponent();
            GridBottom.ReconciliateButton.Visibility = Visibility.Collapsed;
            ResetButton.ReconciliateButton.Visibility = Visibility.Collapsed;
        }
    }
}
