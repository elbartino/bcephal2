using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using Misp.Sourcing.GridViews;
using System;
using System.Collections;
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

namespace Misp.Reconciliation.RecoGrid
{
    /// <summary>
    /// Interaction logic for RecoDialog.xaml
    /// </summary>
    public partial class RecoDialog : Window
    {
        public ReconciliationGridService ReconciliationGridService { get; set; }
        public Kernel.Domain.ReconciliationContext Context { get; set; }

        public RecoDialog()
        {
            InitializeComponent();
            this.confirmationMessageLabel.Content = "You are about to create a reconciliation posting for the selected items.\nDo you confirm the operation?";
            toolbar.HideButtons();
        }

        public void display(IList items, Grille grille)
        {
            this.grid.buildColumns(grille);
            if(Context == null) Context = this.ReconciliationGridService.ReconciliationContextService.getReconciliationContext();
            
            List<Object[]> rows = new List<object[]>(0);
            foreach(object item in items){
                if (item is GridItem) rows.Add(((GridItem)item).Datas);
            }
            grid.displayRows(rows);
            grid.grid.SelectAll();
            this.toolbar.displayBalance(grid.grid.SelectedItems, Context, grille);
            decimal balance = toolbar.getBalance();
            writeOffForm.Visibility = balance == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            if (balance != 0)
            {
                writeOffForm.displayAount(balance);
                List<String> numbers = new List<String>(0);
                List<Account> accounts = new List<Account>(0);
                int accountNbrposition = grille.GetAccountNbrColumn(Context).position;
                int accountNameposition = grille.GetAccountNameColumn(Context).position;
                foreach (object item in items)
                {
                    if (item is GridItem)
                    {
                        object nbr = ((GridItem)item).Datas[accountNbrposition];
                        object name = ((GridItem)item).Datas[accountNameposition];
                        if (nbr != null && !numbers.Contains(nbr.ToString()))
                        {
                            Account account = new Account();
                            account.number = nbr.ToString();
                            account.name = name.ToString();
                            accounts.Add(account);
                            numbers.Add(nbr.ToString());
                        }
                    }
                }
                writeOffForm.debitedOrCreditedAccountComboBox.ItemsSource = accounts;                
                accounts = this.ReconciliationGridService.PostingService.getWriteOffAccounts();
                writeOffForm.writeOffAccountComboBox.ItemsSource = accounts;
                if (accounts.Count == 1) writeOffForm.writeOffAccountComboBox.SelectedIndex = 0;
            }
        }

        public bool validateEdition()
        {
            decimal balance = toolbar.getBalance();
            if (balance != 0)
            {
                if (writeOffForm.writeOffAccountComboBox.SelectedItem == null)
                {
                    MessageDisplayer.DisplayWarning("Reconciliation write-off", "The write-off account can't be empty!");
                    return false;
                }
                if (writeOffForm.debitedOrCreditedAccountComboBox.SelectedItem == null)
                {
                    String message = balance < 0 ? "The account to credite can't be empty!" : "The account to debite can't be empty!";
                    MessageDisplayer.DisplayWarning("Reconciliation write-off", message);
                    return false;
                }
            }
            return true;
        }

        public String getWriteOffDC()
        {
            decimal balance = toolbar.getBalance();
            return balance < 0 ? "D" : "C";
        }

        public decimal getWriteOffAmount()
        {
            return Math.Abs(toolbar.getBalance());
        }

        public Account getWriteOffAccount()
        {
            return (Account)writeOffForm.writeOffAccountComboBox.SelectedItem;
        }

        public Account getDebitedOrCreditedAccount()
        {
            return (Account)writeOffForm.debitedOrCreditedAccountComboBox.SelectedItem;
        }


    }
}
