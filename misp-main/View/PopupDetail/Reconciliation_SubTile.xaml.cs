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

namespace Moriset_Main_final.View.PoPupDetail
{

    public partial class Reconciliation_SubTile : Window
    {
        public Reconciliation_SubTile()
        {
            InitializeComponent();
        }


        public void createBtn()
        {
            Button b = new Button ();
            Label l = new Label()
            {
                Width = 20
            };
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void desactiv(object sender, EventArgs e)
        {
            Close();
        }

    }
}
