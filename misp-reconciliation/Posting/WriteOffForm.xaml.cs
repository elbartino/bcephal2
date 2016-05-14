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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for WriteOffForm.xaml
    /// </summary>
    public partial class WriteOffForm : Grid
    {
        public WriteOffForm()
        {
            InitializeComponent();
        }

        public void displayAount(decimal balance)
        {
            debitedOrCreditedAccountLabel.Content = balance < 0 ? "Account to credite" : "Account to debite";
            writeOffAmountTextBox.Text = "" + Math.Abs(balance);
        }

    }
}
